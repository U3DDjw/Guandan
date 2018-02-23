using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
using UnityEngine.UI;
using ZhiWa;
using DNotificationCenterManager;
using MsgContainer;

public class GoldFiledContext : BaseContext
{
    public GoldFiledContext()
    {
        ViewType = UIType.GoldFiledView;
    }
}

public class UIGoldFiledView : BasesView
{
    [SerializeField]
    GameObject backHomeBtn;
    [SerializeField]
    GameObject priBtn;
    [SerializeField]
    Text priBaseScore;
    [SerializeField]
    Text priBaseGold;

    [SerializeField]
    GameObject advBtn;
    [SerializeField]
    Text advBaseScore;
    [SerializeField]
    Text advBaseGold;

    [SerializeField]
    GameObject highBtn;
    [SerializeField]
    Text highBaseScore;
    [SerializeField]
    Text highBaseGold;

    [SerializeField]
    GameObject topBtn;
    [SerializeField]
    Text topBaseScore;
    [SerializeField]
    Text topBaseGold;


    List<int> priList;
    List<int> advList;
    List<int> highList;
    List<int> topList;

    private void Awake()
    {
        GoldFiledManager.Instance.ChangeFiledStatus(true);
        InitData();
    }

    void InitData()
    {
        List<List<int>> listlistData = GlobalData.GetGoldFiledData;
        priList = listlistData[0];
        advList = listlistData[1];
        highList = listlistData[2];
        topList = listlistData[3];

        priBaseScore.text = "底分:" + priList[0].ToString();
        priBaseGold.text = string.Format("({0}+ 金币)", priList[1]);

        advBaseScore.text = "底分:" + advList[0].ToString();
        advBaseGold.text = string.Format("({0}+ 金币)", advList[1]);

        highBaseScore.text = "底分:" + highList[0].ToString();
        highBaseGold.text = string.Format("({0}+ 金币)", highList[1]);

        topBaseScore.text = "底分:" + topList[0].ToString();
        topBaseGold.text = string.Format("({0}+ 金币)", topList[1]);

    }
    private void Start()
    {
        EventTriggerListener.Get(backHomeBtn).onClick = OnBackHomeBtnClick;
        EventTriggerListener.Get(priBtn).onClick = OnPriBtnClick;
        EventTriggerListener.Get(advBtn).onClick = OnAdvBtnClick;
        EventTriggerListener.Get(highBtn).onClick = OnHighBtnClick;
        EventTriggerListener.Get(topBtn).onClick = OnTopBtnClick;
    }
    void OnBackHomeBtnClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGoldViewBack);
    }
    void OnPriBtnClick(GameObject g)
    {
        GoldFiledManager.Instance.SendEnterGoldFiledServer((int)TGoldPlaygroundType.TGoldPlaygroundType1, (uint)priList[0], (uint)priList[1]);
    }

    void OnAdvBtnClick(GameObject g)
    {
        GoldFiledManager.Instance.SendEnterGoldFiledServer((int)TGoldPlaygroundType.TGoldPlaygroundType2, (uint)advList[0], (uint)advList[1]);
    }

    void OnHighBtnClick(GameObject g)
    {
        GoldFiledManager.Instance.SendEnterGoldFiledServer((int)TGoldPlaygroundType.TGoldPlaygroundType3, (uint)highList[0], (uint)highList[1]);
    }

    void OnTopBtnClick(GameObject g)
    {
        GoldFiledManager.Instance.SendEnterGoldFiledServer((int)TGoldPlaygroundType.TGoldPlaygroundType4, (uint)topList[0], (uint)topList[1]);
    }

    private void OnDestroy()
    {
        GoldFiledManager.Instance.ChangeFiledStatus(false);
    }

}
