using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Sharer.Services
{
    public static class IPManager
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            
            return string.Empty;
        }

        public static int GetAvailablePort(int startingPort = 5000)
        {
            IPEndPoint[] endPoints;
            List<int> portArray = new List<int>();

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            portArray.AddRange(from n in connections
                            where n.LocalEndPoint.Port >= startingPort
                            select n.LocalEndPoint.Port);

            //getting active tcp listners - WCF service listening in tcp
            endPoints = properties.GetActiveTcpListeners();
            portArray.AddRange(from n in endPoints
                            where n.Port >= startingPort
                            select n.Port);

            //getting active udp listeners
            endPoints = properties.GetActiveUdpListeners();
            portArray.AddRange(from n in endPoints
                            where n.Port >= startingPort
                            select n.Port);

            portArray.Sort();

            for (int i = startingPort; i < UInt16.MaxValue; i++)
                if (!portArray.Contains(i))
                    return i;

            return 0;
        }

        public static string[] GenAppRunAddress()
        {
            string myIp = GetLocalIPAddress();
            string availablePort = GetAvailablePort()
                .ToString();
            
            int port2 = GetAvailablePort(); port2++;
            string availablePort2 = port2.ToString();
            
            return new string[]
            {
                $"https://{myIp}:{availablePort}",
                $"https://{myIp}:{availablePort2}"
            };
        }
    }
}