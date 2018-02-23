using DNotificationCenterManager;
using Haoyun.Utils;
using MsgContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorseController : MonoBehaviour
{

    //[SerializeField]
    //TweenPosition horseTween;

    [SerializeField]
    Text content;

    public long curStopTime;
    public float perWaveInteval; // 每波间隔
    public float intervalTime;   // 每个跑马灯间隔秒数
    public int perWaveNumber;  // 每波次数

    public int waveTimes = 0; //第几波

    //List<EventDelegate> delegateList;

    private void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EHorseRefresh, ChangeHorse);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EHorseRefresh, ChangeHorse);
    }

    // Use this for initialization
    void Start()
    {
        if (GameManager.Instance.mCurGameStatus == EGameStatus.ELogin) //登录界面不展示跑马灯
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }

        RequestHorseData();
        //delegateList = horseTween.onFinished;
        //EventDelegate.Add(horseTween.onFinished, TweenCallBack);
    }

    void ChangeHorse(LocalNotification e)
    {
        ChangeHorseTween();
    }

    void ChangeHorseTween()
    {
        var t = PlayerInfo.Instance.GetCurHorseData();
        if (t == null)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        this.transform.GetChild(0).gameObject.SetActive(true);
        content.text = t.content;
        curStopTime = long.Parse(t.ineffectTime);
        perWaveInteval = t.perWaveInteval;
        intervalTime = t.intervalTime;

        //应用于实际动画
        //if (waveTimes == 0)
        //{
        //    horseTween.delay = 0;
        //}
        //horseTween.delay = intervalTime - horseTween.duration;//每个跑马灯间隔秒数
    }

    void RequestHorseData()
    {
        if (PlayerInfo.Instance.mHorseDataList == null) //数据只请求一次
        {
            WWWForm form = new WWWForm();
            form.AddField("pid", PlayerInfo.Instance.mPlayerPid.ToString());
            form.AddField("time", TimeUtils.ConvertToTime(System.DateTime.Now).ToString()); //当前时间戳
            form.AddField("sig", "tobeadded");
            string url = GlobalData.mConstBaseServerUrl + MsgContainer.ServerUrlTitle.Url_GetHorse;
            StartCoroutine(GlobalData.SendPost(url, form, delegate (WWW www)
            {
                Debug.Log("跑马灯数据列表获取成功:" + www.text);
                var list = JsonManager.GetHourseData(www.text);
                {
                    PlayerInfo.Instance.UpdateHorseData(list);
                }
                ChangeHorseTween();
            }));
        }
        else
        {
            ChangeHorseTween();
        }
    }

    void TweenCallBack()
    {
        waveTimes++;

        CheckPerWaveOver();
        CheckInEffect();
    }


    /// <summary>
    ///检测一波是否播放完
    /// </summary>
    void CheckPerWaveOver()
    {
        if (waveTimes == perWaveNumber) // 播放完一波（多少遍）的动画
        {
            waveTimes = 0;
            //用每波间隔
            Invoke("ChangeHorseTween", perWaveInteval);

        }
    }
    /// <summary>
    /// 检测当前跑马灯是否到期，失效
    /// </summary>
    void CheckInEffect()
    {
        long curUnixTime = TimeUtils.ConvertToTime(System.DateTime.Now);
        if (curUnixTime >= curStopTime)
        {

        }
    }

}
