using Org.WebRtc;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

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
                new RTCIceServer() { Url = "stun:stun.1.google.com:19302" }
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
                    RTCSessionDescription rtcSessionDescription = await peerConnection.CreateOffer();
                    await peerConnection.SetLocalDescription(rtcSessionDescription);

                    dataWriter.WriteString("{\"type\":\"offer\", \"offer\":" +
                                           "{\"type\":\"offer\", \"sdp\":\"" + peerConnection.LocalDescription.Sdp.ToString() + "\"}}");
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
        private async void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs e)
        {
            // Lecture réponses WebSocket
            using (DataReader reader = e.GetDataReader())
            {
                try
                {
                    // Parsing réponse WebSocket
                    JObject message = JObject.Parse(reader.ReadString(e.GetDataReader().UnconsumedBufferLength).Replace("\\", ""));

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
                            RTCSessionDescription rtcRemoteDescription = new RTCSessionDescription()
                            {
                                Type = RTCSdpType.Offer,
                                Sdp = descOffer.GetValue("sdp").ToString()
                            };

                            Task remoteDescriptionTask = Task.Factory.StartNew(async () =>
                            {
                                Debug.WriteLine("Remote desc done");
                                await peerConnection.SetRemoteDescription(rtcRemoteDescription);
                                
                            }).ContinueWith(async _ =>
                            {
                                Debug.WriteLine("Answer created");
                                RTCSessionDescription rtcSessionDescription = await peerConnection.CreateAnswer();
                                
                                return rtcSessionDescription;
                            }).ContinueWith(async (answer) =>
                            {
                                Debug.WriteLine("local desc done");
                                await peerConnection.SetLocalDescription(answer.Result.Result);
                                
                            }).ContinueWith(async _ =>
                            {
                                Debug.WriteLine("{\"type\":\"answer\", \"answer\":" +
                                                "{\"type\":\"answer\", \"sdp\":\"" + peerConnection.LocalDescription.Sdp.ToString() + "\"}}");

                                dataWriter.WriteString("{\"type\":\"answer\", \"answer\":" +
                                                       "{\"type\":\"answer\", \"sdp\":\"" + peerConnection.LocalDescription.Sdp.ToString() + "\"}}");
                                await signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());
                            });

                            //Debug.WriteLine(rtcDataChannel.ReadyState);
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
                                RTCIceCandidate rtcIceCandidate = new RTCIceCandidate()
                                {
                                    Candidate = descCandidate.GetValue("candidate").ToString(),
                                    SdpMid = descCandidate.GetValue("sdpMid").ToString(),
                                    SdpMLineIndex = ushort.Parse(descCandidate.GetValue("sdpMLineIndex").ToString())
                                };
                                await peerConnection.AddIceCandidate(rtcIceCandidate);
                            }
                            break;

                        default:
                            Debug.WriteLine("Unsupported SDP type");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Invalid Json :" + ex.Message);
                }
            }

        }

        // Initialisation WebSocket
        private void SetupWebSocket()
        {
            dataWriter.WriteString("{\"type\":\"login\", \"name\":\"sadam\"}");
            signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer()).AsTask().Wait();
            if (peerConnection == null)
            {
                StartRTC(true);
            }
        }








        public async Task RTCRemoteDescription(RTCSessionDescription rtcRemoteDescription)
        {
            await peerConnection.SetRemoteDescription(rtcRemoteDescription);
        }

        public async Task RTCLocalDescription(RTCSessionDescription rtcLocalDescription)
        {
            await peerConnection.SetLocalDescription(rtcLocalDescription);
        }

        public async Task<RTCSessionDescription> RTCAnswer()
        {
            RTCSessionDescription rtcSessionDescription = await peerConnection.CreateAnswer();
            return rtcSessionDescription;
        }

        public async Task RTCWriteAnswer()
        {
            Debug.WriteLine("{\"type\":\"answer\", \"answer\":" +
                            "{\"type\":\"answer\", \"sdp\":\"" + peerConnection.LocalDescription.Sdp.ToString() + "\"}}");

            dataWriter.WriteString("{\"type\":\"answer\", \"answer\":" +
                                   "{\"type\":\"answer\", \"sdp\":\"" + peerConnection.LocalDescription.Sdp.ToString() + "\"}}");
            await signalingChannel.OutputStream.WriteAsync(dataWriter.DetachBuffer());
        }
    }
}
