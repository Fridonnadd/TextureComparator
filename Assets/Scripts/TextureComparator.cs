using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureComparator : MonoBehaviour {
    public ComputeShader shader;
    public Texture originalTexture;
    public Texture changedTexture;
    public Texture maskTexture;
    public int maxShaderThreads = 256;
    public float MaskLimit { get; set; } = 128;

    [HideInInspector]
    public RenderTexture resultTexture = null;
    [HideInInspector]
    public float comparingResult = 0;

    private static readonly int COMPARING_DATA_INSTANCE_SIZE = 9 * 4; //(8 integers and 1 float)
    struct ComparingData{
        public int texHeight;
        public int texWidth;
        public float maskLimit;
        public int equalPixels;
        public int maskedPixels;
        public int maxAreaHeight;
        public int maxAreaWidth;
        public int areaXIndex;
        public int areaYIndex;
    }
    private ComputeBuffer computeBuffer;
    private ComparingData[] comparingDataArray;
    private int shaderHandler;

    private void Awake(){
        initShader();
    }

    void Start(){
        compare();
    }

    private void OnDestroy(){
        resultTexture.Release();
        computeBuffer.Release();
    }

    private void initShader(){
        computeBuffer = new ComputeBuffer(1, COMPARING_DATA_INSTANCE_SIZE);
        comparingDataArray = new ComparingData[] { new ComparingData() };
        shaderHandler = shader.FindKernel("CSMain");
    }

    public void compare(){

        bool textureIsNotNull = new[] { originalTexture, changedTexture, maskTexture }.All(x => x != null);
        if (!textureIsNotNull) {
            comparingResult = -1;
            return;
        }

        bool heightsEqual = new[] { originalTexture.height, changedTexture.height, maskTexture.height }.Distinct().Count() == 1;
        bool widthsEqual = new[] { originalTexture.width, changedTexture.width, maskTexture.width }.Distinct().Count() == 1;      
        if(!heightsEqual || !widthsEqual) {
            comparingResult = -1;
            return;
        }

        if (resultTexture != null) {
            resultTexture.Release();
            resultTexture = null;
        }
        resultTexture = new RenderTexture(originalTexture.width, originalTexture.height, 32);
        resultTexture.enableRandomWrite = true;
        resultTexture.Create();
        shader.SetTexture(shaderHandler, "OriginalTexture", originalTexture);
        shader.SetTexture(shaderHandler, "ChangedTexture", changedTexture);
        shader.SetTexture(shaderHandler, "MaskTexture", maskTexture);
        shader.SetTexture(shaderHandler, "ResultTexture", resultTexture);
        shader.SetBuffer(shaderHandler, "ComparingDataBuffer", computeBuffer);

        comparingResult = 0;
        int calculatedAreas = 0;
        for (int areaXIndex = 0; areaXIndex <= (originalTexture.width - 1)/ maxShaderThreads; areaXIndex++) {
            for(int areaYIndex = 0; areaYIndex <= (originalTexture.height - 1) / maxShaderThreads; areaYIndex++) {

                comparingDataArray[0].maskLimit = MaskLimit/255.0f;
                comparingDataArray[0].texHeight = originalTexture.height;
                comparingDataArray[0].texWidth = originalTexture.width;
                comparingDataArray[0].maskedPixels = 0;
                comparingDataArray[0].equalPixels = 0;
                comparingDataArray[0].maxAreaHeight = maxShaderThreads;
                comparingDataArray[0].maxAreaWidth = maxShaderThreads;
                comparingDataArray[0].areaXIndex = areaXIndex;
                comparingDataArray[0].areaYIndex = areaYIndex;

                computeBuffer.SetData(comparingDataArray);

                shader.Dispatch(shaderHandler, maxShaderThreads, maxShaderThreads, 1);

                computeBuffer.GetData(comparingDataArray);
                if (comparingDataArray[0].maskedPixels > 0) {
                    comparingResult += comparingDataArray[0].equalPixels / (float)comparingDataArray[0].maskedPixels;
                    calculatedAreas++;
                }
            }
        }
        comparingResult = calculatedAreas > 0 ? comparingResult /= calculatedAreas : -1;
    }

}