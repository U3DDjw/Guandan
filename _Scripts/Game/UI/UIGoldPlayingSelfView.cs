using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DNotificationCenterManager;
using HaoyunFramework;
using MsgContainer;
using ZhiWa;

/// <summary>
/// 记得玩家自身的牌以及个人信息（状态）
/// </summary>
public class UIGoldPlayingSelfView : MonoBehaviour
{
    [SerializeField]
    RawImage selfHeadSpr;
    [SerializeField]
    Text selfName;
    [SerializeField]
    Text scoreLab;//玩家的分数 .（金币场中金币数）
    [SerializeField]
    Transform cardItemContainer;
    [SerializeField]
    GameObject lookBtn;
    GameObject singleCardPrefab;
    Transform dragDropArrowPrefab;
    GameObject guidePrefab;
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
    GameObject duijiaIcon;
    [SerializeField]
    GameObject voiceBtn;

    [SerializeField]
    GameObject goldReadyGameBtn;

    [SerializeField]
    GameObject goldExchangeBtn;

    void InitPrefab()
    {
        singleCardPrefab = ResourceManager.Instance.LoadAsset<GameObject>(UIType.SingleCard.Path);
    }
    void InitPlayerPosGM()
    {
        GoldFiledManager.Instance.AddPlayerPosGMToDic(EPlayerPositionType.ESelf, selfHeadSpr.gameObject);
    }
    private void Awake()
    {
        InitPrefab();
        InitPlayerPosGM();

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldRefreshStartGame, RefreshStartGame);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ECanPutOutCard, UpdateCanPutOut);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUpdateTip, UpdateRecevTip);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGameClean, UpdateGameClean);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldOverBureau, UpdateOverBureau); //单局结算结束
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EChooseCardType, SetChoseCardType);//牌型选择
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ESortCard, UpdateSortCard); //理牌成功

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EStartDragDrop, StartDragDrop);//开始拖拽
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReleaseDragDrop, ReleaseDragDrop); //结束拖拽
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldRefreshStartGame, RefreshStartGame);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ECanPutOutCard, UpdateCanPutOut);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUpdateTip, UpdateRecevTip);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGameClean, UpdateGameClean);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldOverBureau, UpdateOverBureau); //单局结算结束
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EChooseCardType, SetChoseCardType);//牌型选择
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ESortCard, UpdateSortCard); //理牌成功

        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EStartDragDrop, StartDragDrop);//开始拖拽
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReleaseDragDrop, ReleaseDragDrop); //结束拖拽

    }
    // Use this for initialization
    void Start()
    {
        AddEventTriggerListener();
        InitLicense();
        InitSelfData();
        InitDragDropArrow();
    }

    void AddEventTriggerListener()
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
        EventTriggerListener.Get(selfHeadSpr.gameObject).onClick = OnHeadClick;
        EventTriggerListener.Get(goldExchangeBtn.gameObject).onClick = OnGoldReadyGameClick;
        EventTriggerListener.Get(goldReadyGameBtn.gameObject).onClick = OnGoldExchangeClick;

    }


    void RefreshGold()
    {
        scoreLab.text = PlayerInfo.Instance.GetCurGold.ToString();
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


    void ReleaseDragDropArrowPos()
    {
        dragDropArrowPrefab.localPosition = new Vector3(5000, -70, 0);
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

    void RefreshAllData(LocalNotification e)
    {
        SelfCardsManager.Instance.ClearAllSelectCards();
        ArgsRefreshData args = e.param as ArgsRefreshData;
        if (args != null)
        {
            var roomInfo = RoomInfo.Instance;

            InitLicense();
            InitSelfData();
            //检查亮牌           
         //   ReConnectShowLightCard();
            //刷新其他玩家剩余牌数
            IEnumerator<MsgPlayerInfo> iPlayersCardLastNum = args.playerInfoList.GetEnumerator();
            while (iPlayersCardLastNum.MoveNext())
            {
                var posType = roomInfo.GetPlayerPosById(iPlayersCardLastNum.Current.player_id);
                var lastCardCount = (int)iPlayersCardLastNum.Current.remaindCarsNum;
              //  playerRestCardNum[posType] = lastCardCount;
               // SetRestCardNum(lastCardCount, posType);
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
                       // RefreshPlayerCards(posType, iPlayersLast3Card.Current.card, false);
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


    void InitLicense()
    {
        ClearLicense();
        var selfManager = SelfCardsManager.Instance;
        var data = PlayingGameInfo.Instance.mSelfBaseCardIdList;
        Debug.Log("Card Count :" + data.Count);
        for (int i = 0; i < data.Count; i++)
        {
            InitSingleCard(data[i]);
        }
        selfManager.InitRefreshRealCardDic(selfManager.mCardDataList, selfManager.mRealCardDic);
        ReSortLicense();
        SetCardPosAndDepth(selfManager.mRealCardDic); // 刷新底部牌
    }

    void RefreshStartGame(LocalNotification e)
    {
        InitLicense();
        InitSelfData();
    }

    void ClearPutOutingCard()
    {
        DestroyChildren(outTargetPos);

    }


    void InitDragDropArrow() //To be added...
    {
        dragDropArrowPrefab = UIManagers.Instance.GetSingleUI(UIType.DragDropArrow).transform;
        dragDropArrowPrefab.SetParent(cardItemContainer.parent);
        dragDropArrowPrefab.localPosition = new Vector3(5000, -280, 0);
        dragDropArrowPrefab.localScale = Vector3.one;
    }



    void SetChoseCardType(LocalNotification e)
    {
        ArgsCanputOutCard args = e.param as ArgsCanputOutCard;
        if (args.msgCardGroup != null) //判断是否有两种重复类型需要选择
        {
            ShowCardCTSelect(args.msgCardGroup);
        }
    }

    void UpdateOverBureau(LocalNotification e)
    {
        DestroyChildren(cardItemContainer);
        duijiaIcon.gameObject.SetActive(false);
        ClearCurBureauGame();
        RefreshGold();
    }

    void UpdateCanPutOut(LocalNotification e)
    {
        ArgsCanputOutCard msg = e.param as ArgsCanputOutCard;
        if (null != msg)
        {
            var curPlayerPosType = RoomInfo.Instance.GetPlayerPositionType(msg.playerId);
            if (curPlayerPosType == EPlayerPositionType.ESelf)
            {
                ClearPutOutingCard();
            }

            var lastPlayerPosType = RoomInfo.Instance.GetPlayerPositionType(msg.lastPlayerId);
            //验证自动刷新牌
            if (lastPlayerPosType == EPlayerPositionType.ESelf)
            {
                SetSelfPutOutSuc(e);
            }

            //检测清空游状态的最后一轮牌
            CheckClearCleanCard(lastPlayerPosType);

            Debug.Log("msg.DjCards:" + msg.DjCards.Count);
            CheckShowTeamerCards(msg.DjCards); //展示对家的牌
        }
    }

    void CheckClearCleanCard(EPlayerPositionType posType)
    {
        //检测刚刚出牌玩家的下家是否是游，是的话，清空上家牌；检测上上家的牌是否是游 ，如果是，则清空
        var nextCleanPosType = PlayingGameInfo.Instance.GetNeedLastClearOutCards(posType);
        if (nextCleanPosType != posType)
        {
            if (nextCleanPosType == EPlayerPositionType.ESelf) { ClearPutOutingCard(); }
            var lastCleanPosType = PlayingGameInfo.Instance.GetNeedLastClearOutCards(nextCleanPosType); //检测上家的上家，可能头游，二游连续出现的情况，需要清空二游牌
            if (lastCleanPosType != nextCleanPosType)
            {
                if (lastCleanPosType == EPlayerPositionType.ESelf)
                {
                    ClearPutOutingCard();
                }
            }
        }

    }

    void UpdateRecevTip(LocalNotification e)
    {
        AchieveCardTipFunc();
    }

    void UpdateGameClean(LocalNotification e)
    {
        ArgsGameCleanInfo msg = e.param as ArgsGameCleanInfo;
        if (msg != null)
        {
            if (msg.playerId == PlayerInfo.Instance.mPlayerPid)
            {
                Debug.Log("GameClean:" + msg.playerId + "Rank:" + msg.rank);
                CleanSelfGameStatus(msg.rank);
            }
        }
    }

    /// <summary>
    /// 实现实际的Tip功能
    /// </summary>
    void AchieveCardTipFunc()
    {
        var manager = SelfCardsManager.Instance;
        manager.ClearAllSelectCards();
        if (manager.IsNoHaveTipCard())
        {
            OnRefusePutOutCardClick();
            return;
        }
        manager.SendTipSelectCard();
    }


    /// <summary>
    /// 不要 
    /// </summary>
    /// <param name="g"></param>
    void OnRefusePutOutCardClick()
    {
        AudioManager.Instance.PlayClickBtnAudio();
        SelfCardsManager.Instance.SendRefusePutOutCard();
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
        DestroyChildren(cardItemContainer);

        var data = djList;
        curTeamerCardCount = data.Count;
        List<SingleCard> cardList = new List<SingleCard>();
        List<List<SingleCard>> dic = new List<List<SingleCard>>();
        for (int i = 0; i < curTeamerCardCount; i++)
        {
            GameObject item = Instantiate(singleCardPrefab);
            item.transform.SetParent(cardItemContainer);
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
            GameObject item = Instantiate(singleCardPrefab, cardItemContainer);
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
    void InitSingleCard(uint i)
    {
        GameObject item = GameObject.Instantiate(singleCardPrefab) as GameObject;
        item.transform.SetParent(cardItemContainer);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        SingleCard singleCard = item.GetComponent<SingleCard>();
        singleCard.SetCardData(i);
        singleCard.mIsIn = true;
        SelfCardsManager.Instance.mCardDataList.Add(singleCard);
    }

    void ReSortLicense()
    {
        int sortCardRecordCount = SelfCardsManager.Instance.mSortRecordList.Count;
        if (sortCardRecordCount > 0)
        {
            for (int i = 0; i < sortCardRecordCount; i++)
            {
                AutoSortRealCard(i);
            }
            SelfCardsManager.Instance.ClearAllSelectCards();
        }
    }

    void ClearLicense()
    {
        DestroyChildren(cardItemContainer);
        SelfCardsManager.Instance.mCardDataList.Clear();
        SelfCardsManager.Instance.ClearAllSelectCards();
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
    public void ClearCurBureauGame()
    {
        ClearOutCards(); //清空打出的牌
        ClearLightCard();//清空明牌数据
        selfGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false; //重置 头游状态
    }


    void CleanSelfGameStatus(MsgGuandanGameRank rank)
    {
        selfGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
        string spritePath = GlobalData.GetHeadCleanCpr((uint)rank);
        Sprite spr = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, spritePath);
        selfGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
    }
    void ClearOutCards()
    {
        DestroyChildren(outTargetPos);
    }

    void ClearLightCard()
    {
        if (selfHeadSpr.transform.childCount > 0)
        {
            DestroyChildren(selfHeadSpr.transform.GetChild(0));//子物体somePos 与其他的player 统一而已
        }
    }


    void InitSelfData()
    {
        ClearCurBureauGame();
        var roomInfo = RoomInfo.Instance;
        var playingInfo = PlayingGameInfo.Instance;
        var dic = roomInfo.mPlayersDic;
        string url = PlayerInfo.Instance.mPlayerData.headPortrait;
        StartCoroutine(GlobalData.GetHeadTextureByIdx(selfHeadSpr, url));
        selfName.text = PlayerInfo.Instance.mPlayerData.name;

        ShowSelfScore();
        RefreshGold();
    }

    void ShowSelfScore()
    {
        Color winColor = new Color(1, 229 / 255.0f, 141 / 255.0f, 1);
        Color loseColor = new Color(86 / 255.0f, 180 / 255.0f, 1, 1);
        var roomInfo = RoomInfo.Instance;
    }

    #region Btn Function
    float curLookBgTime = 0;
    float totalLookBgTime = 0.15f;
    int PressLookBgId = 0; //0，表示没有按，1表示，按，2表示按了实现方法了
    void OnLookBgClick(GameObject g, bool isPress)
    {
        if (SelfCardsManager.Instance.mIsDragDroping || SelfCardsManager.Instance.mIsDowing) { return; }
        SelfCardsManager.Instance.mIsPressingBg = isPress;
        PressLookBgId = isPress == true ? 1 : 0;
        //Debug.Log("Look bg : isPress" + isPress);
        if (isPress)
        {
            OnClearSelectCardIdxClick(null);
        }
        else
        {
            curLookBgTime = 0;
            OnClearSelectCardIdxClick(null);
        }
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


    void OnGoldReadyGameClick(GameObject g)
    {

    }


    void OnGoldExchangeClick(GameObject g)
    {

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

    void OnMessageClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new EmoticonViewContext());
    }

    void OnVoiceClick(GameObject g, bool isPress)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        OpenVoiceView(isPress);
    }

    GameObject voiceGm = null;
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
    #endregion


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
            item.transform.SetParent(cardItemContainer);
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

}


