using UnityEngine;
using System.Collections;
using ZhiWa;
using Net;
using MsgContainer;
using DNotificationCenterManager;
using System.Collections.Generic;
using UnityEngine.UI;
using HaoyunFramework;


public class CreateRoomContext : BaseContext
{
    public CreateRoomContext()
    {
        ViewType = UIType.CreateRoomView;
    }
}
public class UICreateRoomView : BasesView
{


    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    GameObject createBtn;
    [SerializeField]
    Text curRCNum;//房卡数量
    [SerializeField]
    SelectOne gameType;
    [SerializeField]
    SelectOne guanDanContainer;
    [SerializeField]
    SelectOne zhuanDanContainer;
    [SerializeField]
    SelectOne payType;
    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    List<Text> cardLabs = new List<Text>();
    [SerializeField]
    SelectType selectRoomType;

    [SerializeField]
    GameObject dkRoomBtn;//代开房间按钮
    [SerializeField]
    GameObject hadRoomBtn;//已开房间按钮

    [Tooltip("子物体的顺序需要和RoomType中的顺序一致")]
    [SerializeField]
    GameObject bottomContainer;
    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGUANDAN_ROOM_GD_FKBZ, UpdateFKBZ);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGUANDAN_ROOM_GD_KF_SUC, UpdateSuccess);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGUANDAN_ROOM_GD_FKBZ, UpdateFKBZ);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ERoomJoin_Suc, UpdateJoinRoomSuc);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGUANDAN_ROOM_GD_KF_SUC, UpdateSuccess);
    }

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(closeBtn).onClick = OnCloseClick;
        EventTriggerListener.Get(createBtn).onClick = OnCreateClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
        EventTriggerListener.Get(dkRoomBtn).onClick = OnDkRoomClick;
        EventTriggerListener.Get(hadRoomBtn).onClick = OnHasRoomClick;
        InitRoomType();
        payType.OnHasChange = SetListLabsData;
    }
    void OnDkRoomClick(GameObject g)
    {
        Debug.Log("点击了代开房间按钮");
    }
    void OnHasRoomClick(GameObject g)
    {
        Debug.Log("点击了已开房间按钮");
        //ContextManager.Instance.Push(new BaseContext() { ViewType =UIType.HasOpenRoomView});
        UIManagers.Instance.GetSingleUI(UIType.HasOpenRoomView).transform.SetParent(this.transform);
    }
    void InitRoomType()
    {
        selectRoomType.selectType = SelectType_EventSelectType; //销毁自然销毁
        selectRoomType.SetRoomType(1);//默认自开房间
        InitCreateView();
    }

    private void SelectType_EventSelectType(int type)
    {
        Debug.Log("当前创建房间类型:" + type);
        //子物体最后一个公共的 所以长度-1
        for (int i = 0; i < bottomContainer.transform.childCount - 1; i++)
        {
            bottomContainer.transform.GetChild(i).gameObject.SetActive(i == type - 1);
        }
    }

    void InitCreateView()
    {
        curRCNum.text = "拥有房卡:" + PlayerInfo.Instance.GetRoomCard.ToString();
        ESelectType selectType = (ESelectType)XPlayerPrefs.Instance.mGamePay;
        //加载蛋的模式和付费缓存
        payType.SetDefaultCreateRoominfo(selectType);
        //加载游戏的具体模式的缓存
        if (XPlayerPrefs.Instance.mGameType == 1)//转蛋
        {
            gameType.SetDefaultCreateRoominfo((ESelectType)1);
            int num = XPlayerPrefs.Instance.mGameNum;
            zhuanDanContainer.SetDefaultCreateRoominfo((ESelectType)(num / 4));
        }
        else if (XPlayerPrefs.Instance.mGameType >= 2)//掼蛋
        {
            gameType.SetDefaultCreateRoominfo((ESelectType)2);
            int num = XPlayerPrefs.Instance.mGameNum;
            int newgametype = XPlayerPrefs.Instance.mGameType;
            //打二
            if (XPlayerPrefs.Instance.mGameType > 2)//这个是因为 如果是掼蛋一直2，然后再打打到6默认num是大于0的所以根据type判断更好
            {
                guanDanContainer.SetDefaultCreateRoominfo((ESelectType)newgametype);
            }
            //非打二
            else
            {
                guanDanContainer.SetDefaultCreateRoominfo((ESelectType)(num / 4));
            }
        }
        SetListLabsData(XPlayerPrefs.Instance.mGamePay);
    }
    /// <summary>
    /// 设置房卡数值
    /// </summary>
    /// <param name="paytype"></param>
    void SetListLabsData(int paytype)
    {
        if (cardLabs.Count != 0)
        {
            string[] A = new string[] { "1", "1", "2", "1", "1", "0", "0", "0", "3" };//AA  前三个是转蛋中的
            string[] B = new string[] { "2", "3", "6", "2", "3", "0", "0", "0", "9" };//房主支付 掼蛋横着过去
            if (paytype == 1)//AA
            {
                for (int i = 0; i < cardLabs.Count; i++)
                {
                    cardLabs[i].text = "X" + A[i];
                }
            }
            else
            {
                for (int i = 0; i < cardLabs.Count; i++)
                {
                    cardLabs[i].text = "X" + B[i];
                }
            }
        }
        else
        {
            Debug.LogError("房卡数组数量为0");
        }
    }
    void UpdateFKBZ(LocalNotification e)
    {
        UIManagers.Instance.EnqueueTip("房卡不足");
    }

    void UpdateSuccess(LocalNotification e)
    {
        AudioManager.Instance.PlayCommonAudio(GlobalData.AudioNameJoinRoomPlayer);
        ContextManager.Instance.Pop(curContext);
        ContextManager.Instance.Push(new WaitGameContext());
    }

    void UpdateJoinRoomSuc(LocalNotification e)
    {
        ContextManager.Instance.Pop(curContext);
    }
    public void OnCreateClick(GameObject gm)
    {
        Debug.Log("Click " + gm.name);

        if (RoomInfo.Instance.mIsExistWaitGameView)
        {
            UIManagers.Instance.EnqueueTip("房间已存在");
        }
        else
        {
            SendServerArgs();
        }

    }

    void OnCloseClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Pop(curContext);
    }

    void SendServerArgs()
    {
        MsgGlobal mGl = new MsgGlobal();
        mGl.room_info = new MsgRoomInfo();
       MsgRoomInfo msg = mGl.room_info;
        msg.creater_id = (ulong)PlayerInfo.Instance.mPlayerPid;
        //默认游戏类型的变化需要修改下面这行代码
        msg.game_type = (int)gameType.mSelectTeam == 1 ? TGuanDanGameType.TGuanDanGameTypeZhuanDan : TGuanDanGameType.TGuanDanGameTypeGuanDan;
        msg.card_use_type = (int)payType.mSelectTeam == 1 ? TGuanDanCardUseType.TGuanDanCardUseTypeAa : TGuanDanCardUseType.TGuanDanCardUseTypeCreater;
        if (msg.game_type == TGuanDanGameType.TGuanDanGameTypeZhuanDan)
        {
            msg.game_num = GetZhuandanNumType((int)zhuanDanContainer.mSelectTeam);
        }
        else if (msg.game_type == TGuanDanGameType.TGuanDanGameTypeGuanDan)
        {
            if (guanDanContainer.mSelectTeam == ESelectType.EA || guanDanContainer.mSelectTeam == ESelectType.EB)
            {
                msg.game_num = GetZhuandanNumType((int)guanDanContainer.mSelectTeam);
            }
            else
            {
                msg.game_type = GetGameType(guanDanContainer.mSelectTeam);
            }
        }

        //-------缓存游戏模式
        XPlayerPrefs.Instance.mGamePay = (int)msg.card_use_type == 1 ? 2 : 1;
        XPlayerPrefs.Instance.mGameType = (int)msg.game_type;
        XPlayerPrefs.Instance.mGameNum = (int)msg.game_num;

        //房卡消耗，和打到几
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_CREATE_ROOM, mGl);

        Debug.Log("游戏类型：" + msg.game_type);
        Debug.Log("游戏局数：" + msg.game_num);
        Debug.Log("支付方式：" + msg.card_use_type);
    }

    uint GetZhuandanNumType(int num)
    {
        uint returnValue = 2;
        switch (num)
        {
            //打几把
            case 1:
                returnValue = 4;
                break;
            case 2:
                returnValue = 8;//暂定
                break;
            //打到几
            case 3:
                returnValue = 12;
                break;
        }
        return (uint)returnValue;
    }
    TGuanDanGameType GetGameType(ESelectType selectType)
    {
        TGuanDanGameType tg = TGuanDanGameType.TGuanDanGameTypeGuanDan6;
        switch (selectType)
        {
            case ESelectType.EC:
                tg = TGuanDanGameType.TGuanDanGameTypeGuanDan6;
                break;
            case ESelectType.ED:
                tg = TGuanDanGameType.TGuanDanGameTypeGuanDan8;
                break;
            case ESelectType.EE:
                tg = TGuanDanGameType.TGuanDanGameTypeGuanDan10;
                break;
            case ESelectType.EF:
                tg = TGuanDanGameType.TGuanDanGameTypeGuanDanA;
                break;
        }
        return tg;
    }
}
