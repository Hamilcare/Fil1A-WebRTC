using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.TransformationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP
{

    public class TransformationSkeletonService
    {
        private MiddleThreadInfos<Skeleton> _middleThreadInfos { get; set; }

        public TransformationSkeletonService(DataTransferer<Skeleton> udpToMiddle, DataTransferer<Skeleton> middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos<Skeleton>(udpToMiddle, middleToWebRtc);
        }

        private void StartProsecute(object threadInfos)
        {
            MiddleThreadInfos<Skeleton> ti = (MiddleThreadInfos<Skeleton>)threadInfos;
            Console.WriteLine("Thread middle démarrée");
            
            while (true)
            {
                if (ti._udpToMiddle.IsEmpty())
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    ti._middleToWebRtc.AddData(ti._udpToMiddle.ConsumeData());
                    Console.WriteLine("Transfert");

                }

            }
        }
    }
    
}
