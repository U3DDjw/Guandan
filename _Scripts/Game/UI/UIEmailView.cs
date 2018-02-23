using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MsgContainer;
using Haoyun.Utils;
using System;
using DNotificationCenterManager;
using HaoyunFramework;
using UnityEngine.UI;

public class EmailViewContext : BaseContext
{
    public EmailViewContext()
    {
        ViewType = UIType.EmailView;
    }
}
public class UIEmailView : BasesView
{

    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    GameObject shareBtn;
    [SerializeField]
    GameObject oneKeyBtn;
    [SerializeField]
    Transform itemContainer;
    [SerializeField]
    Image exitBtn;
    [SerializeField]
    GameObject noEmailBg;
    [SerializeField]
    GameObject haveEmailBg;
    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EGetEmailSuccess, UpdateGetEmailSuc);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EGetEmailSuccess, UpdateGetEmailSuc);
        EmailManager.Instance.ClearEmailItemDic();//清除emailItem与id的关系
        EmailManager.Instance.ClearNoReadList();
    }
    void Start()
    {
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
        EventTriggerListener.Get(shareBtn).onClick = OnShareClick;
        EventTriggerListener.Get(oneKeyBtn).onClick = OnOneKeyClick;
        EventTriggerListener.Get(exitBtn.gameObject).onClick = OnCloseClick;
        EmailManager.Instance.curGetType = EGetType.ENull;
        RequestNoReadEmail();
        //Test();
    }
    //测试用
    void Test()
    {

        List<EmailData> listEmailData = new List<EmailData>();

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                EmailData email = new EmailData() { emailId = (i + 1).ToString(), rcNum = null, goldNum = null, content = string.Format("你得到房卡{0}张，你得到金币0块", i), title = "购买房卡" };
                listEmailData.Add(email);
            }
            else
            {
                EmailData email = new EmailData() { emailId = (i + 1).ToString(), rcNum = (i).ToString(), goldNum = 2.ToString(), content = string.Format("你得到房卡{0}张，你得到金币2块", i), title = "购买房卡" };
                listEmailData.Add(email);
            }
        }

        EmailManager.Instance.UpdateNoReadList(listEmailData);
        InitNoReadEmailView();
    }

    //领取成功后调用 item中
    void UpdateGetEmailSuc(LocalNotification e)
    {
        //ArgsReceiveEmail args = e.param as ArgsReceiveEmail;
        //if (args != null)
        //{
        //    RemoveNoReadEmailItemView(args.emailId);
        //}
    }


    //设置了item父物体的组件的UIGrid中的排序方式为字母排序
    //item中的领取后的方法中将名字改变为了Z+原来的名字
    void RefreshItemsPos()
    {
        //GridLayoutGroup grid = itemContainer.GetComponent<GridLayoutGroup>();
        //grid.sorting = UIGrid.Sorting.Alphabetic;
        //grid.enabled = true;
    }
    void RequestNoReadEmail()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("time", TimeUtils.ConvertToTime(DateTime.Now).ToString()); //当前时间戳
        form.AddField("sig", "tobeadded");
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_NoReadEmail;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            Debug.Log("未读邮件列表获取成功" + www.text);
            var list = JsonManager.GetEmailData(www.text);
            noEmailBg.SetActive(list.Count > 0 ? false : true);
            EmailManager.Instance.UpdateNoReadList(list);
            InitNoReadEmailView();
        }));
    }
    void InitNoReadEmailView()
    {
        DestroyChildren(itemContainer);
        GameObject itemPre = ResourceManager.Instance.LoadAsset<GameObject>(UIType.EmailItem.Path);
        var list = EmailManager.Instance.mNoReadList;

        bool isNoEmail = list.Count == 0;
        noEmailBg.SetActive(isNoEmail);
        haveEmailBg.SetActive(!isNoEmail);
        maskBg.GetComponent<Image>().color = isNoEmail ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);

        for (int i = 0; i < list.Count; i++)
        {
            GameObject item = GameObject.Instantiate(itemPre);
            item.transform.SetParent(itemContainer);
            item.transform.localPosition = new Vector3(0, -100 * i, 0);
            item.transform.localScale = Vector3.one;
            //SetData
            item.GetComponent<UIEmailItem>().SetData(list[i]);
            EmailManager.Instance.AddEmailItemToDic(list[i].emailId, item);
        }
        //RefreshItemsPos();
    }
    void DestroyChildren(Transform t)
    {
        if (t == null) return;
        while (t.childCount > 0)
        {
            Destroy(t.GetChild(0));
        }
    }
    //————————————————————————————按钮
    void OnShareClick(GameObject g)
    {
        SDKManager.Instance.WeChatShareNativeImage();
    }
    void OnOneKeyClick(GameObject g)
    {
        EmailManager.Instance.OneKeyGet(this.transform.GetChild(0));
    }
    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
    }

}
