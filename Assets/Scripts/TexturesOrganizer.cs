using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextureComparator))]
public class TexturesOrganizer : MonoBehaviour{

    [SerializeField]
    public TexturesSet[] textures;

    [Serializable]
    public class TexturesSet{
        public Texture2D originalTexture;
        public Texture2D changedTexture;
        public Texture2D mask;
    }

    private TextureComparator textureComparator;
    private int currentTexturesSetIndex = 0;

    private void Awake(){
        textureComparator = GetComponent<TextureComparator>();
    }

    void Start(){
        if(textures.Length > 0) {
            setTexturesToTextureComparator();
        }
    }

    private void setTexturesToTextureComparator(){
        textureComparator.originalTexture = textures[currentTexturesSetIndex].originalTexture;
        textureComparator.changedTexture = textures[currentTexturesSetIndex].changedTexture;
        textureComparator.maskTexture = textures[currentTexturesSetIndex].mask;
    }

    public void previousTextures(){
        if(currentTexturesSetIndex > 0) {
            currentTexturesSetIndex--;
            setTexturesToTextureComparator();
        }
    }

    public void nextTextures(){
        if(currentTexturesSetIndex < textures.Length - 1) {
            currentTexturesSetIndex++;
            setTexturesToTextureComparator();
        }
    }

    public int getCurrentTexturesSetIndex(){
        return currentTexturesSetIndex;
    }

    public int getTexturesSetsCount(){
        return textures.Length;
    }

}
