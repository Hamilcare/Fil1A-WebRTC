﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spitfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ServeurFusion.EnvoiRTC
{
    /// <summary>
    /// WebRTC protocol implementation for communication with a client
    /// </summary>
    public class WebRtcCommunication : IDisposable
    {
        private UdpClient _udpServer;

        private SpitfireRtc _rtcPeerConnection;

        private WebSocket _signallingServer;

        // TODO: List of users for multi-clients implementation ?
        private string _connectedUser;

        private string _dataChannelLabel;

        public WebRtcCommunication()
        {
            _udpServer = new UdpClient(9877);

            // Setup signaling server
            _signallingServer = new WebSocket("ws://barnab2.tk:9090");
            _signallingServer.OnMessage += WebSocketOnMessage;

            _signallingServer.OnOpen += (sender, e) =>
            {
                Console.WriteLine("WebSocket Opened");
            };
            _signallingServer.OnError += (sender, e) =>
            {
                Console.WriteLine("WebSocket error : " + e.Message);
            };
            _signallingServer.OnClose += (sender, e) =>
            {
                Console.WriteLine("WebSocket Closed");
            };

            _signallingServer.Connect();

            // Enable WebRTC full logging
#if DEBUG
            //SpitfireRtc.EnableLogging();
#endif
            _rtcPeerConnection = new SpitfireRtc();


            SetupCallbacks();

            // Adding Stun server
            _rtcPeerConnection.AddServerConfig(new Spitfire.ServerConfig()
            {
                Host = "stun.1.google.com",
                Port = 19302,
                Type = ServerType.Stun
            });

            bool started = _rtcPeerConnection.InitializePeerConnection();
        }

        /// <summary>
        /// Connecting to signaling server
        /// </summary>
        public void Connect()
        {
            _signallingServer.Send("{\"type\":\"login\", \"name\":\"xx\"}");
        }

        /// <summary>
        /// WebSocket received messages handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Data sent</param>
        private void WebSocketOnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("WebSocket received message : " + e.Data);

            string message = e.Data;

            // Ignore Hello World message
            if (message.Contains("Hello world"))
                return;

            // Parsing received message
            var messageJson = JObject.Parse(message);

            // Switch received message type
            string messageType = messageJson.GetValue("type").ToString();
            Console.WriteLine("WebSocket received message type : " + messageType);
            switch (messageType)
            {
                // Login message
                case "login":
                    bool success = messageJson.GetValue("success").ToObject<bool>();
                    if (success)
                        Console.WriteLine("WebSocket : Login success");
                    else
                        Console.WriteLine("WebSocket : Login error");
                    break;

                // Offer message
                case "offer":
                    _connectedUser = messageJson.GetValue("name").ToString();
                    var offerJson = JObject.Parse(messageJson.GetValue("offer").ToString());
                    string sdpOffer = offerJson.GetValue("sdp").ToString();
                    _rtcPeerConnection.SetOfferRequest(sdpOffer);
                    break;

                // Candidate message
                case "candidate":
                    var candidateStr = messageJson.GetValue("candidate").ToString();
                    if (!String.IsNullOrWhiteSpace(candidateStr))
                    {
                        Console.WriteLine($"AddCandidate : {candidateStr}");
                        var candidateJson = JObject.Parse(messageJson.GetValue("candidate").ToString());
                        string sdp = candidateJson.GetValue("candidate").ToString();
                        string sdpMid = candidateJson.GetValue("sdpMid").ToString();
                        int sdpMLineIndex = candidateJson.GetValue("sdpMLineIndex").ToObject<int>();
                        _rtcPeerConnection.AddIceCandidate(sdpMid, sdpMLineIndex, sdp);
                    }
                    break;
            }
        }

        /// <summary>
        /// Setup WebRTC events handlers
        /// </summary>
        private void SetupCallbacks()
        {
            _rtcPeerConnection.OnIceCandidate += OnIceCandidate;
            _rtcPeerConnection.OnDataChannelOpen += DataChannelOpen;
            _rtcPeerConnection.OnDataChannelClose += OnDataChannelClose;
            _rtcPeerConnection.OnDataMessage += HandleMessage;
            _rtcPeerConnection.OnIceStateChange += IceStateChange;
            _rtcPeerConnection.OnSuccessAnswer += OnSuccessAnswer;
            _rtcPeerConnection.OnFailure += OnFail;
            _rtcPeerConnection.OnError += OnError;
            _rtcPeerConnection.OnSuccessOffer += OnSuccessOffer;
        }

        /// <summary>
        /// Console message when success Offer
        /// </summary>
        /// <param name="sdp">SDP of the offer</param>
        private void OnSuccessOffer(SpitfireSdp sdp)
        {
            Console.WriteLine("SuccessOffer : " + sdp.Sdp + " ; Type : " + sdp.Type);
        }

        /// <summary>
        /// Console message when WebRTC fail
        /// </summary>
        /// <param name="err"></param>
        private void OnFail(string err)
        {
            Console.WriteLine("WebRTC Fail : " + err);
        }

        /// <summary>
        /// Console message when WebRTC error
        /// </summary>
        private void OnError()
        {
            Console.WriteLine("WebRTC error");
        }

        /// <summary>
        /// Handling new IceCandidate
        /// </summary>
        /// <param name="iceCandidate">IceCandidate</param>
        private void OnIceCandidate(SpitfireIceCandidate iceCandidate)
        {
            Console.WriteLine("New IceCandidate : {0} {1} {2}", iceCandidate.Sdp, iceCandidate.SdpMid, iceCandidate.SdpIndex);
            string answerJson;
            if (String.IsNullOrWhiteSpace(_connectedUser))
                answerJson = JsonConvert.SerializeObject(new { type = "candidate", candidate = new { candidate = iceCandidate.Sdp, sdpMid = iceCandidate.SdpMid,
                                                               sdpMLineIndex = iceCandidate.SdpIndex } });
            else
                answerJson = JsonConvert.SerializeObject(new { type = "candidate", candidate = new { candidate = iceCandidate.Sdp, sdpMid = iceCandidate.SdpMid,
                                                               sdpMLineIndex = iceCandidate.SdpIndex }, name = _connectedUser });
            _signallingServer.Send(answerJson);
        }

        /// <summary>
        /// Get the SDP of the created Answer
        /// </summary>
        /// <param name="sdp">SDP of the Answer</param>
        private void OnSuccessAnswer(SpitfireSdp sdp)
        {
            Console.WriteLine("SuccessAnswer : " + sdp.Sdp);
            _rtcPeerConnection.SetOfferReply(sdp.Type.ToString(), sdp.Sdp);
            string answerJson;
            if (String.IsNullOrWhiteSpace(_connectedUser))
                answerJson = JsonConvert.SerializeObject(new { type = "answer", answer = new { type = "answer", sdp = sdp.Sdp } });
            else
                answerJson = JsonConvert.SerializeObject(new { type = "answer", answer = new { type = "answer", sdp = sdp.Sdp }, name = _connectedUser });
            Console.WriteLine("Answer sent : " + answerJson);
            _signallingServer.Send(answerJson);
        }

        /// <summary>
        /// Console message when IceState changed
        /// </summary>
        /// <param name="state">IceSate</param>
        private void IceStateChange(IceConnectionState state)
        {
            Console.WriteLine("IceState changed : " + state.ToString());
        }

        /// <summary>
        /// Console message when a message is received on DataChannel
        /// </summary>
        /// <param name="label">DataChannel name</param>
        /// <param name="msg">Received message</param>
        private void HandleMessage(string label, DataMessage msg)
        {
            if (msg.IsBinary)
            {
                Console.WriteLine(msg.RawData.Length);
            }
            else
            {
                Console.WriteLine(msg.Data);
            }
        }

        /// <summary>
        /// Console message when DataChannel is closed
        /// </summary>
        /// <param name="label">DataChannel name</param>
        private void OnDataChannelClose(string label)
        {
            Console.WriteLine("DataChannel Closed");
        }

        private void DataChannelOpen(string label)
        {
            Console.WriteLine("DataChannel Opened");
            _dataChannelLabel = label;

            var remoteEP = new IPEndPoint(IPAddress.Any, 9876);
            while (true)
            {
                // Receiving frames from KinectStreamer
                var data = _udpServer.Receive(ref remoteEP);
                int count = 0;
                // Processing Skeleton
                Skeleton skeleton = new Skeleton()
                {
                    Timestamp = BitConverter.ToInt64(data, 0),
                    Tag = data[8],
                    SkeletonPoints = new List<SkeletonPoint>()
                };
                count = 9;
                while(count < 409)
                {
                    // Processing SkeletonPoints
                    SkeletonPoint skeletonPoint = new SkeletonPoint();
                    skeletonPoint.X = BitConverter.ToSingle(data, count);
                    count += 4;
                    skeletonPoint.Y = BitConverter.ToSingle(data, count);
                    count += 4;
                    skeletonPoint.Z = BitConverter.ToSingle(data, count);
                    count += 4;
                    skeletonPoint.R = data[count];
                    count += 1;
                    skeletonPoint.G = data[count];
                    count += 1;
                    skeletonPoint.B = data[count];
                    count += 1;
                    skeletonPoint.Tag = data[count];
                    count += 1;
                    skeleton.SkeletonPoints.Add(skeletonPoint);
                }

                // Formatting and sending data
                string formattedMessage = "";
                skeleton.SkeletonPoints.ForEach(s => formattedMessage+= $"{s.X};{s.Y};{s.Z};{s.R};{s.G};{s.B};".Replace(',','.'));
                formattedMessage = formattedMessage.Remove(formattedMessage.Length-1, 1);
                //Console.WriteLine(formattedMessage+"\n");

                _rtcPeerConnection.DataChannelSendText(_dataChannelLabel, formattedMessage);
            }

            //_rtcPeerConnection.DataChannelSendText(_dataChannelLabel, "HELLO WORLD!");
            //Console.WriteLine(_rtcPeerConnection.GetDataChannelInfo(label).Reliable);
        }

        /// <summary>
        /// Closing connection
        /// </summary>
        public void Dispose()
        {
            if (_signallingServer != null)
                if (_signallingServer.IsAlive)
                    _signallingServer.Close();
        }
    }
}
