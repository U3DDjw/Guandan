using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNotificationCenterManager;
using HaoyunFramework;
using MsgContainer;
using ZhiWa;
using UnityEngine.UI;
public class UIGoldPlayingPlayerView : MonoBehaviour
{
    [SerializeField]
    RawImage rightHeadSpr;
    [SerializeField]
    RawImage topHeadSpr;
    [SerializeField]
    RawImage leftHeadSpr;
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
    //Other Players Pos
    [SerializeField]
    Transform playerPosRight;

    [SerializeField]
    Transform playerPosTop;

    [SerializeField]
    Transform playerPosLeft;
    GameObject singleCardPrefab;
    //玩家状态，头游啥的
    [SerializeField]
    Image leftGameCleanSpr;
    [SerializeField]
    Image rightGameCleanSpr;
    [SerializeField]
    Image topGameCleanSpr;

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

    void InitPrefab()
    {
        singleCardPrefab = ResourceManager.Instance.LoadAsset<GameObject>(UIType.SingleCard.Path);
    }
    void InitPlayerPosGM()
    {
        GoldFiledManager.Instance.AddPlayerPosGMToDic(EPlayerPositionType.ETop, topHeadSpr.gameObject);
        GoldFiledManager.Instance.AddPlayerPosGMToDic(EPlayerPositionType.ERight, rightHeadSpr.gameObject);
        GoldFiledManager.Instance.AddPlayerPosGMToDic(EPlayerPositionType.ELeft, leftHeadSpr.gameObject);
    }

    void Awake()
    {
        InitPrefab();
        InitPlayerPosGM();

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ECanPutOutCard, UpdateCanPutOut);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGameClean, UpdateGameClean);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver); //小结算
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOnLinePlayer, UpdateOnLinePlayer); //玩家上线
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EOfflinePlayer, UpdateOffLinePlayer); //玩家下线
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldOverBureau, UpdateOverBureau); //单局结算结束
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldReadySuc, GoldReadySuc); //金币场准备成功 


        playerRestCardNum = new Dictionary<EPlayerPositionType, int>() {
            { EPlayerPositionType.ERight,GlobalData.mPlayerOwnCardCount },
            { EPlayerPositionType.ETop,GlobalData.mPlayerOwnCardCount },
            { EPlayerPositionType.ELeft,GlobalData.mPlayerOwnCardCount },
        };
    }

    void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ECanPutOutCard, UpdateCanPutOut);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGameClean, UpdateGameClean);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGameBureauOver, UpdateGameBureauOver); //小结算
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EOnLinePlayer, UpdateOnLinePlayer); //玩家上线
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EOfflinePlayer, UpdateOffLinePlayer); //玩家下线
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldOverBureau, UpdateOverBureau); //单局结算结束
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldReadySuc, GoldReadySuc); //金币场准备成功 


    }
    // Use this for initialization
    void Start()
    {
        AddEventTriggerListener();
        InitPlayerData();
    }
    void AddEventTriggerListener()
    {

    }

    void GoldReadySuc(LocalNotification e)
    {
        ArgsPlayerList args = e.param as ArgsPlayerList;
        if (args != null)
        {
              RefreshOtherPlayerReadyStatus(args.playerIdList);
        }
    }

    void RefreshOtherPlayerReadyStatus(List<ulong> playerList)
    {
        bool isShow = false;
        //left
        var leftId = RoomInfo.Instance.GetPlayerIdByPos(EPlayerPositionType.ELeft);
        isShow = playerList.Contains(leftId);
        leftHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = isShow;

        //right
        var rightId = RoomInfo.Instance.GetPlayerIdByPos(EPlayerPositionType.ERight);
        isShow = playerList.Contains(rightId);
        rightHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = isShow;
        //top

        var topId = RoomInfo.Instance.GetPlayerIdByPos(EPlayerPositionType.ETop);
        isShow = playerList.Contains(topId);
        topHeadSpr.transform.parent.Find("ReadyTip").GetComponent<Image>().enabled = isShow;
    }


    void RefreshAllData(LocalNotification e)
    {
        ArgsRefreshData args = e.param as ArgsRefreshData;
        if (args != null)
        {
            var roomInfo = RoomInfo.Instance;
            InitPlayerData();
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

        }
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

    /// <summary>
    /// Inits the player data.（游戏，名字）
    /// </summary>
    void InitPlayerData()
    {
        var roomInfo = RoomInfo.Instance;
        string rightUrl = roomInfo.GetPlayerHeadPortraitByPos((int)EPlayerPositionType.ERight);
        StartCoroutine(GlobalData.GetHeadTextureByIdx(rightHeadSpr, rightUrl));

        string topUrl = roomInfo.GetPlayerHeadPortraitByPos((int)EPlayerPositionType.ETop);
        StartCoroutine(GlobalData.GetHeadTextureByIdx(topHeadSpr, topUrl));

        string leftUrl = roomInfo.GetPlayerHeadPortraitByPos((int)EPlayerPositionType.ELeft);
        StartCoroutine(GlobalData.GetHeadTextureByIdx(leftHeadSpr, leftUrl));

        rightName.text = roomInfo.GetPlayerNameByPos((int)EPlayerPositionType.ERight);
        topName.text = roomInfo.GetPlayerNameByPos((int)EPlayerPositionType.ETop);
        leftName.text = roomInfo.GetPlayerNameByPos((int)EPlayerPositionType.ELeft);
    }

    /// <summary>
    /// Refreshs the out cards.
    /// </summary>
    /// <param name="type">Type.</param>
    void RefreshOutCards(int type)
    {

    }


    void RefreshScore(int type)
    {

    }

    /// <summary>
    /// 刷新头游状态
    /// </summary>
    /// <param name="type">Type.</param>
    void RefreshGameClean(int type)
    {

    }

    /// <summary>
    /// 刷新玩家剩余牌数目
    /// </summary>
    /// <param name="type">Type.</param>
    void RefreshResetCardNum(int type)
    {

    }
    //其他人出牌，刷新其他人出牌,依次三次，第三次（playerid为上家）为上家出牌，需要记录保存，
    void UpdateCanPutOut(LocalNotification e)
    {
        ArgsCanputOutCard msg = e.param as ArgsCanputOutCard;
        if (null != msg)
        {
            var curPlayerPosType = RoomInfo.Instance.GetPlayerPositionType(msg.playerId);

            //清空当前需要出牌的玩家的牌
            if (curPlayerPosType != EPlayerPositionType.ESelf)
            {
                ClearPutOutingCard(curPlayerPosType);
            }

            //显示刚刚出牌的玩家的牌
            var lastPlayerPosType = RoomInfo.Instance.GetPlayerPositionType(msg.lastPlayerId);
            Debug.Log("刚刚出牌玩家:" + lastPlayerPosType.ToString() + "类型" + msg.putOutType);
            if (lastPlayerPosType != EPlayerPositionType.ESelf)
            {
                SetOtherData(lastPlayerPosType, msg.cards, msg.putOutType);
            }
            //检测清空游状态最后一轮牌
            CheckClearCleanCard(lastPlayerPosType);

        }
    }

    void CheckClearCleanCard(EPlayerPositionType posType)
    {
        //检测刚刚出牌玩家的下家是否是游，是的话，清空上家牌；检测上上家的牌是否是游 ，如果是，则清空
        var nextCleanPosType = PlayingGameInfo.Instance.GetNeedLastClearOutCards(posType);
        if (nextCleanPosType != posType)
        {
            if (nextCleanPosType != EPlayerPositionType.ESelf)
            {
                ClearPutOutingCard(nextCleanPosType);
            }
            var lastCleanPosType = PlayingGameInfo.Instance.GetNeedLastClearOutCards(nextCleanPosType); //检测上家的上家，可能头游，二游连续出现的情况，需要清空二游牌
            if (lastCleanPosType != nextCleanPosType)
            {
                if (lastCleanPosType != EPlayerPositionType.ESelf)
                {
                    ClearPutOutingCard(lastCleanPosType);
                }
            }
        }

    }


    void UpdateOverBureau(LocalNotification e)
    {
        //清空一些界面结算数据（他人玩家的剩余的展示的牌）
        ClearPutOutingCard(EPlayerPositionType.ERight);
        ClearPutOutingCard(EPlayerPositionType.ELeft);
        ClearPutOutingCard(EPlayerPositionType.ETop);

        //清空其他玩家游的状态
        ClearPlayerCleanState();

        //清空玩家剩余牌数目
        ClearAllResetCardNum();
    }

    
    void ClearPlayerCleanState()
    {
        rightGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false;
        leftGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false;
        topGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = false;
    }


    void UpdateGameBureauOver(LocalNotification e)
    {
        //展示最后出牌的玩家
        ArgsMsgGameOverMsg args = e.param as ArgsMsgGameOverMsg;
        if (args != null)
        {
            Debug.Log("展示最后出牌玩家剩余的牌的玩家数量:" + args.mShowLastCard.Count);
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


    void UpdateGameClean(LocalNotification e)
    {
        ArgsGameCleanInfo msg = e.param as ArgsGameCleanInfo;
        if (msg != null)
        {
            Debug.Log("GameClean:" + msg.playerId + "Rank:" + msg.rank);

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
            if (playerId != PlayerInfo.Instance.mPlayerPid)
            {
                CleanOtherGameStatus(RoomInfo.Instance.GetPlayerPositionType((uint)playerId), rank);
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
                rightGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
                rightGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
                break;
            case EPlayerPositionType.ELeft:
                leftGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
                leftGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
                break;
            case EPlayerPositionType.ETop:
                topGameCleanSpr.transform.GetChild(0).GetComponent<Image>().enabled = true;
                topGameCleanSpr.transform.GetChild(0).GetComponent<Image>().sprite = spr;
                break;
        }
    }
    //显示其他人出牌的信息
    void SetOtherData(EPlayerPositionType type, List<uint> list, TGuanDanCT putOutType)
    {
        //为解决 玩家不出的bug
        int lastCardCount = Mathf.Clamp(playerRestCardNum[type] - list.Count, 0, GlobalData.mPlayerOwnCardCount); //(减完当前牌)剩余牌的张数
        SetPlayerCardsPos(type, list, putOutType);
        SetRestCardNum(lastCardCount, type);
    }


    void ClearAllResetCardNum()
    {
        SetRestCardNum(20,EPlayerPositionType.ELeft);
        SetRestCardNum(20,EPlayerPositionType.ERight);
        SetRestCardNum(20,EPlayerPositionType.ETop);
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
    /// <summary>
    /// 清空当前已经打出的牌
    /// </summary>
    /// <param name="posType"></param>
    public void ClearPutOutingCard(EPlayerPositionType posType)
    {
        DestroyChildren(GetCardsParentPosContainer(posType));
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
        }
        return playerPosRight;
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
}

