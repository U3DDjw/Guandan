using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net;
using DNotificationCenterManager;

public class AppFouse : MonoBehaviour
{

    void OnEnable()
    {
    }


    void OnApplicationFocus(bool isFocus)
    {
        Debug.Log("--------OnApplicationPause---" + isFocus);
        if (isFocus)
        {
            if (SDKManager.Instance != null)
                SDKManager.Instance.GetClipRoomCodeEnterRoom();
        }
        else
        {
            // Debug.Log("离开游戏");  //  返回游戏的时候触发     执行顺序 1
        }
    }



}
