using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MsgContainer;
using DNotificationCenterManager;

public class logdata
{
    public string output = "";
    public string stack = "";
    public static logdata Init(string o, string s)
    {
        logdata log = new logdata();
        log.output = o;
        log.stack = s;
        return log;
    }

    public static logdata Init(string o)
    {
        logdata log = new logdata();
        log.output = o;
        return log;
    }
    bool showstack = false;
    public void Show(/*bool showstack*/)
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.red;
        GUILayout.Label(output, style);
        if (showstack)
        {
            GUILayout.Label(stack);
        }
    }
}
/// <summary>
/// 手机调试脚本
/// 本脚本挂在一个空对象或转换场景时不删除的对象即可
/// 错误和异常输出日记路径 Application.persistentDataPath
/// </summary>
public class ShowDebugInPhone : MonoBehaviour
{

    List<logdata> logDatas = new List<logdata>();//log链表
    List<logdata> errorDatas = new List<logdata>();//错误和异常链表
    List<logdata> warningDatas = new List<logdata>();//警告链表
    List<logdata> putCardDatas = new List<logdata>();//打牌记录
    List<logdata> sessionDatas = new List<logdata>();//Sessionjilui
    static List<string> mWriteTxt = new List<string>();
    public List<string> mPutCardCardTxt = new List<string>();
    public List<string> mSessionTxt = new List<string>();
    Vector2 uiLog;
    Vector2 uiError;
    Vector2 uiWarning;
    bool open = false;
    bool showLog = false;
    bool showError = true;
    bool showWarning = false;
    private string outpath;
    private string putCardPath;
    private string sessionPath;
    void Start()
    {
        //Application.persistentDataPath Unity中只有这个路径是既可以读也可以写的。
        //Debug.Log(Application.persistentDataPath);
        outpath = /*DataUrl.LOCAL_URL_PREFIX +*/ Application.persistentDataPath + "/outLog.txt";
        putCardPath = Application.persistentDataPath + "/CardLog.txt";
        sessionPath = Application.persistentDataPath + "/Session.txt";
        //每次启动客户端删除之前保存的Log
        if (System.IO.File.Exists(outpath))
        {
            File.Delete(outpath);
        }
        if (System.IO.File.Exists(putCardPath))
        {
            File.Delete(outpath);
        }

        if (System.IO.File.Exists(sessionPath))
        {
            File.Delete(sessionPath);
        }
        //转换场景不删除
        Application.DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ENull, TestReceveMsg);
        //注册log监听
        Application.RegisterLogCallback(HangleLog);


    }

    void OnDisable()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ENull, TestReceveMsg);
        // Remove callback when object goes out of scope
        //当对象超出范围，删除回调。
        Application.RegisterLogCallback(null);
    }

    void TestReceveMsg(LocalNotification e)
    {
        ArgsMsgTest args = e.param as ArgsMsgTest;
        HangleLog("======================================="+args.testStr, "DDDDDDDDDDD", LogType.Log);
    }

    void HangleLog(string logString, string stackTrace, LogType type)
    {
        logString = System.DateTime.Now.ToLongTimeString() + ": " + logString;
        switch (type)
        {
            case LogType.Log:
                logDatas.Add(logdata.Init(logString, stackTrace));
                mWriteTxt.Add(logString);
                mWriteTxt.Add(stackTrace);
                break;
            case LogType.Error:
            case LogType.Exception:
                errorDatas.Add(logdata.Init(logString, stackTrace));
                mWriteTxt.Add(logString);
                mWriteTxt.Add(stackTrace);
                break;
            case LogType.Warning:
                warningDatas.Add(logdata.Init(logString, stackTrace));
                mWriteTxt.Add(logString);
                break;
        }
        if (logString.Contains("========="))
        {
            HandleLog(logString);
        }

        if(logString.Contains("Session>>>>>>>>>>>>>"))
        {
            HandleSessionLog(logString);
        }
    }

    void HandleLog(string logString)
    {
        logString = System.DateTime.Now.ToLongTimeString() + ": " + logString;
        putCardDatas.Add(logdata.Init(logString));
        mPutCardCardTxt.Add(logString);
    }

    void HandleSessionLog(string logString)
    {
        logString = System.DateTime.Now.ToLongTimeString() + ": " + logString;
        sessionDatas.Add(logdata.Init(logString));
        mSessionTxt.Add(logString);
    }

    void Update()
    {
        //因为写入文件的操作必须在主线程中完成，所以在Update中才给你写入文件。
        if (errorDatas.Count > 0 || logDatas.Count > 0 || warningDatas.Count > 0)
        {
            string[] temp = mWriteTxt.ToArray();
            foreach (string t in temp)
            {
                using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))
                {
                    writer.WriteLine(t);
                }
                mWriteTxt.Remove(t);
            }
        }

        if (putCardDatas.Count > 0)
        {
            string[] temp = mPutCardCardTxt.ToArray();
            foreach (string t in temp)
            {
                using (StreamWriter writer = new StreamWriter(putCardPath, true, Encoding.UTF8))
                {
                    writer.WriteLine(t);
                }
                mPutCardCardTxt.Remove(t);
            }
        }

        if (sessionDatas.Count > 0)
        {
            string[] temp = mSessionTxt.ToArray();
            foreach (string t in temp)
            {
                using (StreamWriter writer = new StreamWriter(sessionPath, true, Encoding.UTF8))
                {
                    writer.WriteLine(t);
                }
                mSessionTxt.Remove(t);
            }
        }
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(">>Open", GUILayout.Height(150), GUILayout.Width(150)))
            open = !open;
        if (open)
        {
            if (GUILayout.Button("清理", GUILayout.Height(150), GUILayout.Width(150)))
            {
                logDatas = new List<logdata>();
                errorDatas = new List<logdata>();
                warningDatas = new List<logdata>();
            }
            if (GUILayout.Button("显示log日志:" + showLog, GUILayout.Height(150), GUILayout.Width(200)))
            {
                showLog = !showLog;
                if (open == true)
                    open = !open;
            }
            if (GUILayout.Button("显示error日志:" + showError, GUILayout.Height(150), GUILayout.Width(200)))
            {
                showError = !showError;
                if (open == true)
                    open = !open;
            }
            if (GUILayout.Button("显示warning日志:" + showWarning, GUILayout.Height(150), GUILayout.Width(200)))
            {
                showWarning = !showWarning;
                if (open == true)
                    open = !open;
            }
        }
        GUILayout.EndHorizontal();

        if (showLog)
        {
            GUI.color = Color.black;

            uiLog = GUILayout.BeginScrollView(uiLog);
            if (logDatas == null)
            {
                return;
            }
            foreach (var va in logDatas)
            {
                va.Show();
            }
            GUILayout.EndScrollView();
        }
        if (showError)
        {
            GUI.color = Color.red;
            uiError = GUILayout.BeginScrollView(uiError);
            foreach (var va in errorDatas)
            {
                va.Show();
            }
            GUILayout.EndScrollView();
        }
        if (showWarning)
        {
            GUI.color = Color.yellow;
            uiWarning = GUILayout.BeginScrollView(uiWarning);
            foreach (var va in warningDatas)
            {
                va.Show();
            }
            GUILayout.EndScrollView();
        }
    }
}