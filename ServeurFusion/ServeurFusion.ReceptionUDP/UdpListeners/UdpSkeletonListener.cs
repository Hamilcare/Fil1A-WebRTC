using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.UdpListeners;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServeurFusion.ReceptionUDP
{
    /// <summary>
    /// Udp listener for receive the skeleton send by the streamer in udp
    /// </summary>
    public class UdpSkeletonListener : UdpListener<Skeleton>
    {
        /// <summary>
        /// The udp client
        /// </summary>
        private UdpClient _udp;

        public UdpSkeletonListener(BlockingCollection<Skeleton> dataTransferer, int port)
        {
            _udpThreadInfos = new UdpThreadInfos<Skeleton>(dataTransferer, port);
        }

        /// <summary>
        /// THe method who start the listening and add the receive data to the queue
        /// </summary>
        /// <param name="threadInfos">The informations to pass to the thread</param>
        override protected void StartListening(object threadInfos)
        {
            UdpThreadInfos<Skeleton> ti = (UdpThreadInfos<Skeleton>)threadInfos;
            Console.WriteLine("UdpSkeletonListener thread started");

            _udp = new UdpClient(ti.Port);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, ti.Port);

            while (true)
            {
                // Receiving frames from KinectStreamer
                byte[] data = null;
                try
                {
                    data = _udp.Receive(ref remoteEP);
                } catch (Exception ex)
                {

                }
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

        /// <summary>
        /// Method who stop the listening;
        /// </summary>
        override protected void StopListening()
        {
            Console.WriteLine("Stop listening on UdpSkeletonListener thread");
            _udp.Close();
        }
    }
}
