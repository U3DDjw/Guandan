using MsgContainer;
using Net;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZhiWa;
using HaoyunFramework;
using UnityEngine.UI;

public class EmoticonViewContext : BaseContext
{
    public EmoticonViewContext()
    {
        ViewType = UIType.EmoticonView;
    }
}
public class UIEmoticonView : BasesView
{
    [SerializeField]
    GameObject emoticonContainer;
    //[SerializeField]
    //GameObject oftenContainer;//常用语容器
    [SerializeField]
    InputField showTextLab;
    [SerializeField]
    Image sendBtn;
    [SerializeField]
    GameObject maskBg;

    List<Transform> listEmoticon;
    List<Transform> listOften;

    string audioName = "mcj";
    void Start()
    {
        EventTriggerListener.Get(sendBtn.gameObject).onClick = OnSendClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
        InitEmoticon();
        //InitOften();
    }
    void InitEmoticon()
    {
        int needCount = 12;
        int nowCount = emoticonContainer.transform.childCount;
        Transform emoticonGm = emoticonContainer.transform.GetChild(0);
        EventTriggerListener.Get(emoticonGm.gameObject).onClick = OnEmoticonClick;
        listEmoticon = new List<Transform>();
        listEmoticon.Add(emoticonGm);
        while (listEmoticon.Count < needCount)
        {
            Transform newEmot = GameObject.Instantiate(emoticonGm);
            newEmot.GetComponent<Image>().sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "news_expression_" + (listEmoticon.Count + 1));
            newEmot.SetParent(emoticonContainer.transform);
            newEmot.localPosition = Vector3.zero;
            newEmot.localScale = Vector3.one;
            EventTriggerListener.Get(newEmot.gameObject).onClick = OnEmoticonClick;
            listEmoticon.Add(newEmot);
        }
    }
    //常用语
    //void InitOften()
    //{
    //    int needLength = GlobalData.oftenStrings.Length;
    //    Transform oftenGm = oftenContainer.transform.GetChild(0);
    //    EventTriggerListener.Get(oftenGm.gameObject).onClick = OnOftenClick;
    //    listOften = new List<Transform>();
    //    oftenGm.GetComponent<Text>().text = GlobalData.oftenStrings[0];
    //    listOften.Add(oftenGm);
    //    while (listOften.Count < needLength)
    //    {
    //        Transform newOften = GameObject.Instantiate(oftenGm);
    //        newOften.parent = oftenContainer.transform;
    //        newOften.localScale = Vector3.one;
    //        newOften.GetComponent<Text>().text = GlobalData.oftenStrings[listOften.Count];
    //        EventTriggerListener.Get(newOften.gameObject).onClick = OnOftenClick;
    //        listOften.Add(newOften);
    //    }
    //}
    void OnEmoticonClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (!listEmoticon.Contains(g.transform))
        {
            return;
        }
        int index = listEmoticon.IndexOf(g.transform);
        //发送数据到服务器
        RequestEmticonInfo((MsgEmoticon)(index + 1));
    }

    void OnOftenClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (!listOften.Contains(g.transform))
        {
            return;
        }
        int index = listOften.IndexOf(g.transform);
        showTextLab.text = GlobalData.mGetUsefulExpressions()[index];
        OnSendClick(null);
    }
    void OnSendClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (showTextLab.text.Length != 0)
        {
            RequestEmticonInfo(0, showTextLab.text);
        }
        else
        {
            UIManagers.Instance.EnqueueTip("发送文本内容为空！");
        }
    }
    public void OnCloseClick(GameObject g)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        maskBg.GetComponent<Image>().raycastTarget = false;
        this.GetComponent<ViewTween>().TweenPlayBack(delegate ()
        {
            ContextManager.Instance.Pop(curContext);
        });

    }
    //消息的发送等
    void RequestEmticonInfo(MsgEmoticon em = 0, string message = "")
    {
        MsgGlobal gl = new MsgGlobal();
        gl.emotion_info = new MsgEmotionInfo();
        var msg = gl.emotion_info;
        msg.action_id = PlayerInfo.Instance.mPlayerPid;
        msg.emoticonId = em;
        msg.message = message;
        TCPNetWork.GetInstance().SendMsgToServer(ServerMsgKey.CLIENT_USE_EMOTICON, gl);
        OnCloseClick(null);
    }
}
