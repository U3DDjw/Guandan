using UnityEngine;
using System.Collections;
using DNotificationCenterManager;
using System.IO;
using LitJson;
using ZhiWa;
using Net;
using MsgContainer;
using System;
using Haoyun.Utils;
using HaoyunFramework;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoginContext : BaseContext
{
    public LoginContext()
    {
        ViewType = UIType.LoginView;
    }
}

public class UILoginView : BasesView
{
    [SerializeField]
    Text uuidLab;
    [SerializeField]
    Text versionLab;

    [SerializeField]
    GameObject weChatBtn;

    [SerializeField]
    GameObject nameObj;
    [SerializeField]
    Image loginSpr;

    void Awake()
    {
     //   NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ELoginSucc, UpdateLoginSucc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ELoginFail, UpdateLoginfail);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ELoginByWeChatSuc, UpdateLoginByWeChatSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EResetLogin, ResetLogin);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EHideLogining, HideLogining);
    }
    void OnDestroy()
    {
       // NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ELoginSucc, UpdateLoginSucc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ELoginFail, UpdateLoginfail);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ELoginByWeChatSuc, UpdateLoginByWeChatSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EResetLogin, ResetLogin);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EHideLogining, HideLogining);
    }

    private void Start()
    {

        bool isDebug = GameManager.Instance.mGameMode == EGameMode.EDebug;
        nameObj.gameObject.SetActive(isDebug);
       
        TweenManager.Instance.SwitchLoadingSpr(false);  
        SetVersionLab();
        CheckQuickLogin();
        RequestUrldata();
        EventTriggerListener.Get(loginSpr.gameObject).onClick = OnWeChatLoginClick;
    }



    void RequestUrldata()
    {
        var tempDic = new Dictionary<string, string>();
        tempDic.Add("appId", GlobalData.mAppId);
        tempDic.Add("hostId", GlobalData.mHostId);
        tempDic.Add("time", Haoyun.Utils.TimeUtils.ConvertToTime(System.DateTime.Now).ToString());
        WWWForm form = new WWWForm();
        form.AddField("sig", RSAVerify.SerifizationSignature(form, tempDic));
       
        //地址更新
        string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_NoticInfo;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {

            TextureURLData mUrlData = JsonManager.GetNoticData(www.text);
            PlayerInfo.Instance.mUrlData = mUrlData;

            // 根据上架的版本号来判断是否是上架中，使用来mUrlData中的两个预留字段
            if (mUrlData.yuliu1 == GlobalData.mVersion)
            {
                if (mUrlData.yuliu2 == "0")
                {
                    PlayerInfo.Instance.mUrlData.iosSj = true;
                }
                else
                {
                    PlayerInfo.Instance.mUrlData.iosSj = false;
                }
            }
            else
            {
                PlayerInfo.Instance.mUrlData.iosSj = false;
            }

            bool isVisitor = GameManager.Instance.mGameMode == EGameMode.EAppleOnLine && mUrlData.iosSj;
            CheckAppleOnLine(isVisitor);

        }));
    }

    void CheckAppleOnLine(bool isVisitor)
    {
        if (isVisitor)
        {
            loginSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "btn_lodin3");
        }
    }

    void HideLogining(LocalNotification e)
    {
        TweenManager.Instance.SwitchLoadingSpr(false);
    }

    /// <summary>
    /// 是否拥有Token
    /// </summary>
    /// <returns></returns>
    bool IsTokenLogin()
    {
        var curToken = XPlayerPrefs.Instance.mPlayerToken;
        if (curToken == null || curToken == "" || curToken.Length == 0)
        {
            return false;
        }

        return true;
    }
    void CheckQuickLogin()
    {
        if (!IsTokenLogin())
        {
            return;
        }
        var curToken = XPlayerPrefs.Instance.mPlayerToken;
        WWWForm form = new WWWForm();
        var tempDic = new Dictionary<string, string>();
        tempDic.Add("loginType", "TOKEN");
        tempDic.Add("token", curToken);
        tempDic.Add("appId", GlobalData.mAppId);
        tempDic.Add("hostId", GlobalData.mHostId);
        tempDic.Add("uuid", SystemInfo.deviceUniqueIdentifier.ToString());
        form.AddField("sig", RSAVerify.SerifizationSignature(form, tempDic));

        StartCoroutine(GlobalData.SendPost(MsgContainer.GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_Login, form, (WWW www) => HttpLoginSucCallBack(www)));
    }

    void SetVersionLab()
    {
        versionLab.text = "版本号：" + GlobalData.mVersion;
    }

    //void UpdateLoginSucc(LocalNotification e)
    //{
    //    ContextManager.Instance.PopAll();
    //    GameManager.Instance.SwitchGameStatus(EGameStatus.EHome);
    //}


    void UpdateLoginfail(LocalNotification e)
    {
        TweenManager.Instance.SwitchLoadingSpr(false);

        Debug.Log("UDP请求失败");
        ConnectTCPServer(PlayerInfo.Instance.GetServerIp, PlayerInfo.Instance.GetServerPort, SendServerToLogin);
    }



    void OnLoginClick(GameObject m)
    {
        var tempDic = new Dictionary<string, string>();

        var curToken = XPlayerPrefs.Instance.mPlayerToken;
        bool isVisitor = GameManager.Instance.mGameMode == EGameMode.EAppleOnLine && PlayerInfo.Instance.mUrlData.iosSj;
        if (GameManager.Instance.mGameMode == EGameMode.EFormal || isVisitor)
        {
            uuidLab.text = SystemInfo.deviceUniqueIdentifier.ToString(); //这边获取唯一设备号，可能还不是很完善
        }
        else if (GameManager.Instance.mGameMode == EGameMode.EDebug)
        {
            curToken = "";
        }


        if (curToken != "" || curToken.Length > 0)
        {
            tempDic.Add("loginType", "TOKEN");
            tempDic.Add("token", curToken);
        }
        else
        {
            tempDic.Add("loginType", "YK");
            tempDic.Add("channelId", "APPSTORE");
            if (GameManager.Instance.mGameMode == EGameMode.EDebug)
            {
                if (uuidLab.text == "")
                {
                    var tData = (DateTime.Now.Ticks).ToString();
                    tempDic.Add("uuid", tData);
                }
                else
                {
                    tempDic.Add("uuid", uuidLab.text);
                }
            }
            else
            {
                tempDic.Add("uuid", uuidLab.text);
            }
        }
        tempDic.Add("appId", GlobalData.mAppId);
        tempDic.Add("hostId", GlobalData.mHostId);
        WWWForm form = new WWWForm();
        form.AddField("sig", RSAVerify.SerifizationSignature(form, tempDic));
        StartCoroutine(GlobalData.SendPost(MsgContainer.GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_Login, form, (WWW www) => HttpLoginSucCallBack(www)));
    }

    void UpdateLoginByWeChatSuc(LocalNotification e)
    {
        ArgsWeChat args = e.param as ArgsWeChat;
        if (args != null)
        {
            LoginByWeChat(args.refreshToken, args.access_token, args.openid);
        }
    }

    void ResetLogin(LocalNotification e)
    {
        XPlayerPrefs.Instance.mPlayerToken = "";
        OnWeChatLoginClick(null);
    }
    void LoginByWeChat(string refreshToken, string access_token, string openid)
    {
        var dic = new Dictionary<string, string>();
        var curToken = XPlayerPrefs.Instance.mPlayerToken;
        if (curToken != "" || curToken.Length > 0)
        {
            dic.Add("loginType", "TOKEN");
            dic.Add("token", curToken);
        }
        else
        {
            dic.Add("loginType", "WECHAT");
            dic.Add("openid", openid);
            dic.Add("refreshToken", refreshToken);
            dic.Add("accessToken", access_token);
        }
        dic.Add("appId", GlobalData.mAppId);
        dic.Add("hostId", GlobalData.mHostId);
        dic.Add("uuid", SystemInfo.deviceUniqueIdentifier.ToString());
        WWWForm form = new WWWForm();
        form.AddField("sig", RSAVerify.SerifizationSignature(form, dic));
        StartCoroutine(GlobalData.SendPost(MsgContainer.GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_Login, form, (WWW www)=>  HttpLoginSucCallBack(www)));
    }


    void HttpLoginSucCallBack(WWW www)
    {
        Debug.Log("HTTP----登录成功");
        Debug.Log(www.text);

        PlayerInfo.Instance.mFightServer = JsonManager.GetFightServerData(www.text);
        PlayerInfo.Instance.mPlayerData = JsonManager.GetPlayerData(www.text);
        PlayerInfo.Instance.mIsSfzFlg = JsonManager.GetIsSfzFlg(www.text);
        PlayerInfo.Instance.mIsSfzShow = JsonManager.GetIsSfzShow(www.text);
        ConnectTCPServer(PlayerInfo.Instance.GetServerIp, PlayerInfo.Instance.GetServerPort, SendServerToLogin);
    }

    void ConnectTCPServer(string ip,int port,Action callBack)
    {
        TCPNetWork.GetInstance().Connect(ip,port,callBack) ;
    }


    void SendServerToLogin()
    {
        Debug.Log("Send Servver To Login");
        var fighter = PlayerInfo.Instance.mFightServer;
        var player = PlayerInfo.Instance.mPlayerData;
        MsgGlobal mGl = new MsgGlobal();

        mGl.login = new @public.MsgLogin();
        var msg = mGl.login;

        msg.token = player.token;
        msg.appId = player.appId;
        msg.hostId = GlobalData.mHostId;
        msg.channelId = player.channelId;
        msg.uuid = player.uuid;
        msg.pid = (ulong)player.pid;
        msg.time = (ulong)TimeUtils.ConvertToTime(DateTime.Now);
        msg.ipAds = SDKManager.Instance.ipAds;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_LOGIN, mGl);
    }

    void OnWeChatLoginClick(GameObject gm)
    {
        Debug.Log("OnClick Login");
		bool isIosSj = GameManager.Instance.mGameMode == EGameMode.EAppleOnLine && PlayerInfo.Instance.mUrlData.iosSj;
		if (GameManager.Instance.mGameMode == EGameMode.EDebug || isIosSj)
        {
            OnLoginClick(null);
        }
        else
        {
            SDKManager.Instance.WeChatLogin();
        }
        TweenManager.Instance.SwitchLoadingSpr(true);
    }


}
