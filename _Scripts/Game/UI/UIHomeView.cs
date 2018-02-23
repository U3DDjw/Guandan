using UnityEngine;
using System.Collections;
using MsgContainer;
using DNotificationCenterManager;
using Haoyun.Utils;
using System;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;
using HaoyunFramework;
using UnityEngine.EventSystems;

public class HomeContext : BaseContext
{
    public HomeContext()
    {
        ViewType = UIType.HomeView;
    }
}

public class UIHomeView : BasesView
{
    [SerializeField]
    GameObject joinRoomBtn;
    [SerializeField]
    Image createRoomSpr;
    [SerializeField]
    GameObject coinRoomBtn;

    [SerializeField]
    RawImage headSpr;
    [SerializeField]
    Text nameLab;
    [SerializeField]
    Text idLab;
    [SerializeField]
    Text roomCardLab;
    [SerializeField]
    Text goldLab;
    [SerializeField]
    GameObject settingBtn;
    [SerializeField]
    GameObject noticeBtn;
    [SerializeField]
    GameObject ruleBtn;
    [SerializeField]
    GameObject emailBtn;
    [SerializeField]
    GameObject recordBtn;

    [SerializeField]
    GameObject addMoneyBtn;
    [SerializeField]
    GameObject addCoinBtn;

    [SerializeField]
    GameObject headInfoBtn;
    [SerializeField]
    GameObject bindBtn;
    [SerializeField]
    GameObject inviteBtn;
    [SerializeField]
    GameObject bindCardIdBtn;
    [SerializeField]
    GameObject shareBtn;
    [SerializeField]
    GameObject personAnimParent;

    [SerializeField]
    GameObject goldGameBtn;

    [SerializeField]
    GameObject rightContainer;
    GameObject datingNoticContainer;//loadview赋值

  
    void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUpdatePlayerInfo, UpdatePlayerInfo);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUpdateCreateRoomSpr, UpdateCreateRoomSpr);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERefreshEmailBall, RefreshEmailBall);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldViewBack, OnGoldBackClick);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldOutSuc, UpdatePlayerInfo);

        
    }
    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUpdatePlayerInfo, UpdatePlayerInfo);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUpdateCreateRoomSpr, UpdateCreateRoomSpr);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERefreshEmailBall, RefreshEmailBall);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldViewBack, OnGoldBackClick);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldOutSuc, UpdatePlayerInfo);

        Debug.Log("HomeView Destroy");
    }
    void Start()
    {
        EventTriggerListener.Get(joinRoomBtn).onClick = OnJoinRoomClick;
        EventTriggerListener.Get(createRoomSpr.gameObject).onClick = OnCreateRoomClick;
        EventTriggerListener.Get(settingBtn).onClick = OnOpenSettingClick;
        EventTriggerListener.Get(noticeBtn).onClick = OnOpenNoticeClick;
        EventTriggerListener.Get(ruleBtn).onClick = OnOpenRuleClick;
        EventTriggerListener.Get(emailBtn).onClick = OnOpenEmailClick;
        EventTriggerListener.Get(addMoneyBtn).onClick = OnOpenShopClick;
        EventTriggerListener.Get(recordBtn).onClick = OnRecordClick;
        EventTriggerListener.Get(headInfoBtn).onClick = OnEditHeadInfoCLick;
        EventTriggerListener.Get(bindBtn).onClick = OnBindClick;
        EventTriggerListener.Get(inviteBtn).onClick = OnInviteClick;
        EventTriggerListener.Get(addCoinBtn).onClick = OnAddCoinClick;
        EventTriggerListener.Get(shareBtn).onClick = OnShareIconClick;
        EventTriggerListener.Get(coinRoomBtn).onClick = OnCoinRoomClick;
        EventTriggerListener.Get(bindCardIdBtn).onClick = OnCardIdClick;

        LoadView();
        CheckShowCheckCardId();

        UpdatePlayerInfoData();
        CheckReConnect();
        RequestNoReadEmail();
        CheckQuickJoinRoom();
        CheckIsPopingNoticView(XPlayerPrefs.IsLoadNotic);//只执行一次
        AudioManager.Instance.IsPlayBackgroundAudio(true);
        CheckGameModeView();

        TestHotFixInHomeView();
        InitAnimation();//部分动画的初始与否取决bindBtn和InviteBtn是否active=true;

        CheckShowCoinBtn();

    }

    /// <summary>
    /// 验证是否需要弹出身份验证
    /// </summary>
    void CheckShowCheckCardId()
    {
       if(PlayerInfo.Instance.mIsSfzFlg) 
        {
            ContextManager.Instance.Push(new CheckCardIdContext());
        }

        bindCardIdBtn.gameObject.SetActive(PlayerInfo.Instance.mIsSfzShow);
    }

    void OnTaskBtnClick(GameObject g)
    {
        print("任务按钮点击");
    }
    void CheckShowCoinBtn()
    {
        bool isSj = (PlayerInfo.Instance.mUrlData.iosSj && GameManager.Instance.mGameMode == EGameMode.EAppleOnLine);
        goldGameBtn.SetActive(!isSj);
    }


    void UpdatePlayerInfo(LocalNotification e)
    {
        //刷新数据
        UpdatePlayerInfoData();
    }
    void UpdateJoinRoomSuc(LocalNotification e)
    {
        // if (PlayingGameInfo.Instance.IsConnectWaitingStatus)
        AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameJoinRoomPlayer);
        ContextManager.Instance.Push(new WaitGameContext());
    }
    void UpdateCreateRoomSpr(LocalNotification e)
    {
        string sprName = RoomInfo.Instance.mIsExistWaitGameView ? "dating_btn4" : "dating_btn3";
        createRoomSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, sprName);

        createRoomSpr.GetComponent<Button>().spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EMain, sprName + "_click");
    }
    void RefreshEmailBall(LocalNotification e)
    {
        emailBtn.transform.GetChild(0).GetComponent<Image>().enabled = false;
    }
    void LoadView()
    {
        GameObject gm = UIManagers.Instance.GetSingleUI(UIType.HomeViewDatingNotic);
        gm.transform.SetParent(this.transform);
        datingNoticContainer = gm;
        CheckLoadActivityBtn();
    }
    //
    void SetHomeViewActive(bool isshow)
    {
        headInfoBtn.SetActive(isshow);
        rightContainer.SetActive(isshow);
        personAnimParent.SetActive(isshow);
        if (datingNoticContainer != null)
            datingNoticContainer.SetActive(isshow);
    }
    void OnGoldBackClick(LocalNotification e)
    {
        SetHomeViewActive(true);
    }



        void CheckLoadActivityBtn()
    {
        if (true)  //缺个红包显示条件
        {
            GameObject taskBtn = UIManagers.Instance.GetSingleUI(UIType.homeViewTaskBtn);
            taskBtn.transform.SetParent(this.transform);
            taskBtn.transform.localPosition = Vector3.zero;
            taskBtn.transform.localScale = Vector3.one;
            taskBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(107, 50);
        }
    }
    void InitAnimation()
    {
        if (bindBtn.gameObject.activeSelf)
            TweenManager.Instance.LoadViewSingleAnimation(EViewAnimType.EBindBtn, bindBtn.transform);
        if (inviteBtn.gameObject.activeSelf)
            TweenManager.Instance.LoadViewSingleAnimation(EViewAnimType.EInviteBtn, inviteBtn.transform);

        TweenManager.Instance.LoadViewSingleAnimation(EViewAnimType.EPerson, personAnimParent.transform);
    }
    void OnCoinRoomClick(GameObject g)
    {
        SetHomeViewActive(false);
        //UIManagers.Instance.EnqueueTip("金币房间暂未开放");
        ContextManager.Instance.Push(new GoldFiledContext());
    }

    void OnCardIdClick(GameObject g)
    {
        ContextManager.Instance.Push(new CheckCardIdContext());
    }
    void CheckGameModeView()
    {
        if (GameManager.Instance.mGameMode == EGameMode.EAppleOnLine)
        {
            if (PlayerInfo.Instance.mUrlData.iosSj)
            {
                bindBtn.SetActive(false);
                inviteBtn.SetActive(false);
                //shareBtn.SetActive(false);
            }
            //else
            //{
            //    bindBtn.SetActive(true);
            //    inviteBtn.SetActive(true);
            //    shareBtn.SetActive(true);
            //}
        }
    }
    void CheckQuickJoinRoom()
    {
        SDKManager.Instance.viewStarted = 1;
        var roomCode = SDKManager.Instance.mRoomCode;
        if (roomCode > 0)
        {
            RoomInfo.Instance.SendJoinRoomServer(roomCode);
        }
    }
    void OnInviteClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new InviteContext());
    }
    void OnBindClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new BindPhoneContext());
    }
    void CheckReConnect()
    {
        if (PlayingGameInfo.Instance.IsConnectPlayingStatus)
        {
            GameManager.Instance.SwitchGameStatus(EGameStatus.EPlaying);
        }
        else if (PlayingGameInfo.Instance.IsConnectWaitingStatus)
        {
            if (RoomInfo.Instance.mIsGoildRoom)
            {
                GoldFiledManager.Instance.ReSendEnterGoldFiledServer();
            }
            else
            {
                RoomInfo.Instance.SendJoinRoomServer((int)RoomInfo.Instance.mRoomCode);
            }
        }
    }


    void OnRecordClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new RecordViewContext());
    }
    void RequestNoReadEmail()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("time", TimeUtils.ConvertToTime(DateTime.Now).ToString()); //当前时间戳
        form.AddField("sig", "tobeadded");
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_NoReadEmail;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            Debug.Log("未读邮件列表获取成功" + www.text);
            var list = JsonManager.GetEmailData(www.text);
            emailBtn.transform.GetChild(0).GetComponent<Image>().enabled = list.Count > 0;
            EmailManager.Instance.UpdateNoReadList(list);
        }));

    }
    void RequestServerMoneyAndRoomCard()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("time", TimeUtils.ConvertToTime(DateTime.Now).ToString()); //当前时间戳
        form.AddField("sig", "tobeadded");
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_GetPlayerInfo;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            Debug.Log("未读邮件列表获取成功" + www.text);
            PlayerMoneryInfo info = JsonManager.GetPlayerMoneyInfo(www.text);
            PlayerInfo.Instance.ServerRoomCardNum(info.roomCardNum);
            Debug.Log(">>>>Http请求的当前的金币的房卡数为:！" + info.roomCardNum);
            PlayerInfo.Instance.ServerMoneyNum(info.money);
            Debug.Log(">>>>Http请求的当前的金币的房卡数为:！" + info.money);
        }));
    }
    void OnOpenEmailClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new EmailViewContext());
    }
    /// <summary>
    /// 根据运行的平台来加载商店
    /// </summary>
    /// <param name="gm"></param>
    void OnOpenShopClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        //UIManager.Instance.ShowConfirmBox("商店","确定","取消",null,null);
        if (GameManager.Instance.mGameMode == EGameMode.EAppleOnLine)
        {
            ContextManager.Instance.Push(new ShopContext());
        }
        else if (GameManager.Instance.mGameMode == EGameMode.EFormal)
        {
            ContextManager.Instance.Push(new RoomcardContext());
        }
        else if (GameManager.Instance.mGameMode == EGameMode.EDebug)
        {
            ContextManager.Instance.Push(new ShopContext());
            //ContextManager.Instance.Push(new RoomcardContext());
        }

        //#if UNITY_IPHONE

        //#elif UNITY_ANDROID

        //#elif UNITY_EDITOR
        //         UIManager.Instance.LoadBaseView("ShopView");
        //#else
        //         Debug.LogError("当前平台不可以加载");
        //#endif
    }
    void OnAddCoinClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new BuyCoinContext());
    }
    void OnOpenRuleClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new RuleContext());
    }
    void OnOpenSettingClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new SettingContext());
        UIManagers.Instance.GetSingleUI(UIType.SettingView);
    }
    void OnOpenNoticeClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        CheckIsPopingNoticView();
    }
    void CheckIsPopingNoticView(bool mustPop = true)
    {
        XPlayerPrefs.IsLoadNotic = true;
        if (PlayingGameInfo.Instance.IsConnectPlayingStatus || PlayingGameInfo.Instance.IsConnectWaitingStatus) //断线重连不弹
        {
            return;
        }
        //从缓存目录中加载
        Texture texture = DataManager.Instance.GetTextureByName(ETextureName.ENotic);
        ContextManager.Instance.Push(new NoticContext());
        if (texture != null && !mustPop)
        {
            UIManagers.Instance.GetSingleUI(UIType.NoticeView).GetComponent<UINoticeView>().LoadTexture(texture);
            return;
        }
        else if (texture != null && mustPop)
        {
            UIManagers.Instance.GetSingleUI(UIType.NoticeView).GetComponent<UINoticeView>().LoadTexture(texture);
        }
        else if (texture == null && mustPop)
        {
            UIManagers.Instance.GetSingleUI(UIType.NoticeView).GetComponent<UINoticeView>().LoadTexture(null);
        }
    }
    void SetHeadTexture(Texture texture)
    {
        if (texture == null)
        {
            string url = PlayerInfo.Instance.mPlayerData.headPortrait;
            StartCoroutine(GlobalData.GetHeadTextureByIdx(headSpr, url));
        }
        else
        {
            headSpr.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        }
    }
    void UpdatePlayerInfoData()
    {
        bindBtn.SetActive(PlayerInfo.Instance.mPlayerData.phone == null);//已经绑定手机号
        goldLab.transform.parent.gameObject.SetActive(true);//(PlayerInfo.Instance.mPlayerData.hideFlag != 0);
        SetHeadTexture(DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1));
        nameLab.text = PlayerInfo.Instance.mPlayerData.name;
        idLab.text = PlayerInfo.Instance.mPlayerPid.ToString();
        roomCardLab.text = PlayerInfo.Instance.mPlayerData.roomCardNum.ToString();
        goldLab.text = PlayerInfo.Instance.GetCurGold.ToString();
    }
    //为了避免没有修改的问题,我需要将自己根据性别决定的头像发送到其他人
    void SendToServer()
    {
        string curHeadPortrait = PlayerInfo.Instance.mPlayerData.headPortrait;
        WWWForm form = new WWWForm();
        form.AddField("appId", "HY_NJ_GD");
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("headPortrait", curHeadPortrait);
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_EditHead;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            PlayerInfo.Instance.mPlayerData.sex = PlayerInfo.Instance.GetSexIsMan ? 1 : 2;
        }));
    }
    //这边可以改为第一次加载为Instantiate，第二次加载为setactive，优化不是很大。（后期有空改吧）
    void OnJoinRoomClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (RoomInfo.Instance.mIsExistWaitGameView)
        {
            UIManagers.Instance.EnqueueTip("房间已存在,请先退出房间");
        }
        else
        {
            //UIManager.Instance.LoadView("JoinRoomView");
            ContextManager.Instance.Push(new JoinRoomContext());
        }
    }
    void OnCreateRoomClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (RoomInfo.Instance.mIsExistWaitGameView)
        {
            RoomInfo.Instance.ExistWaitRoom();
        }
        else
        {
            //UIManager.Instance.LoadView("CreateRoomView");
            ContextManager.Instance.Push(new CreateRoomContext());
        }
    }
    void OnEditHeadInfoCLick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new EditPlayerInfoContext());
    }
    void OnShareIconClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (Application.platform == RuntimePlatform.WindowsEditor)//编辑状态点击没效果
        { return; }
        SDKManager.Instance.WeChatShareNativeImage();
    }
    public void TestHotFixInHomeView()
    {
        Debug.Log("this is homeView in Csharp");
        //   UIManagers.Instance.GetSingleUIByPath("View/TestHotView");

    }
}
