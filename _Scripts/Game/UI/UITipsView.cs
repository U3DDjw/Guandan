using UnityEngine;
using System.Collections;
using DNotificationCenterManager;
using HaoyunFramework;
using UnityEngine.UI;

public class TipsViewContext : BaseContext
{
    public TipsViewContext()
    {
        ViewType = UIType.TipsView;
    }
}
public class UITipsView : BasesView
{

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(sureLab.transform.parent.gameObject).onClick = OnClickSure;
        EventTriggerListener.Get(cancelLab.transform.parent.gameObject).onClick = OnClickCancel;
    }

    private void OnDestroy()
    {

    }

    HandleMakeSureEvent sureClick;
    HandleMakeSureEvent cancelClick;
    [SerializeField]
    Text contentLab;
    [SerializeField]
    Text sureLab;
    [SerializeField]
    Text cancelLab;
    public void SetEventHandle(string contentStr, string sureStr, string cancelStr, HandleMakeSureEvent sureClick, HandleMakeSureEvent cancelClick)
    {
        this.sureClick = sureClick;
        this.cancelClick = cancelClick;
        contentLab.text = contentStr;
        sureLab.text = sureStr;
        cancelLab.text = cancelStr;
    }

    ///// <summary>
    ///// 暂时还用不到
    ///// </summary>
    ///// <param name="titleStr"></param>
    ///// <param name="sureStr"></param>
    ///// <param name="contentStr"></param>
    ///// <param name="sureClick"></param>
    //public void SetEventHandle(string titleStr,string sureStr,string contentStr,HandleMakeSureEvent sureClick)
    //{
    //    this.sureClick = sureClick;
    //    contentLab.text = contentStr;
    //    sureLab.text = sureStr;
    //}

    public void OnClickSure(GameObject gm)
    {
        //sureLab.GetComponent<Button>().enabled = false;
        if (this.sureClick != null)
        {
            this.sureClick.Invoke();
        }
        UIManagers.Instance.DestroySingleUI(UIType.TipsView);
    }

    public void OnClickCancel(GameObject gm)
    {
        //cancelLab.GetComponent<Button>().enabled = false;
        if (this.cancelClick != null)
        {
            this.cancelClick.Invoke();
        }
        UIManagers.Instance.DestroySingleUI(UIType.TipsView);
    }


}
