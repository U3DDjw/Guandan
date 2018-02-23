using UnityEngine;
using System.Collections;
using MsgContainer;
using UnityEngine.UI;
using HaoyunFramework;
public class BuyCoinContext : BaseContext
{
    public BuyCoinContext()
    {
        ViewType = UIType.BuyCoinView;
    }
}
public class UIBuyCoinView : BasesView
{
    [SerializeField]
    Image copyBtn;
    [SerializeField]
    Image closeBtn;
    [SerializeField]
    Image maskBg;
    [SerializeField]
    Text buyLab;
    [SerializeField]
    Text serviceLab;
    void Start()
    {
        EventTriggerListener.Get(copyBtn.gameObject).onClick = OnCopyClcik;
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(maskBg.gameObject).onClick = OnCloseClick;
        if (PlayerInfo.Instance.mUrlData.someText != null)
            buyLab.text = PlayerInfo.Instance.mUrlData.someText.goumai;
    }
    void OnCopyClcik(GameObject g)
    {
        GlobalData.CopyTextFromLab(serviceLab.text.Trim());
    }
    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
    }

}
