using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaoyunFramework;
using MsgContainer;

public class ShopItemContext : BaseContext
{
    public ShopItemContext()
    {
        ViewType = UIType.ShopItem;
    }
}
public class UIShopItem : BasesView
{
    [SerializeField]
    Text pricelab;
    [SerializeField]
    Text cardNumLab;
    [SerializeField]
    Image cardSpr;

    string fkNum;

    private void Start()
    {
        EventTriggerListener.Get(this.gameObject).onClick = OnBuyClick;
    }
    public void SetData(string sprName, uint buyCardNum, uint price, bool MakePixelPerfect)
    {
        cardSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, sprName);
        fkNum = buyCardNum.ToString();
        cardNumLab.text = "X" + buyCardNum.ToString();
        pricelab.text = "¥" + price.ToString();
        if (MakePixelPerfect)
            cardSpr.SetNativeSize();
    }
    public void OnBuyClick(GameObject g)
    {
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
}
