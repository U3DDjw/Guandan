using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZhiWa;
using HaoyunFramework;
using UnityEngine.UI;
using DNotificationCenterManager;
using MsgContainer;
using DG.Tweening;

public class UIGoldPlayingStatusView : MonoBehaviour
{

    [SerializeField]
    Image clockSpr;

    [SerializeField]
    GameObject tributeBtn;
    [SerializeField]
    GameObject backTributeBtn;

    [SerializeField]
    GameObject refusePutOutBtn;

    [SerializeField]
    GameObject putOutBtn;

    [SerializeField]
    GameObject tipCardBtn;

    [SerializeField]
    GameObject clockObj;

    [SerializeField]
    GameObject selfStatusContainer;
    UIGoldPlayingSelfView selfView;


    [SerializeField]
    GameObject jinggongContainer;
    [SerializeField]
    GameObject KangGongIcon;

    [SerializeField]
    GameObject autoBtn;


    [SerializeField]
    GameObject processModeContainer;

    private void Awake()
    {

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver); //小结算
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EPlayerPlayingStatus, UpdatePlayerPutCard);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldRefreshStartGame, StartGame);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ETributeSuc, UpdateTributeSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldOverBureau, UpdateOverBureau); //单局结算结束

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldReadySuc, GoldReadySuc); //金币场准备成功 

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连
      
    }


    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EPlayerPlayingStatus, UpdatePlayerPutCard);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldRefreshStartGame, StartGame);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ETributeSuc, UpdateTributeSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldOverBureau, UpdateOverBureau); //单局结算结束
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldReadySuc, GoldReadySuc); //金币场准备成功 
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连


        AudioManager.Instance.StopEffectAudio(GlobalData.AudioNameDaojishi);
    }
    private void Start()
    {
        AudioManager.Instance.IsPlayBackgroundAudio(false);
        EventTriggerListener.Get(tributeBtn).onClick = OnTributeClick;
        EventTriggerListener.Get(backTributeBtn).onClick = OnBackTributeClick;
        EventTriggerListener.Get(putOutBtn).onClick = OnPutOutCardsClick;
        EventTriggerListener.Get(refusePutOutBtn).onClick = OnRefusePutOutCardClick;
        EventTriggerListener.Get(tipCardBtn).onClick = OnTipCardClick;
        EventTriggerListener.Get(processModeContainer.transform.GetChild(1).gameObject).onClick = OnExchangeRoomClick;
        EventTriggerListener.Get(processModeContainer.transform.GetChild(2).gameObject).onClick = OnReadyGameClick;

        CheckShowBtn();

        if (GameManager.Instance.mGameMode == EGameMode.EDebug)
        {
            EventTriggerListener.Get(autoBtn).onClick = OnAutoPlayClick;
            autoBtn.SetActive(true);
        }

        selfView = UIManagers.Instance.GetSingleUI(UIType.GoldPlayingSelfView).GetComponent<UIGoldPlayingSelfView>();
    }


    /// <summary>
    /// 用于第一局显示按钮
    /// </summary>
    void CheckShowBtn()
    {
        var firstPlayerId = GoldFiledManager.Instance.mFirstPlayerId;
        var isStart = GoldFiledManager.Instance.mIsStartGame;
        Debug.Log("First Player Put Show" + firstPlayerId + "isStart:" + isStart);


        if (isStart)
        {
            //掼蛋第一把 需要一张明牌 表示谁先出牌
            if (PlayingGameInfo.Instance.mGameInfo.game_index == 1)
            {
                CheckCloseClock();
                Debug.Log("明牌动画播放开始..");
                TweenLightCard();
            }
            else
            {
                var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(firstPlayerId);
                SetArrowPlayer(curPlayerPosType);
                ShowStatusBtn(firstPlayerId == PlayerInfo.Instance.mPlayerPid, true);
            }
        }

        // RefreshTributeData(isStart);
    }
    private void TweenLightCard()
    {
        uint lightCardId = PlayingGameInfo.Instance.mGameInfo.light_card;
        var lightPlayerList = PlayingGameInfo.Instance.mGameInfo.light_player_id;
        Debug.Log("Show Light Card:" + lightCardId + "人数:" + lightPlayerList.Count);
        if (lightPlayerList.Count == 0) //没有亮牌
        {
            ShowStartGameBtn();
            return;
        }
        else
        {
            GameObject singleCardPrefab = ResourceManager.Instance.LoadAsset<GameObject>(UIType.SingleCard.Path);
            int listCardCount = 1;
            //转蛋模式 和 掼蛋第一局 有明牌
            if (RoomInfo.Instance.IsZhuandanGameType)
                listCardCount = 2;
            else if (PlayingGameInfo.Instance.mGameInfo.game_index == 1)
                listCardCount = 1;

            //一个人有两张明牌的情况
            bool isSamePlayerLight = false;

            //listCardCount = 2;//测试
            //烟雾也用的这个位置
            float posY = 180 - 90 / 2 + 72 / 2;//头像位置y轴-头像height/2+card的Height/2
            for (int i = 0; i < listCardCount; i++)
            {
                if (i == 1) isSamePlayerLight = lightPlayerList.Count == 1;

                ulong curPlayerId = isSamePlayerLight ? lightPlayerList[0] : lightPlayerList[i];
                EPlayerPositionType playerPos = RoomInfo.Instance.GetPlayerPositionType(curPlayerId);
                GameObject playerPosGm = GoldFiledManager.Instance.GetPlayerPosGameobject(playerPos);
                GameObject item = GameObject.Instantiate(singleCardPrefab, this.transform);

                item.transform.localScale = Vector3.one;

                float posX = i == 0 ? -31 : 31;//small 的宽度62/2
                if (listCardCount == 1) posX = 0;//掼蛋一张明牌的时候位置为0

                item.transform.localPosition = new Vector3(posX, posY, 0);

                item = SetLightCardInfo(lightCardId, item, playerPosGm.transform);
                Vector3 itemEndPos = GetLightCardEndPos(isSamePlayerLight, playerPos, item.transform);
                StartLightCardTween(item, itemEndPos);
            }
        }
    }
    private GameObject SetLightCardInfo(uint cardId, GameObject lightCardItem, Transform parent)
    {
        lightCardItem.transform.SetParent(parent.GetChild(0));
        SingleCard item = lightCardItem.GetComponent<SingleCard>();
        item.mCurStatus = ECardStatus.ELightCard;
        item.SetSprName(cardId, "small");
        item.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
        item.transform.GetChild(0).GetChild(0).GetComponent<Image>().raycastTarget = false;
        item.transform.GetChild(0).GetChild(1).GetComponent<Image>().raycastTarget = false;
        return lightCardItem;
    }
    private Vector3 GetLightCardEndPos(bool isSamePlayer, EPlayerPositionType type, Transform item)
    {
        //1，8为阴影误差
        Vector2 size = item.GetComponent<RectTransform>().rect.size;
        Vector3 endPos = new Vector3();
        switch (type)
        {
            case EPlayerPositionType.ESelf:
            case EPlayerPositionType.ETop:
            case EPlayerPositionType.ELeft:
                float endX = -50 + 62 / 2 / 2 + 8;//parent.width/2+card.width/2/2[scale原来的0.5]
                float endY = -50 + 70 / 2 / 2 + 8;
                endPos = new Vector3(endX, endY, 0);
                if (isSamePlayer) //true了肯定是第二张
                {
                    endPos += new Vector3(30, 0, 0);//62 / 2 >>31,35为缩小的扑克牌大小 误差【阴影】为1
                }
                break;
            case EPlayerPositionType.ERight:
                float endXR = /*item.parent.localPosition.x*/ +50 - 62 / 2 / 2 - 8;//parent.width+card.width/2/2[scale原来的0.5]
                float endYR = /*item.parent.localPosition.y*/ -50 + 70 / 2 / 2 + 8;
                endPos = new Vector3(endXR, endYR, 0);
                if (isSamePlayer) //true了肯定是第二张
                {
                    endPos -= new Vector3(31 - 1, 0, 0);
                }
                break;

        }
        return endPos;
    }
    private void StartLightCardTween(GameObject lightCardItem, Vector3 endPos)
    {
        lightCardItem.transform.localScale = Vector3.zero;
        float posY = 180 - 90 / 2 + 72 / 2;//头像位置y轴-头像height/2+card的Height/2;
        TweenManager.Instance.PlayLightCardSmokeAnim(transform, posY);//1.7秒这样

        float unitTime = 1f;
        Sequence lightTween = DOTween.Sequence();
        lightTween.AppendInterval(unitTime * 1.7f);
        Tween scaleToOne = lightCardItem.transform.DOScale(1, unitTime * 0.3f).SetEase(Ease.Linear);
        Tween move = lightCardItem.transform.DOLocalMove(endPos, unitTime).SetEase(Ease.Linear);
        Tween scaleOneToSmall = lightCardItem.transform.DOScale(0.5f, unitTime * 0.3f).SetEase(Ease.Linear);
        lightTween.Append(scaleToOne);
        lightTween.Append(move);
        lightTween.Join(scaleOneToSmall);
        lightTween.AppendCallback(
            () =>
            {
                ShowStartGameBtn();
            });
        lightTween.Play();
        lightTween.SetAutoKill();
    }
    void InitGame(LocalNotification e) //初始化 房间游戏信息，按钮全部置false
    {
        AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameStartGame);
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
            OnRefusePutOutCardClick(null);
            return;
        }
        manager.SendTipSelectCard();
    }


    void SetGoldReadyStatus(bool isReady)
    {
        processModeContainer.transform.GetChild(2).gameObject.gameObject.SetActive(!isReady);
        processModeContainer.transform.GetChild(1).gameObject.SetActive(!isReady);
        processModeContainer.transform.GetChild(0).gameObject.SetActive (isReady);
    }


    void OnExchangeRoomClick(GameObject g)
    {
        this.transform.parent.GetComponent<UIGoldPlayingGameView>().ClosePlayingView();
        GoldFiledManager.Instance.SendExchangeRoomServer();
    }

    void OnReadyGameClick(GameObject g)
    {
        GoldFiledManager.Instance.SendReadyGoldStartGameServer();
    }


    /// <summary>
    /// 提示按钮
    /// </summary>
    /// <param name="g"></param>
    void OnTipCardClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (SelfCardsManager.Instance.mIsRecevCardTip)
        {
            AchieveCardTipFunc();
        }
        else
        {
            SelfCardsManager.Instance.SendServerCardTip();
        }
    }


    /// <summary>
    /// 不要 
    /// </summary>
    /// <param name="g"></param>
    void OnRefusePutOutCardClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (IsMustPutOutCard())
        {
            UIManagers.Instance.EnqueueTip("必须要出牌");
            return;
        }
        SelfCardsManager.Instance.SendRefusePutOutCard();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSelf"></param>
    /// <param name="isGray">必须出牌</param>
    void ShowStatusBtn(bool isSelf, bool isMustPut)
    {
        refusePutOutBtn.SetActive(isSelf);
        tipCardBtn.SetActive(isSelf);
        putOutBtn.SetActive(isSelf);
        if (isSelf) //自己的情况，刷新 不出按钮
        {
            string spriteName = isMustPut ? "pk_btn_font4_black" : "pk_btn_font4";
            refusePutOutBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, spriteName);

            string spriteName2 = isMustPut ? "pk_btn_font4_black" : "pk_btn_font4_click";
            refusePutOutBtn.GetComponent<Button>().spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EPlaying, spriteName2);
            refusePutOutBtn.GetComponent<Button>().enabled = !isMustPut;
            refusePutOutBtn.GetComponent<Image>().raycastTarget = !isMustPut;
        }
    }

    void CheckStartGame(bool isStart)
    {
        if (isStart)
        {
            ShowStatusBtn(true, true);
            SetArrowPlayer(EPlayerPositionType.ESelf);
            //干掉进贡中
        }
    }



    /// <summary>
    /// 进贡状态
    /// </summary>
    /// <param name="isShow"></param>


    bool isShowJGStatus = false; //避免重复加载JGStatus
    void RefreshJGStatus(bool isShow)
    {
        if (isShow)
        {
            if (!isShowJGStatus)
            {
                jinggongContainer.SetActive(true);
                isShowJGStatus = true;
                PlayingGameInfo.Instance.mPlayingStatus = EPlayingStatus.ETribute;
            }
        }
        else
        {
            if (isShowJGStatus)
            {
                jinggongContainer.SetActive(false);
                isShowJGStatus = false;
                PlayingGameInfo.Instance.mPlayingStatus = EPlayingStatus.EPlaying;
            }
        }

    }


    public void OnPutOutCardsClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (SelfCardsManager.Instance.mIsOkPutOutCard)
        {
            //获取选中的卡片，通过方法转为，服务器需要的数据，发送
            SelfCardsManager.Instance.SendPutOutCard();
        }
    }

    void OnTributeClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        var manager = SelfCardsManager.Instance;
        if (manager.mCurSelectCardIds.Count == 1)
        {
            var t = manager.mCurSelectCardIds[0];
            PlayingGameInfo.Instance.SendTribute(t);
        }
        else
        {
            UIManagers.Instance.EnqueueTip("只能选一张");
        }
    }

    void OnBackTributeClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        var manager = SelfCardsManager.Instance;
        if (manager.mCurSelectCardIds.Count == 1)
        {
            var t = manager.mCurSelectCardIds[0];
            PlayingGameInfo.Instance.SendBackTribute(t);
        }
        else
        {
            UIManagers.Instance.EnqueueTip("只能选一张");
        }

    }
    bool IsMustPutOutCard()
    {
        return refusePutOutBtn.GetComponent<Image>().sprite == ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "btn_black");
    }


    bool isCardTipYbq = false;

    /// <summary>
    /// 判断是否弹要不起
    /// </summary>
    /// <param name="isYbq"></param>
    void CheckShowYQTip(bool isYbq)
    {
        isCardTipYbq = isYbq;
        if (isYbq)
        {
            UIManagers.Instance.EnqueueTip("要不起");
        }
    }


    void CloseProcessModeBtn()
    {
        processModeContainer.SetActive(false);
    }


    void OpenProcessModeBtn()
    {
        processModeContainer.SetActive(true);
        SetGoldReadyStatus(false);
    }


    void StartGame(LocalNotification e)
    {
        Debug.Log("StartGame...");

        CloseProcessModeBtn();
        //显示第一位出牌玩家按钮,及箭头显示指向
        CheckOpenClock();
        var firstPlayerId = GoldFiledManager.Instance.mFirstPlayerId;
        var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(firstPlayerId);
        SetArrowPlayer(curPlayerPosType);
        bool isSelfStart = curPlayerPosType == EPlayerPositionType.ESelf;
        ShowStatusBtn(firstPlayerId == PlayerInfo.Instance.mPlayerPid, isSelfStart);
        RefreshTributeData(GoldFiledManager.Instance.mIsStartGame);
    }

    void UpdatePlayerPutCard(LocalNotification e) //只转箭头
    {
        ArgsPlayerPut args = e.param as ArgsPlayerPut;
        if (args != null)
        {
            var selfId = PlayerInfo.Instance.mPlayerPid;
            var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(args.playerId);
            bool isJiefeng = args.type == MsgActionType.MsgActionTypeJiefeng;
            bool isMustPutCard = args.lastOperationId == selfId || isJiefeng;

            CheckShowYQTip(args.isYbq);
            ShowStatusBtn(args.playerId == PlayerInfo.Instance.mPlayerPid, isMustPutCard);
            SetArrowPlayer(curPlayerPosType);

            if (isJiefeng) //展示借风动画
            {
                PlayJiefengTween(curPlayerPosType);
            }

            if (args.playerId == selfId)
            {
                StartCoroutine(CheckAutoPlay());
            }
        }
    }

    #region 托管出牌

    void OnAutoPlayClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        PlayingGameInfo.Instance.mIsAutoPlay = !PlayingGameInfo.Instance.mIsAutoPlay;
        autoBtn.transform.GetChild(0).GetComponent<Image>().enabled = PlayingGameInfo.Instance.mIsAutoPlay;
    }
    /// <summary>
    /// 托管出牌(本地缓存ybq变量，ybq从gdoc中获取是否有可出牌)
    /// </summary>
    /// <param name="isSelf"></param>
    IEnumerator CheckAutoPlay()
    {

        if (PlayingGameInfo.Instance.mIsAutoPlay)
        {
            float speed = 0.3f;
            yield return new WaitForSeconds(speed);

            var manager = SelfCardsManager.Instance;

            if (isCardTipYbq)
            {
                OnRefusePutOutCardClick(null);
            }
            else
            {
                OnTipCardClick(null);
                yield return new WaitForSeconds(speed);
                OnPutOutCardsClick(null);
            }

        }
    }

    #endregion

    void PlayJiefengTween(EPlayerPositionType posType)
    {
        Debug.Log("Jiefeng Tween:" + posType);
    }

    IEnumerator LoadGameBureauOver(LocalNotification e)
    {
        yield return new WaitForSeconds(GlobalData.mLoadGameBreauOverWaitTime);
        ArgsMsgGameOverMsg msg = e.param as ArgsMsgGameOverMsg;
        if (msg != null)
        {
            bool isOver = false;
            #region 下局打几 和加载总结算还是小结算      
            //当前玩家的队
            EPlayersTeam mTeamtype = (EPlayersTeam)RoomInfo.Instance.GetPlayerInfoById(PlayerInfo.Instance.mPlayerPid).teamType;
            bool isGuandanNotTwo = !RoomInfo.Instance.IsZhuandanGameType && !RoomInfo.Instance.IsGuandan2GameType;//是否是掼蛋非打二
                                                                                                                  //组别是为了结算显示的打到几的 颜色相同
            for (int i = 0; i < msg.mGameOverList.Count; i++)
            {
                //当前id判断是否是己方
                ulong pid = msg.mGameOverList[i].player_id;

                var nowPlayerInfo = RoomInfo.Instance.GetPlayerInfoById(pid);
                EPlayersTeam pt = (EPlayersTeam)nowPlayerInfo.teamType;
                if (isGuandanNotTwo)
                {
                    //头游
                    if (pid == PlayerInfo.Instance.mPlayerPid)//是当前的玩家id
                    {
                        PlayerInfo.Instance.mTeamSelfIndex = msg.mGameOverList[i].upgrade_card.ToString(); //己方打几
                    }
                    else if (pt != mTeamtype)
                    {
                        PlayerInfo.Instance.mTeamEnemyIndex = msg.mGameOverList[i].upgrade_card.ToString();//对方打几
                    }

                    if (msg.mGameOverList[i].rank == MsgGuandanGameRank.MsgGuandanGameRankFirst)//头游打几 决定下一局打几
                    {
                        RoomInfo.Instance.mNextIndex = msg.mGameOverList[i].upgrade_card.ToString();
                    }
                }
                isOver = msg.mGameOverList[i].over == 1;
            }

            #endregion
            //——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
            if (isOver && isGuandanNotTwo)//掼蛋的情况
            {
                Debug.Log("加载掼蛋非打二总结算");
                UpdateTotalBureauOverGame(e);
            }
            else
            {
                Debug.Log("加载打二情况小结算");
                ContextManager.Instance.Push(new BureauOverContext());
                GameObject gameBureau = UIManagers.Instance.GetSingleUI(UIType.BureauOverView);
                gameBureau.GetComponent<UIBureauOverView>().SetItems(msg.mGameOverList, msg.mIsTotalOver);
            }
        }

        SelfCardsManager.Instance.ClearInfo();
        PlayingGameInfo.Instance.ClearCurGameInfo();
    }
    void UpdateGameBureauOver(LocalNotification e)
    {
        CheckCloseClock();
        ShowStatusBtn(false, false);

        StartCoroutine(LoadGameBureauOver(e));
    }

    /// <summary>
    /// 总结算 初始化
    /// </summary>
    /// <param name="e"></param>
    void UpdateTotalBureauOverGame(LocalNotification e)
    {
        if (!UIManagers.Instance.IsDicContainsType(UIType.TotalBureauOverView.Name))
        {
            ContextManager.Instance.Push(new TotalBureauOverViewContext());
            UIManagers.Instance.GetSingleUI(UIType.TotalBureauOverView).GetComponent<UITotalBureauOverView>().SetData(e);
        }
    }
    void CheckOpenClock()
    {
        if (!clockObj.activeSelf)
        {
            clockObj.SetActive(true);
        }
    }

    void CheckCloseClock()
    {
        if (clockObj.activeSelf)
        {
            clockObj.SetActive(false);
        }
    }

    void SetArrowPlayer(EPlayerPositionType posType)
    {
        clockObj.GetComponent<ClockRun>().ResetStartRun(posType);
        switch (posType)
        {
            case EPlayerPositionType.ESelf:
                clockSpr.transform.localPosition = new Vector3(0, -50, 0);
                break;
            case EPlayerPositionType.ETop:
                clockSpr.transform.localPosition = new Vector3(0, 50, 0);
                break;
            case EPlayerPositionType.ELeft:
                clockSpr.transform.localPosition = new Vector3(-425, 25, 0);
                break;
            case EPlayerPositionType.ERight:
                clockSpr.transform.localPosition = new Vector3(400, 25, 0);
                break;
        }
    }
    #region 贡牌
    /// <summary>
    /// 刷新贡牌数据
    /// </summary>
    void RefreshTributeData(bool isStart)
    {
        var manager = PlayingGameInfo.Instance;
        if (isStart)
        {
            if (manager.IsNoTribute) //抗贡状态
            {
                TweenManager.Instance.PlayKangGongTween(KangGongIcon);//动画改为抗贡动画
            }

            tributeBtn.gameObject.SetActive(false);
            backTributeBtn.SetActive(false);
        }
        else //贡的状态
        {
            RefreshJGStatus(true);
            CheckCloseClock();
            ShowStatusBtn(false, false);
            if (manager.IsJGPlayer)
            {
                if (manager.mTributeInfo.jgpz_card_native == 0) //有没有进贡，null代表没有进贡
                {
                    tributeBtn.gameObject.SetActive(true);
                }
                else //有进贡
                {
                    tributeBtn.gameObject.SetActive(false);
                    if (manager.mTributeInfo.hgpz_card_native != 0) //有回贡
                    {
                        ShowTributeCard(manager.mTributeInfo.hgpz_card_native);
                    }
                }
            }
            else if (manager.IsHGPlayer)
            {
                if (manager.mTributeInfo.jgpz_card_native != 0) //有被进贡
                {
                    if (manager.mTributeInfo.hgpz_card_native != 0) //有回贡
                    {
                        backTributeBtn.gameObject.SetActive(false);
                        ShowTributeCard(manager.mTributeInfo.hgpz_card_native);
                    }
                    else
                    {
                        backTributeBtn.gameObject.SetActive(true);
                    }

                }
            }
            else
            {
                //添加 Tip，谁进贡
                backTributeBtn.gameObject.SetActive(false);
                tributeBtn.gameObject.SetActive(false);
            }

        }

    }

    void ShowTributeCard(uint cardId)
    {
        List<uint> list = new List<uint>();
        list.Add(cardId);
        SelfCardsManager.Instance.PostSendCardToSelectStatus(list, ECardStatus.ESelected);
    }


    void ShowStartGameBtn()
    {
        var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(GoldFiledManager.Instance.mFirstPlayerId);
        Debug.Log("转蛋Start..." + curPlayerPosType);
        CheckOpenClock();
        SetArrowPlayer(curPlayerPosType);
        ShowStatusBtn(curPlayerPosType == EPlayerPositionType.ESelf, true);
    }



    void RefreshAllData(LocalNotification e)
    {
        ArgsRefreshData args = e.param as ArgsRefreshData;
        if (args != null)
        {
            if (args.action == null || args.action.action_id == 0)
            {
                OpenProcessModeBtn();
                return;
            }
            var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(args.action.action_id);
            GoldFiledManager.Instance.mFirstPlayerId = args.action.action_id;
            var selfId = PlayerInfo.Instance.mPlayerPid;
            bool isJiefeng = args.action.action_type == MsgActionType.MsgActionTypeJiefeng;
            bool isMustPutCard = args.action.last_operation_pid == selfId || isJiefeng || args.action.last_operation_pid == 0;
            if (PlayingGameInfo.Instance.mTributeInfo != null && PlayingGameInfo.Instance.mTributeInfo.start_tag != 1)
            {
                RefreshTributeData(false);
                ShowStatusBtn(false, false);
            }
            else
            {

                ShowStatusBtn(args.action.action_id == PlayerInfo.Instance.mPlayerPid, isMustPutCard);
            }
            SetArrowPlayer(curPlayerPosType);
        }
    }


    void GoldReadySuc(LocalNotification e)
    {
        ArgsPlayerList args = e.param as ArgsPlayerList;
        if (args != null && args.playerIdList.Contains(PlayerInfo.Instance.mPlayerPid))
        {
            SetGoldReadyStatus(true);
        }
    }

    void UpdateOverBureau(LocalNotification e)
    {
        //显示换房间，准备按钮
        OpenProcessModeBtn();
    }


    void UpdateTributeSuc(LocalNotification e)
    {
        ArgsTribute args = e.param as ArgsTribute;
        if (args != null)
        {
            var selfId = PlayerInfo.Instance.mPlayerPid;
            string strInfo = "";

            string removeName = RoomInfo.Instance.GetPlayerInfoById(args.removeId).name;
            string addName = RoomInfo.Instance.GetPlayerInfoById(args.addId).name;
            string cardName = SelfCardsManager.Instance.getCardNameFromInt((int)args.card);
            if (args.isJingong)
            {
                strInfo = string.Format("{0}向{1},进贡{2}", removeName, addName, cardName);
                if (args.addId == selfId)
                {
                    backTributeBtn.gameObject.SetActive(true);
                    selfView.ChangeBaseSelfCard(args.card, true);
                }
                if (args.removeId == selfId)
                {
                    tributeBtn.gameObject.SetActive(false);
                    selfView.ChangeBaseSelfCard(args.card, false);
                }
            }
            else
            {
                strInfo = string.Format("{0}向{1},回贡{2}", removeName, addName, cardName);
                if (args.addId == selfId)
                {
                    selfView.ChangeBaseSelfCard(args.card, true);
                }

                if (args.removeId == selfId)
                {
                    backTributeBtn.gameObject.SetActive(false);
                    selfView.ChangeBaseSelfCard(args.card, false);
                }

                if (args.isStart)
                {
                    // StartCoroutine(TributeOverToStartGame());
                    PlayingGameInfo.Instance.ClearTributeInfo();
                    PlayingGameInfo.Instance.mPlayingStatus = EPlayingStatus.EPlaying;
                    RefreshJGStatus(false);
                    if (GoldFiledManager.Instance.mFirstPlayerId == selfId)
                    {
                        CheckStartGame(true);
                    }
                    else
                    {
                        ShowStatusBtn(false, false);
                        var playerPos = RoomInfo.Instance.GetPlayerPosById(GoldFiledManager.Instance.mFirstPlayerId);
                        SetArrowPlayer(playerPos);
                    }
                }

            }
            UIManagers.Instance.EnqueueTip(strInfo);
        }
    }

    #endregion
}
