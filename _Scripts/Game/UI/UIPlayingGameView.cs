using UnityEngine;
using System.Collections;
using MsgContainer;
using System.Collections.Generic;
using DNotificationCenterManager;
using ZhiWa;
using System.Text;
using DG.Tweening;
using UnityEngine.UI;
using HaoyunFramework;
public class PlayingGameContext : BaseContext
{
    public PlayingGameContext()
    {
        ViewType = UIType.PlayingGameView;
    }
}

public class UIPlayingGameView : BasesView

{
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
    Text scoreLab;//玩家的分数

    [SerializeField]
    Text rightName;
    [SerializeField]
    Text leftName;
    [SerializeField]
    Text topName;

    [SerializeField]
    GameObject leftRestCardNum;
    [SerializeField]
    GameObject rightRestCardNum;
    [SerializeField]
    GameObject topRestCardNum;



    [SerializeField]
    Transform mParentContainer;

    [SerializeField]
    GameObject lookBtn;
    //Other Players Pos
    [SerializeField]
    Transform playerPosRight;

    [SerializeField]
    Transform playerPosTop;


    [SerializeField]
    Transform playerPosLeft;



    GameObject singleCardPrefab;
    Transform dragDropArrowPrefab;
    GameObject guidePrefab;

    //玩家状态，头游啥的
    [SerializeField]
    Image leftGameCleanSpr;

    [SerializeField]
    Image rightGameCleanSpr;
    [SerializeField]
    Image topGameCleanSpr;

    [SerializeField]
    Image selfGameCleanSpr;


    [SerializeField]
    GameObject tonghuashunBtn;

    [SerializeField]
    GameObject orderCardBtn;

    [SerializeField]
    GameObject revertCardBtn;

    [SerializeField]
    GameObject mainBgColliderBtn;

    [SerializeField]
    Transform outTargetPos;

    [SerializeField]
    Image messageBtn;

    [SerializeField]
    RawImage mPlayingBg;
    [SerializeField]
    RawImage loadingTex;

    [SerializeField]
    GameObject duijiaIcon;

    [SerializeField]
    GameObject voiceBtn;


    private void Awake()
    {
           singleCardPrefab = ResourceManager.Instance.LoadAsset<GameObject>(UIType.SingleCard.Path);

        guidePrefab = ResourceManager.Instance.LoadAsset<GameObject>(UIType.GuideArrowPrefab.Path);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EInitLicense, InitGame); //只用于发牌
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ECanPutOutCard, UpdateCanPutOut);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EChooseCardType, SetSelfPutOutSuc);//牌型选择
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGameClean, UpdateGameClean);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver); //小结算
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshReconnect);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ETributeSuc, UpdateTributeSuc);
        //聊天 使用表情
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUseEmoticon, UseEmoticon);
        //道具
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUseProp, UseProps);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ESortCard, UpdateSortCard); //理牌成功
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ETalkNotify, UseTalk);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDissmisRoomResultSuc, DisRoomResultSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EStopAllCoroutines, StopCoroutines);//关闭协程

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EStartDragDrop, StartDragDrop);//开始拖拽
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReleaseDragDrop, ReleaseDragDrop); //结束拖拽

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOnLinePlayer, UpdateOnLinePlayer); //玩家上线
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOfflinePlayer, UpdateOffLinePlayer); //玩家下线


        playerRestCardNum = new Dictionary<EPlayerPositionType, int>() {
        { EPlayerPositionType.ERight,GlobalData.mPlayerOwnCardCount },
        { EPlayerPositionType.ETop,GlobalData.mPlayerOwnCardCount },
        { EPlayerPositionType.ELeft,GlobalData.mPlayerOwnCardCount },
        };
    }

    void Start()
    {   
        EventTriggerListener.Get(lookBtn).onPress = OnLookBgClick;
        //EventTriggerListener.Get(lookBtn).onClick = OnClearSelectCardIdxClick;
        EventTriggerListener.Get(tonghuashunBtn).onClick = OnTonghuashunClick;

        EventTriggerListener.Get(mainBgColliderBtn).onUp = OnClearSelectCardIdxClick;
        EventTriggerListener.Get(mainBgColliderBtn).onPress = OnLookBgClick;

        EventTriggerListener.Get(orderCardBtn).onClick = OnOrderCardClick;
        EventTriggerListener.Get(revertCardBtn).onClick = OnRevertCardClick;
        EventTriggerListener.Get(messageBtn.gameObject).onClick = OnMessageClick;
        EventTriggerListener.Get(voiceBtn).onPress = OnVoiceClick;
        EventTriggerListener.Get(leftHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(rightHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(topHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(selfHeadSpr.gameObject).onClick = OnHeadClick;
        UIManagers.Instance.DestroySingleUI(UIType.PlayBackView);//销毁回放的界面
        //Test();
        LoadView();
     
        CheckIsReConnect();
        InitPlayerData();

       // StartCoroutine(CheckCards());
    }

    //IEnumerator CheckCards()
    //{
    //    int idx = 0;
    //    while (idx < 3)
    //    {
    //        idx++;
    //        yield return new WaitForSeconds(1.0f);
    //        Debug.Log("检测牌数:" + PlayingGameInfo.Instance.mSelfBaseCardIdList.Count);

    //        if (PlayingGameInfo.Instance.mSelfBaseCardIdList.Count == 0)
    //        {
    //            PlayingGameInfo.Instance.SendReconnectServer();
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }

    //}
    void Test()
    {
        //  TweenManager.Instance.PlayJiefengTween(GetCardsParentPosContainer(EPlayerPositionType.ESelf ), GetLightParentPosContainer(EPlayerPositionType.ESelf), EPlayerPositionType.ESelf);
        TweenManager.Instance.PlayCardTween(TGuanDanCT.CT_TONG_HUA_SHUN, mParentContainer);
    }
    void TestShowDiffrent()
    {
        List<MsgCardGroup> cardGroup = new List<MsgCardGroup>();
        for (int i = 0; i < 2; i++)
        {
            MsgCardGroup msg = new MsgCardGroup();
            if (i == 0)
            {
                msg.ct = TGuanDanCT.CT_SHUN_ZI;
            }
            else
            {
                msg.ct = TGuanDanCT.CT_LIANG_LIAN_DUI;
            }
            for (uint j = 3; j < 9; j++)
            {
                msg.card.Add(j);
            }
            cardGroup.Add(msg);
        }

        ShowCardCTSelect(cardGroup);
    }
    void CheckIsReConnect()
    {
        Debug.Log("PlayingGameInfo.Instance.IsConnectPlayingStatus" + PlayingGameInfo.Instance.IsConnectPlayingStatus);
        if (PlayingGameInfo.Instance.IsConnectPlayingStatus)
        {
            PlayingGameInfo.Instance.SendReconnectServer();
        }
    }
    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EInitLicense, InitGame);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ECanPutOutCard, UpdateCanPutOut);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGameClean, UpdateGameClean);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver); //小结算

        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshReconnect);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUseProp, UseProps);
        //聊天 使用表情
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUseEmoticon, UseEmoticon);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ETributeSuc, UpdateTributeSuc);
        //道具
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUseProp, UseProps);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ESortCard, UpdateSortCard);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ETalkNotify, UseTalk);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDissmisRoomResultSuc, DisRoomResultSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EStopAllCoroutines, StopCoroutines);//关闭协程
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EStartDragDrop, StartDragDrop);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERefreshDragDropPosAndDepth, RefreshDragDropPosAndDepth);//刷新牌的位置       
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EChooseCardType, SetSelfPutOutSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReleaseDragDrop, ReleaseDragDrop); //结束拖拽

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOnLinePlayer,UpdateOnLinePlayer); //玩家上线
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOfflinePlayer, UpdateOffLinePlayer); //玩家下线

    }
    // 关闭协程

    void StopCoroutines(LocalNotification e)
    {
        StopAllCoroutines();
    }

    void ShowInitView()
    {
        selfGameCleanSpr.enabled = true;
        rightGameCleanSpr.enabled = true;
        topGameCleanSpr.enabled = true;
        leftGameCleanSpr.enabled = true;
        duijiaIcon.gameObject.SetActive(false);
    }

    void InitGame(LocalNotification args)
    {
        // mainBgColliderBtn.GetComponent<UITexture>().depth = 0; //To be added...
        //CheckCloseLoadingStatus();
        Debug.Log("=====InitGame ");
        ShowInitView();
        InitSelfLicense();//初始化牌组(发牌)
        ShowResetCardNum();
        InitPlayerData(); //当前游戏状态信息
        CheckCloseLoadingStatus();
    }

    void ShowResetCardNum()
    {
        playerRestCardNum = new Dictionary<EPlayerPositionType, int>() {
        { EPlayerPositionType.ERight,GlobalData.mPlayerOwnCardCount},
        { EPlayerPositionType.ETop,GlobalData.mPlayerOwnCardCount },
        { EPlayerPositionType.ELeft,GlobalData.mPlayerOwnCardCount }, };

        warningPlayerDic = new Dictionary<EPlayerPositionType, bool>()
    {
         { EPlayerPositionType.ERight,false },
        { EPlayerPositionType.ETop,false },
        { EPlayerPositionType.ELeft,false },
    };
        SetRestCardNum(GlobalData.mPlayerOwnCardCount, EPlayerPositionType.ELeft);
        SetRestCardNum(GlobalData.mPlayerOwnCardCount, EPlayerPositionType.ERight);
        SetRestCardNum(GlobalData.mPlayerOwnCardCount, EPlayerPositionType.ETop);
    }
    void CheckCloseLoadingStatus()
    {
        // mPlayingBg.depth = -1; //To be added...
        loadingTex.enabled = false;
    }
    void TestDebugCards(List<uint> list)
    {
        StringBuilder strInfo = new StringBuilder();
        for (int i = 0; i < list.Count; i++)
        {
            strInfo.Append(list[i].ToString() + ",");
        }

        Debug.Log("=====CardLicense,count:" + list.Count + ":::" + strInfo);
    }
    /// <summary>
    /// 转蛋模式下，每局开始，需要显示明牌，通知其他玩家哪两个玩家为何一组
    /// </summary>
    public void CheckShowLightCard()
    {
        uint lightCardId = PlayingGameInfo.Instance.mGameInfo.light_card;
        var lightPlayerList = PlayingGameInfo.Instance.mGameInfo.light_player_id;
        Debug.Log("Show Light Card:"+lightCardId+"人数:"+lightPlayerList.Count);
        bool isSamePlayer = false; //一个人有两张明牌的情况
        float PosY = 124;
        int playerNum = 2;
        //掼蛋模式第一局也需要亮牌 用于谁先出牌用
        if (RoomInfo.Instance.IsZhuandanGameType)
        {
            playerNum = 2;
        }
        else if (PlayingGameInfo.Instance.mGameInfo.game_index == 1)
        {
            playerNum = 1;
        }
        //明牌初始位置(初始位置永远不变,横着排两个)
        //开始动画之后改变为各自的父物体
        //运动
        for (int i = 0; i < playerNum; i++)
        {
            if (i == 1)
            {
                isSamePlayer = lightPlayerList.Count == 1; //一个人有两张明牌的情况
            }

            ulong curPlayerId = 0;
            curPlayerId = isSamePlayer ? lightPlayerList[0] : lightPlayerList[i];
            var playerPos = RoomInfo.Instance.GetPlayerPositionType(curPlayerId);
            Transform parentContainer = GetLightParentPosContainer(playerPos);
            GameObject item = GameObject.Instantiate(singleCardPrefab, this.transform);
            item.transform.localScale = Vector3.one;

            float PosX = i == 0 ? -40 : 40;
            if (playerNum == 1) PosX = 0;//掼蛋一张明牌的时候位置为0
            item.transform.localPosition = new Vector3(PosX, PosY, 0);
            TweenManager.Instance.PlayLightCard(i, item, lightCardId, parentContainer, isSamePlayer);
        }
        TweenManager.Instance.PlayLightCardSmokeAnim(transform, PosY);
    }

    /// <summary>
    /// 换座位烟雾动画
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="isCurEnable"></param>
    /// <param name="isShow"></param>
    void CheckShowJoinTween(EPlayerPositionType posType, ulong playerId)
    {
        if (!curOtherPosPlayer.ContainsKey(posType))
        {
            curOtherPosPlayer.Add(posType, playerId);
            return;
        }

        if (curOtherPosPlayer[posType] != playerId)
        {
            var parent = GetLightParentPosContainer(posType);
            TweenManager.Instance.PlaySmoke(parent);
            curOtherPosPlayer[posType] = playerId;
        }
        //获取当前位置头像是否有，作出对比，如果，是出现，则显示动画
    }
    public void ReConnectShowLightCard()
    {
        int playerNum = 2;//需要明牌的数量
        if (!RoomInfo.Instance.IsZhuandanGameType && !RoomInfo.Instance.IsGuandan2GameType)
        {
            if (PlayingGameInfo.Instance.mGameInfo.game_index == 1)
            {
                playerNum = 1;
            }
            else
            {
                return;
            }
        }

        if (RoomInfo.Instance.IsGuandan2GameType) { playerNum = 1; }

        uint lightCardId = PlayingGameInfo.Instance.mGameInfo.light_card;
        var lightPlayerList = PlayingGameInfo.Instance.mGameInfo.light_player_id;
        if (lightPlayerList.Count == 0)
        {
            return;
        }
        bool isSamePlayer = false;

        //明牌初始位置(初始位置永远不变,横着排两个)
        //开始动画之后改变为各自的父物体
        for (int i = 0; i < playerNum; i++)
        {
            if (i == 1)
            {
                isSamePlayer = lightPlayerList.Count == 1; //一个人有两张明牌的情况
            }
            ulong curPlayerId = 0;
            curPlayerId = isSamePlayer ? lightPlayerList[0] : lightPlayerList[i];
            var playerPos = RoomInfo.Instance.GetPlayerPositionType(curPlayerId);
            Transform parentContainer = GetLightParentPosContainer(playerPos);
            //开始就是结束 相当于没有中间的播放动画的过程 父物体不会改变
            GameObject item = GameObject.Instantiate(singleCardPrefab, parentContainer);
            item.transform.localScale = new Vector3(0.48f, 0.48f, 0.48f);
            item.transform.localPosition = PlayingGameInfo.Instance.GetLightCardPosEnd(item, isSamePlayer);

            SingleCard card = item.GetComponent<SingleCard>();
            card.mCurStatus = ECardStatus.ELightCard;
            card.SetSprName(lightCardId, "small");
        }
    }

    void OnMessageClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new EmoticonViewContext());

    }
    GameObject voiceGm = null;
    void OnVoiceClick(GameObject g, bool isPress)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        OpenVoiceView(isPress);
    }
    void OpenVoiceView(bool isPress)   //to be added ....
    {
        if (isPress)
        {
            if (voiceGm != null)
            {
                voiceGm.GetComponent<UIVoiceView>().EndVoice();
                voiceGm = null;
            }
            voiceGm = UIManagers.Instance.GetSingleUI(UIType.VoiceView);
            voiceGm.transform.SetParent(transform);
        }
        else
        {
            if (voiceGm != null)
                voiceGm.GetComponent<UIVoiceView>().EndVoice();
            voiceGm = null;
        }
    }
    void LoadView()
    {
        GameObject gameInfoView = UIManagers.Instance.GetSingleUI(UIType.GameInfoView);
        gameInfoView.transform.SetParent(mainBgColliderBtn.transform);
        gameInfoView.transform.SetAsLastSibling();
        gameInfoView.transform.localScale = Vector3.one;
        GameObject swicthView = UIManagers.Instance.GetSingleUI(UIType.SwitchBtnFunctionView);
        swicthView.transform.SetParent( mainBgColliderBtn.transform);
        swicthView.transform.GetComponent<Image>().enabled = true; 
        swicthView.transform.GetComponent<SwitchBtnFunction>().enabled = true;
        swicthView.transform.SetAsLastSibling();
        swicthView.transform.localScale = Vector3.one;
        InitDragDropArrow();
    }


    void UpdateGameClean(LocalNotification e)
    {
        Debug.Log("GameClean");
        ArgsGameCleanInfo msg = e.param as ArgsGameCleanInfo;
        if (msg != null)
        {
            ShowCleanPlayer(msg);
        }
    }

    void ShowCleanPlayer(ArgsGameCleanInfo msg)
    {
        if (msg != null)
        {
            //这边加一个暂停
            ulong playerId = msg.playerId;
            MsgGuandanGameRank rank = msg.rank;
            if (playerId == (ulong)PlayerInfo.Instance.mPlayerPid)
            {
                CleanSelfGameStatus(rank);
            }
            else
            {
                CleanOtherGameStatus(RoomInfo.Instance.GetPlayerPositionType((uint)playerId), rank);
            }
        }
    }
    void CleanSelfGameStatus(MsgGuandanGameRank rank)
    {
        // selfGameCleanSpr.depth = 55;
        selfGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
        string spritePath = GlobalData.GetHeadCleanCpr((uint)rank);
        Sprite spr = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, spritePath);
        selfGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
    }
    void UpdateGameBureauOver(LocalNotification e)
    {
        //展示最后出牌的玩家
        ArgsMsgGameOverMsg args = e.param as ArgsMsgGameOverMsg;
        if (args != null)
        {
            if (args.mShowLastCard.Count > 0)
            {
                IEnumerator<MsgSendCardInfo> iPlayersLastCard = args.mShowLastCard.GetEnumerator();
                while (iPlayersLastCard.MoveNext())
                {
                    var posType = RoomInfo.Instance.GetPlayerPosById(iPlayersLastCard.Current.player_id);
                    if (posType != EPlayerPositionType.ENull && posType != EPlayerPositionType.ESelf)
                    {
                        if (iPlayersLastCard.Current.card.Count > 0)
                        {
                            RefreshPlayerCards(posType, iPlayersLastCard.Current.card, true);
                        }
                    }
                }
            }

        }
    }
    void CleanOtherGameStatus(EPlayerPositionType type, MsgGuandanGameRank rank)
    {
        string spritePath = GlobalData.GetHeadCleanCpr((uint)rank);
        Sprite spr = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, spritePath);
        switch (type)
        {
            case EPlayerPositionType.ERight:
                // rightGameCleanSpr.depth = 55;
                rightGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
                rightGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
                break;
            case EPlayerPositionType.ELeft:
                //leftGameCleanSpr.depth = 55;
                leftGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
                leftGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
                break;
            case EPlayerPositionType.ETop:
                // topGameCleanSpr.depth = 55;
                topGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
                topGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
                break;
        }
    }
    /// <summary>
    /// 选择出牌类型
    /// </summary>
    /// <param name="list">牌的类型的种类</param>
    void ShowCardCTSelect(List<MsgCardGroup> list) //To be added ....
    {
        //获取数据
        ContextManager.Instance.Push(new DifferentCardContenxt());
        GameObject cardType = UIManagers.Instance.GetSingleUI(UIType.CardTypeTips);
        cardType.transform.SetParent(this.transform);
        cardType.GetComponent<UIDifferentCardType>().SetData(list);
        //出牌
    }

    void UpdateOnLinePlayer(LocalNotification e)
    {
        ArgsOfflinePlayer args = e.param as ArgsOfflinePlayer;
        if(args!=null)
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
        switch(posType)
        {
            case EPlayerPositionType.ELeft:
                leftHeadSpr.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case EPlayerPositionType.ERight:
                rightHeadSpr.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case EPlayerPositionType.ETop:
                topHeadSpr.transform.GetChild(1).gameObject.SetActive(false);
                break;
        }
    }

    void ShowOffLinePlayer(EPlayerPositionType posType)
    {
        switch (posType)
        {
            case EPlayerPositionType.ELeft:
                leftHeadSpr.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case EPlayerPositionType.ERight:
                rightHeadSpr.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case EPlayerPositionType.ETop:
                topHeadSpr.transform.GetChild(1).gameObject.SetActive(true);
                break;
        }
    }

    void SetSelfPutOutSuc(LocalNotification e)
    {
        ArgsCanputOutCard args = e.param as ArgsCanputOutCard;
        if (args.msgCardGroup != null) //判断是否有两种重复类型需要选择
        {
            ShowCardCTSelect(args.msgCardGroup);
            return;
        }


        //不要和其他显示效果不同
        bool isBuyao = args.putOutType == TGuanDanCT.CT_BUCHU;
        var manager = SelfCardsManager.Instance;
        if (!isBuyao)
        {
            manager.mCurSelectCardIds = args.cards;
            AudioManager.Instance.PlayEffectAudio(manager.GetCardAudioFileName(args.putOutType, args.cards[0], AudioManager.Instance.mAudioStyle));
            //存储当前的卡牌
            //页面动画打出及显示
            for (int i = 0; i < args.cards.Count; i++)
            {
                SingleCard singleCard = manager.GetSingleCardById(args.cards[i]);
                if (singleCard == null)
                {
                    Debug.LogError("item为null,idx:" + args.cards[i]);
                    return;
                }
                singleCard.SwitchStatus(ECardStatus.EOuted);
            }
            //Play Tween 根据服务器发送过来的牌的类型播放动画
            TweenManager.Instance.PlayCardTween(args.putOutType, outTargetPos);
        }
        else
        {
            AudioManager.Instance.PlayEffectAudio(manager.GetCardAudioFileName(args.putOutType, 0, AudioManager.Instance.mAudioStyle));
            GameObject item = Instantiate(singleCardPrefab, mParentContainer);
            item.transform.localPosition = Vector3.zero;//GlobalData.mSelectCardToTargetPos;
            //item.transform.localScale = Vector3.one;
            SingleCard singleCard = item.GetComponent<SingleCard>(); //将多余的牌拿出来
            singleCard.SetCardData(GlobalData.mRefuseCardNumId);
            singleCard.SwitchStatus(ECardStatus.EOuted);
        }
        //刷新Controller层数据
        for (int i = 0; i < args.cards.Count; i++)
        {
            manager.UpdatePutOutInCardList(args.cards[i]); //更新了isIn状态
        }
        manager.ClearAllSelectCards();  //清空所有被选中的打出的牌
        manager.SendShakeNormalCards();
        manager.RefreshRealDic(); //刷新isIn
        SetCardPosAndDepth(manager.mRealCardDic); // 刷新底部牌
    }

    void CheckAutoRefreshCard(List<MsgSendCardInfo> infoList)
    {
        if (PlayingGameInfo.Instance.mPlayingStatus == EPlayingStatus.ETribute)
        {
            return;
        }
        for (int i = 0; i < infoList.Count; i++)
        {
            MsgSendCardInfo info = infoList[i];
            if (info.player_id == PlayerInfo.Instance.mPlayerPid)
            {
                if (info.card_num < SelfCardsManager.Instance.GetRealCardCount())
                {
                    PlayingGameInfo.Instance.SendRefreshGameServer();
                    break;
                }
                else if (info.card_num > SelfCardsManager.Instance.GetRealCardCount())
                {
                    PlayingGameInfo.Instance.SendReconnectServer();
                    break;
                }
            }
            else
            {
                if (info.card_num != playerRestCardNum[RoomInfo.Instance.GetPlayerPositionType(info.player_id)])
                {
                    PlayingGameInfo.Instance.SendRefreshGameServer();
                    break;
                }
            }

        }

    }

    int curTeamerCardCount = 0;
    /// <summary>
    /// 检测显示对家牌
    /// </summary>
    void CheckShowTeamerCards(List<uint> djList)
    {
        if (djList == null) { return; }
        duijiaIcon.gameObject.SetActive(djList.Count > 0);

        if (djList.Count == 0) { return; }
        DestroyChildren(mParentContainer);

        var data = djList;
        curTeamerCardCount = data.Count;
        List<SingleCard> cardList = new List<SingleCard>();
        List<List<SingleCard>> dic = new List<List<SingleCard>>();
        for (int i = 0; i < curTeamerCardCount; i++)
        {
            GameObject item = Instantiate(singleCardPrefab);
            item.transform.SetParent(mParentContainer);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            SingleCard singleCard = item.GetComponent<SingleCard>();
            singleCard.SetCardData(data[i]);
            singleCard.posType = EPlayerPositionType.ENull;
            cardList.Add(singleCard);
        }
        SelfCardsManager.Instance.InitRefreshRealCardDic(cardList, dic);
        SetTeamCardPosAndDepth(dic); // 刷新底部牌
    }

    //其他人出牌，刷新其他人出牌,依次三次，第三次（playerid为上家）为上家出牌，需要记录保存，
    void UpdateCanPutOut(LocalNotification e)
    {
        ArgsCanputOutCard msg = e.param as ArgsCanputOutCard;
        if (null != msg)
        {

            var selfId = PlayerInfo.Instance.mPlayerPid;
            var lastPlayerPosType = RoomInfo.Instance.GetPlayerPositionType(msg.lastPlayerId);
            //验证自动刷新牌

            if (lastPlayerPosType != EPlayerPositionType.ESelf)
            {
                SetOtherData(lastPlayerPosType, msg.cards, msg.putOutType);
            }
            else
            {
                SetSelfPutOutSuc(e);
            }
            //清空轮到 玩家的面前的牌
            var curPlayerPosType = RoomInfo.Instance.GetPlayerPositionType(msg.playerId);
            string strLog2 = string.Format("=========$$$$$$$$55555555轮到当前出牌玩家:{0}", curPlayerPosType);
            Debug.Log(strLog2);
            //  ClearPutOutingCard(curPlayerPosType);
            if (msg.sendCardInfo != null)
            {
                CheckAutoRefreshCard(msg.sendCardInfo);
            }
            if (PlayingGameInfo.Instance.IsHavePlayerGameClean())
            {
                //检测是否刷新清空掉最后出的一批牌
                CheckCleanLastCards();
            }
            Debug.Log("msg.DjCards:" + msg.DjCards.Count);


            CheckShowTeamerCards(msg.DjCards); //展示对家的牌
        }
    }


   
 
    /// <summary>
    /// 清空当前已经打出的牌
    /// </summary>
    /// <param name="posType"></param>
    public void ClearPutOutingCard(EPlayerPositionType posType)
    {
        DestroyChildren(GetCardsParentPosContainer(posType));
    }

    //显示其他人出牌的信息
    void SetOtherData(EPlayerPositionType type, List<uint> list, TGuanDanCT putOutType)
    {
        //为解决 玩家不出的bug
        int lastCardCount = Mathf.Clamp(playerRestCardNum[type] - list.Count, 0, GlobalData.mPlayerOwnCardCount); //(减完当前牌)剩余牌的张数
        SetPlayerCardsPos(type, list, putOutType);
        SetRestCardNum(lastCardCount, type);
    }
    #region 检测其他玩家最后剩余一批牌的信息
    void CheckCleanLastCards()
    {
        var tempDic = new Dictionary<EPlayerPositionType, LastCleanPlayerInfomation>();
        foreach (var v in PlayingGameInfo.Instance.mLastCleanPlayerInfoDic)
        {
            LastCleanPlayerInfomation d = v.Value;
            tempDic.Add(v.Key, d);
        }

        foreach (var v in tempDic)
        {
            if (v.Value.idx > v.Value.totaIdxTimes) { continue; }

            if (v.Value.idx == v.Value.totaIdxTimes)
            {
                Debug.Log("发送清牌消息" + v.Key);
                ClearPutOutingCard(v.Key);
                LastCleanPlayerInfomation last = tempDic[v.Key];
                last.idx++; 
                PlayingGameInfo.Instance.mLastCleanPlayerInfoDic[v.Key] = last;
            }
            else
            {
                LastCleanPlayerInfomation last = tempDic[v.Key];
                last.idx++;
                Debug.Log("type:" + v.Key + "idx:" + last.idx);
                PlayingGameInfo.Instance.mLastCleanPlayerInfoDic[v.Key] = last;
            }
        }
    }

    #endregion

    /// <summary>
    /// 其他玩家剩余牌数
    /// </summary>
    Dictionary<EPlayerPositionType, int> playerRestCardNum;


    /// <summary>
    /// 其他玩家是否警报过（警报只发生一次）
    /// </summary>
    Dictionary<EPlayerPositionType, bool> warningPlayerDic = new Dictionary<EPlayerPositionType, bool>()
    {
         { EPlayerPositionType.ERight,false },
        { EPlayerPositionType.ETop,false },
        { EPlayerPositionType.ELeft,false },
    };
    void ShowResetCardNum(EPlayerPositionType type, GameObject m, int count)
    {
        m.SetActive(count <= 10 ? true : false);
        if (count <= 10)
        {
            if (!warningPlayerDic[type])
            {
                if (type == EPlayerPositionType.ESelf)
                {
                    AudioManager.Instance.PlayWarningAudio(); //声音只响一次
                }
                warningPlayerDic[type] = true;
            }

            if (count == 0)
            {
                m.SetActive(false);
            }

        }

        if (m.activeSelf)
        {
            m.transform.GetChild(0).GetComponent<Text>().text = count.ToString();
        }
    }



    /// <summary>
    /// 剩余玩家卡牌数量
    /// </summary>
    /// <param name="curNum"></param>
    void SetRestCardNum(int lastCount, EPlayerPositionType type)
    {
        playerRestCardNum[type] = lastCount;
        switch (type)
        {
            case EPlayerPositionType.ERight:
                ShowResetCardNum(EPlayerPositionType.ERight, rightRestCardNum, lastCount);
                break;
            case EPlayerPositionType.ETop:
                ShowResetCardNum(EPlayerPositionType.ETop, topRestCardNum, lastCount);
                break;
            case EPlayerPositionType.ELeft:
                ShowResetCardNum(EPlayerPositionType.ELeft, leftRestCardNum, lastCount);
                break;
        }
    }

    public Transform GetCardsParentPosContainer(EPlayerPositionType type)
    {
        switch (type)
        {
            case EPlayerPositionType.ELeft:
                return playerPosLeft;
            case EPlayerPositionType.ERight:
                return playerPosRight;
            case EPlayerPositionType.ETop:
                return playerPosTop;
            case EPlayerPositionType.ESelf:
                return outTargetPos;
        }
        return playerPosRight;
    }

    public Transform GetLightParentPosContainer(EPlayerPositionType type)
    {
        switch (type)
        {
            case EPlayerPositionType.ELeft:
                return leftHeadSpr.transform.GetChild(0);
            case EPlayerPositionType.ERight:
                return rightHeadSpr.transform.GetChild(0);
            case EPlayerPositionType.ETop:
                return topHeadSpr.transform.GetChild(0);
            case EPlayerPositionType.ESelf:
                return selfHeadSpr.transform;
        }
        return null;
    }

    void SetPlayerCardsPos(EPlayerPositionType type, List<uint> list, TGuanDanCT putOutType)
    {
        //他人出牌只用于显示
        Transform parentContainer = GetCardsParentPosContainer(type);
        EAudioStyle audioStyle = RoomInfo.Instance.GetPlayerAudioStyleByPlayerPos(type);

        //PlayTween
        TweenManager.Instance.PlayCardTween(putOutType, parentContainer);

        if (putOutType != TGuanDanCT.CT_BUCHU)
        {
            AudioManager.Instance.PlayEffectAudio(SelfCardsManager.Instance.GetCardAudioFileName(putOutType, list[0], audioStyle));
            //Debug.LogError("玩家位置："+type.ToString()+"玩家的音效类型："+audioStyle);
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject item = Instantiate(singleCardPrefab);
                item.transform.SetParent(parentContainer);
                item.transform.localPosition = GetOtherCardsPos(type, i, count);
                item.transform.localScale = Vector3.one;
                SingleCard singleCard = item.transform.GetComponent<SingleCard>();
                singleCard.SetOtherData(list[i], type, false);
            }
        }
        else if (putOutType == TGuanDanCT.CT_BUCHU)
        {
            AudioManager.Instance.PlayEffectAudio(SelfCardsManager.Instance.GetCardAudioFileName(putOutType, 0, audioStyle));
            //Debug.LogError("玩家位置：" + type.ToString() + "玩家的音效类型：" + audioStyle);
            // mRefuseCardNumId 为 不要的牌 id ，这里把自己的不出也当做别人的不出
            GameObject item = GameObject.Instantiate(singleCardPrefab) as GameObject;
            item.transform.SetParent(parentContainer);
            item.transform.localPosition = GetOtherCardsPos(type, 0, 1);
            item.transform.localScale = Vector3.one;
            SingleCard singleCard = item.transform.GetComponent<SingleCard>();
            singleCard.SetOtherData(GlobalData.mRefuseCardNumId, type, false);
        }


    }

    Vector3 GetOtherCardsPos(EPlayerPositionType type, int idx, int totalCount)
    {
        //不同玩家类型，控制角度

        float ratePosX = 0;
        if (type == EPlayerPositionType.ESelf)
        {
            ratePosX = GlobalData.mSingleCardWidth / 2;
        }
        else
        {
            ratePosX = GlobalData.mSingleCardWidth / 2.5f;
        }
        float middle = totalCount % 2 == 0 ? totalCount / 2 - 0.5f : totalCount / 2;
        float posX = (idx - middle) * ratePosX;
        return new Vector3(posX, 0, 0);
    }

    void UpdatePutOutFail(LocalNotification e)
    {
        // GameManager.Instance.mIsRequesting = false;
        Debug.Log("=====不能出牌，客户端同步");
   
        //弹出Tips，不能打。
    }

    void OnTonghuashunClick(GameObject g)
    {
        var manager = SelfCardsManager.Instance;
        if (!SelfCardsManager.Instance.IsNoneRequestTonghuashunTip)
        {
            manager.PostTonghuashunToCards();
        }
        else
        {
            manager.SendServerTonghuashunTip();
        }
    }

    #region Press Bg Look
    float curLookBgTime = 0;
    float totalLookBgTime = 0.15f;
    int PressLookBgId = 0; //0，表示没有按，1表示，按，2表示按了实现方法了
    void CheckPressLookBg()
    {
        if (PressLookBgId == 1)
        {
            curLookBgTime += Time.deltaTime;
            if (curLookBgTime > totalLookBgTime && PressLookBgId == 1)
            {
                PressLookBgId = 2;
                ArgsCardStatus msg = new ArgsCardStatus();
                IEnumerator<uint> iters = SelfCardsManager.Instance.GetOwnCardIdList().GetEnumerator();
                while (iters.MoveNext())
                {
                    msg.idList.Add(iters.Current);
                }
                msg.status = ECardStatus.ETransparent;
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EChangeCardSelectStatus, msg);
            }
        }
    }

    void OnLookBgClick(GameObject g, bool isPress)
    {
       
            if (SelfCardsManager.Instance.mIsDragDroping || SelfCardsManager.Instance.mIsDowing) { return; }

            SelfCardsManager.Instance.mIsPressingBg = isPress;
            PressLookBgId = isPress == true ? 1 : 0;

            if (isPress)
            {
                //SelfCardsManager.Instance.PostSendCardToSelectStatus(SelfCardsManager.Instance.GetOwnCardIdList(), ECardStatus.ENormal);
                OnClearSelectCardIdxClick(null);
            }
            else
            {
                curLookBgTime = 0;
                OnClearSelectCardIdxClick(null);
            }
    }
    #endregion
   

    /// <summary>
    /// 发牌
    /// </summary>
    void InitSelfLicense()
    {
        var selfManager = SelfCardsManager.Instance;
        mParentContainer.gameObject.SetActive(true);
        DestroyChildren(mParentContainer);
        SelfCardsManager.Instance.mCardDataList.Clear();
        var data = PlayingGameInfo.Instance.mSelfBaseCardIdList;
        Debug.Log("Card Count :" + data.Count);
        for (int i = 0; i < data.Count; i++)
        {
            GameObject item = GameObject.Instantiate(singleCardPrefab) as GameObject;
            item.transform.SetParent(mParentContainer);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            SingleCard singleCard = item.GetComponent<SingleCard>();
            singleCard.SetCardData(data[i]);
            singleCard.mIsIn = true;
            selfManager.mCardDataList.Add(singleCard);
        }
        //---------------
        selfManager.ClearAllSelectCards();
        selfManager.InitRefreshRealCardDic(selfManager.mCardDataList, selfManager.mRealCardDic);
        int sortCardRecordCount = selfManager.mSortRecordList.Count;
        if (sortCardRecordCount > 0)
        {
            for (int i = 0; i < sortCardRecordCount; i++)
            {
                AutoSortRealCard(i);
            }
            selfManager.ClearAllSelectCards();
        }
        SetCardPosAndDepth(selfManager.mRealCardDic); // 刷新底部牌
        //------------------
    }

    /// <summary>
    /// 改变最初始的牌(专门用于进贡，回贡，改变最基础的牌)
    /// </summary>
    public void ChangeBaseSelfCard(uint id, bool isAdd)
    {
        var playingManager = PlayingGameInfo.Instance;
        var selfManager = SelfCardsManager.Instance;
        if (isAdd == true)
        {
            GameObject item = GameObject.Instantiate(singleCardPrefab) as GameObject;
            item.transform.SetParent(mParentContainer);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            SingleCard singleCard = item.GetComponent<SingleCard>();
            singleCard.SetCardData(id);
            singleCard.mIsIn = true;
            playingManager.mSelfBaseCardIdList.Add(id);
            selfManager.AddCardDataListItem(singleCard);
        }
        else
        {
            GameObject.Destroy(selfManager.GetSingleCardById(id).gameObject);
            playingManager.mSelfBaseCardIdList.Remove(id);
            selfManager.RemoveCardDataListItem(id);
        }



        // 存理牌记录，自动理牌n次
        selfManager.ClearAllSelectCards();
        selfManager.InitRefreshRealCardDic(SelfCardsManager.Instance.mCardDataList, SelfCardsManager.Instance.mRealCardDic);
        int sortCardRecordCount = SelfCardsManager.Instance.mSortRecordList.Count;
        if (sortCardRecordCount > 0)
        {
            for (int i = 0; i < sortCardRecordCount; i++)
            {
                AutoSortRealCard(i);
            }
            SelfCardsManager.Instance.ClearAllSelectCards();
        }
        SetCardPosAndDepth(SelfCardsManager.Instance.mRealCardDic); // 刷新底部牌
                                                                    //----------
        if (isAdd)
        {
            //显示增加的牌（改变颜色）
            selfManager.AddSelectCard(id);
            selfManager.PostSendCardToSelectStatus(selfManager.mCurSelectCardIds, ECardStatus.ESelected);
        }
        selfManager.SendShakeNormalCards();
    }


    void AutoSortRealCard(int idx)
    {
        var manager = SelfCardsManager.Instance;
        var typeList = manager.mSortRecordTypeList;
        var cardList = manager.mSortRecordList;
        manager.mCurSelectCardIds.Clear();
        for (int i = 0; i < cardList[idx].Count; i++)
        {
            manager.mCurSelectCardIds.Add(cardList[idx][i]);
        }
        SelfCardsManager.Instance.OrderRealDicToList(typeList[idx]);
    }

    /// <summary>
    /// RealDic 的View层哦
    /// </summary>
    void SetCardPosAndDepth(List<List<SingleCard>> cardDic)
    {
        for (int k = 0; k < cardDic.Count; k++)
        {
            var value = cardDic[k];
            for (int i = 0; i < value.Count; i++)
            {
                Transform obj = value[i].transform;
                obj.transform.localPosition = SelfCardsManager.Instance.GetRealCardPos(cardDic, value[i].mId);
                value[i].AdjustColliderToLong(i == value.Count - 1);
            }
        }

        //Sort Depth
        SelfCardsManager.Instance.ResetSibling(cardDic);
    }

    void SetTeamCardPosAndDepth(List<List<SingleCard>> cardDic)
    {
        for (int k = 0; k < cardDic.Count; k++)
        {
            var value = cardDic[k];
            for (int i = 0; i < value.Count; i++)
            {
                Transform obj = value[i].GetComponent<Transform>();
                obj.transform.localPosition = SelfCardsManager.Instance.GetRealCardPos(cardDic, value[i].mId);
                value[i].AdjustColliderToLong(i == value.Count - 1);
            }
        }
        SelfCardsManager.Instance.ResetSibling(cardDic);
    }
    void AdjustSelectCardPosAndDepthInReal()
    {
        var list = SelfCardsManager.Instance.mCurSelectCardIds;
        for (int i = 0; i < list.Count; i++)
        {
            Transform obj = SelfCardsManager.Instance.GetSingleCardById(list[i]).GetComponent<Transform>();
            obj.transform.localPosition += new Vector3(0, GlobalData.mDropDragHeight, 0);
        }
    }


    void SetLastCardPosAndDepth(List<List<SingleCard>> cardDic)
    {
        var dic = new List<List<SingleCard>>();
        for (int k = 0; k < cardDic.Count; k++)
        {
            dic.Add(cardDic[k]);
        }

        for (int k = 0; k < dic.Count; k++)
        {
            var value = dic[k];
            for (int i = 0; i < value.Count; i++)
            {
                Transform obj = value[i].GetComponent<Transform>();
                obj.transform.localPosition = SelfCardsManager.Instance.GetLastDicPos(cardDic, value[i].mId);
            }
        }

        SelfCardsManager.Instance.ResetSibling(cardDic);
    }



    /// <summary>
    /// 当前位置的玩家playerId
    /// </summary>
    private Dictionary<EPlayerPositionType, ulong> curOtherPosPlayer = new Dictionary<EPlayerPositionType, ulong>();
    void InitPlayerData()
    {
        ClearCurBureauGame();
        var roomInfo = RoomInfo.Instance;
        var playingInfo = PlayingGameInfo.Instance;
        var dic = roomInfo.mPlayersDic;
        if (dic[EPlayerPositionType.ETop] == null)
        {
            return;// 断线重连，从start方法调用时，return掉
        }

        string rightUrl = roomInfo.GetPlayerInfoById((uint)dic[EPlayerPositionType.ERight].player_id).head_portrait;
        Debug.Log("rightPlayerId:" + dic[EPlayerPositionType.ERight].player_id.ToString() + ",url:" + rightUrl);

        StartCoroutine(GlobalData.GetHeadTextureByIdx(rightHeadSpr, rightUrl));
        CheckShowJoinTween(EPlayerPositionType.ERight, dic[EPlayerPositionType.ERight].player_id);

        string topUrl = roomInfo.GetPlayerInfoById((uint)dic[EPlayerPositionType.ETop].player_id).head_portrait;
        Debug.Log("topPlayerId:" + dic[EPlayerPositionType.ETop].player_id.ToString() + ",url:" + topUrl);

        StartCoroutine(GlobalData.GetHeadTextureByIdx(topHeadSpr, topUrl));
        CheckShowJoinTween(EPlayerPositionType.ETop, dic[EPlayerPositionType.ETop].player_id);

        string leftUrl = roomInfo.GetPlayerInfoById((uint)dic[EPlayerPositionType.ELeft].player_id).head_portrait;
        Debug.Log("leftPlayerId:" + dic[EPlayerPositionType.ELeft].player_id.ToString() + ",url:" + leftUrl);
        StartCoroutine(GlobalData.GetHeadTextureByIdx(leftHeadSpr, leftUrl));
        CheckShowJoinTween(EPlayerPositionType.ELeft, dic[EPlayerPositionType.ELeft].player_id);

        MsgPlayerInfo rightInfo = roomInfo.GetPlayerInfoById((uint)dic[EPlayerPositionType.ERight].player_id);
        MsgPlayerInfo topInfo = roomInfo.GetPlayerInfoById((uint)dic[EPlayerPositionType.ETop].player_id);
        MsgPlayerInfo leftInfo = roomInfo.GetPlayerInfoById((uint)dic[EPlayerPositionType.ELeft].player_id);
        rightName.text = rightInfo.name;
        topName.text = topInfo.name;
        leftName.text = leftInfo.name;

        ResetGameClean();
        string url = PlayerInfo.Instance.mPlayerData.headPortrait;
        StartCoroutine(GlobalData.GetHeadTextureByIdx(selfHeadSpr, url));
        selfName.text = PlayerInfo.Instance.mPlayerData.name;
        Color winColor = new Color(1, 229 / 255.0f, 141 / 255.0f, 1);
        Color loseColor = new Color(86 / 255.0f, 180 / 255.0f, 1, 1);
        if (roomInfo.IsZhuandanGameType || RoomInfo.Instance.IsGuandan2GameType)
        {
            //scoreLab.transform.GetChild(0).gameObject.SetActive(true);
            scoreLab.color = playingInfo.mScore >= 0 ? winColor : loseColor;
            scoreLab.text =/* "积分:" + */playingInfo.mScore.ToString();
            //显示积分

            rightName.transform.GetChild(0).gameObject.SetActive(true);
            rightName.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = rightInfo.goal >= 0 ? winColor : loseColor;
            rightName.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = rightInfo.goal.ToString();//积分lab
            leftName.transform.GetChild(0).gameObject.SetActive(true);
            leftName.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = leftInfo.goal >= 0 ? winColor : loseColor;
            leftName.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = leftInfo.goal.ToString();

            topName.transform.GetChild(0).gameObject.SetActive(true);
            topName.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = topInfo.goal >= 0 ? winColor : loseColor;
            topName.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = topInfo.goal.ToString();
        }
        else //掼蛋模式下，非打2的第二把及以上
        {
            //己方就是A 对方就是B 
            scoreLab.color = Color.white;
            scoreLab.text = "本局打:<color=#FFE58DFF>" + roomInfo.mNextIndex.ToString() + "</color>";
        }
    }

    void ResetGameClean()
    {
        //var initName = GlobalData.GetHeadCleanCpr(0);
        //selfGameCleanSpr.depth = 8;
        //leftGameCleanSpr.depth = 8;
        //rightGameCleanSpr.depth = 8;
        //topGameCleanSpr.depth = 8;
        selfGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false;
        rightGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false;
        leftGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false;
        topGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false;
        //selfGameCleanSpr.spriteName = initName;
        //leftGameCleanSpr.spriteName = initName;
        //rightGameCleanSpr.spriteName = initName;
        //topGameCleanSpr.spriteName = initName;
    }

    private void Update()
    {
        CheckPressLookBg();
        // check网络是否可用
        //if (SDKManager.Instance != null)
        //{
        //    SDKManager.Instance.netCheck();
        //}
        //private void Update()
        //{
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    AudioManager.Instance.IsPauseAllEffectAudio(false);

        //}
        //else if (Input.GetKeyDown(KeyCode.W))
        //{
        //    AudioManager.Instance.IsPauseAllEffectAudio(true);
        //}

        //}

        //if (Input.touchCount <= 0)
        //    return;
        //if (Input.touchCount == 1) //单点触碰移动摄像机
        //{
        //    if (Input.touches[0].phase == TouchPhase.Began)
        //        Debug.Log("====开始触摸");
        //    //  m_screenPos = Input.touches[0].position;   //记录手指刚触碰的位置
        //    if (Input.touches[0].phase == TouchPhase.Moved) //手指在屏幕上移动，移动摄像机
        //    {
        //        Debug.Log("====正在移动" + Input.touches[0].deltaPosition.x);
        //        //   transform.Translate(new Vector3(Input.touches[0].deltaPosition.x * Time.deltaTime, Input.touches[0].deltaPosition.y * Time.deltaTime, 0));
        //    }
        //    if (Input.touches[0].phase == TouchPhase.Ended)
        //    {
        //        Debug.Log("=====释放");
        //    }
        //}
        //Test==========
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    List<uint> cardIdList = new List<uint>()
        //    {
        //75,76,22,23,24,25,26 //大小王       ,15空出出为Common牌
        //    };

        //    RefreshPlayerCards(EPlayerPositionType.ETop, cardIdList, true);
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    List<uint> cardList = new List<uint>() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
        //    CheckShowTeamerCards(cardList);
        //}
        //else if (Input.GetKeyDown(KeyCode.W))
        //{
        //    // SelfCardsManager.Instance.
        //    PlayingGameInfo.Instance.SendReconnectServer();
        //}
        //==========
    }

    /// <summary>
    /// 撤销当前所有选中的牌的状态
    /// </summary>
    /// <param name="g"></param>
    void OnClearSelectCardIdxClick(GameObject g)
    {
        if (SelfCardsManager.Instance.mIsDragDroping || SelfCardsManager.Instance.mIsDowing) { return; }
        SelfCardsManager.Instance.ClearAllSelectCards();
        SelfCardsManager.Instance.SendShakeNormalCards();
    }


    /// <summary>
    /// 理牌
    /// </summary>
    /// <param name="g"></param>
    void OnOrderCardClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        var manager = SelfCardsManager.Instance;
        var selectCards = manager.mCurSelectCardIds;
        if (selectCards.Count == 0)
        {
            UIManagers.Instance.EnqueueTip("请选择手牌");
            return;
        }

        SelfCardsManager.Instance.SendShakeNormalCards();
        SelfCardsManager.Instance.SendServerSortCard();
    }

    void OnRevertCardClick(GameObject g)
    {
        //    if (SelfCardsManager.Instance.mIsDragDroping || SelfCardsManager.Instance.mIsDowing) { return; }
        SelfCardsManager.Instance.mIsDowing = false;
        SelfCardsManager.Instance.mIsDragDroping = false;
        Debug.Log("Revert...");

        AudioManager.Instance.PlayClickBtnAudio();
        SelfCardsManager.Instance.mIsDragDroping = false;
        SelfCardsManager.Instance.ClearAllSelectCards();
        SelfCardsManager.Instance.SendShakeNormalCards();
        SelfCardsManager.Instance.ResetToInitDic();
        SelfCardsManager.Instance.ClearSortRecordInfo();
        SetCardPosAndDepth(SelfCardsManager.Instance.mRealCardDic); // 刷新底部牌
    }


    #region 刷新数据


    /// <summary>
    /// 代替InitSelfLicense
    /// </summary>
    void ResetPutCards()
    {
        var manager = SelfCardsManager.Instance;
        if (manager.mIsDragDroping || manager.mIsDowing) //拖拽状态不用刷新牌
        {
            return;
        }

        var data = PlayingGameInfo.Instance.mSelfBaseCardIdList;
        Debug.Log("Card Count :" + data.Count);
        if (data.Count == 0) { return; }
        //Test!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //data.RemoveAt(data.Count - 1);// 此处测试减少最后一个牌
        //Test!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        List<List<SingleCard>> tempRealDic = new List<List<SingleCard>>();
        for (int k = 0; k < manager.mRealCardDic.Count; k++)
        {
            var list = new List<SingleCard>();
            IEnumerator<SingleCard> iters = manager.mRealCardDic[k].GetEnumerator();
            while (iters.MoveNext())
            {
                list.Add(iters.Current);
            }
            tempRealDic.Add(list);
        }
        for (int i = 0; i < data.Count; i++)
        {
            for (int k = 0; k < tempRealDic.Count; k++)
            {
                bool isBreak = false;
                var vaData = tempRealDic[k];
                for (int j = 0; j < vaData.Count; j++)
                {
                    if (vaData[j].mId == data[i])
                    {
                        tempRealDic[k].RemoveAt(j);
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }
        }


        for (int k = 0; k < tempRealDic.Count; k++)
        {
            var value = tempRealDic[k];
            for (int j = 0; j < value.Count; j++)
            {
                GameObject.Destroy(value[j].gameObject);
            }
        }
        for (int k = 0; k < tempRealDic.Count; k++)
        {
            var value = tempRealDic[k];
            for (int i = 0; i < value.Count; i++)
            {
                manager.GetRealCardDataIsOut(value[i].mId);
            }
        }
        SelfCardsManager.Instance.RefreshRealDic(); //刷新isIn
        SetCardPosAndDepth(SelfCardsManager.Instance.mRealCardDic); // 刷新底部牌
    }

    void RefreshReconnect(LocalNotification e)
    {
        SelfCardsManager.Instance.ClearAllSelectCards();
        CheckCloseLoadingStatus();
        ArgsRefreshData args = e.param as ArgsRefreshData;
        if (args != null)
        {
            var roomInfo = RoomInfo.Instance;
            ShowInitView();

            InitPlayerData();
            //刷新自己手牌  
            InitSelfLicense(); //
            //检查亮牌           
            ReConnectShowLightCard();
            //刷新其他玩家剩余牌数
            IEnumerator<MsgPlayerInfo> iPlayersCardLastNum = args.playerInfoList.GetEnumerator();
            while (iPlayersCardLastNum.MoveNext())
            {
                var posType = roomInfo.GetPlayerPosById(iPlayersCardLastNum.Current.player_id);
                var lastCardCount = (int)iPlayersCardLastNum.Current.remaindCarsNum;
                playerRestCardNum[posType] = lastCardCount;
                SetRestCardNum(lastCardCount, posType);
            }
            //刷新最后三轮牌的数据
            IEnumerator<MsgLast3Card> iPlayersLast3Card = args.last3CardList.GetEnumerator();
            while (iPlayersLast3Card.MoveNext())
            {
                var posType = roomInfo.GetPlayerPosById(iPlayersLast3Card.Current.player_id);
                if (posType != EPlayerPositionType.ENull)
                {
                    //   if (!IsGameOverPlayer(args.gameOverInfoList, iPlayersLast3Card.Current.player_id)) ;
                    {
                        RefreshPlayerCards(posType, iPlayersLast3Card.Current.card, false);
                    }
                }
            }
            //刷新所有玩家头游状态
            if (args.gameOverInfoList != null && args.gameOverInfoList.Count > 0)
            {
                for (int i = 0; i < args.gameOverInfoList.Count; i++)
                {
                    var gameOverItem = args.gameOverInfoList[i];
                    PlayingGameInfo.Instance.AddLastCleanPlayerDic(roomInfo.GetPlayerPosById(gameOverItem.player_id));

                    ArgsGameCleanInfo cleanArgs = new ArgsGameCleanInfo();
                    cleanArgs.playerId = gameOverItem.player_id;
                    cleanArgs.rank = gameOverItem.rank;
                    NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGameClean, cleanArgs);
                }
            }

            CheckShowTeamerCards(args.djCards); //展示对家的牌

        }
    }

    /// <summary>
    /// 是否是 结束（游的玩家）
    /// </summary>
    /// <param name="list"></param>
    /// <param name="playerId"></param>
    /// <returns></returns>
    bool IsGameOverPlayer(List<MsgGameOverInfo> list, ulong playerId)
    {
        if (list == null || list.Count == 0)
        {
            return false;
        }

        IEnumerator<MsgGameOverInfo> iGameOverInfo = list.GetEnumerator();
        while (iGameOverInfo.MoveNext())
        {
            if (iGameOverInfo.Current.player_id == playerId)
            {
                return true;
            }
        }
        return false;
    }
    //刷新数据
    void RefreshAllData(LocalNotification e)
    {
        ArgsRefreshData args = e.param as ArgsRefreshData;
        //  SelfCardsManager.Instance.ClearAllSelectCardsForRefresh(args.oldSelfCardIdList);
        CheckCloseLoadingStatus();
        if (args != null)
        {
            var roomInfo = RoomInfo.Instance;
            ShowInitView();

            //刷新自己手牌  
            ResetPutCards();
            ClearCurBureauGame();
            ReConnectShowLightCard();
            //刷新其他玩家剩余牌数
            IEnumerator<MsgPlayerInfo> iPlayersCardLastNum = args.playerInfoList.GetEnumerator();
            while (iPlayersCardLastNum.MoveNext())
            {
                var posType = roomInfo.GetPlayerPosById(iPlayersCardLastNum.Current.player_id);
                var lastCardCount = (int)iPlayersCardLastNum.Current.remaindCarsNum;
                playerRestCardNum[posType] = lastCardCount;
                SetRestCardNum(lastCardCount, posType);
            }

            //刷新最后三轮牌的数据
            IEnumerator<MsgLast3Card> iPlayersLast3Card = args.last3CardList.GetEnumerator();
            var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(args.action.action_id);
            while (iPlayersLast3Card.MoveNext())
            {
                if (iPlayersLast3Card.Current.player_id == 0)
                { continue; }
                var posType = roomInfo.GetPlayerPosById(iPlayersLast3Card.Current.player_id);
                if (posType != EPlayerPositionType.ENull && curPlayerPosType != posType)
                {
                    RefreshPlayerCards(posType, iPlayersLast3Card.Current.card, false);
                }
            }
            //刷新所有玩家头游状态
            if (args.gameOverInfoList != null && args.gameOverInfoList.Count > 0)
            {
                for (int i = 0; i < args.gameOverInfoList.Count; i++)
                {
                    ArgsGameCleanInfo cleanArgs = new ArgsGameCleanInfo();
                    cleanArgs.playerId = args.gameOverInfoList[i].player_id;
                    cleanArgs.rank = args.gameOverInfoList[i].rank;
                    NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGameClean, cleanArgs);
                }
            }
            CheckShowTeamerCards(args.djCards);
        }
    }
    public void ClearCurBureauGame()
    {
        ClearEachCards();
        ClearLightCard();
    }
    void ClearEachCards()
    {
        DestroyChildren(playerPosLeft);
        DestroyChildren(playerPosRight);
        DestroyChildren(playerPosTop);
        DestroyChildren(outTargetPos);
    }
    public void DestroyChildren(Transform t)
    {
        bool isPlaying = Application.isPlaying;

        while (t.childCount != 0)
        {
            Transform child = t.GetChild(0);

            if (isPlaying)
            {
                child.SetParent(null);
                UnityEngine.GameObject.Destroy(child.gameObject);
            }
            else UnityEngine.GameObject.DestroyImmediate(child.gameObject);
        }
    }

    void ClearLightCard()
    {
        if (selfHeadSpr.transform.childCount > 0)
        {
            DestroyChildren(selfHeadSpr.transform);
        }

        if (rightHeadSpr.transform.GetChild(0).childCount > 0)
        {
            DestroyChildren(rightHeadSpr.transform.GetChild(0));
        }
        if (topHeadSpr.transform.GetChild(0).childCount > 0)
        {
            DestroyChildren(topHeadSpr.transform.GetChild(0));
        }
        if (leftHeadSpr.transform.GetChild(0).childCount > 0)
        {
            DestroyChildren(leftHeadSpr.transform.GetChild(0));
        }

    }

    /// <summary>
    /// 用于单纯同步数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="list"></param>
    void RefreshPlayerCards(EPlayerPositionType type, List<uint> list, bool isLast)
    {
        ClearPutOutingCard(type);
        Transform parentContainer = GetCardsParentPosContainer(type);
        float PosY = 0;
        if (EPlayerPositionType.ESelf == type)
        {
            PosY = GlobalData.mSelectCardToTargetPos.y;
        }
        else if (EPlayerPositionType.ERight == type && isLast)
        {
            PosY = -100;
        }

        List<SingleCard> singleCardList = new List<SingleCard>();
        if (list.Count > 0)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject item = GameObject.Instantiate(singleCardPrefab) as GameObject;
                item.transform.SetParent(parentContainer);
                item.transform.localPosition = new Vector3(GetOtherCardsPos(type, i, count).x, PosY, 0);
                item.transform.localScale = isLast ? Vector3.one * 0.5f : Vector3.one;
                SingleCard singleCard = item.transform.GetComponent<SingleCard>();
                singleCard.SetOtherData(list[i], type, true);
                singleCardList.Add(singleCard);
            }
        }
        else
        {
            GameObject item = GameObject.Instantiate(singleCardPrefab) as GameObject;
            item.transform.SetParent(parentContainer);
            item.transform.localPosition = new Vector3(GetOtherCardsPos(type, 0, 1).x, PosY, 0);
            item.transform.localScale = isLast ? Vector3.one * 0.5f : Vector3.one;
            SingleCard singleCard = item.transform.GetComponent<SingleCard>();
            singleCard.SetOtherData(GlobalData.mRefuseCardNumId, type, true);
        }
        if (isLast)
        {
            List<List<SingleCard>> cardDic = new List<List<SingleCard>>();
            SelfCardsManager.Instance.InitRefreshRealCardDic(singleCardList, cardDic);
            SetLastCardPosAndDepth(cardDic); // 刷新底部牌
        }
    }
    #endregion

    /// <summary>
    /// 道具
    /// </summary>
    /// <param name="g"></param>
    void OnHeadClick(GameObject g)
    {
        //To be added...
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
        Transform startPos = GetHeadParent(startType);

        if (propInfo.target_id == 0)
            endType = EPlayerPositionType.ESelf;

        Transform endPos = GetHeadParent(endType);
        TweenManager.Instance.PlayPropsAnimation(propInfo, this.transform, startPos, endPos);
        PlayerInfo.Instance.UpdateGold(-(int)propInfo.action_id);
    }
    void UpdateSortCard(LocalNotification e)
    {
        ArgsCard args = e.param as ArgsCard;
        SelfCardsManager.Instance.OrderRealDicToList(args.type == TGuanDanCT.CT_TONG_HUA_SHUN ? 0 : 100);
        SetCardPosAndDepth(SelfCardsManager.Instance.mRealCardDic); // 刷新底部牌
        SelfCardsManager.Instance.ClearAllSelectCards();

        AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameSortCard);
    }

    /// <summary>
    /// 使用表情
    /// </summary>
    /// <param name="e"></param>
    void UseEmoticon(LocalNotification e)   //to be added...
    {
        Debug.Log("表情发送成功");
        EPlayerPositionType type = EPlayerPositionType.ENull;
        ArgsEmoticonInfo args = e.param as ArgsEmoticonInfo;
        //表情
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
                emoticonTweenGm.transform.SetParent(this.GetCardsParentPosContainer(type).parent);
            }
            emoticonTweenGm.transform.localScale = Vector3.one * 0.6f;
            emoticonTweenGm.transform.localPosition = Vector3.zero + new Vector3(0, -13, 0);
            PlayerInfo.Instance.AddNewEmoticon(type, emoticonTweenGm, true);//规定了什么时候销毁 没办法用协成 dotween的回调 ，每个对象都有属于自己的生存时长      

            //问题：时间被固定了，不好停止 ，因为管理类没有继承monobehavior 没办法调用停止协成的方法
            //StartCoroutine(TweenManager.Instance.WaitDestroyEmoticonTween(stateInfo.length * 2, type));
            return;
        }
        //聊天或者语音
        GameObject gm = UIManagers.Instance.GetSingleUI(UIType.ChatMessageTip);
        if (args.action_id == PlayerInfo.Instance.mPlayerPid)
        {
            type = EPlayerPositionType.ESelf;
            gm.transform.SetParent(selfHeadSpr.transform.parent);
            gm.transform.localPosition = selfHeadSpr.transform.localPosition;
        }
        else
        {
            type = RoomInfo.Instance.GetPlayerPosById(args.action_id);
            gm.transform.SetParent(this.GetCardsParentPosContainer(type).parent);
            gm.transform.localPosition = Vector3.zero;
        }


        PlayerInfo.Instance.AddNewEmoticon(type, gm, false);
        if (args.message != null && args.message.Length != 0)
        {
            StartCoroutine(gm.GetComponent<UIChatMessagTipView>().WaitOneDelta(args.message, null, type));
        }
    }
    Transform GetHeadParent(EPlayerPositionType type)
    {
        switch (type)
        {
            case EPlayerPositionType.ELeft:
                return leftHeadSpr.transform.parent;
            case EPlayerPositionType.ERight:
                return rightHeadSpr.transform.parent;
            case EPlayerPositionType.ETop:
                return topHeadSpr.transform.parent;
            case EPlayerPositionType.ESelf:
                return selfHeadSpr.transform;
        }
        return null;
    }


    void InitDragDropArrow() //To be added...
    {
        dragDropArrowPrefab = UIManagers.Instance.GetSingleUI(UIType.DragDropArrow).transform;
        dragDropArrowPrefab.SetParent(mParentContainer.parent);
        dragDropArrowPrefab.localPosition = new Vector3(5000, -70, 0);
        dragDropArrowPrefab.localScale = Vector3.one;
    }



    void ReleaseDragDropArrowPos()
    {
        dragDropArrowPrefab.localPosition = new Vector3(5000, -70, 0);
    }
    /// <summary>
    /// 根据 mRealCardDic改变化，View层重新刷新。
    /// </summary>
    /// <param name="e"></param>
    void StartDragDrop(LocalNotification e)
    {
        ArgsStartDragDrop args = e.param as ArgsStartDragDrop;
        if (args != null)
        {
            uint mCurId = args.mCardId;
            var selfManager = SelfCardsManager.Instance;
            int nextIdxPos = 0;
            selfManager.OrderRealDicToList(mCurId, out nextIdxPos);
            selfManager.mInitDrogDropIdx = nextIdxPos;
            SetCardPosAndDepth(selfManager.mRealCardDic);
            AdjustSelectCardPosAndDepthInReal();
            selfManager.SendShakeNormalCards();
            selfManager.SetSibling(selfManager.mCurSelectCardIds);
        }
    }


    void ReleaseDragDrop(LocalNotification e)
    {
        ReleaseDragDropArrowPos();
    }

    void RefreshDragDropPosAndDepth(LocalNotification e)
    {
        SetCardPosAndDepth(SelfCardsManager.Instance.mRealCardDic);
        AdjustSelectCardPosAndDepthInReal();
    }


    void DisRoomResultSuc(LocalNotification e) //存在问题，会清空房间号(需要解决一个，刚好没有解散房间界面，也没有总结算界面，但是房间号已经解散的问题有)
    {
        ContextManager.Instance.Pop(UIType.VoteDismissView.Name);

        if (ContextManager.Instance.IsContains(UIType.TotalBureauOverView.Name)) { return; }

        Debug.Log("解散结果---成功");
        if (RoomInfo.Instance.IsZhuandanGameType || RoomInfo.Instance.IsGuandan2GameType)
        {
            MsgGlobal mG = new MsgGlobal();
            mG.room_id = RoomInfo.Instance.mRoomId;
            TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_TOTAL_GOAL,mG);
        }
        //掼蛋非打2，第一把 直接退出，第二把及以上，掉大结算
        else
        {
            ContextManager.Instance.Push(new TotalBureauOverViewContext());
            GameObject gameBureau = UIManagers.Instance.GetSingleUI(UIType.TotalBureauOverView);
            ArgsMsgGameOverMsg argsGameOver = new ArgsMsgGameOverMsg();
            //第一局数据自己造
            if (PlayingGameInfo.Instance.mGameInfo.game_index == 1)
            {
                PlayingGameInfo.Instance.mGameOverInfoList = CreateEndInfo();
            }
            else
            {
                if (PlayingGameInfo.Instance.mGameOverInfoList.Count == 0)
                {
                    PlayingGameInfo.Instance.mGameOverInfoList = PlayingGameInfo.Instance.GetmGameOverInfoList();
                }
            }
            for (int i = 0; i < PlayingGameInfo.Instance.mGameOverInfoList.Count; i++)
            {
                var itemInfo = PlayingGameInfo.Instance.mGameOverInfoList[i];
                argsGameOver.mGameOverList.Add(itemInfo);
            }
            gameBureau.GetComponent<UITotalBureauOverView>().SetDissData(argsGameOver);
        }
    }
    private List<MsgGameOverInfo> CreateEndInfo()
    {
        List<MsgGameOverInfo> mGameOverInfoList = new List<MsgGameOverInfo>();
        ArgsMsgRoomInfo roominfo = RoomInfo.Instance.mRoom;
        MsgGameOverInfo gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = PlayerInfo.Instance.mPlayerPid;//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = RoomInfo.Instance.GetIdByName("left");//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = RoomInfo.Instance.GetIdByName("right");//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = RoomInfo.Instance.GetIdByName("top");//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);
        return mGameOverInfoList;
    }

    void UseTalk(LocalNotification e)
    {
        if (e == null)
        {
            Debug.LogError("语音为空");
            return;
        }
        Debug.Log("正在播放语音");
        EPlayerPositionType type = EPlayerPositionType.ENull;
        ArgsTalk args = e.param as ArgsTalk;
        Debug.Log("玩家" + args.talkPid + "正在发送语音");
        GameObject gm = UIManagers.Instance.GetSingleUI(UIType.ChatMessageTip);
        gm.transform.localScale = Vector3.one;

        if (args.talkPid == PlayerInfo.Instance.mPlayerPid)
        {
            type = EPlayerPositionType.ESelf;
            gm.transform.parent = selfHeadSpr.transform;
        }
        else
        {
            type = RoomInfo.Instance.GetPlayerPosById(args.talkPid);
            gm.transform.parent = this.GetCardsParentPosContainer(type).parent;
        }
        gm.transform.localPosition = Vector3.zero;

        PlayerInfo.Instance.AddNewEmoticon(type, gm, false);
        if (args.voiceName != null && args.voiceName.Length != 0)
        {
            StartCoroutine(gm.GetComponent<UIChatMessagTipView>().WaitOneDelta(null, args.voiceName, type));
        }

    }
    void UpdateTributeSuc(LocalNotification e)
    {
        ArgsTribute args = e.param as ArgsTribute;
        if (args != null)
        {
            var selfId = PlayerInfo.Instance.mPlayerPid;

            var SendPosType = RoomInfo.Instance.GetPlayerPosById(args.removeId);
            var ToPosType = RoomInfo.Instance.GetPlayerPosById(args.addId);
            uint cardId = args.card;

            GameObject item = GameObject.Instantiate(singleCardPrefab, GetCardsParentPosContainer(SendPosType));
            item.GetComponent<SingleCard>().SetSprName(cardId, "");
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = new Vector3(0.66f, 0.66f, 0.66f);

            item.transform.parent = GetLightParentPosContainer(ToPosType);
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(DOTween.To(() => item.transform.localPosition, x => item.transform.localPosition = x, Vector3.zero, 1));
            mySequence.AppendInterval(2.5f);
            mySequence.AppendCallback(() => { Destroy(item); }
                );
            mySequence.Play();
        }

    }


   
    //private void FixedUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //         PlayingGameInfo.Instance.SendReconnectServer();
    //    }
    //}



}
