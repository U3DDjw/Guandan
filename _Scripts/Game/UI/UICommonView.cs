using UnityEngine;
using System.Collections;
using DG.Tweening;
using HaoyunFramework;
using UnityEngine.UI;

public class CommonContext : BaseContext
{
    public CommonContext()
    {
        ViewType = UIType.CommonView;
    }
}
public class UICommonView : BasesView
{
    [SerializeField]
    Text tipLab;

    [SerializeField]
    Image sprBg;

    [SerializeField]
    GameObject loadingContainer;


    int offset = 200;

    int targetPosY = 367;
    float tipBgHeight = 49;
    public float sprWidthSpr = 0;


    private void Start()
    {
      //  loadingContainer.transform.DOLocalRotate(new Vector3(0, 0, 180), 1).SetLoops(-1);
        CloseLoadingSpr();
    }

    private void OnDestroy()
    {
        Debug.Log("CommonView Destroy");
    }
    IEnumerator WaitOneDelta()
    {
        yield return null; 
        sprBg.rectTransform.sizeDelta = new Vector2(tipLab.rectTransform.rect.width + offset, 49);
    }
    public void PlayTipsTween()
    {
        StartCoroutine(WaitOneDelta());

        float tweenTime = 0.3f;
        float intervalTime = 1.5f;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(sprBg.transform.DOLocalMove(sprBg.transform.localPosition + new Vector3(0, -50, 0), tweenTime));
        mySequence.Join(DOTween.ToAlpha(
            () => sprBg.color, x => sprBg.color = x, 1, tweenTime)
            );
        mySequence.AppendInterval(intervalTime);

        mySequence.Append(sprBg.transform.DOLocalMove(sprBg.transform.localPosition, tweenTime));
        mySequence.Join(DOTween.ToAlpha(
            () => sprBg.color, x => sprBg.color = x, 0, tweenTime)
            );
        mySequence.AppendCallback(TweenCallBack);
        mySequence.Play();
    }
    bool isPlayingTween = false;
    private void Update()
    {
        if (UIManagers.Instance.mTipsQueue.Count > 0 && !isPlayingTween)
        {
            isPlayingTween = true;
            tipLab.text = UIManagers.Instance.PeekTip();
            PlayTipsTween();
        }

		LoadingSprRotate ();
        //if (Input.GetMouseButtonDown(0))
        //{
        //    PlayTipsTween();
        //}

        //Test=====================

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    UIManagers.Instance.EnqueueTip("Test");
        //}

        //Test=====================
    }

	float rotateSpeed = 2.0f;
	void LoadingSprRotate()
	{
		loadingContainer.transform.Rotate (Vector3.forward * rotateSpeed);
	}




    void TweenCallBack()
    {
        UIManagers.Instance.DequeueTip();
        isPlayingTween = false;
    }


    /// <summary>
    /// 打开或关闭loading圈
    /// </summary>
    /// <param name="isOpen"></param>
    public void SwitchLoadingSpr(bool isOpen)
    {
        if(isOpen)
        {
            ShowLoadingSpr();
         //   loadingContainer.transform.DOLocalRotate(new Vector3(0, 0, -180), 1).SetLoops(-1);
        }
        else
        {
            CloseLoadingSpr();
        }
    }

    void ShowLoadingSpr()
    {
        loadingContainer.gameObject.SetActive(true);
        //  DOTween.To(() => loadingContainer.transform.localRotation, r => loadingContainer.transform.localRotation = r, new Vector3(5, 5, 5), 1);
    }

    void CloseLoadingSpr()
    {
        loadingContainer.gameObject.SetActive(false);
    }


}
