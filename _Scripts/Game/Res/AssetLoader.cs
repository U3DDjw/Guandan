using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using MsgContainer;
using System.Text;

class BundleInfo
{
    public string bundleName;
    public string assetName;
    public List<string> dependencies;
    public void AddDependence(string dep)
    {
        if (dependencies == null)
        {
            dependencies = new List<string>();
        }
        dependencies.Add(dep);
    }
}
public class AssetLoader : IResourceLoader
{

    private Dictionary<string, BundleInfo> fileMap = new Dictionary<string, BundleInfo>();

    private Dictionary<string, AssetBundle> depsDic = new Dictionary<string, AssetBundle>();//  已经加载过的依赖关系的AssetBundle

    public Dictionary<string, AssetBundle> loadedBundleDic = new Dictionary<string, AssetBundle>();//已经加载过的file的Bundle       

    public AssetLoader()
    {
        Debug.Log("AssetLoader....................实例化成功");
        //  InitFileMap();
        // LoadAtlas();
    }


    //void LoadAtlas()
    //{
    //    string[] atlsPaths = { "MainAtlas.unity3d", "PlayingAtlas.unity3d", "cardAtlas.unity3d" };
    //    for (int i = 0; i < atlsPaths.Length; i++)
    //    {
    //        AssetBundle asset = null;
    //        string path = atlsPaths[i];
    //        //bool isPersistentDataPath = System.IO.File.Exists(path);
    //        //if (!isPersistentDataPath)
    //        //{
    //        //    path = DataUrl.GetFileStreamingUrl(info.bundleName);
    //        //    if (!System.IO.File.Exists(path))
    //        //        AssetBundle asset = null;
    //        loadedBundleDic.TryGetValue(path, out asset);
    //        if (asset == null)
    //        {
    //            path =DataUrl.GetFilePersistentUrl(atlsPaths[i]);
    //            asset = AssetBundle.LoadFromFile(path);
    //            if(asset == null)
    //            {
    //                path = DataUrl.GetFileStreamingUrl(atlsPaths[i]);
    //                asset = AssetBundle.LoadFromFile(path);
    //                if(asset == null)
    //                {

    //                }
    //                else
    //                {
    //                    loadedBundleDic.Add(atlsPaths[i], asset);
    //                }
    //            }
    //            else
    //            {
    //                loadedBundleDic.Add(atlsPaths[i], asset);
    //            }
    //        }
    //        else
    //        {
    //            loadedBundleDic.Add(atlsPaths[i], asset);
    //        }
    //    }
    //}
    /// <summary>
    /// StreamingAssets.xml 初始化
    /// </summary>
    void InitFileMap()
    {
        string fileName = GlobalData.mStreamingAssetsXml;
        string path = DataUrl.GetFilePersistentUrl(fileName);
        bool isPersistentPath = System.IO.File.Exists(path);
        Debug.Log("isPersistentPath:" + isPersistentPath.ToString());
        if (!isPersistentPath)
        {
            path = DataUrl.GetFileStreamingUrl(fileName);
            //安卓读取该目录有错误，所以，只能读取Per目录中的数据。
            if (!System.IO.File.Exists(path))
            {
                //Debug.LogError(fileName + "文件加载失败...");
                return;
            }
        }

        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        Debug.Log(fileName + doc.Name);
        XmlNodeList nodeList = doc.SelectSingleNode("files").ChildNodes;
        foreach (XmlElement xe in nodeList)
        {
            BundleInfo info = new BundleInfo();
            string _fileName = xe.SelectSingleNode("fileName").InnerText.Replace("\\", "/");
            info.assetName = xe.SelectSingleNode("assetName").InnerText.Replace("\\", "/");
            info.bundleName = xe.SelectSingleNode("bundleName").InnerText.Replace("\\", "/");

            XmlNode deps = xe.SelectSingleNode("deps");
            if (null != deps)
            {
                XmlNodeList depList = deps.ChildNodes;
                foreach (XmlElement _xe in depList)
                {
                    info.AddDependence(_xe.InnerText.Replace("\\", "/"));
                }
            }
            string[] strs = _fileName.Substring("Assets/Resources/".Length).Split('.');
            string strValue = strs[0];
            fileMap.Add(strValue, info);


        }

    }

    public override T LoadAsset<T>(string fileName)
    {
        string[] files = fileName.Split('/');
        fileName = files[files.Length - 1];

        BundleInfo info = null;
        fileMap.TryGetValue(fileName, out info);
        if (null == info)
        {
            return null;
        }

        string path = DataUrl.GetFilePersistentUrl(info.bundleName);
        bool isPersistentDataPath = System.IO.File.Exists(path);
        if (!isPersistentDataPath)
        {
            path = DataUrl.GetFileStreamingUrl(info.bundleName);
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
        }

        AssetBundle asset = null;
        loadedBundleDic.TryGetValue(path, out asset);
        if (asset == null)
        {
            asset = AssetBundle.LoadFromFile(path);
            loadedBundleDic.Add(path, asset);
        }

        if (asset == null)
        {
            return null;
        }

        if (null != info.dependencies)
        {
            foreach (string dep in info.dependencies)
            {
                AssetBundle _asset = null;
                string dependPath = isPersistentDataPath ? DataUrl.GetFilePersistentUrl(dep) : DataUrl.GetFileStreamingUrl(dep);
                if (IsHadLoadDeps(dependPath))
                {
                    continue;
                }
                _asset = AssetBundle.LoadFromFile(dependPath);
                depsDic.Add(dependPath, _asset);
            }
        }

        return asset.LoadAsset<T>(info.assetName);

    }

    public AssetBundle GetAtlasBundleByName(string name)
    {
        AssetBundle b;
        loadedBundleDic.TryGetValue(name, out b);
        return b;
    }

    bool IsHadLoadDeps(string url)
    {
        if (depsDic.ContainsKey(url))
        {
            return true;
        }
        return false;
    }


    //暂时不采用异步加载，
    public override T LoadAssetAsync<T>(string fileName)
    {
        //这边不知道，在路径错误的情况下isDone是啥样的，
        //可以判断request.assetBundle为空的情况下，就继续异步加载.(暂时先不弄，没需求)
        string path = DataUrl.GetFilePersistentUrl(fileName);
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        if (request.isDone)
        {
            //这边异步加载Streaming目录即可。
            return request.assetBundle.LoadAsset<T>(fileName);
        }
        return null;
    }

    public override string LoadText(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset == null)
        {
            Debug.LogError(fileName + "不存在文本文件");
            return null;
        }
        string text = Encoding.UTF8.GetString(textAsset.bytes, 0, textAsset.bytes.Length);
        return textAsset.text;

    }


}
