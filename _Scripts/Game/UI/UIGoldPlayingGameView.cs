using DNotificationCenterManager;
using HaoyunFramework;
using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPlayingGameContext:BaseContext
{
    public GoldPlayingGameContext()
    {
        ViewType = UIType.GoldPlayingGameView;
    }
}

public class UIGoldPlayingGameView : BasesView {
    List<UIType> uiList;

    private void Awake()
    {
        CheckIsReconnect();

        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldOutSuc, GoldOutSuc); //金币场退出成功 
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldNotEnough, UpdateGoldNotEnough);
    }


    void CheckIsReconnect()
    {
        Debug.Log("PlayingGameInfo.Instance.IsConnectPlayingStatus" + PlayingGameInfo.Instance.IsConnectPlayingStatus);
        if (PlayingGameInfo.Instance.IsConnectPlayingStatus)
        {
            PlayingGameInfo.Instance.SendReconnectServer();
        }
    }


     void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldOutSuc, GoldOutSuc); //金币场退出成功 
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldNotEnough, UpdateGoldNotEnough);
    }

    void UpdateGoldNotEnough(LocalNotification e)
    {
        ContextManager.Instance.Push(new ShopContext());
    }


    void GoldOutSuc(LocalNotification e)
    {
        ArgsPlayerId args = e.param as ArgsPlayerId;

        if (args != null)
        {
            var playerId = args.playerId;
            if (playerId == PlayerInfo.Instance.mPlayerPid) //直接退出房间，回到大厅
            {
                AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameOtherLeaveRoom);

                UIManagers.Instance.EnqueueTip("退出房间成功");
                GameManager.Instance.ClearCurGame();
                ContextManager.Instance.Pop(curContext);
            }
            else  //退出战斗，回到等待房间
            {
                string strTip = string.Format("玩家{0}退出房间",playerId);
                UIManagers.Instance.EnqueueTip(strTip);
                GameManager.Instance.ClearCurGame();
                ContextManager.Instance.Pop(curContext);
                GoldFiledManager.Instance.ReSendEnterGoldFiledServer();
            }
        }
    }


    // Use this for initialization
    void Start () {
        InitView();
        LoadView();
    }

	void InitView()
    {
		uiList = new List<UIType>() {  UIType.GoldPlayingSelfView, UIType.GoldPlayingPlayerView,UIType.GoldPlayingStatusView, UIType.GoldPlayingInfoView };
    //    uiList = new List<UIType>() { UIType.GoldPlayingStatusView, UIType.GoldPlayingSelfView, UIType.GoldPlayingPlayerView, UIType.GoldPlayingInfoView };
    }

    void LoadView()
    {
        for(int i =0;i< uiList.Count;i++)
        {
            GameObject gameInfoView = UIManagers.Instance.GetSingleUI(uiList[i]);
			gameInfoView.transform.SetParent(this.transform);
            gameInfoView.transform.SetAsLastSibling();
        }
    }

    public void ClosePlayingView()
    {
        ContextManager.Instance.Pop(this.curContext);
    }

}
