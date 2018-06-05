using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.TransformationServices;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ServeurFusion.ReceptionUDP
{

    public class TransformationSkeletonService : TransformationService<Skeleton>
    {
        public TransformationSkeletonService(BlockingCollection<Skeleton> udpToMiddle, BlockingCollection<Skeleton> middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos<Skeleton>(udpToMiddle, middleToWebRtc);
        }

        override protected void Launch(object threadInfos)
        {
            MiddleThreadInfos<Skeleton> ti = (MiddleThreadInfos<Skeleton>)threadInfos;
            Console.WriteLine("Thread middle démarré");
            
            while (true)
            {
                var data = ti._udpToMiddle.Take();
                ti._middleToWebRtc.Add(data);
            }
        }
    }
}
