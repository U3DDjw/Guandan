using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DNotificationCenterManager;

public enum TweenType
{
    Null = 0,
    RightToLeft = 1,
    InToOut = 2,
    RecordViewTween = 3//特殊
}
public class ViewTween : MonoBehaviour
{
    public TweenCallback callBackFun;
    [SerializeField]
    TweenType tweenType = TweenType.InToOut;
    [SerializeField]
    Transform bgTransform;//被操控的对象,存放View内容的
    Sequence mySeq = null;
    void Start()
    {
        ViewPlayTween(tweenType);
    }
    void ViewPlayTween(TweenType type)
    {
        switch (type)
        {
            case TweenType.RightToLeft:
                RightToLeft();
                break;
            case TweenType.InToOut:
                InToOut();
                break;
            case TweenType.RecordViewTween:
                RecordTween();
                break;
        }
    }
    /// <summary>
    /// 从右到左
    /// </summary>
    void RightToLeft()
    {
        mySeq = DOTween.Sequence();
        if (bgTransform == null)
        {
            bgTransform = this.transform.GetChild(0);
        }
        mySeq.Append(bgTransform.DOLocalMove(bgTransform.localPosition + new Vector3(-510, 0, 0), 0.5f)).SetEase(Ease.InCubic);
        mySeq.Play();
    }
    /// <summary>
    /// 从里到外
    /// </summary>
    void InToOut()
    {
        if (bgTransform == null)
        {
            bgTransform = this.transform.Find("Bg");
        }
        bgTransform.localScale = Vector3.zero;
        mySeq = DOTween.Sequence();
        mySeq.Append(bgTransform.DOScale(Vector3.one, 0.1f));
        if (callBackFun != null)
        {
            mySeq.AppendCallback(callBackFun);
        }
        mySeq.Play();
    }
    public void RecordTween()
    {
        if (bgTransform == null)
        {
            bgTransform = this.transform.Find("Bg");
        }
        bgTransform.localScale = Vector3.zero;
        GameObject bg = GameObject.Find("HomeView");
        if (bg == null) return;
        RawImage homeBg = bg.transform.Find("Bg").GetComponent<RawImage>();
        RawImage recordBg = this.transform.GetChild(0).GetComponent<RawImage>();
        Rect rect = recordBg.rectTransform.rect;
        rect.width = homeBg.rectTransform.rect.width;
        rect.height = homeBg.rectTransform.rect.height;
        mySeq = DOTween.Sequence();
        mySeq.Append(bgTransform.DOScale(Vector3.one, 0.1f));
        if (callBackFun != null)
        {
            mySeq.AppendCallback(callBackFun);
        }
        mySeq.Play();
    }
    public void TweenPlayBack(TweenCallback fun)
    {
        mySeq = DOTween.Sequence();
        if (bgTransform == null)
        {
            bgTransform = this.transform.GetChild(0);
        }
        mySeq.Append(bgTransform.DOLocalMove(bgTransform.localPosition + new Vector3(510, 0, 0), 0.5f)).SetEase(Ease.InCubic);
        mySeq.AppendCallback(fun);
        mySeq.Play();
    }

}
