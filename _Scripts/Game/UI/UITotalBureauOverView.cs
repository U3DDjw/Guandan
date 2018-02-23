using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZhiWa;
using System;
using MsgContainer;
using DNotificationCenterManager;
using HaoyunFramework;
using UnityEngine.UI;

public class TotalBureauOverViewContext : BaseContext
{
    public TotalBureauOverViewContext()
    {
        ViewType = UIType.TotalBureauOverView;
    }
}
public class UITotalBureauOverView : BasesView
{

    [SerializeField]
    GameObject escapeBtn;
    [SerializeField]
    GameObject deseBtn;
    [SerializeField]
    Transform itemParent;
    [SerializeField]
    GameObject closeBtn;
    //[SerializeField]
    //Image lightTex;
    //[SerializeField]
    //Image titleBgSprite;

    [SerializeField]
    Text nowTimeLab;

    [SerializeField]
    Text roomCodeLab;
    //[SerializeField]
    //Texture textureLose, textureWin;
    //[SerializeField]
    //GameObject starContainer;

    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
    }

    public void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
    }

    void UpdateJoinRoomSuc(LocalNotification e)
    {
        ContextManager.Instance.Pop(curContext);
    }

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(escapeBtn).onClick = OnEscapeClick;
        EventTriggerListener.Get(deseBtn).onClick = OnDeseClick;
        EventTriggerListener.Get(closeBtn).onClick = OnEscapeClick;
        ShowRoomInfo();
    }
    void OnDeseClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        SDKManager.Instance.WeChatShareImage();
    }
    void OnEscapeClick(GameObject g)
    {
        Debug.Log("点击了关闭当前界面");
        ContextManager.Instance.Pop(curContext);
        GameManager.Instance.BackToHomeModule();
    }
    EPlayersTeam winTeam = EPlayersTeam.ENull;
    public void SetData(LocalNotification e)
    {

        if (itemParent.childCount == 4) { return; } //屏蔽之后传过来的消息
        bool IsWin = false;
        //打二
        if (RoomInfo.Instance.IsZhuandanGameType || RoomInfo.Instance.IsGuandan2GameType)
        {
            ArgsMsgTotalScore args = e.param as ArgsMsgTotalScore;
            var list = args.List;
            Debug.Log("TotalBureauView：" + args.List.Count);
            GameObject totalPrefabItem = ResourceManager.Instance.LoadAsset<GameObject>(UIType.TotalBureauOverItem.Path);
            for (int i = 0; i < list.Count; i++)
            {
                GameObject item = GameObject.Instantiate(totalPrefabItem);
                item.transform.SetParent(itemParent);
                //item.transform.localPosition = new Vector3(0, 225 - 130 * i, 0);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                item.GetComponent<UITotalBureauOverItem>().SetData(list[i].player_id, list[i].play_goal);
                if (list[i].player_id == PlayerInfo.Instance.mPlayerPid)
                {
                    //图片的显示，和输赢有关系
                    IsWin = list[i].play_goal >= 0;
                }
            }
        }
        else //非打二
        {
            ArgsMsgGameOverMsg args = e.param as ArgsMsgGameOverMsg;
            var list = args.mGameOverList;
            bool isTie = PlayerInfo.Instance.mTeamEnemyIndex == PlayerInfo.Instance.mTeamSelfIndex;//平局
            GameObject totalPrefabItem = ResourceManager.Instance.LoadAsset<GameObject>(UIType.TotalBureauOverItem.Path);
            for (int i = 0; i < list.Count; i++)
            {
                GameObject item = GameObject.Instantiate(totalPrefabItem);
                item.transform.SetParent(itemParent);
                //item.transform.localPosition = new Vector3(0, 225 - 130 * i, 0);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                EPlayersTeam team = (EPlayersTeam)RoomInfo.Instance.GetPlayerInfoById(list[i].player_id).teamType;
                if (winTeam == EPlayersTeam.ENull)
                    winTeam = list[i].rank == MsgGuandanGameRank.MsgGuandanGameRankFirst ? team : EPlayersTeam.ENull;
                if (isTie) { winTeam = team; }
                item.GetComponent<UITotalBureauOverItem>().SetData(list[i], winTeam);

                if (list[i].player_id == PlayerInfo.Instance.mPlayerPid)
                {
                    IsWin = team == winTeam;//平局
                }
                if (isTie) { IsWin = true; }
            }
        }
        CheckPlayerAudio(IsWin);
    }
    void CheckPlayerAudio(bool isWin)
    {
        AudioManager.Instance.PlayCommonAudio(isWin ? GlobalData.AudioNameTotalGameOverWin : GlobalData.AudioNameTotalGameOverLose);
    }
    public void SetDissData(ArgsMsgGameOverMsg args)
    {
        bool IsWin = false;
        //打二
        if (RoomInfo.Instance.IsZhuandanGameType || RoomInfo.Instance.IsGuandan2GameType)
        {
        }
        else //非打二
        {
            var list = args.mGameOverList;
            GameObject totalPrefabItem = ResourceManager.Instance.LoadAsset<GameObject>(UIType.TotalBureauOverItem.Path);
            bool isTie = PlayerInfo.Instance.mTeamEnemyIndex == PlayerInfo.Instance.mTeamSelfIndex;//平局
            for (int i = 0; i < list.Count; i++)
            {
                GameObject item = GameObject.Instantiate(totalPrefabItem);
                item.transform.SetParent(itemParent);
                //item.transform.localPosition = new Vector3(0, 60 - 55 * i, 0);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                EPlayersTeam team = (EPlayersTeam)RoomInfo.Instance.GetPlayerInfoById(list[i].player_id).teamType;
                if (winTeam == EPlayersTeam.ENull)
                    winTeam = list[i].rank == MsgGuandanGameRank.MsgGuandanGameRankFirst ? team : EPlayersTeam.ENull;

                item.GetComponent<UITotalBureauOverItem>().SetData(list[i], winTeam);

                if (list[i].player_id == PlayerInfo.Instance.mPlayerPid)
                {
                    IsWin = team == winTeam;
                }
            }
        }
    }
    void ShowRoomInfo()
    {
        //DateTime.Now.ToString("yyyy年MM月dd日"); yyyy年MM月dd日
        //DateTime.Now.ToString("HH:mm"); 19:48
        nowTimeLab.text = DateTime.Now.ToString("yyyy年MM月dd日") + DateTime.Now.ToString("HH:mm");
        string gameType = "转蛋", payType = "AA支付";
        string endInfo = string.Format("共{0}把", RoomInfo.Instance.mRoom.game_num);
        if (!RoomInfo.Instance.IsZhuandanGameType) gameType = "掼蛋";
        if (RoomInfo.Instance.IsCreaterPay()) payType = "房主支付";
        if (GuanDanEndNum() != null) endInfo = GuanDanEndNum();
        //房间Id:678356 共8把 掼蛋 AA支付
        roomCodeLab.text = string.Format("房间ID:{0}  {1}  {2}  {3}", RoomInfo.Instance.mRoomCode.ToString(), endInfo, gameType, payType);
    }

    string GuanDanEndNum()
    {

        if (RoomInfo.Instance.mRoom.game_type == TGuanDanGameType.TGuanDanGameTypeGuanDan6)
        {
            return "打到6";
        }
        else if (RoomInfo.Instance.mRoom.game_type == TGuanDanGameType.TGuanDanGameTypeGuanDan8)
        {
            return "打到8";
        }
        else if (RoomInfo.Instance.mRoom.game_type == TGuanDanGameType.TGuanDanGameTypeGuanDan10)
        {
            return "打到10";
        }
        else if (RoomInfo.Instance.mRoom.game_type == TGuanDanGameType.TGuanDanGameTypeGuanDanA)
        {
            return "打到A";
        }
        return null;
    }
}
