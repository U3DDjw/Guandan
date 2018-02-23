using UnityEngine;
using System.Collections;
using MsgContainer;
using System.Collections.Generic;
using Net;
using DNotificationCenterManager;
using System.IO;
using ZhiWa;
using Umeng;
using HaoyunFramework;
public class LoginModule : BaseModule
{

	public LoginModule()
    {
        mIsLoadByMiddleSence = false;
    }

    public override void LoadModule()
    {
        base.LoadModule();
        ContextManager.Instance.Push(new CommonContext());
        ContextManager.Instance.Push(new LoginContext());
    }
    public override void AddServerCallBack()
    {
        base.AddServerCallBack();
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_LOGIN_SUC, RevcLoginSuc);
        TCPNetWork.GetInstance().AddCallback(ServerMsgKey.GUANDAN_ROOM_FAIL, RevcLoginFial);
    }
    /// <summary>
    /// 登录成功，进入游戏界面
    /// </summary>
    /// <param name="msg"></param>
    private void RevcLoginSuc(MsgGlobal msg)
    {
        Debug.Log("UDP请求第一次登录成功");
        SDKManager.Instance.isWechatCallback = 0;
        GA.ProfileSignIn(PlayerInfo.Instance.mPlayerData.pid.ToString(), "wechat"); // 用户登录ID

        // TalkingData
        TDGAAccount tdAccount = TDGAAccount.SetAccount(PlayerInfo.Instance.mPlayerData.pid.ToString());
        tdAccount.SetAccountType(AccountType.TYPE1);
        tdAccount.SetAccountName(PlayerInfo.Instance.mPlayerData.name);
        if (PlayerInfo.Instance.mPlayerData.sex == 1)
        {
            tdAccount.SetGender(Gender.MALE);
        }
        else if (PlayerInfo.Instance.mPlayerData.sex == 2)
        {
            tdAccount.SetGender(Gender.FEMALE);
        }
        else
        {
            tdAccount.SetGender(Gender.UNKNOW);
        }

        MsgPlayerInfo data = msg.player_info;
        PlayerInfo.Instance.mPlayerData.ip = data.ip;

        PlayingGameInfo.Instance.mReconnectId = 3;
        if (RoomInfo.Instance.mRoomCode > 0) //断线重连
        {
           //UIManager.Instance.EnqueueTip("断线重连: " + RoomInfo.Instance.mRoomCode);
           if(data.roomCode ==1) //等待界面
            {
                PlayingGameInfo.Instance.mReconnectId = 1;
            }
           else if(data.roomCode ==2) //战斗
            {
                PlayingGameInfo.Instance.mReconnectId =2;
            }
        }
        SavePlayerToken();
        ContextManager.Instance.PopAll();
        GameManager.Instance.SwitchGameStatus(EGameStatus.EHome);

        if (RoomInfo.Instance.mRoomCode <= 0)
        {
            //Android 可以直接进入不需要判断
            //#if UNITY_IPHONE && !UNITY_EDITOR
            Debug.Log(">>>>>>使用剪切板房间号信息登录");
            SDKManager.Instance.GetClipRoomCodeEnterRoom();
            //#endif
        }

        if (GameManager.Instance.mGameMode != EGameMode.EDebug) {
            SDKManager.Instance.yayaLogin();// 呀呀语音登录
        }

		// 高德初始化
		bool isIosSj = GameManager.Instance.mGameMode == EGameMode.EAppleOnLine && PlayerInfo.Instance.mUrlData.iosSj;
		if (!isIosSj) {
			SDKManager.Instance.doOpenLocationService ();
		}
    }

    void SavePlayerToken()
    {
        if(GameManager.Instance.mGameMode == EGameMode.EDebug)
        {
            return;
        }
        XPlayerPrefs.Instance.mPlayerToken = PlayerInfo.Instance.mPlayerData.token;
    }
    /// <summary>
    /// 登录失败，继续请求
    /// </summary>
    /// <param name="msg"></param>
    private void RevcLoginFial(MsgGlobal msg)
    {
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ELoginFail);

        Debug.Log("UDP请求登录失败，继续请求");

    }

}
