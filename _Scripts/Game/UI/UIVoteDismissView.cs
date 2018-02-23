using UnityEngine;
using System.Collections;
using DNotificationCenterManager;
using MsgContainer;
using System.Collections.Generic;
using Net;
using ZhiWa;
using Haoyun.Utils;
using System;
using UnityEngine.UI;
using HaoyunFramework;
public class VoteDismissViewContext : BaseContext
{
    public VoteDismissViewContext()
    {
        ViewType = UIType.VoteDismissView;
    }
}
public class UIVoteDismissView : BasesView
{
    [SerializeField]
    GameObject yesBtn;
    [SerializeField]
    GameObject noBtn;

    [SerializeField]
    Transform parnetItemContainer;

    [SerializeField]
    Text timer;
    [SerializeField]
    Text tipLab;
    Dictionary<ulong, UIVoteDismissItem> curPlayerDic = new Dictionary<ulong, UIVoteDismissItem>();
    bool isHadSelected = false;
    float usedTime = 0;
    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ESingleDissRoomApplyResult, UpdatePlayerResult);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDissmisRoomResultFail, UpdateResultFail);
        //    NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDissmisRoomResultSuc, UpdateResultSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReconnectRefresh, UpdateResultFail);

    }
    //防止切后台
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            usedTime = PlayerInfo.Instance.PlayerPrefsTime();
        else
            PlayerPrefsSave();
        Debug.Log("后台切回使用时间:" + usedTime);
    }
    private void OnDestroy()
    {
        if (!isHadSelected)
            PlayerPrefsSave();
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ESingleDissRoomApplyResult, UpdatePlayerResult);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDissmisRoomResultFail, UpdateResultFail);
        //  NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDissmisRoomResultSuc, UpdateResultSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReconnectRefresh, UpdateResultFail);
    }
    private void PlayerPrefsSave()
    {
        XPlayerPrefs.Instance.mKeyRecordTimeType = 1;
        XPlayerPrefs.Instance.mKey_RoomCode = (int)RoomInfo.Instance.mRoomCode;
        XPlayerPrefs.Instance.mKey_ExitTime = DateTime.Now.ToBinary().ToString();
        XPlayerPrefs.Instance.mkey_ResidueTime = GlobalData.mSameIpTotalTime - usedTime;
        PlayerPrefs.Save();
    }
    private List<MsgGameOverInfo> CreateEndInfo()
    {
        List<MsgGameOverInfo> mGameOverInfoList = new List<MsgGameOverInfo>();
        ArgsMsgRoomInfo roominfo = RoomInfo.Instance.mRoom;
        MsgGameOverInfo gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = PlayerInfo.Instance.mPlayerPid;//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = RoomInfo.Instance.GetIdByName("left");//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = RoomInfo.Instance.GetIdByName("right");//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);

        gameinfo = new MsgGameOverInfo();
        gameinfo.player_id = RoomInfo.Instance.GetIdByName("top");//只用到了id
        gameinfo.upgrade_card = 2;//打到几
        mGameOverInfoList.Add(gameinfo);
        return mGameOverInfoList;
    }
    void UpdateResultFail(LocalNotification e)
    {
        Debug.Log("解散结果---失败");
        isHadSelected = true;
        XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();
        ContextManager.Instance.Pop(curContext);
    }

    void UpdatePlayerResult(LocalNotification e)
    {
        ArgsDismissRoomApplyResult msg = e.param as ArgsDismissRoomApplyResult;
        if (msg.playerId == PlayerInfo.Instance.mPlayerPid)
            HadSelectedStatus();
        //  msg.isAgree
        UpdateItemData(msg.playerId, msg.isAgree ? EDissmissRoomResult.EAgree : EDissmissRoomResult.EDisagree);
    }

    // Use this for initialization  
    void Start()
    {
        EventTriggerListener.Get(yesBtn).onClick = OnYesClick;
        EventTriggerListener.Get(noBtn).onClick = OnNoClick;
        RunAutoEnd(PlayerInfo.Instance.PlayerPrefsTime());//init中重连会调用一次
    }

    void RunAutoEnd(float voteHasUsedTime)
    {
        usedTime = voteHasUsedTime;
        Debug.Log("重连后时间:" + usedTime);
        StartCoroutine(CauculateTime());
    }

    IEnumerator CauculateTime()
    {
        while (usedTime < GlobalData.mDismissRoomAutoResultTime)
        {
            //  Debug.Log(curTime);
            timer.text = ((int)(GlobalData.mDismissRoomAutoResultTime - usedTime)).ToString() + "秒后自动同意";
            usedTime += Time.deltaTime;
            yield return null;
        }
        if (!isHadSelected)
        {
            OnYesClick(null);
        }
    }
    void HadSelectedStatus()
    {
        yesBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "btn_blue_black");
        yesBtn.GetComponent<Image>().raycastTarget = false;
        yesBtn.transform.GetChild(0).GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "disbanded_font2_black");
        noBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "btn_blue_black");
        noBtn.GetComponent<Image>().raycastTarget = false;
        noBtn.transform.GetChild(0).GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "disbanded_font1_black");

        yesBtn.GetComponent<Button>().enabled = false;
        noBtn.GetComponent<Button>().enabled = false;
        isHadSelected = true;
        //timer.enabled = false;
    }

    public void InitData(ulong playerId, List<ulong> selectedPlayerList/*, string time*/)
    {
        if (parnetItemContainer.childCount == 4) { return; }

        GameObject voteItemPre = ResourceManager.Instance.LoadAsset<GameObject>(UIType.VoteDismissItem.Path);
        var playerInfo = PlayerInfo.Instance;
        tipLab.text = string.Format("<color=#18285BFF>{0}</color>请求解散房间", RoomInfo.Instance.GetPlayerInfoById(playerId).name);
        //是否是发起投票人
        if (playerId == playerInfo.mPlayerPid)
        {
            HadSelectedStatus();
            int idx = 0;
            //Self
            GameObject obj = GameObject.Instantiate(voteItemPre);
            obj.transform.SetParent(parnetItemContainer);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, 135, 0);
            obj.transform.GetComponent<UIVoteDismissItem>().InitData((uint)PlayerInfo.Instance.mPlayerPid, PlayerInfo.Instance.mPlayerData.name, EDissmissRoomResult.EAgree);

            idx++;
            //Others
            var dic = RoomInfo.Instance.mPlayersDic;
            foreach (var v in dic.Values)
            {
                obj = GameObject.Instantiate(voteItemPre);
                obj.transform.SetParent(parnetItemContainer);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(0, 135 - 90 * idx, 0);
                if (selectedPlayerList.Contains(v.player_id))
                {
                    obj.transform.GetComponent<UIVoteDismissItem>().InitData(v.player_id, v.name, EDissmissRoomResult.EAgree);
                }
                else
                {
                    obj.transform.GetComponent<UIVoteDismissItem>().InitData(v.player_id, v.name, EDissmissRoomResult.EUnSelect);
                }
                curPlayerDic.Add(v.player_id, obj.transform.GetComponent<UIVoteDismissItem>());
                idx++;
            }
        }
        else
        {
            int idx = 0;
            //Self
            GameObject obj = GameObject.Instantiate(voteItemPre);
            obj.transform.SetParent(parnetItemContainer);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, 135, 0);
            if (selectedPlayerList.Contains(PlayerInfo.Instance.mPlayerPid))
            {
                obj.transform.GetComponent<UIVoteDismissItem>().InitData((uint)PlayerInfo.Instance.mPlayerPid, PlayerInfo.Instance.mPlayerData.name, EDissmissRoomResult.EAgree);
                HadSelectedStatus();
            }
            else
            {
                obj.transform.GetComponent<UIVoteDismissItem>().InitData((uint)PlayerInfo.Instance.mPlayerPid, PlayerInfo.Instance.mPlayerData.name, EDissmissRoomResult.EUnSelect);
            }

            curPlayerDic.Add((uint)PlayerInfo.Instance.mPlayerPid, obj.transform.GetComponent<UIVoteDismissItem>());
            idx++;
            //Others
            var dic = RoomInfo.Instance.mPlayersDic;
            foreach (var v in dic.Values)
            {
                obj = GameObject.Instantiate(voteItemPre);
                obj.transform.SetParent(parnetItemContainer);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(0, 135 - 90 * idx, 0);
                if (selectedPlayerList.Contains(v.player_id))
                {
                    obj.transform.GetComponent<UIVoteDismissItem>().InitData(v.player_id, v.name, EDissmissRoomResult.EAgree);
                }
                else
                {
                    obj.transform.GetComponent<UIVoteDismissItem>().InitData(v.player_id, v.name, EDissmissRoomResult.EUnSelect);
                }

                curPlayerDic.Add(v.player_id, obj.transform.GetComponent<UIVoteDismissItem>());
                idx++;
            }
        }
    }

    void OnYesClick(GameObject g)
    {
        XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();
        AudioManager.Instance.PlayClickBtnAudio();
        PlayingGameInfo.Instance.SendDissRoomAgreeMsg();
    }

    void OnNoClick(GameObject g)
    {
        XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();
        AudioManager.Instance.PlayClickBtnAudio();
        PlayingGameInfo.Instance.SendDissRoomRefuseMsg();
    }

    public void UpdateItemData(ulong playerId, EDissmissRoomResult result)
    {
        curPlayerDic[playerId].UpdateData(result);
    }
}
