using Common;
using DNotificationCenterManager;
using MsgContainer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
public enum EGetType
{

    ENull = 0,
    EGet = 1,
    EOneKeyGet = 2
}
public class EmailManager : SingleTon<EmailManager>
{

    #region 未读邮件列表

    public List<EmailData> noReadList;
    public List<EmailData> mNoReadList
    {
        get
        {
            return noReadList;
        }
    }

    public void UpdateNoReadList(List<EmailData> list)
    {
        if (noReadList == null)
        {
            noReadList = new List<EmailData>();
        }

        noReadList = list;
    }
    public void ClearNoReadList()
    {
        if (noReadList != null)
        {
            noReadList.Clear();
        }
    }

    public void RemoveNoReadItem(string id)
    {
        EmailData data = null;
        IEnumerator<EmailData> iEmails = noReadList.GetEnumerator();
        while (iEmails.MoveNext())
        {
            if (iEmails.Current.emailId == id)
            {
                data = iEmails.Current;
            }
        }
        if (data != null)
        {
            noReadList.Remove(data);
        }
    }
    #endregion

    #region 已读邮件列表

    //public List<EmailData> readedList;
    //public List<EmailData> mReadedList
    //{
    //    get
    //    {
    //        return readedList;
    //    }
    //}
    //public void InitReadedList()
    //{
    //    if (readedList == null)
    //    {
    //        readedList = new List<EmailData>();
    //    }
    //}
    //public void UpdateReadedList(List<EmailData> list)
    //{
    //    if (readedList == null)
    //    {
    //        readedList = new List<EmailData>();
    //    }

    //    readedList = list;
    //}
    #endregion

    /// <summary>
    /// 是否含有未读邮件
    /// </summary>
    public bool IsHasNoReadedEmail
    {
        get
        {
            return noReadList.Count > 0;
        }

    }


    /// <summary>
    /// id和 item列表
    /// </summary>
    private Dictionary<string, GameObject> emailItemDic = null;
    public Dictionary<string, GameObject> mEmailItemDic
    {
        get
        {
            emailItemDic = emailItemDic ?? new Dictionary<string, GameObject>();
            return emailItemDic;
        }
        set { emailItemDic = value; }
    }
    public void ClearEmailItemDic()
    {
        if (mEmailItemDic != null)
        {
            mEmailItemDic.Clear();
        }
    }
    public void AddEmailItemToDic(string key, GameObject item)
    {
        if (emailItemDic == null)
        {
            emailItemDic = new Dictionary<string, GameObject>();
        }
        if (!emailItemDic.ContainsKey(key))
        {
            emailItemDic.Add(key, item);
        }
        else
        {
            emailItemDic[key] = item;
        }
    }
    public void RemoverEmailItemFromDic(string key)
    {
        if (emailItemDic.ContainsKey(key))
        {
            emailItemDic.Remove(key);
        }
        else
        {
            Debug.Log("不存在这个邮件的id");
        }
        if (emailItemDic.Count == 0)
        {
            //刷新清除红点
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ERefreshEmailBall);

        }
    }
    /// <summary>
    /// 动画
    /// </summary>
    /// 
    public EGetType curGetType = EGetType.EGet;
    public EGetType CurGetType
    {
        get { return curGetType; }
        set { curGetType = value; }
    }
    int rank = 0;//item中的第几个动画
    bool isOver = true;//针对一键领取，默认表示非一键领取
    /// <summary>
    /// 动画播放
    /// </summary>
    /// <param name="emailData"></param>
    /// <param name="parent"></param>
    public void PlayTween(ref EmailData emailData, Transform parent)
    {
        GameObject emilTweenPre = ResourceManager.Instance.LoadAsset<GameObject>(UIType.EmailTween.Path);
        rank++;

        int rcNum = int.Parse(emailData.rcNum = emailData.rcNum ?? "0");
        int goldNum = int.Parse(emailData.goldNum = emailData.goldNum ?? "0");

        if (rank == 1)
        {
            if (rcNum <= 0) { rank++; }//没有房卡直接进入第二个动画
            if (goldNum <= 0) { rank++; }//没有金币直接结束
        }
        if (rank == 3)
        {
            if (curGetType == EGetType.EOneKeyGet)
            {
                OneEmailDataOver(emailData, parent);//rank归于0  移除已读从dic和list中   
                OneKeyGet(parent);//先OneEmailDataOver 
            }
            else
            {
                OneEmailDataOver(emailData, parent);//rank归于0  移除已读从dic和list中   
            }
        }
        else if (rcNum > 0)
        {
            if (rank == 1)
            {
                GameObject emailTween = SpawnEmailGameObject(emilTweenPre, parent);
                emailTween.GetComponent<UIEmailTween>().SetInitData(rcNum, EEmailType.ERoomCard, emailData, PlayTween);
                emailData.rcNum = 0.ToString();
            }
            //当前的item里面的数据结束
            else if (rank == 2)
            {
                GameObject emailTween = SpawnEmailGameObject(emilTweenPre, parent);
                emailTween.GetComponent<UIEmailTween>().SetInitData(rcNum, EEmailType.ERoomCard, emailData, null);
                emailData.rcNum = 0.ToString();
                OneEmailDataOver(emailData, parent);//rank归于0  移除已读从dic和list中
            }
        }
        else if (goldNum > 0 && rank == 2)
        {
            GameObject tween = SpawnEmailGameObject(emilTweenPre, parent);
            tween.GetComponent<UIEmailTween>().SetInitData(goldNum, EEmailType.EGoldCoin, emailData, null);
            emailData.goldNum = 0.ToString();
            OneEmailDataOver(emailData, parent);
        }
    }
    GameObject SpawnEmailGameObject(GameObject emilTweenPre, Transform parent)
    {
        GameObject tween = GameObject.Instantiate(emilTweenPre, parent);
        tween.transform.SetParent(parent, false);
        tween.transform.localPosition = Vector3.zero;
        tween.transform.localScale = Vector3.one;
        return tween;
    }
    void OneEmailDataOver(EmailData emailData, Transform parent)
    {
        rank = 0;
        EmailManager.Instance.RemoverEmailItemFromDic(emailData.emailId);
        EmailManager.Instance.RemoveNoReadItem(emailData.emailId);
        //放在这里上一个动画没有播放完毕就会进入下一个，同时出现两个动画
        //if (curGetType == EGetType.EOneKeyGet)
        //{
        //    OneKeyGet(parent);
        //}
    }
    public void OneKeyGet(Transform parent)
    {
        if (EmailManager.Instance.noReadList.Count <= 0)
        {
            UIManagers.Instance.EnqueueTip("当前没有可以领取的邮件了!");
            return;
        }
        curGetType = EGetType.EOneKeyGet;
        //没有领取的，开始领取

        string nowLastEmailId = noReadList[noReadList.Count - 1].emailId;
        UIEmailItem emailItem = emailItemDic[nowLastEmailId].GetComponent<UIEmailItem>();
        emailItem.SendReceiveEmail();
        //emailItem.Test();
    }

}

