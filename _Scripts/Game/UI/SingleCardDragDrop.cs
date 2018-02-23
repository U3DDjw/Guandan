using DNotificationCenterManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MsgContainer;


public class SingleCardDragDrop : MonoBehaviour {
     SingleCard singleCard;
    void Start()
    {
        singleCard = this.transform.parent.parent.GetComponent<SingleCard>();
        EventTriggerListener.Get(this.gameObject).onDrag = OnDrag;
        EventTriggerListener.Get(this.gameObject).onLongPress = OnLongPress;
        EventTriggerListener.Get(this.gameObject).onUp = OnUp;
    }

    void OnLongPress(GameObject g)
    {
        Debug.Log("OnLongPress ");
        if (SelfCardsManager.Instance.mCurSelectCardIds.Count == 0 || SelfCardsManager.Instance.mIsDowing)
        {
            return;
        }
        _isPressed = true;
        DragDropStart();

    }

   void OnUp(GameObject g)
    {
        Debug.Log("OnUp ");
        _isPressed = false;

        if (SelfCardsManager.Instance.mIsDragDroping)
        {
            DragDropRelease();
        }
    }

    void DragDropRelease()
    {
        Debug.Log("this.Release mId:" + singleCard.mId);
        SelfCardsManager.Instance.mIsDowing = true;
        StartCoroutine(ReleaseCard());
    }

    void DragDropStart()
    {
        if (SelfCardsManager.Instance.mCurSelectCardIds.Count == 0)
        {
            SelfCardsManager.Instance.mIsDragDroping = false;
            SelfCardsManager.Instance.mIsDowing = false;
            ResetPressStatus();
            return;
        }
        SelfCardsManager.Instance.mIsDragDroping = true;

        ArgsStartDragDrop args = new ArgsStartDragDrop();
        args.mCardId = singleCard.mId;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EStartDragDrop, args);
        Debug.Log("DragDropStart");
    }
    IEnumerator ReleaseCard()
    {
        yield return new WaitForSeconds(0.15f);
        var selfManager = SelfCardsManager.Instance;
        selfManager.mIsDragDroping = false;
        float xTarget = selfManager.GetSingleCardById(selfManager.mCurSelectCardIds[0]).transform.localPosition.x;
        int curHorizontalIdx = selfManager.GetRealHorizontalIdxByPostion(xTarget);
        //string strDebug = string.Format("position.x:{0},curHorizontalIdx:{1}", singleCard.transform.localPosition.x, curHorizontalIdx);
    //    Debug.Log(strDebug);
        selfManager.ReverseTwoListInRealDic(selfManager.mInitDrogDropIdx, curHorizontalIdx);
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EReleaseDragDrop);
        PostNotifyClearSelectCards();
    } 

    /// <summary>
    /// 清空已经select的牌
    /// </summary>
    void PostNotifyClearSelectCards()
    {
        var selfManager = SelfCardsManager.Instance;
        var list = selfManager.mCurSelectCardIds;
        selfManager.ResetSibling(selfManager.mRealCardDic);
        SelfCardsManager.Instance.PostSendCardToSelectStatus(list, ECardStatus.ENormal);
    }

      void OnDrag(GameObject g,Vector2 delta)
    {
           Debug.Log("OnDrag Id" + singleCard.mId);
        if (_isPressed && SelfCardsManager.Instance.mIsDragDroping)
        {
            ArgsDragDroping args = new ArgsDragDroping();
            args.deltaPosX = delta.x * GlobalData.mDragDrogTouchRate;
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.EDragDroping, args);
        }
    }

    public void ResetPressStatus()
    {
         _isPressed = false; //是否按下
    }
    bool _isPressed = false; //是否按下
  
}
