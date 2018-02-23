using HaoyunFramework;
using MsgContainer;
using Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZhiWa;
public class DifferentCardContenxt : BaseContext
{
    public DifferentCardContenxt()
    {
        ViewType = UIType.CardTypeTips;
    }
}
public class UIDifferentCardType : BasesView
{
    [SerializeField]
    GameObject leftContainer;
    [SerializeField]
    GameObject rightContainer;
    [SerializeField]
    GameObject leftTipsBtn;
    [SerializeField]
    GameObject rightTipsBtn;
    // Use this for initialization
    List<MsgCardGroup> listGroup;
    void Start()
    {
        EventTriggerListener.Get(leftTipsBtn).onClick = OnLeftTipsClick;
        EventTriggerListener.Get(rightTipsBtn).onClick = OnRightTipsClick;
    }
    public void SetData(List<MsgCardGroup> listGroup)
    {
        leftTipsBtn.transform.GetChild(0).GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, GetSprName(listGroup[0].ct));
        rightTipsBtn.transform.GetChild(0).GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, GetSprName(listGroup[1].ct));
        this.listGroup = listGroup;
        for (int group = 0; group < listGroup.Count; group++)
        {
            List<uint> cards = listGroup[group].card;
            if (group == 0)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    GameObject item = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(UIType.SingleCard.Path));
                    item.transform.SetParent(leftContainer.transform);
                    //item.transform.GetChild(0).GetComponent<UIWidget>().depth = leftContainer.GetComponent<UIWidget>().depth + 1;
                    //SingleCard singleCard = item.GetComponent<SingleCard>(); //将多余的牌拿出来
                    item.transform.localScale = Vector3.one * MsgContainer.GlobalData.mCardTypeTipRate;
                    item.transform.localPosition = new Vector3(MsgContainer.GlobalData.mSingelCardRateY * i, 0, 0);
                    item.GetComponent<SingleCard>().SetSprName((uint)cards[i], "");//更改
                    //点击出那个，就将这个类型发送出去
                }
            }
            else
            {
                for (int i = 0; i < listGroup[group].card.Count; i++)
                {
                    GameObject item = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(UIType.SingleCard.Path));
                    item.transform.SetParent(rightContainer.transform);
                    //item.transform.GetChild(0).GetComponent<UIWidget>().depth = leftContainer.GetComponent<UIWidget>().depth + 1;
                    item.transform.localScale = Vector3.one * MsgContainer.GlobalData.mCardTypeTipRate;
                    item.transform.localPosition = new Vector3(-MsgContainer.GlobalData.mSingelCardRateY * (cards.Count - 1 - i), 0, 0);//更改
                    item.GetComponent<SingleCard>().SetSprName((uint)cards[i], "");
                    //点击出那个，就将这个类型发送出去
                }
            }
        }
    }
    void OnLeftTipsClick(GameObject g)
    {
        TGuanDanCT A = listGroup[0].ct; //数据0

        SelfCardsManager.Instance.SendPutOutCardWithCT(A);
        //2.销毁当前的View
        ContextManager.Instance.Pop(curContext);
    }
    void OnRightTipsClick(GameObject g)
    {
        //listGroup[1] //数据1
        TGuanDanCT B = listGroup[1].ct; //数据0
        SelfCardsManager.Instance.SendPutOutCardWithCT(B);
        ContextManager.Instance.Pop(curContext);
    }
    string GetSprName(TGuanDanCT type)
    {
        string sprName = "";
        switch (type)
        {
            case TGuanDanCT.CT_BA_ZHANG_BOMB:
            case TGuanDanCT.CT_SI_ZHANG_BOMB:
            case TGuanDanCT.CT_WU_ZHANG_BOMB:
            case TGuanDanCT.CT_LIU_ZHANG_BOMB:
            case TGuanDanCT.CT_QI_ZHANG_BOMB:
                sprName = "pk_zha";
                break;
            case TGuanDanCT.CT_SHUN_ZI://顺子
                sprName = "pk_shunzi";
                break;
            case TGuanDanCT.CT_LIANG_LIAN_DUI://连对
                sprName = "pk_liandui_0";
                break;
            case TGuanDanCT.CT_TONG_HUA_SHUN://同花顺
                sprName = "pk_tonghuashun";
                break;
            case TGuanDanCT.CT_GANG_BAN://飞机
                sprName = "pk_feiji";
                break;
            case TGuanDanCT.CT_HU_LU://借风
                sprName = "pk_jiefeng";
                break;
        }
        return sprName;
    }
}
