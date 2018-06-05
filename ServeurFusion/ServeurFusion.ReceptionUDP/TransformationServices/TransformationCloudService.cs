using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using System;
using System.Collections.Concurrent;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    public class TransformationCloudService : TransformationService<Cloud>
    {

        public TransformationCloudService(BlockingCollection<Cloud> udpToMiddle, BlockingCollection<Cloud> middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos<Cloud>(udpToMiddle, middleToWebRtc);
        }

        override protected void Launch(object threadInfos)
        {
            MiddleThreadInfos<Cloud> ti = (MiddleThreadInfos<Cloud>)threadInfos;
            Console.WriteLine("TransformationCloud thread started");

            while (true)
            {
                var data = ti._udpToMiddle.Take(); ;
                ti._middleToWebRtc.Add(data);
            }
        }
    }
}
