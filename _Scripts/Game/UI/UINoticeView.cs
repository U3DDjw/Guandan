using UnityEngine;
using System.Collections;
using Haoyun.Utils;
using MsgContainer;
using System.Collections.Generic;
using HaoyunFramework;
using UnityEngine.UI;

public class NoticContext : BaseContext
{
    public NoticContext()
    {
        ViewType = UIType.NoticeView;
    }
}
public class UINoticeView : BasesView
{

    //[SerializeField]
    //Text contentLab;
    //[SerializeField]
    //Text titleLab;
    [SerializeField]
    GameObject maskBg;
    [SerializeField]
    GameObject closeBtn;
    [SerializeField]
    RawImage noticTexture;

    private void Start()
    {
        EventTriggerListener.Get(closeBtn).onClick = OnCloseClick;
        EventTriggerListener.Get(maskBg).onClick = OnCloseClick;
        EventTriggerListener.Get(noticTexture.gameObject).onClick = OnNoticeTexClick;
    }
    public void OnCloseClick(GameObject gm)
    {
        ContextManager.Instance.Pop(curContext);
    }

    void OnNoticeTexClick(GameObject gm)
    {
        AudioManager.Instance.PlayClickBtnAudio();
        if (Application.platform != RuntimePlatform.WindowsEditor)
            SDKManager.Instance.WeChatShareNativeImage();
    }
    
    public void LoadTexture(Texture text)
    {
       // noticTexture.texture = text;
    }

}
