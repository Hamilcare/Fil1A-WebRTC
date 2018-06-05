using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.UdpListeners;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServeurFusion.ReceptionUDP
{
    public class UdpSkeletonListener : UdpListener<Skeleton>
    {
        private UdpClient _udp;
        public UdpSkeletonListener(BlockingCollection<Skeleton> dataTransferer, int port)
        {
            _udpThreadInfos = new UdpThreadInfos<Skeleton>(dataTransferer, port);
        }

        override protected void StartListening(object threadInfos)
        {
            UdpThreadInfos<Skeleton> ti = (UdpThreadInfos<Skeleton>)threadInfos;
            Console.WriteLine("Thread udp démarré");

            _udp = new UdpClient(ti.Port);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, ti.Port);

            while (true)
            {
                // Receiving frames from KinectStreamer
                var data = _udp.Receive(ref remoteEP);
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
                
                ti.DataTransferer.Add(skeleton);
            }
        }

        override protected void StopListening()
        {
            _udp.Close();
        }
    }
}
