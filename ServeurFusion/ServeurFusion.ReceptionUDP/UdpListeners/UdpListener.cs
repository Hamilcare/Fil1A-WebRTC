using System.Collections.Concurrent;
using System.Threading;

namespace ServeurFusion.ReceptionUDP.UdpListeners
{
    public abstract class UdpListener<T>
    {
        private Thread _udpListenerThread;
        protected UdpThreadInfos<T> _udpThreadInfos;

        public void Listen()
        {
            _udpListenerThread = new Thread(new ParameterizedThreadStart(StartListening));
            _udpListenerThread.Start(_udpThreadInfos);
        }

        public void Stop()
        {
            StopListening();
            _udpListenerThread.Abort();
        }

        protected abstract void StartListening(object obj);

        protected abstract void StopListening();
    }


    public class UdpThreadInfos<T>
    {
        public BlockingCollection<T> DataTransferer { get; set; }
        public int Port { get; set; }

        public UdpThreadInfos(BlockingCollection<T> dataTransferer, int port)
        {
            DataTransferer = dataTransferer;
            Port = port;
        }
    }
}
