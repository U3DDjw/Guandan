using UnityEngine;
using System.Collections;
using MsgContainer;
using System.Collections.Generic;
using DNotificationCenterManager;
using ZhiWa;
using UnityEngine.UI;
using HaoyunFramework;
using System;
using Haoyun.Utils;

public class SameIpViewContext : BaseContext
{
    public SameIpViewContext()
    {
        ViewType = UIType.SameIpView;
    }
}
//数据由哪里发过来的？
//数据需要有 玩家id 根据id获取信息：头像id,玩家名字，玩家id
public class UISameIpView : BasesView
{
    HandleMakeSureEvent continueClick;
    HandleMakeSureEvent leaveClick;
    [SerializeField]
    Image continueGamebtn;
    [SerializeField]
    Image leaveRoomBtn;
    [SerializeField]
    Text timeLab;
    [SerializeField]
    GameObject itemContainer;
    [SerializeField]
    Image maskBg;
    List<GameObject> listSameIpItem = new List<GameObject>();
    string[] ipColors = { "#D74E2F", "#375329", "#18285B" };
    int colorIndex = -1;
    float time = 0;
    bool isSaveTime = true;//退出重加相同的房间，其他人离开房间
    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGUANDAG_ROOM_LEAVE, UpdateLevelRoom);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoom_other_leave, UpdateLevelRoom);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EIpConfirmNotify, UpdatePlayerIpConfirm);
    }
    private void OnDestroy()
    {
        if (isSaveTime)
            PlayerPrefsSave();
        else
            //之前是每隔一秒保存一次，最后如果不需要保存的话就需要删除
            XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();

        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGUANDAG_ROOM_LEAVE, UpdateLevelRoom);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoom_other_leave, UpdateLevelRoom);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EIpConfirmNotify, UpdatePlayerIpConfirm);
    }
    //防止切后台
    private void OnApplicationFocus(bool focus)
    {
        if (!isSaveTime) return;
        if (focus)
            InitPlayerPrefsTime();
        else
            PlayerPrefsSave();
    }
    private void PlayerPrefsSave()
    {
        Debug.Log("保存时间");
        XPlayerPrefs.Instance.mKeyRecordTimeType = 0;
        XPlayerPrefs.Instance.mKey_RoomCode = (int)RoomInfo.Instance.mRoomCode;
        XPlayerPrefs.Instance.mKey_ExitTime = DateTime.Now.ToBinary().ToString();
        XPlayerPrefs.Instance.mkey_ResidueTime = GlobalData.mSameIpTotalTime - time;
        PlayerPrefs.Save();
    }
    void UpdateLevelRoom(LocalNotification e)
    {
        isSaveTime = false;
        ContextManager.Instance.Pop(this.curContext);
    }

    List<UISameIpItem> playerList = new List<UISameIpItem>();
    void UpdatePlayerIpConfirm(LocalNotification e)
    {
        ArgsIpConfirmPlayer args = e.param as ArgsIpConfirmPlayer;
        if (args != null)
        {
            RefreshConfirm(args.playerId, args.mPlayerIds);
        }
    }

    void RefreshAllItem(List<ulong> playerIds)
    {
        if (playerIds != null && playerIds.Count == 0)
        {
            return;
        }
        IEnumerator<UISameIpItem> iItems = playerList.GetEnumerator();
        while (iItems.MoveNext())
        {
            if (playerIds.Contains(iItems.Current.mPlayerId))
            {
                iItems.Current.RefreshConfirm(true);
            }
        }
    }

    void RefreshConfirm(uint playerId, List<ulong> playerIds)
    {
        RefreshAllItem(playerIds);
        IEnumerator<UISameIpItem> iItems = playerList.GetEnumerator();
        while (iItems.MoveNext())
        {
            if (iItems.Current.mPlayerId == playerId)
            {
                iItems.Current.RefreshConfirm(true);
                break;
            }
        }

        if (playerId == PlayerInfo.Instance.mPlayerPid)
        {
            //按钮失效
            ConfirmAfter();
        }

        if (IsAllConfirm())
        {
            if (this.continueClick != null)
            {
                this.continueClick.Invoke();
            }
            ContextManager.Instance.Pop(curContext);
        }

    }

    bool IsAllConfirm()
    {
        IEnumerator<UISameIpItem> iItems = playerList.GetEnumerator();
        while (iItems.MoveNext())
        {
            if (!iItems.Current.mIsConfirm)
            {
                return false;
            }
        }
        return true;
    }
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(continueGamebtn.gameObject).onClick = OnClickContinue;
        EventTriggerListener.Get(leaveRoomBtn.gameObject).onClick = OnClickLeave;
        maskBg.GetComponent<Image>().enabled = true;
        timeLab.text = "0";

        InitPlayerPrefsTime();
        StartCoroutine(CauculateTime());
    }
    void InitPlayerPrefsTime()
    {

        if (XPlayerPrefs.Instance.mKeyRecordTimeType != 0 || XPlayerPrefs.Instance.mKey_RoomCode == 0)
        {
            XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();//上一次
            return;
        }

        if (XPlayerPrefs.Instance.mKey_RoomCode == RoomInfo.Instance.mRoomCode)
        {
            long exitTimeBin;
            long.TryParse(XPlayerPrefs.Instance.mKey_ExitTime, out exitTimeBin);
            DateTime exitTime = System.DateTime.FromBinary(exitTimeBin);
            int totalSeconds = TimeUtils.GetDateTimeIntervalSeconds(exitTime, DateTime.Now);
            float hasSpendTime = GlobalData.mSameIpTotalTime - XPlayerPrefs.Instance.mkey_ResidueTime;
            time = totalSeconds + hasSpendTime;
        }

        if (time > GlobalData.mSameIpTotalTime)//同ip再加入同一个房间号大于 120 秒就归0 四个人退出并未考虑到
        {
            time = 0;
        }
        XPlayerPrefs.Instance.ClearSameIpTimeLinkPlayerPrefs();
    }
    IEnumerator CauculateTime()
    {
        while (true)
        {
            time += Time.deltaTime;
            timeLab.text = "有玩家IP相同\t" + ((int)(GlobalData.mSameIpTotalTime - time)).ToString() + "秒关闭提示";
            if (time > GlobalData.mSameIpTotalTime)
            {
                if (!isHadSelected)
                {
                    OnClickLeave(null);
                }
                break;
            }
            yield return null;
        }
    }

    public void SetInfo(LocalNotification args, HandleMakeSureEvent contin, HandleMakeSureEvent leave)
    {
        if (playerList.Count == 4) return;
        this.continueClick = contin;
        this.leaveClick = leave;
        ArgsSameIpPlayer msg = args.param as ArgsSameIpPlayer;
        var infoList = msg.list;
        GameObject sameIpItemPre = ResourceManager.Instance.LoadAsset<GameObject>(UIType.SameIpItem.Path);
        for (int i = 0; i < infoList.Count; i++)
        {
            var data = infoList[i];

            GameObject sameIpItem = GameObject.Instantiate(sameIpItemPre);
            sameIpItem.transform.SetParent(itemContainer.transform);
            //sameIpItem.transform.localPosition = new Vector3(0, -86 * i, 0);
            sameIpItem.transform.localScale = Vector3.one;
            sameIpItem.transform.localPosition = Vector3.zero;
            UISameIpItem item = sameIpItem.GetComponent<UISameIpItem>();
            bool isConfirm = data.isIpConfirm == 1;
            item.SetData(data.player_id, data.ip, isConfirm, ItemIpColor(msg, i));
            playerList.Add(item);

            if (isConfirm && data.player_id == PlayerInfo.Instance.mPlayerPid)
            {
                ConfirmAfter();
            }
        }
        if (IsAllConfirm())
        {
            ContextManager.Instance.Pop(curContext);
        }
    }
    public string ItemIpColor(ArgsSameIpPlayer msg, int index)
    {
        var list = msg.list;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (list[index].ip == list[i].ip)
            {
                return playerList[i].ipColor;
            }
        }
        colorIndex++;
        return ipColors[colorIndex];
    }
    public void OnClickContinue(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        PlayingGameInfo.Instance.SendConfirmSameIpServer();
    }

    public void OnClickLeave(GameObject gm)
    {
        isSaveTime = false;
        AudioManager.Instance.PlayClickBtnAudio();
        maskBg.GetComponent<Image>().enabled = false;
        if (this.leaveClick != null)
        {
            this.leaveClick.Invoke();
        }
        ContextManager.Instance.Pop(curContext);
    }

    bool isHadSelected = false;
    void ConfirmAfter()
    {
        leaveRoomBtn.transform.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "btn_blue_black");
        continueGamebtn.transform.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "btn_blue_black");
        leaveRoomBtn.GetComponent<Button>().enabled = false;
        continueGamebtn.GetComponent<Button>().enabled = false;
        leaveRoomBtn.raycastTarget = false;
        continueGamebtn.raycastTarget = false;
        isHadSelected = true;
    }

}
