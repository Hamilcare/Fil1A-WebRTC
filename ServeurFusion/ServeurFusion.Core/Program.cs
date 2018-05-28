using ServeurFusion.EnvoiRTC;
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
            using (var _webRtcCommunication = new WebRtcCommunication())
            {
                _webRtcCommunication.Connect();
                Console.ReadKey(true);
            }
        }
    }
}
