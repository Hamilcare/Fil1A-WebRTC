var ws = null;
var user = "";
var user2 = "";
var config = {"iceServers":[{"url":"stun:stun.l.google.com:19302"}]};
var peerConnection;
var dataChannelCloud; //DataChannel pour transférer le nuage de points
var dataChannelSkeleton; //DataChannel pour transférer le squelette
var message = [];



function main(){
    ws = new WebSocket("ws://192.168.43.222:8088");
    setMyId("client");
    ws.onopen = function(){
        console.log("Websocket opened");
    };
    ws.onclose = function(){
        console.log("Websocket closed");
    };
    ws.onmessage = function(e){
        console.log("Websocket message received: " + e.data);

        var json = JSON.parse(e.data);

        if(json.action == "candidate"){
            if(json.to == user){
                processIce(json.data);
            }
        } else if(json.action == "offer"){
            if(json.to == user){
                user2 = json.from;
                processOffer(json.data)
            }
        } else if(json.action == "answer"){
            if(json.to == user){
                processAnswer(json.data);
            }
        }
    };
    ws.onerror = function(){
        console.log("Websocket error");
    };
}

document.getElementById('Start').onclick = function(){
    connectTo("fusion");
};

function setMyId(e){
    user = e;
    return false;
}

function connectTo(e){
    user2 = e;
    openDataChannel();
    var sdpConstraints = { offerToReceiveAudio: true,  offerToReceiveVideo: false };
    peerConnection.createOffer(sdpConstraints).then(function (sdp) {
        peerConnection.setLocalDescription(sdp);
        sendNegotiation("offer", sdp);
        console.log("------ SEND OFFER ------");
    }, function (err) {
        console.log(err)
    });

}

function sendDirect(e){
    dataChannel.send(e);
    console.log("Sending over datachannel: "+e);
}

function openDataChannel (){
    peerConnection = new RTCPeerConnection(config);
    peerConnection.onicecandidate = function(e){
        if (!peerConnection || !e || !e.candidate) return;
        var candidate = e.candidate;
        sendNegotiation("candidate", candidate);
    };

	
    //Recupere les evenements lié a la creation de dataChannel sur la peerConnection
    peerConnection.ondatachannel = function (ev) {
      if(ev.channel.id==1)
      {
        dataChannelCloud = ev.channel; //Recupere le dataChannel liés au nuage de points
      }
      else if(ev.channel.id==2)
      {
        dataChannelSkeleton = ev.channel; //Recupere le dataChannel lié au squelette
      }
	  
	  		//Evenements du dataChannel liés au nuage de points
    dataChannelCloud.onopen = function(){console.log("------ Ouverture du dataChannelCloud ------");};
    dataChannelCloud.onclose = function(){console.log("------ Fermeture du dataChannelCloud ------")};
    dataChannelCloud.onerror = function(){console.log("------ Erreur du dataChannelCloud ------")};
    dataChannelCloud.onmessage = function(e){
        var fr = new FileReader();

        //Genere un tableau dans le FileReader
        fr.readAsArrayBuffer(e.data);

        //Evenement geré lorsque le fichier est completement chargé
        fr.onload = function(){
            var data = new Int16Array(fr.result);
            generatePoint(data);
        };
      };

    //Evenements du dataChannel liés au squelette
    if(dataChannelSkeleton!=null)
    {
        dataChannelSkeleton.onopen = function(){console.log("------ Ouverture du dataChannelSkeleton ------");};
        dataChannelSkeleton.onclose = function(){console.log("------ Fermeture du dataChannelSkeleton ------")};
        dataChannelSkeleton.onerror = function(){console.log("------ Erreur du dataChannelSkeleton ------")};
        dataChannelSkeleton.onmessage = function(e){
          var fr = new FileReader();

          //Genere un tableau dans le FileReader
          fr.readAsText(e.data);

          //Evenement geré lorsque le fichier est completement chargé
          fr.onload = function(){
                var skeleton = JSON.parse(fr.result);
                var joints = skeleton.joints; // array of joints*/
                generateSkeleton(joints); 
          }; 
            }; 
    }
    };
	

    return peerConnection;
}

function sendNegotiation(type, sdp){
    var json = { from: user, to: user2, action: type, data: sdp};
    ws.send(JSON.stringify(json));
    console.log("Sending ["+user+"] to ["+user2+"]: " + JSON.stringify(sdp));
}

function processOffer(offer){
    var peerConnection = openDataChannel();
    peerConnection.setRemoteDescription(new RTCSessionDescription(offer));

    var sdpConstraints = {'mandatory':
        {
            'OfferToReceiveAudio': false,
            'OfferToReceiveVideo': false
        }
    };

    peerConnection.createAnswer(sdpConstraints).then(function (sdp) {
        return peerConnection.setLocalDescription(sdp).then(function() {
            sendNegotiation("answer", sdp);
            console.log("------ SEND ANSWER ------");
        })
    }, function(err) {
        console.log(err)
    });
    console.log("------ PROCESSED OFFER ------");

}

function processAnswer(answer){
    peerConnection.setRemoteDescription(new RTCSessionDescription(answer));
    console.log("------ PROCESSED ANSWER ------");
    return true;
}

function processIce(iceCandidate){
    peerConnection.addIceCandidate(new RTCIceCandidate(iceCandidate));
}
