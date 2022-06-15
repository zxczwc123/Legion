using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Framework.Utility
{
    public class NetUtility
    {
        public static string GetIP()
        {
            string hostName = Dns.GetHostName();//本机名   
            IPAddress[] addressList = Dns.GetHostEntry(hostName).AddressList;
            foreach (IPAddress ipAddress in addressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    string strLocalIP = ipAddress.ToString();
                    return strLocalIP;
                }
            }
            return null;
        }
    }
}

