using ServeurFusion.EnvoiRTC;
using ServeurFusion.ReceptionUDP;
using ServeurFusion.ReceptionUDP.Datas;
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
            //TestWebRtc();
            TestUdp();
        }
        private static void TestUdp()
        {
            DataTransferer<Skeleton> udpToMiddle = new DataTransferer<Skeleton>();
            DataTransferer<Skeleton> middleToWebRtc = new DataTransferer<Skeleton>();

            var udpListener = new UdpSkeletonListener(udpToMiddle, 9877);
            var transformationService = new TransformationSkeletonService(udpToMiddle, middleToWebRtc);
            WebRtcCommunication webRtcSender = new WebRtcCommunication()
            {
                SkeletonToWebRtc = middleToWebRtc
            };

            udpListener.Listen();
            transformationService.Prosecute();
            webRtcSender.StartWebRtcCommunication();
        }
    }
}
