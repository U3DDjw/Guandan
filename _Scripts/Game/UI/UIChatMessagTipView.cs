using HaoyunFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChatMessagTipView : BasesView
{
    [SerializeField]
    Text messageLab;
    [SerializeField]
    Image voiceTweenSpr;//有spr 有动画 有语音 三个组件
    //public void Start()
    //{
    //    StartCoroutine(WaitOneDelta("有动画 有语音 三个组件"));
    //}
    public IEnumerator WaitOneDelta(string text = null, string voiceName = null, EPlayerPositionType type = EPlayerPositionType.ELeft)
    {
        messageLab.text = text;
        yield return null;
        //获取其中的另一个对象
        Image bg = messageLab.transform.parent.GetComponent<Image>();
        float time = 3.0f;//3秒后销毁自己
        //emoticonSpr.gameObject.SetActive(emoticonname != null);
        messageLab.gameObject.SetActive(text != null && text.Length > 0);
        voiceTweenSpr.gameObject.SetActive(voiceName != null);
        Debug.Log("chat开始位置：" + transform.localPosition);
        //整体旋转
        if (type == EPlayerPositionType.ERight)
        {
            messageLab.transform.rotation = Quaternion.Euler(Vector3.up * 180);
            this.transform.rotation = Quaternion.Euler(Vector3.up * 180);
        }
        if (text != null)
        {
            //为了显示的效果，根据不同的位置进行调整             
            RectTransform rectTrans = bg.GetComponent<RectTransform>();
            rectTrans.sizeDelta = new Vector2(messageLab.rectTransform.rect.width + 50, 61);
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector3(messageLab.rectTransform.rect.width / 2 + 77, 30);
            Debug.Log("chat结束位置：" + transform.localPosition);
            if (type == EPlayerPositionType.ETop)
            {
                bg.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 10);
            }
        }
        StartCoroutine(DestroySelf(time, type));
    }
    IEnumerator DestroySelf(float time, EPlayerPositionType type)
    {
        yield return new WaitForSeconds(time);
        UIManagers.Instance.DestroySingleUI(UIType.ChatMessageTip);
    }
}
