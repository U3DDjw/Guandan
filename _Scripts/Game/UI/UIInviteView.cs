using Haoyun.Utils;
using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
using UnityEngine.UI;

public class InviteContext : BaseContext
{
    public InviteContext()
    {
        ViewType = UIType.InviteView;
    }
}
public class UIInviteView : BasesView
{
    [SerializeField]
    Text infoLab;
    [SerializeField]
    Text noticNumLabOne;
    [SerializeField]
    Text roomCardLabOne;
    [SerializeField]
    Text noticNumLabTwo;
    [SerializeField]
    Text roomCardLabTwo;
    [SerializeField]
    InputField inviteIdInput;
    [SerializeField]
    Text inviteTipLab;
    [SerializeField]
    Image InviteBtn;
    [SerializeField]
    Image sureBtn;

    [SerializeField]
    Image closeBtn;
    [SerializeField]
    Image maskBg;
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(InviteBtn.gameObject).onClick = OnInviteClick;
        EventTriggerListener.Get(sureBtn.gameObject).onClick = OnSureClick;
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnCloseClick;
        EventTriggerListener.Get(maskBg.gameObject).onClick = OnCloseClick;
        //UIEventListener.Get(weChatGm.transform.GetChild(0).gameObject).onClick = OnFriendClick;
        //UIEventListener.Get(weChatGm.transform.GetChild(1).gameObject).onClick = OnFriendQuanClick;
        RequestData();
    }
    void OnInviteClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        //weChatGm.SetActive(!weChatGm.activeSelf);
        var playerId = PlayerInfo.Instance.mPlayerData.pid;
        var playerName = PlayerInfo.Instance.mPlayerData.name;
        string title = GlobalData.mGameName + " 新手任务领现金红包";
        string descripition = playerName + " 邀请你一起来掼蛋!\r\n绑定ID: " + playerId;
		string url = GlobalData.WeChatShareUrl + "?pid=" + playerId;
        SDKManager.Instance.WeChatShareLink(title, descripition, url);
    }
    //void OnFriendClick(GameObject g)
    //{
    //    var playerId = PlayerInfo.Instance.mPlayerData.pid;
    //    var playerName = PlayerInfo.Instance.mPlayerData.name;
    //    string title = "好运掼蛋 绑定有礼";
    //    string descripition = playerName + " 邀请你一起来掼蛋!\r\n绑定ID: " + playerId;
    //    string url = GlobalData.WeChatShareUrl;
    //    SDKManager.Instance.WeChatShareLink(title, descripition, url);
    //}
    //void OnFriendQuanClick(GameObject g)
    //{
    //    SDKManager.Instance.WeChatShareImage();
    //}
    void OnSureClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (inviteIdInput.text == null || inviteIdInput.text.Length == 0)
        {
            UIManagers.Instance.EnqueueTip("请输入邀请者Id");
        }
        else
        {
            RequestBindInviter();
        }
    }
    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
    }
    void RequestData()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("time", TimeUtils.ConvertToTime(System.DateTime.Now).ToString());
        // form.AddField("sig", GlobalData.sig);
        string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_GetInviterInfo;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            string jsonText = www.text;
            if (jsonText != null)
            {
                InviterData data = JsonManager.GetInviterData(jsonText);
                SetData(data.count, data.count2, data.inviterId);
            }
        }
            ));
    }
    void SetData(int inviteNum, int inviteNum2, ulong inviteId)
    {
        inviteIdInput.text = null;
        infoLab.text = string.Format("活动规则:每邀请一名好友,让其填写您游戏ID,您即可获赠{0}张房卡,好友获赠10张房卡!好友每邀请1名新玩家,您可再获得1张房卡,每日30张房卡封顶!活动最终解释权归官方所有!", 3);
        noticNumLabOne.text = string.Format("{0}名", inviteNum);
        roomCardLabOne.text = string.Format("获得房卡{0}张", inviteNum * 3);

        noticNumLabTwo.text = string.Format("{0}名", inviteNum2);
        roomCardLabTwo.text = string.Format("获得房卡{0}张", inviteNum2 * 1);

        if (inviteId == 0) { inviteIdInput.text = ""; sureBtn.gameObject.SetActive(true); }
        else
        {
            inviteIdInput.text = inviteId.ToString();
            HaveInviterId();
        }

    }
    void HaveInviterId()
    {
        sureBtn.gameObject.SetActive(false);
        inviteIdInput.GetComponent<InputField>().interactable = false;
        inviteIdInput.GetComponent<Image>().raycastTarget = false;
        inviteTipLab.text = "邀请者ID";
    }
    void RequestBindInviter()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", GlobalData.mAppId);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("inviterId", inviteIdInput.text);
        form.AddField("time", TimeUtils.ConvertToTime(System.DateTime.Now).ToString());
        //form.AddField("sig", GlobalData.sig);
        string url = GlobalData.mConstBaseServerUrl + ServerUrlTitle.Url_BindInviter;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            UIManagers.Instance.EnqueueTip(www.text);
            HaveInviterId();
        }
        ));
    }
}
