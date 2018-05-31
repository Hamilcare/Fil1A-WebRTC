using ServeurFusion.ReceptionUDP;
using ServeurFusion.ReceptionUDP.Datas;
using Spitfire;
using System;
using System.Threading;

namespace ServeurFusion.EnvoiRTC
{
    public class SkeletonThreadInfos
    {
        public DataTransferer<Skeleton> SkeletonToWebRTC { get; set; }
        public SpitfireRtc RTCPeerConnection { get; set; }

        public SkeletonThreadInfos(DataTransferer<Skeleton> skeletonToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            SkeletonToWebRTC = skeletonToWebRTC;
            RTCPeerConnection = rtcPeerConnection;
        }
    }

    public class SkeletonThreadWebRTC
    {
        private SkeletonThreadInfos SkeletonThreadInfos { get; set; }

        public SkeletonThreadWebRTC(DataTransferer<Skeleton> skeletonToWebRTC, SpitfireRtc rtcPeerConnection)
        {
            SkeletonThreadInfos = new SkeletonThreadInfos(skeletonToWebRTC, rtcPeerConnection);
        }

        private void StartSkeletonThread(object threadInfos)
        {
            SkeletonThreadInfos skeletonThreadInfos = (SkeletonThreadInfos)threadInfos;
            Console.WriteLine("Thread Skeleton sender started");

            while (true)
            {
                if (skeletonThreadInfos.SkeletonToWebRTC.IsEmpty())
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Skeleton skeleton = skeletonThreadInfos.SkeletonToWebRTC.ConsumeData();

                    string formattedSkeletonMessage = "";
                    skeleton.SkeletonPoints.ForEach(s => formattedSkeletonMessage += $"{s.X};{s.Y};{s.Z};{s.R};{s.G};{s.B};".Replace(',', '.'));
                    formattedSkeletonMessage = formattedSkeletonMessage.Remove(formattedSkeletonMessage.Length - 1, 1);
                    skeletonThreadInfos.RTCPeerConnection.DataChannelSendText("skeletonChannel", formattedSkeletonMessage);
                }
            }
        }

        public void Prosecute()
        {
            Thread skeletonThread = new Thread(new ParameterizedThreadStart(StartSkeletonThread));
            skeletonThread.Start(SkeletonThreadInfos);
        }
    } 
}
