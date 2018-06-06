using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ServeurFusion.ReceptionUDP.TransformationServices
{
    /// <summary>
    /// Generic class which actually do nothing, it's not used, it will be if we have to add some logic in the middle.
    /// </summary>
    /// <typeparam name="T">The generic type to pass from a queue to another</typeparam>
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
            Console.WriteLine("TransformationSkeleton thread stopped");
            _transformationServiceThread.Abort();
        }

        protected abstract void Launch(object obj);
    }

    /// <summary>
    /// Information to pass to the thread
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
