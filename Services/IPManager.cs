using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Sharer.Services
{
    public static class IPManager
    {
        public static List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ips = new List<string>();

            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    ips.Add(ip.ToString());
            
            return ips == null ? null : ips;
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
            {
                if (!portArray.Contains(i))
                    return i;
            }

            return 0;
        }

        public static string[] GenAppRunAddress()
        {
            var mips = GetLocalIPAddress();
            var port = GetAvailablePort(); 

            if(mips != null && port != 0)
            {
                string availablePort = GetAvailablePort()
                    .ToString();
            
                var addresses = new List<string>();
                int count = 1;
                
                foreach (var ip in mips)
                {
                    if(count >= 20)
                        break;
                    
                    addresses.Add($"https://{ip}:{port}");
                    count++;

                    var cert = JsonConvert.SerializeObject(CertificateService.CreateSelfSignedCertificate(ip), Formatting.Indented);
                    FileOperationService.SaveFile(cert, "/Certs");
                }

                return addresses.ToArray();
            }   

            return null;
        }
    }
}