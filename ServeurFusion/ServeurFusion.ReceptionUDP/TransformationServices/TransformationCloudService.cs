using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using System;
using System.Collections.Concurrent;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    /// <summary>
    /// Class who transfert the cloud point from a queue to another
    /// </summary>
    public class TransformationCloudService : TransformationService<Cloud>
    {

        public TransformationCloudService(BlockingCollection<Cloud> udpToMiddle, BlockingCollection<Cloud> middleToWebRtc)
        {
            _middleThreadInfos = new MiddleThreadInfos<Cloud>(udpToMiddle, middleToWebRtc);
        }

        /// <summary>
        /// Transport data from a queue to another
        /// </summary>
        /// <param name="threadInfos">the object who contains the data to transfert to one queue to another </param>
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
