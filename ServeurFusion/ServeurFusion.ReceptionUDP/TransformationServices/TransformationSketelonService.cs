using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.TransformationServices;
using System;
using System.Threading;

namespace ServeurFusion.ReceptionUDP
{

    public class TransformationSkeletonService : TransformationService<Skeleton>
    {
        public TransformationSkeletonService(DataTransferer<Skeleton> udpToMiddle, DataTransferer<Skeleton> middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos<Skeleton>(udpToMiddle, middleToWebRtc);
        }

        override protected void Launch(object threadInfos)
        {
            MiddleThreadInfos<Skeleton> ti = (MiddleThreadInfos<Skeleton>)threadInfos;
            Console.WriteLine("Thread middle démarré");
            
            while (true)
            {
                if (!ti._udpToMiddle.IsEmpty())
                {
                    var data = ti._udpToMiddle.ConsumeData();
                    ti._middleToWebRtc.AddData(data);
                }
            }
        }
    }
}
