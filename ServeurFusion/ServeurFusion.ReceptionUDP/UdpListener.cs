using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP
{
    public class UdpListener : IDisposable
    {
        private UdpClient _udpServer;

        public UdpListener()
        {
            _udpServer = new UdpClient(9876);
        }

        public void Listen()
        {
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 9876);
                var data = _udpServer.Receive(ref remoteEP);
                Console.WriteLine("receive data from " + remoteEP.ToString() + " ; Lenght = " + data.Length + " ; content = " + System.Text.Encoding.UTF8.GetString(data));
            }
        }

        public void Dispose()
        {
            if (_udpServer != null)
                _udpServer.Close();
        }
    }
}
