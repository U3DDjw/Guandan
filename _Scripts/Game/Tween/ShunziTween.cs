using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ShunziTween : MonoBehaviour {

    [SerializeField]
    Image light;
    [SerializeField]
    Transform maskBg;
    
	
    void Start()
    {
        //Init Data
        // Vector3 initPos = new Vector3(250, 0, 0);
        light.color = new Color(1, 1, 1, 0);
        float tweenTime = 1f;
        Sequence mySequence = DOTween.Sequence();
        //Play
        mySequence.Append(maskBg.DOLocalMoveX(0, 1)).SetEase(Ease.OutCubic);
        mySequence.Join(light.transform.DOLocalMoveX(0, 1)).SetEase(Ease.OutCubic);
        mySequence.Join(DOTween.ToAlpha(
            () => light.color, x => light.color = x, 1, tweenTime * 2)
            );
        mySequence.Append(DOTween.ToAlpha(
           () => light.color, x => light.color = x, 0, 0.5f*tweenTime)
           ).SetEase(Ease.InQuart);
        // mySequence.Append(.// (Vector3.one, tweenTime * 4)).SetEase(Ease.InOutQuad);


        //mySequence.Append(lightLeft.transform.DOScale(Vector3.one, tweenTime * 3)).SetEase(Ease.OutQuad);
        //mySequence.Join(DOTween.ToAlpha(
        //    () => lightLeft.color, x => lightLeft.color = x, 1, tweenTime * 3)
        //    );
        mySequence.AppendCallback(
            delegate()
            {
                Destroy(this.gameObject);
            }
            );
    }
}
