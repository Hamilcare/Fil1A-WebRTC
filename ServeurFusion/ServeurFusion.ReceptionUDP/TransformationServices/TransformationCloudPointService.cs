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
            _middleThreadInfos = new MiddleThreadInfos<Cloud>(udpToMiddle, middleToWebRtc);
        }

        override protected void Launch(object threadInfos)
        {
            MiddleThreadInfos<Cloud> ti = (MiddleThreadInfos<Cloud>)threadInfos;
            Console.WriteLine("Thread middle démarré");

            while (true)
            {
                if (!ti._udpToMiddle.IsEmpty())
                {
                    ti._middleToWebRtc.AddData(ti._udpToMiddle.ConsumeData());
                }

            }
        }
    }
}
