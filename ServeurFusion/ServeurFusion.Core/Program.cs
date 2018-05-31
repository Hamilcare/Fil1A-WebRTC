using ServeurFusion.EnvoiRTC;
using ServeurFusion.ReceptionUDP;
using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using ServeurFusion.ReceptionUDP.UdpListeners;
using ServeurFusion.ReceptionUDP.TransformationServices;
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
            DataTransferer<Skeleton> skeletonUdpToMiddle = new DataTransferer<Skeleton>();
            DataTransferer<Skeleton> skeletonMiddleToWebRtc = new DataTransferer<Skeleton>();

            DataTransferer<Cloud> cloudUdpToMiddle = new DataTransferer<Cloud>();
            DataTransferer<Cloud> cloudMiddleToWebRtc = new DataTransferer<Cloud>();

            var skeletonUdpListener = new UdpSkeletonListener(skeletonUdpToMiddle, 9877);
            var cloudUdpListener = new UdpCloudPointListener(cloudUdpToMiddle, 9876);

            var skeletonTransformationService = new TransformationSkeletonService(skeletonUdpToMiddle, skeletonMiddleToWebRtc);
            var cloudTransformationService = new TransformationCloudPointService(cloudUdpToMiddle, cloudMiddleToWebRtc);

            WebRtcCommunication webRtcSender = new WebRtcCommunication(skeletonMiddleToWebRtc, cloudMiddleToWebRtc);

            skeletonUdpListener.Listen();
            skeletonTransformationService.Prosecute();

            cloudUdpListener.Listen();
            cloudTransformationService.Prosecute();

            webRtcSender.Connect();
            Console.WriteLine("coucou");
            Console.ReadLine();
        }
    }
}
