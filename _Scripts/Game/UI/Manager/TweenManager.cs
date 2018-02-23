using UnityEngine;
using System.Collections;
using Common;
using System.Collections.Generic;
using ZhiWa;
using System;
using MsgContainer;
using DG.Tweening;
using HaoyunFramework;
using UnityEngine.UI;
using Spine.Unity;

public class TweenManager : SingleTon<TweenManager>
{
    float deltaTime = 0.04f;//按照一秒25帧计算
    private List<GameObject> propList = new List<GameObject>();
    public List<GameObject> mPropList
    {
        get { return propList; }
    }
    public void ClearPropList()
    {
        for (int i = 0; i < mPropList.Count; i++)
        {
            DestroyTween(mPropList[i]);
        }
    }
    private List<GameObject> emoticonList = new List<GameObject>();
    public List<GameObject> mEmoticonList
    {
        get { return emoticonList; }
    }
    public void ClearEmoticonList()
    {
        for (int i = 0; i < mEmoticonList.Count; i++)
        {
            DestroyTween(mEmoticonList[i]);
        }
    }
    #region 界面动画的控制
    //只可能同时出现一次
    private Dictionary<EViewAnimType, GameObject> dicSingleViewAnim = new Dictionary<EViewAnimType, GameObject>();//界面中的动画 除去表情道具
    public void LoadViewSingleAnimation(EViewAnimType type, Transform parent)
    {
        if (dicSingleViewAnim.ContainsKey(type))
        {
            return;
        }
        string loadPath = null;
        GameObject gm = null;
        switch (type)
        {
            case EViewAnimType.EBindBtn:
                loadPath = GlobalData.mLocalViewAnimationPath + "AnimBind";
                gm = ResourceManager.Instance.LoadAsset<GameObject>(loadPath);
                gm = GameObject.Instantiate(gm);
                gm.transform.SetParent(parent);
                gm.transform.localPosition = Vector3.zero;
                gm.transform.localScale = Vector3.one * 0.9f;
                break;
            case EViewAnimType.EInviteBtn:
                loadPath = GlobalData.mLocalViewAnimationPath + "AnimInvite";
                gm = ResourceManager.Instance.LoadAsset<GameObject>(loadPath);
                gm = GameObject.Instantiate(gm);
                gm.transform.SetParent(parent);
                gm.transform.localPosition = Vector3.zero;
                gm.transform.localScale = Vector3.one * 0.9f;
                break;
            case EViewAnimType.EPerson:
                loadPath = GlobalData.mLocalViewAnimationPath + "AnimDatingPerson";
                gm = ResourceManager.Instance.LoadAsset<GameObject>(loadPath);
                gm = GameObject.Instantiate(gm);
                gm.transform.SetParent(parent);
                gm.transform.localScale = Vector3.one;
                RectTransform rectTran = gm.GetComponent<RectTransform>();
                //rectTran.localPosition = new Vector3(0, rectTran.rect.height / 2, 100);//点位人的位置
                rectTran.localPosition = new Vector3(0, 0, 100);
                rectTran.anchoredPosition = new Vector3(0, rectTran.rect.height / 2 - 10, 100);//定位锚点的相对位置              
                break;
        }
        if (gm == null)
        {
            Debug.LogError(type + "类型动画加载失败");
        }
        dicSingleViewAnim.Add(type, gm);
    }
    public void ClearViewSingleAnimation()
    {
        dicSingleViewAnim.Clear();
    }
    ////可以同时出现多个的动画
    //private Dictionary<EViewAnimType, List<GameObject>> dicMutipleViewAnim = new Dictionary<EViewAnimType, List<GameObject>>();//爆炸等动画 除去表情道具
    //public void LoadViewAnimation(EViewAnimType type, Transform parent)
    //{
    //    string loadPath = null;
    //    switch (type)
    //    {
    //        case EViewAnimType.EBindBtn:
    //            loadPath = GlobalData.mLocalViewAnimationPath + "AnimBind";
    //            break;
    //        case EViewAnimType.EInviteBtn:
    //            loadPath = GlobalData.mLocalViewAnimationPath + "AnimInvite";
    //            break;
    //        case EViewAnimType.EPerson:
    //            loadPath = GlobalData.mLocalViewAnimationPath + " AnimDatingPerson";
    //            break;
    //    }
    //    GameObject gm = ResourceManager.Instance.LoadAsset<GameObject>(loadPath);
    //    if (gm == null)
    //    {
    //        Debug.LogError(type + "类型动画加载失败");
    //    }
    //    if (dicMutipleViewAnim.ContainsKey(type))
    //    {
    //        dicMutipleViewAnim[type].Add(gm);
    //    }
    //    else
    //    {
    //        List<GameObject> listAnim = new List<GameObject>();
    //        listAnim.Add(gm);
    //        dicMutipleViewAnim.Add(type, listAnim);
    //    }
    //}
    //public void RemoveMutipleViewAnim(EViewAnimType type, GameObject gm)
    //{
    //    if (dicMutipleViewAnim.ContainsKey(type))
    //    {
    //        dicMutipleViewAnim[type].Remove(gm);
    //        GameObject.Destroy(gm);
    //    }
    //    else
    //        return;
    //    if (dicMutipleViewAnim[type].Count == 0)
    //    {
    //        dicMutipleViewAnim.Remove(type);
    //    }
    //}
    //public void ClearMutipleViewAnim()
    //{
    //    int enumCount = GlobalData.EnumToListInfo(typeof(EViewAnimType), 0).Count;
    //    while (enumCount > 0)
    //    {
    //        if (dicMutipleViewAnim.ContainsKey((EViewAnimType)enumCount))
    //        {
    //            for (int i = 0; i < dicMutipleViewAnim[(EViewAnimType)enumCount].Count; i++)
    //            {
    //                GameObject.Destroy(dicMutipleViewAnim[(EViewAnimType)enumCount][i]);
    //            }
    //        }
    //        enumCount--;
    //    }
    //    dicMutipleViewAnim.Clear();
    //}
    #endregion
    private List<GameObject> listCardTypeAnims = new List<GameObject>();
    public List<GameObject> mListCardTypeAnims
    {
        get
        {
            return listCardTypeAnims;
        }
    }
    public void ClearListCardTypeAnims()
    {
        for (int i = 0; i < listCardTypeAnims.Count; i++)
        {
            listCardTypeAnims.Remove(listCardTypeAnims[i]);
            if (listCardTypeAnims.Contains(listCardTypeAnims[i]))
                GameObject.Destroy(listCardTypeAnims[i]);
        }
    }
    public GameObject PlayCardTween(TGuanDanCT type, Transform parent)
    {
        if (type == TGuanDanCT.CT_BUCHU || type == TGuanDanCT.CT_ERROR)
        {
            return null;
        }
        GameObject guandanAnimType = null;
        switch (type)
        {

            case TGuanDanCT.CT_SI_ZHANG_BOMB:
            case TGuanDanCT.CT_WU_ZHANG_BOMB:
            case TGuanDanCT.CT_LIU_ZHANG_BOMB:
            case TGuanDanCT.CT_QI_ZHANG_BOMB:
            case TGuanDanCT.CT_BA_ZHANG_BOMB:
            case TGuanDanCT.CT_JIU_ZHANG_BOMB:
            case TGuanDanCT.CT_SHI_ZHANG_BOMB:
                string path = GlobalData.mLocalViewAnimationPath + "AnimCardType";

                guandanAnimType = GetBoomAnimGm(parent, path);

                //SharkScreen(parent.parent.parent.parent.gameObject, 40);
                AudioManager.Instance.PlayCardTweenAudio("m_boom");
                PlayPlayingAnim(guandanAnimType, "zhadan");
                //播放炸弹动画
                break;
            case TGuanDanCT.CT_FOUR_KING:
                string pathKing = GlobalData.mLocalViewAnimationPath + "AnimFourKingBoom";

                guandanAnimType = GetBoomAnimGm(parent, pathKing);

                SharkScreen(parent.parent.parent.parent.gameObject, 120);
                AudioManager.Instance.PlayCardTweenAudio("m_boom");
                PlayPlayingAnim(guandanAnimType);
                break;
            case TGuanDanCT.CT_TONG_HUA_SHUN://同花顺
                string pathTonghua = GlobalData.mLocalViewAnimationPath + "AnimCardType";

                guandanAnimType = GetBoomAnimGm(parent, pathTonghua);

                //SharkScreen(parent.parent.parent.parent.gameObject, 80);
                AudioManager.Instance.PlayCardTweenAudio("m_boom");
                PlayPlayingAnim(guandanAnimType, "tonghuashun");
                break;
            case TGuanDanCT.CT_LIANG_LIAN_DUI://连对  
                string pathLiandui = GlobalData.mLocalViewAnimationPath + "AnimCardType";

                guandanAnimType = GetBoomAnimGm(parent, pathLiandui);
                PlayPlayingAnim(guandanAnimType, "liandui");
                break;
            case TGuanDanCT.CT_SHUN_ZI:
                string pathShunzi = GlobalData.mLocalViewAnimationPath + "AnimCardType";

                guandanAnimType = GetBoomAnimGm(parent, pathShunzi);
                PlayPlayingAnim(guandanAnimType, "shunzi");
                break;
            case TGuanDanCT.CT_GANG_BAN://飞机
                FeijiAnim(parent);

                break;
        }
        listCardTypeAnims.Add(guandanAnimType);
        return guandanAnimType;
    }
    void FeijiAnim(Transform parent)
    {
        UIAirPlaneTween airGm = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(GlobalData.mLoadItemTitlePath + "AirplaneTween")).GetComponent<UIAirPlaneTween>();
        airGm.transform.SetParent(GameObject.Find(GlobalData.mUIRootName).transform);
        airGm.transform.localPosition = Vector3.zero;
        airGm.transform.localScale = Vector3.one;
        PlayingGameInfo.Instance.mNowAirGm = airGm.gameObject;
        airGm.SetData(new Vector3(Screen.currentResolution.width / 2.0f, 0, 0), new Vector3(-Screen.currentResolution.width / 2.0f, 0, 0), (TweenCallback)delegate
    {
        string pathTonghua = GlobalData.mLocalViewAnimationPath + "AnimCardType";

        GameObject tonghua = GetBoomAnimGm(parent, pathTonghua);

        //SharkScreen(parent.parent.parent.parent.gameObject, 80);
        AudioManager.Instance.PlayCardTweenAudio("m_boom");
        PlayPlayingAnim(tonghua, "feiji");
    });
        AudioManager.Instance.PlayCardTweenAudio("plane");
        listCardTypeAnims.Add(airGm.gameObject);
    }
    GameObject GetBoomAnimGm(Transform parent, string path)
    {
        GameObject zhadanboom = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(path));
        zhadanboom.transform.SetParent(parent);
        zhadanboom.transform.localPosition = Vector3.zero;
        zhadanboom.transform.localScale = Vector3.one;
        GameObject tran = parent.parent.gameObject;
        zhadanboom.transform.localPosition += GetCardAnimGMPosMove(tran);
        return zhadanboom;
    }
    Vector3 GetCardAnimGMPosMove(GameObject gmNamePos)
    {
        string animName = gmNamePos.name;
        if (animName.ToLower().Contains("self"))
        {
            return Vector3.up * 50;
        }
        else
        {
            return -Vector3.up * 40;
        }
    }
    /// <summary>
    /// 屏幕震动效果
    /// </summary>
    public void SharkScreen(GameObject gm, float randomess)
    {
        //Transform cameraTrans = Camera.main.transform;
        //Camera cam = cameraTrans.GetComponent<Camera>();
        //cam.DOShakePosition(1, 20, 30, 80, false);
        gm.transform.DOShakePosition(1, 20, 30, randomess, true);
        Sequence se = DOTween.Sequence();
        se.AppendInterval(1.0f);
        se.AppendCallback(delegate
        {
            gm.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            gm.transform.localPosition = Vector3.zero;
        });
    }
    /// <summary>
    /// 头游动画
    /// </summary>
    public void PlayTouyouAnim()
    {
        GameObject touyouAnim = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(GlobalData.mLocalViewAnimationPath + "AnimTouyou"));
        touyouAnim.transform.localScale = Vector3.one * 100;
        touyouAnim.transform.localPosition = Vector3.zero;
        PlayPlayingAnim(touyouAnim);
    }
    /// <summary>
    /// 游戏中动画
    /// </summary>
    /// <param name="gm"></param>
    public void PlayPlayingAnim(GameObject gm, string animName = "animation")
    {
        float destroyTime = 3;
        destroyTime = GetAnimatorTime(gm, animName);
        Sequence myse = DOTween.Sequence();
        myse.AppendInterval(destroyTime);
        myse.AppendCallback(delegate
        {
            if (gm != null)
            {
                listCardTypeAnims.Remove(gm);
                GameObject.Destroy(gm);
            }
        });
    }
    public GameObject PlayJiefengTween(Transform tweenPos, Transform parent, EPlayerPositionType posType)
    {
        //Vector3 initPos = Vector3.zero;
        //Vector3 targetPos = Vector3.zero;
        //switch (posType)
        //{
        //    case EPlayerPositionType.ELeft:
        //        initPos = new Vector3(750, 0, 0);
        //        targetPos = Vector3.zero;
        //        break;
        //    case EPlayerPositionType.ERight:
        //        initPos = new Vector3(-750, 0, 0);
        //        targetPos = Vector3.zero;
        //        break;
        //    case EPlayerPositionType.ESelf: //自己接风的情况，对面接风数字票到自己身上
        //        initPos = new Vector3(0, 460, 0);
        //        targetPos = Vector3.zero;
        //        break;
        //    case EPlayerPositionType.ETop:
        //        initPos = new Vector3(0, -400, 0);
        //        targetPos = Vector3.zero;
        //        break;
        //}
        string path = GlobalData.mLocalViewAnimationPath + "AnimCardType";
        GameObject jiefeng = GetBoomAnimGm(parent, path);
        PlayPlayingAnim(jiefeng, "jiefeng");
        return jiefeng;
    }
    public void PlayKangGongTween(Transform parent, EPlayerPositionType posType)
    {
        GameObject item = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>("PlayingCardTween"));
        item.transform.SetParent(parent);
        UIPlayingCardTween itemTween = item.GetComponent<UIPlayingCardTween>();
        itemTween.PlayKangGongTributeTween(posType);//删除了一个参数
    }
    //单个图标的抗贡动画
    public void PlayKangGongTween(GameObject g, Action tweenOver = null)
    {
        Sequence se = DOTween.Sequence();
        Image kangGong = g.GetComponent<Image>();
        if (kangGong != null)
        {
            kangGong.color = new Color(1, 1, 1, 1);
        }
        g.SetActive(true);
        g.transform.localScale = Vector3.zero;
        se.Append(g.transform.DOScale(Vector3.one, 0.5f)).SetEase(Ease.InOutBounce);
        se.AppendInterval(1.0f);
        se.AppendInterval(1.5f);
        if (kangGong != null)
            se.Join(DOTween.ToAlpha(() => kangGong.color, x => kangGong.color = x, 0, 1.5f));
        se.AppendCallback(delegate
        {
            if (g != null)
            {
                g.SetActive(false);
            }
            if (tweenOver != null)
            {
                tweenOver();
            }
        });
        se.Play();
    }
    public void PlayLightCardSmokeAnim(Transform parent, float posY)
    {
        GameObject gm = UIManagers.Instance.GetSingleUI(UIType.MingPai);
        gm.transform.SetParent(parent);
        gm.transform.localScale = Vector3.one;
        gm.transform.localPosition = new Vector3(0, posY, 0);
        float baozhaTime = GetAnimatorTime(gm, "baozha");

        Sequence se = DOTween.Sequence();
        se.AppendInterval(baozhaTime);
        float mingpaiTime = 0;
        se.AppendCallback(delegate
        {
            mingpaiTime = GetAnimatorTime(gm, "mingpai");
            se.AppendInterval(mingpaiTime);
            se.AppendCallback(delegate
            {
                UIManagers.Instance.DestroySingleUI(UIType.MingPai);
            });
        });
        se.Play();
    }
    public void PlayLightCard(int idx, GameObject obj, uint cardId, Transform selfParent, bool isSame = false)
    {
        obj.transform.SetParent(selfParent);
        SingleCard item = obj.GetComponent<SingleCard>();
        item.mCurStatus = ECardStatus.ELightCard;
        item.SetSprName(cardId, "small");
        item.PlayLightCardTween(obj, isSame, idx == 0);
        item.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
        item.transform.GetChild(0).GetChild(0).GetComponent<Image>().raycastTarget = false;
        item.transform.GetChild(0).GetChild(1).GetComponent<Image>().raycastTarget = false;
    }

    #region 道具动画 预支体加载版本

    public void PlayPropsAnimation(ArgsPropsInfo propInfo, Transform toolParent, Transform startPos, Transform endPos)
    {
        MsgProps type = (MsgProps)propInfo.propsId;
        Sequence se = DOTween.Sequence();

        string tweenName = "";
        switch (type)
        {
            case MsgProps.MsgPropsEgg://鸡蛋 >>ok
                tweenName = "AnimProp_jidan";
                CretePropsTween_Two(type, startPos, endPos, tweenName);
                break;
            case MsgProps.MsgPropsBeer://啤酒 >>Ok 修改 同玫,因为有动画状态机 两个动画中间有个转换
                tweenName = "AnimProp_pijiu";
                BeerAnimation(se, type, startPos, endPos, GetScaleMutiple(type, 1), tweenName);
                break;
            case MsgProps.MsgPropsBomb://炸弹 >>ok 
                tweenName = "AnimProp_shoulei";
                CretePropsTween_Two(type, startPos, endPos, tweenName);
                break;
            case MsgProps.MsgPropsRose://玫瑰 >>修改 添加了一个预制体
                tweenName = "AnimProp_yizhihua";
                RoseAnimation(se, type, startPos, endPos, GetScaleMutiple(type, 1), tweenName);
                break;
            case MsgProps.MsgPropsKnife://刀子 >>修改 修改了刀光
                tweenName = "AnimProp_caidao";
                CretePropsTween_Two(type, startPos, endPos, tweenName);
                break;
            case MsgProps.MsgPropsBrick://板砖 >>Ok
                tweenName = "AnimProp_Tbanzhuan";
                CretePropsTween_Two(type, startPos, endPos, tweenName);
                break;
            case MsgProps.MsgPropsIncense://香炉 >>Ok  修改了烟雾
                tweenName = "AnimProp_xianglu";
                CretePropsTween_Two(type, startPos, endPos, tweenName, 4);
                break;
            case MsgProps.MsgPropsGod://财神  >>OK 
                tweenName = "AnimProp_dushen";
                CretePropsTween_Three(type, startPos, endPos, tweenName, 15);
                break;
        }
    }
    void BeerAnimation(Sequence se, MsgProps type, Transform startPos, Transform endPos, float scale, string tweenName)
    {
        GameObject propTween1 = CreatePropsGm(type, startPos, GetScaleMutiple(type, 1), tweenName);
        float playTimeOne = GetAnimatorTime(propTween1, "play1");
        float playTimeTwo = GetAnimatorTime(propTween1, "play2");

        if (startPos == endPos)
        {
            propTween1.GetComponent<SkeletonAnimation>().AnimationName = "play2";
            AudioManager.Instance.PlayPropsAudio((uint)MsgProps.MsgPropsBeer);
        }
        else
        {
            propTween1.GetComponent<SkeletonAnimation>().AnimationName = "play1";
            se.Append(propTween1.transform.DOLocalMove(GetEndPostion(endPos), deltaTime * 16));
            se.AppendCallback(delegate
            {
                propTween1.GetComponent<SkeletonAnimation>().AnimationName = "play2";
                AudioManager.Instance.PlayPropsAudio((uint)MsgProps.MsgPropsBeer);
            });
        }
        se.AppendInterval(playTimeTwo);
        se.AppendCallback(delegate { DestroyTween(propTween1); });
    }
    void RoseAnimation(Sequence se, MsgProps type, Transform startPos, Transform endPos, float scale, string tweenName)
    {
        GameObject propTween1 = CreatePropsGm(type, startPos, GetScaleMutiple(type, 1), tweenName);
        propTween1.transform.localScale = Vector3.one * 0.3f;
        AudioManager.Instance.PlayPropsAudio((uint)MsgProps.MsgPropsRose);
        float destroyTime = GetAnimatorTime(propTween1);
        if (startPos == endPos)
        {
            se.AppendInterval(deltaTime);
        }
        else
        {
            se.Append(propTween1.transform.DOLocalMove(GetEndPostion(endPos), deltaTime * 16));
        }
        se.AppendCallback(delegate
        {
            propTween1.GetComponent<SkeletonAnimation>().AnimationName = "animation";
        });
        se.AppendInterval(destroyTime * 2);
        se.AppendCallback(
            delegate
            {
                DestroyTween(propTween1);
            });
    }
    /// <summary>
    /// 两个预支体的组合
    /// </summary>
    /// <param name="toolParent"></param>
    /// <param name="type"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="tweenName"></param>
    /// <param name="time">第二动画重复的次数</param>
    void CretePropsTween_Two(MsgProps type, Transform startPos, Transform endPos, string tweenName, int time = 1)
    {
        Sequence se = DOTween.Sequence();
        float destroyTime = 0;
        GameObject propTween1 = CreatePropsGm(type, startPos, GetScaleMutiple(type, 1), tweenName + 1);
        //只是获取时间用
        GameObject propTween2 = CreatePropsGm(type, endPos, GetScaleMutiple(type, 2), tweenName + 2);
        //propTween2.transform.localPosition = new Vector3(10000, 10000);   
        destroyTime = GetAnimatorTime(propTween2);
        propTween2.SetActive(false);
        //制作动画
        if (startPos == endPos)
        {
            se.Append(propTween1.transform.DOLocalMove(GetEndPostion(endPos), isNeedMoveOfSelf(type) == true ? deltaTime * 5 : 0));
        }
        else
        {
            if (type == MsgProps.MsgPropsBomb || type == MsgProps.MsgPropsKnife || type == MsgProps.MsgPropsBrick)
                se.Append(propTween1.transform.DOLocalMove(GetEndPostion(endPos), deltaTime * 16));
            else
                se.Append(propTween1.transform.DOLocalMove(GetEndPostion(endPos), deltaTime * 24));
        }
        se.AppendCallback(delegate
        {
            DestroyTween(propTween1);
            AudioManager.Instance.PlayPropsAudio((uint)type);
            propTween2.SetActive(true);
        });

        //开始第二个动画
        if (type == MsgProps.MsgPropsBomb)
        {
            destroyTime += 8 * deltaTime;
        }
        se.AppendInterval(destroyTime * time);
        se.AppendCallback(delegate
        {
            DestroyTween(propTween2);
        });
    }
    void DestroyTween(GameObject tween)
    {
        DOTween.Kill(tween);//杀死动画 否则警告
        propList.Remove(tween);
        GameObject.Destroy(tween);
    }
    bool isNeedMoveOfSelf(MsgProps type)
    {
        if (type == MsgProps.MsgPropsGod)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 三个预支体的组合
    /// </summary>
    /// <param name="toolParent"></param>
    /// <param name="type"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="tweenName"></param>
    /// <param name="time"></param>
    void CretePropsTween_Three(MsgProps type, Transform startPos, Transform endPos, string tweenName, int time = 1)
    {
        Sequence se = DOTween.Sequence();
        float destroyTime = 0;
        float deltaTime = 0.04f;//按照一秒25帧计算

        GameObject propTween1 = CreatePropsGm(type, startPos, GetScaleMutiple(type, 1), tweenName + 1);
        //制作动画
        if (startPos == endPos)
        {
            se.Append(propTween1.transform.DOLocalMove(GetEndPostion(endPos), isNeedMoveOfSelf(type) == true ? deltaTime * 5 : 0));
        }
        else
        {
            se.Append(propTween1.transform.DOLocalMove(GetEndPostion(endPos), deltaTime * 24));
            if (type == MsgProps.MsgPropsGod)
            {
                se.AppendInterval(GetAnimatorTime(propTween1));
            }
        }
        //第二个动画制作
        GameObject propTween2 = CreatePropsGm(type, endPos, GetScaleMutiple(type, 2), tweenName + 2);
        destroyTime = GetAnimatorTime(propTween2);
        propTween2.SetActive(false);
        se.AppendCallback(delegate
        {
            DestroyTween(propTween1);
            //propTween2.transform.localPosition = endPos;
            AudioManager.Instance.PlayPropsAudio((uint)type);
            propTween2.SetActive(true);
        });
        //结束第一个动画,开始第二个动画
        se.AppendInterval(destroyTime);

        //第三个动画制作
        GameObject propTween3 = CreatePropsGm(type, endPos, GetScaleMutiple(type, 3), tweenName + 3);
        Animator animator2 = propTween3.GetComponent<Animator>();
        destroyTime = GetAnimatorTime(propTween3);
        propTween3.SetActive(false);
        //结束第二个动画，开始第三个动画
        se.AppendCallback(delegate
        {
            DestroyTween(propTween2);
            propTween3.SetActive(true);
        });

        se.AppendInterval(destroyTime * time);
        se.AppendCallback(delegate
        {
            DestroyTween(propTween3);
        });
    }
    /// <summary>
    /// 默认大小有改变
    /// </summary>
    /// <param name="type"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    float GetScaleMutiple(MsgProps type, int rank/*第几个动画*/)
    {
        //rank 1 默认100   rank=2  默认100
        float scale = 1;
        switch (type)
        {
            case MsgProps.MsgPropsBomb:
                if (rank == 2)
                {
                    scale = 100;
                }
                break;
            case MsgProps.MsgPropsGod:
                scale = 0.9f;
                break;
        }
        return scale;
    }

    public float GetAnimatorTime(GameObject gm, string animName = "animation")
    {
        SkeletonAnimation skAnim = gm.GetComponent<SkeletonAnimation>();
        skAnim.AnimationName = animName;
        Spine.Animation anim = skAnim.skeletonDataAsset.GetAnimationStateData().skeletonData.FindAnimation(animName);
        return anim.duration;
    }

    public GameObject CreatePropsGm(MsgProps type, Transform createParent, float scale, string prefabname)
    {
        GameObject propTween = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(GlobalData.mLocalPropPrefabPath + prefabname));

        if (GameManager.Instance.mCurGameStatus != EGameStatus.EPlaying)
        {
            propTween.transform.SetParent(createParent.parent);
            propTween.transform.localPosition = createParent.localPosition;
        }
        else
        {
            propTween.transform.SetParent(createParent);
            propTween.transform.localPosition = Vector3.zero;
            propTween.transform.localPosition = createParent.localPosition;
            propTween.transform.SetParent(createParent.parent.parent);
            propTween.transform.localPosition = createParent.localPosition + createParent.parent.localPosition;
        }
        propTween.transform.localScale = Vector3.one * scale;
        propList.Add(propTween);
        return propTween;
    }

    public Vector3 GetEndPostion(Transform endPos)
    {
        if (GameManager.Instance.mCurGameStatus != EGameStatus.EPlaying)
            return endPos.localPosition;
        if (endPos.parent.name.ToLower().Contains("self"))
        {
            return endPos.localPosition + endPos.parent.localPosition;
        }
        return endPos.localPosition;
    }
    #endregion
    public string GetEmotcionPrefabName(MsgEmoticon emoticonId)
    {
        string tweenName = "";
        switch (emoticonId)
        {
            case MsgEmoticon.MsgEmoticonCry://哭
                tweenName = "daku";
                break;
            case MsgEmoticon.MsgEmoticonSnail://蜗牛
                tweenName = "woniu";//
                break;
            case MsgEmoticon.MsgEmoticonSorry://鄙视
                tweenName = "bishi";
                break;
            case MsgEmoticon.MsgEmoticonLovely://害羞
                tweenName = "haixiu";
                break;
            case MsgEmoticon.MsgEmoticonGood://大拇指
                tweenName = "damuzhi";
                break;
            case MsgEmoticon.MsgEmoticonBlack://黑眼镜
                tweenName = "deyi";
                break;
            case MsgEmoticon.MsgEmoticonComplain://奔溃
                tweenName = "bengkui";
                break;
            case MsgEmoticon.MsgEmoticonAmazed://惊讶
                tweenName = "jingya";
                break;
            case MsgEmoticon.MsgEmoticonShutUp://闭嘴
                tweenName = "bizui";
                break;
            case MsgEmoticon.MsgEmoticonSaliva://流口水
                tweenName = "taoxin";
                break;
            case MsgEmoticon.MsgEmoticonQuestion://疑问
                tweenName = "sikao";
                break;
            case MsgEmoticon.MsgEmoticonSmile://哈哈大笑
                tweenName = "xiao";
                break;
                //case MsgEmoticon.://喷水
                //    tweenName = "penshui";
                //    break;
                //case 14://滑稽
                //    tweenName = "huaji";
                //    break;
        }
        return "Anim" + tweenName;
    }
    public IEnumerator DestroySmokeTween(float baozhaTime)
    {
        yield return new WaitForSeconds(baozhaTime);
        UIManagers.Instance.DestroySingleUI(UIType.MingPai);
    }

    public void SwitchLoadingSpr(bool isOpen)
    {
        var commonView = UIManagers.Instance.GetSingleUI(UIType.CommonView).GetComponent<UICommonView>();
        if (commonView != null)
        { 
        commonView.SwitchLoadingSpr(isOpen);
        }
    }

    /// <summary>
    /// 播放烟雾（这边动画其实增加一个播放完成的回调最好）
    /// </summary>
    /// <param name="mParent"></param>
    public void PlaySmoke(Transform mParent)
    {
        GameObject gm = GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject>(UIType.MingPai.Path));
        gm.transform.SetParent(mParent);
        gm.transform.localPosition = Vector3.zero;
        gm.transform.localScale = Vector3.one;
        float baozhaTime = GetAnimatorTime(gm, "baozha");
        GameObject.Destroy(gm, baozhaTime);
       
    }
}
