var signalingChannel = new WebSocket('ws://barnab2.tk:9090');
var configuration = {
    iceServers: [{
        urls: "stun:stun.1.google.com:19302"
    }]
};
var pc;

var loginInput = document.querySelector('#loginInput');
var loginBtn = document.querySelector('#loginBtn');

var otherUsernameInput = document.querySelector('#otherUsernameInput');
var connectToOtherUsernameBtn = document.querySelector('#connectToOtherUsernameBtn');
var connectedUser;

var msgInput = document.querySelector('#msgInput');
var sendMsgBtn = document.querySelector('#sendMsgBtn');

var removeSkeletonBtn = document.querySelector('#removeSkeleton');

removeSkeletonBtn.addEventListener("click", function(event) {
	removeSkeletonFromScene();
});

sendMsgBtn.addEventListener("click", function(event) {
    txt = msgInput.value;
    sendSkeletonChannel(txt);
});

loginBtn.addEventListener("click", function(event) {
    console.log("Votre pseudo est : " + loginInput.value);
    name = loginInput.value
    if (name.length > 0) {
        send({
            type: "login",
            name: name
        });
    }
});

connectToOtherUsernameBtn.addEventListener("click", function(event) {
    if (!pc)
        start(true);
});

//Easier dialog with signaling channel
function send(message) {
    if (connectedUser) {
        message.name = connectedUser;
    }

    signalingChannel.send(JSON.stringify(message));
};

// call start() to initiate
function start(isInitiator) {
    if (!connectedUser)
        connectedUser = otherUsernameInput.value;
    console.log("Beginning start method, connectedUser is " + connectedUser);
    pc = new RTCPeerConnection(configuration);

    // send any ice candidates to the other peer
    pc.onicecandidate = function(evt) {
        send({
            type: "candidate",
            candidate: event.candidate
        });
    };

    console.log(pc);
    // let the "negotiationneeded" event trigger offer generation
    pc.onnegotiationneeded = function() {
        pc.createOffer().then(function(offer) {
                return pc.setLocalDescription(offer);
            })
            .then(function() {
                // send the offer to the other peer
                send({
                    type: "offer",
                    offer: pc.localDescription
                });
            });
        //.catch(logError);
    };

    if (isInitiator) {
        // create data channel and setup chat
        channelSkeleton = pc.createDataChannel("skeletonChannel");
        setupSkeletonChannel();

        channelCloud = pc.createDataChannel("cloudChannel");
        setupCloudChannel();
    } else {
        // setup chat on incoming data channel

        pc.ondatachannel = function(evt) {
					console.log(evt.channel.id);
            if (evt.channel.id == 1) {
                channelSkeleton = evt.channel;
                setupSkeletonChannel();
            } else if (evt.channel.id == 3) {
                channelCloud = evt.channel;
                setupCloudChannel();
            }
        };
    }
}


signalingChannel.onmessage = function(evt) {
    console.log(evt);
    var data = JSON.parse(evt.data);

    if (data.type == "coucou")
        return;


    switch (data.type) {
        // if we get an offer, we need to reply with an answer
        case "offer":
            connectedUser = data.name;
            if (!pc)
                start();
            var desc = data.offer;
            pc.setRemoteDescription(desc).then(function() {
                    return pc.createAnswer();
                })
                .then(function(answer) {
                    return pc.setLocalDescription(answer);
                })
                .then(function() {
                    send({
                        type: "answer",
                        answer: pc.localDescription
                    });
                });
            //.catch(logError);
            break;
        case "answer":
            if (!pc)
                start(false);
            var desc = data.answer;
            pc.setRemoteDescription(desc); //.catch(logError);
            break;
        case "login":
            onLogin(data.success);
            break;
        case "candidate":
            if (!pc)
                start(false);
            if (data.candidate != null)
                pc.addIceCandidate(data.candidate); //.catch(logError);
            break;
        default:
            console.log("Unsupported SDP type. Your code may differ here.");
            break;
    }

};

function onLogin(success) {
    init();
    if (success === false) {
        alert("oops...try a different username");
    } else {
        console.log("Log successfuly to signaling server");
    }
}



function setupSkeletonChannel() {
    channelSkeleton.onopen = function() {
        console.log("skeletonChannel openned");
        sendMsgBtn.disabled = false;
    };

    channelSkeleton.onmessage = function(evt) {
        //console.log("Message received on skeletonChannel");
				//console.log(evt.data);
        handleReceivedSkeletonCoordinates(evt.data);
        // lance l'animation du cube
        //animate();
    };
}

function setupCloudChannel() {
    channelCloud.onopen = function() {
        console.log("cloudChannel openned");
    };

    channelCloud.onmessage = function(evt) {
        //console.log("Message received on cloudChannel");
				//console.log(evt.data);
        handleReceivedCloudCoordinates(evt.data);
        animate();
    };
}

function sendSkeletonChannel(msg) {
    channelSkeleton.send(JSON.stringify(ms));
}

function logError(error) {
    log(error.name + ": " + error.message);
}
