using System.Collections.Concurrent;
using System.Threading;

namespace ServeurFusion.ReceptionUDP.UdpListeners
{
    /// <summary>
    /// Generic class which allow to build udp listener who are going to listen with a thread.
    /// </summary>
    /// <typeparam name="T">The object which will be created by the listener</typeparam>
    public abstract class UdpListener<T>
    {
        /// <summary>
        /// A thread use for listen un udp.
        /// </summary>
        private Thread _udpListenerThread;

        /// <summary>
        /// Informations used in the method start by the thread. 
        /// The thread object allow only one parameters, so we need to put all the infomations in this object.
        /// </summary>
        protected UdpThreadInfos<T> _udpThreadInfos;

        /// <summary>
        ///  Method who are going  to listen in udp.
        /// </summary>
        public void Listen()
        {
            _udpListenerThread = new Thread(new ParameterizedThreadStart(StartListening));
            _udpListenerThread.Start(_udpThreadInfos);
        }

        /// <summary>
        /// Method used to stop the thread.
        /// </summary>
        public void Stop()
        {
            StopListening();
            _udpListenerThread.Abort();
        }

        /// <summary>
        /// Method to redefined in childrens of the class, its the logic of the thread.
        /// </summary>
        /// <param name="obj">The UdpThreadInfo, the method need to have an object in paramaters</param>
        protected abstract void StartListening(object obj);

        /// <summary>
        /// Abstract method for stop the listening
        /// </summary>
        protected abstract void StopListening();
    }

    /// <summary>
    /// The class use to transfert data to the thread
    /// </summary>
    /// <typeparam name="T">the generic type used in the listener</typeparam>
    public class UdpThreadInfos<T>
    {
        /// <summary>
        /// The queue in which we add the datas.
        /// </summary>
        public BlockingCollection<T> DataTransferer { get; set; }
        /// <summary>
        /// The port where to listen
        /// </summary>
        public int Port { get; set; }

        public UdpThreadInfos(BlockingCollection<T> dataTransferer, int port)
        {
            DataTransferer = dataTransferer;
            Port = port;
        }
    }
}
