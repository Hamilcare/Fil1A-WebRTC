using Org.WebRtc;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Data.Json;
using System;

namespace Envoi
{
    public class Sender
    {
        private MessageWebSocket signalingChannel;
        private DataWriter dataWriter;
        private IList<RTCIceServer> iceList;
        private RTCConfiguration rtcConfiguration;
        private RTCPeerConnection peerConnection;
        private RTCDataChannel rtcDataChannel;

        public Sender()
        {
            Debug.WriteLine("Sender Test");

            WebRTC.Initialize(null);

            signalingChannel = new MessageWebSocket();
            signalingChannel.ConnectAsync(new Uri("ws://barnab2.tk:9090")).AsTask().Wait();
            //signalingChannel.MessageReceived += MessageReceived;

            dataWriter = new DataWriter();

            // Test send over websocket
            //string coucou = "{\"type\":\"login\", \"name\":\"cedric2\"}";

            iceList = new List<RTCIceServer>
            {
                new RTCIceServer() { Url = "stun:stun.l.google.com:19302" },
                new RTCIceServer() { Credential = "YzYNCouZM1mhqhmseWk6", Url = "turn:13.250.13.83:3478?transport=udp", Username = "YzYNCouZM1mhqhmseWk6" },
                new RTCIceServer() { Url = "stun:stun1.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun2.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun3.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun4.l.google.com:19302" }
            };

            //rtcDataChannel.OnError += delegate { Debug.WriteLine("Channel error"); };
            //rtcDataChannel.OnOpen += delegate { Debug.WriteLine("Channel open"); };
            //rtcDataChannel.OnClose += delegate { Debug.WriteLine("Channel close"); };

            StartRTC(true);
        }

        private void StartRTC(bool isInitiator)
        {
            rtcConfiguration = new RTCConfiguration()
            {
                BundlePolicy = RTCBundlePolicy.Balanced,
                IceTransportPolicy = RTCIceTransportPolicy.All,
                IceServers = iceList
            };
            peerConnection = new RTCPeerConnection(rtcConfiguration);

            peerConnection.OnIceCandidate += (RTCPeerConnectionIceEvent candidate) =>
            {
                try
                {
                    dataWriter.WriteString("{\"candidate\":" + candidate.Candidate);
                    signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());

                    Debug.WriteLine("Candidate success");
                } catch (Exception e)
                {
                    Debug.WriteLine("Candidate fail : " + e.Message);
                }
                
            };

            peerConnection.OnNegotiationNeeded += delegate
            {
                try
                {
                    peerConnection.CreateOffer();
                    peerConnection.SetLocalDescription(peerConnection.RemoteDescription);

                    dataWriter.WriteString("{\"desc\":" + peerConnection.LocalDescription);
                    signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());

                    Debug.WriteLine("Negociation success");
                } catch (Exception e)
                {
                    Debug.WriteLine("Negociation fail : " + e.Message);
                }
                
            };

            if (isInitiator)
            {
                rtcDataChannel = peerConnection.CreateDataChannel("RTCDataChannel Test", new RTCDataChannelInit());
                // TO DO à l'ouverture du channel
            }
            else
            {
                peerConnection.OnDataChannel += (RTCDataChannelEvent dataChannel) =>
                {
                    rtcDataChannel = dataChannel.Channel;
                    // TO DO à l'ouverture du channel
                };
            }
        }

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    if (peerConnection == null)
                    {
                        StartRTC(false);
                    }

                    var message = JsonObject.Parse(args.GetDataReader().ReadString(reader.UnconsumedBufferLength));
                    if (message.GetNamedValue("desc") != null)
                    {
                        var desc = message.GetNamedValue("desc");

                        if (desc.GetString() == "offer")
                        {
                            peerConnection.SetRemoteDescription(peerConnection.RemoteDescription);
                            peerConnection.CreateAnswer();
                            peerConnection.SetLocalDescription(peerConnection.RemoteDescription);

                            dataWriter.WriteString("{\"desc\":" + peerConnection.LocalDescription);
                            signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());
                        }
                    }
                    else
                    {
                        peerConnection.AddIceCandidate(new RTCIceCandidate() { Candidate = message.GetNamedValue("desc").GetString() });
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void SetupSender()
        {

        }

        private void SendKinectData()
        {

        }
    }
}
