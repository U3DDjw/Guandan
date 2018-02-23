using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 重写了脚本ScrollRect中部分方法
/// </summary>
public class AutoMoveScrollRect : ScrollRect
{
    private float DragBefore, DragAfter;
    private AutoScorllMove autoMove;
    protected override void Start()
    {
        base.Start();
        this.inertia = false;
        autoMove = this.GetComponent<AutoScorllMove>();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        autoMove.isContinueAutoMove = false;
        DragBefore = horizontalScrollbar.value;
        StopLinkCoroutine();
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        DragAfter = horizontalScrollbar.value;
        SetToCenterDir();
    }
    void SetToCenterDir()
    {
        if (DragBefore > DragAfter)
        {
            autoMove.ChildToViewportCenter(MoveDirection.Left);
        }
        else if (DragBefore < DragAfter)
        {
            autoMove.ChildToViewportCenter(MoveDirection.Right);
        }

    }
    void StopLinkCoroutine()
    {
        StopCoroutine("ContentAutoMove");
        StopCoroutine("StartToCenterMove");
    }
}
