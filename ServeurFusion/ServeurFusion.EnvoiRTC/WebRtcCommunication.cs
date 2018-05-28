using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spitfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;


namespace ServeurFusion.EnvoiRTC
{
    /// <summary>
    /// Cette classe s'occupe de la communication en WebRTC
    /// </summary>
    public class WebRtcCommunication : IDisposable
    {
        private SpitfireRtc _spitfire;

        private WebSocket _signallingServer;

        //TODO: à voir s'il ne faut pas une liste
        private string _connectedUser;

        public WebRtcCommunication()
        {
            //Serveur de signalling
            _signallingServer = new WebSocket("ws://barnab2.tk:9090");
            _signallingServer.OnMessage += WebSocketOnMessage;

            _signallingServer.OnOpen += (sender, e) =>
            {
                Console.WriteLine("WebSocket Open");
            };
            _signallingServer.OnError += (sender, e) =>
            {
                Console.WriteLine("WebSocket error : " + e.Message);
            };
            _signallingServer.OnClose += (sender, e) =>
            {
                Console.WriteLine("WebSocket Close");
            };

            _signallingServer.Connect();



            //Active le log WebRTC détaillé dans la console 
#if DEBUG
            //SpitfireRtc.EnableLogging();
#endif
            _spitfire = new SpitfireRtc();


            SetupCallbacks();

            //Ajoute un serveur Stun de Google
            _spitfire.AddServerConfig(new Spitfire.ServerConfig()
            {
                Host = "stun.1.google.com",
                Port = 19302,
                Type = ServerType.Stun
            });

            bool started = _spitfire.InitializePeerConnection();
        }

        public void Connect()
        {
            _signallingServer.Send("{\"type\":\"login\", \"name\":\"uwp_sucks\"}");
        }

        private void WebSocketOnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("WebSocket : message reçu : " + e.Data);

            string message = e.Data;
            //Evite l'exception sur l'Hello World
            if (message.Contains("Hello world"))
                return;

            var messageJson = JObject.Parse(message);

            //Type de message reçu
            string messageType = messageJson.GetValue("type").ToString();
            switch (messageType)
            {
                case "login":
                    bool success = messageJson.GetValue("success").ToObject<bool>();
                    if (success)
                        Console.WriteLine("WebSocket : Login success");
                    else
                        Console.WriteLine("WebSocket : Login Error");
                    break;

                case "offer":
                    _connectedUser = messageJson.GetValue("name").ToString();
                    var offerJson = JObject.Parse(messageJson.GetValue("offer").ToString());
                    string sdpOffer = offerJson.GetValue("sdp").ToString();
                    _spitfire.SetOfferRequest(sdpOffer);
                    _spitfire.CreateOffer();
                    break;
                case "candidate":
                    var candidateStr = messageJson.GetValue("candidate").ToString();
                    if (!String.IsNullOrWhiteSpace(candidateStr))
                    {
                        Console.WriteLine($"AddCandidate : {candidateStr}");
                        var candidateJson = JObject.Parse(messageJson.GetValue("candidate").ToString());
                        string sdp = candidateJson.GetValue("candidate").ToString();
                        string sdpMid = candidateJson.GetValue("sdpMid").ToString();
                        int sdpMLineIndex = candidateJson.GetValue("sdpMLineIndex").ToObject<int>();
                        _spitfire.AddIceCandidate(sdpMid, sdpMLineIndex, sdp);
                    }
                    break;
            }

            Console.WriteLine($"Type : {messageJson.GetValue("type")}");
            Console.WriteLine("WebSocket said : " + e.Data);
        }

        private void SetupCallbacks()
        {
            _spitfire.OnIceCandidate += OnIceCandidate;
            _spitfire.OnDataChannelOpen += DataChannelOpen;
            _spitfire.OnDataChannelClose += SpitfireOnOnDataChannelClose;
            _spitfire.OnDataMessage += HandleMessage;
            _spitfire.OnIceStateChange += IceStateChange;
            _spitfire.OnSuccessAnswer += OnSuccessAnswer;
            _spitfire.OnFailure += SpitFireOnFail;
            _spitfire.OnError += SpitFireOnError;
            _spitfire.OnSuccessOffer += SpitFireOnSuccessAnswer;
        }

        private void SpitFireOnSuccessAnswer(SpitfireSdp sdp)
        {
            Console.WriteLine("SuccessAnswer : " + sdp.Sdp + " ; Type : " + sdp.Type);
        }

        private void SpitFireOnFail(string err)
        {
            Console.WriteLine("Fail : " + err);
        }

        private void SpitFireOnError()
        {
            Console.WriteLine("Error ! ");
        }

        private void OnIceCandidate(SpitfireIceCandidate iceCandidate)
        {
            Console.WriteLine("NEW CANDIDATE: {0} {1} {2}", iceCandidate.Sdp, iceCandidate.SdpMid, iceCandidate.SdpIndex);
            string answerJson;
            if (String.IsNullOrWhiteSpace(_connectedUser))
                answerJson = JsonConvert.SerializeObject(new { type = "candidate", candidate = new { candidate = iceCandidate.Sdp, sdpMid = iceCandidate.SdpMid, sdpMLineIndex = iceCandidate.SdpIndex } });
            else
                answerJson = JsonConvert.SerializeObject(new { type = "candidate", candidate = new { candidate = iceCandidate.Sdp, sdpMid = iceCandidate.SdpMid, sdpMLineIndex = iceCandidate.SdpIndex }, name = _connectedUser });
            _signallingServer.Send(answerJson);
        }

        /// <summary>
        /// Récupère la sdp de la answer créée
        /// </summary>
        /// <param name="sdp"></param>
        private void OnSuccessAnswer(SpitfireSdp sdp)
        {
            Console.WriteLine("SuccessAnswer : " + sdp.Sdp);
            _spitfire.SetOfferReply(sdp.Type.ToString(), sdp.Sdp);
            string answerJson;
            if (String.IsNullOrWhiteSpace(_connectedUser))
                answerJson = JsonConvert.SerializeObject(new { type = "answer", answer = new { type = "answer", sdp = sdp.Sdp } });
            else
                answerJson = JsonConvert.SerializeObject(new { type = "answer", answer = new { type = "answer", sdp = sdp.Sdp }, name = _connectedUser });
            Console.WriteLine("Answer sent : " + answerJson);
            _signallingServer.Send(answerJson);
        }

        private void IceStateChange(IceConnectionState state)
        {
            Console.WriteLine("State changed : " + state.ToString());
            if (state == IceConnectionState.Disconnected)
            {
                Console.WriteLine("ICE has left the building.");
            }
        }

        private static void HandleMessage(string label, DataMessage msg)
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

        private void SpitfireOnOnBufferAmountChange(string label, int previousBufferAmount, int currentBufferAmount,
            int bytesSent,
            int bytesReceived)
        {
        }

        private void SpitfireOnOnDataChannelClose(string label)
        {
            Console.WriteLine("Data Channel Closed!");
        }

        private void DataChannelOpen(string label)
        {
            Console.WriteLine("$Data Channel Opened!");
            Console.WriteLine(_spitfire.GetDataChannelInfo(label).Reliable);
        }

        public void Dispose()
        {
            if (_signallingServer != null)
                if (_signallingServer.IsAlive)
                    _signallingServer.Close();
        }
    }
}
