using UnityEngine;
using System.Collections;
using MsgContainer;
using Net;
using System.IO;
using ZhiWa;
using DNotificationCenterManager;
using System.Collections.Generic;
using HaoyunFramework;

public class PlayingModule : BaseModule
{
    public PlayingModule()
    {
        mIsLoadByMiddleSence = false;
    }

    public override void LoadModule()
    {
        base.LoadModule();
		if(!GoldFiledManager.Instance.mIsGoldFiled)
        {
            ContextManager.Instance.Pop(UIType.WaitGameView.Name);
			ContextManager.Instance.Push(new PlayingGameContext()); //暂时测试，记得加回来
        }
        else
        {
            ContextManager.Instance.Push(new GoldPlayingGameContext()); //金币场
        }
    }

    public override void AddServerCallBack()
    {
        base.AddServerCallBack();
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_OC, RevcCanPutOutCard); //收到出牌消息
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_SHOW_CARD_FAIL, RevcPutOutFail); //出牌失败
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_DISS_AGREE, RecevRoomAgree); //同意解散房间
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_DISS_REFUSE, RecevRoomRefuse);//拒绝解散房间
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_DISS_APPLY, RecevRoomApply);//申请解散回调
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_DISS_FAIL, RecevDisRoomFail);//解散投票结果 --失败

        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GAME_TOTALSCORE, RecevTotalBureau);//总的结算
        //进贡
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_JG_FQSUC, RecevTributeSuc);//进贡
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_JG_NOTMAX, RecevJGMax);//(本人)
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_JG_FQERR, RecevFQErr);//(其余人)
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_JG_HTJP, RecevJG_HTJPErr);//(其余人)

        //回贡成功
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_HG_FQSUC, RecevBackTributeSuc);//回贡RecevBackTributeSuc
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_HG_XYS, RecevHGXys);
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_HG_FQERR, RecevHgFqErr);
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_T_H_S, RecevTonghuanshun);//同花顺
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_LIPAI, RecevSortCard);

        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_SHOW_CARD_CHOOSE, RecevCardTypeChoose);//牌型选择 
    }


    void RecevCardTypeChoose(MsgGlobal msg)
    {
        MsgAction data = msg.action;
        ArgsCanputOutCard args = new ArgsCanputOutCard();
        args.msgCardGroup = data.cardGroup;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EChooseCardType, args);
    }
    void RecevFQErr(MsgGlobal msg)
    {
        UIManagers.Instance.EnqueueTip("进贡发起失败");
    }

    void RecevJG_HTJPErr(MsgGlobal msg)
    {
        UIManagers.Instance.EnqueueTip("进贡不能为为百搭牌");
    }
    void RecevJGMax(MsgGlobal msg)
    {
        UIManagers.Instance.EnqueueTip("进贡不是最大值");
    }

    void RecevHGXys(MsgGlobal msg)
    {
        UIManagers.Instance.EnqueueTip("回贡的牌不能大于10");
    }

    void RecevHgFqErr(MsgGlobal msg)
    {
        UIManagers.Instance.EnqueueTip("回贡发起失败");
    }

    /// <summary>
    /// 进贡成功回调
    /// </summary>
    /// <param name="msg"></param>
    void RecevTributeSuc(MsgGlobal msg)
    {
        //进贡id为自己的情况，自己牌，清空，并出发出动画到被进贡玩家，被进贡玩家牌+1
        MsgTributeInfo data = msg.tribute_info;
        ArgsTribute args = new ArgsTribute();
        args.addId = data.bjg_player_id[0];
        args.removeId = data.jg_player_id[0];
        args.card = data.jgpz_card[0];
        args.isJingong = true;
        args.isStart = false;
        Debug.Log("进贡回调：" + "进贡id" + args.addId + "回贡id" + args.removeId + "牌值:" + args.card + "isjingong" + args.isJingong);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ETributeSuc, args);
    }
    /// <summary>
    /// 回贡成功回调
    /// </summary>
    /// <param name="msg"></param>
    void RecevBackTributeSuc(MsgGlobal msg)
    {
        //回贡成功后，正式开始游戏
        MsgTributeInfo data = msg.tribute_info;
        ArgsTribute args = new ArgsTribute();
        args.addId = data.jg_player_id[0];
        args.removeId = data.bjg_player_id[0];
        args.card = data.hgpz_card;
        args.isJingong = false;
        args.isStart = data.start_tag == 1;
        Debug.Log("回贡回调：" + "回贡id" + args.removeId + "被回贡id" + args.addId + "牌值:" + args.card + "isjingong" + args.isJingong);
        Debug.Log("start_tag:" + data.start_tag);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ETributeSuc, args);
    }


    void RecevTotalBureau(MsgGlobal msg)
    {
        if (RoomInfo.Instance.mRoom == null) {
            Debug.Log("Room 为null");
            return; }

        MsgGuandanRoom data = msg.guandan_room;

        Debug.Log("总结算");

        ArgsMsgTotalScore scoreArgs = new ArgsMsgTotalScore();
        scoreArgs.List = data.game_totalScore;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ETotalBureauOverGame, scoreArgs);
    }



    void RecevDisRoomFail(MsgGlobal msg)
    {
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EDissmisRoomResultFail);
    }


    void RecevRoomApply(MsgGlobal msg)
    {
        MsgDisRoomInfo data = msg.dis_room_info;
        ulong pid = data.voteFirst; //(uint)msg.ReadUInt64(); //发起投票人

        Debug.Log("RecevRoomApply :" + ">>>pid:" + pid);
        ArgsDissmissRoomApplyer args = new ArgsDissmissRoomApplyer();
        args.playerId = (uint)pid;
        args.voteTime = data.voteTime;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EDissmisRoomApply, args);
    }

    void RecevRoomAgree(MsgGlobal msg)
    {
        uint pid = (uint)msg.actionId;
        Debug.Log("RecevRoomApply :"+ ">>>pid:" + pid);
        ArgsDismissRoomApplyResult args = new ArgsDismissRoomApplyResult();
        args.playerId = pid;
        args.isAgree = true;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESingleDissRoomApplyResult, args);
    }

    void RecevRoomRefuse(MsgGlobal msg)
    {
        uint pid = (uint)msg.actionId;
        Debug.Log("RecevRoomApply :"  + ">>>pid:" + pid);
        ArgsDismissRoomApplyResult args = new ArgsDismissRoomApplyResult();
        args.playerId = pid;
        args.isAgree = false;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESingleDissRoomApplyResult, args);
    }



    void NotificaPlayerPut(ulong playerId, ulong lastOperationId, MsgActionType type, bool isYbq)
    {
        ArgsPlayerPut args = new ArgsPlayerPut();
        args.playerId = playerId;
        args.lastOperationId = lastOperationId;
        args.type = type;
        args.isYbq = isYbq;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EPlayerPlayingStatus, args);
    }

    void RevcCanPutOutCard(MsgGlobal msg)
    {
        SelfCardsManager.Instance.initTonghuashunTip();
        MsgGuandanRoom data = msg.guandan_room;
        NotificaPlayerPut(data.action.action_id, data.action.last_operation_pid, data.action.action_type, data.ybq); //玩家出牌箭头指示 （更新玩家出牌状态）
        //游戏中胜利是否判断
        var gameOverInfo = data.game_over_info;

        bool isGameOver = gameOverInfo.Count > 0;
        if (isGameOver)
        {
            PlayingGameInfo.Instance.mGameOverInfoList = gameOverInfo; //用于记录 ，临时退出
        }
        PlayingGameInfo.Instance.mPlayingStatus = EPlayingStatus.EBureau;
        var selfId = PlayerInfo.Instance.mPlayerPid;

        ArgsCanputOutCard args = new ArgsCanputOutCard();
        args.playerId = (uint)data.action.action_id;//当前出牌玩家id
        args.cards = data.action.last_card;  //出牌信息
        args.putOutType = data.action.last_ct;//玩家出牌类型
        args.lastPlayerId = (uint)data.action.last_action_id;
        args.actionType = data.action.action_type; //玩家是否是借风之类
        args.lastOperationPId = data.action.last_operation_pid; // 上一次打出牌的玩家id(非不出)
        args.sendCardInfo = data.send_card_info; // 每个玩家都要对比手牌数量
        args.DjCards = data.djCards;
        SelfCardsManager.Instance.mIsRecevCardTip = false;
        SelfCardsManager.Instance.mMsgAction = data.action;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ECanPutOutCard, args);


        //结算(出头游)
        var gameCleanInfo = data.player_clean_info;

        if (gameCleanInfo != null)
        {
            Debug.Log("=========玩家" + gameCleanInfo.player_id + "胜利" + gameCleanInfo.rank + "游");
            var posType = RoomInfo.Instance.GetPlayerPositionType(gameCleanInfo.player_id);
            PlayingGameInfo.Instance.AddLastCleanPlayerDic(posType);

            ArgsGameCleanInfo e = new ArgsGameCleanInfo();
            e.playerId = gameCleanInfo.player_id;
            e.rank = gameCleanInfo.rank;
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGameClean, e);
        }




        if (isGameOver)
        {
            ArgsMsgGameOverMsg argsGameOver = new ArgsMsgGameOverMsg();
            for (int i = 0; i < gameOverInfo.Count; i++)
            {
                var itemInfo = gameOverInfo[i];
                argsGameOver.mGameOverList.Add(itemInfo);
                if (itemInfo.player_id == selfId) //是自己的情况，计算加分
                {
                    PlayingGameInfo.Instance.mScore += (int)itemInfo.goal;
                }
                else
                {
                    RoomInfo.Instance.GetPlayerInfoById(itemInfo.player_id).goal += (int)itemInfo.goal;
                }
            }

            //展示最后出的手牌
            if (data.game_over_card != null)
            {
                var lastCard = data.game_over_card.send_card_info;
              
                if (lastCard != null && lastCard.Count > 0)
                {
                    for (int i = 0; i < lastCard.Count; i++)
                    {
                        argsGameOver.mShowLastCard.Add(lastCard[i]);
                    }
                }
            }
            argsGameOver.mIsTotalOver = PlayingGameInfo.Instance.mGameInfo.game_index == RoomInfo.Instance.mRoom.game_num;
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGameBureauOver, argsGameOver);
        }
    }


    void RevcPutOutFail(MsgGlobal msg)
    {
        Debug.Log("出牌失败回调");
        UIManagers.Instance.EnqueueTip("出牌规则不符");
    }


    void RecevTonghuanshun(MsgGlobal msg)
    {
        MsgCardTip data = msg.card_tip;
        SelfCardsManager.Instance.mTonghuashunTipList = data.cardGroup;
        Debug.Log("同花顺数量:" + SelfCardsManager.Instance.mTonghuashunTipList.Count);
        SelfCardsManager.Instance.PostTonghuashunToCards();
    }


    //void RecevRefresh(MsgGlobal msg)
    //{
       
    //    Debug.Log("RecevRefresh");
    //    RoomInfo.Instance.mRoomId = msg.room_id;
    //    // SelfCardsManager.Instance.initTonghuashunTip();
    //    if (GameManager.Instance.mIsUseAI) { return; }
    //    MsgGuandanRoom data = msg.guandan_room;
    //    PlayingGameInfo.Instance.mGameInfo.game_joker = data.game_info.game_joker;
    //    PlayingGameInfo.Instance.mGameInfo.game_index = data.game_info.game_index;
    //    SelfCardsManager.Instance.mIsRecevCardTip = false;
    //    SelfCardsManager.Instance.mMsgAction = data.action;
    //    List<uint> oldSelfCardIdList = new List<uint>(); // 用作保留选中的牌
    //    oldSelfCardIdList.AddRange(PlayingGameInfo.Instance.mSelfBaseCardIdList);
    //    PlayingGameInfo.Instance.GetSelfCards(data.send_card_info); //获取自己的手牌
    //    ArgsRefreshData args = new ArgsRefreshData();
    //    args.action = data.action;//下一个出牌人，用于箭头指向
    //    args.gameOverInfoList = data.game_over_info;//刷新是否有人有头游，二游之类
    //    args.last3CardList = data.last3card;//同步最近三个玩家出牌信息
    //    args.playerInfoList = data.player_info; //同步其他玩家剩余牌
    //    args.oldSelfCardIdList = oldSelfCardIdList;
    //    PlayingGameInfo.Instance.GetTributeInfo(data.game_tribute_info);//玩家的贡牌信息 
    //    //Refresh Data
    //    NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ERefreshData, args);


    //    CheckDissRoomStatus(data.dissRoomInfo.dissRoomPlayer, data.dissRoomInfo.voteTime);

    //    //如果本来游戏已经结束了，只会发PlayerInfo。判断actioni为空，就是本局打完，在游戏结算中，发readyForStart
    //}

    void CheckDissRoomStatus(List<ulong> playerList, string time)
    {
        if (playerList == null || playerList.Count == 0 || GameObject.Find("VoteDismissView") != null)
        {
            return;
        }
        ulong firstPlayer = playerList[0];
        ContextManager.Instance.Push(new VoteDismissViewContext());
        UIVoteDismissView dissView = UIManagers.Instance.GetSingleUI(UIType.VoteDismissView).GetComponent<UIVoteDismissView>();
        dissView.InitData(firstPlayer, playerList);
    }

    void RecevMustRefresh(MsgGlobal msg)
    {
        SelfCardsManager.Instance.initTonghuashunTip();
        SelfCardsManager.Instance.ClearAllSelectCards();
        PlayingGameInfo.Instance.SendRefreshGameServer();
    }

    void RecevSortCard(MsgGlobal msg)
    {
        MsgAction data = msg.action;
        var cardList = data.action_card;
        cardList.Reverse();
        SelfCardsManager.Instance.mCurSelectCardIds.Clear();
        for (int i = 0; i < cardList.Count; i++)
        {
            SelfCardsManager.Instance.mCurSelectCardIds.Add(cardList[i]);
        }
        SaveSortRecord(cardList, data.action_ct);
        ArgsCard args = new ArgsCard();
        args.type = data.action_ct;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESortCard, args);
    }

    void SaveSortRecord(List<uint> list, TGuanDanCT type)
    {
        int id = SelfCardsManager.Instance.mSortRecordList.Count;

        List<uint> tempList = new List<uint>();
        for (int i = 0; i < list.Count; i++)
        {
            tempList.Add(list[i]);
        }
        SelfCardsManager.Instance.mSortRecordList.Add(tempList);

        SelfCardsManager.Instance.mSortRecordTypeList.Add(type == TGuanDanCT.CT_TONG_HUA_SHUN ? 0 : 100); //同花顺则放到第一个位置，否则放到最后一个位置(100代替)
    }
}
