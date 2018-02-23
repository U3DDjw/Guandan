using ZhiWa;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Net;
using HaoyunFramework;

public enum SocketErrorType
{
    SocketException,
    Exception,
}
public class TCPWorkInterface {

    public const int MAX_BUFFER_SIZE = 65535;

    public const int MAX_PACKETS_PER_FRAME = 200;

    public const float CONNECT_TIME_OUT = 15;
    const float CHECK_ERROR_TIME = 5f; //检测 异常 间隔时间

    public class ConnectState
    {
        public bool success = true;
        public string connectIP = "";
        public int connectPort = 0;
        public Socket socket = null;
        public TCPWorkInterface netWorkInterface = null;
        public SocketErrorType errorType;
        public string error = "";
    }


    public TCPClientWorker m_tcpWorker;
    private Socket m_socket;

    public int gateType = 0;
    public string ip = "";

    private int port;
    public bool connectCallBackFlag = false;
    public bool Connecting { get; set; }

    private static Action onConSucHandler;

    public TCPWorkInterface()
    {
        this.Connecting = false;
    }

    public bool Vaild()
    {
        if(this.m_socket!=null && (this.m_socket.Connected) && m_tcpWorker!=null &&m_tcpWorker.SocketVaild)
        {
            return true;
        }
        if(this.m_socket!=null&&m_tcpWorker!=null)
        Debug.LogFormat("socket.connected:{0},socketValid:{1}", this.m_socket.Connected, this.m_tcpWorker.SocketVaild);

        return false;
    }

    public void Close()
    {
        if(this.m_socket!=null)
        {
            try
            {
                this.m_socket.Shutdown(SocketShutdown.Both);
                this.m_socket.Close();
                this.m_socket = null;
            }
            catch(Exception ex)
            {
                Debug.LogFormat("Close Exception:{0}", ex.ToString());
            }
        }
        if(m_tcpWorker!=null)
        {
            m_tcpWorker.Close();
            m_tcpWorker = null;
        }
    }

    public void Send(byte[] data)
    {
        if(m_tcpWorker!=null)
        {
            m_tcpWorker.Push(data);
        }
    }

    public void ConnectTo(string ip,int port,Action conSucCallBack)
    {

        if (Vaild())
        {
            Debug.Log("Hava already connected");
        }
        TCPWorkInterface.onConSucHandler = conSucCallBack;

        string newip = ip;
        //这边应还有IOS ipv6的处理。

        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, MAX_BUFFER_SIZE);

        m_socket.NoDelay = true; //启动Nagle分块算法进行传输，（优化传输效率）
        Debug.Log("DonFragment:" + m_socket.DontFragment); //默认flase，则准许分片
        ConnectState state = new ConnectState();
        this.ip = newip;
        this.port = port;
        state.connectIP = newip;
        state.connectPort = port;
        state.socket = m_socket;
        state.netWorkInterface = this;
        Debug.Log("connect To:" + newip + ":" + port);

        try
        {
            m_socket.BeginConnect(new IPEndPoint(IPAddress.Parse(newip), port), new AsyncCallback(ConnectCB), state);
            connectCallBackFlag = false;
            TCPNetWork.GetInstance().StartCoroutine(WaitForConnect(state));
            this.Connecting = true;
        }
        catch(SocketException e)
        {
            Debug.LogFormat("connect To error:{0}", e.ToString());
            state.errorType = SocketErrorType.SocketException;
            state.error = e.ToString();
            state.success = false;
            OnConnectState(state);
        }
        catch(Exception e)
        {
            Debug.LogFormat("Connect To error :{0}", e.ToString());
            state.errorType = SocketErrorType.Exception;
            state.error = e.ToString();
            state.success = false;
            OnConnectState(state);
        }

    }


    /// <summary>
    /// 检测是否连接超时
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private IEnumerator WaitForConnect(ConnectState state)
    {
        float connectTimeAcc = 0.0f;
        bool connectTimeOut = false;
        while(!state.netWorkInterface.connectCallBackFlag)
        {
            connectTimeAcc += Time.deltaTime;
            if(connectTimeAcc>CONNECT_TIME_OUT)
            {
                connectTimeOut = true;
                break;
            }
            yield return null;
        }

        if(connectTimeOut)
        {
            state.error = "connect time out";
            state.errorType = SocketErrorType.SocketException;
            state.success = false;
            OnConnectState(state);
        }
        else
        {
            OnConnectState(state);
        }
    }

    private static void ConnectCB(IAsyncResult ar)
    {
        ConnectState state = null;
        Debug.Log("Socket 连接完成回调");
        try
        {
            state = (ConnectState)ar.AsyncState;
            state.socket.EndConnect(ar);
            //state.success 默认true
        }
        catch(SocketException e)
        {
            state.errorType = SocketErrorType.SocketException;
            state.error = e.ToString();
          //  state.success = true; 
            Debug.LogError("SocketException:" + state.error);
        }
        catch(Exception e)
        {
            state.errorType = SocketErrorType.Exception;
            state.error = e.ToString();
            state.success = false;
            Debug.LogError("Exception:" + state.error);

        }

        state.netWorkInterface.connectCallBackFlag = true;
    }


    private void StartTCPWorker()
    {
        if (gateType == 1)
        {
            m_socket.ReceiveTimeout = 20000;//20s
        }
       
        m_tcpWorker = new TCPClientWorker(m_socket);
    }
    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="state"></param>
    private static void OnConnectState(ConnectState state)
    {
        Debug.Log("Connect Status");
        TCPWorkInterface networkInterface = state.netWorkInterface;
        networkInterface.Connecting = false;
        if(state.success)
        {
            //创建接收线程和发送线程
            networkInterface.StartTCPWorker();
            OnConnSucFunc();
            //启动协程判断网络是否断开(暂时备用 )
            TCPNetWork.GetInstance().StartCoroutine(CheckNetStateLoop(state));
        }
        else
        {
            networkInterface.Close();
            Debug.Log("connect error :" + state.success);
            //  增加检测重连5s一次
        }
    }


    static void OnConnSucFunc()
    {
        Debug.Log("OnConnSucFunc");
        if(TCPWorkInterface.onConSucHandler != null)
        {
            TCPWorkInterface.onConSucHandler();
        }
    }

    public static IEnumerator CheckNetStateLoop(ConnectState state)
    {
        while(state.netWorkInterface.Vaild())
        {
            yield return new WaitForSeconds(CHECK_ERROR_TIME);
        }
        //这边增加网络断开，回调接口
        Debug.Log("网络异常或断开");
        TweenManager.Instance.SwitchLoadingSpr(true);
        //开启协程不断请求，直到请求成功
        yield return new WaitForSeconds(CHECK_ERROR_TIME);
      
        if(GameManager.Instance.mIsBackToLoginModule) //断线重连则不进行不断登录
        {
            TweenManager.Instance.SwitchLoadingSpr(false);
            yield break;
        }
        else
        ReConnect(state);
    }


    static void ReConnect(ConnectState state)
    {
        Debug.Log("开始重新连接");

        TCPNetWork.GetInstance().Close();
        TCPNetWork.GetInstance().Connect(state.connectIP, state.connectPort, PlayingGameInfo.Instance.SendReconnectServer);
    }



    /// <summary>
    /// 重新连接Socket
    /// </summary>
    public void ReNowConnect()
    {
        TCPNetWork.GetInstance().Connect(this.ip, this.port, PlayingGameInfo.Instance.SendReconnectServer);
    }
   

    public  IEnumerator ReConnectTCP(TCPWorkInterface inter)
    {
        while(!inter.Vaild()) // 异常情况则不断检测重新连接
        {
            StartTCPWorker();
            yield return new WaitForSeconds(5.0f);
        }
    }
    public void Process()
    {
        int hasReceived = 0;
        while(hasReceived<MAX_PACKETS_PER_FRAME)
        {
            if(m_tcpWorker == null) { return; }

            byte[] data = m_tcpWorker.Recv();
            if(data == null ||data.Length ==0)
            {
                break;
            }
            else
            {
                Execute(data);
                hasReceived++;
            }

        }
    }

    void Execute(byte[] bytes)
    {
        var data = DeSerial(bytes);
        TCPNetWork.GetInstance().Execute(data);
    }
    private MsgGlobal DeSerial(byte[] msg)//将字节数组转化成我们的消息类型SocketModel
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(msg, 0, msg.Length);
            ms.Position = 0;
            return  Serializer.Deserialize<MsgGlobal>(ms);
        }
    }





}
