using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
using UnityEngine.UI;
using MsgContainer;
using ZhiWa;
using DG.Tweening;

public class UIPlayBackView : BasesView
{
    #region  定义私有可见变量
    [SerializeField]
    GameObject mainBg;
    [SerializeField]
    Transform loadingIcon;
    [SerializeField]
    Text playProgress;//播放进度
    [SerializeField]
    GameObject kgIconGm;//抗贡图标
    [SerializeField]
    Transform isShowClockBtn;//展示闹钟选择框
    //右边
    [SerializeField]
    Transform rightHeadFrame;
    [SerializeField]
    Transform rightPosContainer;
    [SerializeField]
    Transform rightPosContainerCardItem;
    [SerializeField]
    Transform rightName;
    [SerializeField]
    Transform rightResetCard;
    [SerializeField]
    Transform rightClock;
    //左边
    [SerializeField]
    Transform leftHeadFrame;
    [SerializeField]
    Transform leftPosContainer;
    [SerializeField]
    Transform leftName;
    [SerializeField]
    Transform leftResetCard;
    [SerializeField]
    Transform leftPosContainerCardItem;
    [SerializeField]
    Transform leftClock;
    //顶部
    [SerializeField]
    Transform topHeadFrame;
    [SerializeField]
    Transform topPosContainer;
    [SerializeField]
    Transform topName;
    [SerializeField]
    Transform topResetCard;
    [SerializeField]
    Transform topPosContainerCardItem;
    [SerializeField]
    Transform topClock;
    //自己
    [SerializeField]
    Transform selfHeadFrame;
    [SerializeField]
    Transform selfPosContainer;
    [SerializeField]
    Transform selfName;
    [SerializeField]
    Transform selfCardContainer;
    [SerializeField]
    Transform selfClock;
    //房间信息
    [SerializeField]
    GameObject roomInfoContainer;
    [SerializeField]
    Text selfIndexLab;
    [SerializeField]
    Text enemyIndexLab;
    [SerializeField]
    Text gameInfoLab; //第几局 信息
    [SerializeField]
    Text roomCodeAndType;//房间类型和房间号
    //--------------下面的按钮
    [SerializeField]
    GameObject speedBtn;
    [SerializeField]
    GameObject pauseBtn;
    [SerializeField]
    GameObject replayBtn;
    [SerializeField]
    GameObject closeBtn;

    [SerializeField]
    float interTime = 4;//出牌间隔时间
    #endregion
    #region 定义私有不可见变量
    //不需要重新初始化字段
    GameObject singleCardPrefab = null;
    Sequence mySeque = null;
    Sequence tributeTween = null;//贡牌动画信息
    ArgsPlayBackRoomInfo mCurRoomInfo;//上一层传的房间信息
    PlayBackData mPlayBackdata;//本局游戏数据
    Text nextClockText = null;//下一个闹钟显示地点
    bool IsShowLeftInfo = false;
    bool isRequestSuc = false;
    float waitTime = 0.1f;//闹钟的时间间隔
    int fightCount = 0;//战斗数据
    //根据情况重置的数据
    bool isPause = false;//针对于时间的暂停与否  暂停按钮还控制这个字段

    //需要重播初始化字段
    bool isPlay = false;//针对于动画播放完毕 与否实现战斗真正开始【需要ispplay=true,isPause=false】
    bool isReplay = false;//重播
    int speed = 1;
    float curIntervalTime = 4; //当前间隔四秒

    int remainPlayerCount = 4;//剩余玩家数量
    int outIndex = 0;//读取到的数据索引
    int rankIndex = 0;//排到第几名了


    //列表字典 >使用自然就会初始化
    Dictionary<EPlayerPositionType, int> dicCheckCardCount;//计算排名
    Dictionary<EPlayerPositionType, ulong> dicPlayPos;//玩家位置
    List<List<SingleCard>> sortSingleCard;
    List<SingleCard> listLightCardItem;
    Dictionary<ulong, List<SingleCard>> dicListSingleCard;
    Dictionary<ulong, List<List<SingleCard>>> dicListListSingleCards;

    #endregion

    #region >>>>>>>>>>>>>>>>初始化数据
    void Start()
    {
        EventTriggerListener.Get(speedBtn).onClick = OnSpeedClick;
        EventTriggerListener.Get(pauseBtn).onClick = OnPauseClick;
        EventTriggerListener.Get(replayBtn).onClick = OnReplayClick;
        EventTriggerListener.Get(closeBtn).onClick = OnCloseClick;
        EventTriggerListener.Get(isShowClockBtn.gameObject).onClick = OnShowClockBtnClick;
        GameObject infoBtn = roomInfoContainer.transform.GetChild(0).gameObject;
        EventTriggerListener.Get(infoBtn).onClick = OnRoomInfoClick;

        speedBtn.transform.GetChild(0).gameObject.SetActive(false);
        speedBtn.transform.GetChild(1).gameObject.SetActive(false);
        singleCardPrefab = ResourceManager.Instance.LoadAsset<GameObject>(UIType.SingleCard.Path);
        InitRoomInfoTween();
    }
    void InitRoomInfoTween()
    {
        mySeque = DOTween.Sequence();
        Vector3 endPos = roomInfoContainer.transform.localPosition + new Vector3(250, 0, 0);
        mySeque.Append(roomInfoContainer.transform.DOLocalMove(endPos, 0.5f));
        mySeque.SetAutoKill(false);
        mySeque.Pause();

        //加载动画
        loadingIcon.gameObject.SetActive(true);
        LoadView();
    }
    void LoadView()
    {
        GameObject swicthView = UIManagers.Instance.GetSingleUI(UIType.SwitchBtnFunctionView);
        swicthView.GetComponent<SwitchBtnFunction>().IsBackMode = true;
        swicthView.transform.SetParent(mainBg.transform);
        swicthView.transform.SetAsLastSibling();
    }
    private void Update()
    {
        RequestDataing();
    }
    void RequestDataing()
    {
        if (!isRequestSuc)
        {
            loadingIcon.transform.Rotate(Vector3.up, 1);
        }
        else
        {
            loadingIcon.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameIndex">游戏类型</param>
    /// <param name="roomCode">房间号</param>
    /// <param name="gameType">第几局</param>
    public void SetRoomInfo(ArgsPlayBackRoomInfo argsRoomInfo)
    {
        this.mCurRoomInfo = argsRoomInfo;
        //if (GameManager.Instance.mGameMode == EGameMode.EDebug)
        //    PlayerInfo.Instance.mPlayerData.pid = 37660;

        string type = RoomInfo.Instance.GetgameTypeString(mCurRoomInfo.gameType);
        string gameNum = "0"; //需要与GetgameTypeString 中的默认值一样
        if (RoomInfo.Instance.mRoom != null)
            gameNum = RoomInfo.Instance.mRoom.game_num.ToString();

        if (type.Contains(gameNum))//替换为实际的数据
        {
            type = type.Replace(gameNum, mCurRoomInfo.gameTotalNum.ToString());
        }
        roomCodeAndType.text = string.Format("房间号:{0}\n{1}", mCurRoomInfo.roomCode, type);

        GetPlayBackData(argsRoomInfo.gameIndex, argsRoomInfo.roomCodeMd5);
    }
    void GetPlayBackData(int gameIndex, string roomCodeMd5)
    {
        string url = null;
        WWWForm wwwform = new WWWForm();
        wwwform.AddField("index", gameIndex);
        wwwform.AddField("roomId", roomCodeMd5);
        UIManagers.Instance.EnqueueTip("请求回放数据中");
        url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_GetPlayBackInfo;
        StartCoroutine(GlobalData.SendPost(url, wwwform, delegate (WWW www)
         {
             if (www.text != null)
             {
                 string text = www.text;
                 mPlayBackdata = JsonManager.GetPlayBackData(text);
                 RequestDataSuc();
             }
             else
             {
                 isRequestSuc = true;
                 UIManagers.Instance.EnqueueTip("不能加载回放功能上线之前的数据");
             }
         }));
    }
    IEnumerator StartPlayBack()
    {
        while (true)
        {
            if (isPause || !isPlay)
            {
                yield return null;
            }
            else
            {
                PlayBackFightData fightData = mPlayBackdata.fightingPutoutlist[outIndex];
                //本局的
                Debug.Log(string.Format("fightData.pid={0}， fightData.cardList.count={1}，fightData.cardtype={2}", fightData.pid, fightData.cardList.Count, fightData.cardtype));

                InitPutOutCard(fightData.pid, fightData.cardList, (TGuanDanCT)fightData.cardtype);
                CheckShowCleanSpr(fightData.pid);

                //下一位玩家准备
                outIndex++;
                playProgress.text = "播放进度:" + outIndex + " /" + fightCount;
                if (outIndex >= fightCount)//outindex 在语句块中当做长度用
                {
                    UIManagers.Instance.EnqueueTip("本局游戏播放完毕");
                    outIndex = 0;
                    SetPause(true);
                    ShowClock(0);
                }
                else//这outindex在语句块中是当做索引用
                {
                    //闹钟显示在出牌的下一个玩家 
                    ulong nextPlayerId = mPlayBackdata.fightingPutoutlist[outIndex].pid;
                    yield return ClockRun(nextPlayerId);
                }
               
                //清除上一轮
                if (outIndex % remainPlayerCount == 0 && outIndex >= remainPlayerCount)
                {
                    ClearContainerItem(false);
                }
            }
        }
    }
    IEnumerator ClockRun(ulong pid)
    {
        if (ShowClock(pid) == null)
        {
            yield return new WaitForSeconds(curIntervalTime);
            yield break;//不执行下面代码
        }
        GameObject clock = ShowClock(pid);
        Text com_text_showStr = clock.transform.GetChild(0).GetComponent<Text>();
        float hasWaitTime = 0;
        while (hasWaitTime < interTime)
        {
            if (isPause)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(waitTime / speed); //等待时间缩短
                hasWaitTime += waitTime;
                float remainTime = interTime - hasWaitTime;
                com_text_showStr.text = remainTime.ToString("0.0");
                if (remainTime < 0)
                {
                    yield break;
                }
            }
        }
    }
    void RequestDataSuc()
    {
        if (mPlayBackdata == null)
        {
            UIManagers.Instance.EnqueueTip("数据请求失败");
            return;
        }
        isRequestSuc = true;
        fightCount = mPlayBackdata.fightingPutoutlist.Count;
        StartCoroutine(StartPlayBack());
        InitData();
    }
    void InitData()
    {
        if (fightCount > 4) //我需要根据这个获取玩家的信息
        {
            InitOtherRoomInfo();
            InitPlayerInfo();
            InitDataReplay();//重新播放 初始化一些数据
            InItPlayerCard();
            InitLightOrJinggongCard();//初始完毕再开始打牌   
        }
        else
        {
            playProgress.text = "本局无战斗数据";
        }
    }
    /// <summary>
    /// 重放会初始化的一些数据
    /// </summary>  
    void InitDataReplay()
    {
        if (isPause) //播放完毕会暂停，然后重放 就恢复正常播放
        {
            SetPause(false);
        }
        isPlay = false;//控制动画 ispause 控制时间
        outIndex = 0;
        ShowClock(0);
        SetSpeed(1, speedBtn); //初始化了speed 和curIntervalTime
        remainPlayerCount = 4;
        rankIndex = 0;
        playProgress.text = "播放进度:" + 0 + " /" + fightCount;

        dicCheckCardCount = new Dictionary<EPlayerPositionType, int>();
        if (dicPlayPos != null)
        {
            for (int i = 0; i < mPlayBackdata.players.Count; i++)
            {
                for (int posIndex = 1; posIndex < 5; posIndex++)
                {
                    if (dicPlayPos.ContainsKey((EPlayerPositionType)posIndex))
                    {
                        EPlayerPositionType pos = (EPlayerPositionType)posIndex;
                        Transform headFrame = GetTransformOfType(dicPlayPos[pos], ETransformType.EHeadFrame);
                        headFrame.GetChild(0).GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "transParent");
                    }
                }
            }
        }
    }
    void InitOtherRoomInfo()
    {
        if (mCurRoomInfo.gameType == TGuanDanGameType.TGuanDanGameTypeZhuanDan || mCurRoomInfo.gameType == TGuanDanGameType.TGuanDanGameTypeGuanDan)
        {
            enemyIndexLab.text = 2.ToString();
            selfIndexLab.text = 2.ToString();
            gameInfoLab.text = string.Format("{0}/{1}局", mCurRoomInfo.gameIndex, mCurRoomInfo.gameTotalNum);
        }
        else
        {
            int levelEnemy = 2;
            int levelSelf = 2;
            int curLevel = 2;//本局打几
            //在数据请求完毕再设置数据
            List<PlayBackFightData> lastEndings = mPlayBackdata.lastEnding;
            for (int i_endingMember = 0; i_endingMember < lastEndings.Count; i_endingMember++)
            {
                bool isSelf = false;
                List<ulong> listMembers = lastEndings[i_endingMember].member;
                for (int i_member = 0; i_member < listMembers.Count; i_member++)
                {
                    if (listMembers[i_member] == PlayerInfo.Instance.mPlayerPid)
                    {
                        isSelf = true;
                        levelSelf = lastEndings[i_endingMember].level;

                        levelEnemy = i_endingMember == 0 ? lastEndings[1].level : lastEndings[0].level;
                        //我赢了吗？
                        if (lastEndings[i_endingMember].win == 0)
                        {
                            curLevel = levelSelf;
                        }
                        else
                        {
                            curLevel = levelEnemy;
                        }
                        break;
                    }
                }
                if (isSelf)
                {
                    break;
                }
            }

            enemyIndexLab.text = levelEnemy.ToString();
            selfIndexLab.text = levelSelf.ToString();

            gameInfoLab.text = string.Format("第{0}局", mCurRoomInfo.gameIndex.ToString());

            //己方就是A 对方就是B 
            Text selfText = selfName.GetChild(0).GetChild(0).GetComponent<Text>();
            selfText.color = Color.white;
            selfText.text = "本局打:<color=#FFE58DFF>" + curLevel + "</color>";
        }
    }
    void InitPlayerInfo()
    {
        dicPlayPos = InitPlayerPosition();//初始化玩家的位置

        Debug.Log("当前curRoomInfo>>>" + mCurRoomInfo);
        SetPlayInfoData();//积分设置
        PlayBackFightData selfArgs = GetPlayInfoData(dicPlayPos[EPlayerPositionType.ESelf]);
        PlayBackFightData rightArgs = GetPlayInfoData(dicPlayPos[EPlayerPositionType.ERight]);
        PlayBackFightData leftArgs = GetPlayInfoData(dicPlayPos[EPlayerPositionType.ELeft]);
        PlayBackFightData topArgs = GetPlayInfoData(dicPlayPos[EPlayerPositionType.ETop]);

        StartCoroutine(GlobalData.GetHeadTextureByIdx(selfHeadFrame.GetChild(1).GetComponent<RawImage>(), selfArgs.headPortrait));
        StartCoroutine(GlobalData.GetHeadTextureByIdx(rightHeadFrame.GetChild(1).GetComponent<RawImage>(), rightArgs.headPortrait));
        StartCoroutine(GlobalData.GetHeadTextureByIdx(leftHeadFrame.GetChild(1).GetComponent<RawImage>(), leftArgs.headPortrait));
        StartCoroutine(GlobalData.GetHeadTextureByIdx(topHeadFrame.GetChild(1).GetComponent<RawImage>(), topArgs.headPortrait));
        selfName.GetComponent<Text>().text = selfArgs.name;
        rightName.GetComponent<Text>().text = rightArgs.name;
        leftName.GetComponent<Text>().text = leftArgs.name;
        topName.GetComponent<Text>().text = topArgs.name;
        if (mCurRoomInfo.gameType == TGuanDanGameType.TGuanDanGameTypeZhuanDan || mCurRoomInfo.gameType == TGuanDanGameType.TGuanDanGameTypeGuanDan)
        {
            Color winColor = new Color(1, 229 / 255.0f, 141 / 255.0f, 1);
            Color loseColor = new Color(86 / 255.0f, 180 / 255.0f, 1, 1);

            //自己
            selfName.GetChild(0).gameObject.SetActive(true);
            selfName.GetChild(0).GetChild(0).GetComponent<Text>().color = selfArgs.goal >= 0 ? winColor : loseColor;
            selfName.GetChild(0).GetChild(0).GetComponent<Text>().text = selfArgs.goal.ToString();
            //右边
            rightName.transform.GetChild(0).gameObject.SetActive(true);
            rightName.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = rightArgs.goal >= 0 ? winColor : loseColor;
            rightName.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = rightArgs.goal.ToString();//积分lab
            //左边
            leftName.transform.GetChild(0).gameObject.SetActive(true);
            leftName.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = leftArgs.goal >= 0 ? winColor : loseColor;
            leftName.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = leftArgs.goal.ToString();
            //顶部
            topName.transform.GetChild(0).gameObject.SetActive(true);
            topName.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = topArgs.goal >= 0 ? winColor : loseColor;
            topName.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = topArgs.goal.ToString();
        }
        else //掼蛋模式下，非打2的第二把及以上
        {
            topName.GetChild(0).gameObject.SetActive(false);
            leftName.GetChild(0).gameObject.SetActive(false);
            rightName.GetChild(0).gameObject.SetActive(false);
            //在房间信息初始化 中写了打几
        }
    }

    void InItPlayerCard()
    {
        dicListListSingleCards = new Dictionary<ulong, List<List<SingleCard>>>();
        dicListSingleCard = new Dictionary<ulong, List<SingleCard>>();
        List<PlayBackFightData> players = mPlayBackdata.players;
        for (int i = 0; i < players.Count; i++)
        {
            ulong pid = players[i].pid;
            List<SingleCard> listCardItem = new List<SingleCard>();
            PlayBackFightData listCard = GetPlayerCardsData(pid);
            //初始化自己的
            for (int j = 0; j < listCard.cardList.Count; j++)
            {
                Transform cardContainer = GetTransformOfType(pid, ETransformType.ECardItemContainer);
                //不等于 0 就创建牌
                uint needCreateId = IsTributeBeforeCars(pid, listCard.cardList[j]);
                GameObject item = null;
                if (needCreateId == 0) //抗贡了 正常创建
                    item = SpwanSingleCard(cardContainer, listCard.cardList[j]);
                else//没有抗贡根据返回的牌创建
                    item = SpwanSingleCard(cardContainer, needCreateId);
                SingleCard singleCard = item.GetComponent<SingleCard>();
                singleCard.mIsIn = true;
                listCardItem.Add(singleCard);

            }
            Debug.Log("listCardItem.Count" + listCardItem.Count);
            dicListSingleCard.Add(pid, listCardItem);
            OrderInitSingleCardList(pid, listCardItem);
        }
    }
    /// <summary>
    /// 初始化进贡牌 或者明牌
    /// </summary>
    void InitLightOrJinggongCard()
    {
        //先销毁
        if (listLightCardItem != null)
        {
            for (int i = 0; i < listLightCardItem.Count; i++)
            {
                GameObject.Destroy(listLightCardItem[i].gameObject);
            }
        }
        listLightCardItem = new List<SingleCard>(); //代替了删除
        //掼蛋一直打二
        if (mPlayBackdata.lightPlayerIds != null && mPlayBackdata.tributeInfo != null)
        {
            SpawnLightCardItem();
            SpawnJingongCardItem();
        }
        //转蛋一直打二
        else if (mPlayBackdata.lightPlayerIds != null)
        {
            SpawnLightCardItem();
            JingGongOrLightTweenOver();
        }
        //掼蛋升级
        else if (mPlayBackdata.tributeInfo != null)
        {
            SpawnJingongCardItem();
        }
    }
    void SpawnLightCardItem()
    {
        //初始化
        ulong[] lightPids = mPlayBackdata.lightPlayerIds.ToArray();
        uint lightCard = mPlayBackdata.lightCard;
        bool isSame = lightPids.Length == 2 ? lightPids[0] == lightPids[1] : false;//亮牌属于同一个玩家

        for (int i = 0; i < lightPids.Length; i++)
        {
            Transform TranParentLightCard = GetTransformOfType(lightPids[i], ETransformType.EHeadFrame);
            GameObject item = SpwanSingleCard(TranParentLightCard, lightCard);
            item.transform.localScale = Vector3.one * 0.25f;
            bool isSpecial = dicPlayPos[EPlayerPositionType.ERight] == lightPids[i];

            item.transform.localPosition = isSpecial ? new Vector3(28, -26) : new Vector3(-28, -26);

            if (isSame && i == 1)
            {
                if (isSpecial)
                    item.transform.localPosition -= new Vector3(GlobalData.mSingleCardWidth * 0.25f, 0, 0);
                else
                    item.transform.localPosition += new Vector3(GlobalData.mSingleCardWidth * 0.25f, 0, 0);
            }
        }
    }
    List<SingleCard> tributeItems_JingGong = null;
    List<SingleCard> tributeItems_HuanGong = null;
    void SpawnJingongCardItem()
    {
        PlayBackFightData tributeInfo = mPlayBackdata.tributeInfo;

        if (tributeInfo.kgbs != 0) //不为0 表示抗贡
        {
            TweenManager.Instance.PlayKangGongTween(kgIconGm, JingGongOrLightTweenOver);
            return;
        }

        ulong[] bjgPid = tributeInfo.bjgPid.ToArray();
        ulong[] jgPid = tributeInfo.jgPid.ToArray();
        uint[] jgpz = tributeInfo.jgpz.ToArray();


        UIManagers.Instance.EnqueueTip("开始进贡");
        if (tributeTween != null) tributeTween.Kill();
        tributeTween = DOTween.Sequence();
        float timeUnit = 1.5f / speed;//单位时间
        //防止点击重新播放的时候，动画还没放完
        if (tributeItems_JingGong != null)
        {
            for (int i = 0; i < tributeItems_JingGong.Count; i++)
            {
                GameObject.Destroy(tributeItems_JingGong[i].gameObject);
                GameObject.Destroy(tributeItems_HuanGong[i].gameObject);
            }
        }


        tributeItems_JingGong = new List<SingleCard>();
        tributeItems_HuanGong = new List<SingleCard>();

        for (int i = 0; i < jgPid.Length; i++)
        {
            uint cardId = GetTributeCardInfo(jgPid[i]);
            if (cardId != 0)
            {
                //处理后---得到了没有进贡的牌
                Transform TranParentLightCard = GetTransformOfType(jgPid[i], ETransformType.EHeadFrame);
                GameObject item = SpwanSingleCard(TranParentLightCard, cardId);
                AddComponet_Canvas(item, true);//UI 下一帧才会初始化完毕

                Vector3 endPos = GetTransformOfType(bjgPid[i], ETransformType.EHeadFrame).position;
                RemoveSingleCard(jgPid[i], cardId);

                tributeTween.Append(item.transform.DOMove(endPos, timeUnit));
                tributeTween.Join(item.transform.DORotate(Vector3.zero, timeUnit, RotateMode.FastBeyond360));
                //从头像哪里移动到理牌的地方
                tributeTween.AppendInterval(timeUnit * 0.5f);
                Vector3 finalEndPos = GetTransformOfType(bjgPid[i], ETransformType.ECardItemContainer).position;
                tributeTween.Append(item.transform.DOMove(finalEndPos, timeUnit * 0.5f));
                tributeItems_JingGong.Add(item.GetComponent<SingleCard>());
            }
        }
        tributeTween.AppendCallback(delegate
        {
            for (int i = 0; i < tributeItems_JingGong.Count; i++)
            {
                Canvas ca = tributeItems_JingGong[i].GetComponent<Canvas>();
                GraphicRaycaster ra = tributeItems_JingGong[i].GetComponent<GraphicRaycaster>();
                if (ra != null) Destroy(ra); //先删除 因为 ra依赖ca
                if (ca != null) Destroy(ca);


                AddSingleCardList(bjgPid[i], new List<SingleCard>() { tributeItems_JingGong[i] });
                UIManagers.Instance.EnqueueTip("开始还贡");
            }
        });
        for (int i = 0; i < bjgPid.Length; i++)
        {
            uint cardId = GetTributeCardInfo(bjgPid[i]);
            if (cardId != 0)
            {
                Transform TranParentLightCard = GetTransformOfType(bjgPid[i], ETransformType.EHeadFrame);
                GameObject item = SpwanSingleCard(TranParentLightCard, cardId);
                AddComponet_Canvas(item, false);

                Vector3 endPos = GetTransformOfType(jgPid[i], ETransformType.EHeadFrame).position;
                //移除的同时并销毁了但是现在不想销毁 所以再创建一个 并整理
                RemoveSingleCard(bjgPid[i], cardId);

                tributeTween.Append(item.transform.DOMove(endPos, timeUnit));
                tributeTween.Join(item.transform.DORotate(Vector3.zero, timeUnit, RotateMode.FastBeyond360));
                //从头像哪里移动到理牌的地方
                tributeTween.AppendInterval(timeUnit * 0.5f);
                Vector3 finalEndPos = GetTransformOfType(jgPid[i], ETransformType.ECardItemContainer).position;
                tributeTween.Append(item.transform.DOMove(finalEndPos, timeUnit * 0.5f));

                tributeItems_HuanGong.Add(item.GetComponent<SingleCard>());
            }
        }
        tributeTween.AppendCallback(delegate
        {
            for (int i = 0; i < tributeItems_HuanGong.Count; i++)
            {
                Canvas ca = tributeItems_HuanGong[i].GetComponent<Canvas>();
                GraphicRaycaster ra = tributeItems_HuanGong[i].GetComponent<GraphicRaycaster>();
                if (ra != null) Destroy(ra); //先删除 因为 ra依赖ca
                if (ca != null) Destroy(ca);
                AddSingleCardList(jgPid[i], new List<SingleCard>() { tributeItems_HuanGong[i] });
            }
            JingGongOrLightTweenOver();
        });
        tributeTween.Play();
    }
    public void JingGongOrLightTweenOver()
    {
        if (tributeItems_HuanGong != null && tributeItems_HuanGong != null)
        {
            tributeItems_JingGong.Clear();
            tributeItems_HuanGong.Clear();
        }
        isReplay = false;//重放的真的开始是在 进贡完毕之后的数据
        isPlay = true;
    }
    private Dictionary<EPlayerPositionType, ulong> InitPlayerPosition()
    {
        dicPlayPos = new Dictionary<EPlayerPositionType, ulong>();
        List<PlayBackFightData> listData = mPlayBackdata.fightingPutoutlist;
        for (int i = 0; i < listData.Count; i++)
        {
            if (listData[i].pid == PlayerInfo.Instance.mPlayerPid)
            {
                switch (i)
                {
                    case 0:
                        dicPlayPos.Add(EPlayerPositionType.ESelf, listData[i].pid);
                        dicPlayPos.Add(EPlayerPositionType.ERight, listData[i + 1].pid);
                        dicPlayPos.Add(EPlayerPositionType.ETop, listData[i + 2].pid);
                        dicPlayPos.Add(EPlayerPositionType.ELeft, listData[i + 3].pid);
                        break;
                    case 1:
                        dicPlayPos.Add(EPlayerPositionType.ELeft, listData[i - 1].pid);
                        dicPlayPos.Add(EPlayerPositionType.ESelf, listData[i].pid);
                        dicPlayPos.Add(EPlayerPositionType.ERight, listData[i + 1].pid);
                        dicPlayPos.Add(EPlayerPositionType.ETop, listData[i + 2].pid);
                        break;
                    case 2:
                        dicPlayPos.Add(EPlayerPositionType.ETop, listData[i - 2].pid);
                        dicPlayPos.Add(EPlayerPositionType.ELeft, listData[i - 1].pid);
                        dicPlayPos.Add(EPlayerPositionType.ESelf, listData[i].pid);
                        dicPlayPos.Add(EPlayerPositionType.ERight, listData[i + 1].pid);
                        break;
                    case 3:
                        dicPlayPos.Add(EPlayerPositionType.ERight, listData[i - 3].pid);
                        dicPlayPos.Add(EPlayerPositionType.ETop, listData[i - 2].pid);
                        dicPlayPos.Add(EPlayerPositionType.ELeft, listData[i - 1].pid);
                        dicPlayPos.Add(EPlayerPositionType.ESelf, listData[i].pid);
                        break;
                }
                break;
            }

        }
        return dicPlayPos;
    }
    #endregion

    #region >>>>>>>>>>>点击事件方法
    void OnShowClockBtnClick(GameObject gm)
    {
        GameObject obj = gm.transform.GetChild(0).gameObject;
        obj.SetActive(!obj.activeSelf);
        if (!obj.activeSelf)
            ShowClock(0);
    }
    void OnRoomInfoClick(GameObject gm)
    {
        SpriteState spriteState = gm.GetComponent<Button>().spriteState;
        string sprName = "";
        if (!IsShowLeftInfo)
        {
            mySeque.PlayForward();
            gm.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "pk_btn_foward2");
            sprName = "pk_btn_foward2" + "_click";
        }
        else
        {
            mySeque.PlayBackwards();
            gm.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "pk_btn_foward1");
            sprName = "pk_btn_foward1" + "_click";
        }
        spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EPlaying, sprName);
        IsShowLeftInfo = !IsShowLeftInfo;
    }
    private void OnSpeedClick(GameObject go)
    {
        speed *= 2;
        if (speed > 4)
        {
            speed = 1;
        }
        Debug.Log("播放速度" + speed);
        SetSpeed(speed, go);
    }
    void SetSpeed(int speed, GameObject go)
    {
        this.speed = speed;
        switch (speed)
        {
            case 1:
                go.transform.GetChild(0).gameObject.SetActive(false);
                go.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case 2:
                go.transform.GetChild(0).gameObject.SetActive(true);
                go.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case 4:
                go.transform.GetChild(0).gameObject.SetActive(false);
                go.transform.GetChild(1).gameObject.SetActive(true);
                break;
        }
        curIntervalTime = interTime / speed;
    }
    private void OnPauseClick(GameObject go)
    {
        isPause = !isPause;
        if (outIndex == 0 && !isPause) //索引为0的时候 相当于重播的意思
        {
            OnReplayClick(replayBtn);
        }
        SetPause(isPause);
    }
    void SetPause(bool isPause)
    {
        Debug.Log("isPause:" + isPause);
        string spritName = null;
        this.isPause = isPause;
        if (isPause)
            spritName = "playback_pauseing";
        else
            spritName = "playback_pause";

        pauseBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, spritName);
    }
    private void OnReplayClick(GameObject go)
    {
        isReplay = true;
        ClearContainerItem();
        InitData();
    }
    private void OnCloseClick(GameObject go)
    {
        UIManagers.Instance.DestroySingleUI(UIType.PlayBackView);
    }
    #endregion

    #region >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>工具方法 
    uint IsTributeBeforeCars(ulong pid, uint cardId)
    {

        PlayBackFightData tributeInfo = mPlayBackdata.tributeInfo;
        if (tributeInfo != null)
        {
            if (mPlayBackdata.tributeInfo.kgbs != 0)
            {
                return 0;//表示可以创建当前牌的id
            }
            List<ulong> bjgPid = tributeInfo.bjgPid;
            List<ulong> jgPid = tributeInfo.jgPid;
            List<uint> jgpz = tributeInfo.jgpz;
            for (int i = 0; i < jgPid.Count; i++)
            {
                //这张牌 是否在进还贡信息中  就取回属于自己的牌   

                //进贡玩家初始化牌 
                if (pid == jgPid[i])
                {
                    //牌 是当前要 初始化的牌
                    if (cardId == GetTributeCardInfo(bjgPid[i]))
                        return GetTributeCardInfo(jgPid[i]);//取回自己的牌
                }
                else
                {
                    if (cardId == GetTributeCardInfo(jgPid[i]))
                        return GetTributeCardInfo(bjgPid[i]);//取回自己的牌
                }


            }
        }
        return cardId;
    }
    /// <summary>
    ///获取 显示倒计时 的对象
    /// </summary>
    /// <param name="pid">0的时候隐藏所有的倒计时</param>
    public GameObject ShowClock(ulong curpid)
    {
        GameObject returnclock = null;
        if (!isShowClockBtn.GetChild(0).gameObject.activeSelf)
        {
            curpid = 0;
        }
        for (int i = 1; i < 5; i++)
        {
            ulong pid = dicPlayPos[(EPlayerPositionType)i];
            GameObject clock = GetTransformOfType(pid, ETransformType.ETimerClock).gameObject;
            //这个pid 传的应该是curpid  pid是一直在变的  
            clock.SetActive(pid == curpid);
            if (pid == curpid)
            {
                returnclock = clock;
            }
        }
        return returnclock;
    }
    void AddSingleCardList(ulong pid, List<SingleCard> cardList)
    {
        if (dicListSingleCard == null || !dicListSingleCard.ContainsKey(pid))
        {
            return;
        }
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.SetParent(GetTransformOfType(pid, ETransformType.ECardItemContainer));
            dicListSingleCard[pid].Add(cardList[i]);
        }
        OrderInitSingleCardList(pid, dicListSingleCard[pid]);
    }
    void AddComponet_Canvas(GameObject item, bool isJinggong)
    {
        //添加组件
        item.AddComponent<GraphicRaycaster>(); //表示可以接受下层的点击事件
        item.AddComponent<Canvas>();
        StartCoroutine(WaitOneFrame(item, isJinggong));
    }
    IEnumerator WaitOneFrame(GameObject item, bool isJingong)
    {
        yield return null;
        Canvas ca = item.GetComponent<Canvas>();
        ca.overrideSorting = true;
        ca.sortingLayerName = GlobalData.mAnimSortingLayer;
        int gong = isJingong ? 1 : 0;
        ca.sortingOrder = GlobalData.mAnimOrderIdLayer + gong;
    }
    /// <summary>
    /// 依据玩家id  获取进贡还贡出去的牌的id
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    uint GetTributeCardInfo(ulong pid)
    {
        List<PlayBackFightData> tributeRecord = mPlayBackdata.tributeRecordLst;
        for (int i = 0; i < tributeRecord.Count; i++)
        {
            if (tributeRecord[i].pid == pid)
            {
                return tributeRecord[i].card;
            }
        }
        return 0;
    }
    GameObject SpwanSingleCard(Transform parent, uint lightCard)
    {
        GameObject item = GameObject.Instantiate(singleCardPrefab) as GameObject;
        item.transform.SetParent(parent);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one * 0.5f;
        RectTransform rect = item.transform.GetComponent<RectTransform>();
        //牌不能点击
        item.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        SingleCard singleCard = item.GetComponent<SingleCard>();
        singleCard.SetCardData(lightCard);
        return item;
    }
    void ClearContainerItem(bool isClearCardItem = true)//是否删除手牌
    {
        //item
        if (isClearCardItem)
        {
            DestroyChildren(selfCardContainer);
            DestroyChildren(topPosContainerCardItem);
            DestroyChildren(leftPosContainerCardItem);
            DestroyChildren(rightPosContainerCardItem);
        }
        //putout item
        DestroyChildren(selfPosContainer);
        DestroyChildren(topPosContainer);
        DestroyChildren(leftPosContainer);
        DestroyChildren(rightPosContainer);

    }
    void CheckShowJieFengTween(bool isBuchu)
    {
        for (int posIndex = 1; posIndex < 5; posIndex++)
        {
            for (int j = 0; j < dicCheckCardCount.Count; j++)
            {
                if (dicCheckCardCount.ContainsKey((EPlayerPositionType)posIndex))
                {
                    EPlayerPositionType pos = (EPlayerPositionType)posIndex;
                    
                    //中间被出牌打断了
                    if (!isBuchu)
                    {
                        DestroyChildren(GetTransformOfType(dicPlayPos[pos], ETransformType.EOutCardItemContainer));
                        dicCheckCardCount.Remove(pos);
                    }
                    else
                    {
                        dicCheckCardCount[pos]++;
                        if (dicCheckCardCount[pos] >= 3)
                        {
                            Debug.Log("清空牌数量为0的玩家" + dicPlayPos[pos]);
                            DestroyChildren(GetTransformOfType(dicPlayPos[pos], ETransformType.EOutCardItemContainer));
                            dicCheckCardCount.Remove(pos);
                            SpwanJieFengAnim(dicPlayPos[pos]);
                        }
                    }
                }
            }
        }
    }
    void CheckShowCleanSpr(ulong pid)
    {
        if (dicListListSingleCards[pid].Count == 0)
        {
            remainPlayerCount--;
            dicCheckCardCount.Add(GetPlayerPos(pid), 0);
            rankIndex++;
            Transform headFrame = GetTransformOfType(pid, ETransformType.EHeadFrame);
            headFrame.GetChild(0).GetComponent<Image>().sprite = GlobalData.GetGameCleanCpr(rankIndex);
        }

    }
    void SpwanJieFengAnim(ulong playerId)
    {
        EPlayerPositionType pos = GetPlayerPos(playerId);
        int team = ((int)pos + 2);
        ulong sameTeamPid = dicPlayPos[(EPlayerPositionType)(team > 4 ? team % 4 : team)]; //对家id
        if (dicListListSingleCards[sameTeamPid].Count == 0) //首先对家有牌的情况下 ，再生成播放动画
        {
            return;
        }
        Transform tranCardAnimParent = GetTransformOfType(sameTeamPid, ETransformType.ECardItemContainer);
        GameObject GMAnimAnimType = TweenManager.Instance.PlayJiefengTween(tranCardAnimParent, tranCardAnimParent, GetPlayerPos(sameTeamPid));
        if (GMAnimAnimType != null)
        {
            GMAnimAnimType.layer = 5;
            MeshRenderer render = GMAnimAnimType.GetComponent<MeshRenderer>();
            render.sortingLayerName = GlobalData.mAnimSortingLayer;
            render.sortingOrder = 1;
        }
    }
    /// <summary>
    ///排列初始生成数据
    /// </summary>
    /// <param name="listCardItem"></param>

    #region >>>>>>>>>>>>>>理牌
    void OrderInitSingleCardList(ulong playerid, List<SingleCard> listCardItem)
    {
        sortSingleCard = new List<List<SingleCard>>();
        SelfCardsManager.Instance.InitRefreshRealCardDic(listCardItem, sortSingleCard);
        if (dicListListSingleCards.ContainsKey(playerid)) //用于牌的进贡
            dicListListSingleCards[playerid] = sortSingleCard;
        else
            dicListListSingleCards.Add(playerid, sortSingleCard);

        SetCardPosAndDepth(playerid, sortSingleCard);
    }
    /// <summary>
    /// 设置牌的位置和层级深度
    /// </summary>
    /// <param name="cardDic"></param>
    void SetCardPosAndDepth(ulong playerid, List<List<SingleCard>> cardDic)
    {
        for (int k = 0; k < cardDic.Count; k++)
        {
            var value = cardDic[k];
            for (int i = 0; i < value.Count; i++)
            {
                Transform obj = value[i].transform;
                obj.transform.localPosition = GetRealCardPos(playerid, cardDic, value[i].mId);
            }
        }
        //Sort Depth
        ResetSibling(playerid, cardDic);
    }
    public Vector3 GetRealCardPos(ulong playerid, List<List<SingleCard>> dic, uint idx)
    {
        //两幅牌，有两张相同的牌,idx在前的就在下面
        int horizontalCount = dic.Count;
        int realHorizontalIdx = GetHorizontalIndexInRealDic(dic, idx);

        int verticalCount = GetInHorizontalRealDicCards(dic, idx).Count;
        int realVerticalIdx = GetVerticalIndexInRealDic(dic, idx);


        Vector3 realPos = Vector3.zero;
        float intervalY = GlobalData.mSingelCardRateY * 2 / 3;

        float xPos = GetCurPosX(realHorizontalIdx, horizontalCount, GetCurDistanceCardPosX(horizontalCount));
        float yPos = GetCurPosY(realVerticalIdx, verticalCount, intervalY); //+牌的1/2
        if (playerid == dicPlayPos[EPlayerPositionType.ESelf])
        {
            realPos = new Vector3(xPos, yPos, 0);
        }
        else if (playerid == dicPlayPos[EPlayerPositionType.ERight])
        {
            realPos = new Vector3(-yPos, xPos, 0);
        }
        else if (playerid == dicPlayPos[EPlayerPositionType.ETop])
        {
            realPos = new Vector3(xPos, -yPos, 0);
        }
        else if (playerid == dicPlayPos[EPlayerPositionType.ELeft])
        {
            realPos = new Vector3(yPos, xPos, 0);
        }
        return realPos;
    }
    /// <summary>
    /// 获取当前的idx的card所在realDic中的所在第几个位置
    /// </summary>
    /// <param name="idx">唯一的idx</param>
    /// <returns></returns>
    int GetHorizontalIndexInRealDic(List<List<SingleCard>> dic, uint idx)
    {
        for (int k = 0; k < dic.Count; k++)
        {
            var value = dic[k];
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].mId == idx)
                {
                    return k;
                }
            }
        }
        return 0;
    }
    /// <summary>
    /// 返回idx所在每一horizontal所有值
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    List<SingleCard> GetInHorizontalRealDicCards(List<List<SingleCard>> dic, uint idx)
    {
        for (int k = 0; k < dic.Count; k++)
        {
            var value = dic[k];
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].mId == idx)
                {
                    return value;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 返回值为从小到上，(牌所在realDic中，纵向位置),返回0-7
    /// </summary>
    /// <param name="idx"></param>
    int GetVerticalIndexInRealDic(List<List<SingleCard>> dic, uint idx)
    {
        var list = GetInHorizontalRealDicCards(dic, idx);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].mId == idx)
            {
                return i;
            }
        }
        return 0;
    }
    /// <summary>
    /// 获取当前两张卡牌的X的距离
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    public float GetCurDistanceCardPosX(int totalCount)
    {
        //   公式 ：/2   *4/5
        return Mathf.Clamp((GlobalData.mCardContainerWidth * 2 / 5) / totalCount, 1, GlobalData.mSingleCardWidth * 2 / 5);
    }
    /// <summary>
    /// 获取当前idx的x轴位置
    /// </summary>
    /// <param name="idx">0- count-1 </param>
    /// <param name="total">总的横向数量</param>
    /// <returns></returns>
    float GetCurPosX(int idx, int total, float ratePosX)
    {
        //  float ratePosX = ;

        float middle = total % 2 == 0 ? total / 2 - 0.5f : total / 2;
        return (idx - middle) * ratePosX;
    }
    float GetCurPosY(int idx, int total, float rateY)
    {
        //  float rateY =;  /*Mathf.Clamp(mCardContainerWidth / total, 1, GlobalData.mSingleCardWidth);*/
        return idx * rateY;
    }
    /// <summary>
    /// 刷新图层
    /// </summary>
    /// <param name="cardsList"></param>
    public void ResetSibling(ulong playerid, List<List<SingleCard>> cardsList)
    {
        //left.col=right.col  left.row=self.row right.row=top.row  top.col=-self.col
        if (dicPlayPos[EPlayerPositionType.ESelf] == playerid)
        {
            for (int i = cardsList.Count - 1; i >= 0; i--)
            {
                var data = cardsList[i];
                for (int j = 0; j < data.Count; j++)
                {
                    data[j].transform.SetAsFirstSibling();
                }
            }
        }
        else if (dicPlayPos[EPlayerPositionType.ETop] == playerid)
        {
            for (int i = cardsList.Count - 1; i >= 0; i--)
            {
                var data = cardsList[i];
                for (int j = data.Count - 1; j >= 0; j--)
                {
                    data[j].transform.SetAsFirstSibling();
                }
            }
        }
        else if (dicPlayPos[EPlayerPositionType.ELeft] == playerid)
        {
            for (int i = 0; i < cardsList.Count; i++)
            {
                var data = cardsList[i];
                for (int j = data.Count - 1; j >= 0; j--)
                {
                    data[j].transform.SetAsFirstSibling();
                }
            }
        }
        else if (dicPlayPos[EPlayerPositionType.ERight] == playerid)
        {
            for (int i = 0; i < cardsList.Count; i++)
            {
                var data = cardsList[i];
                for (int j = 0; j < data.Count; j++)
                {
                    data[j].transform.SetAsFirstSibling();
                }
            }
        }
    }
    #endregion
    void SetPlayInfoData()
    {
        if (mPlayBackdata.goals != null)
        {
            for (int i = 0; i < mPlayBackdata.goals.Count; i++)
            {
                PlayBackFightData data = mPlayBackdata.goals[i];
                for (int pos = 1; pos < 5; pos++)
                {
                    EPlayerPositionType posType = (EPlayerPositionType)pos;
                    if (dicPlayPos[posType] == data.pid)
                    {
                        GetPlayInfoData(data.pid).goal = data.goal;
                        break;
                    }
                }
            }
        }
    }
    float GetPutoutTweenSpeed()
    {
        return 0.5f;
    }
    /// <summary>
    /// 初始化打出去的牌
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="listCard"></param>
    /// <param name="cardType"></param>
    void InitPutOutCard(ulong playerId, List<uint> listCard, TGuanDanCT cardType)
    {
        float tweenSpeed = GetPutoutTweenSpeed();
        List<SingleCard> hasPutOutCardsItem = new List<global::SingleCard>();
        List<SingleCard> outPosCards = new List<SingleCard>();
        SelfCardsManager manager = SelfCardsManager.Instance;
        DestroyChildren(GetTransformOfType(playerId, ETransformType.EOutCardItemContainer));
        bool isBuyao = listCard.Count == 0;
        if (!isBuyao)
        {
            //1.播放语音
            AudioManager.Instance.PlayEffectAudio(manager.GetCardAudioFileName(cardType, listCard[0], AudioManager.Instance.mAudioStyle));
            //2.将牌生成 打出
            hasPutOutCardsItem = GetPutoutSingleCardList(playerId, listCard);

            float putOutCardWidth = GlobalData.mLastSingelCardWidth / 2;
            for (int i = 0; i < hasPutOutCardsItem.Count; i++)
            {
                GameObject cardItem = GameObject.Instantiate(hasPutOutCardsItem[i].gameObject);
                cardItem.transform.SetParent(GetTransformOfType(playerId, ETransformType.EOutCardItemContainer));

                cardItem.transform.localScale = Vector3.one * 0.5f;
                float startX = GetCurPosX(i, hasPutOutCardsItem.Count, putOutCardWidth);
                cardItem.transform.localPosition = new Vector3(startX, 0, 0);
            }
            //3.播放动画
            Transform TranCardAnimParent = GetTransformOfType(playerId, ETransformType.EOutCardItemContainer);
            GameObject GMAnimAnimType = TweenManager.Instance.PlayCardTween(cardType, TranCardAnimParent);

            if (GMAnimAnimType != null)
            {
                GMAnimAnimType.layer = 5;
                MeshRenderer render = GMAnimAnimType.GetComponent<MeshRenderer>();
                render.sortingLayerName = GlobalData.mAnimSortingLayer;
                render.sortingOrder = 1;
            }

        }
        else
        {
            AudioManager.Instance.PlayEffectAudio(manager.GetCardAudioFileName(TGuanDanCT.CT_BUCHU, 0, AudioManager.Instance.mAudioStyle));
            //生成 不出对象
            GameObject item = Instantiate(singleCardPrefab);
            item.transform.SetParent(GetTransformOfType(playerId, ETransformType.EOutCardItemContainer));
            item.transform.localPosition = Vector3.zero;//GlobalData.mSelectCardToTargetPos;
            item.transform.localScale = Vector3.zero;
            item.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            SingleCard singleCard = item.GetComponent<SingleCard>();
            singleCard.SetCardData(GlobalData.mRefuseCardNumId);
            Sequence se = DOTween.Sequence();
            se.Append(item.transform.DOScale(Vector3.one, tweenSpeed / speed).SetEase(Ease.Linear));
        }
        CheckShowJieFengTween(isBuyao);
        //4.清除牌 和预支体对象
        RemoveOutCardList(playerId, hasPutOutCardsItem);
        SetCardPosAndDepth(playerId, dicListListSingleCards[playerId]);//重新整理
    }

    List<SingleCard> GetPutoutSingleCardList(ulong playerId, List<uint> listCardId)
    {
        List<SingleCard> putOutSingleCard = new List<SingleCard>();
        List<List<SingleCard>> curListListSingleCards = dicListListSingleCards[playerId];
        for (int i = 0; i < listCardId.Count; i++)
        {
            bool isFind = false;
            for (int row = 0; row < curListListSingleCards.Count; row++)
            {
                for (int col = 0; col < curListListSingleCards[row].Count; col++)
                {
                    if (curListListSingleCards[row][col].mId == listCardId[i])
                    {
                        isFind = true;
                        putOutSingleCard.Add(curListListSingleCards[row][col]);
                        break;
                    }
                }
                if (isFind)
                {
                    break;
                }
            }
        }
        return putOutSingleCard;
    }
    void RemoveSingleCard(ulong playerId, uint cardId)
    {
        List<List<SingleCard>> listlistSingleCards = dicListListSingleCards[playerId];
        List<SingleCard> listSingleCard = dicListSingleCard[playerId];
        bool isFind = false;
        for (int row = 0; row < listlistSingleCards.Count; row++)
        {
            for (int col = 0; col < listlistSingleCards[row].Count; col++)
            {
                if (listlistSingleCards[row][col].mId == cardId)
                {
                    isFind = true;
                    SingleCard removeCard = listlistSingleCards[row][col];
                    listlistSingleCards[row].Remove(removeCard);
                    listSingleCard.Remove(removeCard);

                    if (listlistSingleCards[row].Count == 0) //这一列没有数据 了 删除这一列
                    {
                        listlistSingleCards.Remove(listlistSingleCards[row]);
                    }
                    GameObject.Destroy(removeCard.gameObject);
                    break;
                }
            }
            if (isFind)
            {
                break;
            }
        }
    }
    void RemoveSingleCardList(ulong pid, List<SingleCard> cardList)
    {
        if (dicListListSingleCards == null || !dicListListSingleCards.ContainsKey(pid))
        {
            return;
        }
        RemoveOutCardList(pid, cardList);
        SetCardPosAndDepth(pid, dicListListSingleCards[pid]);//重新整理
    }

    /// <summary>
    /// 清除已经出去的牌 包含 进贡的/打出的
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="listCardItem"></param>
    void RemoveOutCardList(ulong playerId, List<SingleCard> listCardItem)
    {
        List<List<SingleCard>> listlistSingleCards = dicListListSingleCards[playerId];
        List<SingleCard> listSingleCard = dicListSingleCard[playerId];
        for (int i = 0; i < listCardItem.Count; i++)
        {
            bool isFind = false;
            for (int row = 0; row < listlistSingleCards.Count; row++)
            {
                for (int col = 0; col < listlistSingleCards[row].Count; col++)
                {
                    if (listlistSingleCards[row][col].mId == listCardItem[i].mId)
                    {
                        isFind = true;
                        //listCardItem 中的元素只是为null  对count无影响 
                        listlistSingleCards[row].Remove(listCardItem[i]);
                        listSingleCard.Remove(listCardItem[i]);

                        if (listlistSingleCards[row].Count == 0) //这一列没有数据 了 删除这一列
                        {
                            listlistSingleCards.Remove(listlistSingleCards[row]);
                        }
                        GameObject.Destroy(listCardItem[i].gameObject);
                        break;
                    }
                }
                if (isFind)
                {
                    break;
                }
            }

        }

    }

    /// <summary>
    /// 根据玩家id获取玩家信息
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    private PlayBackFightData GetPlayInfoData(ulong pid)
    {
        for (int i = 0; i < mPlayBackdata.players.Count; i++)
        {
            if (mPlayBackdata.players[i].pid == pid)
            {
                return mPlayBackdata.players[i];
            }
        }
        Debug.Log("不存在的id");
        return null;
    }

    /// <summary>
    /// 根据玩家id获取玩家打牌的数据
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    private PlayBackFightData GetPlayerCardsData(ulong pid)
    {
        List<PlayBackFightData> listCards = mPlayBackdata.cards;
        for (int i = 0; i < listCards.Count; i++)
        {
            if (listCards[i].pid == pid)
            {
                return listCards[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 获取玩家的位置 更局id
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    private EPlayerPositionType GetPlayerPos(ulong pid)
    {
        var v = dicPlayPos.GetEnumerator();
        EPlayerPositionType pos = EPlayerPositionType.ESelf;
        while (v.MoveNext())
        {
            if (pid == v.Current.Value)
            {
                pos = v.Current.Key;
                break;
            }
        }
        return pos;
    }

    /// <summary>
    /// 销毁某个对象下面所有的子物体
    /// </summary>
    /// <param name="t"></param>
    void DestroyChildren(Transform t)
    {
        //没有用getchild(0)的方式去销毁的原因是：销毁是异步的，所以并不是马上销毁 ，while的话报错
        for (int i = 0; i < t.childCount; i++)
        {
            GameObject.Destroy(t.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 获取绝对位置 的某个类型对象transform
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="transType"></param>
    /// <returns></returns>
    Transform GetTransformOfType(ulong playerId, ETransformType transType)
    {
        Transform objectTransform = selfPosContainer;
        if (dicPlayPos[EPlayerPositionType.ESelf] == playerId)
        {
            objectTransform = SelfTransfrom(transType);
        }
        else if (dicPlayPos[EPlayerPositionType.ELeft] == playerId)
        {
            objectTransform = LeftTransfrom(transType);
        }
        else if (dicPlayPos[EPlayerPositionType.ERight] == playerId)
        {
            objectTransform = RightTransfrom(transType);
        }
        else if (dicPlayPos[EPlayerPositionType.ETop] == playerId)
        {
            objectTransform = TopTransfrom(transType);
        }
        return objectTransform;
    }

    Transform SelfTransfrom(ETransformType transType)
    {
        Transform objectTransform = null;
        switch (transType)
        {
            case ETransformType.EOutCardItemContainer:
                objectTransform = selfPosContainer;
                break;
            case ETransformType.ECardItemContainer:
                objectTransform = selfCardContainer;
                break;
            case ETransformType.EHeadFrame:
                objectTransform = selfHeadFrame;
                break;
            case ETransformType.ETimerClock:
                objectTransform = selfClock;
                break;
        }
        return objectTransform;
    }
    Transform RightTransfrom(ETransformType transType)
    {
        Transform objectTransform = null;
        switch (transType)
        {
            case ETransformType.EOutCardItemContainer:
                objectTransform = rightPosContainer;
                break;
            case ETransformType.ECardItemContainer:
                objectTransform = rightPosContainerCardItem;
                break;
            case ETransformType.EHeadFrame:
                objectTransform = rightHeadFrame;
                break;
            case ETransformType.ETimerClock:
                objectTransform = rightClock;
                break;
        }
        return objectTransform;
    }
    Transform TopTransfrom(ETransformType transType)
    {
        Transform objectTransform = null;
        switch (transType)
        {
            case ETransformType.EOutCardItemContainer:
                objectTransform = topPosContainer;
                break;
            case ETransformType.ECardItemContainer:
                objectTransform = topPosContainerCardItem;
                break;
            case ETransformType.EHeadFrame:
                objectTransform = topHeadFrame;
                break;
            case ETransformType.ETimerClock:
                objectTransform = topClock;
                break;
        }
        return objectTransform;
    }
    Transform LeftTransfrom(ETransformType transType)
    {
        Transform objectTransform = null;

        switch (transType)
        {
            case ETransformType.EOutCardItemContainer:
                objectTransform = leftPosContainer;
                break;
            case ETransformType.ECardItemContainer:
                objectTransform = leftPosContainerCardItem;
                break;
            case ETransformType.EHeadFrame:
                objectTransform = leftHeadFrame;
                break;
            case ETransformType.ETimerClock:
                objectTransform = leftClock;
                break;
        }
        return objectTransform;
    }
    #endregion

    #region >>>>>>防止热更方法
    public int UIntToInt(uint i)
    {
        return (int)i;
    }
    public uint IntToUInt(int i)
    {
        return (uint)i;
    }
    public int EnumToInt_ETransformType(ETransformType e)
    {
        return (int)e;
    }
    public int EnumToInt_PlayerPos(EPlayerPositionType type)
    {
        return (int)type;
    }
    public EPlayerPositionType IntToEnum_PlayerPos(int posId)
    {
        return (EPlayerPositionType)posId;
    }
    public ETransformType IntToEnum_ETransformType(int e)
    {
        return (ETransformType)e;
    }
    public List<SingleCard> GetDicValue_DicListSingleCards(ulong pid)
    {
        return dicListSingleCard[pid];
    }
    public List<List<SingleCard>> GetDicValue_DicListListSingleCards(ulong pid)
    {
        return dicListListSingleCards[pid];
    }
    public int GetDicValue_DicCheckCardCount(EPlayerPositionType type)
    {
        return dicCheckCardCount[type];
    }
    public ulong GetDicValue_DicPlayPos(EPlayerPositionType type)
    {
        return dicPlayPos[type];
    }
    public void InitDic_DicCheckCardCount()
    {
        dicCheckCardCount = new Dictionary<EPlayerPositionType, int>();
    }
    public void InitDic_DicPlayPos()
    {
        dicPlayPos = new Dictionary<EPlayerPositionType, ulong>();
    }
    public void InitDic_DicListListSingleCards()
    {
        dicListListSingleCards = new Dictionary<ulong, List<List<SingleCard>>>();
    }
    public void InitDic_DicListSingleCards()
    {
        dicListSingleCard = new Dictionary<ulong, List<SingleCard>>();
    }
    #endregion
}
