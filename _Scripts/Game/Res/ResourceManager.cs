using UnityEngine;
using System.Collections;
using Common;
using MsgContainer;
using System.Collections.Generic;

public class ResourceManager :SingleTon<ResourceManager>{

    private ResourceLoader m_resourceLoader = new ResourceLoader();

    public ResourceManager()
    {
    }
    public T LoadAsset<T>(string fileName) where T:UnityEngine.Object
    {
        T r = LoadBundleController.GetInstance().LoadAsset<T>(fileName);
        if (null != r)
        {
            return r;
        }
        return LoadBundleController.GetInstance().LoadLocalAsset<T>(fileName);
    }

    public TextAsset LoadLuaAB(string fileName)
    {
        TextAsset t = LoadBundleController.GetInstance().LoadLuaAB(fileName);
        return t;
    }

    public T LoadAssetAsync<T>(string fileName) where T:UnityEngine.Object
    {
        T rtn = m_resourceLoader.LoadAssetAsync<T>(fileName);
        if(null!=rtn)
        {
            return rtn;
        }
    
        return m_resourceLoader.LoadAssetAsync<T>(fileName);
    }
   
   


    //Atlas Manager 
    #region Atlas Manager
    string GetAtlsPath(int type)
    {
        switch((EAtlasType)type)
        {
            case EAtlasType.ECard:
                return "Atlas/cardAtlas";
            case EAtlasType.EMain:
                return "Atlas/MainAtlas";
            case EAtlasType.EPlaying:
                return "Atlas/PlayingAtlas";
        }
        return "Atlas/MainAtlas";
    }
    public Sprite GetSpriteByName(int type, string assetName)
    {
        Sprite[] sprites = LoadBundleController.GetInstance().GetSpritesByName(GetAtlsPath(type), assetName);
        //Sprite[] sprites = GetSpritesByName(GetAtlsPath(type), assetName);
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == assetName)
            {
                return sprites[i];
            }
        }
        return null;
    }
 
    #endregion
}
