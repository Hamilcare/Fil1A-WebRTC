using ServeurFusion.ReceptionUDP.Datas;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServeurFusion.ReceptionUDP
{
   
    public class UdpSkeletonListener
    {
        private UdpThreadInfos _threadInfos;

        public UdpSkeletonListener(DataCloudPointTransferer dataTransferer, int port)
        {
            _threadInfos = new UdpThreadInfos(dataTransferer, port);
        }

        private void StartListening(object threadInfos)
        {
            UdpThreadInfos ti = (UdpThreadInfos)threadInfos;
            Console.WriteLine("Thread udp démarrée");

            UdpClient udp = new UdpClient(ti._port);
            var remoteEP = new IPEndPoint(IPAddress.Any, 9876);

            while (true)
            {
                // Receiving frames from KinectStreamer
                var data = udp.Receive(ref remoteEP);
                int count = 0;
                // Processing Skeleton
                Skeleton skeleton = new Skeleton()
                {
                    Timestamp = BitConverter.ToInt64(data, 0),
                    Tag = data[8],
                    SkeletonPoints = new List<SkeletonPoint>()
                };
                count = 9;
                while (count < 409)
                {
                    // Processing SkeletonPoints
                    SkeletonPoint skeletonPoint = new SkeletonPoint();
                    skeletonPoint.X = BitConverter.ToSingle(data, count);
                    count += 4;
                    skeletonPoint.Y = BitConverter.ToSingle(data, count);
                    count += 4;
                    skeletonPoint.Z = BitConverter.ToSingle(data, count);
                    count += 4;
                    skeletonPoint.R = data[count];
                    count += 1;
                    skeletonPoint.G = data[count];
                    count += 1;
                    skeletonPoint.B = data[count];
                    count += 1;
                    skeletonPoint.Tag = data[count];
                    count += 1;
                    skeleton.SkeletonPoints.Add(skeletonPoint);
                }
                
                ti._dataTransferer.AddData(skeleton);
                Console.WriteLine("Ajout d'un skeleton à la liste : " +  skeleton.ToString());
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
        public DataCloudPointTransferer _dataTransferer { get; set; }
        public int _port = 9876;

        public UdpThreadInfos(DataCloudPointTransferer dataTransferer, int port)
        {
            _dataTransferer = dataTransferer;
            _port = port;
        }
    }
}
