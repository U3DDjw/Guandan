using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WinStarTween : MonoBehaviour
{
    public void Start()
    {
        this.transform.localScale = Vector3.zero;
        float ranEnd = 0.0f;
        if (Random.Range(0, 10) > 7)
        {
            ranEnd = Random.Range(0.5f, 1f);
        }
        else
        {
            ranEnd = Random.Range(0.2f, 0.5f);
        }
        Image starSpr = this.GetComponent<Image>();

        Sequence se = DOTween.Sequence();
        se.AppendInterval(0.3f);
        se.Append(starSpr.transform.DOScale(ranEnd, 0.3f)).SetEase(Ease.OutBounce);
        se.Play();
    }
    //备用 便于随机生成
    public void SetData(float invterTime)
    {
        this.transform.localScale = Vector3.zero;
        float ranEnd = Random.Range(0.2f, 1f);
        Image starSpr = this.GetComponent<Image>();

        Sequence se = DOTween.Sequence();
        se.AppendInterval(invterTime);
        se.Append(starSpr.transform.DOScale(ranEnd, 0.3f)).SetEase(Ease.OutBounce);
        se.Play();
    }

}
