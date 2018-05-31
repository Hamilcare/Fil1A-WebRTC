using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP
{
    public class WebRtcSender
    {
        public DataTransferer _middleToWebRtc { get; set; }

        public WebRtcSender(DataTransferer middleToWebRtc)
        {
            _middleToWebRtc = middleToWebRtc;
        }

        private void StartWebRTC(object middleToWebRtc)
        {
            DataTransferer dt = (DataTransferer)middleToWebRtc;
            Console.WriteLine("Thread webRTC démarrée");

            while (true)
            {
                if (dt.IsEmpty())
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Array a = dt.ConsumeData();
                    Console.WriteLine("Lecture de : " +  a.Length + "      " + a.ToString());

                }

            }
        }

        public void WebRTC()
        {
            Thread th = new Thread(new ParameterizedThreadStart(StartWebRTC));

            th.Start(_middleToWebRtc);
        }
    }
}
