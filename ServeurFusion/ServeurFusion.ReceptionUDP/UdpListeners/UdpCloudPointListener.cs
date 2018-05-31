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
    public class UdpSkeletonListener
    {
        private UdpThreadInfos _threadInfos;

        public UdpSkeletonListener(DataTransferer<Skeleton> dataTransferer, int port)
        {
            _threadInfos = new UdpThreadInfos(dataTransferer, port);
        }

        private void StartListening(object threadInfos)
        {
            UdpThreadInfos ti = (UdpThreadInfos)threadInfos;
            Console.WriteLine("Thread udp démarrée");

            UdpClient udp = new UdpClient(ti._port);
            var remoteEP = new IPEndPoint(IPAddress.Any, 9877);

            while (true)
            {
               /* // Receiving frames from KinectStreamer
                var data = udp.Receive(ref remoteEP);
                
                ti._dataTransferer.AddData(data);
                Console.WriteLine("Ajout d'un skeleton à la liste : " + skeleton.ToString());*/
            }
        }

        public void Listen()
        {
            Thread th = new Thread(new ParameterizedThreadStart(StartListening));

            th.Start(_threadInfos);
        }
    }

    public class UdpThreadInfos
    {
        public DataTransferer<Skeleton> _dataTransferer { get; set; }
        public int _port = 9877;

        public UdpThreadInfos(DataTransferer<Skeleton> dataTransferer, int port)
        {
            _dataTransferer = dataTransferer;
            _port = port;
        }
    }
}
