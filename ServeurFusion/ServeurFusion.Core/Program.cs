using ServeurFusion.EnvoiRTC;
using ServeurFusion.ReceptionUDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServeurFusion.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWebRtc();
            //TestUdp();
        }

        private static void TestWebRtc()
        {
            using (var _webRtcCommunication = new WebRtcCommunication())
            {
                _webRtcCommunication.Connect();
                Console.ReadKey(true);
            }
        }

        private static void TestUdp()
        {
            using(var udpListener = new UdpListener())
            {
                udpListener.Listen();
            }
        }
    }
}
