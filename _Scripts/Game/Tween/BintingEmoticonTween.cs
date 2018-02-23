using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BintingEmoticonTween : MonoBehaviour
{
    //emoticonSpr2有锚点定位，只能应用于砖块中的破裂的上面的那张图
    //emticonSpr3共用的，同时存在的第二张图  （香1）
    //emticonSpr4共用的，同时存在的第三张图 （香2）
    //emticonSpr5共用的，同时存在的第4张图（香3）
    // 6,烟
    [SerializeField]
    Image emoticonSpr1, emoticonSpr2, emoticonSpr3, emoticonSpr4, emoticonSpr5, emoticonSpr6;
    Sequence se;
    #region 备用
    //float intervalTime = 0.1f;//间隔时间
    //float time = 0;//计时
    //int endFpsNum = 0;//总帧率
    //int startfpsIndex = 0;//第几帧
    //string sprPrefixname = null;//图片前缀名(除去数字索引的)

    //void Update()
    //{        
    //    if (sprPrefixname != null)
    //    {
    //        Debug.Log("图片名称设置好");
    //        time += Time.deltaTime;
    //        if (startfpsIndex < endFpsNum)
    //        {
    //            if (time >= intervalTime)
    //            {

    //                Debug.Log("播放图片的名称:" + sprPrefixname + startfpsIndex);
    //                emoticonSpr.spriteName = sprPrefixname + startfpsIndex;
    //                startfpsIndex++;
    //                time = 0;
    //            }
    //        }
    //        else
    //        {
    //            if (time > intervalTime *1.5f)
    //            {
    //                Destroy(this.gameObject);
    //            }
    //        }
    //    }
    //}

    //public void PlayEmoticonBoom(int fps, string prefixname, Vector3 startPos, Vector3 endPos, int startIndex = 1, float intervalTime = 0.2f/*,bool outQuad=false*/)
    //{
    //    this.intervalTime = intervalTime;
    //    this.endFpsNum = fps + startIndex;
    //    this.sprPrefixname = prefixname;
    //    this.startfpsIndex = startIndex;
    //}
    //同类型：
    //EBoom,EBrick,ETomato
    //管理类里面   
    #endregion
    bool IsRoate = false;//是否旋转
    float rotSpeed = 30;//旋转速度
    float intervalTime = 0.1f;//间隔时间
    private void Update()
    {
        if (IsRoate)
        {
            this.transform.Rotate(Vector3.forward * rotSpeed);
        }
    }
    //需要
    public void InitTweenData(string name, Vector3 startPos)
    {
        emoticonSpr1.sprite = null;
        emoticonSpr2.sprite = null;
        emoticonSpr3.sprite = null;
        emoticonSpr4.sprite = null;
        emoticonSpr5.sprite = null;
        emoticonSpr6.sprite = null;
        this.transform.localPosition = startPos;
        emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, name);
        emoticonSpr1.SetNativeSize();
        se = DOTween.Sequence();
    }
    public void PlayPathTween(Vector3 endPos)
    {
        Vector3 startPos = this.transform.localPosition;
        if (startPos != endPos)
        {
            //Vector3[] ve = new Vector3[] { startPos, endPos };
            se.Append(this.transform.DOLocalMove(endPos, intervalTime * 10));
            //se.Append(this.transform.DOLocalPath(ve, intervalTime*10, PathType.CatmullRom).SetEase(Ease.InQuart));
        }
        se.AppendCallback(delegate
        {
            IsRoate = false;
        });
    }

    public void PlayBoomTween(Vector3 startPos, Vector3 endPos)
    {
        string prefixBoomName = "boom_";
        string prefixSmokeName = "smoke_";
        InitTweenData(prefixBoomName + 1, startPos);
        PlayPathTween(endPos);

        int NumFps = 6;
        for (int i = 1; i < NumFps - 2; i++)
        {
            int index = i;
            se.AppendInterval(0.15f);
            se.AppendCallback(delegate
            {
                Debug.Log(prefixBoomName + index);
                emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixBoomName + index);
                emoticonSpr1.SetNativeSize();
            });
        }
        for (int i = 1; i < 4; i++)
        {
            int index = i;
            se.AppendInterval(0.15f);
            se.AppendCallback(delegate
            {
                Debug.Log(prefixSmokeName + index);
                emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixSmokeName + index);
                emoticonSpr1.SetNativeSize();
            });
        }
        for (int i = NumFps - 2; i < NumFps + 1; i++)
        {
            int index = i;
            se.AppendInterval(0.2f);
            se.AppendCallback(delegate
            {
                Debug.Log(prefixBoomName + index);
                emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixBoomName + index);
                emoticonSpr1.SetNativeSize();
            });
        }

        se.AppendInterval(1.0f);
        se.AppendCallback(delegate
        {
            Destroy(this.gameObject);
        });
    }
    public void PlayTomatoTween(Vector3 startPos, Vector3 endPos)
    {
        InitTweenData("tomato_1", startPos);
        PlayPathTween(endPos);

        string prefixName = "tomato_";
        int NumFps = 8;

        for (int i = 0; i < NumFps; i++)
        {
            int index = i + 1;
            se.AppendInterval(0.15f);
            se.AppendCallback(delegate
            {
                Debug.Log(i.ToString());
                Debug.Log(prefixName + index);
                emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixName + index);
                emoticonSpr1.SetNativeSize();
            });
        }
        se.AppendInterval(1.0f);
        se.AppendCallback(delegate
        {
            Destroy(this.gameObject);
        });
    }
    public void PlayBeerTween(Vector3 startPos, Vector3 endPos, float rotaZ = 0)
    {
        float intercalTime = 0.15f;
        string prefixName = "beer_";
        InitTweenData(prefixName + 1, startPos);

        this.transform.Rotate(new Vector3(0, rotaZ, 0));//旋转
        se.Append(this.transform.DOLocalMove(endPos, 0.5f).SetEase(Ease.Linear));

        for (int i = 1; i < 4; i++)
        {
            int index = i;
            se.AppendInterval(intercalTime);
            se.AppendCallback(delegate
            {
                emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixName + index);
                emoticonSpr1.SetNativeSize();
            });
        }
        se.AppendInterval(intercalTime * 3);
        se.AppendCallback(
            delegate
            {
                Destroy(this.gameObject);
            }
            );
        se.Play();

    }
    public void PlayBrickTween(Vector3 startPos, Vector3 endPos)
    {
        string prefixbrickName = "brick_";
        string prefixSmokeName = "smoke_";
        InitTweenData(prefixbrickName + 1, startPos);
        PlayPathTween(endPos);
        IsRoate = true;
        //动画播放
        se.AppendInterval(intervalTime);
        se.AppendCallback(delegate
        {
            emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixbrickName + 1);
            this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        });
        se.AppendInterval(intervalTime * 0.5f);
        se.AppendCallback(delegate
        {
            emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixbrickName + 2);
        });
        se.AppendInterval(intervalTime * 2);
        se.AppendCallback(
            delegate
            {
                emoticonSpr1.sprite = null;

                emoticonSpr2.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixbrickName + 3);
                emoticonSpr3.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixbrickName + 4);
                emoticonSpr2.SetNativeSize();
                emoticonSpr3.SetNativeSize();
            });
        //烟雾
        for (int i = 1; i < 4; i++)
        {
            int index = i;
            se.AppendInterval(intervalTime * 0.5f);
            se.AppendCallback(delegate
            {
                Debug.Log(prefixSmokeName + index);

                emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, prefixSmokeName + index);
                //emoticonSpr1.MakePixelPerfect();
            });
        }
        se.Join(emoticonSpr3.transform.DOLocalMoveY(-100, intervalTime * 10f));
        se.AppendInterval(intervalTime * 3);
        se.AppendCallback(delegate { Destroy(this.gameObject); });
        se.Play();
    }
    public void PlayEggTween(Vector3 startPos, Vector3 endPos)
    {
        string preFixname = "egg_";
        InitTweenData(preFixname + 1, startPos);
        PlayPathTween(endPos);
        for (int i = 1; i < 5; i++)
        {
            int index = i;
            se.AppendInterval(intervalTime * 1.5f);
            se.AppendCallback(delegate
            {
                Debug.Log(preFixname + index);

                emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, preFixname + index);
                emoticonSpr1.SetNativeSize();
                //emoticonSpr1.MakePixelPerfect();
            });
        }
        se.AppendInterval(intervalTime * 2);
        se.Append(emoticonSpr1.transform.DOLocalMoveY(-100, intervalTime * 10));
        se.AppendCallback(delegate
        {
            Destroy(this.gameObject);
        });
        se.Play();
    }
    public void PlayFlowerTween(Vector3 startPos, Vector3 endPos, TweenCallback callFunHeart, TweenCallback callFun)
    {
        string preFixName = "flower";
        InitTweenData(preFixName, startPos);
        PlayPathTween(endPos);
        se.AppendCallback(delegate ()
        {
            if (callFun != null) { callFun(); }
            if (callFunHeart != null) { callFunHeart(); }
        });
        se.AppendInterval(7);//个数*每个的间隔时间+最后一个心的结束时间
        se.AppendCallback(() => Destroy(this.gameObject));
        se.Play();
    }
    /// <summary>
    /// 香炉
    /// </summary>
    /// <param name="Pos"></param>
    public void PlayIncenseTween(Vector3 Pos)
    {
        this.transform.localPosition = Pos;

        string preFixname = "incense_";
        InitTweenData(preFixname + 1, Pos);
        emoticonSpr1.transform.localPosition += new Vector3(0, 20, 0);
        se = DOTween.Sequence();
        se.Append(emoticonSpr1.transform.DOLocalMoveY(-40, intervalTime * 10));
        se.Append(emoticonSpr1.transform.DOShakeRotation(1, 30));
        se.AppendCallback(delegate
        {
            emoticonSpr1.transform.SetSiblingIndex(emoticonSpr1.transform.GetSiblingIndex() + 5);
            emoticonSpr1.SetNativeSize();
            emoticonSpr3.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, preFixname + 2);
            emoticonSpr3.SetNativeSize();
        });
        //香1
        emoticonSpr3.transform.localPosition += new Vector3(15, 60, 0);
        se.Append(emoticonSpr3.transform.DOLocalMoveY(0, intervalTime * 5));
        se.AppendCallback(delegate
        {
            emoticonSpr4.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, preFixname + 2);
            emoticonSpr4.SetNativeSize();
        });
        //香2
        emoticonSpr4.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        emoticonSpr4.transform.localPosition += new Vector3(-15, 60, 0);
        se.Append(emoticonSpr4.transform.DOLocalMoveY(0, intervalTime * 5));
        //香中
        se.AppendCallback(delegate
        {
            emoticonSpr5.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, preFixname + 3);
            emoticonSpr5.SetNativeSize();
        });


        emoticonSpr5.transform.localPosition += new Vector3(0, 60, 0);
        se.Append(emoticonSpr5.transform.DOLocalMoveY(5, intervalTime * 5));
        //————中间的香结束

        emoticonSpr6.transform.localScale = Vector3.zero;
        emoticonSpr6.color = new Color(1, 1, 1, 0);
        emoticonSpr6.transform.localPosition = new Vector3(10, 15, 0);
        se.AppendCallback(delegate
        {
            emoticonSpr6.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, preFixname + 4);
            emoticonSpr6.SetNativeSize();
        });
        se.Append(DOTween.ToAlpha(() => emoticonSpr6.color, x => emoticonSpr6.color = x, 1f, intervalTime * 10));
        se.Append(emoticonSpr6.transform.DOLocalMoveY(Pos.y + 60, 6));
        se.Join(DOTween.ToAlpha(() => emoticonSpr6.color, x => emoticonSpr6.color = x, 0f, 6));
        se.AppendCallback(
               () => Destroy(this.gameObject)
            );
        se.Play();
    }
    /// <summary>
    /// 财神到
    /// </summary>
    /// <param name="pos"></param>
    public void PlayWealthTween(Vector3 pos, TweenCallback callFun)
    {
        this.transform.localPosition = pos;

        string preFixname = "wealth_";
        InitTweenData(preFixname + 1, pos);
        //财神
        emoticonSpr1.transform.localPosition += new Vector3(0, 50, 0);
        se = DOTween.Sequence();
        se.Append(emoticonSpr1.transform.DOLocalMoveY(0, intervalTime * 5));
        se.Append(emoticonSpr1.transform.DOShakeRotation(2, 10));
        WealthText(emoticonSpr3, preFixname, 3);//财
        WealthText(emoticonSpr4, preFixname, 4);//神
        WealthText(emoticonSpr5, preFixname, 5);//到
        if (callFun != null)
        {
            se.AppendCallback(
                delegate
                {
                    callFun();
                });
        }
        se.Append(emoticonSpr1.transform.DOShakeRotation(10, 10));
        //se.AppendInterval(6f);//取决于 callFun中动画的时长
        se.AppendCallback(delegate ()
        {
            Destroy(this.gameObject);
        });
        se.Play();
    }
    void WealthText(Image spr, string preFixname, int index)
    {
        spr.transform.localScale = Vector3.one * 2;
        spr.transform.localRotation = Quaternion.Euler(Vector3.forward * 180);
        spr.transform.SetSiblingIndex(spr.transform.GetSiblingIndex() + index);//层级
        int args = index;
        if (index == 3) { args = 1; }
        else if (index == 4) { args = 3; }
        spr.transform.localPosition = new Vector3(emoticonSpr1.rectTransform.rect.width / 2 - 20, emoticonSpr1.rectTransform.rect.height / 2 - 25 * args, 0);
        se.AppendCallback(delegate
        {
            spr.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, preFixname + index);
            spr.SetNativeSize();
        });
        se.Append(spr.transform.DOScale(Vector3.one, intervalTime * 2f));
        se.Join(spr.transform.DORotate(Vector3.forward, intervalTime * 2f));

    }
    //用于星星和元宝的生成
    public void RanGm(string preFixname, Vector3 startPos, Vector3 endPos, float intervaltime, float rotZ = 0, float ranA = 0)
    {
        emoticonSpr1.sprite = null;
        emoticonSpr2.sprite = null;
        emoticonSpr3.sprite = null;
        emoticonSpr4.sprite = null;
        emoticonSpr5.sprite = null;
        emoticonSpr6.sprite = null;
        this.transform.localPosition = startPos;
        se = DOTween.Sequence();
        emoticonSpr1.enabled = false;
        se.AppendInterval(intervaltime);
        se.AppendCallback(delegate
        {
            emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, preFixname);
            emoticonSpr1.SetNativeSize();
            emoticonSpr1.enabled = true;
        });

        //emoticonSpr1.color = new Color(1, 1, 1, ranA);
        emoticonSpr1.SetNativeSize();
        emoticonSpr1.transform.localRotation = Quaternion.Euler(Vector3.forward * rotZ);
        Debug.Log("emoticonSpr1.transform.localPosition" + emoticonSpr1.transform.localPosition);

        se.Append(this.transform.DOLocalMove(endPos, intervalTime * 5));
        se.Join(DOTween.ToAlpha(() => emoticonSpr1.color, x => emoticonSpr1.color = x, ranA, intervalTime * 5));
        se.AppendCallback(delegate
        {
            Destroy(this.gameObject);
        });
        se.Play();

    }
    public void HeartGm(Vector3 startPos, float interTime, int depth)
    {
        InitTweenData("flower_heart", startPos);
        emoticonSpr1.sprite = null;
        emoticonSpr1.color = new Color(1, 1, 1, 1);
        se.AppendInterval(interTime);
        se.AppendCallback(
           delegate
           {
               emoticonSpr1.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.EPlaying, "flower_heart");
               emoticonSpr1.SetNativeSize();
               emoticonSpr1.transform.SetSiblingIndex(emoticonSpr1.transform.GetSiblingIndex() + depth);
           }
            );
        se.Append(this.transform.DOScale(Vector3.one, intervalTime * 10));
        se.Join(DOTween.ToAlpha(() => emoticonSpr1.color, x => emoticonSpr1.color = x, 0f, intervalTime * 10));
        se.AppendCallback(delegate
        {
            Destroy(this.gameObject);
        });
        se.Play();
    }
}
