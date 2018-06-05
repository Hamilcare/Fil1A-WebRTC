using ServeurFusion.ReceptionUDP.Datas.Cloud;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ServeurFusion.ReceptionUDP.UdpListeners
{
    public class UdpCloudListener : UdpListener<Cloud>
    {
        private UdpClient _udp;

        public UdpCloudListener(BlockingCollection<Cloud> dataTransferer, int port)
        {
            _udpThreadInfos = new UdpThreadInfos<Cloud>(dataTransferer, port);
        }

        override protected void StartListening(object threadInfos)
        {
            UdpThreadInfos<Cloud> ti = (UdpThreadInfos<Cloud>)threadInfos;
            Console.WriteLine("UdpCloudListener thread started");

            _udp = new UdpClient(ti.Port);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, ti.Port);

            Cloud aggregateCloud = null;

            while (true)
            {
                var data = _udp.Receive(ref remoteEP);
                var cloud = new Cloud
                {
                    Timestamp = BitConverter.ToInt64(data, 0),
                    Points = new List<CloudPoint>()
                };

                int currentByte = 8;

                while (currentByte < data.Length)
                {
                    var point = new CloudPoint();
                    point.X = BitConverter.ToSingle(data, currentByte);
                    currentByte += 4;
                    point.Y = BitConverter.ToSingle(data, currentByte);
                    currentByte += 4;
                    point.Z = BitConverter.ToSingle(data, currentByte);
                    currentByte += 4;

                    point.R = data.ElementAt(currentByte);
                    currentByte++;
                    point.G = data.ElementAt(currentByte);
                    currentByte++;
                    point.B = data.ElementAt(currentByte);
                    currentByte++;

                    point.Tag = data.ElementAt(currentByte);
                    currentByte++;

                    // Filter points (0,0,0)
                    if(point.X != 0 && point.Y != 0 && point.Z != 0)
                        cloud.Points.Add(point);
                }

                // First frame
                if (aggregateCloud == null)
                    aggregateCloud = cloud;

                // If changing frame, send last complete received one
                else if (aggregateCloud.Timestamp != cloud.Timestamp)
                {
                    ti.DataTransferer.Add(aggregateCloud);
                    aggregateCloud = cloud;
                }

                // If same frame : aggregate
                else
                    aggregateCloud.Points.AddRange(cloud.Points);
            }
        }

        override protected void StopListening()
        {
            Console.WriteLine("Stop listening on UdpCloudListener thread");
            _udp.Close();
        }
    }
}
