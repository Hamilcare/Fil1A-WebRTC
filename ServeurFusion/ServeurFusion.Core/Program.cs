using ServeurFusion.EnvoiRTC;
using ServeurFusion.ReceptionUDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServeurFusion.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWebRTC();
        }

        private static void TestWebRTC()
        {
            DataTransferer udpToMiddle = new DataTransferer();
            DataTransferer middleToWebRtc = new DataTransferer();

            var udpListener = new UdpListener(udpToMiddle, 9877);
            var transformationService = new TransformationService(udpToMiddle, middleToWebRtc);
            var webRtcSender = new WebRtcCommunication()
            {
                SkeletonToWebRtc = middleToWebRtc
            };

            udpListener.Listen();
            transformationService.Prosecute();
            webRtcSender.StartWebRtcCommunication();
        }
    }
}
