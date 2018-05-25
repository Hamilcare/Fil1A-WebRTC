using Org.WebRtc;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Newtonsoft.Json.Linq;

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
            signalingChannel.MessageReceived += MessageReceived;
            signalingChannel.ConnectAsync(new Uri("ws://barnab2.tk:9090")).AsTask().Wait();

            dataWriter = new DataWriter();

            iceList = new List<RTCIceServer>
            {
                new RTCIceServer() { Url = "stun:stun.l.google.com:19302" },
                new RTCIceServer() { Credential = "YzYNCouZM1mhqhmseWk6", Url = "turn:13.250.13.83:3478?transport=udp", Username = "YzYNCouZM1mhqhmseWk6" },
                new RTCIceServer() { Url = "stun:stun1.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun2.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun3.l.google.com:19302" },
                new RTCIceServer() { Url = "stun:stun4.l.google.com:19302" }
            };

            SetupWebSocket();
        }

        // Initialisation WebRTC
        private void StartRTC(bool isInitiator)
        {
            rtcConfiguration = new RTCConfiguration()
            {
                BundlePolicy = RTCBundlePolicy.Balanced,
                IceTransportPolicy = RTCIceTransportPolicy.All,
                IceServers = iceList
            };
            peerConnection = new RTCPeerConnection(rtcConfiguration);

            peerConnection.OnIceCandidate += async (RTCPeerConnectionIceEvent candidate) =>
            {
                try
                {
                    dataWriter.WriteString("{\"candidate\":" + candidate.Candidate + "}");
                    await signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());

                    Debug.WriteLine("Candidate success");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Candidate fail : " + e.Message);
                }

            };

            peerConnection.OnNegotiationNeeded += async delegate
            {
                try
                {
                    await peerConnection.CreateOffer();
                    await peerConnection.SetLocalDescription(peerConnection.RemoteDescription);

                    dataWriter.WriteString("{\"desc\":" + peerConnection.LocalDescription + "}");
                    await signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());

                    Debug.WriteLine("Negociation success");
                }
                catch (Exception e)
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

        // Gestion des messges reçus par la connexion WebSocket
        private async void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            // Lecture réponses WebSocket
            using (DataReader reader = args.GetDataReader())
            {
                JObject message = new JObject();
                try
                {
                    // Parsing réponse WebSocket
                    string json = reader.ReadString(args.GetDataReader().UnconsumedBufferLength).Replace("\\", "");
                    message = JObject.Parse(json);
                    Debug.WriteLine(json);

                    // Types de messages...
                    switch (message.GetValue("type").ToString())
                    {
                        // Message "offer"
                        case "offer":
                            if (peerConnection == null)
                            {
                                StartRTC(false);
                            }
                            JObject descOffer = JObject.Parse(message.GetValue("offer").ToString());
                            Debug.WriteLine(descOffer.GetValue("sdp").ToString());
                            peerConnection.SetRemoteDescription(new RTCSessionDescription(RTCSdpType.Offer, descOffer.GetValue("sdp").ToString()));
                            peerConnection.CreateAnswer();
                            peerConnection.SetLocalDescription(new RTCSessionDescription(RTCSdpType.Offer, descOffer.GetValue("sdp").ToString()));

                            // TO DO problème avec l'appel de peerConnection.LocalDescription

                            //dataWriter.WriteString("{\"type\":\"answer\", \"answer\":" + peerConnection.LocalDescription + "}");
                            await signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());
                            Debug.WriteLine(rtcDataChannel.ReadyState);
                            break;

                        // Message "answer"
                        case "answer":
                            if (peerConnection == null)
                            {
                                StartRTC(false);
                            }
                            var descAnswer = message.GetValue("answer");
                            await peerConnection.SetRemoteDescription(new RTCSessionDescription() { Sdp = descAnswer.ToString() });
                            break;
                            
                        // Message "login"
                        case "login":
                            Debug.WriteLine("Sucess login");
                            break;

                        // Message "candidate"
                        case "candidate":
                            if (peerConnection == null)
                            {
                                StartRTC(false);
                            }
                            var candidate = message.GetValue("candidate").ToString();
                            if (!"".Equals(candidate))
                            {
                                JObject descCandidate = JObject.Parse(message.GetValue("candidate").ToString());
                                Debug.WriteLine(descCandidate);
                                await peerConnection.AddIceCandidate(new RTCIceCandidate()
                                {
                                    Candidate = descCandidate.GetValue("candidate").ToString(),
                                    SdpMid = descCandidate.GetValue("sdpMid").ToString(),
                                    SdpMLineIndex = ushort.Parse(descCandidate.GetValue("sdpMLineIndex").ToString())
                                });
                            }
                            break;

                        default:
                            Debug.WriteLine("Unsupported SDP type");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Invalid Json :" + e.Message);
                }
            }

        }

        // Initialisation WebSocket
        private void SetupWebSocket()
        {
            dataWriter.WriteString("{\"type\":\"login\", \"name\":\"sadam\"}");
            signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());
            if (peerConnection == null)
            {
                StartRTC(true);
            }
        }

        private void SendKinectData()
        {

        }
    }
}
