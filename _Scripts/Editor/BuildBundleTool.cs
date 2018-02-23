using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using MsgContainer;
using UnityEngine.SceneManagement;


public class BuildBundleTool : Editor {

    static string EXPORT_OUT_PATH = Application.streamingAssetsPath+ Path.DirectorySeparatorChar;
    [MenuItem("Bundle/Create Version")]
    public static void CreateVersionFile()
    {
        string resPath = EXPORT_OUT_PATH;
        StringBuilder version = new StringBuilder();

        DjwHelper.ForeachHandle(Application.dataPath + "/StreamingAssets", new List<string> { "ab","txt","lua" }, (string filename) =>
             {
                 string baseName = filename.Replace(Application.streamingAssetsPath + Path.DirectorySeparatorChar, "").Replace(@"\",@"/");
                 string md5 = MD5File(filename);
                 FileInfo info = new FileInfo(filename);
                 string size = System.Math.Ceiling(info.Length / 1024.0) + "KB";
              
               //  Debug.Log(System.Math.Ceiling(info.Length / 1024.0) + "KB");
                 version.Append(baseName).Append(",").Append(md5).Append(",").Append(size).Append("\n");
             });

        //Manifest md5
        string streamingAssetManifest = Application.dataPath + "/StreamingAssets/StreamingAssets";
        string streamingAssetManifestbaseName = Path.GetFileName(streamingAssetManifest);
        string streamingAssetManifestmd5 = MD5File(streamingAssetManifest);
        version.Append(streamingAssetManifestbaseName).Append(",").Append(streamingAssetManifestmd5).Append("\n");

        //生成配置文件
        FileStream stream = new FileStream(resPath + "version.ver", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(version.ToString());
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
        Debug.Log(" 版本文件 " + resPath+"version.ver:" + version);
    }
    [MenuItem("Bundle/清空热更目录")]
    public static void ClearPersistentDataPath()
    {
        System.IO.Directory.Delete(Application.persistentDataPath, true);
        System.IO.Directory.CreateDirectory(Application.persistentDataPath);
    }
    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<retVal.Length;i++)
            {
                sb.Append(retVal[i].ToString());
            }
            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("md5file() fail,error:" + ex.Message);
        }
    }
    static int _currentScence = 0;

    [MenuItem("OpenScene/Login")]
    public static void OpenLogin()
    {
        OpenScene(0);
    }
    [MenuItem("OpenScene/Main")]
    public static void OpenMain()
    {
        OpenScene(1);
    }
    [MenuItem("OpenScene/Transition")]
    public static void OpenTransition()
    {
        OpenScene(2);
    }
    [MenuItem("OpenScene/TestScene")]

    public static void OpenTestScene()
    {
        OpenScene(3);
    }
    static void OpenScene(int i)
    {
        if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
        {
            EditorApplication.OpenScene(EditorBuildSettings.scenes[i].path);
        }
    }

    [MenuItem("Tools/清空玩家缓存")]
    static void ClearPlayerSetting()
    {
        //  XPlayerSettings.Instance.ClearAllInfo();
        XPlayerPrefs.Instance.ClearAllInfo();
    }

    //[MenuItem("Bundle/Build Lua")]
    //static void ConverLuaToTxt()
    //{
    //    string luaPath = Application.dataPath + "/Resources/LuaScripts";
    //    string[] names = Directory.GetFiles(luaPath);
    //    foreach (string filename in names)
    //    {
    //        if (!filename.EndsWith("lua")) { continue; }
    //        string newFile = Path.ChangeExtension(filename, "txt");
    //        File.Move(filename, newFile);
    //    }
    //   names = Directory.GetFiles(luaPath);
    //    foreach(string name in names)
    //    {
    //        if(name.EndsWith(".lua.txt"))
    //        {
    //            File.Delete(name);
    //        }
    //    }
    //}
    //[MenuItem("Bundle/Reset Lua")]
    //static void ConverTxtToLua()
    //{
    //    string luaPath = Application.dataPath + "/Resources/LuaScripts";
    //    string[] names = Directory.GetFiles(luaPath);
    //    foreach (string filename in names)
    //    {
    //        if (!filename.EndsWith("txt")) { continue; }
    //        FileInfo fi = new FileInfo(filename);
    //        fi.MoveTo(Path.ChangeExtension(filename, "lua"));
    //    }

    //   names = Directory.GetFiles(luaPath);
    //    foreach(string name in names)
    //    {
    //       if(name.EndsWith(".txt.lua"))
    //        {
    //            File.Delete(name);
    //        }
    //    }
    //}



   /// <summary>
   /// 打包成一个ab的目录（Lua脚本全部打成一个ab）
   /// </summary>
    static List<string> buildOnePath = new List<string> { "LuaScripts", "OtherRes" };
    [MenuItem("Bundle/Build All Bundle")]
    static void CreateAssetBundles()
    {
        Debug.Log("开始清空StreamingAssets");
        //DjwHelper.ForeachHandle(Application.dataPath + "/StreamingAssets", new List<string> { "ab", "manifest","ver" }, (string filename) =>
        //{
        //    string assetPath = filename.Replace(Application.dataPath, "Assets");
        //    AssetDatabase.DeleteAsset(assetPath);
        //});
        System.IO.Directory.Delete(Application.streamingAssetsPath, true);
        System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
        Debug.Log("StreamingAssets清空完毕...");

        string[] paths = new string[] { "Atlas", "View", "font", "TexturePic", "Audio", "LuaScripts", "Animation", "OtherRes" };


        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
        foreach (string path in paths)
        {
            string basePath = Application.dataPath + "/Resources/" + path;
            {
                if (buildOnePath.Contains(path))
                {
                    List<string> list = new List<string>();
                    DjwHelper.ForeachHandle(basePath, null, (string filename) =>
                    {
                        string assetPath = filename.Replace(Application.dataPath, "Assets");
                        list.Add(assetPath);
                    });
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = path + ".ab";
                    build.assetNames = list.ToArray();
                    buildMap.Add(build);
                }
                else
                {
                    DjwHelper.ForeachHandle(basePath, null, (string filename) =>
                    {
                        string assetPath = filename.Replace(Application.dataPath, "Assets");

                        string baseName = assetPath.Substring(17);
                        AssetBundleBuild build = new AssetBundleBuild();
                        build.assetBundleName = baseName.Split('.')[0] + ".ab"; //去掉后缀名
                    build.assetNames = new string[] { assetPath };

                        buildMap.Add(build);
                    });
                }
            }
        }
       
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, buildMap.ToArray(), BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.DisableWriteTypeTree |
            BuildAssetBundleOptions.ChunkBasedCompression, GetCurTarget());

        AssetDatabase.Refresh();
       // ConverTxtToLua();
    }


    static BuildTarget GetCurTarget()
    {
#if UNITY_ANDROID
        return BuildTarget.Android;
#elif UNITY_IPHONE
		return BuildTarget.iOS;
#else
         return BuildTarget.Android;
#endif
    }
}
