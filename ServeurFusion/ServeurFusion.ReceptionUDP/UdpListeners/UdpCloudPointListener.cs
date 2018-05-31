using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.Datas.Cloud;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
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
    public class UdpCloudPointListener : UdpListener<Cloud>
    {
        public UdpCloudPointListener(DataTransferer<Cloud> dataTransferer, int port)
        {
            this._udpThreadInfos = new UdpThreadInfos<Cloud>(dataTransferer, port);
        }

        override protected void StartListening(object threadInfos)
        {
            UdpThreadInfos<Cloud> ti = (UdpThreadInfos<Cloud>)threadInfos;
            Console.WriteLine("Thread udp démarrée");

            UdpClient udp = new UdpClient(ti._port);
            var remoteEP = new IPEndPoint(IPAddress.Any, 9876);

            Cloud aggregateCloud = null;

            while (true)
            {
                var data = udp.Receive(ref remoteEP);
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

                    cloud.Points.Add(point);
                }
                
                //Premiere frame
                if (aggregateCloud == null)
                    aggregateCloud = cloud;
                //Si on change de frame : on envoi la derniere reçue completement
                else if (aggregateCloud.Timestamp != cloud.Timestamp) {
                    ti._dataTransferer.AddData(aggregateCloud);
                    aggregateCloud = cloud;
                }
                //Sinon c'est qu'on est tjs sur la même frame : on aggrege
                else
                    aggregateCloud.Points.AddRange(cloud.Points);

            }
        }

    }

}
