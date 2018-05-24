using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Reception
{
    public class UdpListener
    {
        public UdpListener()
        {
            
        }

        public void Test()
        {
            byte[] data = new byte[255];

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 9876);
            Socket winSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            winSocket.Bind(serverEndPoint);

            Debug.WriteLine("Waiting for client");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            int recv = winSocket.ReceiveFrom(data, ref Remote);
            Debug.WriteLine("Message received from {0}:", Remote.ToString());
            Debug.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            Debug.WriteLine("Test reception");
        }

        public async void Listen()
        {
            UdpClient udpServer = new UdpClient(9876);
            Debug.WriteLine("Debut fonction");
            while (true)
            {
                Debug.WriteLine("Debut boucle");
                var remoteEP = new IPEndPoint(IPAddress.Any, 9876);
                var data = await udpServer.ReceiveAsync();
                Debug.WriteLine("Fin boucle");
                Debug.WriteLine(data.Buffer);

            }
        }
    }
}
