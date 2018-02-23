using DNotificationCenterManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaoyunFramework;

public class UIDragDropArrow : BasesView
{
  //  int curIdx = -1; 
    void Awake()
    {
        NotificationCenter.Instance().AddEventListener((uint)ENotificationMsgType.EDragDroping, DragDropMoving);
    }

    void OnDestroy()
    {
        NotificationCenter.Instance().RemoveEventListener((uint)ENotificationMsgType.EDragDroping, DragDropMoving);
    }

    void DragDropMoving(LocalNotification e)
    {
        var manager = SelfCardsManager.Instance;
        float xTarget = manager.GetSingleCardById(manager.mCurSelectCardIds[0]).transform.localPosition.x;
        int curHorizontalIdx = manager.GetRealHorizontalIdxByPostion(xTarget);
        float posX = SelfCardsManager.Instance.GetPosXByHorizontalIdx(curHorizontalIdx);
        this.transform.localPosition = new Vector3(posX, -280, 0);
    }
}
