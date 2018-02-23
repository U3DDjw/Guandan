using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadBundleController : MonoBehaviour
{

    //=====分为initBundle和mainBundle，initBundle,otherBundle
    //======mainBundle通过在LoadNextScene中加载
    //otherBundle 是不进行初始Load，用时Load
    private string[] bundleNames;

    /// <summary>
    /// 初始化Bundle(普通View中login,common)+其他所有除View中资源
    /// </summary>
    private List<string> initBundleNames;
    /// <summary>
    /// MainBundle(普通view除login，common)
    /// </summary>
    private List<string> mainBundleNames;
    /// <summary>
    /// Bundle依赖关系列表
    /// </summary>
    AssetBundleManifest mainfest;
    /// <summary>
    /// 已经Load过的 bundle
    /// </summary>
    private Dictionary<string, AssetBundle> loadedBundles;

    private static LoadBundleController _instance;

    private LoadBundleController()
    {
        loadedBundles = new Dictionary<string, AssetBundle>();
        spritesCache = new Dictionary<string, Sprite[]>();
        initBundleNames = new List<string>();
        mainBundleNames = new List<string>();
    }

    public static LoadBundleController GetInstance()
    {
        if (_instance == null)
        {
            GameObject obj = new GameObject();
            DontDestroyOnLoad(obj);
            obj.name = "LoadBundleController";
            _instance = obj.AddComponent<LoadBundleController>();
        }
        return _instance;
    }

    IEnumerator ReadBundleManifest()
    {
        var abAsy= AssetBundle.LoadFromFileAsync(DataUrl.GetFilePersistentUrl("StreamingAssets"));
        yield return abAsy;
        if(abAsy.assetBundle!=null)
        {
            mainfest = abAsy.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            bundleNames = mainfest.GetAllAssetBundles();
            SplitInitOrMainBundle();
            LoadInitBundles(initBundleNames.ToArray(), () =>
            {
                Debug.Log("Bundle加载完成...bundles Count:" + loadedBundles.Count);
                XLuaInstance.Instance.HotStartFixing();
                GameManager.Instance.SetRenderTexture();
                GameManager.Instance.SwitchGameStatus(EGameStatus.ELogin);
            }, (a) => Debug.Log("进度:" + a.ToString()));
        }
    }

    #region splitBundleName

    public int TotalMainBundleCount()
    {
        return mainBundleNames.Count;
    }
    public bool IsContainesMainBundle()
    {
        return mainBundleNames.Count > 0;
    }
    /// <summary>
    /// Manifest 中是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool IsManifestContains(string name)
    {
     
        if (bundleNames == null || bundleNames.Length < 1) { return false; }
        for (int i = 0; i < bundleNames.Length; i++)
        {
            if (bundleNames[i] == name)
            {
                return true;
            }
        }
        return false;
    }


    void GetDependiceBundle(string name, List<string> dependiceList)
    {
        var list = mainfest.GetDirectDependencies(name);
        for (int i = 0; i < list.Length; i++)
        {
            if (!dependiceList.Contains(list[i]))
            {
                dependiceList.Add(list[i]);
            }
        }
    }

    void UpdateBundles(string[] resNames, List<string> bundles)
    {
        for (int i = 0; i < resNames.Length; i++)
        {
            bundles.Add(resNames[i]);
            GetDependiceBundle(resNames[i], bundles);
        }
    }
    void SplitInitOrMainBundle()
    {
        string[] _initResName = { "view/commonview.ab", "view/loginview.ab", "view/loadnextsceneview.ab", "otherres.ab" };
        string[] _mainResName = { "view/homeview.ab", "view/playinggameview.ab", "view/gameinfoview.ab" ,
            "view/createroomview.ab","view/joinroomview.ab","view/waitgameview.ab"};
        List<string> _initBundleNames = new List<string>();
        List<string> _mainBundleNames = new List<string>();
        UpdateBundles(_initResName, initBundleNames);
        UpdateBundles(_mainResName, mainBundleNames);
        for (int i = 0; i < bundleNames.Length; i++)
        {
            string name = bundleNames[i];
            if (_initBundleNames.Contains(name))
            {
                initBundleNames.Add(name);
            }
            else if (_mainBundleNames.Contains(name))
            {
                mainBundleNames.Add(name);
            }
        }
    }

    #endregion
    private void Start()
    {
        if (!GameManager.Instance.mIsHotUpdate)
        {
            return;
        }
        UpdateBundleManifest();
    }

     void UpdateBundleManifest()
    {
        StartCoroutine(ReadBundleManifest());
    }

    public void LoadInitBundles(string[] bundle_names, UnityAction callFunc, UnityAction<int> progressFunc = null)
    {
        if (!GameManager.Instance.mIsHotUpdate)
        {
            callFunc.Invoke();
        }
        else
        {
            StartCoroutine(Load(bundle_names, callFunc, progressFunc));
        }
    }


    public void LoadMainBundles(UnityAction callFunc, UnityAction<int> progressFunc = null)
    {
        StartCoroutine(Load(mainBundleNames.ToArray(), callFunc, progressFunc));
    }
    IEnumerator Load(string[] bundle_names, UnityAction callFunc, UnityAction<int> progressFunc = null)
    {
        int len = bundle_names.Length;

        for (int i = 0; i < len; i++)
        {
            string name = bundle_names[i];
            if (!loadedBundles.ContainsKey(name))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(DataUrl.GetFilePersistentUrl(name));
                loadedBundles.Add(name, ab);
                yield return null;
                if (progressFunc != null)
                    progressFunc.Invoke(i + 1);
            }

            if (i == len - 1)
            {
                callFunc();
            }
        }
    }

    void AddLoadedBundle(string name,AssetBundle ab)
    {
        if (loadedBundles.ContainsKey(name)) { return; }
        loadedBundles.Add(name,ab);
    }
   


    void LoadFromFileAsyncAB(string name)
    {
        AssetBundle ab = AssetBundle.LoadFromFile(DataUrl.GetFilePersistentUrl(name));
        AddLoadedBundle(name, ab);
    }
    /// <summary>
    /// 加载某个Prefab依赖关系Bundle ，并存起来
    /// </summary>
    /// <param name="name"></param>
    void LoadDesBundle(string name)
    {
        var list = mainfest.GetDirectDependencies(name);
        for(int i=0;i<list.Length;i++)
        {
            if(!IsLoadedBundle(list[i]))
            LoadFromFileAsyncAB(list[i]);
        }
    }

    string GetSufixx(string name)
    {
        string[] strs = name.Split('/');
        switch(strs[0])
        {
            case "atlas":
                return "png";
            case "audio":
                return "ogg";
            case "font":
                return "otf";
            case "luascripts":
                return "txt";
            case "texturepic":
                return strs.Length == 2 ? "png":"renderTexture";
        }
        return "prefab";
    }

    public TextAsset LoadLuaAB(string fileName)
    {
        string ABName = "luascripts.ab";
        if (IsManifestContains(ABName)) //判断Manifest中是否存在 bundleName
        {
            //判断是否已经加载过
            AssetBundle ab = null;
            if (IsLoadedBundle(ABName))
            {
                ab = GetBundleByName(ABName);
            }
            else
            {
                string path = DataUrl.GetFilePersistentUrl(ABName);
                ab = AssetBundle.LoadFromFile(path); //注意这边名称是否正确，注意测试
                //加载依赖
                AddLoadedBundle(ABName, ab);
            }
            return ab.LoadAsset<TextAsset>(string.Format("Assets/Resources/{0}.{1}", fileName, "txt"));
        }
        else
        {
            return null;
        }
    }
    public T LoadAsset<T>(string fileName) where T : Object
    {
        //首先 判断 依赖中有没有该资源 
        //1.没有则LoadLoad
        //2.有的话，则判断本地是否加载过
        //2.1.加载过则用本地
        //2.2 没有加载过则进行LoadBundle在添加到本地LoadedBundle
        string newName = fileName.ToLower() + ".ab";
        if (IsManifestContains(newName)) //判断Manifest中是否存在 bundleName
        {
            //判断是否已经加载过
            AssetBundle ab = null;
            if (IsLoadedBundle(newName))
            {
                ab = GetBundleByName(newName);
            }
            else
            {
                string path = DataUrl.GetFilePersistentUrl(newName);
                ab = AssetBundle.LoadFromFile(path); //注意这边名称是否正确，注意测试
                //加载依赖
                LoadDesBundle(newName);
                AddLoadedBundle(newName, ab);
            }
            return ab.LoadAsset<T>(string.Format("Assets/Resources/{0}.{1}", fileName, GetSufixx(newName)));
        }
        else
        {
            return null;
        }
    }


    public T LoadLocalAsset<T>(string fileName) where T : Object
    {
        string path = fileName;
        return Resources.Load<T>(path);
    }

    public T LoadAsset<T>(string bundleName, string assetName) where T : Object
    {
        T prefab = null;
        if (!GameManager.Instance.mIsHotUpdate)
        {
            assetName = assetName.Split('.')[0];
            prefab = Resources.Load<T>(assetName);
        }
        else
        {
            AssetBundle b;
            loadedBundles.TryGetValue(bundleName, out b);
            if (b != null)
            {
                //				Debug.Log(string.Format(LGameConfig.ASSET_BASE_FORMAT, assetName));
                prefab = b.LoadAsset<T>(string.Format(GlobalData.ASSET_BASE_FORMAT, assetName));
            }
            else
            {
                Debug.Log("bundle not exist! : " + bundleName);
            }
        }
        return prefab;
    }
    public Object[] LoadAllAsset(string bundleName, string assetName)
    {
        return LoadAllAsset<Object>(bundleName, assetName);
    }
    public T[] LoadAllAsset<T>(string bundleName, string assetName) where T : Object
    {
        T[] prefabs = null;
        if (!GameManager.Instance.mIsHotUpdate)
        {
            assetName = assetName.Split('.')[0];
            prefabs = Resources.LoadAll<T>(assetName);
        }
        else
        {
            AssetBundle b;
            loadedBundles.TryGetValue(bundleName, out b);
            if (b != null)
            {
                prefabs = b.LoadAllAssets<T>();
            }
        }
        return prefabs;
    }


    public AssetBundle GetBundleByName(string name)
    {
        AssetBundle b;
        loadedBundles.TryGetValue(name, out b);
        return b;
    }

    bool IsLoadedBundle(string name)
    {
        return loadedBundles.ContainsKey(name);
    }
    #region Atlas

    private Dictionary<string, Sprite[]> spritesCache;
    static string GetABNameWithAtlasPath(string path)
    {
        return string.Format("{0}{1}", path.Split('.')[0].ToLower(), GlobalData.ASSETBUNDLE_AFFIX);
    }
    public Sprite[] GetSpritesByName(string bundlePath, string assetName)
    {
        string key = bundlePath;
        if (spritesCache.ContainsKey(key))
        {
            return spritesCache[key];
        }
        else
        {

            if (!GameManager.Instance.mIsHotUpdate)
            {
                Sprite[] sprites = Resources.LoadAll<Sprite>(bundlePath);
                spritesCache.Add(key, sprites);
            }
            else
            {
                string bundleName = GetABNameWithAtlasPath(bundlePath.Split('.')[0] + ".png");
                AssetBundle assetBundle = this.GetBundleByName(bundleName);
                if (assetBundle)
                {
                    Sprite[] sprites = assetBundle.LoadAllAssets<Sprite>();
                    spritesCache.Add(key, sprites);
                }
            }

            List<Sprite> _arr = new List<Sprite>();
            Sprite[] _sprites = spritesCache[key];
            foreach (Sprite s in _sprites)
            {
                if (s.name.StartsWith(assetName))
                {
                    _arr.Add(s);
                }
            }

            return _arr.ToArray();
        }
    }
    #endregion

    public void UnloadBundles(string[] bundle_names)
    {
        for (int i = 0; i < bundle_names.Length; i++)
        {
            AssetBundle b;
            loadedBundles.TryGetValue(bundle_names[i], out b);
            if (b != null)
            {
                b.Unload(true);
                loadedBundles.Remove(bundle_names[i]);
            }
        }
    }
}
