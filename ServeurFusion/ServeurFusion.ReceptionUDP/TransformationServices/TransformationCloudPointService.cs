using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    class TransformationCloudPointService
    {
        private MiddleThreadInfos _middleThreadInfos { get; set; }

        public TransformationCloudPointService(DataCloudPointTransferer udpToMiddle, DataCloudPointTransferer middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos(udpToMiddle, middleToWebRtc);
        }

        private void StartProsecute(object threadInfos)
        {
            MiddleThreadInfos ti = (MiddleThreadInfos)threadInfos;
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

        public void Prosecute()
        {
            Thread th = new Thread(new ParameterizedThreadStart(StartProsecute));

            th.Start(_middleThreadInfos);
        }
    }

    public class MiddleThreadInfos
    {
        public DataCloudPointTransferer _udpToMiddle { get; set; }
        public DataCloudPointTransferer _middleToWebRtc { get; set; }

        public MiddleThreadInfos(DataCloudPointTransferer udpToMiddle, DataCloudPointTransferer middleToWebRtc)
        {
            _udpToMiddle = udpToMiddle;
            _middleToWebRtc = middleToWebRtc;
        }
    }
}
}
