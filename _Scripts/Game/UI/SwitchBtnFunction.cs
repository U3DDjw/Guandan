using UnityEngine;
using System.Collections;
using DG.Tweening;
using Net;
using MsgContainer;
using DNotificationCenterManager;
using HaoyunFramework;
using System.Collections.Generic;

public class SwitchBtnFunction : BasesView
{
    Sequence mySequence = null;
    float tweenTime = 0.35f;

    [SerializeField]
    GameObject exitBtn;
    [SerializeField]
    GameObject settingBtn;
    [SerializeField]
    GameObject reBtn;
    public bool IsBackMode
    {
        get; set;
    }

    private void Awake()
    {
        //收到解散消息，清空该obj，实例化出投票页面
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDissmisRoomApply, UpdateDissmgsRoomApply);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDissmisRoomApply, UpdateDissmgsRoomApply);
    }

    void UpdateDissmgsRoomApply(LocalNotification e)
    {
        ArgsDissmissRoomApplyer args = e.param as ArgsDissmissRoomApplyer;
        ulong pId = args.playerId;
        List<ulong> playerList = new List<ulong>();
        playerList.Add(pId);
        ContextManager.Instance.Push(new VoteDismissViewContext());
        UIManagers.Instance.GetSingleUI(UIType.VoteDismissView).GetComponent<UIVoteDismissView>().InitData(pId, playerList/*, args.voteTime*/);
    }



    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(exitBtn).onClick = OnExitClick;
        EventTriggerListener.Get(settingBtn).onClick = OnSettingClick;
        EventTriggerListener.Get(reBtn.gameObject).onClick = OnReClick;
        reBtn.transform.GetChild(0).gameObject.SetActive(SelfCardsManager.Instance.IsPlaySingleOrMulity);
    }
    void OnSettingClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Push(new SettingContext());
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="g"></param>
    void OnReClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        SelfCardsManager.Instance.IsPlaySingleOrMulity = !SelfCardsManager.Instance.IsPlaySingleOrMulity;
        reBtn.transform.GetChild(0).gameObject.SetActive(SelfCardsManager.Instance.IsPlaySingleOrMulity);
    }
    public void OnExitClick(GameObject gm)
    {
        if (IsBackMode)
        {
            UIManagers.Instance.DestroySingleUI(UIType.PlayBackView);
            return;
        }
        AudioManager.Instance.PlayClickBtnAudio();
        if (!GoldFiledManager.Instance.mIsGoldFiled)
        {
            if (GameManager.Instance.mCurGameStatus == EGameStatus.EPlaying) //战斗状态下的退出是申请解散功能
            {
                UIManagers.Instance.ShowConfirmBox("确认发起解房间吗？", "确认", "取消", sureDissmissRoomClickEvent, null);

            }
            else //Home
            {
                RoomInfo.Instance.LeaveWaitRoom();
            }
        }
        else
        {
        UIManagers.Instance.ShowConfirmBox("确定离开房间吗？", "确认", "取消", SureGoldFiledExit, null);
        }
    }


    void SureGoldFiledExit()
    {
        GoldFiledManager.Instance.SendExitGoldFiledServer();
    }

    bool isStartDiss = false;
    void sureDissmissRoomClickEvent()
    {
        if (isStartDiss)
        {
            UIManagers.Instance.EnqueueTip(string.Format("还有{0}秒，才能发起解散", (int)(GlobalData.mDismissRoomCDTime - curTime)));
            return;
        }
        PlayingGameInfo.Instance.SendDissRoomApplyMsg();
        isStartDiss = true;
    }

    float curTime = 0;
    private void Update()
    {
        if (isStartDiss)
        {
            curTime += Time.deltaTime;
            if (curTime > GlobalData.mDismissRoomCDTime)
            {
                isStartDiss = false;
                curTime = 0;
            }
        }

    }

}
