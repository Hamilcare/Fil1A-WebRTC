using ServeurFusion.ReceptionUDP;
using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using Spitfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.EnvoiRTC
{
    public class CloudThreadInfos
    {
        public DataTransferer<Cloud> CloudToWebRTC { get; set; }
        public SpitfireRtc RTCPeerConnection { get; set; }

        public CloudThreadInfos(DataTransferer<Cloud> cloudToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            CloudToWebRTC = cloudToWebRTC;
            RTCPeerConnection = rtcPeerConnection;
        }
    }
    class CloudThreadWebRTC
    {
        private CloudThreadInfos CloudThreadInfos { get; set; }

        public CloudThreadWebRTC(DataTransferer<Cloud> cloudToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            CloudThreadInfos = new CloudThreadInfos(cloudToWebRTC, rtcPeerConnection);
        }

        private void StartCloudThread(object threadInfos)
        {
            CloudThreadInfos cloudThreadInfos = (CloudThreadInfos)threadInfos;
            Console.WriteLine("Thread Cloud sender started");

            while (true)
            {
                if (cloudThreadInfos.CloudToWebRTC.IsEmpty())
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Cloud cloud = cloudThreadInfos.CloudToWebRTC.ConsumeData();

                    string formattedCloudMessage = "";
                    cloud.Points.ForEach(s => formattedCloudMessage += $"{s.X};{s.Y};{s.Z};{s.R};{s.G};{s.B};".Replace(',', '.'));
                    formattedCloudMessage = formattedCloudMessage.Remove(formattedCloudMessage.Length - 1, 1);
                    Console.WriteLine(formattedCloudMessage + "\n");

                    cloudThreadInfos.RTCPeerConnection.DataChannelSendText("cloudChannel", formattedCloudMessage);
                }

            }
        }

        public void Prosecute()
        {
            Thread cloudThread = new Thread(new ParameterizedThreadStart(StartCloudThread));
            cloudThread.Start(CloudThreadInfos);
        }
    }
}
