using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DNotificationCenterManager
{
    public delegate void NotificationDelegate(LocalNotification notific);
    public class NotificationCenter
    {
        private static NotificationCenter instance = null;
        public static NotificationCenter Instance()
        {
            if (instance == null)
            {
                instance = new NotificationCenter();
                return instance;
            }
            return instance;
        }

        private Dictionary<uint, NotificationDelegate> eventListeners
            = new Dictionary<uint, NotificationDelegate>();
        public void AddEventListener(uint eventKey, NotificationDelegate listener)
        {
            if (!HasEventListener(eventKey))
            {
                eventListeners[eventKey] = listener; //注册接收者的监听
            }
            else
            {
                eventListeners[eventKey] += listener; //注册接收者的监听
            }
        }
        public void RemoveEventListener(uint eventKey, NotificationDelegate listener)
        {
            if (!HasEventListener(eventKey))
                return;
            eventListeners[eventKey] -= listener;

            if (eventListeners[eventKey] == null)
            {
                RemoveEventListener(eventKey);
            }
        }
        public void RemoveEventListener(uint eventKey)
        {
            eventListeners.Remove(eventKey);
        }

        /// <summary>
        /// 分发事件，需要知道发送者，具体消息的情况
        /// </summary>
        ///<param name="eventKey">事件Key
        ///<param name="sender">发送者
        ///<param name="param">通知内容
        public void PostDispatchEvent(uint eventKey, GameObject sender, EventArgs param)
        {
            if (!HasEventListener(eventKey))
                return;
            eventListeners[eventKey](new LocalNotification(sender, param));
        }
        public void PostDispatchEvent(uint eventKey)
        {
            if (!HasEventListener(eventKey))
                return;
            eventListeners[eventKey](new LocalNotification());
        }

        /// <summary>
        /// 分发事件，不需要知道任何，只需要知道发送过来消息了
        /// </summary>
        ///<param name="eventKey">事件Key
        ///<param name="param">通知内容
        public void PostDispatchEvent(uint eventKey, EventArgs param)
        {
            if (!HasEventListener(eventKey))
                return;
            eventListeners[eventKey](new LocalNotification(param));
        }

        /// <summary>
        /// 是否存在指定事件的监听器
        /// </summary>
        public bool HasEventListener(uint eventKey)
        {
            return eventListeners.ContainsKey(eventKey);
        }
    }
}

