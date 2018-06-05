using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using System;
using System.Collections.Concurrent;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    public class TransformationCloudPointService : TransformationService<Cloud>
    {

        public TransformationCloudPointService(BlockingCollection<Cloud> udpToMiddle, BlockingCollection<Cloud> middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos<Cloud>(udpToMiddle, middleToWebRtc);
        }

        override protected void Launch(object threadInfos)
        {
            MiddleThreadInfos<Cloud> ti = (MiddleThreadInfos<Cloud>)threadInfos;
            Console.WriteLine("Thread middle démarré");

            while (true)
            {
                var data = ti._udpToMiddle.Take(); ;
                ti._middleToWebRtc.Add(data);
            }
        }
    }
}
