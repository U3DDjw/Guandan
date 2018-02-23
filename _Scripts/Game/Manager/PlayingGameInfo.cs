using UnityEngine;
using System.Collections;
using Common;
using ZhiWa;
using System.Collections.Generic;
using MsgContainer;
using Net;
using DNotificationCenterManager;
using Haoyun.Utils;

public class LastCleanPlayerInfomation
{
    public int idx;
    public int totaIdxTimes;
}
/// <summary>
/// 打牌游戏`状态
/// </summary>
public class PlayingGameInfo : SingleTon<PlayingGameInfo>
{
    public int mReconnectId = 0;//重连是加入房间，还是加入战斗，还是正常，1，2，3
    public int mScore; //当前得分
    public bool mIsAutoPlay = false; // 自动出牌
    public EPlayingStatus mPlayingStatus = EPlayingStatus.ENull;
    public List<MsgGameOverInfo> mGameOverInfoList = new List<MsgGameOverInfo>();
    private MsgGameInfo gameInfo = new MsgGameInfo();
    public List<MsgGameOverInfo> GetmGameOverInfoList()
    {
        ArgsMsgRoomInfo roominfo = RoomInfo.Instance.mRoom;
        if (roominfo == null) return null;
        if (roominfo.endingInfo == null) return null;
        if (roominfo.endingInfo.Count == 0)
        {
            Debug.LogError("ArgsMsgRoomInfo.endingInfo的count数量为0");
            return null;
        }
        MsgGameOverInfo gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = roominfo.endingInfo[0].player_id[0];//只用到了id
        gameinfo.upgrade_card = roominfo.endingInfo[0].level;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = roominfo.endingInfo[0].player_id[1];//只用到了id
        gameinfo.upgrade_card = roominfo.endingInfo[0].level;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = roominfo.endingInfo[1].player_id[0];//只用到了id
        gameinfo.upgrade_card = roominfo.endingInfo[1].level;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = roominfo.endingInfo[1].player_id[1];//只用到了id
        gameinfo.upgrade_card = roominfo.endingInfo[1].level;//打到几
        mGameOverInfoList.Add(gameinfo);

        return mGameOverInfoList;
    }
    /// <summary>
    /// 断线重连 连战斗状态
    /// </summary>
    public bool IsConnectPlayingStatus
    {
        get
        {
            return mReconnectId == 2;
        }
    }

    /// <summary>
    /// 断线重连 连等候状态
    /// </summary>
    public bool IsConnectWaitingStatus
    {
        get
        {
            return mReconnectId == 1;
        }
    }
    /// <summary>
    /// 当前游戏信息（局数）
    /// </summary>
    public MsgGameInfo mGameInfo
    {
        get
        {
            return gameInfo;
        }

        set
        {
            if (gameInfo == null)
            {
                gameInfo = new MsgGameInfo();
            }
            gameInfo = value;
        }
    }

    /// <summary>
    /// 百塔牌
    /// </summary>
    public uint mCommonJokerCard
    {
        get
        {
            if (gameInfo != null)
            {
                return gameInfo.game_joker;
            }
            return 2;
        }
    }
    
    public GameObject mNowAirGm
    {
        get; set;
    }
    private List<MsgFightingCardLog> fightingCardLog = null;
    /// <summary>
    /// 断线重连之后其他人出的牌信息(读取完数据之后应该清空)
    /// </summary>
    public List<MsgFightingCardLog> mFightingCardLog
    {
        get
        {
            return fightingCardLog;
        }

        set
        {
            if (fightingCardLog == null)
            {
                fightingCardLog = new List<MsgFightingCardLog>();
            }

            fightingCardLog = value;
        }
    }


    /// <summary>
    /// 获取自己牌
    /// </summary>
    /// <param name="list"></param>
    public void GetSelfCards(List<MsgSendCardInfo> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].player_id == PlayerInfo.Instance.mPlayerPid)
            {
                mSelfBaseCardIdList.Clear();
                IEnumerator<uint> iCards = list[i].card.GetEnumerator();
                while (iCards.MoveNext())
                {
                    mSelfBaseCardIdList.Add(iCards.Current);
                }
                return;
            }
        }
    }

    private List<uint> selfBaseCardIdList = new List<uint>();
    /// <summary>
    /// 自己牌的详细数据
    /// </summary>
    public List<uint> mSelfBaseCardIdList
    {
        get
        {
            return selfBaseCardIdList;
        }
    }

    Dictionary<EPlayerPositionType, LastCleanPlayerInfomation> lastCleanPlayerInfoDic =
       new Dictionary<EPlayerPositionType, LastCleanPlayerInfomation>();
    /// <summary>
    /// 每局完成 完成游 的玩家,以及打完之后的信息，用于打完一轮之后清空牌
    /// </summary>
    public Dictionary<EPlayerPositionType, LastCleanPlayerInfomation> mLastCleanPlayerInfoDic
    {
        get
        {
            if (lastCleanPlayerInfoDic == null)
            {
                lastCleanPlayerInfoDic = new Dictionary<EPlayerPositionType, LastCleanPlayerInfomation>();
            }
            return lastCleanPlayerInfoDic;
        }
        set
        {
            if (lastCleanPlayerInfoDic == null)
            {
                lastCleanPlayerInfoDic = new Dictionary<EPlayerPositionType, LastCleanPlayerInfomation>();
            }
            lastCleanPlayerInfoDic = value;
        }
    }

    public void AddLastCleanPlayerDic(EPlayerPositionType type)
    {
        if (lastCleanPlayerInfoDic == null)
        {
            lastCleanPlayerInfoDic = new Dictionary<EPlayerPositionType, LastCleanPlayerInfomation>();
        }
        if (!lastCleanPlayerInfoDic.ContainsKey(type))
        {
            LastCleanPlayerInfomation info = new LastCleanPlayerInfomation();
            info.idx = 1;
            info.totaIdxTimes = 4 - lastCleanPlayerInfoDic.Count;
            lastCleanPlayerInfoDic.Add(type, info);
        }

    }

    /// <summary>
    /// 获取当前玩家上家是否是游，如果是，则返回上家Pos
    /// </summary>
    /// <param name="curPosType"></param>
    /// <returns></returns>
    public EPlayerPositionType GetNeedLastClearOutCards(EPlayerPositionType curPosType)
    {
        if(IsHavePlayerGameClean())
        {
            int posId = (int)curPosType;
            posId++;
            if (posId > 4) {
                posId = 1;
            }
            if(lastCleanPlayerInfoDic.ContainsKey((EPlayerPositionType)posId))
            {
                return (EPlayerPositionType)posId;
            }
        }
        return curPosType;
    }


    public bool IsHavePlayerGameClean()
    {
        if (lastCleanPlayerInfoDic == null)
        {
            return false;
        }

        if (lastCleanPlayerInfoDic.Count > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 解散房间请求 （战斗中）
    /// </summary>
    public void SendDissRoomApplyMsg()
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.dis_room_info = new MsgDisRoomInfo();
        MsgDisRoomInfo msg = mGl.dis_room_info;
        msg.voteTime = TimeUtils.ConvertDateTimeInt(System.DateTime.Now).ToString();
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_DISS_ROOM, mGl);
    }


    /// <summary>
    /// 拒绝解散请求
    /// </summary>
    public void SendDissRoomRefuseMsg()
    {
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_DISS_ROOM_DISAGREE);
    }

    /// <summary>
    /// 同意解散请求
    /// </summary>
    public void SendDissRoomAgreeMsg()
    {
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_DISS_ROOM_AGREE);
    }

    /// <summary>
    /// 进入下一局
    /// </summary>
    public void EnterNextBureauGame()
    {
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EStartLoadingGame);
        SendStartGame();
    }

    public void SendStartGame()
    {
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_READY_START);
    }

    public void changeToEPlaying()
    {
        GameManager.Instance.SwitchGameStatus(EGameStatus.EPlaying);
    }

    #region 进贡
    private MsgTributeInfo tributeInfo = null;
    /// <summary>
    /// 贡牌数据
    /// </summary>
    /// 
    public MsgTributeInfo mTributeInfo
    {
        get
        {
            return tributeInfo;
        }
    }

    public void ClearTributeInfo()
    {
        tributeInfo = null;
    }

    public void GetTributeInfo(MsgTributeInfo info)
    {
        if (tributeInfo == null)
        {
            tributeInfo = new MsgTributeInfo();
        }
        tributeInfo = info;

    }
    /// <summary>
    /// 是否是贡的状态(非打2)
    /// </summary>
    public bool IsTributeStatus
    {
        get
        {
            if (tributeInfo != null)
            {
                return tributeInfo.kgbs_tag != 2 ? true : false;
            }
            return false;
        }
    }


    ///// <summary>
    ///// 仅仅 进贡，回贡状态(用于显示点击颜色)
    ///// </summary>
    //public bool IsOnlyTributeStatus
    //{
    //    get
    //    {
    //        if (tributeInfo != null)
    //        {
    //            if (tributeInfo.kgbs_tag == 0)
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //}
    /// <summary>
    /// 抗贡
    /// </summary>
    public bool IsNoTribute
    {
        get
        {
            if (tributeInfo != null)
            {
                return tributeInfo.kgbs_tag == 1;
            }
            return false;
        }
    }
    /// <summary>
    /// 是否是进贡玩家
    /// </summary>
    public bool IsJGPlayer
    {
        get
        {
            if(tributeInfo == null)
            {
                return false;
            }

            return tributeInfo.jg_player_id.Contains(PlayerInfo.Instance.mPlayerPid);
        }
    }

    public bool IsHGPlayer
    {
        get
        {
            return tributeInfo.bjg_player_id.Contains(PlayerInfo.Instance.mPlayerPid);
        }
    }


    public EGuandanGameStatus GuandanStatus
    {
        get
        {
            if (tributeInfo != null)
            {
                switch (tributeInfo.kgbs_tag)
                {
                    case 0:
                        return EGuandanGameStatus.EGuandanNormal;
                    case 1:
                        return EGuandanGameStatus.EGuandanKanggong;
                    case 2:
                        return EGuandanGameStatus.EGuandan2;
                }
            }
            return EGuandanGameStatus.ENull;
        }
    }

    /// <summary>
    /// 发起进贡
    /// </summary>
    /// <param name="cardId"></param>
    public void SendTribute(uint cardId)
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.tribute_info = new MsgTributeInfo();
        MsgTributeInfo msg = mGl.tribute_info;
        msg.jgpz_card.Add(cardId);
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_TRIBUTE, mGl);
    }

    /// <summary>
    /// 发起回贡
    /// </summary>
    public void SendBackTribute(uint cardId)
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.tribute_info = new MsgTributeInfo();
       MsgTributeInfo msg = mGl.tribute_info;
        msg.hgpz_card = cardId;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_BACK_TRIBUTE, mGl);
    }
    #endregion
    /// <summary>
    /// 清空当前所有开的游戏数据
    /// </summary>
    public void ClearAllGameInfo()
    {
        gameInfo = null;
        lastCleanPlayerInfoDic = null;
        mScore = 0;

        mSelfBaseCardIdList.Clear();
        ClearTributeInfo();
        mPlayingStatus = EPlayingStatus.ENull;
    }

    /// <summary>
    /// 清空当前正在开的局数的数据
    /// </summary>
    public void ClearCurGameInfo()
    {
        mSelfBaseCardIdList.Clear();
        lastCleanPlayerInfoDic = null;
        ClearTributeInfo();
        mPlayingStatus = EPlayingStatus.EBureau;
    }

    public void SendConfirmSameIpServer()
    {
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_IP_CONFIRM);
    }

    public void SendRefreshGameServer()
    {
        return; //暂时屏蔽
        Debug.Log("SendRefreshGameServer");
        if (RoomInfo.Instance.mRoomCode < 1)
        {
            return;
        }
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_REFRESH);
    }

    public void SendReconnectServer()
    {
        TweenManager.Instance.SwitchLoadingSpr(false);
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_RECONNECT);
    }
    /// <summary>
    /// 亮牌动画结束位置
    /// </summary>
    /// <param name="gm"></param>
    /// <returns></returns>
    public Vector3 GetLightCardPosEnd(GameObject gm, bool isSame = false)
    {
        Vector3 endPos = Vector3.zero;
        if (gm.transform.IsChildOf(GameObject.Find("PlayerRight").transform))
        {
            endPos = new Vector3(29f, -27.5f, 0);
            if (isSame)
            {
                endPos += new Vector3(-29, 0, 0);
            }
        }
        else if (gm.transform.IsChildOf(GameObject.Find("PlayerLeft").transform))
        {

            endPos = new Vector3(-29f, -27.5f, 0);
            if (isSame)
            {
                endPos += new Vector3(29, 0, 0);
            }
        }
        else if (gm.transform.IsChildOf(GameObject.Find("PlayerTop").transform))
        {

            endPos = new Vector3(-29f, -27.5f, 0);
            if (isSame)
            {
                endPos += new Vector3(29, 0, 0);
            }
        }
        else if (gm.transform.IsChildOf(GameObject.Find("SelfCardsContainer").transform))//parent 一级
        {

            endPos = new Vector3(-29f, -27.5f, 0);

            if (isSame)
            {
                endPos += new Vector3(29, 0, 0);
            }
        }
        return endPos;

    }
}
