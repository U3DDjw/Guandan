using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MsgContainer;
using Common;
using UnityEngine.SceneManagement;
using Net;
using ZhiWa;
using System.IO;
using DNotificationCenterManager;
using HaoyunFramework;
using Haoyun.Utils;

public class BaseModule : SingleTon<BaseModule>, IModule
{
    private bool isLoadMiddleScene = false;
    /// <summary>
    /// 模块加载方式
    /// </summary>
    public bool mIsLoadByMiddleSence
    {
        get
        {
            return isLoadMiddleScene;
        }

        set
        {
            isLoadMiddleScene = value;
        }
    }

    public Game mGameController
    {
        get
        {
            var find = GameObject.Find(GlobalData.mGameControllerName);
            if (find != null)
            {
                return find.GetComponent<Game>();
            }
            else
            {
                Debug.LogError("加载GameController失败");
                return null;
            }
        }
    }

    /// <summary>
    /// 切换Module有两种，一种是byMidddleScene，二是通过加载View
    /// </summary>
    public virtual void LoadModule()
    {
        AddServerCallBack(); //首先挂载服务器回调协议
        if (mIsLoadByMiddleSence)
        {
            mGameController.LoadNextScene(GlobalData.mScene_Transition);
        }
        AddServerCall();
    }
    void AddServerCall()
    {
        //TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_NEED_RECONNECT, RecevNeedReconnect);//强制断线重连
    }

    public virtual void AddServerCallBack()
    {
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GAME_ERR, RevcError); //公用回调协议
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_RGI, RevcReconnectData); //断线重连回调

        TCPNetWork.GetInstance ().AddCallback (ServerMsgKey.GUANDAN_ROOM_GD_START_GAME, StartInitLicense); //游戏开始，发牌,暂时测试，记得改回去

        //道具
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_GOLD_NOT_ENOUGH, RecevGoldNotEnough);

        
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_USE_PROP, RecevUseProp);//收到表情回调
        //使用表情聊天
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_USE_EMOTICON, RecevUseEmoticon);
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_CARD_TIP, RecevCardTip); //Tip回调
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_REPEAT_LOGIN, RecevRepeatLogin);
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_TALK, RecevRoomTalk); //语音回调
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_HORSE_INFO, RecevHorseInfo); //跑马灯回调

        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_DISS_SUC, RecevDisRoomSuc);//解散投票结果 --成功

        //金币场
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_GOLD_READY_SUC, RecevReadyGoldSuc);//准备金币场成功 --成功
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_GOLD_OUT_PLAYGROUND_SUC, RecevGoldFiledOutSuc);//金币场退出 --成功
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GOLD_GD_START_GAME, RecevGoldStartGame);//金币场开始游戏(发牌) --成功

        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GAME_OFFLINE, RecevOtherPlayerOffLine);//其他玩家掉线

        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_ONLINE, RevcOnline); //玩家上线，通知其他人


    }


  


    void RecevOtherPlayerOffLine(MsgGlobal mGl)
    {
        var offPlayerId = mGl.actionId;
        Debug.Log("Recv OtherPlayer OffLine :"+offPlayerId);
        UIManagers.Instance.EnqueueTip(string.Format("玩家{0} 下线", offPlayerId));
        ArgsOfflinePlayer args = new ArgsOfflinePlayer();
        args.playerId = (uint)offPlayerId;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EOfflinePlayer, args);
    }

    void RevcOnline(MsgGlobal mGl)
    {
        var onLinePlayerId = mGl.actionId;
        Debug.Log("On Line PlayerId:"+onLinePlayerId);
        UIManagers.Instance.EnqueueTip(string.Format("玩家{0} 上线", onLinePlayerId));
        ArgsOfflinePlayer args = new ArgsOfflinePlayer();
        args.playerId = (uint)onLinePlayerId;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EOnLinePlayer, args);
    }

    #region 金币场
    void RecevGoldStartGame(MsgGlobal mGl)
    {
      
        Debug.Log("=========StartGame:开始游戏发牌消息" + System.DateTime.Now.ToLongTimeString());
        MsgGuandanRoom data = mGl.guandan_room;
		Debug.Log("Common_Joker:" + data.game_info.game_joker);

        //计算消耗金币
        PlayerInfo.Instance.UpdateGold(-1*(int)GoldFiledManager.Instance.GetCostGold);

        SetOtherPlayerPosition(data.game_info.fight_player_id);
        // 同步牌
        PlayingGameInfo.Instance.mGameInfo = data.game_info;
        PlayingGameInfo.Instance.GetSelfCards(data.send_card_info); //获取自己牌
        bool isStart = true;
		CheckIsStartGame (data.game_tribute_info, out isStart);
		ResetGameStatus (data.action.action_id,  isStart);
		StartRefreshGame (data.game_info.game_index);
     
    //    PostFirstPutPlayer(data.action.action_id, isStart);
    }


    void PostFirstPutPlayer(ulong playerId,bool isStart)
    {
        SelfCardsManager.Instance.mIsRecevCardTip = false;
        PlayingGameInfo.Instance.mPlayingStatus = isStart ? EPlayingStatus.EPlaying : EPlayingStatus.ETribute;
        ArgsFirstPutPlayer firstArgs = new ArgsFirstPutPlayer();
        firstArgs.playerId = playerId;
        firstArgs.isStart = isStart;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EFirstPutOutCard, firstArgs);
    }

	/// <summary>
	/// 正式开始游戏发牌
	/// </summary>
	/// <param name="idxGame">Index game.</param>
	void StartRefreshGame(uint idxGame)
	{
		Debug.Log("当前第data.game_info.game_index:"+idxGame+"局");
		if (idxGame == 1) {
            ContextManager.Instance.Pop(UIType.WaitGameView.Name);
			PlayingGameInfo.Instance.changeToEPlaying ();
		} else {
			NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldRefreshStartGame);
		}
	}

	void CheckIsStartGame(MsgTributeInfo info, out bool isStart)
	{
		 isStart = true;
		if (info != null  && info.kgbs_tag != 2) //掼蛋模式下(非2，屏蔽第一局数据,0抗贡，1表)
		{
			PlayingGameInfo.Instance.GetTributeInfo(info);
			isStart = info.start_tag == 1;
			Debug.Log("data.game_tribute_info.start_tag:" + info.start_tag);
		}
	}

	void ResetGameStatus(ulong firstPlayerId,bool isStart)
    {
		string strLog = string.Format ("先出牌玩家:{0},isStart:{1}", firstPlayerId, isStart);


		SelfCardsManager.Instance.mIsRecevCardTip = false;
		PlayingGameInfo.Instance.mPlayingStatus = EPlayingStatus.EReady;
		PlayingGameInfo.Instance.mPlayingStatus = isStart ? EPlayingStatus.EPlaying : EPlayingStatus.ETribute;

		GoldFiledManager.Instance.mFirstPlayerId =firstPlayerId;
		GoldFiledManager.Instance.mIsStartGame = isStart;
    }


    void RecevGoldFiledOutSuc(MsgGlobal mGl)
    {
        MsgGoldOutPlayer info = mGl.gold_out_player;
        var pId = info.pid;
        if (pId == PlayerInfo.Instance.mPlayerPid)
        {
            RoomInfo.Instance.ClearInfo();
        }
        else
        {
            RoomInfo.Instance.RemovePlayer((uint)pId);
        }
        ArgsPlayerId args = new ArgsPlayerId();
        args.playerId = pId;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldOutSuc,args);
    }

    void RecevReadyGoldSuc(MsgGlobal mGl)
    {
        Debug.Log("RecevReadyGoldSuc");
        MsgGoldReadyPlayer info = mGl.gold_ready_player;
        if (info == null) { return; }
        var list = info.pid;
        if( list != null)
        {
            GoldFiledManager.Instance.UpdateReadyPlayer(list);
            ArgsPlayerList args = new ArgsPlayerList();
            for(int i=0;i<list.Count;i++)
            {
                args.playerIdList.Add(list[i]);
            }
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldReadySuc,args);
        }
    }


	#endregion
    void RecevNeedReconnect(MsgGlobal msg)
    {
        Debug.Log("强制 断线重连消息");
        PlayingGameInfo.Instance.SendReconnectServer();
    }

    void RecevDisRoomSuc(MsgGlobal mGl)
    {
        if (RoomInfo.Instance.mRoom == null) { return; }

        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EDissmisRoomResultSuc);
    }
    void RecevGoldNotEnough(MsgGlobal msg)
    {
        Debug.Log("金币不够");
        UIManagers.Instance.EnqueueTip("金币不足");
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldNotEnough);
    }
    void RecevUseProp(MsgGlobal mGl)
    {
        if (RoomInfo.Instance.mIsExistWaitGameView)
        {
            return;
        }
        MsgPropsInfo data = mGl.props_info;
        Debug.Log(">>>>>>>>>>>>可以使用道具");
        ArgsPropsInfo args = new ArgsPropsInfo();
        args.action_id = data.action_id;
        args.target_id = data.target_id;
        args.propsId = data.propsId;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUseProp, args);
    }

    void RecevRepeatLogin(MsgGlobal msg)
    {
        UIManagers.Instance.EnqueueTip("检测该账号异地登陆，请重新登陆");
        //直接跳到登录界面
        XPlayerPrefs.Instance.ClearAllInfo();
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EBackToLoginScene);
    }

    void RecevHorseInfo(MsgGlobal mGl)
    {
        var info = mGl.horse_info; //待补充，这边改为list形式，直接给我展示出来
        //HorseData data = new HorseData();
        //data.content = info.content;
        //data.enableTime = info.enable_time;
        //data.horseId = info.horse_id;
        //data.ineffectTime = info.ineffect_time;
        //data.intervalTime = (int)info.intervalTime;
        //data.perWaveInteval = (int)info.perWaveInteval;
        //data.perWaveNumber = (int)info.perWaveNumber;
        //data.sender = info.send_time;
        //PlayerInfo.Instance.UpdateCurHorseData(data);
        //NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EHorseRefresh);
    }

    void RecevRoomTalk(MsgGlobal mGl)
    {
        MsgGuandanRoom info = mGl.guandan_room;

        ArgsTalk args = new MsgContainer.ArgsTalk();
        args.voiceName = info.voiceName;
        args.time = (float)info.msg_talk.time;
        args.talkPid = (uint)info.action.action_id;
        AudioManager.Instance.mTalkQueues.Enqueue(args);

        Debug.Log("voiceName:" + args.voiceName + "   talkPid:" + args.talkPid);
        //  SDKManager.Instance.yayaPlay(info.voiceName);
        Debug.Log("voiceName:" + args.voiceName);
        Debug.Log("talkPid:" + args.talkPid);
        // 表现谁在说话
        //   NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ETalkNotify, args);
    }

    void RecevCardTip(MsgGlobal mGl)
    {
        Debug.Log("Receive Card Tip");
        MsgAction info = mGl.action;
        SelfCardsManager.Instance.UpdateTipCardList(info.cardGroup);
        SelfCardsManager.Instance.mIsRecevCardTip = true;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUpdateTip);
    }

    void RecevUseEmoticon(MsgGlobal mGl)
    {
        if (RoomInfo.Instance.mIsExistWaitGameView)
        {
            return;
        }
        var info = mGl.emotion_info;
        ArgsEmoticonInfo args = new ArgsEmoticonInfo();

        args.action_id = info.action_id;
        args.emoticonId = info.emoticonId;
        args.message = info.message;
        if (args.emoticonId != 0)
        {
            AudioManager.Instance.PlayerEmotionAudio(args.action_id, (uint)args.emoticonId);
        }
        if (args.message != null && args.message.Length != 0)
        {
            AudioManager.Instance.CheckHaveCommon(args.action_id, args.message);
        }
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUseEmoticon, args);
    }
    void RefreshRoomCode()
    {
        PlayerInfo.Instance.AddRoomCard(-RoomInfo.Instance.GetPayNum());
    }
    /// <summary>
    /// 进入战斗状态，发牌
    /// </summary>
    /// <param name="msg"></param>
    void StartInitLicense(MsgGlobal mGl)
    {
        Debug.Log("=========StartGame:开始游戏发牌消息" + System.DateTime.Now.ToLongTimeString());
        MsgGuandanRoom data = mGl.guandan_room;
        if (RoomInfo.Instance.mRoom == null) { return; } //房间已经解散
           
        // 同步牌
        PlayingGameInfo.Instance.mGameInfo = data.game_info;
        PlayingGameInfo.Instance.mPlayingStatus = EPlayingStatus.EReady;
        if (data.game_info.game_index == 1) //刷新房卡，只在游戏第一局结算
        {
            RefreshRoomCode();
        }

        Debug.Log("Common_Joker:" + data.game_info.game_joker);
        PlayingGameInfo.Instance.GetSelfCards(data.send_card_info); //获取自己牌

        bool isStart = true;
        if (data.game_tribute_info != null && !RoomInfo.Instance.IsZhuandanGameType && data.game_tribute_info.kgbs_tag != 2) //掼蛋模式下(非2，屏蔽第一局数据,0抗贡，1表)
        {
            PlayingGameInfo.Instance.GetTributeInfo(data.game_tribute_info);
            isStart = data.game_tribute_info.start_tag == 1;
            Debug.Log("data.game_tribute_info.start_tag:" + data.game_tribute_info.start_tag);
        }
        SetOtherPlayerPosition(data.game_info.fight_player_id);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EInitLicense);

        Debug.Log("First Show card Player:" + data.action.action_id);
     

        PostFirstPutPlayer(data.action.action_id, isStart);
    }
    void SetOtherPlayerPosition(List<ulong> list)
    {
        if (list == null || list.Count < 4)
        {
            return;
        }
        TestDebugList(list);
        RoomInfo.Instance.UpdatePlayingGamePlayersDic(list);
    }

    void TestDebugList(List<ulong> list)
    {
        string str = "==============玩家位置重整 List";
        for (int i = 0; i < list.Count; i++)
        {
            str += list[i] + ",";
        }
        Debug.Log(str);
    }

    /// <summary>
    /// 重新连接
    /// </summary>
    /// <param name="msg"></param>
    void RevcReconnectData(MsgGlobal mGl)
    {
        RoomInfo.Instance.mRoomId = mGl.room_id;
        if (SelfCardsManager.Instance.mIsDragDroping) { return; } //拖拽状态不能掉断线重连。

        SelfCardsManager.Instance.initTonghuashunTip();
        MsgGuandanRoom data = mGl.guandan_room;
        Debug.Log("重连回调所有数据");
        if (data.Tag == 2) //等待房间,调进入房间接口，MessageSession中有RoomCode
        {
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EResetWaitGameView);
            return;
        }
        SelfCardsManager.Instance.mMsgAction = data.action;
        //PlayingGameInfo.Instance.mGameInfo.game_joker = data.game_info.game_joker;
        //PlayingGameInfo.Instance.mGameInfo.game_index = data.game_info.game_index;
        PlayingGameInfo.Instance.mGameInfo = data.game_info;//游戏中数据，为了使用亮牌
        Debug.Log("gameIndex:" + data.game_info.game_index);

        SelfCardsManager.Instance.mIsRecevCardTip = false;
        PlayingGameInfo.Instance.GetSelfCards(data.send_card_info); //获取自己的手牌          
        RoomInfo.Instance.GetRoomInfo(data.room_info); //获取房间信息
        RoomInfo.Instance.UpdatePlayersDic(data.player_info); //获取其他玩家信息   
        RoomInfo.Instance.GetNowIndex(data.room_info.gd_ending);//玩家当前打几      
        //刷新积分 
        if (data.game_totalScore != null && data.game_totalScore.Count > 0)
        {
            for (int i = 0; i < data.game_totalScore.Count; i++)
            {
                var itemInfo = data.game_totalScore[i];
                if (itemInfo.player_id == PlayerInfo.Instance.mPlayerPid)
                {
                    PlayingGameInfo.Instance.mScore = itemInfo.play_goal;
                }
                else
                {
                    RoomInfo.Instance.GetPlayerInfoById(itemInfo.player_id).goal = itemInfo.play_goal;
                }
            }
        }
        ArgsRefreshData args = new ArgsRefreshData();
        args.action = data.action;//下一个出牌人，用于箭头指向
        args.gameOverInfoList = data.game_over_info;//刷新是否有人有头游，二游之类
        args.last3CardList = data.last3card;//同步最近三个玩家出牌信息
        args.playerInfoList = data.player_info; //同步其他玩家剩余牌
        if (data.djCards != null && data.djCards.Count > 0)
        {
            args.djCards = data.djCards;
        }

        PlayingGameInfo.Instance.GetTributeInfo(data.game_tribute_info);//玩家的贡牌信息 

        //金币场
        GoldFiledManager.Instance.UpdateGoldPlayerGroundData(mGl.guandan_room.gold_playground);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EReconnectRefresh, args);
        if (data.Tag == 0) //0,准备战斗，1，战斗中，
        {
            if (!RoomInfo.Instance.mIsGoildRoom) //金币场 准备/换桌 状态（默认，让其再次自己点击一次）
            {
                PlayingGameInfo.Instance.SendStartGame();
            }
            else
            {
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldReadyStatus);
            }
        }

        if (data.dissRoomInfo != null)
        {
            CheckDissRoomStatus(data.dissRoomInfo.dissRoomPlayer);
        }

        //如果本来游戏已经结束了，只会发PlayerInfo。判断actioni为空，就是本局打完，在游戏结算中，发readyForStart
    }  
    void CheckDissRoomStatus(List<ulong> playerList)
    {
        if (playerList == null || playerList.Count == 0)
        {
            return;
        }
        ulong firstPlayer = playerList[0];
        ContextManager.Instance.Push(new VoteDismissViewContext());
        UIVoteDismissView dissView = UIManagers.Instance.GetSingleUI(UIType.VoteDismissView).GetComponent<UIVoteDismissView>();

        dissView.InitData(firstPlayer, playerList);
    }

    //void RevcOnline(MsgGlobal msg)
    //{
    //    MsgOnLine data =msg.onl
    //    string strInfo = data.player_id + "上线" + data.roomCode.ToString();
    //    Debug.Log(strInfo);
    //    ArgsOfflinePlayer args = new ArgsOfflinePlayer();
    //    args.playerId = (uint)data.player_id;
    //    NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EOnLinePlayer, args);
    //}

    //void RevcOffline(MsgGlobal msg)
    //{
    //    MsgOffLine data = Game.Serial.Deserialize(ms, null, typeof(MsgOffLine)) as MsgOffLine;
    //    string strInfo = data.player_id + "掉线" + data.roomCode.ToString();
    //    Debug.Log(strInfo);
    //    ArgsOfflinePlayer args = new ArgsOfflinePlayer();
    //    args.playerId = (uint)data.player_id;
    //    NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EOfflinePlayer, args);
    //}

    void RevcError(MsgGlobal mGl)
    {
        MsgGameError data = mGl.game_error;
        string strInfo = "";
        switch (data.errId)
        {
            case MsgErrors.MsgCreateRoomErr: // 创建房间错误
                strInfo = "创建房间错误";
                break;
            case MsgErrors.MsgEnterRoomErr: // 进入房间错误
                strInfo = "进入房间错误";
                break;
            case MsgErrors.MsgApplyChangeSeatsErr: // 申请调换座位错误
                strInfo = "申请调换座位错误";
                break;
            case MsgErrors.MsgAgreeChangeSeatsErr: // 同意调换座位错误
                strInfo = "同意调换座位错误";
                break;
            case MsgErrors.MsgDissRoomAgreeErr:// 同意解散房间错误（游戏中）
                strInfo = "同意解散房间错误（游戏中）";
                break;
            case MsgErrors.MsgDissRoomDisAgreeErr:// 不同意解散房间错误（游戏中）
                strInfo = "不同意解散房间错误（游戏中）";
                break;
            case MsgErrors.MsgDissRoomErr:// 申请解散房间错误（游戏中）
                strInfo = "申请解散房间错误（游戏中）";
                break;
            case MsgErrors.MsgGdShowCardErr: // 惯蛋战斗错误
                strInfo = "惯蛋战斗错误";
                break;
            case MsgErrors.MsgHertBeatErr:// 心跳错误
                strInfo = "心跳错误";
                break;
            case MsgErrors.MsgOutRoomByCreaterErr: // 房主解散房间错误（游戏未开始）
                strInfo = "房主解散房间错误（游戏未开始）";
                break;
            case MsgErrors.MsgOutRoomByPlayerErr: // 玩家离开房间错误（游戏未开始）
                strInfo = "玩家离开房间错误（游戏未开始）";
                break;
            case MsgErrors.MsgReadyForGdStartErr:// 惯蛋准备开始错误
                strInfo = " 惯蛋准备开始错误";
                break;
            case MsgErrors.MsgReadForStartErr: // 转蛋准备开始错误
                strInfo = " 转蛋准备开始错误";
                break;
            case MsgErrors.MsgReconnectErr: // 重新连接错误
                strInfo = "重新连接错误";
                break;
            case MsgErrors.MsgShowCardErr:// 转蛋战斗错误 
                strInfo = "转蛋战斗错误";
                break;
            case MsgErrors.MsgUsePropsErr: // 使用道具错误
                strInfo = "使用道具错误";
                break;
            case MsgErrors.MsgCancleReadyForStartErr: // 取消准备开始游戏错误
                strInfo = "取消准备开始游戏错误";
                break;
            case MsgErrors.MsgGameQuiteErr:// 退出游戏错误
                strInfo = "退出游戏错误";
                break;
            case MsgErrors.MsgGameTalkErr:// 游戏语音错误
                strInfo = "游戏语音错误";
                break;
            case MsgErrors.MsgUseEmoticonErr:
                strInfo = "使用表情错误";//使用表情错误
                break;
        }
        UIManagers.Instance.EnqueueTip(strInfo);

    }
}
