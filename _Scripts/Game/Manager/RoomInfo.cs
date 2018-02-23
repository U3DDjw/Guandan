using UnityEngine;
using System.Collections;
using Common;
using MsgContainer;
using System.Collections.Generic;
using ZhiWa;
using Net;
using DNotificationCenterManager;
using HaoyunFramework;

public class RoomInfo : SingleTon<RoomInfo>
{

    public bool mIsExistWaitGameView = false;//暂时离开房间（暂离房间，改变Depth，因为还存在接收消息）
    public uint mRoomCode
    {
        get
        {
            if (PlayerInfo.Instance.mPlayerData != null)
            {
                return PlayerInfo.Instance.mPlayerData.roomCode;
            }
            return 0;
        }
    }

    /// <summary>
    /// 是否是金币场模式
    /// </summary>
    public bool mIsGoildRoom {
        get
        {
            return mRoomCode < 100000;
        }
    }



    public string mRoomId; //服务器要的，不知道干啥的
    /// <summary>
    /// 下局打几——zf
    /// </summary>
    private string nextIndex = "2";
    public string mNextIndex
    {
        get
        {
            nextIndex = GlobalData.StringUpgradeIndex(nextIndex);
            return nextIndex;
        }
        set { nextIndex = value; }
    }


    /// <summary>
    /// 判断房间是否解散
    /// </summary>
    public bool mRoomIsDis = false;
    private ArgsMsgRoomInfo room = new ArgsMsgRoomInfo();
    public ArgsMsgRoomInfo mRoom
    {
        get
        {
            return room;
        }

        set
        {
            if (room == null)
            {
                room = new ArgsMsgRoomInfo();
            }
            room = value;
        }
    }

    private ArgsSameIpPlayer sameIpArgs = new ArgsSameIpPlayer();
    public ArgsSameIpPlayer mSameIpArgs
    {
        get
        {
            return sameIpArgs;
        }

        set
        {
            if (sameIpArgs == null)
            {
                sameIpArgs = new ArgsSameIpPlayer();
            }
            sameIpArgs = value;
        }
    }
    public TGuanDanGameType mGuandanGameType
    {
        get
        {
            if (room != null)
            {
                return room.game_type;
            }
            return TGuanDanGameType.TGuanDanGameTypeZhuanDan;
        }
    }

    public bool IsZhuandanGameType
    {
        get
        {
            return mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeZhuanDan;
        }
    }
    public bool IsGuandan2GameType
    {
        get
        {
            return mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeGuanDan;
        }
    }

    /// <summary>
    /// 获取team和其他相关
    /// </summary>
    /// <param name="listEnd"></param>
    public void GetNowIndex(List<MsgGdEnding> listEnd)
    {
        if (RoomInfo.Instance.IsZhuandanGameType || RoomInfo.Instance.IsGuandan2GameType)
        {
            return;
        }
        #region 
        for (int i = 0; i < listEnd.Count; i++)
        {
            List<ulong> listId = listEnd[i].player_id;//队
            for (int index = 0; index < listId.Count; index++)//同队的玩家列表
            {
                if (listId[index] == PlayerInfo.Instance.mPlayerPid)//当前的玩家
                {
                    PlayerInfo.Instance.mTeamSelfIndex = listEnd[i].level.ToString();
                    if (listEnd[i].win == 0)//0--胜利 1--失败
                    {
                        mNextIndex = listEnd[i].level.ToString();
                        if (i == 0)
                        {
                            PlayerInfo.Instance.mTeamEnemyIndex = listEnd[1].level.ToString();
                        }
                        else
                        {
                            PlayerInfo.Instance.mTeamEnemyIndex = listEnd[0].level.ToString();
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            mNextIndex = listEnd[1].level.ToString();
                            PlayerInfo.Instance.mTeamEnemyIndex = listEnd[1].level.ToString();
                        }
                        else
                        {
                            mNextIndex = listEnd[0].level.ToString();
                            PlayerInfo.Instance.mTeamEnemyIndex = listEnd[0].level.ToString();
                        }
                    }
                }

            }

        }
        #endregion
    }
    public string GetEndgradOfTGuanDanGameType()
    {
        TGuanDanGameType index = mRoom.game_type;
        string nextTo = "";
        switch (index)
        {
            case TGuanDanGameType.TGuanDanGameTypeGuanDanA:
                nextTo = "A";
                break;
            case TGuanDanGameType.TGuanDanGameTypeGuanDan10:
                nextTo = "10";
                break;
            case TGuanDanGameType.TGuanDanGameTypeGuanDan8:
                nextTo = "8";
                break;
            case TGuanDanGameType.TGuanDanGameTypeGuanDan6:
                nextTo = "6";
                break;
        }
        return nextTo;
    }
    public void GetRoomInfo(MsgRoomInfo info)
    {
        if (room == null)
        {
            room = new ArgsMsgRoomInfo();
        }
        room.card_use_type = info.card_use_type;
        room.creater_id = info.creater_id;
        room.creater_pay = info.creater_pay;
        room.game_num = info.game_num;
        room.game_type = info.game_type;
        room.other_pay = info.other_pay;
        room.endingInfo = info.gd_ending;
    }

    bool IsRoomCreater
    {
        get
        {
            return room.creater_id == PlayerInfo.Instance.mPlayerPid;
        }
    }

    public int GetPayNum()
    {
        if (room.card_use_type == TGuanDanCardUseType.TGuanDanCardUseTypeCreater)
        {
            return IsRoomCreater ? (int)room.creater_pay : 0;
        }
        else
        {
            return (int)room.other_pay;//AA的时候除以房卡的数量
        }
    }
    private Dictionary<EPlayerPositionType, MsgPlayerInfo> playersDic = new Dictionary<EPlayerPositionType, MsgPlayerInfo>(3)
    {
        { EPlayerPositionType.ERight,null },
        { EPlayerPositionType.ETop,null},
        { EPlayerPositionType.ELeft,null}
    };
    /// <summary>
    /// 其他玩家集合
    /// </summary>
    public Dictionary<EPlayerPositionType, MsgPlayerInfo> mPlayersDic
    {
        get
        {
            return playersDic;
        }
    }
    public void ClearOtherPlayerPosition()
    {
        playersDic = new Dictionary<EPlayerPositionType, MsgPlayerInfo>(3)
    {
        { EPlayerPositionType.ERight,null },
        { EPlayerPositionType.ETop,null},
        { EPlayerPositionType.ELeft,null}
    };
    }

    public bool IsExist(uint id)
    {
        foreach (var value in playersDic.Values)
        {
            if (value == null || value.player_id == 0)
            {
                continue;
            }

            if (value.player_id == id)
            {
                return true;
            }
        }
        return false;
    }

    public void RemovePlayer(uint id)
    {
        if (IsExist(id))
        {
            RemovePlayerToNull(id);
        }
    }

    /// <summary>
    /// 获取下一个即将添加的玩家
    /// </summary>
    /// <returns></returns>
    EPlayerPositionType GetNextNewPlayerPosition()
    {
        foreach (var v in playersDic)
        {
            if (v.Value == null || v.Value.player_id == 0)
            {
                return v.Key;
            }
        }
        Debug.LogError("!!!新玩家加入位置错误!!!");
        return EPlayerPositionType.ENull;
    }
    /// <summary>
    /// 更新所有玩家的位置 Important
    /// </summary>
    /// <param name="players"></param>
    public void UpdatePlayingGamePlayersDic(List<ulong> players)
    {
        if (players.Count < 4)
        {
            return;
        }
        //保存playerDic中的数据到temp之后再取出来
        List<MsgPlayerInfo> tempPlayerInfo = new List<MsgPlayerInfo>(3); //临时存储玩家信息
        foreach (var v in playersDic.Values)
        {
            tempPlayerInfo.Add(v);
        }
        ClearOtherPlayerPosition();
        int selfPos = 0;
        IEnumerator<ulong> iters = players.GetEnumerator();
        int idx = 0;
        while (iters.MoveNext())
        {
            if (iters.Current == (ulong)PlayerInfo.Instance.mPlayerPid)
            {
                selfPos = idx;
                break;
            }
            idx++;
        }


        List<ulong> playerPosList = new List<ulong>(3);

        if (idx == 0)
        {
            playerPosList.Add(players[1]);
            playerPosList.Add(players[2]);
            playerPosList.Add(players[3]);
        }
        else if (idx == 1)
        {
            playerPosList.Add(players[2]);
            playerPosList.Add(players[3]);
            playerPosList.Add(players[0]);
        }
        else if (idx == 2)
        {
            playerPosList.Add(players[3]);
            playerPosList.Add(players[0]);
            playerPosList.Add(players[1]);
        }
        else
        {
            playerPosList.Add(players[0]);
            playerPosList.Add(players[1]);
            playerPosList.Add(players[2]);
        }
        playersDic[EPlayerPositionType.ERight] = GetArgsPlayerInfoById(tempPlayerInfo, playerPosList[0], EPlayerPositionType.ERight);
        playersDic[EPlayerPositionType.ETop] = GetArgsPlayerInfoById(tempPlayerInfo, playerPosList[1], EPlayerPositionType.ETop);
        playersDic[EPlayerPositionType.ELeft] = GetArgsPlayerInfoById(tempPlayerInfo, playerPosList[2], EPlayerPositionType.ELeft);
    }

    /// <summary>
    /// 通过其他Id获取其他玩家信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    MsgPlayerInfo GetArgsPlayerInfoById(List<MsgPlayerInfo> list, ulong id, EPlayerPositionType posType)
    {
        IEnumerator<MsgPlayerInfo> iters = list.GetEnumerator();
        while (iters.MoveNext())
        {
            if (iters.Current.player_id == id)
            {
                MsgPlayerInfo info = iters.Current;
                if (posType == EPlayerPositionType.ESelf || posType == EPlayerPositionType.ETop)
                {
                    info.teamType = (int)EPlayersTeam.ETeamA;
                }
                else
                {
                    info.teamType = (int)EPlayersTeam.ETeamB;
                }
                return info;
            }
        }
        return null;
    }
    void RemovePlayerToNull(uint id)
    {
        EPlayerPositionType tempType = EPlayerPositionType.ENull;
        foreach (var t in playersDic)
        {
            if (t.Value != null && t.Value.player_id == id)
            {
                tempType = t.Key;
            }
        }

        playersDic[tempType] = null;
    }
    //当前房间
    public bool IsCreaterPay()
    {
        return room.creater_pay != room.other_pay ? true : false;
    }

    public bool IsCreater()
    {
        return room.creater_id == PlayerInfo.Instance.mPlayerPid ? true : false;
    }

    public int GetRealPlayerCount()
    {
        int count = 0;
        foreach (var t in playersDic)
        {
            if (t.Value != null && t.Value.player_id != 0)
            {
                count++;
            }
        }
        return count;
    }

    public void ClearInfo()
    {
        room = null;
        mSameIpArgs = new ArgsSameIpPlayer();
        PlayerInfo.Instance.mTeamSelfIndex = "2";
        PlayerInfo.Instance.mTeamEnemyIndex = "2";
        nextIndex = "2";
        PlayerInfo.Instance.mPlayerData.roomCode = 0;
        RoomInfo.Instance.mIsExistWaitGameView = false;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUpdateCreateRoomSpr);
        ClearPlayersDic();
    }

    void ClearPlayersDic()
    {
        playersDic[EPlayerPositionType.ELeft] = null;
        playersDic[EPlayerPositionType.ERight] = null;
        playersDic[EPlayerPositionType.ETop] = null;
    }

	public string GetPlayerHeadPortraitByPos(int type)
	{
        if (GetPlayerInfoByPos((EPlayerPositionType)type) != null)
		return GetPlayerInfoByPos ((EPlayerPositionType)type).head_portrait;
        return "";
	}

	public string GetPlayerNameByPos(int type)
	{
        if (GetPlayerInfoByPos((EPlayerPositionType)type) != null)
        {
            return GetPlayerInfoByPos((EPlayerPositionType)type).name;
        }
        return "";
	}

    public MsgPlayerInfo GetPlayerInfoById(ulong pid)
    {
        foreach (var v in playersDic)
        {
            if (v.Value != null && v.Value.player_id != 0)
            {
                if (v.Value.player_id == pid)
                {
                    v.Value.teamType = v.Key == EPlayerPositionType.ETop ? (uint)EPlayersTeam.ETeamA : (uint)EPlayersTeam.ETeamB;
                    return v.Value;
                }
            }
        }

        if (pid == PlayerInfo.Instance.mPlayerPid)
        {
            MsgPlayerInfo playerInfo = new MsgPlayerInfo();
            playerInfo.head_portrait = PlayerInfo.Instance.mPlayerData.headPortrait;
            playerInfo.money = (uint)PlayerInfo.Instance.mPlayerData.money;
            playerInfo.name = PlayerInfo.Instance.mPlayerData.name;
            playerInfo.sex = (uint)PlayerInfo.Instance.mPlayerData.sex;
            playerInfo.player_id = (uint)PlayerInfo.Instance.mPlayerData.pid;
            playerInfo.teamType = (int)EPlayersTeam.ETeamA;
            playerInfo.goal = PlayingGameInfo.Instance.mScore;
            return playerInfo;
        }

        return null;
    }


    public EPlayerPositionType GetPlayerPosById(ulong playerId)
    {
        foreach (var v in playersDic)
        {
            if (v.Value != null)
            {
                if (v.Value.player_id == playerId)
                {
                    return v.Key;
                }
            }
        }

        if (playerId == PlayerInfo.Instance.mPlayerPid)
        {
            return EPlayerPositionType.ESelf;
        }
        //   Debug.LogError("Error Method : GetPlayerPosById");
        return EPlayerPositionType.ENull;
    }
    public MsgPlayerInfo GetPlayerInfoByPos(EPlayerPositionType posType)
    {
        foreach (var value in playersDic)
        {
            if (value.Value != null)
            {
                if (value.Key == posType)
                {
                    return value.Value;
                }
            }
        }

        if (EPlayerPositionType.ESelf == posType)
        {
            MsgPlayerInfo playerInfo = new MsgPlayerInfo();
            playerInfo.head_portrait = PlayerInfo.Instance.mPlayerData.headPortrait;
            playerInfo.money = (uint)PlayerInfo.Instance.mPlayerData.money;
            playerInfo.name = PlayerInfo.Instance.mPlayerData.name;
            playerInfo.player_id = (uint)PlayerInfo.Instance.mPlayerData.pid;
            playerInfo.teamType = (int)EPlayersTeam.ETeamA;
            return playerInfo;
        }

        return null;
    }


    public ulong GetPlayerIdByPos(EPlayerPositionType pos)
    {
        return GetPlayerInfoByPos(pos).player_id;
    }

    public EAudioStyle GetPlayerAudioStyleByPlayerPos(EPlayerPositionType type)
    {
        foreach (var v in playersDic)
        {
            if (v.Key == type)
            {
                return v.Value.sex == 1 ? EAudioStyle.ENormalMan : EAudioStyle.ENormalWoman;
            }

        }

        if (type == EPlayerPositionType.ESelf)
        {
            return PlayerInfo.Instance.GetSexIsMan ? EAudioStyle.ENormalMan : EAudioStyle.ENormalWoman;
        }
        return EAudioStyle.ENormalWoman;
    }

    /// <summary>
    /// 通过pid获取玩家位置
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public EPlayerPositionType GetPlayerPositionType(ulong pid)
    {
        foreach (var t in playersDic)
        {
            if (t.Value != null)
            {
                if (t.Value.player_id == pid)
                {
                    return t.Key;
                }
            }
        }
        if (pid == PlayerInfo.Instance.mPlayerPid)
        {
            return EPlayerPositionType.ESelf;
        }
        return EPlayerPositionType.ENull;
    }

    public void SendJoinRoomServer(int roomCode)
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.room_info = new MsgRoomInfo();
       MsgRoomInfo msg = mGl.room_info;
        msg.roomCode = (uint)roomCode;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_ENTER_ROOM, mGl);
    }

    public void SendExitRoomServer()
    {
        if (RoomInfo.Instance.mRoom == null || RoomInfo.Instance.mRoomCode.Equals(null) || RoomInfo.Instance.mRoomCode == 0)
        {
            return;
        }

        if (RoomInfo.Instance.IsCreater()) //房主,且不是aa的情况下，才凸显出房主的消息
        {
            TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_OUT_ROOM_BY_CREATER);
        }
        else
        {
            TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_OUT_ROOM_BY_PLAYER);
        }
    }
    private Dictionary<ulong, int> dicScore = null;
    public string GetScoreOfId(ulong id)
    {
        if (dicScore.ContainsKey(id))
        {
            return dicScore[id].ToString();
        }
        return null;
    }
    public void UpdateDicScore(Dictionary<ulong, int> mdicScore)
    {
        if (dicScore == null)
        {
            dicScore = new Dictionary<ulong, int>();
        }
        dicScore = mdicScore;
    }


    #region 换座位
    /// <summary>
    /// 申请换座位
    /// </summary>
    /// <param name="toPlayerId"></param>
    public void SendApplyChangeSeat(ulong toPlayerId)
    {
        if (toPlayerId == 0)
        {
            UIManagers.Instance.EnqueueTip("还没有人");
            return;
        }

        MsgGlobal mGl = new MsgGlobal();
        mGl.change_seat_info = new MsgChangeSeatInfo();
       MsgChangeSeatInfo msg = mGl.change_seat_info;
        msg.apply_player_id = PlayerInfo.Instance.mPlayerPid;
        msg.to_player_id = toPlayerId;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_APPLY_CHANGE_SEAT, mGl);
    }

    /// <summary>
    /// 回答是否换座位
    /// </summary>
    /// <param name="toPlayerId"></param>
    public void SendAgreeChangeSeat(ulong applyId, uint result)
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.change_seat_info = new MsgChangeSeatInfo();
        MsgChangeSeatInfo msg = mGl.change_seat_info;
        msg.apply_player_id = applyId;
        msg.to_player_id = PlayerInfo.Instance.mPlayerPid;
        msg.apply_result = result;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_AGREE_CHANGE_SEAT, mGl);
    }


    void CopyClass(MsgPlayerInfo info, EPlayerPositionType posType)
    {
        if (playersDic[posType] == null)
        {
            return;
        }
        info.head_portrait = playersDic[posType].head_portrait;
        info.money = playersDic[posType].money;
        info.name = playersDic[posType].name;
        info.player_id = playersDic[posType].player_id;
        info.sex = playersDic[posType].sex;
        info.teamType = playersDic[posType].teamType;
    }

    public void ChangeTwoPlayerInfo(EPlayerPositionType applyPos, EPlayerPositionType toPos)
    {
        //自己的data永远不变
        if (applyPos == EPlayerPositionType.ESelf || toPos == EPlayerPositionType.ESelf)
        {
            MsgPlayerInfo tempRight = new MsgPlayerInfo();
            MsgPlayerInfo tempLeft = new MsgPlayerInfo();
            MsgPlayerInfo tempTop = new MsgPlayerInfo();
            CopyClass(tempRight, EPlayerPositionType.ERight);
            CopyClass(tempLeft, EPlayerPositionType.ELeft);
            CopyClass(tempTop, EPlayerPositionType.ETop);
            if (tempRight.name == null) { tempRight = null; }
            if (tempLeft.name == null) { tempLeft = null; }
            if (tempTop.name == null) { tempTop = null; }
            ClearPlayersDic();
            //Self 为apply ，则 
            switch (toPos)
            {
                case EPlayerPositionType.ELeft: //left==>right,right==>top,top==>left
                    playersDic[EPlayerPositionType.ELeft] = tempTop;
                    playersDic[EPlayerPositionType.ERight] = tempLeft;
                    playersDic[EPlayerPositionType.ETop] = tempRight;
                    break;
                case EPlayerPositionType.ERight://left==>top,right==>left,top==>right
                    playersDic[EPlayerPositionType.ELeft] = tempRight;
                    playersDic[EPlayerPositionType.ERight] = tempTop;
                    playersDic[EPlayerPositionType.ETop] = tempLeft;
                    break;
                case EPlayerPositionType.ETop://left==>right,right==>left,top==> top
                    playersDic[EPlayerPositionType.ELeft] = tempRight;
                    playersDic[EPlayerPositionType.ERight] = tempLeft;
                    playersDic[EPlayerPositionType.ETop] = tempTop;
                    break;
                case EPlayerPositionType.ESelf:
                    switch (applyPos)
                    {
                        case EPlayerPositionType.ELeft: //left==>right,right==>top,top==>left
                            playersDic[EPlayerPositionType.ELeft] = tempTop;
                            playersDic[EPlayerPositionType.ERight] = tempLeft;
                            playersDic[EPlayerPositionType.ETop] = tempRight;
                            break;
                        case EPlayerPositionType.ERight://left==>top,right==>left,top==>right
                            playersDic[EPlayerPositionType.ELeft] = tempRight;
                            playersDic[EPlayerPositionType.ERight] = tempTop;
                            playersDic[EPlayerPositionType.ETop] = tempLeft;
                            break;
                        case EPlayerPositionType.ETop://left==>right,right==>left,top==> top
                            playersDic[EPlayerPositionType.ELeft] = tempRight;
                            playersDic[EPlayerPositionType.ERight] = tempLeft;
                            playersDic[EPlayerPositionType.ETop] = tempTop;
                            break;
                    }
                    break;
            }
        }
        else
        {
            MsgPlayerInfo tempApply = new MsgPlayerInfo();
            MsgPlayerInfo tempTo = new MsgPlayerInfo();
            CopyClass(tempApply, applyPos);
            CopyClass(tempTo, toPos);
            playersDic[applyPos] = tempTo;
            playersDic[toPos] = tempApply;
        }
    }

    #endregion
    //根据掼蛋类型获取相应的数据
    public string GetgameTypeString(TGuanDanGameType type = TGuanDanGameType.TGuanDanGameTypeNull)
    {
        TGuanDanGameType gameType;
        if (type != TGuanDanGameType.TGuanDanGameTypeNull)
        {
            gameType = type;
        }
        else
        {
            gameType = mRoom.game_type;
        }

        switch (gameType)
        {
            case TGuanDanGameType.TGuanDanGameTypeGuanDan:
                if (Instance.mRoom == null)
                    return "掼蛋:一直打2(" + 0 + "局)";
                else
                    return "掼蛋:一直打2(" + (Instance.mRoom.game_num) + "局)";
            case TGuanDanGameType.TGuanDanGameTypeZhuanDan:
                if (Instance.mRoom == null)
                    return "转蛋:一直打2(" + 0 + "局)";
                else
                    return "转蛋:一直打2(" + (Instance.mRoom.game_num) + "局)";
            case TGuanDanGameType.TGuanDanGameTypeGuanDan6:
                return "掼蛋:打过6";

            case TGuanDanGameType.TGuanDanGameTypeGuanDan8:
                return "掼蛋:打过8";

            case TGuanDanGameType.TGuanDanGameTypeGuanDan10:
                return "掼蛋:打过10";

            case TGuanDanGameType.TGuanDanGameTypeGuanDanA:
                return "掼蛋:打过A";
        }
        return null;
    }

    // 支付方式
    public string GetPayTypeString()
    {
        TGuanDanCardUseType cardUseType = RoomInfo.Instance.mRoom.card_use_type;
        switch (cardUseType)
        {
            case TGuanDanCardUseType.TGuanDanCardUseTypeAa:
                return "AA制";
            case TGuanDanCardUseType.TGuanDanCardUseTypeCreater:
                return "房主支付";
        }
        return "";
    }

    /// <summary>
    /// 根据对象名字中带有的left等位置信息判断位置，获取对应的id
    /// </summary>
    /// <param name="gmName"></param>
    /// <returns></returns>
    public uint GetIdByName(string gmName)
    {
        EPlayerPositionType nowPos = EPlayerPositionType.ENull;
        Debug.Log("当前点击的头像名：——————————" + gmName);
        if (gmName.ToLower().Contains("left"))
        {
            nowPos = EPlayerPositionType.ELeft;
            return (uint)RoomInfo.Instance.mPlayersDic[nowPos].player_id;
        }
        else if (gmName.ToLower().Contains("right"))
        {
            nowPos = EPlayerPositionType.ERight;
            return (uint)RoomInfo.Instance.mPlayersDic[nowPos].player_id;
        }
        else if (gmName.ToLower().Contains("top"))
        {
            nowPos = EPlayerPositionType.ETop;
            return (uint)RoomInfo.Instance.mPlayersDic[nowPos].player_id;
        }
        else
        {
            nowPos = EPlayerPositionType.ESelf;
            return (uint)PlayerInfo.Instance.mPlayerPid;
        }
    }


    public void AddNewPlayerDic(MsgPlayerInfo info)
    {
        playersDic[GetNextNewPlayerPosition()] = info;
    }

    public void UpdatePlayersDic(List<MsgPlayerInfo> list)
    {
        int idx = 0;
        for (int i = 0; i < 4; i++)
        {
            if (i >= list.Count)
            {
                list.Add(new MsgPlayerInfo());
            }

            if (list[i].player_id == PlayerInfo.Instance.mPlayerPid)
            {
                idx = i;
            }
        }

        List<int> sortList = new List<int>();
        switch (idx)
        {
            case 0:
                sortList = new List<int>() { 0, 1, 2, 3 };
                break;
            case 1:
                sortList = new List<int>() { 1, 2, 3, 0 };
                break;
            case 2:
                sortList = new List<int>() { 2, 3, 0, 1 };
                break;
            case 3:
                sortList = new List<int>() { 3, 0, 1, 2 };
                break;
        }

        for (int i = 0; i < sortList.Count; i++)
        {
            if (i == 0) { continue; }
            UpdatePlayerInfoData(list[sortList[i]], PlayerPosByIdx(i));
        }
    }

    EPlayerPositionType PlayerPosByIdx(int idx)
    {
        switch (idx)
        {
            case 1:
                return EPlayerPositionType.ERight;
            case 2:
                return EPlayerPositionType.ETop;
            case 3:
                return EPlayerPositionType.ELeft;
        }
        return EPlayerPositionType.ENull;
    }


    void UpdatePlayerInfoData(MsgPlayerInfo info, EPlayerPositionType playerPosType)
    {
        playersDic[playerPosType] = info;
    }

    /// <summary>
    /// 暂离房间
    /// </summary>
    public void LeaveWaitRoom()
    {
        GameObject.Find("WaitGameView").transform.SetAsFirstSibling();

        mIsExistWaitGameView = true;
        //回到大厅     
        //TweenManager.Instance.HomeViewAnimationShow(true);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUpdateCreateRoomSpr);
    }

    /// <summary>
    /// 进入房间
    /// </summary>
    public void ExistWaitRoom()
    {
        GameObject.Find("WaitGameView").transform.SetAsLastSibling();
        mIsExistWaitGameView = false;
        //TweenManager.Instance.HomeViewAnimationShow(false);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUpdateCreateRoomSpr);
    }
}
