using UnityEngine;
using System.Collections;

public class DataUrl  {

   
    

    public static string GetFilePersistentUrl(string path)
    {
         return Application.persistentDataPath+"/" + path;
    }

    public static string GetFileStreamingUrl(string path)
    {
      //  return Application.streamingAssetsPath+"/" + path;
        if (!Application.isMobilePlatform)
        {
            return Application.streamingAssetsPath + "/" + path;
         //   return "file://" + Application.dataPath + "/StreamingAssets/"+path;
        }
        else
        {
            return
#if UNITY_ANDROID
        Application.streamingAssetsPath + "/"+path;
#else
      "file://" + Application.dataPath + "/StreamingAssets" + "/"+path;
#endif
        }
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    public static readonly string LOCAL_URL_PREFIX = "file:///";
#else
    public static readonly string LOCAL_URL_PREFIX="file://";
#endif


    /// <summary>
    /// HotFix.lua 文件读取路径(pc读取streaming，移动端读取persistentDataPath)
    /// </summary>
    public static  string HOTFIX_URL
    {
        get
        {
            if (GameManager.Instance.mIsHotUpdate && GameManager.Instance.mIsLuaUpdate)
            {
                return Application.persistentDataPath + "/";
            }
            else
            {
                return "Assets/Resources/LuaScripts/";
            }
        }
    }

    public static readonly string STREAMINGASSETS_URL =
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.persistentDataPath + "/";
#elif UNITY_ANDROID && UNITY_EDITOR || UNITY_STANDALONE_WIN && UNITY_EDITOR || UNITY_IPHONE && UNITY_EDITOR
        "Assets/StreamingAssets/";
#elif UNITY_IPHONE && !UNITY_EDITOR
        "file://" + Application.dataPath + "/Raw/";
#elif UNITY_IPHONE && UNITY_EDITOR || UNITY_STANDALONE_OSX
        "file://" + Application.dataPath + "/StreamingAssets/";
#else
        string.Empty;
#endif
}
