using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    public class TransformationCloudPointService : TransformationService<Cloud>
    {

        public TransformationCloudPointService(DataTransferer<Cloud> udpToMiddle, DataTransferer<Cloud> middleToWebRtc)
        {
            this._middleThreadInfos = new MiddleThreadInfos<Cloud>(udpToMiddle, middleToWebRtc);
        }

        override protected void StartProsecute(object threadInfos)
        {
            MiddleThreadInfos<Cloud> ti = (MiddleThreadInfos<Cloud>)threadInfos;
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
