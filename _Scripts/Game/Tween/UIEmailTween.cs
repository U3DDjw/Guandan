using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using HaoyunFramework;
public class EmailTweenContext : BaseContext
{
    public EmailTweenContext()
    {
        ViewType = UIType.EmailTween;
    }
}
public class UIEmailTween : BasesView
{
    [SerializeField]
    RawImage lightTex;
    [SerializeField]
    Image iconSpr;
    [SerializeField]
    Image xSpr;//乘号的图片

    [SerializeField]
    Text fonTipLab;
    [SerializeField]
    Text fontNumLab;

    [SerializeField]
    Image maskBg;

    bool isLightRoat = false;
    EmailData emailData = null;
    public delegate void ActionNextTween(ref EmailData data, Transform gm);
    public ActionNextTween nextTween;
    private void Start()
    {
        EventTriggerListener.Get(maskBg.gameObject).onClick = OnCloseClick;
        maskBg.rectTransform.sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
    }
    void Init()
    {

        fonTipLab.gameObject.SetActive(false);
        xSpr.gameObject.SetActive(false);
        fontNumLab.gameObject.SetActive(false);
        lightTex.gameObject.SetActive(false);
        iconSpr.SetNativeSize();
        iconSpr.transform.localScale = Vector3.zero;
    }
    private void Update()
    {
        if (isLightRoat)
        {
            lightTex.transform.Rotate(Vector3.forward, 2);
        }
    }

    public void PlayTween()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(iconSpr.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBounce));
        mySequence.AppendCallback(delegate
        {
            fonTipLab.gameObject.SetActive(true);
            xSpr.gameObject.SetActive(true);
            fontNumLab.gameObject.SetActive(true);
            lightTex.gameObject.SetActive(true);
            isLightRoat = true;
        });
        mySequence.AppendInterval(2.0f);
        mySequence.AppendCallback(delegate
        {
            OnCloseClick(null);
        });
    }
    public void SetInitData(int num, EEmailType emailType, EmailData emailData = null, ActionNextTween nextTween = null)
    {
        this.emailData = emailData;

        this.nextTween = nextTween;
        if (emailType == EEmailType.ERoomCard)
        {
            iconSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "mail_card");
            fontNumLab.text = num.ToString();
        }
        else if (emailType == EEmailType.EGoldCoin)
        {
            iconSpr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EMain, "mail_coin");
            fontNumLab.text = num.ToString();
        }
        Init();
        PlayTween();
    }
    void OnCloseClick(GameObject g)
    {
        if (this.nextTween != null)
        {
            this.nextTween(ref emailData, this.transform.parent);
        }
        else
        {
            if (EmailManager.Instance.curGetType == EGetType.EOneKeyGet)
            {
                EmailManager.Instance.OneKeyGet(this.transform.parent);
            }
        }
        Destroy(this.gameObject);
    }
}
