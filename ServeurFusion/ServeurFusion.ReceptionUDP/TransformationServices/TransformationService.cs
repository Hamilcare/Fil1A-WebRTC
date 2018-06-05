using System.Collections.Concurrent;
using System.Threading;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    public abstract class TransformationService<T>
    {
        private Thread _transformationServiceThread;
        protected MiddleThreadInfos<T> _middleThreadInfos{ get; set; }


        public void Start()
        {
            _transformationServiceThread = new Thread(new ParameterizedThreadStart(Launch));
            _transformationServiceThread.Start(_middleThreadInfos);
        }

        public void Stop()
        {
            _transformationServiceThread.Abort();
        }

        protected abstract void Launch(object obj);
    }

    public class MiddleThreadInfos<T>
    {
        public BlockingCollection<T> _udpToMiddle { get; set; }
        public BlockingCollection<T> _middleToWebRtc { get; set; }

        public MiddleThreadInfos(BlockingCollection<T> udpToMiddle, BlockingCollection<T> middleToWebRtc)
        {
            _udpToMiddle = udpToMiddle;
            _middleToWebRtc = middleToWebRtc;
        }
    }
}
