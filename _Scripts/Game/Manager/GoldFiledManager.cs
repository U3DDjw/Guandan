using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using ZhiWa;
using Net;
using MsgContainer;

public class GoldFiledManager : SingleTon<GoldFiledManager> {

    private bool isGoldFiled = false;
    /// <summary>
    /// 金币场状态
    /// </summary>
    public bool mIsGoldFiled
    {
        get
        {
            return isGoldFiled || RoomInfo.Instance.mIsGoildRoom;
        }
    }

    public void ChangeFiledStatus(bool isFiled)
    {
        isGoldFiled = isFiled;
    }


    private MsgGoldPlayground goldPlayerGround;


    /// <summary>
    /// 更新当前场景信息
    /// </summary>
    /// <param name="msg"></param>
    public void UpdateGoldPlayerGroundData(MsgGoldPlayground msg )
    {
        goldPlayerGround = msg;
    }


    public int GetBaseCost
    {
        get
        {
            return (int)goldPlayerGround.base_gold;
        }
    }

    /// <summary>
    /// 准备的玩家list
    /// </summary>
     List<ulong> readyPlayerId = new List<ulong>();


    public bool IsReadyPlayerContains(ulong player)
    {
        return readyPlayerId.Contains(player);
    }

    public void UpdateReadyPlayer(List<ulong> list)
    {
        readyPlayerId.Clear();
        if (list == null || list.Count == 0) { return; }

     
            for (int i = 0; i < list.Count; i++)
            {
            readyPlayerId.Add(list[i]);
            }
    }

    /// <summary>
    /// 是否开始游戏
    /// </summary>
    public bool mIsStartGame = false;
    /// <summary>
    /// 首先出牌玩家id
    /// </summary>
    public ulong mFirstPlayerId = 0;


    TGoldPlaygroundType curGroundType;
    uint curGroundCost;
    uint curGroundMinScore;

    void SaveCurMode(TGoldPlaygroundType type,uint min,uint cost)
    {
        curGroundType = type;
        curGroundMinScore = min;
        curGroundCost = cost;
    }


    public uint GetMinScore
    {
        get
        {
            return curGroundMinScore;
        }
    }


    public uint GetCostGold
    {
        get
        {
            if (goldPlayerGround != null)
            {
                return goldPlayerGround.cost;
            }
            return 0;
        }
    }

    public string GetCurGroundType
    {
        get
        {
            switch(curGroundType)
            {
                case TGoldPlaygroundType.TGoldPlaygroundType1:
                    return "初级";
                case TGoldPlaygroundType.TGoldPlaygroundType2:
                    return "进阶";
                case TGoldPlaygroundType.TGoldPlaygroundType3:
                    return "高级";
                case TGoldPlaygroundType.TGoldPlaygroundType4:
                    return "大师";
            }
            return "初级";
        }
    }


    public void SendEnterGoldFiledServer(int groundType,uint cost,uint min)
    {
        SaveCurMode((TGoldPlaygroundType)groundType,cost,min);

        MsgGlobal mGl = new MsgGlobal();
        mGl.gold_playground = new MsgGoldPlayground();
        MsgGoldPlayground msg = mGl.gold_playground;
        msg.gold_playground_type = (TGoldPlaygroundType)groundType;
        msg.cost = cost;
        msg.min = (uint)min;
        msg.type = (uint)EJoinRoomType.EEnter;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_ENTER_GOLD_FILED, mGl);
    }


    public void ReSendEnterGoldFiledServer()
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.gold_playground = new MsgGoldPlayground();
        MsgGoldPlayground msg = mGl.gold_playground;
        msg.gold_playground_type = (TGoldPlaygroundType)curGroundType;
        msg.cost = curGroundCost;
        msg.min = curGroundMinScore;
        msg.type = (uint)EJoinRoomType.EEnter;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_ENTER_GOLD_FILED, mGl);
    }


    public void SendExchangeRoomServer()
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.gold_playground = new MsgGoldPlayground();
        MsgGoldPlayground msg = mGl.gold_playground;
        msg.gold_playground_type = curGroundType;
        msg.cost = curGroundCost;
        msg.min = curGroundMinScore;
        msg.type = (uint)EJoinRoomType.EExchange;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_GOLD_EXCHANGE_ROOM, mGl);
    }

    public void SendExitGoldFiledServer()
    {
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_GOLD_QUIT_ROOM);
    }

    public void SendReadyGoldStartGameServer()
    {
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_READY_GOLD);
    }

    //--------------------玩家位置对应的 GameObject
    private Dictionary<EPlayerPositionType, GameObject> dicPlayerPosGm = new Dictionary<EPlayerPositionType, GameObject>();
    public Dictionary<EPlayerPositionType, GameObject> mDicPlayerPosGm
    {
        get
        {
            return dicPlayerPosGm;
        }
    }
    public void AddPlayerPosGMToDic(EPlayerPositionType pos, GameObject posGm)
    {
        if (dicPlayerPosGm.ContainsKey(pos))
        {
            mDicPlayerPosGm[pos] = posGm;
        }
        else
        {
            mDicPlayerPosGm.Add(pos, posGm);
        }
    }
    public GameObject GetPlayerPosGameobject(EPlayerPositionType playerPosType)
    {
        foreach (var v in dicPlayerPosGm)
        {
            if (v.Key == playerPosType)
            {
                return v.Value;
            }
        }
        return null;
    }
}
