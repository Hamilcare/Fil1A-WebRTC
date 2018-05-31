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
            this._middleThreadInfos = new MiddleThreadInfos<Skeleton>(udpToMiddle, middleToWebRtc);
        }

        override protected void StartProsecute(object threadInfos)
        {
            MiddleThreadInfos<Skeleton> ti = (MiddleThreadInfos<Skeleton>)threadInfos;
            Console.WriteLine("Thread middle démarrée");
            
            while (true)
            {
                if (ti._udpToMiddle.IsEmpty())
                {
                    //Thread.Sleep(1000);
                }
                else
                {
                    ti._middleToWebRtc.AddData(ti._udpToMiddle.ConsumeData());
                    //Console.WriteLine("Transfert");
                }
            }
        }
    }
    
}
