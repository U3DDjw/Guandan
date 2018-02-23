using UnityEngine;
using System.Collections;
using MsgContainer;
using System.Collections.Generic;
using Net;
using DNotificationCenterManager;
using System.IO;
using ZhiWa;
using HaoyunFramework;

public class HomeModule : BaseModule
{

    public HomeModule()
    {
         mIsLoadByMiddleSence = true;
    }

    public override void LoadModule()
    {
        base.LoadModule();
        //这边干点自己的事
    }

    public override void AddServerCallBack()
    {
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_FKBZ, RoomKFBZ);
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_KF_SUC, RoomCreateSuccess); //创建房间成功
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_PLAYER_OUT, RoomGDPlayerOut);//非房主离开游戏
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAG_ROOM_LEAVE, RoomCreaterOut); //房主离开游戏
        //进入房间
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_JF_FAIL_FKBZ, RoomJoinFKBZ); //房卡不足
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_JF_FAIL_RSYM, RoomJoinFAIL); //房间不存在
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_NO_ROOM, RoomJoinFull); //人数已满



        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_NEW_PLAYER, RoomJoinNewPlayer); //新玩家进入房间

        //    NetManager.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_SAME_IP, SameIp); //IP冲突
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_SQDHZW_SUC, RecevSQDHZWSuc); //调换申请成功
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_SQDHZW_FAIL, RecevSQDHZWFail); //调换申请失败


        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_DHZW_SUC, RecevDHZWSuc); //调换座位成功
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_DHZW_JJ, RecevDHZWFail); //拒绝调换座位
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_IP_CONFIRME, RecevIpConfirm);//Ip冲突 收到确认回调

        //金币场
     //   TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_GOLD_NOT_ENOUGH, RecevEnterGoldNotEnough);//加入金币场，金币不足 
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_GOLD_ENTER_SUC, RecevGoldEnterSuc);//加入金币场成功 
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_GD_JF_SUC, RoomJoinSUC); //进入房间成功

    }


    void RoomJoinSUC(MsgGlobal msg)
    {

        SDKManager.Instance.shareRoomCode = PlayerInfo.Instance.mPlayerData.roomCode;
        SDKManager.Instance.isWechatCallback = 0;
        //思想：Module层控制数据，View层纯显示
        RoomInfo.Instance.mRoomId = msg.room_id;
        MsgGuandanRoom data = msg.guandan_room;

        RoomInfo.Instance.GetRoomInfo(data.room_info);
        PlayerInfo.Instance.mPlayerData.roomCode = data.room_info.roomCode;
        RoomInfo.Instance.UpdatePlayersDic(data.player_info);
        RoomInfo.Instance.mSameIpArgs.list.Clear();
        if (data.same_ip_player != null && data.same_ip_player.Count == 4)
        {
            for (int i = 0; i < data.same_ip_player.Count; i++)
            {
                RoomInfo.Instance.mSameIpArgs.list.Add(data.same_ip_player[i]);
            }
            Debug.Log("IP 冲突" + data.same_ip_player.Count + "人数");
        }
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ERoomJoin_Suc);
    }


    //void RecevEnterGoldNotEnough(MsgGlobal msg)
    //{
    //    Debug.Log("金币不够使用道具");
    //    UIManagers.Instance.EnqueueTip("金币不足");
    //    NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldNotEnough);
    //}

    void RecevGoldEnterSuc(MsgGlobal msg)
    {
        Debug.Log("RecevEnterSuc");


        //思想：Module层控制数据，View层纯显示
        GameManager.Instance.ClearCurGame();

        MsgGuandanRoom data = msg.guandan_room;
     
        if(data.gold_ready_player!=null)
        {
            var playerReadyList = data.gold_ready_player.pid;
            if(playerReadyList!=null)
            {
                GoldFiledManager.Instance.UpdateReadyPlayer(playerReadyList);
            }
        }

        if(data.gold_playground.type ==(uint)EJoinRoomType.EEnter)
        {
            UIManagers.Instance.EnqueueTip("加入房间成功");
        }
        else
        {
            UIManagers.Instance.EnqueueTip("加入房间成功");
        }

        GoldFiledManager.Instance.UpdateGoldPlayerGroundData(msg.guandan_room.gold_playground);
        RoomInfo.Instance.GetRoomInfo(data.room_info);
        PlayerInfo.Instance.mPlayerData.roomCode = data.room_info.roomCode;
        RoomInfo.Instance.UpdatePlayersDic(data.player_info);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ERoomJoin_Suc);
   
    }


    void RecevIpConfirm(MsgGlobal msg)
    {
        MsgIpConfirm data = msg.ip_confirm;
        ArgsIpConfirmPlayer args = new ArgsIpConfirmPlayer();
        Debug.Log("IpConFirm" + args.playerId);
        args.playerId = (uint)data.player_id;
        args.mPlayerIds = data.pids;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EIpConfirmNotify, args);


    }

    void RecevSQDHZWSuc(MsgGlobal msg)
    {

        MsgChangeSeatInfo data = msg.change_seat_info;
        ArgsChangeSeatInfo args = new ArgsChangeSeatInfo();
        args.applyId = data.apply_player_id;
        args.toId = data.to_player_id;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EApplyChangeSeat, args);
    }


    void RecevSQDHZWFail(MsgGlobal msg)
    {
        UIManagers.Instance.EnqueueTip("申请换座位失败");
    }


    void RecevDHZWSuc(MsgGlobal msg)
    {
        MsgChangeSeatInfo data = msg.change_seat_info;
        string str = string.Format("applyId:{0},toId:{1}", data.apply_player_id, data.to_player_id);
        Debug.Log(str);
        ArgsChangeSeatInfo args = new ArgsChangeSeatInfo();
        args.applyId = data.apply_player_id;
        args.toId = data.to_player_id;
        args.result = 0;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EAgreeChangeSeat, args);
    }

    void RecevDHZWFail(MsgGlobal msg)
    {
        MsgChangeSeatInfo data = msg.change_seat_info;
        string str = string.Format("applyId:{0},toId:{1}", data.apply_player_id, data.to_player_id);
        Debug.Log(str);
        ArgsChangeSeatInfo args = new ArgsChangeSeatInfo();
        args.applyId = data.apply_player_id;
        args.toId = data.to_player_id;
        args.result = 1;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EAgreeChangeSeat, args);
    }

    void RoomJoinFull(MsgGlobal msg)
    {
        Debug.Log("房间不存在");
        ArgsJoinRoomReason args = new ArgsJoinRoomReason();
        args.reason = EJoinRoomFailReason.EROOM_NULL;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EJoinRoomFail, args);

    }
    void RoomJoinFKBZ(MsgGlobal msg)
    {
        ArgsJoinRoomReason args = new ArgsJoinRoomReason();
        args.reason = EJoinRoomFailReason.EFKBZ;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EJoinRoomFail, args);
    }

    void RoomJoinFAIL(MsgGlobal msg)
    {
        ArgsJoinRoomReason args = new ArgsJoinRoomReason();
        args.reason = EJoinRoomFailReason.ERSYM;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EJoinRoomFail, args);
    }

    

    void RoomJoinNewPlayer(MsgGlobal msg)
    {
        MsgGuandanRoom data = msg.guandan_room;

        //Test============================================
        string str = "=====";
        for (int i = 0; i < data.player_info.Count; i++)
        {
            str += data.player_info[i].name + "&&&";
        }
        Debug.Log(str);
        //Test============================================
        RoomInfo.Instance.UpdatePlayersDic(data.player_info);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ERoomJoin_other_join);

    
        if(GoldFiledManager.Instance.mIsGoldFiled) //金币场不作准备
        {
            return;
        }

        if (data.same_ip_player != null && data.same_ip_player.Count == 4)
        {
            ArgsSameIpPlayer args = new ArgsSameIpPlayer();
            for (int i = 0; i < data.same_ip_player.Count; i++)
            {
                args.list.Add(data.same_ip_player[i]);
            }
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESameIpPlayer, args);
        }
        else if (RoomInfo.Instance.GetRealPlayerCount() == 3)
        {
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ESendCanReadyGame);
        }
  

      
    }

    private void RoomKFBZ(MsgGlobal msg)
    {
        Debug.Log("房卡不足");
        //MemoryStream ms = new MemoryStream(msg.ReadBytes());
        //MsgRoomInfo msg_data_head = Game.Serial.Deserialize(ms, null, typeof(MsgRoomInfo)) as MsgRoomInfo;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGUANDAN_ROOM_GD_FKBZ);
    }

    void RoomCreaterOut(MsgGlobal msg)
    {
        if (!RoomInfo.Instance.IsCreater())
            UIManagers.Instance.EnqueueTip("房主解散房间");

        DismissRoom();
    }

    void DismissRoom()
    {
        AudioManager.Instance.IsPlayBackgroundAudio(true);
        RoomInfo.Instance.ClearInfo();
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGUANDAG_ROOM_LEAVE);
    }
    void RoomGDPlayerOut(MsgGlobal msg)
    {
        //接收一个pid，去掉这个pid即可、
        uint pid = (uint)msg.actionId;
        if (pid == PlayerInfo.Instance.mPlayerPid) //解散自己
        {
            DismissRoom();
        }
        else
        {
            RoomInfo.Instance.RemovePlayer(pid);
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ERoom_other_leave);
        }
    }
    private void RoomCreateSuccess(MsgGlobal msg)
    {
        RoomInfo.Instance.mRoomId = msg.room_id;
        SDKManager.Instance.shareRoomCode = PlayerInfo.Instance.mPlayerData.roomCode;
        Debug.Log("开房成功");
        MsgRoomInfo data = msg.room_info;
        Debug.Log("RoomCode: " + RoomInfo.Instance.mRoomCode.ToString());
        RoomInfo.Instance.GetRoomInfo(data);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGUANDAN_ROOM_GD_KF_SUC);
    }
}
