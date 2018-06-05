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
        private Thread _cloudThread;
        private CloudThreadInfos _cloudThreadInfos;

        public CloudThreadWebRTC(DataTransferer<Cloud> cloudToWebRTC, SpitfireRtc rtcPeerConnection)
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
                Cloud cloud = cloudThreadInfos.CloudToWebRTC.ConsumeData();

                string formattedCloudMessage = String.Empty;
                for(int i = 0; i < cloud.Points.Count; i++)
                {
                    if (i % 50 != 0)
                        continue;
                    var s = cloud.Points.ElementAt(i);
                    formattedCloudMessage += $"{s.X};{s.Y};{s.Z};{s.R};{s.G};{s.B};".Replace(',', '.');
                }
                formattedCloudMessage = formattedCloudMessage.Remove(formattedCloudMessage.Length - 1, 1);
                cloudThreadInfos.RTCPeerConnection.DataChannelSendText("cloudChannel", formattedCloudMessage);
            }
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
