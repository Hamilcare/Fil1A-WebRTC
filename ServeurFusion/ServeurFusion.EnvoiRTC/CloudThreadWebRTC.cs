using ServeurFusion.ReceptionUDP.Datas.Cloud;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using Spitfire;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServeurFusion.EnvoiRTC
{
    public class CloudThreadInfos
    {
        public BlockingCollection<Cloud> CloudToWebRTC { get; set; }
        public SpitfireRtc RTCPeerConnection { get; set; }

        public CloudThreadInfos(BlockingCollection<Cloud> cloudToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            CloudToWebRTC = cloudToWebRTC;
            RTCPeerConnection = rtcPeerConnection;
        }
    }
    class CloudThreadWebRTC
    {
        private Thread _cloudThread;
        private CloudThreadInfos _cloudThreadInfos;

        public CloudThreadWebRTC(BlockingCollection<Cloud> cloudToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            _cloudThreadInfos = new CloudThreadInfos(cloudToWebRTC, rtcPeerConnection);
            _cloudThread = new Thread(new ParameterizedThreadStart(StartCloudThread));
        }

        private void StartCloudThread(object threadInfos)
        {
            CloudThreadInfos cloudThreadInfos = (CloudThreadInfos)threadInfos;
            Console.WriteLine("Thread Cloud sender started");

            while (true)
            {
                Cloud cloud = cloudThreadInfos.CloudToWebRTC.Take();
                /*
                //On envoi les points par paquets de nbPointsParPaquet
                int nbPointsParPaquet = 50;
                int cpt1 = 0;
                while(cpt1 + nbPointsParPaquet <= cloud.Points.Count)
                {
                    var pointsToSend = cloud.Points.GetRange(cpt1, nbPointsParPaquet);
                    string formattedMsg = FormateMessage(cloud.Timestamp, pointsToSend);

                    cloudThreadInfos.RTCPeerConnection.DataChannelSendText("cloudChannel", formattedMsg);
                    cpt1 += nbPointsParPaquet;
                }
                //On envoi le reste (s'il y en a)
                if(cpt1 < cloud.Points.Count)
                {
                    var pointsToSend = cloud.Points.GetRange(cpt1, cloud.Points.Count - cpt1);
                    string formattedMsg = FormateMessage(cloud.Timestamp, pointsToSend);

                    cloudThreadInfos.RTCPeerConnection.DataChannelSendText("cloudChannel", formattedMsg);
                }
                */
                //Ancienne méthode pour envoyer tout en une trame (pb : trame découpée par SpitFire)
                string formattedCloudMessage = String.Empty;
                for(int i = 0; i < cloud.Points.Count; i++)
                {
                    if (i % 35 != 0)
                        continue;
                    var s = cloud.Points.ElementAt(i);
                    formattedCloudMessage += $"{s.X};{s.Y};{s.Z};{s.R};{s.G};{s.B};".Replace(',', '.');
                }
                formattedCloudMessage = formattedCloudMessage.Remove(formattedCloudMessage.Length - 1, 1);
                cloudThreadInfos.RTCPeerConnection.DataChannelSendText("cloudChannel", formattedCloudMessage);
            }
        }

        private string FormateMessage(long timestamp, IList<CloudPoint> points)
        {
            StringBuilder formattedMsg = new StringBuilder();
            formattedMsg.Append(timestamp);
            foreach(var point in points)
            {
                formattedMsg.Append($";{point.X};{point.Y};{point.Z};{point.R};{point.G};{point.B}".Replace(',', '.'));
            }

            return formattedMsg.ToString();
        }

        public void Start()
        {
            _cloudThread.Start(_cloudThreadInfos);
        }

        public void Stop()
        {
            _cloudThread.Abort();
            Console.WriteLine("Thread Cloud sender stopped"); 
        }
    }
}
