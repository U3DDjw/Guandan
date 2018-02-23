using DG.Tweening;
using DNotificationCenterManager;
using HaoyunFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZhiWa;
/// <summary>
/// 金币场 战斗界面 上半部分
/// </summary>
public class UIGoldPlayingInfoView : BasesView
{
    #region  ------------------字段定义
    [SerializeField]
    GameObject gmLeftInfoContainer;
    [SerializeField]
    GameObject btnLeftInfoIcon;
    [SerializeField]
    GameObject btnExit;
    [SerializeField]
    Text textSelfIndex;
    [SerializeField]
    Text textEnemyIndex;
    [SerializeField]
    Text textNumInfo;//当前局数信息
    [SerializeField]
    Text textRoomInfo;//房间信息

    //不可见字段
    Sequence mySeque = null;
    bool IsShowLeftInfo = false;
    #endregion

    private void Awake()
    {
        AddNotification();
    }
    private void OnDestroy()
    {
        RemoveNotification();
    }
    private void AddNotification()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshData);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldRefreshStartGame, RefreshData);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGoldOverBureau, RefreshData); //单局结算结束
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连

    }
    private void RemoveNotification()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshData);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldRefreshStartGame, RefreshData);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGoldOverBureau, RefreshData); //单局结算结束
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, RefreshAllData);//断线重连
    }



    void RefreshAllData(LocalNotification e)
    {
        InitData();
    }


    private void Start()
    {
        InitEventListener();
        InitTween();
        InitData();
    }
    private void InitEventListener()
    {
        EventTriggerListener.Get(btnLeftInfoIcon).onClick = OnLeftInfoClick;
        EventTriggerListener.Get(btnExit).onClick = OnExitClick;
    }
    private void InitTween()
    {
        mySeque = DOTween.Sequence();
        mySeque.Append(gmLeftInfoContainer.transform.DOLocalMove(gmLeftInfoContainer.transform.localPosition + new Vector3(250, 0, 0), 0.5f));
        mySeque.SetAutoKill(false);
        mySeque.Pause();
    }
    private void InitData()
    {
        textRoomInfo.text = string.Format("房间号:{0}\n {1}场\n底分:{2}", RoomInfo.Instance.mRoomCode, GoldFiledManager.Instance.GetCurGroundType, GoldFiledManager.Instance.GetMinScore);

        textSelfIndex.text = PlayerInfo.Instance.mTeamSelfIndex;
        textEnemyIndex.text = PlayerInfo.Instance.mTeamEnemyIndex;

        uint gameCurIndexNum = PlayingGameInfo.Instance.mGameInfo.game_index;
        string gameCurIndextString = gameCurIndexNum == 0 ? 1.ToString() : gameCurIndexNum.ToString();
        if (RoomInfo.Instance.mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeZhuanDan || RoomInfo.Instance.mGuandanGameType == TGuanDanGameType.TGuanDanGameTypeGuanDan)
        {
            string gameTotalNum = RoomInfo.Instance.mRoom.game_num.ToString();
            textNumInfo.text = string.Format("{0}/{1}局", gameCurIndextString, gameTotalNum);
        }
        else
        {
            textNumInfo.text = string.Format("第{0}局", gameCurIndextString);
        }
    }
    private void OnLeftInfoClick(GameObject g)
    {
        SpriteState spriteState = g.GetComponent<Button>().spriteState;
        string sprName = "";
        if (!IsShowLeftInfo)
        {
            mySeque.PlayForward();
            g.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "pk_btn_foward2");
            sprName = "pk_btn_foward2" + "_click";
        }
        else
        {
            mySeque.PlayBackwards();
            g.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "pk_btn_foward1");
            sprName = "pk_btn_foward1" + "_click";
        }
        spriteState = UIManagers.Instance.SetButtonStateSpr(EAtlasType.EPlaying, sprName);
        IsShowLeftInfo = !IsShowLeftInfo;
    }
    private void OnExitClick(GameObject btn)
    {
        UIManagers.Instance.ShowConfirmBox("退出后将扣除您10倍底分，确定逃跑吗？", "确认", "取消", SureGoldFiledExit, null);
    }
    void SureGoldFiledExit()
    {
        GoldFiledManager.Instance.SendExitGoldFiledServer();
    }
    #region ------------------刷新与重连
    private void RefreshData(LocalNotification info)
    {
        InitData();
    }
    //private void ReconnectData(LocalNotification info)
    //{
    //    InitData();
    //}
    #endregion 
}
