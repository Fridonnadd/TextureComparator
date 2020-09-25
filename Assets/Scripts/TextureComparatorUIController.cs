using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextureComparator))]
[RequireComponent(typeof(TexturesOrganizer))]
public class TextureComparatorUIController : MonoBehaviour{

    private static readonly string EMPTY_TEXTURES_SETS_ARRAY_MESSAGE = "ERROR (Empty textures sets array, add few to TexturesOrganizer)";
    private static readonly string EMPTY_TEXTURES_SETS_COUNTER_VALUE = "0/0";
    private static readonly string TEXTURES_DIMENSIONS_ERROR_MESSAGE = "ERROR (Different/zero-dimentional textures or null)";
    private static readonly string RESULT_MESSAGE_FORMAT = "{0:P1}";
    private static readonly string TEXTURES_SETS_COUNTER_FORMAT = "{0}/{1}";

    public RawImage originalImage;
    public RawImage changedImage;
    public RawImage maskImage;
    public RawImage resultImage;
    public Text resultMessageText;
    public Text texturesSetsCounterText;
    public Text maskLimitSliderValueText;

    private TextureComparator textureComparator;
    private TexturesOrganizer texturesOrganizer;

    private void Awake(){
        textureComparator = GetComponent<TextureComparator>();
        texturesOrganizer = GetComponent<TexturesOrganizer>();
    }

    void Start(){
        updateUI();
    }

    public void updateUI(){
        originalImage.texture = textureComparator.originalTexture;
        changedImage.texture = textureComparator.changedTexture;
        maskImage.texture = textureComparator.maskTexture;
        resultImage.texture = textureComparator.resultTexture;
        maskLimitSliderValueText.text = textureComparator.MaskLimit.ToString();
        if (texturesOrganizer.getTexturesSetsCount() > 0) {
            string resultTextString = textureComparator.comparingResult < 0 ? TEXTURES_DIMENSIONS_ERROR_MESSAGE :
                string.Format(RESULT_MESSAGE_FORMAT, textureComparator.comparingResult);
            string counterString = string.Format(TEXTURES_SETS_COUNTER_FORMAT, texturesOrganizer.getCurrentTexturesSetIndex() + 1, texturesOrganizer.getTexturesSetsCount());
            updateUIText(resultTextString, counterString);
        } else {
            updateUIText(EMPTY_TEXTURES_SETS_ARRAY_MESSAGE, EMPTY_TEXTURES_SETS_COUNTER_VALUE);
        }
    }

    private void updateUIText(string resultMessage, string texturesCount){
        resultMessageText.text = resultMessage;
        texturesSetsCounterText.text = texturesCount;
    }
}
