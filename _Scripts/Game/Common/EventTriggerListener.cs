using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class EventTriggerListener : EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;
    
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onLongPress; //长按事件
    public delegate void FloatDelegate(GameObject go, float delta);
    public delegate void VectorDelegate(GameObject go, Vector2 delta);
    public delegate void ObjectDelegate(GameObject go, GameObject obj);
    public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

    public delegate void BoolDelegate(GameObject go, bool state);
    public BoolDelegate onPress;

    public VectorDelegate onDrag;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    void OnPress(bool isPressed)
    {
        if (onPress != null)
        {
            onPress(gameObject, isPressed);
        }
    }
  
    public override void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log("OnPointerClick" + gameObject.transform.parent.parent.name);
        if (!longPressTriggered)
        {
            if (onClick != null) onClick(gameObject);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
       // base.OnDrag(eventData);
        if (onDrag != null)
            onDrag(gameObject, eventData.delta);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {

        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;

        OnPress(true);
      //  Debug.Log("OnPointerDown" + gameObject.transform.parent.parent.name);
        if (onDown != null)
            onDown(gameObject);
    }

    /// <summary>
    /// 该Enter专为SingleCard拖拽牌用 为解决  "发送给一个对象，当另一个对象被拖到它的区域时" 该问题
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null)
        {
                onEnter(gameObject);
        }

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (longPressTriggered) {
            isPointerDown = false;
            longPressTriggered = false;
            return;
        }

        isPointerDown = false;
        if (onExit != null) onExit(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        OnPress(false);
        if (onUp != null) onUp(gameObject);
    }
   
    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;
    public float durationThreshold = 0.6f;
    private void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            if (Time.time - timePressStarted > durationThreshold)
            {
                longPressTriggered = true;
                if(onLongPress!=null)
                onLongPress(gameObject);
            }
        }
    }








    //public override void OnSelect(BaseEventData eventData)
    //{
    //    if (onSelect != null) onSelect(gameObject);
    //}
    //public override void OnUpdateSelected(BaseEventData eventData)
    //{
    //    if (onUpdateSelect != null) onUpdateSelect(gameObject);
    //}
}