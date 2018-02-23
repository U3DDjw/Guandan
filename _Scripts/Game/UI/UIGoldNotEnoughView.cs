using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
public class GoldNotEnoughContext : BaseContext
{
    public GoldNotEnoughContext()
    {
        ViewType = UIType.GoldNotEnoughView;
    }
}
public class UIGoldNotEnoughView : BasesView
{
    [SerializeField]
    GameObject btnGiveUp;
    [SerializeField]
    GameObject btnBuy;
    void Start()
    {
        EventTriggerListener.Get(btnGiveUp).onClick = OnBtnGiveUp;
        EventTriggerListener.Get(btnBuy).onClick = OnBuyClick;
    }

    void OnBtnGiveUp(GameObject btn)
    {
        //发送消息放弃

        //销毁自己
        CloseSelf();
    }
    void OnBuyClick(GameObject btn)
    {
        Debug.Log("花去一个6，得到无数666");
        CloseSelf();
    }
    void CloseSelf()
    {
        ContextManager.Instance.Pop(curContext);
    }
}
