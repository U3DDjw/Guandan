using HaoyunFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordBureauContext : BaseContext
{
    public RecordBureauContext()
    {
        ViewType = UIType.RecordBureauView;
    }
}
public class RecordBureauView : BasesView
{
    [SerializeField]
    Text  roomInfo;
    [SerializeField]
    Text  playInfo;
    [SerializeField]
    Text  recordInfo;
    [SerializeField]
    Image maskBg;
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(maskBg.gameObject).onClick = OnCloseClick;
    }
    public void SetData(string roomInfo, string playerInfo, string recordInfo)
    {
        this.roomInfo.text = roomInfo;
        this.playInfo.text = playerInfo;
        this.recordInfo.text = recordInfo;
    }
    void OnCloseClick(GameObject g)
    {
        RecordManager.Instance.ToZero();
        //ContextManager.Instance.Pop();
        UIManagers.Instance.DestroySingleUI(UIType.RecordBureauView);
    }
}
