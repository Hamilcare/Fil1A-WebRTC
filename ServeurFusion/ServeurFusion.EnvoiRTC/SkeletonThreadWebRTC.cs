using ServeurFusion.ReceptionUDP.Datas;
using Spitfire;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ServeurFusion.EnvoiRTC
{
    public class SkeletonThreadInfos
    {
        public BlockingCollection<Skeleton> SkeletonToWebRTC { get; set; }
        public SpitfireRtc RTCPeerConnection { get; set; }

        public SkeletonThreadInfos(BlockingCollection<Skeleton> skeletonToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            SkeletonToWebRTC = skeletonToWebRTC;
            RTCPeerConnection = rtcPeerConnection;
        }
    }

    public class SkeletonThreadWebRTC
    {
        private Thread _skeletonThread;
        private SkeletonThreadInfos _skeletonThreadInfos;

        public SkeletonThreadWebRTC(BlockingCollection<Skeleton> skeletonToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            _skeletonThreadInfos = new SkeletonThreadInfos(skeletonToWebRTC, rtcPeerConnection);
            _skeletonThread = new Thread(new ParameterizedThreadStart(StartSkeletonThread));
        }

        private void StartSkeletonThread(object threadInfos)
        {
            SkeletonThreadInfos skeletonThreadInfos = (SkeletonThreadInfos)threadInfos;
            Console.WriteLine("Thread Skeleton sender started");

            while (true)
            {
                Skeleton skeleton = skeletonThreadInfos.SkeletonToWebRTC.Take();

                string formattedSkeletonMessage = "";
                skeleton.SkeletonPoints.ForEach(s => formattedSkeletonMessage += $"{s.X};{s.Y};{s.Z};{s.R};{s.G};{s.B};".Replace(',', '.'));
                formattedSkeletonMessage = formattedSkeletonMessage.Remove(formattedSkeletonMessage.Length - 1, 1);
                skeletonThreadInfos.RTCPeerConnection.DataChannelSendText("skeletonChannel", formattedSkeletonMessage);
            }
        }

        public void Start()
        {
            _skeletonThread.Start(_skeletonThreadInfos);
        }

        public void Stop()
        {
            _skeletonThread.Abort();
            Console.WriteLine("Thread Skeleton sender stopped");
        }
    } 
}
