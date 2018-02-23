using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using ZhiWa;
using @public;
using MsgContainer;
using DNotificationCenterManager;

namespace Net
{
    public delegate void MsgServerHandler(MsgGlobal msg);

    public class MessagePool
    {

        public MessagePool()
        {
            m_CallBacks = new Dictionary<string, MsgServerHandler>();
        }
     
        void TestDebugInLog(string str)
        {
            ArgsMsgTest args = new ArgsMsgTest();
            args.testStr = str;
            NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ENull, args);
        }
        public void addCallback(string msgtype, MsgServerHandler callback)
        {
            if (!m_CallBacks.ContainsKey(msgtype))
            {
                m_CallBacks[msgtype] = callback;
            }
        }
      

        void RefreshSession(MsgSession session)
        {
            PlayerInfo.Instance.mPlayerData.roomCode = session.roomcode;
         
        }

        public void Execute(MsgGlobal msg)
        {
            var callBackCode = msg.session.callbackCode;
            RefreshSession(msg.session);

            if (m_CallBacks.ContainsKey(callBackCode))
            {
                Debug.Log("========================Execute:" + callBackCode);
                try
                {
                    m_CallBacks[callBackCode](msg);
                }
                catch (Exception what)
                {
                    //这边应该添加 向服务器发送数据
                    Debug.LogError(what.StackTrace);
                    Debug.LogError(what.Message);
                    Debug.LogError("!!!!!!!!RecvMsg Execute:" +callBackCode);
                }
        }
            else
            {
                Debug.LogError("Msg Not Found Callback: " + callBackCode);
            }
        }

        private Dictionary<string, MsgServerHandler> m_CallBacks;
    }
}