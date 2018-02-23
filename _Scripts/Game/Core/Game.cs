using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System.Threading;
using Net;
using System.Linq;
using System.IO;
using MsgContainer;
using DNotificationCenterManager;
using UnityEngine.SceneManagement;
using Umeng;
using YunvaIM;
using HaoyunFramework;
using Haoyun.Utils;

public class Game : MonoBehaviour, IGame
{
    [HideInInspector]
    public GameObject mUIRoot;


    public EGameMode mGameMode = EGameMode.EDebug;
    public EGameChanel mGameChanel = EGameChanel.EHY;
    public EGameServerUrl mGameServerType = EGameServerUrl.ESCZ;
    public bool mIsOpenDebugInMobile = false;
    public bool mIsUseAI = true;
    public bool mIsHotUpdate = false;
    public bool mIsLuaUpdate = false;
    public bool mIsFormalResServer = false;


    void OnDestroy()
    {
        Debug.Log("游戏已被强制关闭");
        SDKManager.Instance.yayaLogout();// 呀呀语音退出
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EBackToLoginScene, BackToLoginScene);
        Resources.UnloadUnusedAssets();
        XLuaInstance.DestroyInstance();
    }
    void Awake()
    {
        GameManager.Instance.mGameChanel = this.mGameChanel;
        GameManager.Instance.mGameUrlType = mGameServerType;
        GameManager.Instance.mGameMode = mGameMode;
        GameManager.Instance.mIsUseAI = this.mIsUseAI;
        GameManager.Instance.mIsOpenDebugInMobile = this.mIsOpenDebugInMobile;
        GameManager.Instance.mIsHotUpdate = this.mIsHotUpdate;
        GameManager.Instance.mGameObj = this.gameObject;
        GameManager.Instance.mIsLuaUpdate = this.mIsLuaUpdate;
        GameManager.Instance.mIsFormalResServer = this.mIsFormalResServer;

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EBackToLoginScene, BackToLoginScene);
        RefreshUIRoot();
        DontDestroyOnLoad(this);
        InitEnvironment();
    }


    void BackToLoginScene(LocalNotification e)
    {
        StartCoroutine(LoadScene(GlobalData.mScene_Login, () =>
        {
            //Clear All View
            TweenManager.Instance.ClearViewSingleAnimation();
            //Clear All Data
            GameManager.Instance.mIsBackToLoginModule = true;
            RoomInfo.Instance.ClearInfo();
            ContextManager.Instance.PopAll();
            SDKManager.Instance.yayaLogout();// 呀呀语音退出
            Debug.Log("Back To LoginModule...");
            DestroyImmediate(this.gameObject);
        }));
    }

    void InitEnvironment()
    {
        Analytics.StartWithAppKeyAndChannelId(GlobalData.umengAppkey, GlobalData.mHostId); // umeng game analytics 初始化
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Application.targetFrameRate = 25;
        Application.runInBackground = true;
        Input.multiTouchEnabled = false;
        if (mIsOpenDebugInMobile)
        {
            GameObject gm = GameObject.Instantiate(Resources.Load<GameObject>("_LocalResource/Tools/DebugInPhoneObj"));
        }
        else
        {
            if (GameManager.Instance.mGameMode == EGameMode.EFormal || GameManager.Instance.mGameMode == EGameMode.EAppleOnLine)
            {
                //Debug.logger.logEnabled = false;
            }
        }
    }




    IEnumerator LoadScene(string sceneName, FinishCallHandler callBack = null)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); //        
        while (op.isDone)
        {
            yield return null;
        }
        if (callBack != null)
        {
            callBack();
        }
        op.allowSceneActivation = true;
    }

    public void RefreshUIRoot()
    {
        mUIRoot = GameObject.Find(GlobalData.mUIRootName);
    }
    void Start()
    {
        ////////这边写了防止丢包
        //var threradLoom = Loom.Current;
        //Loom.RunAsync(() =>
        //{
        //    UdpClient.GetInstance().RecvRun();
        //    UdpClient.GetInstance().SendRun();
        //    Loom.QueueOnMainThread(() =>
        //    {
        //        UdpClient.GetInstance().RecvRun();
        //        UdpClient.GetInstance().SendRun();
        //    });
        //}
        //);

        TCPNetWork.GetInstance();

        AudioManager.Instance.Load();

        SDKManager.Instance.GetAnotherQuest();

        // 苹果支付 初始化
#if UNITY_IPHONE || UNITY_IOS
        IOSIAP.GetIns().InitIOSPurchaseItem();
#endif
        // 呀呀语音初始化
        int init = YunVaImSDK.instance.YunVa_Init(0, GlobalData.yayaAppid, Application.persistentDataPath, false, false);
        if (init != 0)
        {
            Debug.Log("呀呀语音初始化失败...");
        }
    }







    void Update()
    {
        AudioManager.Instance.OnUpdate();

        //安卓按钮退出按钮(大退)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManagers.Instance.ShowConfirmBox("确认退出游戏吗？", "确认", "取消", SureExitgame, null);
        }
    }
    void SureExitgame()
    {
        Application.Quit();
    }


    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadNormalScene(sceneName));
    }

    IEnumerator LoadNormalScene(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); //        
        while (op.isDone)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
    }

}


