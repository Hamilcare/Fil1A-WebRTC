using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.WebRtc;

namespace Envoi
{
    class DataTest : IDataChannelMessage
    {
        public DataTest(RTCDataChannelMessageType type)
        {
            this.DataType = type;
        }

        public RTCDataChannelMessageType DataType { get; set; }

        public String test { get; set; }
    }
}
