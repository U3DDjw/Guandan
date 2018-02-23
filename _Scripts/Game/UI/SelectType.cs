using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectType : MonoBehaviour
{
    public  Action<int> selectType;
    [Tooltip("选择状态的物体索引")]
    [SerializeField]
    int selectedIndex = 0;

    [Tooltip("列表对象中的元素\nchild(0)是选择的状态\nchild(1)是未选择状态")]
    [SerializeField]
    List<GameObject> typeContainerList = new List<GameObject>();
    void Start()
    {
        for (int i = 0; i < typeContainerList.Count; i++)
        {
            EventTriggerListener.Get(typeContainerList[i]).onClick = OnTypeClick;
        }
    }
    /// <summary>
    /// 设置房间类型
    /// </summary>
    /// <param name="index">1开始</param>
    public void SetRoomType(int index)
    {
        Start(); //因为set比Start先执行
        OnTypeClick(typeContainerList[index-1]);
    }
    void OnTypeClick(GameObject g)
    {
        for (int i = 0; i < typeContainerList.Count; i++)
        {
            int noSelectedIndex = 1;
            noSelectedIndex = selectedIndex == 0 ? 1 : 0;

            bool isCurClick = g == typeContainerList[i];//是否当前点击的按钮

            typeContainerList[i].transform.GetChild(selectedIndex).gameObject.SetActive(isCurClick);

            typeContainerList[i].transform.GetChild(noSelectedIndex).gameObject.SetActive(!isCurClick);
            EventTriggerListener trigger = typeContainerList[i].GetComponent<EventTriggerListener>();
            if (trigger != null)
            {
                trigger.enabled = !isCurClick;
            }
            if (g == typeContainerList[i] && selectType != null)
                selectType((i + 1));
        }
    }
}
