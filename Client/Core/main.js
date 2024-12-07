// Init
//////////////////////////////////////////////////////////
function StartConnection() {
    connection.start()
    .then(function () {
        let user = GetName();        
        document.querySelector(".send-button").disabled = false;     
        connection.invoke("JoinChat", User(user,GetRoom()),UserBadges())
        .then(function (response) {
            MessageInput.value = "";
            MessageInput.focus();            
            if (UserBadges().includes(Badges.Broadcaster) || UserBadges().includes(Badges.Moderator)) {
                document.querySelector(".mod-icons-area").classList.remove("d-none")
            }
            // Reset Retry 
            retryCount = 0;
        })
        .catch(function (err) {
            if (shouldReconnect) {
                setTimeout(() => {
                    RetryRecursively();
                    // console.error(err.toString());
                }, 5000); 
            }
        });
    })
    .catch(function (err) {
        RetryRecursively();
        // console.error(err.toString());
    });
}

function RetryStartConnection(timeout = 3000){
    // Only attempt to reconnect if the connection is not explicitly closed
    if (shouldReconnect) {
        setTimeout(() => StartConnection(), timeout); // Attempt to reconnect after 3 seconds
    } else {
        console.log("Connection was intentionally closed. No reconnection attempt.");
    }
}

function RetryRecursively() {
    if (retryCount < MaxRetry) {
        retryCount ++;
        CannotConnect(`Cannot connect , retry ${retryCount}..`,true);
        shouldReconnect = true;
        // Recursive
        RetryStartConnection();
    } else {
        retryCount ++;
        CannotConnect(`Cannot connect , retry ${retryCount} after 30 seconds`,true);
        RetryStartConnection(30000);
        // ConnectionTab.querySelector("div:first-child .spinner-border").remove();
        // CannotConnect("Cannot connect to the server!");
    }
}

function CannotConnect(message,isRetry = false) {
    ConnectionTab.classList.remove("d-none");
    ConnectionText.innerHTML = `<span class='welcome-text ${isRetry ? "retry-text" : "text-danger"}'>${message}</span>`;
}

function sendMessage() {
    let user = GetName();
    let message = document.getElementById("msgInput").value;
    if (message != "") {
        MessageInput.value = "";
        MessageMarkup.innerHTML = "";
        // MessageMarkup.focus();
        // clearTimeout(msgScrollTimeOut);
        connection.invoke("SendMessage", Message(User(user,GetRoom()),message),ReplyMessageId)
        .then(function (response) {
            CloseFloatingBox();
            CloseReplyMessage();
            if (isScrollAtEnd) {
                // msgScrollTimeOut = setTimeout(() => {
                //     ChatBox.scrollTo({ top: ChatBox.scrollHeight, behavior: "smooth" });
                // }, 2500);
            }
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
    }
}


