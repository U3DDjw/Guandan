using UnityEngine;
using System.Collections;
using DNotificationCenterManager;
using MsgContainer;
using UnityEngine.UI;
using HaoyunFramework;

public class LoadingConext : BaseContext
{
    public LoadingConext()
    {
        ViewType = UIType.LoadingView;
    }
}

public class UILoadingView : BasesView
{
    [SerializeField]
    Text FillerText; //Lab显示进度，仿照王者荣耀更新资源包显示
    [SerializeField]
    Text tipText;
    [SerializeField]
    Text progressText;
    [SerializeField]
    Slider handle;
    void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDownLoadProgress, UpdateDownLoad);
    }
    void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDownLoadProgress, UpdateDownLoad);
    }

    void ResetToOverStatus()
    {
        Debug.Log("Over LoginView");
        if (XPlayerPrefs.Instance.mFristLoad == 1)
        {
            XPlayerPrefs.Instance.mFristLoad = 0;
        }
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(2).gameObject.SetActive(false);
    }


    /// <summary>
    /// 通过DownLoad中 算是总的KB;根据每个包的KB *progress ==>>最终进度
    /// </summary>
    /// <param name="e"></param>
    void UpdateDownLoad(LocalNotification e)
    {
        ArgsDownLoadProgress msg = e.param as ArgsDownLoadProgress;
        if (msg != null)
        {
            UpdateProgress(msg.total, msg.progress);
        }
    }

    void UpdateProgress(float totalKB, float progress)
    {
        curProgress = progress;
        this.totalKB = totalKB;
        if (DownLoadManager.downLoadTime > 0)
        {
            if (DownLoadManager.downLoadTime > 1)
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(2).gameObject.SetActive(true);
            }             
            SetProgressData(progress);
        }     
        if (progress == 1)
        {           
           
            if (DownLoadManager.downLoadTime <= 1f && DownLoadManager.downLoadTime != 0)
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(2).gameObject.SetActive(true);        
                StartCoroutine(FakeSlider());
            }
            else
            {
                ResetToOverStatus();
            }
           
        }
    }
    IEnumerator FakeSlider()
    {
        FillerText.gameObject.SetActive(false);
        handle.value = 0;
        for (int i = 0; i <20; i++)
        {
            handle.value += 0.05f;
            progressText.text = (int)(handle.value/0.01f) + "%";
            yield return new WaitForSeconds(0.04f);

        }
        ResetToOverStatus();

    }
    string CovertSpeedKBOrMB(float size)
    {
        string curSpeed = "";
        if (size > 1024)
        {
            curSpeed = string.Format("{0:F}MB", size / 1024);
        }
        else
        {
            curSpeed = string.Format("{0:F}KB", size); ;
        }
        return curSpeed;
    }
    void SetProgressData(float t)
    {
        string downData = string.Format("{0}/s\t\t{1}/{2}", CovertSpeedKBOrMB(speed), CovertSpeedKBOrMB(t * totalKB), CovertSpeedKBOrMB(totalKB));
        progressText.text = string.Format("{0:F1}%", t * totalKB / totalKB * 100.0f);
        FillerText.text = downData;
        handle.value = t;
    }
    // Use this for initialization
    void Start()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(2).gameObject.SetActive(false);
        if (XPlayerPrefs.Instance.mFristLoad == 1)
        {
            tipText.enabled = true;
        }
        else
        {
            tipText.enabled = false;
        }
        UIManagers.Instance.RefreshParentCanvas();

        if (GameManager.Instance.mIsBackToLoginModule) //返回登录
        {
            ResetToOverStatus();
            GameManager.Instance.SwitchGameStatus(EGameStatus.ELogin);
        }
        else
        {
            if (GameManager.Instance.mIsHotUpdate) //线上都是开启热更的，这边是正常流程
            {
                this.gameObject.AddComponent<DownLoadManager>();
                StartCoroutine(GetDownSpeed());
            }
            else
            {
                XLuaInstance.Instance.HotStartFixing();
                ResetToOverStatus();
                GameManager.Instance.SwitchGameStatus(EGameStatus.ELogin);
            }
        }
    }

    float curProgress = 0;
    float lastProgress = 0;
    float totalKB = 0;
    float speed = 0;
    IEnumerator GetDownSpeed()
    {
        while (curProgress < 1)
        {
            yield return new WaitForSeconds(1.0f);
            speed = (curProgress - lastProgress) * totalKB;
            Debug.Log("speed:" + speed);
            lastProgress = curProgress;
        }
    }

}
