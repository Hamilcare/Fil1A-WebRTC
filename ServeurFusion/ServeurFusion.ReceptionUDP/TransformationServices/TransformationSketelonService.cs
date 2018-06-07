using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.TransformationServices;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ServeurFusion.ReceptionUDP
{
    /// <summary>
    /// Class who transfert the cloud point from a queue to another
    /// </summary>
    public class TransformationSkeletonService : TransformationService<Skeleton>
    {
        public TransformationSkeletonService(BlockingCollection<Skeleton> udpToMiddle, BlockingCollection<Skeleton> middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos<Skeleton>(udpToMiddle, middleToWebRtc);
        }

        /// <summary>
        /// Transport data from a queue to another
        /// </summary>
        /// <param name="threadInfos">the object who contains the data to transfert to one queue to another </param>
        override protected void Launch(object threadInfos)
        {
            MiddleThreadInfos<Skeleton> ti = (MiddleThreadInfos<Skeleton>)threadInfos;
            Console.WriteLine("TransformationSkeleton thread started");
            
            while (true)
            {
                var data = ti._udpToMiddle.Take();
                ti._middleToWebRtc.Add(data);
            }
        }
    }
}
