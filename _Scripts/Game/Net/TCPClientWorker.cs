using DNotificationCenterManager;
using MsgContainer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// TCP Client 类，用于异步收发包
/// 做了并包处理,使用.Net TCP 实现
/// </summary>
public class TCPClientWorker  {
    private readonly Queue<byte[]> m_sendQueue = new Queue<byte[]>();
    private readonly Queue<byte[]> m_recvQueue = new Queue<byte[]>();
    private  int MAX_SEND_BUFFER_SIZE = 4096;
    private  int MAX_RECV_BUFFER_SIZE = 65535; //65535 缓冲区

    private readonly byte[] m_sendBuffer;
    private readonly byte[] m_recvBuffer;
    private int m_nRecvBufferSize = 0;
    private Socket m_socket = null;

    private Thread m_receiveThread;
    private Thread m_sendThread;


    /// <summary>
    /// 接收数据队列同步锁
    /// </summary>
    private readonly object m_recvQueueLocker = new object();
   /// <summary>
   /// 发送数据队列同步锁
   /// </summary>
    private readonly object m_sendQueueLocker = new object();

    /// <summary>
    /// 读取数据流量标记
    /// </summary>
    private Int32 bytesRead;

    private bool m_asynSendSwitch = true;

    public bool SocketVaild = true;
    public TCPClientWorker(Socket socket)
    {
        m_socket = socket;
        this.m_sendBuffer = new byte[MAX_SEND_BUFFER_SIZE];
        this.m_recvBuffer = new byte[MAX_RECV_BUFFER_SIZE];
        StartSendThread();
        StartRecvThread();
    }

    private Thread StartSendThread()
    {
        Debug.Log("Start Send Thread...");
        m_sendThread = new Thread(new ThreadStart(this.AsynSend));
        m_sendThread.IsBackground = true;
        if (!m_sendThread.IsAlive)
        {
            Debug.Log("Start AsynSend:" + this.m_asynSendSwitch);
            m_sendThread.Start();
        }
        return m_sendThread;
    }

    private Thread StartRecvThread()
    {
        Debug.Log("Start Recv Thread...");
        m_receiveThread = new Thread(new ThreadStart(this.AsynRecv));
        m_receiveThread.IsBackground = true;
        m_receiveThread.Start();
        return m_receiveThread;
    }

    private void AsynSend()
    {
        while(this.m_asynSendSwitch)
        {
            this.DoSend();
            Thread.Sleep(20);
        }
    }




    void TestDebugInLog(string str)
    {
      //  return; //不做日志处理
        ArgsMsgTest args = new ArgsMsgTest();
        args.testStr = str;
        NotificationCenter.Instance().PostDispatchEvent((uint)ENotificationMsgType.ENull, args);
    }
    private void AsynRecv()
    {
        int zeroCount = 0;
        int timeout = 0;
        do
        {
            this.bytesRead = 0;
            try
            {
                int size = MAX_RECV_BUFFER_SIZE - this.m_nRecvBufferSize;
                if(size >0)
                {
                    this.bytesRead = this.m_socket.Receive(this.m_recvBuffer, this.m_nRecvBufferSize, size, SocketFlags.None);
                    //  this.bytesRead = this.m_socket.Receive(this.m_recvBuffer, this.m_nRecvBufferSize, size, SocketFlags.None);
                    string strLog = string.Format("m_nRecvBufferSize:{0},bytesRead:{1}", m_nRecvBufferSize, bytesRead);
                    Debug.Log(strLog);
                    TestDebugInLog(strLog);

                    this.m_nRecvBufferSize += this.bytesRead;
                    if(this.bytesRead == 0)
                    {
                        zeroCount++;
                        if (zeroCount < 3)
                        {
                            this.bytesRead = 1;
                        }
                    }
                    else
                    {
                        zeroCount = 0;
                        timeout = 0;
                    }
                }

                this.SplitPackets();
            }
            catch(SocketException e)
            {
                if(e.SocketErrorCode == SocketError.TimedOut &&timeout<3) //Socket异常 超时再超时，则视为异常处理，退出
                {
                    Debug.LogFormat("socket timeout {0}", timeout);
                    timeout++;
                    this.bytesRead = 1;
                }
            }
            catch(Exception e)
            {
                Debug.LogFormat("e:{0}", e.ToString());
                this.bytesRead = 0;
            }

        } while (this.bytesRead > 0);
        SocketVaild = false;
        Debug.Log("DataReceive Thread Exit");
    }


    bool CheckByteLength(int headLength, int dataLength)
    {
        return (this.m_nRecvBufferSize - headLength == dataLength);
    }
    /// <summary>
    /// 分包，块包，粘包等的处理 (暂时未做处理，只是跟服务器一致有数值压缩方法)
    /// </summary>
    void SplitPackets()
    {
        //步骤，得出data长度，截取剩余长度是否大于data长度，大于，则截取，保留剩余字节；
        int offset = 0;
        int headLength = 0;
        int dataLength = 0;
   
        int testCount = 0;
        while (this.m_nRecvBufferSize >= 2) //长度字节至少 1 + data字节长度至少 1
        {
            dataLength = (int)ReadUInt32Variant(this.m_recvBuffer, offset, out headLength);
            string strLog = string.Format("dataLength:{0},headLength:{1}", dataLength, headLength);
            TestDebugInLog(strLog);

            if (this.m_nRecvBufferSize >= dataLength + headLength)
            {
                byte[] packet = new byte[dataLength];
                Buffer.BlockCopy(this.m_recvBuffer, offset+headLength, packet, 0, dataLength);
                lock (this.m_recvQueueLocker) //此处理为独立线程处理，需加锁，否则会出现丢包
                {
                    this.m_recvQueue.Enqueue(packet);
                }
                int packageLength = dataLength + headLength;
                this.m_nRecvBufferSize -= (packageLength);
                offset += packageLength;
            }
            else
            {
                break;
            }

            testCount++;
        }
        if (testCount >= 2)
        {
            TestDebugInLog("$$$$$$$$$$$$$$$$$$$$$粘包了，但是解决了");
        }
        TestDebugInLog("offset:" + offset);

        // 整理 RecvBuffer， 将buffer内容前移
        Buffer.BlockCopy(this.m_recvBuffer, offset, this.m_recvBuffer, 0,
                this.m_nRecvBufferSize);
    }



    /// <summary>
    /// 获取数值压缩存储方法的长度(动态根据前几个字节判断)
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private uint ReadUInt32Variant(byte[] data,int offset, out int headLength)
    {
        headLength = 0;

        uint value = data[offset+0];
        if ((value & 0x80) == 0)
        {
            headLength = 1;
            return value;
        }
        value &= 0x7F;
        uint chunk = data[offset+1];
        value |= (chunk & 0x7F) << 7;
        if ((chunk & 0x80) == 0)
        {
            headLength = 2;
            return value;
        }
            chunk = data[offset+2];
        value |= (chunk & 0x7F) << 14;
        if ((chunk & 0x80) == 0)
        {
            headLength = 3;
            return value;
        }

        chunk = data[offset+3];
        value |= (chunk & 0x7F) << 21;
        if ((chunk & 0x80) == 0)
        {
            headLength = 4;
            return value;
        }
        chunk = data[offset+4]; ;
        value |= chunk << 28;
        if ((chunk & 0xF0) == 0)
        {
            headLength = 5;
            return value;
        }
        throw new OverflowException("ReadUInt32Variant Error!");
    }
    DateTime LastSendMsgTime = DateTime.Now;
    private void DoSend()
    {
        if(this.m_socket == null || this.m_socket.Connected == false)
        {
            return;
        }
      
        if (WaitSendMsg())
        {

        }
        else //没有消息发送的情况
        {
            TimeSpan timeSpan = DateTime.Now - LastSendMsgTime;
            if (timeSpan.TotalSeconds >7)
            {
                LastSendMsgTime = DateTime.Now;
                SendHeartBeat();
            }
        }
    }

    

    void SendHeartBeat()
    {
        byte[] heartBytes = new byte[1] { 0}; //心跳数据包
        int ret = this.m_socket.Send(heartBytes, 0, 1, SocketFlags.None);
       // Debug.Log("心跳包  ret:" + ret+"   NowTime:"+DateTime.Now);
    }

   

    bool WaitSendMsg()
    {
        if (this.m_sendQueue.Count <= 0) { return false; }

        lock (this.m_sendQueueLocker)
        {
            while (this.m_sendQueue.Count > 0)
            {
                byte[] packet = this.m_sendQueue.Peek();
                int length = packet.Length;
                Buffer.BlockCopy(packet, 0, this.m_sendBuffer, 0, length);
                this.m_sendQueue.Dequeue();

                try
                {
                    //发送数据
                    int ret = this.m_socket.Send(this.m_sendBuffer, 0, length, SocketFlags.None);
                    Debug.Log("发送结束 ret Length:" + ret + DateTime.Now.ToString());
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.ToString());
                    SocketVaild = false;
                }
            }
        }

        LastSendMsgTime = DateTime.Now;
        return true;
    }

    public void Push(byte[] bytes)
    {
        lock(this.m_sendQueueLocker)
        {
            this.m_sendQueue.Enqueue(bytes);
        }
    }

    public byte[] Recv()
    {
        if(this.m_recvQueue.Count>0)
        {
            byte[] res;
            lock (this.m_recvQueueLocker)
                res = this.m_recvQueue.Dequeue();
            return res;
        }
        else
        {
            return null;
        }
    }

    public void Close()
    {
        Debug.Log("TcpClientWorker Close:m_SendThread & m_RecvThread");

        lock(this.m_sendQueueLocker)
        {
            this.m_sendQueue.Clear();
            this.m_sendQueue.TrimExcess();
        }
        this.m_asynSendSwitch = false;
        this.m_sendThread = null;
        this.m_receiveThread = null;
        GC.Collect();
    }

}
