using UnityEngine;
using System.Collections;
using ZhiWa;
using MsgContainer;
using Net;
using DNotificationCenterManager;
using System.Collections.Generic;
using DG.Tweening;
using Haoyun.Utils;
using UnityEngine.UI;
using HaoyunFramework;
public class WaitGameContext : BaseContext
{
    public WaitGameContext()
    {
        ViewType = UIType.WaitGameView;
    }
}
public class UIWaitGameView : BasesView
{
    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGUANDAG_ROOM_LEAVE, UpdateLevelRoom);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoomJoin_other_join, UpdateOtherJoin);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoom_other_leave, UpdateOtherLeaveJoin);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ESendCanReadyGame, UpdateReadyGame);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ESameIpPlayer, UpdateSameIpPlayer);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EApplyChangeSeat, UpdateApplyChangeSeat);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EAgreeChangeSeat, UpdateAgreeChangeSeat);
        //道具
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldNotEnough, UpdateGoldNotEnough);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUseProp, UseProps);
        //聊天 使用表情
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUseEmoticon, UseEmoticon);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ETalkNotify, UseTalk);

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, ReconnectToPlaying);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDissmisRoomResultSuc, DisRoomResultSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EResetWaitGameView, ResetConnectView);

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, ResetRefreshView); //用于断线重连，刷新等候玩家的数据  
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldReadySuc, GoldReadySuc); //金币场准备成功 
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldOutSuc, GoldOutSuc); //金币场退出成功 

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOnLinePlayer, UpdateOnLinePlayer); //玩家上线
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOfflinePlayer, UpdateOffLinePlayer); //玩家下线
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGUANDAG_ROOM_LEAVE, UpdateLevelRoom);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoomJoin_other_join, UpdateOtherJoin);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoom_other_leave, UpdateOtherLeaveJoin);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ESendCanReadyGame, UpdateReadyGame);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ESameIpPlayer, UpdateSameIpPlayer);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EApplyChangeSeat, UpdateApplyChangeSeat);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EAgreeChangeSeat, UpdateAgreeChangeSeat);
        //道具
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldNotEnough, UpdateGoldNotEnough);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUseProp, UseProps);
        //使用表情
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUseEmoticon, UseEmoticon);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ETalkNotify, UseTalk);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, ReconnectToPlaying);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDissmisRoomResultSuc, DisRoomResultSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EResetWaitGameView, ResetConnectView);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, ResetRefreshView); //用于断线重连，刷新等候玩家的数据       
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldReadySuc, GoldReadySuc); //金币场准备成功 
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldOutSuc, GoldOutSuc); //金币场退出成功 

        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EOnLinePlayer, UpdateOnLinePlayer); //玩家上线
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EOfflinePlayer, UpdateOffLinePlayer); //玩家下线
        StopAllCoroutines();
    }

    [SerializeField]
    Text roomInfo;
    [SerializeField]
    RawImage selfHeadSpr;
    [SerializeField]
    RawImage rightHeadSpr;
    [SerializeField]
    RawImage topHeadSpr;
    [SerializeField]
    RawImage leftHeadSpr;

    [SerializeField]
    Text selfName;
    [SerializeField]
    Text leftName;
    [SerializeField]
    Text rightName;
    [SerializeField]
    Text topName;

    [SerializeField]
    Image timeSpr;
    [SerializeField]
    GameObject waitgameTweenContainer;
    //换座位三个按钮
    [SerializeField]
    Image leftBtn;
    [SerializeField]
    Image rightBtn;
    [SerializeField]
    Image topBtn;
    [SerializeField]
    Image inviteFriendBtn;//邀请好友
    [SerializeField]
    Image voteDissmissBtn;//解散房间

    //聊天的两个按钮
    [SerializeField]
    Image chatOfTextBtn;
    [SerializeField]
    Image chatOfVoiceBtn;

    [SerializeField]
    GameObject leftContainer;
    [SerializeField]
    GameObject RightContainer;
    [SerializeField]
    GameObject TopContainer;
    [SerializeField]
    GameObject SeContainer;
    [SerializeField]
    Text versionLab;

    [SerializeField]
    Image leaveRoomSpr;

    [SerializeField]
    RawImage bgTex;
    [SerializeField]
    GameObject leftInfoIconBtn;
    [SerializeField]
    GameObject leftInfoContainer;
    [SerializeField]
    Text gamNumLab;//局数

    //自己的准备按钮，其他人的准备提示

    [SerializeField]
    Image selfReadyBtn;

    [SerializeField]
    Image selfReadySpr;
    [SerializeField]
    GameObject goldChangeTableBtn;

    [SerializeField]
    GameObject copyRoomCodeBtn;

    Sequence mySeque = null;
    bool IsShowLeftInfo = false;


    bool isStartTime = false;
    float readyTime = 0f;
    float waitTime;
    float curEndTime = 0;
    // Use this for initialization
    void Start()
    {
        AudioManager.Instance.IsPlayBackgroundAudio(false);
        if (!RoomInfo.Instance.IsZhuandanGameType)
        {
            readyTime = GlobalData.mReadyGameWaitTime;
        }
        waitTime = GlobalData.mStartGameWaitTime;
        PlayingGameInfo.Instance.mReconnectId = 3;
        EventTriggerListener.Get(leftBtn.gameObject).onClick = OnExchangeClick;
        EventTriggerListener.Get(rightBtn.gameObject).onClick = OnExchangeClick;
        EventTriggerListener.Get(topBtn.gameObject).onClick = OnExchangeClick;

        EventTriggerListener.Get(inviteFriendBtn.gameObject).onClick = OnInviteFriendClick;
        EventTriggerListener.Get(chatOfTextBtn.gameObject).onClick = OnChatTextBtn;

        EventTriggerListener.Get(voteDissmissBtn.gameObject).onClick = OnVoteBtnClick;
        EventTriggerListener.Get(leftHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(rightHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(topHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(selfHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(leftInfoIconBtn).onClick = OnLeftInfoClick;
        EventTriggerListener.Get(chatOfVoiceBtn.gameObject).onPress = OnChatVoiceBtn;
        EventTriggerListener.Get(goldChangeTableBtn).onClick = OnChangeTableClick;
        EventTriggerListener.Get(selfReadyBtn.gameObject).onClick = OnReadyStartGameClick;
        EventTriggerListener.Get(copyRoomCodeBtn.gameObject).onClick = OnCopyRoomCodeClick;


        RefreshGoldData(true);
        InitTween();
        WaitingTween();
        CheckGameType();
        LoadView();
        RefreshSelfData();
        RefreshRoomPlayer();

        CheckStartPlaying();
        OnLeftInfoClick(leftInfoIconBtn);
    }
    /// <summary>
    /// 换桌
    /// </summary>
    void OnChangeTableClick(GameObject gm)
    {
        Debug.Log("换桌");
        GoldFiledManager.Instance.SendExchangeRoomServer();
    }
    void CheckStartPlaying()
    {
        if (!GoldFiledManager.Instance.mIsGoldFiled)
        {
            ArgsSameIpPlayer args = RoomInfo.Instance.mSameIpArgs;
            if (args != null && args.list.Count == 4)
            {
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESameIpPlayer, args);
            }
            else if (RoomInfo.Instance.GetRealPlayerCount() == 3)
            {
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESendCanReadyGame);
            }
        }

    }

    IEnumerator AutoReconnect()
    {
        while (true)
        {
            yield return new WaitForSeconds(8.0f);
            if (!ContextManager.Instance.IsContains(UIType.SameIpView.Name))
            {
                PlayingGameInfo.Instance.SendReconnectServer();
            }
        }
    }

    void InitTween()
    {
        mySeque = DOTween.Sequence();
        mySeque.Append(leftInfoContainer.transform.DOLocalMove(leftInfoContainer.transform.localPosition + new Vector3(251, 0, 0), 0.5f));
        mySeque.SetAutoKill(false);
        mySeque.Pause();
    }

    void OnLeftInfoClick(GameObject g)
    {
        SpriteState spriteState = g.GetComponent<Button>().spriteState;
        string sprName = "";
        if (!IsShowLeftInfo)
        {
            mySeque.PlayForward();
            g.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "pk_btn_foward2");
            sprName = "pk_btn_foward2" + "_click";
        }
        else
        {
            mySeque.PlayBackwards();
            g.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "pk_btn_foward1");
            sprName = "pk_btn_foward1" + "_click";
        }
        spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EPlaying, sprName);
        IsShowLeftInfo = !IsShowLeftInfo;
    }

    void GoldReadySuc(LocalNotification e)
    {
        ArgsPlayerList args = e.param as ArgsPlayerList;
        if (args != null)
        {
            if (args.playerIdList.Contains(PlayerInfo.Instance.mPlayerPid))
            {
                RefreshGoldData(false);
            }

            RefreshOtherPlayerReadyStatus(args.playerIdList);

        }
    }


    void RefreshOtherPlayerReadyStatus(List<ulong> playerList)
    {
        //left
        var leftId = RoomInfo.Instance.GetPlayerIdByPos(EPlayerPositionType.ELeft);
        leftHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = playerList.Contains(leftId);

        //right
        var rightId = RoomInfo.Instance.GetPlayerIdByPos(EPlayerPositionType.ERight);
        rightHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = playerList.Contains(rightId);
        //top

        var topId = RoomInfo.Instance.GetPlayerIdByPos(EPlayerPositionType.ETop);
        topHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = playerList.Contains(topId);
    }


    void ClearPlayerReadyStatus(EPlayerPositionType posType)
    {
        switch (posType)
        {
            case EPlayerPositionType.ELeft:
                leftHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = false;
                break;
            case EPlayerPositionType.ERight:
                rightHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = false;
                break;
            case EPlayerPositionType.ETop:
                topHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = false;
                break;
        }
    }

    void ShowSetReady(Image image, bool isShow)
    {
        image.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = isShow;
    }

    void GoldOutSuc(LocalNotification e)
    {
        ArgsPlayerId args = e.param as ArgsPlayerId;
        if (args != null)
        {
            var playerId = args.playerId;
            if (playerId == PlayerInfo.Instance.mPlayerPid)
            {
                UIManagers.Instance.EnqueueTip("退出房间成功");
                ContextManager.Instance.Pop(curContext);
            }
            else
            {
                AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameOtherLeaveRoom);
                RefreshRoomPlayer(); //刷新
            }
        }
    }



    void CheckGameType()
    {
        bool isGold = GoldFiledManager.Instance.mIsGoldFiled;
        voteDissmissBtn.gameObject.SetActive(!isGold);
        inviteFriendBtn.gameObject.SetActive(!isGold);
        //金币的不显示
        selfReadyBtn.gameObject.SetActive(isGold);
        goldChangeTableBtn.SetActive(isGold);
        selfReadySpr.enabled = false;
        if (!isGold)
        {
            var isShow = !RoomInfo.Instance.IsZhuandanGameType;
            topBtn.gameObject.SetActive(isShow);//换座位按钮
            leftBtn.gameObject.SetActive(isShow);
            rightBtn.gameObject.SetActive(isShow);
        }
        else
        {
            //显示准备按钮
            SetGoldReadyStatus(false);
            copyRoomCodeBtn.SetActive(false);
        }
    }


    void SetGoldReadyStatus(bool isReady)
    {
        selfReadyBtn.gameObject.SetActive(!isReady);
        goldChangeTableBtn.SetActive(!isReady);
        selfReadySpr.enabled = isReady;
    }

    /// <summary>
    /// 交换按钮
    /// </summary>
    /// <param name="btn"></param>
    void OnExchangeClick(GameObject btn)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        var roomInfo = RoomInfo.Instance;

        ulong toPlayerId = 0;
        if (btn.name == leftBtn.name)
        {
            if (roomInfo.GetPlayerInfoByPos(EPlayerPositionType.ELeft) != null)
            {
                toPlayerId = roomInfo.GetPlayerInfoByPos(EPlayerPositionType.ELeft).player_id;
            }
        }
        else if (btn.name == rightBtn.name)
        {
            if (roomInfo.GetPlayerInfoByPos(EPlayerPositionType.ERight) != null)
            {
                toPlayerId = roomInfo.GetPlayerInfoByPos(EPlayerPositionType.ERight).player_id;
            }
        }
        else if (btn.name == topBtn.name)
        {
            if (roomInfo.GetPlayerInfoByPos(EPlayerPositionType.ETop) != null)
            {
                toPlayerId = roomInfo.GetPlayerInfoByPos(EPlayerPositionType.ETop).player_id;
            }
        }
        RoomInfo.Instance.SendApplyChangeSeat(toPlayerId);
    }

    //邀请好友
    void OnInviteFriendClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        var pid = PlayerInfo.Instance.mPlayerPid;
        var roomCode = SDKManager.Instance.shareRoomCode;
        var time = TimeUtils.ConvertToTime(System.DateTime.Now).ToString();
        string url = GlobalData.WeChatShareUrl + "?pid=" + pid + "&roomnumber=" + roomCode + "&enter_time_out=" + time;
        //Debug.Log("url:" + url);
        var playerName = PlayerInfo.Instance.mPlayerData.name;
        string title = GlobalData.mGameName + " 房间号:" + roomCode;
        string descripition = playerName + " 邀请你一起来掼蛋! " + RoomInfo.Instance.GetgameTypeString() + "  " + RoomInfo.Instance.GetPayTypeString();
        SDKManager.Instance.WeChatShareLink(title, descripition, url);
    }

    /// <summary>
    /// 文字聊天按钮
    /// </summary>
    /// <param name="g"></param>
    void OnChatTextBtn(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new EmoticonViewContext());
    }

    GameObject voiceGm = null;
    /// <summary>
    /// 语音聊天
    /// </summary>
    /// <param name="g"></param>
    void OnChatVoiceBtn(GameObject g, bool isPress)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        OpenVoiceView(isPress);
    }

    void OnReadyStartGameClick(GameObject g)
    {
        GoldFiledManager.Instance.SendReadyGoldStartGameServer();
    }


    void OnCopyRoomCodeClick(GameObject g)
    {
        string roomCode = string.Format("房间号: *{0}*\n", RoomInfo.Instance.mRoomCode.ToString());
        string type = "玩法:" + RoomInfo.Instance.GetgameTypeString() + "\n";
        string curCount = "当前人数:" + (RoomInfo.Instance.GetRealPlayerCount() + 1) + "\n";
        string finalInfo = roomCode + type + curCount;
        string tip = "(复制信息打开app自动进入房间)";
        finalInfo += tip;
        Debug.Log("房间信息：" + finalInfo);
        GlobalData.CopyTextFromLab(finalInfo);
        SDKManager.Instance.openWechat();
    }

    void OpenVoiceView(bool isHover)
    {
        if (isHover)
        {
            if (voiceGm != null)
            {
                voiceGm.GetComponent<UIVoiceView>().EndVoice();
                voiceGm = null;
            }
            voiceGm = UIManagers.Instance.GetSingleUI(UIType.VoiceView);
            voiceGm.transform.SetParent(this.transform);
        }
        else
        {
            if (voiceGm != null)
                voiceGm.GetComponent<UIVoiceView>().EndVoice();
            voiceGm = null;
        }
    }

    IEnumerator WaitTabs(float waitTime, bool isHover)
    {
        yield return new WaitForSeconds(waitTime);//等待0.3秒打开录音界面
        OpenVoiceView(isHover);
    }

    void LoadView()
    {
        UIManagers.Instance.GetSingleUI(UIType.SwitchBtnFunctionView).transform.SetParent(this.transform);

        if (GoldFiledManager.Instance.mIsGoldFiled)
        {
            SetGoldReadyStatus(false);
        }

    }

    float curTime = 0;
    void Update()
    {
        if (isStartTime)
        {
            waitgameTweenContainer.SetActive(false);
            curEndTime += Time.deltaTime;
            timeSpr.enabled = true;
            int endTime = Mathf.Clamp((int)(readyTime - curEndTime), 1, (int)GlobalData.mReadyGameWaitTime);
            timeSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "font_number_" + endTime);
            if (curEndTime > readyTime - 0.1f)
            {
                isStartTime = false;
                curEndTime = 0;

                SendToServerStartGame();
            }
        }
        else
        {
            timeSpr.enabled = false;
        }

        if (isStartDiss)
        {
            curTime += Time.deltaTime;
            if (curTime > GlobalData.mDismissRoomCDTime)
            {
                isStartDiss = false;
                curTime = 0;
            }
        }
    }
    /// <summary>
    /// 向服务器发送正式的开始发牌游戏请求
    /// </summary>
    void SendToServerStartGame()
    {
        //   ContextManager.Instance.Pop(curContext);
        PlayingGameInfo.Instance.changeToEPlaying();
        PlayingGameInfo.Instance.SendStartGame();
    }

    void RefreshSelfData()
    {
        selfName.text = PlayerInfo.Instance.mPlayerData.name;
        selfHeadSpr.texture = DataManager.Instance.GetTextureByName(ETextureName.EHeadTex1);
        if (GoldFiledManager.Instance.mIsGoldFiled)
        {
            roomInfo.text = string.Format("房间号:{0}\n {1}场\n底分:{2}", RoomInfo.Instance.mRoomCode, GoldFiledManager.Instance.GetCurGroundType, GoldFiledManager.Instance.GetMinScore);
        }
        else
            roomInfo.text = "房间号: " + RoomInfo.Instance.mRoomCode.ToString() + '\n' + RoomInfo.Instance.GetgameTypeString();
        versionLab.text = "版本号:" + GlobalData.mVersion;
        string sprPath = RoomInfo.Instance.IsCreater() ? "btn_pk_06_font1" : "btn_pk_06_font2";
        voteDissmissBtn.transform.GetChild(0).GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, sprPath);
    }
    void RefreshRoomPlayer()
    {

        var dic = RoomInfo.Instance.mPlayersDic;
        //右，上，左
        foreach (var t in dic)
        {
            SetPlayerData(t.Key, t.Value);
        }

    }

    /// <summary>
    /// 展示新玩家加入烟雾动画
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isShow"></param>
    void CheckShowJoinTween(Transform parent, bool isCurEnable, bool isShow)
    {
        //获取当前位置头像是否有，作出对比，如果，是出现，则显示动画
        if (!isCurEnable && isShow)
        {
            TweenManager.Instance.PlaySmoke(parent);
        }
    }

    void SetPlayerData(EPlayerPositionType posType, MsgPlayerInfo args)
    {
        ClearPlayerReadyStatus(posType);
        bool isShow = args == null || args.player_id == 0 ? false : true;

        if (posType == EPlayerPositionType.ERight)
        {
            CheckShowJoinTween(rightHeadSpr.transform, RightContainer.activeSelf, isShow);
            RightContainer.SetActive(isShow);
            if (isShow)
            {
                StartCoroutine(GlobalData.GetHeadTextureByIdx(rightHeadSpr, args.head_portrait));
                rightName.text = args.name;
            }
        }
        else if (posType == EPlayerPositionType.ETop)
        {
            CheckShowJoinTween(topHeadSpr.transform, TopContainer.activeSelf, isShow);
            TopContainer.SetActive(isShow);
            if (isShow)
            {
                StartCoroutine(GlobalData.GetHeadTextureByIdx(topHeadSpr, args.head_portrait));
                topName.text = args.name;
            }
        }
        else
        {
            CheckShowJoinTween(leftHeadSpr.transform, leftContainer.activeSelf, isShow);
            leftContainer.SetActive(isShow);
            if (isShow)
            {
                StartCoroutine(GlobalData.GetHeadTextureByIdx(leftHeadSpr, args.head_portrait));
                leftName.text = args.name;
            }
        }
    }

    void UpdateOtherLeaveJoin(LocalNotification e)
    {
        AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameOtherLeaveRoom);
        ResetStopGameTime();
        RefreshRoomPlayer(); //刷新显示
    }

    void UpdateOtherJoin(LocalNotification e)
    {
        AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameJoinRoomPlayer);
        RefreshRoomPlayer(); //刷新显示
    }

    void UpdateReadyGame(LocalNotification e)
    {
        //倒计时开始，倒计时结束后向服务器发送开始游戏请求.
        isStartTime = true;
    }

    void UpdateLevelRoom(LocalNotification e)
    {
        ContextManager.Instance.Pop(this.curContext);
    }

    public void UpdateApplyChangeSeat(LocalNotification e)
    {
        ArgsChangeSeatInfo args = e.param as ArgsChangeSeatInfo;
        //刷新倒计时 重置
        if (RoomInfo.Instance.GetRealPlayerCount() == 3)
        {
            ResetStopGameTime();
        }
        if (args != null)
        {
            string strInfo = "";
            if (args.toId == PlayerInfo.Instance.mPlayerPid) //被要求换座位
            {
                string applyName = RoomInfo.Instance.GetPlayerInfoById(args.applyId).name;
                strInfo = string.Format("{0}申请和你换座位", applyName);
                UIManagers.Instance.ShowConfirmBox(
                    strInfo, "同意", "拒绝", () => RoomInfo.Instance.SendAgreeChangeSeat(args.applyId, 0)
                    , () => RoomInfo.Instance.SendAgreeChangeSeat(args.applyId, 1));
            }
            else
            {
                string applyName = RoomInfo.Instance.GetPlayerInfoById(args.applyId).name;
                string toName = RoomInfo.Instance.GetPlayerInfoById(args.toId).name;
                strInfo = string.Format("{0}申请和{1}换座位", applyName, toName);
            }
            UIManagers.Instance.EnqueueTip(strInfo);
        }

    }

    /// <summary>
    /// 重置停止刷新时间
    /// </summary>
    void ResetStopGameTime()
    {
        curEndTime = 0;
        isStartTime = false;
    }

    /// <summary>
    /// 重置开始刷新时间
    /// </summary>
    void ResetStartGameTime()
    {
        curEndTime = 0;
        isStartTime = true;
    }
    public void UpdateAgreeChangeSeat(LocalNotification e)
    {
        var roomInfo = RoomInfo.Instance;
        ArgsChangeSeatInfo args = e.param as ArgsChangeSeatInfo;
        if (RoomInfo.Instance.GetRealPlayerCount() == 3)
        {
            ResetStartGameTime();
        }
        if (args != null)
        {
            string strInfo = "";
            if (args.result == 0) //同意
            {
                string applyName = RoomInfo.Instance.GetPlayerInfoById(args.applyId).name;
                string toName = RoomInfo.Instance.GetPlayerInfoById(args.toId).name;
                strInfo = string.Format("{0}同意和{1}换座位", toName, applyName);
                var applyPos = roomInfo.GetPlayerPosById(args.applyId);
                var toPos = roomInfo.GetPlayerPosById(args.toId);
                CheckShowChangeSeatTween(applyPos, toPos); //换座位动画
                RoomInfo.Instance.ChangeTwoPlayerInfo(applyPos, toPos);
                RefreshRoomPlayer();


            }
            else
            {
                if (args.applyId == PlayerInfo.Instance.mPlayerPid)
                {
                    string toName = RoomInfo.Instance.GetPlayerInfoById(args.toId).name;
                    strInfo = string.Format("{0}拒绝和你换座位", toName);
                }
                else
                {
                    string applyName = RoomInfo.Instance.GetPlayerInfoById(args.applyId).name;
                    string toName = RoomInfo.Instance.GetPlayerInfoById(args.toId).name;
                    strInfo = string.Format("{0}拒绝和{1}换座位", toName, applyName);
                }
            }

            UIManagers.Instance.EnqueueTip(strInfo);
        }
    }

    /// <summary>
    /// 换座位动画
    /// </summary>
    /// <param name="applyPos"></param>
    /// <param name="toPos"></param>
    void CheckShowChangeSeatTween(EPlayerPositionType applyPos, EPlayerPositionType toPos)
    {
        //var sprite = GetRootContainerByPlayerPos(applyPos);
        //Sequence mySequence = DOTween.Sequence();
        //mySequence.Append(DOTween.To(() => sprite.color, x => sprite.color = x, new Color(1, 1, 1, 0), 0.5f));
        //mySequence.Append(DOTween.To(() => sprite.color, x => sprite.color = x, new Color(1, 1, 1, 1), 0.5f));
        //mySequence.Play();

        //var spriteTo = GetRootContainerByPlayerPos(toPos);
        //Sequence mySequenceTo = DOTween.Sequence();
        //mySequenceTo.Append(DOTween.To(() => spriteTo.color, x => spriteTo.color = x, new Color(1, 1, 1, 0), 0.5f));
        //mySequenceTo.Append(DOTween.To(() => spriteTo.color, x => spriteTo.color = x, new Color(1, 1, 1, 1), 0.5f));
        //mySequenceTo.Play();
        //换座位动画改为和新玩家加入动画一样
        CheckShowJoinTween(GetHeadSpr(applyPos), false, true);
        CheckShowJoinTween(GetHeadSpr(toPos), false, true);
    }

    Transform GetHeadSpr(EPlayerPositionType posType)
    {
        switch (posType)
        {
            case EPlayerPositionType.ELeft:
                return leftHeadSpr.transform;
            case EPlayerPositionType.ERight:
                return rightHeadSpr.transform;
            case EPlayerPositionType.ETop:
                return topHeadSpr.transform;
            case EPlayerPositionType.ESelf:
                return selfHeadSpr.transform;
        }
        return selfHeadSpr.transform;
    }

    IEnumerator LoadSameIpView(LocalNotification args)
    {
        yield return new WaitForSeconds(0.2f);
        ContextManager.Instance.Push(new SameIpViewContext());
        GameObject gm = UIManagers.Instance.GetSingleUI(UIType.SameIpView);
        ArgsSameIpPlayer msg = args.param as ArgsSameIpPlayer;

        Debug.Log("Texture---------");
        Debug.Log(leftHeadSpr.mainTexture);
        Debug.Log(rightHeadSpr.mainTexture);
        Debug.Log(topHeadSpr.mainTexture);
        Debug.Log(selfHeadSpr.mainTexture);
        gm.GetComponent<UISameIpView>().SetInfo(args, () => UpdateReadyGame(null), () =>
        {
            if (RoomInfo.Instance.IsCreater()) //房主,且不是aa的情况下，才凸显出房主的消息
            {
                TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_OUT_ROOM_BY_CREATER);
            }
            else
            {
                TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_OUT_ROOM_BY_PLAYER);
            }
        });
    }


    void UpdateOnLinePlayer(LocalNotification e)
    {
        ArgsOfflinePlayer args = e.param as ArgsOfflinePlayer;
        if (args != null)
        {
            var playerId = args.playerId;
            var playerPos = RoomInfo.Instance.GetPlayerPosById(playerId);
            ShowOnLinePlayer(playerPos);
        }
    }

    void UpdateOffLinePlayer(LocalNotification e)
    {
        ArgsOfflinePlayer args = e.param as ArgsOfflinePlayer;
        if (args != null)
        {
            var playerId = args.playerId;
            var playerPos = RoomInfo.Instance.GetPlayerPosById(playerId);
            ShowOffLinePlayer(playerPos);
        }
    }


    void ShowOnLinePlayer(EPlayerPositionType posType)
    {
        switch (posType)
        {
            case EPlayerPositionType.ELeft:
                leftHeadSpr.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case EPlayerPositionType.ERight:
                rightHeadSpr.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case EPlayerPositionType.ETop:
                topHeadSpr.transform.GetChild(0).gameObject.SetActive(false);
                break;
        }
    }

    void ShowOffLinePlayer(EPlayerPositionType posType)
    {
        switch (posType)
        {
            case EPlayerPositionType.ELeft:
                leftHeadSpr.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case EPlayerPositionType.ERight:
                rightHeadSpr.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case EPlayerPositionType.ETop:
                topHeadSpr.transform.GetChild(0).gameObject.SetActive(true);
                break;
        }
    }
    void UpdateSameIpPlayer(LocalNotification args)
    {
        Debug.Log("SameIpView");
        StartCoroutine(LoadSameIpView(args));
    }
    /// <summary>
    /// 离开
    /// </summary>
    /// <param name="vote"></param>
    void OnVoteBtnClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        // RoomInfo.Instance.LeaveWaitRoom();
        if (GameManager.Instance.mCurGameStatus == EGameStatus.EPlaying) //战斗状态下的退出是申请解散功能
        {
            UIManagers.Instance.ShowConfirmBox("确认发起解房间吗？", "确认", "取消", sureDissmissRoomClickEvent, null);
        }
        else //Home
        {
            UIManagers.Instance.ShowConfirmBox("确认离开房间吗？", "确认", "取消", sureLeaveClickEvent, null);
        }
    }

    bool isStartDiss = false;
    void sureDissmissRoomClickEvent()
    {
        if (isStartDiss)
        {
            UIManagers.Instance.EnqueueTip(string.Format("还有{0}秒，才能发起解散", (int)(GlobalData.mDismissRoomCDTime - curTime)));
            return;
        }
        PlayingGameInfo.Instance.SendDissRoomApplyMsg();
        isStartDiss = true;
    }

    void sureLeaveClickEvent()
    {
        RoomInfo.Instance.SendExitRoomServer();
    }

    /// <summary>
    /// 等待中。。。 动画
    /// </summary>
    void WaitingTween()
    {
        int childCount = 6;
        List<Transform> listWaitTween = new List<Transform>();
        for (int i = 0; i < childCount; i++)
        {
            listWaitTween.Add(waitgameTweenContainer.transform.GetChild(i));
        }

        Sequence mySequence = DOTween.Sequence();
        for (int i = 0; i < listWaitTween.Count; i++)
        {
            mySequence.Append(listWaitTween[i].DOLocalMoveY(17, 0.4f).SetEase(Ease.Linear));
            if (i > 0)
            {
                mySequence.Join(listWaitTween[i - 1].DOLocalMoveY(0, 0.2f).SetEase(Ease.InQuad));
            }
            //else
            //    mySequence.Join(listWaitTween[listWaitTween.Count-1].DOLocalMoveY(0, 0.1f).SetEase(Ease.InQuad));
        }
        mySequence.SetLoops(-1);//-1表示无限循环

    }

    void UpdateGoldNotEnough(LocalNotification e)
    {
        ContextManager.Instance.Push(new ShopContext());
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="g"></param>
    public void OnHeadClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new ToolsViewContext());
        GameObject toolsGm = UIManagers.Instance.GetSingleUI(UIType.PropView);
        uint pid = RoomInfo.Instance.GetIdByName(g.name);
        MsgPlayerInfo data = RoomInfo.Instance.GetPlayerInfoById(pid);
        toolsGm.GetComponent<UIToolsView>().SetData(data, pid);
    }

    void UseProps(LocalNotification e)
    {
        ArgsPropsInfo propInfo = e.param as ArgsPropsInfo;
        EPlayerPositionType startType = RoomInfo.Instance.GetPlayerPosById(propInfo.action_id);
        EPlayerPositionType endType = RoomInfo.Instance.GetPlayerPosById(propInfo.target_id);
        Transform startPos = GetHeadPos(startType);

        if (propInfo.target_id == 0)
            endType = EPlayerPositionType.ESelf;

        Transform endPos = GetHeadPos(endType)/*.localPosition*/;
        TweenManager.Instance.PlayPropsAnimation(propInfo, this.transform, startPos, endPos);
        PlayerInfo.Instance.UpdateGold(0);//扣除当前的玩家的金币（暂时不扣除金币）
    }

    void UseEmoticon(LocalNotification e)
    {
        Debug.Log("表情发送成功");
        if (e == null)
        {
            return;
        }
        ArgsEmoticonInfo args = e.param as ArgsEmoticonInfo;
        EPlayerPositionType type = EPlayerPositionType.ENull;
        //表情
        if (args.action_id == PlayerInfo.Instance.mPlayerPid)
        {
            type = EPlayerPositionType.ESelf;
        }
        else
        {
            type = RoomInfo.Instance.GetPlayerPosById(args.action_id);
        }
        if (args.emoticonId != 0)
        {
            GameObject emoticonTweenGm = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(GlobalData.mLocalEmoticonPrefabPath + TweenManager.Instance.GetEmotcionPrefabName(args.emoticonId)));
            emoticonTweenGm.name = TweenManager.Instance.GetEmotcionPrefabName(args.emoticonId);
            if (args.action_id == PlayerInfo.Instance.mPlayerPid)
            {
                type = EPlayerPositionType.ESelf;
                emoticonTweenGm.transform.SetParent(selfHeadSpr.transform);
            }
            else
            {
                emoticonTweenGm.transform.SetParent(this.GetHeadPos(type));
            }
            emoticonTweenGm.transform.localScale = Vector3.one * 0.6f;
            emoticonTweenGm.transform.localPosition = Vector3.zero + new Vector3(0, -13, 0);
            PlayerInfo.Instance.AddNewEmoticon(type, emoticonTweenGm, true);
            TweenManager.Instance.mEmoticonList.Add(emoticonTweenGm);
            return;
        }
        //语音和文字
        GameObject gm = UIManagers.Instance.GetSingleUI(UIType.ChatMessageTip);
        gm.transform.localScale = Vector3.one;
        gm.transform.SetParent(GetHeadPos(type));
        gm.transform.localPosition = Vector3.zero;
        PlayerInfo.Instance.AddNewEmoticon(type, gm, false);
        if (args.message != null && args.message.Length != 0)
        {
            StartCoroutine(gm.GetComponent<UIChatMessagTipView>().WaitOneDelta(args.message, null, type));
        }
    }

    public Transform GetHeadPos(EPlayerPositionType type)
    {
        switch (type)
        {
            case EPlayerPositionType.ELeft:
                return leftHeadSpr.transform.parent.transform;
            case EPlayerPositionType.ERight:
                return rightHeadSpr.transform.parent.transform;
            case EPlayerPositionType.ETop:
                return topHeadSpr.transform.parent.transform;
            case EPlayerPositionType.ESelf:
                return selfHeadSpr.transform.parent.transform;
        }
        return null;
    }

    void ReconnectToPlaying(LocalNotification e)
    {
        PlayingGameInfo.Instance.mReconnectId = 2; //断线重连
        GameManager.Instance.SwitchGameStatus(EGameStatus.EPlaying);
    }

    void ResetConnectView(LocalNotification e)
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

    void ResetRefreshView(LocalNotification e)
    {
        Debug.Log("ResetRefreshView");
        CheckGameType();
        RefreshRoomPlayer();
        RefreshSelfData();
        ContextManager.Instance.Pop(UIType.SameIpView.Name);
        if (GoldFiledManager.Instance.mIsGoldFiled)
        {
            return;
        }
        ArgsSameIpPlayer args = RoomInfo.Instance.mSameIpArgs;
        if (args != null && args.list.Count == 4)
        {
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESameIpPlayer, args);
        }
        else if (RoomInfo.Instance.GetRealPlayerCount() == 3)
        {
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESendCanReadyGame);
        }
    }

    void DisRoomResultSuc(LocalNotification e)
    {
        AudioManager.Instance.IsPlayBackgroundAudio(false);
        GameManager.Instance.ClearCurGame();
        ContextManager.Instance.Pop(UIType.SameIpView.Name);
        ContextManager.Instance.Pop(curContext);
    }

    void UseTalk(LocalNotification e)
    {
        if (e == null)
        {
            Debug.LogError("语音为空");
            return;
        }

        ArgsTalk args = e.param as ArgsTalk;
        Debug.Log("玩家" + args.talkPid + "正在发送语音");
        EPlayerPositionType type = EPlayerPositionType.ENull;
        if (args.talkPid == PlayerInfo.Instance.mPlayerPid)
        {
            type = EPlayerPositionType.ESelf;
        }
        else
        {
            type = RoomInfo.Instance.GetPlayerPosById(args.talkPid);
        }

        GameObject gm = UIManagers.Instance.GetSingleUI(UIType.ChatMessageTip);
        gm.transform.localScale = Vector3.one;
        gm.transform.SetParent(GetHeadPos(type));
        gm.transform.localPosition = Vector3.zero;
        PlayerInfo.Instance.AddNewEmoticon(type, gm, false);
        if (args.voiceName != null && args.voiceName.Length != 0)
        {
            StartCoroutine(gm.GetComponent<UIChatMessagTipView>().WaitOneDelta(null, args.voiceName, type));
        }
    }

    /// <summary>
    /// 金币场玩家状态数据（其他玩个玩家的准备按钮，自己的准备好icon，自己的准备按钮）
    /// </summary>
    void RefreshGoldData(bool isStart)
    {
        var manager = GoldFiledManager.Instance;
        if (!manager.mIsGoldFiled) { return; }
        bool isSelfReady = manager.IsReadyPlayerContains(PlayerInfo.Instance.mPlayerPid);
        if (!isStart)
        {
            SetGoldReadyStatus(isSelfReady);
        }
    }
}
