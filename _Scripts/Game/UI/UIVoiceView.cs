using HaoyunFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIVoiceView : BasesView
{
    [SerializeField]
    Text  tipLab;
    [SerializeField]
    GameObject textureTween;
    [SerializeField]
    Image tiaoSpr;
    [SerializeField]
    private float cancelTabDistance = 0.5f;//取消录音的距离
    bool endVoice = false;
    float tabsTime = 0;//已经录音的时间 
    float minTabsTime = 1f;//录音最短时间限制
    float maxTabsTime = 10.0f;//录音最长时间限制

    bool isCancelTape = false;//没有取消
    bool isHandMoveUp = false;//手指上滑
    void Start()
    {
        endVoice = false;
        Recording();
    }
    //更新
    private void Update()
    {
        //if (Input.GetAxis("Mouse Y") >= cancelTabDistance || isHandMoveUp)
        //{
        //    isHandMoveUp = true;
        //    tipLab.text = "松手 取消";
        //    PauseTape();
        //    if (Input.GetMouseButtonUp(0))
        //        CancelTheTape();
        //    else if (Input.GetAxis("Mouse Y") <= -cancelTabDistance)
        //    {
        //        isHandMoveUp = false;
        //        tipLab.text = "上滑 取消";
        //        //ContinueTape();
        //    }
        //}
        if (!endVoice)
        {
            tabsTime += Time.deltaTime;
            if (tabsTime > maxTabsTime)
            {
                float percent = tabsTime / maxTabsTime;
                tiaoSpr.fillAmount = 1;
                EndVoice();
            }
            else
            {
                float percent = tabsTime / maxTabsTime;
                //Debug.Log("百分比："+percent);
                tiaoSpr.fillAmount = percent;
            }
        }
    }
    /// <summary>
    /// 暂停录音
    /// </summary>
    public void PauseTape()
    {

    }
    /// <summary>
    /// 继续录音
    /// </summary>
    public void ContinueTape()
    {

    }
    /// <summary>
    /// 录音中
    /// </summary>
    public void Recording()
    {
        SDKManager.Instance.yayaStart();
    }
    /// <summary>
    /// 取消录音
    /// </summary>
    public void CancelTheTape()
    {
        tipLab.text = "取消录音";
        for (int i = 0; i < textureTween.transform.childCount; i++)
        {
            textureTween.transform.GetChild(i).gameObject.SetActive(true);
        }
        StartCoroutine(CancelTabs());
        SDKManager.Instance.yayaStop(false, 0);
    }
    /// <summary>
    /// 处理录音
    /// </summary>
    void ExecuteTabs()
    {
        tipLab.text = "处理录音中...";
        textureTween.GetComponent<Animator>().enabled = false;
        StartCoroutine(CancelTabs());

        SDKManager.Instance.yayaStop(true, (uint)tabsTime + 1);

    }

    public void EndVoice()
    {
        textureTween.GetComponent<Animator>().enabled = false;
        if (tabsTime > minTabsTime)
        {
            //处理录音          
            ExecuteTabs();
        }
        else
        {
            UIManagers.Instance.EnqueueTip("录音时间太短小于1秒");
            Destroy(this.gameObject);
        }
        textureTween.GetComponent<Animator>().enabled = false;
        endVoice = true;
    }
    IEnumerator CancelTabs()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
