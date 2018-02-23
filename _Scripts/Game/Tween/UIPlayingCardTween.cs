using UnityEngine;
using System.Collections;
using DG.Tweening;
using ZhiWa;
using System.Collections.Generic;
using MsgContainer;
using UnityEngine.UI;
using HaoyunFramework;

public class UIPlayingCardTween : MonoBehaviour
{
    [SerializeField]
    Image spr;
    [SerializeField]
    Image dui;//连对中的对字显示
    [SerializeField]
    RawImage bgTex;//背景
    [SerializeField]
    RawImage lineTex;//线

    float startWaitTime = 0.1f;//等待时间再播放动画
    float destWaitTime = 1.0f;//销毁等待时间
    Sequence se = null;
    private void Start()
    {
        this.transform.localPosition -= new Vector3(0, 24, 0);//牌的一半
    }
    public void InItGm()
    {
        dui.enabled = false;
        bgTex.enabled = false;
        lineTex.enabled = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">图片名称</param>
    /// <param name="isCardAtlas">是否切换为卡片的图集</param>
    void SetSprName(string name, bool isCardAtlas = false)
    {
        int atlas = (int)EAtlasType.EPlaying;
        if (isCardAtlas)
        {
            atlas = (int)EAtlasType.ECard;
        }
        spr.sprite = ResourceManager.Instance.GetSpriteByName(atlas, name);
        spr.SetNativeSize();
    }
    public void PlayKangGongTributeTween(EPlayerPositionType posType)
    {
        #region spr设置开始位置和结束位置
        float endValue = 0;
        bool isLeftOrRight = false;
        bool isSelfOrTop = false;

        Sequence mySequence = DOTween.Sequence();
        se = mySequence;
        SetSprName("card_0_17", true);
        if (posType == EPlayerPositionType.ETop)
        {
            spr.transform.localPosition = new Vector3(0, 130, 0);
            endValue = -100;
            isSelfOrTop = true;
        }
        else if (posType == EPlayerPositionType.ESelf)
        {
            spr.transform.localPosition = new Vector3(0, -200, 0);
            endValue = 0;
            isSelfOrTop = true;
        }
        else if (posType == EPlayerPositionType.ELeft)
        {
            spr.transform.localPosition = new Vector3(-300, 0);
            endValue = 100;
            isLeftOrRight = true;
        }
        else if (posType == EPlayerPositionType.ERight)
        {
            spr.transform.localPosition = new Vector3(300, 0);
            endValue = -100;
            isLeftOrRight = true;
        }
        #endregion
        #region -------------大王移动

        float duration = 0.25f;
        if (isLeftOrRight)
        {
            mySequence.Append(spr.transform.DOLocalMoveX(endValue, duration * 4f)).SetEase(Ease.OutQuad);
        }
        else if (isSelfOrTop)
        {
            mySequence.Append(spr.transform.DOLocalMoveY(endValue, duration * 4)).SetEase(Ease.OutQuad);
        }
        //抗贡字
        mySequence.Append(DOTween.ToAlpha(() => spr.color, x => spr.color = x, 0, 0.5f));
        mySequence.AppendCallback(
            delegate
            {
                SetSprName("pk_kanggong");
            });
        mySequence.Append(DOTween.ToAlpha(() => spr.color, x => spr.color = x, 1, 0.5f));
        mySequence.AppendInterval(0.7f);
        mySequence.AppendCallback(delegate { Destroy(this.gameObject); });

        mySequence.Play();
        #endregion

    }

    private void OnDestroy()
    {
        if (se != null)
            se.Kill();
    }
}
