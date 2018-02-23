using DG.Tweening;
using DNotificationCenterManager;
using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZhiWa;
using HaoyunFramework;
using UnityEngine.UI;

public class UIGameInfoView : MonoBehaviour
{
    [SerializeField]
    Text selfIndexLab;

    [SerializeField]
    Text enemyIndexLab;
    //GameInfo
    [SerializeField]
    Text roomInfoLab;

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
    UIPlayingGameView playingView;


    [SerializeField]
    GameObject jinggongContainer;

    [SerializeField]
    GameObject leftInfoIconBtn;
    [SerializeField]
    GameObject leftInfoContainer;
    [SerializeField]
    GameObject KangGongIcon;
    [SerializeField]
    Text gameNumLab;//第几局
    [SerializeField]
    Text versionLab;//版本号
    [SerializeField]
    GameObject autoBtn;
    Sequence mySeque = null;
    bool IsShowLeftInfo = false;
    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver); //小结算
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ETotalBureauOverGame, UpdateTotalBureauOverGame);//总结算
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EInitLicense, InitGame); //初始化游戏数据
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EPlayerPlayingStatus, UpdatePlayerPutCard);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EFirstPutOutCard, UpdateFirstPutOut);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ETributeSuc, UpdateTributeSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EUpdateTip, UpdateRecevTip);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EZhuandanStartGame, UpdateZhuandanStart);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EStopAllCoroutines, StopCoroutines);//关闭协程
    }


    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ETotalBureauOverGame, UpdateTotalBureauOverGame);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EInitLicense, InitGame);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EPlayerPlayingStatus, UpdatePlayerPutCard);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EFirstPutOutCard, UpdateFirstPutOut);

        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ETributeSuc, UpdateTributeSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EUpdateTip, UpdateRecevTip);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EZhuandanStartGame, UpdateZhuandanStart);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EStopAllCoroutines, StopCoroutines);//关闭协程

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
        EventTriggerListener.Get(leftInfoIconBtn).onClick = OnLeftInfoClick;

        CheckShowAutoBtn();
        if (GameManager.Instance.mGameMode == EGameMode.EDebug)
        {
            EventTriggerListener.Get(autoBtn).onClick = OnAutoPlayClick;
            autoBtn.SetActive(true);
        }
        playingView = UIManagers.Instance.GetSingleUI(UIType.PlayingGameView).GetComponent<UIPlayingGameView>();
        InitRoomInfo();
        WaitingTween();
        InitTween();

    }

    void CheckShowAutoBtn()
    {
        if (GameManager.Instance.mGameMode == EGameMode.EDebug)
        {
            autoBtn.SetActive(true);
            EventTriggerListener.Get(autoBtn).onClick = OnAutoPlayClick;
        }


    }

    void InitTween() //====================To be added...
    {
        mySeque = DOTween.Sequence();
        mySeque.Append(leftInfoContainer.transform.DOLocalMove(leftInfoContainer.transform.localPosition + new Vector3(250, 0, 0), 0.5f));
        mySeque.SetAutoKill(false);
        mySeque.Pause();
    }
    void OnLeftInfoClick(GameObject g)//====================To be added...
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
    // 关闭协程
    void StopCoroutines(LocalNotification e)
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// zf 只供游戏的第一局使用，为了实现进房间就有房间信息的感觉，重连也会调用一次，但是刷新会覆盖这个
    /// </summary>
    void InitRoomInfo()
    {
        versionLab.text = "版本号:" + GlobalData.mVersion;
        string gametype = RoomInfo.Instance.GetgameTypeString();

        uint game_Index = 1;
        if (game_Index == 0)
        {
            game_Index = 1;
        }
        if (RoomInfo.Instance.mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeZhuanDan || RoomInfo.Instance.mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeGuanDan)
            gameNumLab.text = string.Format("{0}/{1}局", game_Index, RoomInfo.Instance.mRoom.game_num);
        else
            gameNumLab.text = string.Format("第{0}局", game_Index);
        roomInfoLab.text = string.Format("房间号:{0}\n{1}", RoomInfo.Instance.mRoomCode, gametype);//第几局
        selfIndexLab.text = PlayerInfo.Instance.mTeamSelfIndex;
        enemyIndexLab.text = PlayerInfo.Instance.mTeamEnemyIndex;
        //versionLab.text = "版本号:" + GlobalData.mVersion;
    }
    /// <summary>
    /// 等待中。。。 动画
    /// </summary>
    void WaitingTween()//====================To be added...
    {
        int childCount = 6;
        List<Transform> listWaitTween = new List<Transform>();
        for (int i = 0; i < childCount; i++)
        {
            listWaitTween.Add(jinggongContainer.transform.GetChild(0).transform.GetChild(i));
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
    void InitGame(LocalNotification e) //初始化 房间游戏信息，按钮全部置false
    {
        AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameStartGame);
        InitGameInfo();
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
    void UpdateRecevTip(LocalNotification e)
    {
        AchieveCardTipFunc();
    }

    void UpdateZhuandanStart(LocalNotification e)
    {
        Debug.Log("转蛋Start...");
        var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(firstPlayerId);
        SetArrowPlayer(curPlayerPosType);
        ShowStatusBtn(firstPlayerId == PlayerInfo.Instance.mPlayerPid, true);
    }
    void RefreshAllData(LocalNotification e)
    {
        ArgsRefreshData args = e.param as ArgsRefreshData;
        if (args != null)
        {
            InitGameInfo();

            if (args.action == null || args.action.action_id == 0)
            {
                return;
            }
            var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(args.action.action_id);
            firstPlayerId = args.action.action_id;
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
    ulong firstPlayerId = 0;


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

            CheckPlayjiefengTween(isJiefeng, curPlayerPosType);

            if (args.playerId == selfId)
            {
                StartCoroutine(CheckAutoPlay());
            }

            playingView.ClearPutOutingCard(curPlayerPosType);
        }
    }
    void CheckPlayjiefengTween(bool isJiefeng, EPlayerPositionType posType)
    {
        if (isJiefeng)
        {
            TweenManager.Instance.PlayJiefengTween(playingView.GetCardsParentPosContainer(posType), playingView.GetLightParentPosContainer(posType), posType);
        }
    }

    void UpdateFirstPutOut(LocalNotification e)
    {
        Debug.Log("First Player Put Show");
        ArgsFirstPutPlayer msg = e.param as ArgsFirstPutPlayer;
        if (msg != null)
        {
            if (GameManager.Instance.mIsUseAI)
            {
                firstPlayerId = PlayerInfo.Instance.mPlayerPid;
            }
            else
            {
                firstPlayerId = msg.playerId;
            }
            if (msg.isStart)
            {
                //掼蛋第一把 需要一张明牌 表示谁先出牌
                if (RoomInfo.Instance.IsZhuandanGameType || PlayingGameInfo.Instance.mGameInfo.game_index == 1)
                {
                    
                    CheckCloseClock();
                    playingView.CheckShowLightCard();
                }
                else
                {
                    var curPlayerPosType = RoomInfo.Instance.GetPlayerPosById(msg.playerId);
                    SetArrowPlayer(curPlayerPosType);
                    ShowStatusBtn(msg.playerId == PlayerInfo.Instance.mPlayerPid, true);
                }
            }
            RefreshTributeData(msg.isStart);
        }
    }



    /// <summary>
    /// 初始化房间信息
    /// </summary>
    void InitGameInfo()
    {
        //versionLab.text = "版本号:" + GlobalData.mVersion;
        CheckCloseClock();

        tributeBtn.SetActive(false);
        backTributeBtn.SetActive(false);

        RefreshView();
    }

    void RefreshView()
    {
        var roomInfo = RoomInfo.Instance;
        var playingInfo = PlayingGameInfo.Instance;
        selfIndexLab.text = PlayerInfo.Instance.mTeamSelfIndex;
        enemyIndexLab.text = PlayerInfo.Instance.mTeamEnemyIndex;
        //游戏类型
        string gametype = roomInfo.GetgameTypeString();
        uint game_index = playingInfo.mGameInfo.game_index;
        if (game_index == 0)
        {
            game_index = 1;
        }
        if (RoomInfo.Instance.mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeZhuanDan || RoomInfo.Instance.mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeGuanDan)
            gameNumLab.text = string.Format("{0}/{1}局", game_index, RoomInfo.Instance.mRoom.game_num);
        else
            gameNumLab.text = string.Format("第{0}局", game_index);
        roomInfoLab.text = string.Format("房间号:{0}\n{1}", roomInfo.mRoomCode, gametype);//第几局
    }
    IEnumerator LoadGameBureauOver(LocalNotification e)
    {
        yield return new WaitForSeconds(GlobalData.mLoadGameBreauOverWaitTime);
        ArgsMsgGameOverMsg msg = e.param as ArgsMsgGameOverMsg;
        if (msg != null)
        {
            playingView.ClearCurBureauGame();
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
                gameBureau.GetComponent<UIBureauOverView>().SetItems(msg.mGameOverList, msg.mIsTotalOver); //To be added...
            }
            RefreshView();
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
            yield return new WaitForSeconds(0.8f);

            var manager = SelfCardsManager.Instance;
            if (isCardTipYbq)
            {
                OnRefusePutOutCardClick(null);
            }
            else
            {
                OnTipCardClick(null);
                yield return new WaitForSeconds(0.5f);
                OnPutOutCardsClick(null);
            }

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
        if (!(PlayingGameInfo.Instance.mPlayingStatus == EPlayingStatus.ETribute))
            CheckOpenClock();
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
                //播放抗贡动画
                //IEnumerator<ulong> iPlayers = manager.mTributeInfo.jg_player_id.GetEnumerator();
                //while (iPlayers.MoveNext())
                //{
                //    var playerPosType = RoomInfo.Instance.GetPlayerPositionType(iPlayers.Current);
                //    TweenManager.Instance.PlayKangGongTween(playingView.GetCardsParentPosContainer(playerPosType), playerPosType);//动画改为抗贡动画

                //}
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
                    playingView.ChangeBaseSelfCard(args.card, true);
                }
                if (args.removeId == selfId)
                {
                    tributeBtn.gameObject.SetActive(false);
                    playingView.ChangeBaseSelfCard(args.card, false);
                }
            }
            else
            {
                strInfo = string.Format("{0}向{1},回贡{2}", removeName, addName, cardName);
                if (args.addId == selfId)
                {
                    playingView.ChangeBaseSelfCard(args.card, true);
                }

                if (args.removeId == selfId)
                {
                    backTributeBtn.gameObject.SetActive(false);
                    playingView.ChangeBaseSelfCard(args.card, false);
                }

                if (args.isStart)
                {
                    // StartCoroutine(TributeOverToStartGame());
                    PlayingGameInfo.Instance.ClearTributeInfo();
                    PlayingGameInfo.Instance.mPlayingStatus = EPlayingStatus.EPlaying;
                    RefreshJGStatus(false);
                    if (firstPlayerId == selfId)
                    {
                        CheckStartGame(true);
                    }
                    else
                    {
                        ShowStatusBtn(false, false);
                        var playerPos = RoomInfo.Instance.GetPlayerPosById(firstPlayerId);
                        SetArrowPlayer(playerPos);
                    }
                }

            }
            UIManagers.Instance.EnqueueTip(strInfo);
        }
    }
    #endregion
}
