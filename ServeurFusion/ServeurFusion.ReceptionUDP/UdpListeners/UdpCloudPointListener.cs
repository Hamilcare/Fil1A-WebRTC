using ServeurFusion.ReceptionUDP.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.UdpListeners
{
    public class UdpCloudPointListener : UdpListener<Array>
    {
        public UdpCloudPointListener(DataTransferer<Array> dataTransferer, int port)
        {
            this._udpThreadInfos = new UdpThreadInfos<Array>(dataTransferer, port);
        }

        override protected void StartListening(object threadInfos)
        {
            UdpThreadInfos<Array> ti = (UdpThreadInfos<Array>)threadInfos;
            Console.WriteLine("Thread udp démarrée");

            UdpClient udp = new UdpClient(ti._port);
            var remoteEP = new IPEndPoint(IPAddress.Any, 9877);

            while (true)
            {
            }
        }

    }

}
