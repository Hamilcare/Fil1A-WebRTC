using ServeurFusion.ReceptionUDP.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.UdpListeners
{
    public abstract class UdpListener<T>
    {
        protected UdpThreadInfos<T> _udpThreadInfos;

        public void Listen()
        {
            Thread th = new Thread(new ParameterizedThreadStart(StartListening));

            th.Start(_udpThreadInfos);
        }

        protected abstract void StartListening(object obj);
    }


    public class UdpThreadInfos<T>
    {
        public DataTransferer<T> _dataTransferer { get; set; }
        public int _port { get; set; }

        public UdpThreadInfos(DataTransferer<T> dataTransferer, int port)
        {
            _dataTransferer = dataTransferer;
            _port = port;
        }
    }
}
