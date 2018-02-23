using UnityEngine;
using System.Collections;
using HaoyunFramework;
using UnityEngine.UI;
using System.Collections.Generic;
using MsgContainer;

public class ShopContext : BaseContext
{
    public ShopContext()
    {
        ViewType = UIType.ShopView;
    }
}
public class UIShopView : BasesView
{

    [SerializeField]
    GameObject maskBg;

    [SerializeField]
    Text zhaoShangLab;//招商
    [SerializeField]
    Text buyLab;
    [SerializeField]
    Image closeBtn;
    [SerializeField]
    List<RawImage> listRoomCardBtn = new List<RawImage>();
    [SerializeField]
    List<RawImage> listGoldBtn = new List<RawImage>();

    GameObject curSelected;//当前的选择
    //uint[] card_priceArray = new uint[] { 6, 18, 60 };
    uint[] cardNum = new uint[] { 3, 9, 30 };

    //uint[] coin_price = new uint[] { 6, 25, 50 };
    uint[] coinNum = new uint[] { 1200, 5000, 10000 };
    void Start()
    {
        Debug.Log("打开商城界面");
        EventTriggerListener.Get(maskBg).onClick = OnMaskClick;
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnMaskClick;

        InItData();
    }
    void SetLabActive(bool isshow)
    {
        zhaoShangLab.gameObject.SetActive(isshow);
        buyLab.gameObject.SetActive(isshow);
    }
    void InItData()
    {
        if (PlayerInfo.Instance.mUrlData == null)
        {
            Debug.Log("mUlData==null");
        }
        else
        {
            TextureURLData data = PlayerInfo.Instance.mUrlData;
            bool isShow = !data.iosSj;
            SetLabActive(isShow);
            if (data.someText != null)
            {
                zhaoShangLab.text = "掼蛋招商:" + data.someText.zhaoshang;
                buyLab.text = "掼蛋客服:" + data.someText.kefu;
            }
            else
            {
                Debug.Log("SomeText==null");
            }
        }
        for (int i = 0; i < listRoomCardBtn.Count; i++)
        {
            EventTriggerListener.Get(listRoomCardBtn[i].gameObject).onClick = OnRoomcardClick;
        }
        for (int i = 0; i < listGoldBtn.Count; i++)
        {
            EventTriggerListener.Get(listGoldBtn[i].gameObject).onClick = OnGoldClick;
        }
    }
    //购买金币按钮点击
    void OnGoldClick(GameObject g)
    {
        int i = listGoldBtn.IndexOf(g.GetComponent<RawImage>());
        string goldNum = coinNum[i].ToString();
        Debug.Log("购买金币的数量" + goldNum);
    }
    void OnRoomcardClick(GameObject g)
    {
        int i = listRoomCardBtn.IndexOf(g.GetComponent<RawImage>());
        string fkNum = cardNum[i].ToString();
        Debug.Log("购买的房卡数量：" + fkNum);

#if UNITY_IPHONE || UNITY_IOS
        AudioManager.Instance.PlayClickBtnAudio();
		if (GameManager.Instance.mGameMode == EGameMode.EAppleOnLine) {
			GlobalData.applePayOrderId = System.Guid.NewGuid().ToString("N");
			string itemID = "fk" + fkNum;
			double rmb = 0.0;
			if(fkNum=="3"){
				rmb = 6;
			} else if(fkNum == "9"){
				rmb = 18;
			} else if(fkNum == "30"){
				rmb = 60;
			}
			TDGAVirtualCurrency.OnChargeRequest(GlobalData.applePayOrderId,itemID,rmb,"CNY",System.Convert.ToDouble(fkNum),"applePay");
			IOSIAP.GetIns ().PurchaseItem (itemID);
		}
#endif

    }
    void OnMaskClick(GameObject gm)
    {
        ContextManager.Instance.Pop(curContext);
    }

}
