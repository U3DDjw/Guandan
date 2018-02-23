using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Contexts;
using MsgContainer;
using UnityEngine.UI;
using HaoyunFramework;
public class RoomcardContext : BaseContext
{
    public RoomcardContext()
    {
        ViewType = UIType.RoomcardView;
    }
}
public class UIRoomcardView : BasesView
{
    [SerializeField]
    Image copyBtn1;
    [SerializeField]
    Image copyBtn2;
    [SerializeField]
    Image closeBtn;
    [SerializeField]
    Text  zhaoShangLab;
    [SerializeField]
    Text keFuLab;
    [SerializeField]
    Image maskBg;

    Text curLab;
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(copyBtn1.gameObject).onClick = OnCopyClcik;
        EventTriggerListener.Get(copyBtn2.gameObject).onClick = OnCopyClcik;
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(maskBg.gameObject).onClick = OnCloseClick;
        InitData();
    }
    void InitData()
    {
        SomeText someText = PlayerInfo.Instance.mUrlData.someText;
        if (someText != null)
        {
            zhaoShangLab.text = someText.zhaoshang;
            keFuLab.text = someText.kefu;
        }
    }
    void OnCopyClcik(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        curLab = g == copyBtn1.gameObject ? zhaoShangLab : keFuLab;
        GlobalData.CopyTextFromLab(curLab.text.Trim());
       
    }
    void OnCloseClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        ContextManager.Instance.Pop(curContext);
    }
    void Android()
    {
        AndroidJavaObject androidObject = new AndroidJavaObject("com.test.ClipboardTools");
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

    }


}
