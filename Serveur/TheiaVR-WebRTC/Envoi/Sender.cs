using Org.WebRtc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.WebSockets;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using System;
using System.Text;
using Windows.Storage.Streams;

namespace Envoi
{
    public class Sender
    {
        private MessageWebSocket signalingChannel;
        private IList<RTCIceServer> iceList;
        private RTCConfiguration rtcConfiguration;
        private RTCPeerConnection peerConnection;
        private RTCDataChannel rtcDataChannel;

        public Sender()
        {
            Debug.WriteLine("Sender Test");

            WebRTC.Initialize(CoreApplication.MainView.CoreWindow.Dispatcher);

            signalingChannel = new MessageWebSocket();
            signalingChannel.ConnectAsync(new Uri("ws://barnab2.tk:9090")).AsTask().Wait();

            // Test send over websocket
            /*string coucou = "{\"type\":\"login\", \"name\":\"cedric2\"}";
            Windows.Storage.Streams.IBuffer buffer;
            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteString(coucou);
            buffer = dataWriter.DetachBuffer();
            signalingChannel.OutputStream.WriteAsync(buffer);*/

            iceList = new List<RTCIceServer>
            {
                new RTCIceServer() { Url = "stun:stun.l.google.com:19302" },
                new RTCIceServer() { Credential = "YzYNCouZM1mhqhmseWk6", Url = "turn:13.250.13.83:3478?transport=udp", Username = "YzYNCouZM1mhqhmseWk6" },
                new RTCIceServer() { Url = "stun:stun1.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun2.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun3.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun4.l.google.com:19302" }
            };

            startRTC(true);

            rtcDataChannel.OnError += delegate { Debug.WriteLine("Channel error"); };
            rtcDataChannel.OnOpen += delegate { Debug.WriteLine("Channel open"); };
            rtcDataChannel.OnClose += delegate { Debug.WriteLine("Channel close"); };

            signalingChannel.MessageReceived += delegate (MessageWebSocket eventSource, MessageWebSocketMessageReceivedEventArgs eventData)
            {
                OnMessageHandler(eventData);
            };

            
        }

        private void startRTC(bool isInitiator)
        {
            rtcConfiguration = new RTCConfiguration()
            {
                BundlePolicy = RTCBundlePolicy.Balanced,
                IceTransportPolicy = RTCIceTransportPolicy.All,
                IceServers = iceList
            };
            peerConnection = new RTCPeerConnection(rtcConfiguration);

            peerConnection.OnIceCandidate += delegate (RTCPeerConnectionIceEvent candidate)
            {
                //signalingChannel.OutputStream.WriteAsync();
                // TO DO buffer de JSON.stringify({ candidate: evt.candidate })
            };

            peerConnection.OnNegotiationNeeded += delegate
            {
                peerConnection.CreateOffer();
                // setLocalDescription(offer); ????
            };

            if (isInitiator)
            {
                rtcDataChannel = peerConnection.CreateDataChannel("RTCDataChannel Test", new RTCDataChannelInit());
                // TO DO à l'ouverture du channel
            }
            else
            {
                peerConnection.OnDataChannel += delegate(RTCDataChannelEvent dataChannel)
                {
                    rtcDataChannel = dataChannel.Channel;
                    // TO DO à l'ouverture du channel
                };
            }

            Debug.WriteLine(rtcDataChannel.ReadyState);
        }

        private void OnMessageHandler(MessageWebSocketMessageReceivedEventArgs messageEvent)
        {
            if (peerConnection == null)
            {
                startRTC(false);
            }
            
            // TO DO handle offer
        }

        private void SetupSender()
        {

        }

        private void SendKinectData()
        {

        }
    }
}
