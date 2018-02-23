using UnityEngine;
using System.Collections;
using MsgContainer;
using DNotificationCenterManager;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class SingleCard : MonoBehaviour
{
    public bool mIsIn = true;
    /// <summary>
    /// 卡片的id号,用于显示
    /// </summary>
    public uint mId = 0;

    /// <summary>
    /// 是否是可改变状态
    /// </summary>
    bool IsCanChangeStatus
    {
        get
        {
            if (mCurStatus == ECardStatus.ELightCard || mId == 0||mCurStatus == ECardStatus.ERefreshCard)
            {
                return false;
            }
            return posType == EPlayerPositionType.ESelf;
        }
    }

    public int GetInRealDicHorizontalIdx
    {
        get
        {
            return SelfCardsManager.Instance.GetCurRealDicHorizontalIdx(this.mId);
        }
    }


    public int GetInRealDicListIdx
    {
        get
        {
            return SelfCardsManager.Instance.GetCurRealDicInListIdx(mId);
        }
    }
    public ECardStatus mCurStatus = ECardStatus.ENormal;

    [SerializeField]
    Image selectMask;

    [SerializeField]
    Image sprName;
    [SerializeField]
    GameObject btnCollider;

    [SerializeField]
    Text mIdLab;
    public EPlayerPositionType posType = EPlayerPositionType.ESelf;

    Transform cardSprTrans;
    void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EChangeCardSelectStatus, UpdateCardStatus);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.ETouchCardShake, ShakeCard);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDragDroping, DragDropMoving);
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EReleaseDragDrop, ReleaseDragDrop);
    }
    void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EChangeCardSelectStatus, UpdateCardStatus);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.ETouchCardShake, ShakeCard);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDragDroping, DragDropMoving);
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EReleaseDragDrop, ReleaseDragDrop);
    }
    void Start()
    {
        EventTriggerListener.Get(btnCollider).onExit = ButtonClick;
        cardSprTrans = sprName.transform;
    }

    void UpdateCardStatus(LocalNotification e)
    {
        ArgsCardStatus args = e.param as ArgsCardStatus;
        if (args != null)
        {
            if (args.idList.Contains(this.mId))
            {
                SwitchStatus(args.status);
            }
        }
    }

    bool isShaked = false;
    void ShakeCard(LocalNotification e)
    {
        if (!IsCanChangeStatus) { return; }

        ArgsShakeCard args = e.param as ArgsShakeCard;
        if (!args.mList.Contains(mId)) { return; }

        if (args != null)
        {
            if (isShaked == args.isShake) { return; }
            if (!isShaked)
            {
                cardSprTrans.DOLocalMoveY(GlobalData.mCardTouchMoveY * GetInRealDicListIdx, GlobalData.mCardTouchTime);
            }
            else
            {
                cardSprTrans.DOLocalMoveY(0, GlobalData.mCardTouchTime);
            }
            isShaked = args.isShake;
        }
    }


    public void ButtonClick(GameObject btn)
    {
        if (SelfCardsManager.Instance.mIsPressingBg)
        {
            return;
        }

        bool isReturn = false;
        CheckTouchSingleOrMulity(out isReturn);
        if (isReturn)
        {
            return;
        }
        SwitchStatus(mCurStatus == ECardStatus.ESelected ? ECardStatus.ENormal : ECardStatus.ESelected);
    }

    public void SwitchStatus(ECardStatus status)
    {
        if (!IsCanChangeStatus || SelfCardsManager.Instance.mIsDragDroping)
        {
            return;
        }
        var manager = SelfCardsManager.Instance;
        switch (status)
        {
            case ECardStatus.ENormal: //Selected===>>>Normal
                this.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1f);
                selectMask.enabled = false;
                manager.RemoveSelectCard(mId);
                break;
            case ECardStatus.ESelected: //Normal===>>>Selected

                selectMask.enabled = true;
                manager.AddSelectCard(mId);
                SendShakeCards();
                break;
            case ECardStatus.EOuted: //Selected==>>Outed
                CheckShakeToNormal();

                selectMask.enabled = false;
                if (mId != GlobalData.mRefuseCardNumId)
                {
                    SetSprName(mId, "small");
                }
                PlaySelfOutTween(manager.GetSelectCardPutOutToTargetPos(this.mId)); //播放动画
                break;
            case ECardStatus.ETransparent:
                SetTransparentStatusData();
                break;
        }

        mCurStatus = status;
    }

    /// <summary>
    /// 将Shake状态置为普通
    /// </summary>
    void CheckShakeToNormal()
    {
        if (isShaked)
        {
            cardSprTrans.transform.localPosition = Vector3.zero;
            isShaked = false;
        }
    }

    void SetTransparentStatusData()
    {
        this.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
    }


    void SendShakeCards()
    {
        if (isShaked) { return; }

        var list = SelfCardsManager.Instance.GetCurRealDicIdList(mId);
        if (list == null) { return; }
        ArgsShakeCard msg = new ArgsShakeCard();
        msg.mList.AddRange(list);
        msg.isShake = true;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ETouchCardShake, msg);
    }


    void SetDepth(int depth)
    {
        var list = SelfCardsManager.Instance.mCurSelectCardIds;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == this.mId)
            {
                sprName.GetComponent<Image>().transform.SetSiblingIndex(i + depth);
                btnCollider.GetComponent<Image>().transform.SetSiblingIndex(i + depth);
                selectMask.GetComponent<Image>().transform.SetSiblingIndex(i + depth);
                return;
            }
        }

    }

    void SwitchOtherStatus(EPlayerPositionType playerPosType)
    {
        btnCollider.gameObject.SetActive(false);
        PlayOtherOutTween(playerPosType);
        mCurStatus = ECardStatus.EOther;
    }

    void SwitchRefreshStatus(EPlayerPositionType playerPosType)
    {
        btnCollider.gameObject.SetActive(false);

        mCurStatus = ECardStatus.ERefreshCard;
    }


    public void SetSprName(uint id, string other)
    {

        if (other != "")
        {
            string spriteName = other + "_" + GlobalData.GetCardName(id);
            sprName.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.ECard, spriteName);
        }
        else
        {
            sprName.sprite = ResourceManager.Instance.GetSpriteByName((int)EAtlasType.ECard, GlobalData.GetCardName(id));
        }

        if (GameManager.Instance.mGameMode == EGameMode.EDebug)
        {
            mIdLab.text = id.ToString();
        }

        sprName.SetNativeSize();
    }



    void SetName()
    {
        this.gameObject.name = mId.ToString();
    }
    /// <summary>
    /// 设置自己牌的初始
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="id"></param>
    public void SetCardData(uint id)
    {
        this.mId = id;
        SetName();
        this.posType = EPlayerPositionType.ESelf;
        SetSprName(id, "");
        SwitchStatus(ECardStatus.ENormal);
    }


    int GetInitDepthByPlayerPosition(EPlayerPositionType type)
    {
        switch (type)
        {
            case EPlayerPositionType.ERight:
                return 10;
            case EPlayerPositionType.ETop:
                return 20;
            case EPlayerPositionType.ELeft:
                return 30;
        }
        return 20;
    }
    /// <summary>
    /// 初始别人的牌，并播放动画。
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="isLast"></param>
    public void SetOtherData(uint id, EPlayerPositionType type, bool isRefresh)
    {
        this.posType = type;
        this.mId = id;
        SetName();
        if (id != 1)
        {
            SetSprName(this.mId, "small");
        }
        else
        {
            SetSprName(this.mId, "");
        }

        if (isRefresh)
        {
            // this.transform.localScale = new Vector3(GlobalData.mPutOutCardRate, GlobalData.mPutOutCardRate, GlobalData.mPutOutCardRate);
            SwitchRefreshStatus(type);
        }
        else
        {
            SwitchOtherStatus(type);
        }
    }


    /// <summary>
    /// 播放动画,普通出牌，飞
    /// </summary>
    /// <param name="targetPos"></param>
    public void PlaySelfOutTween(Vector3 targetPos)
    {
        Transform parent = null;
        if(GoldFiledManager.Instance.mIsGoldFiled)
        {
             parent = this.transform.parent.parent.GetChild(0).Find("outCardTargetPos");
        }
        else
        {
             parent = this.transform.parent.parent.Find("OutTargetPos");
        }
		
		this.transform.SetParent(parent);
        float tweenTime = 0.25f;
        float endScale = GlobalData.mPutOutCardRate;
        Sequence mySequence = DOTween.Sequence();
        //pos，scale，alpha
        mySequence.Append(this.transform.DOLocalMove(targetPos, tweenTime)).SetEase(Ease.InOutQuad);
        mySequence.Join(this.transform.DOScale(endScale, tweenTime)).SetEase(Ease.InOutQuad);
        mySequence.AppendCallback(TweenSelfCallBack);
        mySequence.Play();
    }

    void TweenSelfCallBack()
    {
        btnCollider.SetActive(false);
    }

    /// <summary>
    /// 不出，缩放效果
    /// </summary>
    public void PlayOtherOutTween(EPlayerPositionType PosType)
    {
        Vector3 targetScale = Vector3.one;
        float tweenTime = 0.25f;
        Sequence mySequence = DOTween.Sequence();
        //Scale
        mySequence.Append(this.transform.DOScale(targetScale * 0.5f, tweenTime)).SetEase(Ease.OutCubic);  //放大
        mySequence.Append(this.transform.DOScale(targetScale * GlobalData.mPutOutCardRate, tweenTime));
        mySequence.Play();
    }
    //明牌动画
    public void PlayLightCardTween(GameObject gm, bool isSame = false,bool isStartGameId =false)//对象
    {
        float tweenTime = 1f;
        float intervalTime = 0.5f;
        sprName.color = new Color(1, 1, 1, 0.2f);
        sprName.gameObject.SetActive(false);
        Vector3 endPostion = PlayingGameInfo.Instance.GetLightCardPosEnd(gm, isSame);
        //smokeTween中烟雾1.2秒消失，明牌字样1秒消失
        Sequence mySequence = DOTween.Sequence();
        //Scale，alpha ,rotation
        mySequence.AppendInterval(intervalTime * 0.8f);//烟雾的时间
        mySequence.AppendCallback(delegate
        {
            sprName.gameObject.SetActive(true);
        });
        mySequence.Append(DOTween.ToAlpha(
            () => sprName.color, x => sprName.color = x, 1, tweenTime)
            );
        mySequence.AppendInterval(intervalTime);//明牌消失
        //牌移动
        mySequence.Append(this.transform.DOLocalMove(endPostion, tweenTime).SetEase(Ease.Linear));
        mySequence.Join(this.transform.DOScale(Vector3.one * 0.48f, tweenTime)).SetEase(Ease.Linear);
        mySequence.Join(DOTween.ToAlpha(() => sprName.color, x => x = sprName.color, 1, 1).SetEase(Ease.InBounce));
        //游戏开始显示按钮回调
        if (isStartGameId)
        {
            mySequence.AppendCallback(() => DNotificationCenterManager.NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EZhuandanStartGame));
        }
        mySequence.Play();
    }

    /// <summary>
    /// 用于调整Collider 触发拉长，优化用户体验
    /// </summary>
    /// <param name="isLong"></param>
    public void AdjustColliderToLong(bool isLong)
    {
        if (isLong)
        {
            btnCollider.transform.localPosition = new Vector3(0, 30, 0);
        }
        else
        {
            btnCollider.transform.localPosition = Vector3.zero;
        }
    }

    #region 拖拽 & 多选

    /// <summary>
    /// 底部牌左右移动中
    /// </summary>
    public bool isMoving = false;
    void CheckTouchSingleOrMulity(out bool isReturn)
    {
        var selfManager = SelfCardsManager.Instance;
        isReturn = false;
        //获取
        if (!selfManager.IsPlaySingleOrMulity)
        {
            var list = selfManager.GetIdListByHorizontalIdx(GetInRealDicHorizontalIdx);
            if (list.Count == 1)
            {
                return;
            }
            //获取当前所在列所有id
            //I.判断是否全部不为select
            //则自动选中当前列 全部为select

            //II.判断全部为select状态，
            //清掉所在列其他 所有id select状态

            if (selfManager.IsAllNotSelect(list))
            {
                selfManager.PostSendCardToSelectStatus(list, ECardStatus.ESelected);
                isReturn = true;
            }
            else if (selfManager.IsAllSelect(list))
            {
                selfManager.PostSendCardToSelectStatus(list, ECardStatus.ENormal);
            }
        }
    }
    void ReleaseDragDrop(LocalNotification e)
    {
        if (mIsIn && IsCanChangeStatus && this.mId != 1)
        {
            var selfManager = SelfCardsManager.Instance;
            //让拖拽悬浮card 回位
            if (selfManager.mCurSelectCardIds.Contains(mId))
            {
                var targetPos = selfManager.GetRealCardPos(selfManager.mRealCardDic, mId);
                ;
                Sequence quence = DOTween.Sequence();
                quence.Append(this.transform.DOLocalMove(targetPos, GlobalData.mCardTouchTime));
                quence.AppendCallback(() => SelfCardsManager.Instance.mIsDowing = false);
                quence.Play();
            }
        }
    }


    void MoveCardPosX(Vector3 targetPos)
    {
        isMoving = true;
        var manager = SelfCardsManager.Instance;
        Tween tween = DOTween.To(() => transform.localPosition, r => transform.localPosition = r, targetPos, GlobalData.mCardTouchTime / 2).SetEase(Ease.InExpo);
        tween.OnComplete(() =>
        {
            isMoving = false;

        }
        );

        tween.Play();
    }



    void DragDropMoving(LocalNotification e)
    {
        if (!IsCanChangeStatus || mId == 1) { return; }

        var manager = SelfCardsManager.Instance;
        if (!manager.mCurSelectCardIds.Contains(this.mId)) { return; }

        ArgsDragDroping args = e.param as ArgsDragDroping;
        if (args != null)
        {
            //Debug.Log("args.deltaPosX :" + args.deltaPosX);
            this.transform.localPosition += new Vector3(args.deltaPosX, 0, 0);
        }
    }


    private void FixedUpdate()
    {
        if (SelfCardsManager.Instance.mIsDragDroping)
        {
            //mid ==1 为不要.排除
            if (mIsIn && this.mId != 1 && IsCanChangeStatus && SelfCardsManager.Instance.mCurSelectCardIds.Count > 0)
            {
                if (isMoving) { return; }

                Vector3 mMoveTargetPos;
                var selfManager = SelfCardsManager.Instance;
                //  Vector3 targetPos = Vector3.zero;
                if (selfManager.mInitDrogDropIdx < GetInRealDicHorizontalIdx)
                {
                    float xTarget = selfManager.GetSingleCardById(selfManager.mCurSelectCardIds[0]).transform.localPosition.x;
                    float xSelf = this.transform.localPosition.x;
                    float xDis = selfManager.GetCurDistanceCardPosX(selfManager.mRealCardDic.Count);
                    if (xSelf > xTarget)
                    {
                        //this.transform.localPosition = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId) /*+ new Vector3(xDis, 0, 0)*/;
                        //  Debug.Log("=============selfManager.mRealCardDic.count:" + selfManager.mRealCardDic.Count + "this.mid:" + this.mId);
                        mMoveTargetPos = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId);
                        MoveCardPosX(mMoveTargetPos);
                    }
                    else if (xSelf < xTarget)
                    {
                        //  Debug.Log("=============selfManager.mRealCardDic.count:" + selfManager.mRealCardDic.Count + "this.mid:" + this.mId);
                        mMoveTargetPos = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId) - new Vector3(xDis, 0, 0);
                        // this.transform.localPosition = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId) - new Vector3(xDis, 0, 0);
                        MoveCardPosX(mMoveTargetPos);
                    }
                }
                else if (selfManager.mInitDrogDropIdx > GetInRealDicHorizontalIdx)
                {
                    float xTarget = selfManager.GetSingleCardById(selfManager.mCurSelectCardIds[0]).transform.localPosition.x;
                    float xSelf = this.transform.localPosition.x;
                    float xDis = selfManager.GetCurDistanceCardPosX(selfManager.mRealCardDic.Count);
                    if (xSelf > xTarget)
                    {
                        // this.transform.localPosition = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId) + new Vector3(xDis, 0, 0);
                        mMoveTargetPos = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId) + new Vector3(xDis, 0, 0);
                        MoveCardPosX(mMoveTargetPos);
                    }
                    else if (xSelf < xTarget)
                    {
                        //   this.transform.localPosition = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId);
                        mMoveTargetPos = selfManager.GetRealCardPos(selfManager.mRealCardDic, this.mId);
                        MoveCardPosX(mMoveTargetPos);
                    }
                }
            }
        }
    }

    #endregion
}
