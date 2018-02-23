using DNotificationCenterManager;
using Haoyun.Utils;
using HaoyunFramework;
using MsgContainer;
using Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using YunvaIM;
using ZhiWa;
using System.Runtime.InteropServices;

public class SDKManager : MonoBehaviour
{

    public static SDKManager Instance;

    Social.AuthDelegate authcallback =
        delegate (Platform platform, int stCode, Dictionary<string, string> message)
        {
            SDKManager.Instance.isWechatCallback = 0;
            if (stCode == Social.SUCCESS)
            {
                //var str = "success";
                //foreach (KeyValuePair<string, string> kv in message)
                //{
                //    string n = kv.Key;
                //    string s = kv.Value;
                //    str = str + "   " + n + ":" + s;
                //}
                //Debug.Log(str);
                SDKManager.Instance.isWechatCallback = 1;
                ArgsWeChat args = new ArgsWeChat();
                args.access_token = message["accessToken"];
                args.openid = message["uid"];
                args.refreshToken = message["refreshToken"];
                Debug.Log(args);
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ELoginByWeChatSuc, args);
            }
            else
            {
                var str = "fail=";
                foreach (KeyValuePair<string, string> kv in message)
                {
                    string n = kv.Key;
                    string s = kv.Value;
                    str = str + "   " + n + ":" + s;
                }
                Debug.Log(str);
            }
            ;
        };

    Social.ShareDelegate sharecallback =
        delegate (Platform platform, int stCode, string errorMsg)
        {
            //SDKManager.Instance.isWechatCallback = 0;
            if (stCode == Social.SUCCESS)
            {
                Debug.Log("分享SUC");
            }
            else
            {
                Debug.Log("fail" + errorMsg);
            };
        };

    Social.ShareDelegate sharecallbackForRoomcard =
        delegate (Platform platform, int stCode, string errorMsg)
        {
            //SDKManager.Instance.isWechatCallback = 0;
            if (stCode == Social.SUCCESS)
            {
                TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.GUANDAN_ROOM_WECHAT_SHARE_FOR_ROOMCARD);
                UIManagers.Instance.EnqueueTip("分享成功，请到邮箱领取奖励");
            }
            else
            {
                Debug.Log("fail" + errorMsg);
                UIManagers.Instance.EnqueueTip("分享取消");
            };
        };
    public int mRoomCode;
    public int viewStarted;
    void Awake()
    {
        Instance = this;
        if (!Application.isMobilePlatform) { return; }
    }


    /// <summary>
    /// 接收服务器发送过来的code
    /// </summary>
    /// <param name="code"></param>
    public void ReCode(string code)
    {
        Debug.Log("code," + code);
        string WXAPPID = GlobalData.WXAPPID;//非mappId
        string SECRET =GlobalData.SECRET;
        string CODE = code;
        string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + WXAPPID + "&secret=" + SECRET + "&code=" + CODE + "&grant_type=authorization_code";
        Debug.Log("ReCode,url:" + url);
        StartCoroutine(SendGet(url));
    }


    public void ReceiveTest(string str)
    {
        Debug.Log("收到消息啦" + str);
    }
    IEnumerator SendGet(string _url)
    {
        WWW getData = new WWW(_url);
        yield return getData;
#if UNITY_IPHONE  || UNITY_STANDALONE_WIN
        if (getData.error != "")
        {
            Debug.LogError(getData.error);
        }
#elif UNITY_ANDROID
        if (getData.error != null)
        {
            Debug.LogError(getData.error);
        }
#endif
        else
        {
            Debug.Log(getData.text);

            var t = JsonManager.GetWeChatLoginSucData(getData.text);
            ArgsWeChat args = new ArgsWeChat();
            args.access_token = t.access_token;
            args.openid = t.openid;
            args.refreshToken = t.refresh_token;
            //Debug.Log(t);
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ELoginByWeChatSuc, args);

        }
    }

    public void WeChatLogin()
    {
        isWechatCallback = 1;
        Social.Authorize(Platform.WEIXIN, authcallback);
    }

    /****************************************************  微信分享方法 *********************************************************/
    public uint shareRoomCode = 0;
#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")] 
		private static extern void doOpenWechat(); 
#endif
    public void openWechat()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
		doOpenWechat();
#elif UNITY_ANDROID && !UNITY_EDITOR
		var javaClass = new AndroidJavaObject(GlobalData.mGamePackageName + ".CSharpToJava");
        javaClass.Call("openWechat");
#endif

    }
    /// <summary>
    /// 微信分享链接
    /// </summary>
    public void WeChatShareLink(string title, string descripition, string url)
    {
        //isWechatCallback = 1;
        Social.DirectShare(Platform.WEIXIN, descripition, "https://hygg.3399hy.com/pic_resource/HY_NJ_ZQ/" + GlobalData.mHostId + "/icon.png", title, url, sharecallback);
    }

    // 截屏然后微信分享图片
    public void WeChatShareImage()
    {
        //isWechatCallback = 1;
        StartCoroutine(CaptureScreen());
    }
    public IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();
        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string filename = Application.persistentDataPath + "/" + Haoyun.Utils.TimeUtils.ConvertToTime(DateTime.Now).ToString() + ".png";
        File.WriteAllBytes(filename, bytes);

        Platform[] platforms = { Platform.WEIXIN, Platform.WEIXIN_CIRCLE };
        Social.OpenShareWithImagePath(platforms, "", filename, "", "", sharecallback);
    }

    // 微信分享固定图片
    public void WeChatShareNativeImage()
    {
        //Debug.Log ("----WeChatShareNativeImage----");
        //isWechatCallback = 1;
        if (System.IO.File.Exists(Application.persistentDataPath + "/share.png"))
        {
            //Debug.Log ("----WeChatShareNativeImage----1--");
            Social.DirectShare(Platform.WEIXIN_CIRCLE, "", Application.persistentDataPath + "/share.png", "", "", sharecallbackForRoomcard);
        }
        else
        {
            //Debug.Log ("----WeChatShareNativeImage----2--");
            Social.DirectShare(Platform.WEIXIN_CIRCLE, "", PlayerInfo.Instance.mUrlData.sharePicUrl, "", "", sharecallbackForRoomcard);
        }
    }

    // 判断是否是直接进入房间
    public void GetAnotherQuest()
    {
        if (!Application.isMobilePlatform) { return; }
    }
    /****************************************************  微信分享url 进入房间 *********************************************************/
    public void ReAnotherRequest(string roomCode)
    {
        Debug.Log("直接加入房间：" + roomCode);
        isWechatCallback = 1;
        mRoomCode = int.Parse(roomCode);
        Debug.Log("viewStarted=" + viewStarted);
        if (viewStarted > 0 && mRoomCode > 0)
        {
            if (RoomInfo.Instance.mRoomCode > 0) //有房间的情况被唤起，首先退出当前房间，再加入新的房间
            {
                if (RoomInfo.Instance.mRoomCode == mRoomCode)
                {
                    Debug.Log("ReAnotherRequest-joinTheSameRoom:" + mRoomCode);
                    JoinRoom();
                }
                else
                {
                    Debug.Log("ReAnotherRequest-exitOldRoomCode:" + RoomInfo.Instance.mRoomCode);
                    RoomInfo.Instance.SendExitRoomServer();
                    Invoke("JoinRoom", 0.5f); //等待房间退出完
                }
            }
            else
            {
                Debug.Log("ReAnotherRequest-joinRoom:" + roomCode);
                JoinRoom();
            }
        }
    }

    void JoinRoom()
    {
        RoomInfo.Instance.SendJoinRoomServer(mRoomCode);
    }

    /****************************************************  网络切换，刷新 *********************************************************/
    // 切换网络
    public void changeNet(string newNetName)
    {
        //PlayingGameInfo.Instance.SendRefreshGameServer();
        PlayingGameInfo.Instance.SendReconnectServer();
    }

    private bool netIsActive = true;
    private int needCheck = 0;
    string checkUrl = "http://www.baidu.com";
    public void netCheck()
    {
        //if(UdpClient.GetInstance().mIsPause){
        needCheck++;
        if (needCheck == 100)
        {
            needCheck = 0;
            WWW www = new WWW(checkUrl);
            StartCoroutine(doNetCheck(www));
        }
        //}
    }
    public IEnumerator doNetCheck(WWW www)
    {
        float endtime = Time.realtimeSinceStartup + 4;
        while (true)
        {
            if (www.isDone)
            {
                if (www.error == null || www.error == "")
                {
                    // Debug.Log("-------可用");
                    if (!netIsActive)
                    {
                        // Debug.Log("-------不可用---可用");
                        PlayingGameInfo.Instance.SendReconnectServer();
                    }
                    netIsActive = true;
                }
                else
                {
                    // Debug.Log("-------不可用");
                    if (netIsActive)
                    {
                        // Debug.Log("-------可用----不可用");
                        //UIManagers.Instance.EnqueueTip("网络异常，请检查网络连接状况...");
                    }
                    netIsActive = false;
                }
                www.Dispose();
                yield break;
            }
            if (Time.realtimeSinceStartup > endtime)
            {
                // Debug.Log("-------不可用");
                if (netIsActive)
                {
                    // Debug.Log("-------可用----不可用");
                    //UIManagers.Instance.EnqueueTip("网络异常，请检查网络连接状况...");
                }
                netIsActive = false;
                www.Dispose();
                yield break;
            }
            yield return null;
        }
    }

    /****************************************************  通过高德获取具体地址 *********************************************************/
    public string ipAds = "";

#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")] 
		private static extern void openLocationService(); 
#endif

    public void doOpenLocationService()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
			openLocationService ();
#elif UNITY_ANDROID && !UNITY_EDITOR
		var javaClass = new AndroidJavaObject(GlobalData.mGamePackageName + ".CSharpToJava");
        javaClass.Call("openLocationService");
#endif
    }

    public void getAddressFromGd(string address)
    {
        Debug.Log("高德定位:" + address);
        ipAds = address;
        PlayerInfo.Instance.mPlayerData.ipAds = address;
        MsgGlobal msgGlobal = new MsgGlobal();
        msgGlobal.login = new @public.MsgLogin();
        var msg = msgGlobal.login;
        msg.appId = PlayerInfo.Instance.mPlayerData.appId;
        msg.pid = (ulong)PlayerInfo.Instance.mPlayerData.pid;
        msg.ipAds = ipAds;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.GUANDAN_ROOM_UPDATE_IPADS, msgGlobal);
    }

    /****************************************************  ios挂起之后重启线程 *********************************************************/
    public int isWechatCallback = 0;
    public string clipRoomCode = "";
    public void restartThreads()
    {
        //Debug.Log ("restartThreads----GetUDPServerPort:  " + PlayerInfo.Instance.GetUDPServerPort);
        //Debug.Log ("restartThreads----isWechatCallback:  " + isWechatCallback);
        if (PlayerInfo.Instance.GetUDPServerPort > 0 && isWechatCallback == 0)
        {
            Debug.Log("-------restartSocket--------");
            TCPNetWork.GetInstance().ReNowConnect();
        }
    }
    /****************************************************  读取剪切板数据进入房间 *********************************************************/
#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport ("__Internal")]  
		private static extern string _getClipboardText();  
#endif
    public void GetClipRoomCodeEnterRoom()
    {
        Debug.Log("返回到游戏--------");  //  返回游戏的时候触发     执行顺序 2
        var text = "";
#if UNITY_IPHONE && !UNITY_EDITOR
		text = _getClipboardText();


#elif UNITY_ANDROID && !UNITY_EDITOR
		var javaClass = new AndroidJavaObject(GlobalData.mGamePackageName + ".CSharpToJava");
		text = javaClass.Call<string>("getClipboardText");

#endif

        if (text != null && text != "")
        {
            Debug.Log(text);
            string[] texts = text.Split('*');
            if (texts.Length >= 3)
            {
                string roomCode = texts[1];
                bool isHomeView = GameManager.Instance.mCurGameStatus == EGameStatus.EHome && !ContextManager.Instance.IsContains(UIType.WaitGameView.Name);
                if (isHomeView)
                {
                    Debug.Log("当前处主界面");
                    SDKManager.Instance.ReAnotherRequest(roomCode);
                    GlobalData.CopyTextFromLab("");
                }
            }

        }
    }
    /**************************************************** Apple Pay 回调 *********************************************************/
    // itemID =　fk3 : 3张房卡
    // itemID =　fk9 : 9张房卡
    // itemID =　fk30 : 30张房卡
    public void iosiapCallback(string receipt, string itemID)
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("pid", PlayerInfo.Instance.mPlayerData.pid.ToString());
        form.AddField("itemId", itemID);
        form.AddField("receiptData", receipt);
        form.AddField("time", Haoyun.Utils.TimeUtils.ConvertToTime(System.DateTime.Now).ToString());
        //form.AddField("sig", GlobalData.sig);

        string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_Iosiap;

        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            if (www.text == "OK")
            {
                //uint rcNum = uint.Parse(itemID.Remove(0, 2));
                //GameObject emailTween = ResourceManager.Instance.LoadAsset<GameObject>("EmailTween");
                //emailTween.transform.SetParent(transform.parent);
                //emailTween.transform.localPosition = Vector3.zero;
                //emailTween.transform.localScale = Vector3.one;
                //emailTween.GetComponent<UIEmailTween>().SetInitData((int)rcNum, EEmailType.ERoomCard);
                //PlayerInfo.Instance.AddRoomCard((int)rcNum);
                UIManagers.Instance.EnqueueTip("支付成功，请到邮箱领取");
                TDGAVirtualCurrency.OnChargeSuccess(GlobalData.applePayOrderId);
            }
            else
            {
                UIManagers.Instance.EnqueueTip("支付失败");
            }
        }));
    }

    /**************************************************** 呀呀语音相关 *********************************************************/
    // 登录服务器
    public void yayaLogin()
    {
        string ttFormat = "{{\"nickname\":\"{0}\",\"uid\":\"{1}\"}}";
        string pid = PlayerInfo.Instance.mPlayerData.pid.ToString();
        string tt = string.Format(ttFormat, pid, pid);
        string[] wildcard = new string[2];
        wildcard[0] = "0x001";
        wildcard[1] = "0x002";
        YunVaImSDK.instance.YunVaOnLogin(tt, "1", wildcard, 0, (data) =>
        {
            if (data.result == 0)
            {
                Debug.Log(string.Format("登录成功，昵称:{0},用户ID:{1}", data.nickName, data.userId));
                //YunVaImSDK.instance.RecordSetInfoReq(true);//开启录音的音量大小回调
            }
            else
            {
                Debug.Log(string.Format("登录失败，错误消息：{0}", data.msg));
            }
        });
    }
    // 退出服务器
    public void yayaLogout()
    {
        if (Application.isMobilePlatform)
        {
            YunVaImSDK.instance.YunVaLogOut();
        }
    }
    // 开始录音
    public string yayaStart()
    {
        Debug.Log("正在录音中。。。。。。");
        var fileName = DateTime.Now.ToFileTime().ToString();
        var filePath = string.Format("{0}/{1}.amr", Application.persistentDataPath, fileName);
        Debug.Log("FilePath:" + filePath);
        YunVaImSDK.instance.RecordStartRequest(filePath, 1);
        return fileName + ".amr";
    }

    // 停止录音
    public void yayaStop(bool isSend, uint time)
    {
        Debug.Log("停止录音.........");

        YunVaImSDK.instance.RecordStopRequest((data1) =>
        {
            Debug.Log("停止录音返回:" + data1.strfilepath);
        },
        (data2) =>
        {
            string voiceName = data2.fileurl;
            Debug.Log("上传返回:" + voiceName);
            if (isSend)
            {
                AudioManager.Instance.SendTalkServer(voiceName, time);
            }
        },
        (data3) =>
        {
            Debug.Log("识别返回:" + data3.text);
        });

    }
    public void yayaPlay(string recordPath)
    {
        Debug.Log("播放语音.........");
        YunVaImSDK.instance.RecordStartPlayRequest("", recordPath, DateTime.Now.ToFileTime().ToString(), (data2) =>
        {
            if (data2.result == 0)
            {
                Debug.Log("播放成功");
            }
            else
            {
                Debug.Log("播放失败");
            }
        });
    }

    /**************************************************** 将网络图片存到本地 *********************************************************/
    public void saveSharePic()
    {
        Texture2D _texture = ResourceManager.Instance.LoadAsset<Texture2D>("TexturePic/share");

        byte[] bytes = _texture.EncodeToPNG();
        string filename = Application.persistentDataPath + "/share.png";
        Debug.Log("------saveSharePic-----" + filename);

        File.WriteAllBytes(filename, bytes);
    }
}
