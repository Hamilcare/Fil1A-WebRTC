using ServeurFusion.ReceptionUDP.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    class TransformationCloudPointService : TransformationService<Array>
    {

        public TransformationCloudPointService(DataTransferer<Array> udpToMiddle, DataTransferer<Array> middleToWebRtc)
        {
            this._middleThreadInfos = new MiddleThreadInfos<Array>(udpToMiddle, middleToWebRtc);
        }

        override protected void StartProsecute(object threadInfos)
        {
            MiddleThreadInfos<Array> ti = (MiddleThreadInfos<Array>)threadInfos;
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
