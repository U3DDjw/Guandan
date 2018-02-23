using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZhiWa;
using System.IO;
using Net;
using @public;
using ProtoBuf;

public class TCPNetWork : MonoBehaviour {

    private static TCPNetWork _instance;
    public Action onDisConnect;
    private bool _isOnConn;
    private bool _isOnDisConn;
    private bool _connParam;

    private TCPWorkInterface inter;
    public MessagePool MsgPool;

    public static TCPNetWork GetInstance()
    {
        if(_instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = "NetWork";
            DontDestroyOnLoad(obj);
            _instance = obj.AddComponent<TCPNetWork>();
        }
        return _instance;
    }

    private void Awake()
    {
        MsgPool = new MessagePool();
        inter = new TCPWorkInterface();
    }

    public void Update()
    {
        if (inter != null)
        {
            inter.Process();
        }
    }

    public void Connect(string ip,int port,Action conSucCallBack)
    {
        inter.ConnectTo(ip, port,conSucCallBack);
    }


    /// <summary>
    /// 关闭，并立刻重启Socket
    /// </summary>
    public void ReNowConnect()
    {
        Close();
        inter.ReNowConnect();
    }

    
    public void SendMsgToServer(string key, MsgGlobal msgGlobal)
    {
        msgGlobal.session = new MsgSession();    
        SetSession(msgGlobal.session, key);
        Debug.Log("SendMsgToServer.session:" + msgGlobal.session);
        Debug.Log("msgGlobal:" + msgGlobal);

        SendByteToServer(Serial(msgGlobal));
        Debug.Log("SendMsgTo Server :" + key);
    }

    public void SendMsgToServer(string key)
    {
        MsgGlobal msg = new MsgGlobal();
        msg.session = new MsgSession();
        SetSession(msg.session,key);
        SendByteToServer(Serial(msg));
        Debug.Log("SendMsgTo Server :" + key);
    }

    void SetSession(MsgSession msgSession,string key)
    {
        msgSession.pid = (ulong)PlayerInfo.Instance.mPlayerPid;
        msgSession.actionCode = key;
        msgSession.roomcode = RoomInfo.Instance.mRoomCode;
    }
    private byte[] Serial(ProtoBuf.IExtensible data)//将SocketModel转化成字节数组
    {
        using (MemoryStream m = new MemoryStream())
        {
            byte[] buffer = null;
            Debug.Log("Serial----data" + data);
            Serializer.Serialize(m, data);
            m.Position = 0;
            int length = (int)m.Length;
            buffer = new byte[length];
            m.Read(buffer, 0, length);
            return buffer;
        }
    }

    void SendByteToServer(byte[] buf)
    {
       Send(buf);
    }

    public void AddCallback(string msgtype, MsgServerHandler callback)
    {
        MsgPool.addCallback(msgtype, callback);
    }


    public void Execute(MsgGlobal msg)
    {
        MsgPool.Execute(msg);
    }

     void Send(byte[] bytes) // 传给服务器，先给 实际内容长度的一个字节（这里跟一般4字节为一个长度不同，需要注意） +实际内容（序列化）
    {
        var len = bytes.Length;

        Message msg = new Message();
        EncodeBytes(msg, len, bytes);
        inter.Send(msg.ReadBytes());
    }


    /// <summary>
    /// 编码：采用 数值压缩存储方法
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="length"></param>
    /// <param name="dataBytes"></param>
    void EncodeBytes(Message msg, int length,Byte[] dataBytes)
    {
        msg.WriteRaw(length);
        msg.AddBuffers(dataBytes, dataBytes.Length);
    }
    

    public bool Vaild()
    {
        return inter.Vaild();
    }

    public void Close()
    {
        if (inter != null)
        {
            Debug.Log("TCPNet Work Close");
            inter.Close();
        }
    }

   
    private void OnDestroy()
    {
        Close();
    }
}
