using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using HaoyunFramework;
public class RuleContext : BaseContext
{
    public RuleContext()
    {
        ViewType = UIType.RuleView;
    }
}
public class UIRuleView : BasesView
{
    [SerializeField]
    RawImage guandanBtn;
    [SerializeField]
    RawImage zhuandanBtn;
    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    Image closeBtn;
    [SerializeField]
    Text ruleContent;
    [SerializeField]
    Text ruleContentZhuandan;
    string guandanContent = "";
    string zhuandanContent = "";
    GameObject curClickBtn = null;
    GameObject mCurClickBtn
    {
        get
        {
            return curClickBtn;
        }
        set
        {
            curClickBtn = value;
            GameObject zhuanDanSelected = zhuandanBtn.transform.GetChild(1).gameObject;
            GameObject guandanSelected = guandanBtn.transform.GetChild(1).gameObject;
            if (curClickBtn == zhuandanBtn.gameObject)
            {
                guandanSelected.SetActive(false);
                zhuanDanSelected.SetActive(true);
                SetContent(zhuandanContent);
            }
            else
            {
                guandanSelected.SetActive(true);
                zhuanDanSelected.SetActive(false);
                SetContent(guandanContent);
            }
        }
    }
    void Start()
    {
        EventTriggerListener.Get(guandanBtn.gameObject).onClick = OnBtnClick;
        EventTriggerListener.Get(zhuandanBtn.gameObject).onClick = OnBtnClick;
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
        InitData();
    }
    void InitData()
    {
        //string path = MsgContainer.GlobalData.Path_DataBase + "RuleContentData";
        //string guanDanText = ResourceManager.Instance.LoadText(path);
        //guandanContent = guanDanText;    
		ruleContent.GetComponentInParent<ScrollRect>().vertical=false;
		StartCoroutine (waitOnedelta ());
        guandanContent = ruleContent.text;
        zhuandanContent = ruleContentZhuandan.text;
    }
	IEnumerator waitOnedelta()
	{
		yield return null;
		ruleContent.GetComponentInParent<ScrollRect>().vertical=true;
	}
    void SetContent(string content)
    {
        ruleContent.text = content;
    }
    void OnBtnClick(GameObject g)
    {
        mCurClickBtn = g;
    }
    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
    }
}
