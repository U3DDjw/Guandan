using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MsgContainer;
using UnityEngine.Events;
using System.IO;
using System.Text;
using DNotificationCenterManager;

public class DownLoadManager : MonoBehaviour
{

    public static readonly string VERSION_FILE = "version.ver";

    private Dictionary<string, string> LocalResVersion;
    private Dictionary<string, int> LocalResSize;
    private Dictionary<string, string> ServerResVersion;
    private List<string> NeedDownFiles;
    private bool NeedUpdateLocalVersionFile = false;
    private int downCount;

    public UnityAction onCompleteHandler;

    private void Start()
    {
        checkUpdate();
    }
    public static string LOCAL_RES_URL
    {
        get
        {
            if (GameManager.Instance.mIsHotUpdate)
            {
                return DataUrl.LOCAL_URL_PREFIX + Application.persistentDataPath + "/";
            }
            else
            {
#if UNITY_STANDALONE_WIN
				return "file://" + Application.dataPath + "/StreamingAssets/";  
#elif UNITY_ANDROID
                return Application.streamingAssetsPath + "/";
#else
                    return string.Empty;  
#endif
            }
        }
    }

    public static string LOCAL_RES_PATH
    {
        get
        {
            return Application.persistentDataPath + "/";
        }
    }

    public void checkUpdate()
    {
        downLoadTime = 0;
        Debug.Log("开始热更");
        //初始化  
        LocalResVersion = new Dictionary<string, string>();
        ServerResVersion = new Dictionary<string, string>();
        LocalResSize = new Dictionary<string, int>();
        NeedDownFiles = new List<string>();

        Debug.Log("客户端ver:" + LOCAL_RES_URL + VERSION_FILE);
        //加载本地version配置  
        StartCoroutine(DownLoad(LOCAL_RES_URL + VERSION_FILE, false, delegate (WWW localVersion)
         {
            //保存本地的version  
            ParseVersionFile(localVersion.text, LocalResVersion);
             Debug.Log("服务端ver:" + GlobalData.mServerResIp + VERSION_FILE);
            //加载服务端version配置  
            StartCoroutine(this.DownLoad(GlobalData.mServerResIp + VERSION_FILE, false, delegate (WWW serverVersion)
             {
                //保存服务端version  
                ParseVersionFile(serverVersion.text, ServerResVersion, LocalResSize);
                //计算出需要重新加载的资源  
                CompareVersion(LocalResSize);
                //加载需要更新的资源  
                DownLoadRes();
             }));
         }));
    }

    //依次加载需要更新的资源  
    private void DownLoadRes()
    {

        if (NeedDownFiles.Count == 0) //这边可以优化，下载一个资源，记录一个到本地version
        {
            UpdateLocalVersionFile();
            return;
        }

        string file = NeedDownFiles[0];
        NeedDownFiles.RemoveAt(0);

        StartCoroutine(this.DownLoad(GlobalData.mServerResIp + file, true, delegate (WWW w)
        {
            //将下载的资源替换本地就的资源  
            ReplaceLocalRes(file, w.bytes, () =>
            {
                DownLoadRes();
            });
        }));
    }


    void CreateFolder(string fileName)
    {
        if (fileName.Contains("/"))
        {
            string[] strs = fileName.ToLower().Split('/');

            if (!Directory.Exists(strs[0]))
            {
                Directory.CreateDirectory(LOCAL_RES_PATH + strs[0]);
                if (strs.Length > 2)
                {
                    if (!Directory.Exists(strs[1]))
                    {
                        Directory.CreateDirectory(LOCAL_RES_PATH + strs[0] + "/" + strs[1]);
                    }
                }
            }
            else
            {
                if (strs.Length > 2)
                {
                    if (!Directory.Exists(strs[1]))
                    {
                        Directory.CreateDirectory(LOCAL_RES_PATH + strs[0] + "/" + strs[1]);
                    }
                }
            }
        }
    }
    private void ReplaceLocalRes(string fileName, byte[] data, UnityAction onComplete)
    {
        CreateFolder(fileName);

        string filePath = LOCAL_RES_PATH + fileName;

        FileStream stream = new FileStream(filePath, FileMode.Create);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
        onComplete.Invoke();
    }


    //显示资源
    private void Complate()
    {
        if (onCompleteHandler != null)
        {
            onCompleteHandler.Invoke();
        }
        Debug.Log("资源更新完毕");
        LoadBundleController.GetInstance();
        SendDownOverArgs();
    }

    //更新本地的version配置  
    private void UpdateLocalVersionFile()
    {
        if (NeedUpdateLocalVersionFile)
        {
            StringBuilder versions = new StringBuilder();
            var e = ServerResVersion.GetEnumerator();
            while (e.MoveNext())
            {
                versions.Append(e.Current.Key).Append(",").Append(e.Current.Value).Append("\n");
            }

            FileStream stream = new FileStream(LOCAL_RES_PATH + VERSION_FILE, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(versions.ToString());
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();

            Debug.Log("更新版本号");
        }
        //加载显示对象  
        //StartCoroutine(Complate());
        Complate();
    }

    private void CompareVersion(Dictionary<string, int> sizeDic)
    {
        foreach (var version in ServerResVersion)
        {
            string fileName = version.Key;
            string serverMd5 = version.Value;

            //新增的资源  
            if (!LocalResVersion.ContainsKey(fileName))
            {
                Debug.Log("需更新：" + fileName);
                NeedDownFiles.Add(fileName);
            }
            else
            {
                //需要替换的资源  
                string localMd5;
                LocalResVersion.TryGetValue(fileName, out localMd5);
                if (!serverMd5.Equals(localMd5))
                {
                    Debug.Log("需更新：" + fileName);
                    NeedDownFiles.Add(fileName);
                }
                else //不是新增资源 也不是更新资源
                {
                    sizeDic.Remove(fileName);     
                }
            }
        }

        //本次有更新，同时更新本地的version.ver  
        downCount = NeedDownFiles.Count;
        NeedUpdateLocalVersionFile = downCount > 0;
        if (!NeedUpdateLocalVersionFile)
        {
            Debug.Log("无更新....");
        }
        else
        {
            totalDownKB = TotalDownKB(sizeDic);
        }
    }
    private void ParseVersionFile(string content, Dictionary<string, string> dict)
    {
        if (content == null || content.Length == 0)
        {
            return;
        }
        string[] items = content.Split(new char[] { '\n' });
        int itemsLen = items.Length;
        for (int i = 0; i < itemsLen; i++)
        {
            string[] info = items[i].Split(new char[] { ',' });
            if (info != null && info.Length > 1)
            {
                dict.Add(info[0], info[1]);
            }
        }

    }
    private void ParseVersionFile(string content, Dictionary<string, string> dict, Dictionary<string, int> sizeDic)
    {
        if (content == null || content.Length == 0)
        {
            return;
        }
        string[] items = content.Split(new char[] { '\n' });
        int itemsLen = items.Length;
        for (int i = 0; i < itemsLen; i++)
        {
            string[] info = items[i].Split(new char[] { ',' });

            if (info != null)
            {
                if (info.Length == 3)
                {
                    dict.Add(info[0], info[1]);
                    sizeDic.Add(info[0], ConvertToSize(info[2]));
                }
                else if (info.Length == 2)
                {
                    dict.Add(info[0], info[1]);
                }
            }
        }
    }

    int loadResIdx = 0;
    public static float downLoadTime = 0;
    private IEnumerator DownLoad(string url, bool isRes, HandleFinishDownload finishFun)
    {
      
        WWW www = new WWW(url);
        if (isRes)
        {
            loadResIdx++;
        }    
        while (isRes && !www.isDone)
        {
            if (www.progress > 0.01f)
            {           
                SendDownLoadProgressArgs(loadResIdx, url, www.progress);
                yield return null;
                downLoadTime += Time.deltaTime;
            }
        }
        
        yield return www;
        if (finishFun != null)
        {
            finishFun(www);
        }
        www.Dispose();
    }


    int ConvertToSize(string name)
    {
        string str = name.Replace("KB", "");
        return int.Parse(str);
    }

    float TotalDownKB(Dictionary<string, int> sizeDic)
    {
        float total = 0;
        var iDowns = sizeDic.GetEnumerator();
        while (iDowns.MoveNext())
        {
            total += iDowns.Current.Value;
        }
        return total;
    }
    float totalDownKB = 0;
    void SendDownLoadProgressArgs(int idx, string url, float progress)
    {
        ArgsDownLoadProgress msg = new ArgsDownLoadProgress();
        msg.total = totalDownKB;
        msg.progress = GetCurKb(idx, progress) / totalDownKB;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EDownLoadProgress, msg);
    }

    void SendDownOverArgs()
    {
        Debug.Log("isOver++++"+ downLoadTime);
        ArgsDownLoadProgress msg = new ArgsDownLoadProgress();
        msg.total = totalDownKB;
        msg.progress = 1;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EDownLoadProgress, msg);
    }
    float GetCurKb(int idx, float progress)
    {
        float curKB = 0;
        int i = 0;
        foreach (var v in LocalResSize)
        {
            i++;
            if (i == idx)
            {
                curKB += v.Value * progress;
                break;
            }
            else
            {
                curKB += v.Value;
            }
        }
        return curKB;
    }
}
