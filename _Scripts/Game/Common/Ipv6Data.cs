using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
#if UNITY_IPHONE
using System.Runtime.InteropServices; 
#endif
public class Ipv6Data : MonoBehaviour {

#if UNITY_IPHONE && !UNITY_EDITOR 
   [DllImport("__Internal")] 
   private static extern string getIPv6(string host, string port); 
#endif 



    //"192.168.1.1&&ipv4"
    public static string GetIPv6(string host, string port)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
       string ipv6 = getIPv6(host, port); 
       return ipv6; 
#else
        return host + "&&ipv4";
#endif
    }

    //void getIPType(String serverIp, String serverPorts, out String newServerIp, out AddressFamily mIPType)
    //{
    //    mIPType = AddressFamily.InterNetwork;
    //    newServerIp = serverIp;
    //    try
    //    {
    //        string mIPv6 = GetIPv6(serverIp, serverPorts);
    //        if (!string.IsNullOrEmpty(mIPv6))
    //        {
    //            string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
    //            if (m_StrTemp != null && m_StrTemp.Length >= 2)
    //            {
    //                string IPType = m_StrTemp[1];
    //                if (IPType == "ipv6")
    //                {
    //                    newServerIp = m_StrTemp[0];
    //                    mIPType = AddressFamily.InterNetworkV6;
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("GetIPv6 error:" + e);
    //    }

    //}

    //public SocketClient(String serverIp, String serverPorts)
    //{
    //    String newServerIp = "";
    //    AddressFamily newAddressFamily = AddressFamily.InterNetwork;
    //    getIPType(serverIp, serverPorts, out newServerIp, out newAddressFamily);
    //    if (!string.IsNullOrEmpty(newServerIp)) { serverIp = newServerIp; }
    //    socketClient = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
    //    ClientLog.Instance.Log("Socket AddressFamily :" + newAddressFamily.ToString() + "ServerIp:" + serverIp);
    //}
}
