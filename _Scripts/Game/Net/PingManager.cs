
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PingManager : MonoBehaviour {

    [SerializeField]
     Text pingLab;

    private  UnityAction<float> s_callback = null;
    private  UnityAction<float> callback;
    private  int s_timeout = 5;


    private void Awake()
    {
        callback += ShowPing;
    }
    public  void CreatePing(string ip, UnityAction<float> callback)
    {
        if (string.IsNullOrEmpty(ip)) return;
        if (callback == null) return;
        s_callback = callback;
    }

    /// <summary>
    /// 超时时间（单位秒）
    /// </summary>
    public  int Timeout
    {
        set
        {
            if (value > 0)
            {
                s_timeout = value;
            }
        }
        get { return s_timeout; }
    }


     void SetLabColor(float delay)
    {

        if(delay == -1)
        {
            pingLab.color = Color.gray;
            return;
        }
        if (delay < 90)
        {
            pingLab.color = Color.green;
        }
        else if (delay < 180)
        {
            pingLab.color = Color.yellow;
        }
        else 
        {
            pingLab.color = Color.red;
        }
    }

    public void ShowPing(float delay)
    {
        SetLabColor(delay);
        if (delay == -1)
        {
            //离线重新登录，大退
            pingLab.text = "已断线";
        }
        else
        {
            pingLab.text = delay.ToString() + "ms";
        }
    }


private void Start()
    {
        if (PlayerInfo.Instance.mPlayerData == null) { return; }
     
        CreatePing(PlayerInfo.Instance.mPlayerData.ip,callback);
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaCarrierDataNetwork: // 3G/4G
            case NetworkReachability.ReachableViaLocalAreaNetwork: // WIFI
                {
                    StartCoroutine(this.PingConnect());
                }
                break;
            case NetworkReachability.NotReachable: // 网络不可用
            default:
                {
                    if (s_callback != null)
                    {
                        s_callback(s_timeout*1000);
                    }
                }
                break;
        }
    }

    private void OnDestroy()
    {
        s_timeout = 5;
        s_callback = null;
        StopCoroutine(this.PingConnect());
    }

    IEnumerator PingConnect()
    {
        // Ping網站 
        Ping ping = new Ping(PlayerInfo.Instance.GetIp);
        // 等待请求返回
        while (!ping.isDone)
        {
            yield return new WaitForSeconds(0.02f);

            // 链接失败
            if (ping.time  ==-1)
            {
                ShowPing(-1);
             //   yield break;
            }
        }

        // 链接成功
        if (ping.isDone)
        {
            ShowPing(ping.time);
            yield return null;
        }
        yield return new WaitForSeconds(2.0f); //2s刷新一次
        StartCoroutine("PingConnect");
    }
}
