using Org.WebRtc;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.UI.Core;

namespace Envoi
{
    public class Sender
    {
        
        public Sender()
        {
            Debug.WriteLine("Test envoi");

            List<RTCIceServer> iceList = new List<RTCIceServer>();
            iceList.Add(new RTCIceServer() { Url = "stun:stun.l.google.com:19302" });
            iceList.Add(new RTCIceServer() { Credential = "YzYNCouZM1mhqhmseWk6", Url = "turn:13.250.13.83:3478?transport=udp", Username = "YzYNCouZM1mhqhmseWk6" });
            iceList.Add(new RTCIceServer() { Url = "stun:stun1.l.google.com:19302" });
            iceList.Add(new RTCIceServer() { Url = "stun:stun2.l.google.com:19302" });
            iceList.Add(new RTCIceServer() { Url = "stun:stun3.l.google.com:19302" });
            iceList.Add(new RTCIceServer() { Url = "stun:stun4.l.google.com:19302" });

            RTCConfiguration rtcConfiguration = new RTCConfiguration()
            {
                BundlePolicy = RTCBundlePolicy.Balanced,
                IceTransportPolicy = RTCIceTransportPolicy.All,
                IceServers = iceList
            };

            WebRTC.Initialize(null);
            RTCPeerConnection peerConnection = new RTCPeerConnection(rtcConfiguration);

            RTCDataChannel rtcDataChannel = peerConnection.CreateDataChannel("channel test", new RTCDataChannelInit());

            rtcDataChannel.OnError += delegate { Debug.WriteLine("Channel error"); };
            rtcDataChannel.OnOpen += delegate { Debug.WriteLine("Channel open"); };
            rtcDataChannel.OnClose += delegate { Debug.WriteLine("Channel close"); };
            rtcDataChannel.OnMessage += delegate { Debug.WriteLine("Channel message"); };

            DataTest dataChannelMessage = new DataTest(RTCDataChannelMessageType.String)
            {
                test = "channel data test"
            };

            rtcDataChannel.Send(dataChannelMessage);
        }
        
    }
}
