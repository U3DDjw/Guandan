using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
using MsgContainer;
using DNotificationCenterManager;

public class HomeViewTaskBtnContext : BaseContext
{
    public HomeViewTaskBtnContext()
    {
        ViewType = UIType.SameIpView;
    }
}
public class UIHomeViewTaskBtn : MonoBehaviour
{
    GameObject webViewGameObject = null;
    UniWebView webView = null;
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(this.gameObject).onClick = OnBtnClick;
        Debug.Log("屏幕的大小" + new Vector2(Screen.width, Screen.height));
    }

    void OnBtnClick(GameObject g)
    {
        Debug.Log(this.gameObject.name + ":Click");
        Request();
        InitHtml();//for Test
    }
    public void Request()
    {
        WWWForm form = new WWWForm();
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("sig", "sig");
        string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_ActivityInfo;
        StartCoroutine(GlobalData.SendPost(url, form, FunCallBack));
    }

    public void FunCallBack(WWW www)
    {
        print("www.text" + www.text);
        if (www.text == "true")
        {
            //  ActivityInfo info = JsonManager.GetActivityInfo(www.text);
        }
        else
        {
            UIManagers.Instance.EnqueueTip("获取失败");
        }
    }
    void InitHtml()
    {
        ContextManager.Instance.Push(new TaskContext());
        GameObject taskView = UIManagers.Instance.GetSingleUI(UIType.TaskView);
        taskView.transform.SetParent(this.transform.parent.parent);
        taskView.transform.localPosition = Vector3.zero;
        string url = string.Format(GlobalData.mTaskActivityHtmlUrl, GlobalData.mAppId, PlayerInfo.Instance.mPlayerPid);
        Debug.Log("点击链接" + url);
        taskView.GetComponent<UITaskView>().SetData(url);
    }

}
