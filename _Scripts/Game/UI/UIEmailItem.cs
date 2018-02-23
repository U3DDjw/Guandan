using UnityEngine;
using System.Collections;
using Haoyun.Utils;
using MsgContainer;
using System;
using DNotificationCenterManager;
using System.Collections.Generic;
using UnityEngine.UI;
using HaoyunFramework;

public class EmailItemContext : BaseContext
{
    public EmailItemContext()
    {
        ViewType = UIType.EmailItem;
    }
}
public class UIEmailItem : BasesView
{

    [SerializeField]
    GameObject getBtn;
    [SerializeField]
    Image getInfo;
    [SerializeField]
    Image icon;

    [SerializeField]
    Text title;
    [SerializeField]
    Text content;
    private EmailData emailData = null;
    public EmailData mEmailData
    {
        get { return emailData; }
        set { emailData = value; }
    }

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(getBtn).onClick = OnGetClick;
        //HadGet();
    }
    void OnGetClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (!EmailManager.Instance.mEmailItemDic.ContainsKey(mEmailData.emailId))
        {
            UIManagers.Instance.EnqueueTip("该邮件已领取");
        }
        else
        {
            //放在send里面就不能表示是按钮点击
            EmailManager.Instance.curGetType = EGetType.EGet;
            //下面两句只用其中一个
            SendReceiveEmail();
            //Test();
        }
    }
    //测试用
    public void Test()
    {
        ArgsReceiveEmail args = new ArgsReceiveEmail();
        args.emailId = mEmailData.emailId;
        EmailManager.Instance.PlayTween(ref emailData, transform.parent.parent.parent.parent);
        HadGet();//整体item的显示的变化
        //领取成功
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGetEmailSuccess, args);
    }
    public void SendReceiveEmail()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        //   form.AddField("pid", "1430");
        form.AddField("id", emailData.emailId);
        form.AddField("time", TimeUtils.ConvertToTime(DateTime.Now).ToString()); //当前时间戳
        form.AddField("sig", "tobeadded");
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_ReceiveEmail;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            Debug.Log("邮件领取结果:" + www.text);
            if (www.text == "success")
            {
                PlayerInfo.Instance.AddRoomCard(int.Parse(mEmailData.rcNum = mEmailData.rcNum ?? "0"));
                PlayerInfo.Instance.UpdateGold(int.Parse(mEmailData.goldNum = mEmailData.goldNum ?? "0"));

                ArgsReceiveEmail args = new ArgsReceiveEmail();
                args.emailId = mEmailData.emailId;
                EmailManager.Instance.PlayTween(ref emailData, this.transform.parent.parent.parent);
                HadGet();//整体item的显示的变化
                //领取成功
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EGetEmailSuccess, args);
            }
            else
            {
                UIManagers.Instance.EnqueueTip("领取失败");
            }
        }));
    }
    //被修改
    public void SetData(EmailData emailData)
    {
        this.mEmailData = emailData;
        title.text = emailData.title;
        content.text = emailData.content;
        getInfo.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "title_mail_font1"); ;
    }

    void HadGet()
    {
        this.name = "Z" + this.name;
        getBtn.GetComponent<Image>().raycastTarget = false;
        getBtn.GetComponent<Button>().enabled = false;
        getInfo.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "title_mail_font3");
        //title.color = Color.gray;
        //content.color = Color.gray;

        //this.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "record_list" + "_black");
        icon.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "mail_mail2");
        getBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "btn_blue_black");

    }
}
