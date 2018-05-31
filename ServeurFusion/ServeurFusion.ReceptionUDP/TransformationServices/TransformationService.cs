using ServeurFusion.ReceptionUDP.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    public abstract class TransformationService<T>
    {
        protected MiddleThreadInfos<T> _middleThreadInfos{ get; set; }


    public void Prosecute()
        {
            Thread th = new Thread(new ParameterizedThreadStart(StartProsecute));

            th.Start(_middleThreadInfos);
        }

        protected abstract void StartProsecute(object obj);
    }

    public class MiddleThreadInfos<T>
    {
        public DataTransferer<T> _udpToMiddle { get; set; }
        public DataTransferer<T> _middleToWebRtc { get; set; }

        public MiddleThreadInfos(DataTransferer<T> udpToMiddle, DataTransferer<T> middleToWebRtc)
        {
            _udpToMiddle = udpToMiddle;
            _middleToWebRtc = middleToWebRtc;
        }
    }
    
}
