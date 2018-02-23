using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;
using UnityEngine.UI;
using DG.Tweening;
using MsgContainer;
using UnityEngine.EventSystems;

public class HomeViewDatingNoticContext : BaseContext
{
    public HomeViewDatingNoticContext()
    {
        ViewType = UIType.HomeViewDatingNotic;
    }
}
public class UIHomeViewDatingNoticView : BasesView
{
    [SerializeField]
    RawImage noticeTexture;//实现公告纹理的轮换,纹理个数根据这个生成
    [SerializeField]
    Scrollbar noticSlider;//
    [SerializeField]
    GameObject noticPointContainer;//大厅公告点的容器

    List<float> values = new List<float>();//每个公告对应的值
    float factor = 0;
    List<RawImage> listNotic = new List<RawImage>();//纹理对象列表
    List<GameObject> listPoint = new List<GameObject>();//大厅公告数量显示的点 备用
    void Start()
    {
        noticSlider.onValueChanged.AddListener(OnNoticSliderValueChange);
        InItDaTingNoticData();
    }
    void InItDaTingNoticData()
    {
        List<Texture> listTexs = new List<Texture>();
        ArgsDatingNotic[] args = GlobalData.mDatingNoticArgs();
        for (int i = 0; i < args.Length; i++)
        {
            Texture datingNotic = ResourceManager.Instance.LoadAsset<Texture>(args[i].path);
            if (datingNotic != null)
            {
                listTexs.Add(datingNotic);
            }
        }
        for (int i = 0; i < listTexs.Count; i++)
        {
            if (i == 0)
            {
                noticeTexture.texture = listTexs[0];
                if (args[i].isCanClick)
                {
                    noticeTexture.gameObject.SetActive(true);
                    noticeTexture.transform.GetComponent<Button>().enabled = true;
                    noticeTexture.transform.GetComponent<Button>().onClick.AddListener(OnShareClick);
                }
                listNotic.Add(noticeTexture);
                continue;
            }

            Transform gridGm = GameObject.Instantiate(noticeTexture.gameObject).transform;
            gridGm.SetParent(noticeTexture.transform.parent);
            gridGm.localScale = Vector3.one;
            gridGm.GetComponent<RawImage>().texture = listTexs[i];
            gridGm.localPosition = new Vector3(330 * i, 0, 0);
            if (args[i].isCanClick)
            {
                gridGm.gameObject.SetActive(true);
                gridGm.transform.GetComponent<Button>().enabled = true;
                gridGm.transform.GetComponent<Button>().onClick.AddListener(OnShareClick);
            }
            listNotic.Add(noticeTexture);
        }
        CreatePoint(listTexs.Count);
        InitValues();
    }
    void CreatePoint(int datingNoticNum)
    {
        GameObject gm = noticPointContainer.transform.GetChild(0).gameObject;
        if (datingNoticNum == 1)
        {
            gm.SetActive(true);
            listPoint.Add(gm);
        }
        else
        {
            Vector3 leftMax = Vector3.zero;
            if (datingNoticNum % 2 == 0)
            {
                leftMax = gm.transform.localPosition + new Vector3(-datingNoticNum / 2 * 15 + 7.5f, 0, 0);
            }
            else
            {
                leftMax = gm.transform.localPosition + new Vector3(-datingNoticNum / 2 * 15, 0, 0);
            }
            for (int i = 0; i < datingNoticNum; i++)
            {
                GameObject newPoint = GameObject.Instantiate(gm);
                newPoint.transform.SetParent(gm.transform.parent);
                newPoint.transform.localScale = Vector3.one;
                newPoint.transform.localPosition = leftMax + new Vector3(i * 15, 0, 0);

                newPoint.SetActive(true);
                listPoint.Add(newPoint);
            }
        }
        OnNoticSliderValueChange(0);
    }
    void InitValues()
    {
        float res = listNotic.Count - 1;
        if (res == 0) return;

        factor = 1 / res;
        float nowValue = 0;

        values.Add(nowValue);
        while ((nowValue += factor) < (1 - factor / 2))
        {
            values.Add(nowValue);
        }
        values.Add(1);
    }
    void OnNoticSliderValueChange(float value)
    {
        float noticCount = listNotic.Count;
        if (noticCount == 0) return;
        float sliderValue = noticSlider.value;
        float factor = noticCount == 1 ? 1 : 1 / (noticCount - 1);//每次叠加的因子值
        for (int i = 0; i < listPoint.Count; i++)
        {
            bool isShow = sliderValue > (i - 1) * factor + factor / 3 && sliderValue < (i + 1) * factor - factor / 3;//factor/3 范围差
            listPoint[i].GetComponent<Image>().color = isShow ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
        }
    }
    void OnShareClick()
    {
        Debug.Log("分享链接 onClick");
        AudioManager.Instance.PlayClickBtnAudio();
        if (Application.platform == RuntimePlatform.WindowsEditor)
        { return; }
        SDKManager.Instance.WeChatShareNativeImage();
    }
}
