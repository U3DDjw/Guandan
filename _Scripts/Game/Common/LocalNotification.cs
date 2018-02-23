using UnityEngine;
using System.Collections;
using System;

public class LocalNotification {

    /// <summary>
    /// 发送者
    /// </summary>
    public GameObject sender;

    /// <summary>
    /// 消息内容
    /// </summary>
    public EventArgs param;

    /// <summary>
    /// 构造函数 （初始化）
    /// </summary>
    ///<param name="sender">通知发送者
    ///<param name="param">通知内容
    public LocalNotification(GameObject sender, EventArgs param)
    {
        this.sender = sender;
        this.param = param;
    }

    public LocalNotification()
    {

    }

    /// <summary>
    /// 构造函数
    /// </summary>
    ///<param name="param">
    public LocalNotification(EventArgs param)
    {
        this.sender = null;
        this.param = param;
    }
}


