using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingleTon<DataManager>
{
    #region Texture 

    public List<int> mTextureKeyNameList = new List<int>();
    private Dictionary<int, Texture> wwwTexture = new Dictionary<int, Texture>();
    public Dictionary<int, Texture> mWWWTexture
    {
        get
        {
            return wwwTexture;
        }

        set
        {
            wwwTexture = value;
        }
    }
    private List<bool> isCanClicks = new List<bool>();
    public List<bool> IsCanClicks
    {
        get { return isCanClicks; }
    }
    public void AddTexture(int keyName, Texture texture)
    {
        if (!mWWWTexture.ContainsKey(keyName))
        {
            mWWWTexture.Add(keyName, texture);
        }
        else
        {
            mWWWTexture[keyName] = texture;
        }
    }

    public void InitData()
    {
        InitTexture();
    }

    void InitTexture()
    {
        DataManager.Instance.mTextureKeyNameList.Add((int)ETextureName.EHeadTex1);
        DataManager.Instance.mTextureKeyNameList.Add((int)ETextureName.EDatingNotic);
        DataManager.Instance.mTextureKeyNameList.Add((int)ETextureName.ENotic);
    }


    private List<Texture> daTingNoticTextureList = new List<Texture>();
    public List<Texture> mDaTingNoticTextureList
    {
        get { return daTingNoticTextureList; }
    }
    public delegate void DelLoadTexEnd(Texture tex);
    //加载大厅图片专用
    public IEnumerator LoadDaTingTexture(string url, DelLoadTexEnd endFun)
    {
        WWW www = new WWW(url);
        yield return www;
#if UNITY_IPHONE || UNITY_STANDALONE_WIN
        if (www.error == "" || www.error == null)
        {
             endFun(www.texture);
        }
#elif UNITY_ANDROID
        if (www.error == null || www.error == "")
        {
            endFun(www.texture);
        }
#endif
        else
        {
            Debug.LogError("error:" + www.error);
        }
        www.Dispose();
    }
    


    public IEnumerator LoadEachTexture(string url, HandleFinishDownload finishFunc)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null || www.error == "")
        {
            finishFunc(www);
        }
        else
            LoadEachTexture(url, finishFunc);
            {
             //   Debug.LogError("url"+url+"error:" + www.error);
            }
            www.Dispose();
    }
    public Texture GetTextureByName(ETextureName name)
    {
        if(mWWWTexture.ContainsKey((int)name))
        {
            return mWWWTexture[(int)name];
        }
        return null;
    }

    ///// <summary>
    ///// 其他玩家头像集合
    ///// </summary>
    //private Dictionary<string, Texture> otherPlayerHead = new Dictionary<string, Texture>();
    //public Dictionary<string,Texture> mOtherPlayerHead
    //{
    //    get
    //    {
    //        return otherPlayerHead;
    //    }
    //}

    //public void UpdateOtherPlayerHead(string keyName,Texture texture)
    //{
    //   if(mOtherPlayerHead.ContainsKey(keyName))
    //    {
    //        mOtherPlayerHead[keyName] = texture;
    //    }
    //    else
    //    {
    //        mOtherPlayerHead.Add(keyName, texture);
    //    }
    //}

    //public Texture GetOtherPlayerHead(string keyName)
    //{
    //    if (mOtherPlayerHead.ContainsKey(keyName))
    //    {
    //        return mOtherPlayerHead[keyName];
    //    }
        
    //    return null;
    //}

   
    #endregion

  
}
