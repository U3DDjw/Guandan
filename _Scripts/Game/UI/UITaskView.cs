using HaoyunFramework;
using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using ZhiWa;
using Net;
using DNotificationCenterManager;

public class TaskContext : BaseContext
{
    public TaskContext()
    {
        ViewType = UIType.TaskView;
    }
}
public class UITaskView : BasesView
{
    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    UniWebView webView;
    [SerializeField]
    GameObject anim;
    bool isLoadEnd = false;
    bool isAnimEnd = false;
    void Start()
    {
        EventTriggerListener.Get(closeBtn).onClick = OnCloseClick;
        InitTween();
    }
    void ShowWebView()
    {
        webView.alpha = 1;
        webView.Show(true, UniWebViewTransitionEdge.None, 0.1f);
    }
    void InitTween()
    {
        Sequence se = DOTween.Sequence();
        float animTime = TweenManager.Instance.GetAnimatorTime(anim);
        se.AppendInterval(animTime - 1.5f);
        Vector3 endPos = closeBtn.transform.localPosition - Vector3.up * 190;
        se.Append(closeBtn.transform.DOLocalMove(endPos, 0.5f).SetEase(Ease.OutBounce));
        se.AppendCallback(delegate ()
        {
            isAnimEnd = true;
            if (isLoadEnd)
            {
                print("动画播放完毕，加载完毕");
                ShowWebView();
            }
        });
        se.Play();
    }
    public void SetData(string url)
    {
        webView.url = url;
        Vector2 wAndH = GlobalData.GetRealWAndH(new Vector2(1920, 1080), new Vector2(100, 120));
        print("wAndH" + wAndH);
        Debug.Log(webView.autoShowWhenLoadComplete);
        webView.alpha = 0;
        webView.SetBackgroundColor(new Color(0, 0, 0, 0f));
        //webView.SetUseWideViewPort(true);//扩展页面到和市口一样大的，仅仅适用于安卓
        webView.autoShowWhenLoadComplete = false;
        webView.backButtonEnable = false;//返回按钮是否可以关闭网页
        webView.insets = new UniWebViewEdgeInsets((int)wAndH.y, (int)wAndH.x, (int)wAndH.y, (int)wAndH.x);
        webView.OnReceivedMessage += OnReceivedMessage;
        webView.OnLoadComplete += LoadComplete;
    }
    public void LoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        print("网页加载完毕");
        //加载成功了，再让webView显示出来
        if (success)
        {
            isLoadEnd = true;
            if (isAnimEnd)
            {
                Debug.Log("webView.url" + webView.url);
                print("加载完毕，动画完毕");
                ShowWebView();
            }
        }
        else
        {
            print("加载失败原因：" + errorMessage);
            webView.Reload();
        }
    }
    public void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        Debug.Log(message.path);
        // However, there is also a "rawMessage" property you could use if you need to use some other formats and want to parse it yourself.
        string str = string.Format("来自Js中{0}消细", message.path);
        // UIManagers.Instance.EnqueueTip(str);
        if (message.path == "close")
        {
            ContextManager.Instance.Pop(UIType.TaskView.Name);
        }
        else if (message.path == "createRoom")
        {
            CreateRoom();
            ContextManager.Instance.Pop(UIType.TaskView.Name);

        }
        else if (message.path == "share")
        {
            Share();
        }
    }
    void Share()
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (Application.platform == RuntimePlatform.WindowsEditor)//编辑状态点击没效果
        { return; }
        var playerId = PlayerInfo.Instance.mPlayerData.pid;
        var playerName = PlayerInfo.Instance.mPlayerData.name;
        string title = "玩" + GlobalData.mGameName + " 领现金红包";
        string descripition = playerName + " 邀请你一起来掼蛋!\r\n绑定ID: " + playerId;
        string url = GlobalData.WeChatShareUrl + "?pid=" + playerId;
        SDKManager.Instance.WeChatShareLink(title, descripition, url);
        //     SDKManager.Instance.();
    }
    public void CreateRoom()
    {
        //判断是否有房间
        if (RoomInfo.Instance.mIsExistWaitGameView)
        {
            RoomInfo.Instance.ExistWaitRoom();
            return;
        }
        ContextManager.Instance.Push(new CreateRoomContext());
        //GameObject createRoomPre = UIManagers.Instance.GetSingleUI(UIType.CreateRoomView);
        //UICreateRoomView createRoom = createRoomPre.GetComponent<UICreateRoomView>();
        //createRoom.OnCreateClick(createRoom.gameObject);
        //createRoomPre.SetActive(false);
    }

    public void OnCloseClick(GameObject g)
    {
        print("销毁活动面板");
        ContextManager.Instance.Pop(UIType.TaskView.Name);
    }
}
