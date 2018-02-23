using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using MsgContainer;
using Haoyun.Utils;
using DNotificationCenterManager;
using HaoyunFramework;
using UnityEngine.UI;

public class BindPhoneContext : BaseContext
{
    public BindPhoneContext()
    {
        ViewType = UIType.BindPhoneView;
    }
}
public class UIBindPhoneView : BasesView
{

    [SerializeField]
    Image getCheckNumBtn;
    [SerializeField]
    GameObject bindBtn;
    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    InputField phoneNumInput;
    [SerializeField]
    InputField passwordInput;
    [SerializeField]
    InputField checkNumInput;
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(getCheckNumBtn.gameObject).onClick = OnGetCheckCodeClick;
        EventTriggerListener.Get(closeBtn).onClick = OnCloseClick;
        EventTriggerListener.Get(bindBtn).onClick = OnBindClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
    }
    void OnGetCheckCodeClick(GameObject g)
    {
        bool end = CheckPhoneNum(phoneNumInput.text);
        if (end)
        {
            phoneNumInput.GetComponent<Image>().raycastTarget = false;
            UIManagers.Instance.EnqueueTip("正在向手机号:" + phoneNumInput.text + "发送验证码");
            StartCoroutine(CountDown());//开始倒计时
            SendMessage();
        }
        else
        {
            Debug.Log("手机号输入错误");
            UIManagers.Instance.EnqueueTip("手机号输入错误");
        }
    }
    void OnBindClick(GameObject g)
    {
        if (CheckSecurityCode() && CheckPassword())
        {
            SendInputInfoToService();
        }
    }


    void OnCloseClick(GameObject g)
    {
        ContextManager.Instance.Pop(curContext);
    }
    //手机号验证
    bool CheckPhoneNum(string phone)
    {
        bool end = Regex.IsMatch(phone, @"0?(13|14|15|17|18)[0-9]{9}");
        //验证成功发送消息
        return end;
    }
    //密码验证
    bool CheckPassword()
    {
        string password = passwordInput.text;
        if (password.Length > 12 || password.Length < 6)
        {
            UIManagers.Instance.EnqueueTip("密码输入长度不在限制的范围之内！");
            return false;
        }
        else
        {
            //^开始匹配 $表示结束匹配
            if (!Regex.IsMatch(password, @"^[0-9A-Za-z/_]{6,12}$"))
            {
                UIManagers.Instance.EnqueueTip("密码由数字,英文,下划线组成");
                return false;
            }
        }
        return true;

    }
    bool CheckSecurityCode()
    {
        if (checkNumInput.text == null || checkNumInput.text.Length == 0)
        {
            UIManagers.Instance.EnqueueTip("验证码不能为空");
            return false;
        }
        return true;
        //验证验证码输入是否准确
        //准确1.提示：绑定成功，2.发送礼物，到邮箱
        //UIManager.Instance.EnqueueTip("绑定成功，奖励已经发送到邮箱!");
        //string phoneNum = phoneNumInput.value;
    }
    void SendMessage()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", "HY_NJ_GD");
        form.AddField("phone", phoneNumInput.text);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("time", TimeUtils.ConvertToTime(System.DateTime.Now).ToString());
        //form.AddField("sig", "tobeadded");//手机标识
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_SendMessage;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
          {
              string text = www.text;
              if (text == "OK")
              {
                  UIManagers.Instance.EnqueueTip("短信发送成功,请注意查收！");
              }
              else
              {
                  UIManagers.Instance.EnqueueTip("短信发送失败");
              }
          }));
    }
    void SendInputInfoToService()
    {   //将手机号和密码发送到服务器
        WWWForm form = new WWWForm();
        form.AddField("appId", "HY_NJ_GD");
        form.AddField("phone", phoneNumInput.text);
        form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
        form.AddField("code", checkNumInput.text);
        form.AddField("password", passwordInput.text);
        form.AddField("time", TimeUtils.ConvertToTime(System.DateTime.Now).ToString());
        //form.AddField("sig", "tobeadded");//手机标识
        string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_BindSubmit;
        StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
        {
            string text = www.text;
            if (text == "OK")
            {
                UIManagers.Instance.EnqueueTip("绑定成功！奖励将通过邮箱发送");
                PlayerInfo.Instance.mPlayerData.phone = phoneNumInput.text;
                NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EUpdatePlayerInfo);
            }
            else
            {
                UIManagers.Instance.EnqueueTip(www.text);
            }
            OnCloseClick(null);
        }));
    }
    IEnumerator CountDown()
    {
        GameObject checkChildOne = getCheckNumBtn.transform.GetChild(0).gameObject;
        GameObject checkChildTwo = getCheckNumBtn.transform.GetChild(1).gameObject;
        getCheckNumBtn.GetComponent<Button>().enabled = false;

        checkChildOne.SetActive(false);
        checkChildTwo.SetActive(true);
        getCheckNumBtn.GetComponent<Image>().raycastTarget = false;
        int i = 61;
        while (i > 0)
        {
            yield return new WaitForSeconds(1);
            i--;
            checkChildTwo.GetComponent<Text>().text = i.ToString();
        }
        getCheckNumBtn.GetComponent<Button>().enabled = true;
        checkChildOne.SetActive(true);
        checkChildTwo.SetActive(false);
        getCheckNumBtn.GetComponent<Image>().raycastTarget = true;
    }
}
