
function Onclose() {
    if (!IsDisconnected) {
    // console.warn("Disconnected from SignalR hub.");
    CannotConnect("Lost connection , try reconnect..");
    shouldReconnect = true;
    RetryStartConnection();
    }
}

///////////////////////////////////////////////////////////////////////////

function OnReConnecting(error) {
    console.log("Reconnecting:", error);
    // Update UI to indicate reconnection attempt
}

///////////////////////////////////////////////////////////////////////////

function OnReConnected() {
    updateStatus("Reconnected");
    CannotConnect("Successfully reconnected to SignalR hub.",true);
}

///////////////////////////////////////////////////////////////////////////

function WelcomeMessage(user,message,oldMessages,pinMessage) {
    // Reset Chat
    // ChatBox.innerHTML = "";
    CurrentUser = user;
    Username.innerHTML = GenerateUerBadges(user.roles) + " " + user.username;
    if (user.roles.includes(Badges.Broadcaster) || user.roles.includes(Badges.Moderator)) {
        document.querySelector(".mod-icons-area").classList.remove("d-none");
    }
    // Set OldMessages
    Object.keys(oldMessages).forEach(guid => {
        var msg = oldMessages[guid];
        CreateMessage(msg,true);
    });    

    setToolTips();
    // Check if the Message New After [Old Messages] set [New] keyword
    if (ChatBox.children.length == 0 || !ChatBox.lastChild.classList.contains("my-2")){
        const msg = document.createElement("div");
        msg.classList.add("my-2")
        msg.innerHTML = `<span class='welcome-text'>${message}</span>`;
        document.getElementById("ChatBox").appendChild(msg);
    }
    ConnectionTab.classList.add("d-none");
    // Set Pin Message
    setTimeout(() => {
        ChatBox.scrollTo({top: ChatBox.scrollHeight + 100000});
    }, 100);
    PinMessage(pinMessage);
}

///////////////////////////////////////////////////////////////////////////

async function ReceiveMessage(messages) {
    // Check if the Message New After [Old Messages] set [New] keyword
    if (ChatBox.children.length > 1 && ChatBox.lastChild.classList.contains("my-2")){
        const msg = document.createElement("div");
        msg.classList.add("new-message-line","d-flex","align-items-center","gap-3")
        msg.innerHTML = `<div><span></span></div><span data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Chat messages above were sent in the past hour">NEW</span>`;
        document.getElementById("ChatBox").appendChild(msg);
    }


    // Remove Messages when the buffered message reach 30 message
    if (document.querySelectorAll(".chat-box > div").length + messages.length >= 110) {
        var deleted = [...document.querySelectorAll(".chat-box > div")].slice(0,document.querySelectorAll(".chat-box > div").length > (110 * 2) ? document.querySelectorAll(".chat-box > div").length - 110 : 30);
        deleted.forEach(message => message.remove())
    }
    const fragment = document.createDocumentFragment();
    messages.forEach(msg => fragment.appendChild(CreateMessageElement(msg)));
    document.getElementById("ChatBox").appendChild(fragment);

    if (isScrollAtEnd) {
        ChatBox.scrollTo({top: ChatBox.scrollHeight/*,behavior: 'smooth'*/});
        ToggleScroller(false);
    } else {
        ToggleScroller();
    }

    setToolTips();
}

///////////////////////////////////////////////////////////////////////////

function OldMessages(messages) {
    const fragment = document.createDocumentFragment();
    messages.forEach(msg => fragment.appendChild(CreateMessageElement(msg,true)));
    document.getElementById("ChatBox").appendChild(fragment);
}

///////////////////////////////////////////////////////////////////////////

function LeftMessage(user) {
    const msg = document.createElement("div");
    msg.innerHTML = `<span class='welcome-text text-danger'>${user}</span>`;
    document.getElementById("ChatBox").appendChild(msg);
}

///////////////////////////////////////////////////////////////////////////

function MessageDeleted(message) {
    const msg = document.querySelector(`[data-message='${message}']`);
    msg.querySelector(".message").innerHTML = `<span class='welcome-text'><i>message deleted by a moderator.</i></span>`;
}

///////////////////////////////////////////////////////////////////////////

function Timeout(seconds) {
    setTimeout(async () => {
        MessageInput.setAttribute(
            "placeholder",
            `You timed out, can chat after ${seconds} seconds.`
        );
        if (seconds > 0) {
            Timeout(seconds - 1);
        } else if (seconds == 0) {
            MessageInput.setAttribute("placeholder", "Send a message.");
        }
    }, 1000);
}

///////////////////////////////////////////////////////////////////////////

function Banned() {
    MessageInput.value = '';
    MessageInput.setAttribute("placeholder", "You've been banned from Chat");
    MessageInput.setAttribute("disabled", "disabled");
    MessageMarkup.removeAttribute("contenteditable");
    document.querySelector(".send-button").disabled = true;
    EmojiButton.remove();
    // document.querySelector(".message-right").classList.add("ms-auto");
    // document.querySelector(".message-left").classList.add("d-none");
    ChatBox.classList.add("d-flex", "flex-column", "align-items-center", "justify-content-center");
    PinMsg.classList.add("d-none");
    RestorePinMsg.classList.add("d-none");
    ConnectionTab.classList.add("d-none");
    document.querySelector(".mod-icons-area").remove();
    document.querySelector(".settings-area").remove();
    document.querySelector(".message-left").remove();
    ChatBox.innerHTML = BanTemplate();
    CommunityBtn.remove();
    Community.remove();
    ChatBox.removeAttribute("id");
    EmojiButton.remove();
}

///////////////////////////////////////////////////////////////////////////

function UserBannedMessage(message) {
    const msg = document.createElement("div");
    msg.innerHTML = `<span class='welcome-text text-danger'>${message}</span>`;
    document.getElementById("ChatBox").appendChild(msg);
}

///////////////////////////////////////////////////////////////////////////

function UnBanned() {
    window.location.reload();
}

///////////////////////////////////////////////////////////////////////////

function PinMessage(pin) {
    var msgDate = new Date(pin.message.date);
    const options = {
        hour: 'numeric',
        minute: 'numeric',
    };
    const timeString = new Intl.DateTimeFormat('en-US', options).format(msgDate);

    PinMsg.innerHTML = PinMessageTemplate(
        `<span>${GenerateUerBadges(pin.user.roles, 14) + " " + pin.user.username}</span>`,
        handleMessageText(pin.message.text),
        GenerateUerBadges(pin.message.sender.roles) + " " + `<span style='color:${pin.message.sender.color};margin-bottom:-3px;'>${pin.message.sender.username}</span>`,
        timeString
    );

    if (PinMsg.querySelector("#UnPin")) {
        PinMsg.querySelector("#UnPin").addEventListener("click",function(e) {
            connection.invoke("UnPin", GetRoom()).then((response) => {});
        });
    }

    if (PinMsg.querySelector("#HidePin")) {
        PinMsg.querySelector("#HidePin").addEventListener("click",function(e) {
            PinMsg.classList.add("d-none");
            RestorePinMsg.classList.remove("d-none");
        });
    }
    

    PinMsg.classList.remove("d-none");
    RestorePin.classList.add("d-none");
    setToolTips();
}
///////////////////////////////////////////////////////////////////////////

function UnPinMessage() {
    PinMsg.innerHTML = '';
    PinMsg.classList.add("d-none");
    RestorePinMsg.classList.add("d-none");
}

///////////////////////////////////////////////////////////////////////////

function CurrentUsers(users) {
    AllUsers = users;
    document.querySelector(".community").innerHTML = CommunityTemplate();


    // Check if current user is timmed out 
    var isCurrentClientTimmedOut = AllUsers.flat().filter(s => s.userInfo.user.username == CurrentUser.username);
    if (isCurrentClientTimmedOut.length > 0 && isCurrentClientTimmedOut[0].userInfo.isTimeout) {
        PreventMessage(true,false);
    }
    
}

///////////////////////////////////////////////////////////////////////////
// Function to intentionally close the connection

function CloseConnection() {
    shouldReconnect = false;
    IsDisconnected = true;
    connection.stop().then(() => {
        ScrollBottom.classList.add("d-none");
        // ConnectionTab.classList.add("d-none");
        CannotConnect("Client Disconnected.");
        ConnectionTab.querySelector("div:first-child .spinner-border").remove();
        EmojiButton.remove();
        MessageInput.setAttribute("placeholder", "Chat Disconnected.");
        MessageInput.setAttribute("disabled", "disabled");
        MessageMarkup.removeAttribute("contenteditable");
        document.querySelector(".send-button").disabled = true;
        
    });
}

function RoomClosed() {
    shouldReconnect = true;
    IsDisconnected = false;
    connection.stop().then(() => {
        ScrollBottom.classList.add("d-none");
        // ConnectionTab.classList.add("d-none");
        CannotConnect("Lost connection , try reconnect..");
        document.querySelector(".send-button").disabled = true;
    });
}