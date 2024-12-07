///////////////////////////////////////////////////////////////////////////
// Connection

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7179/chat")
    // .configureLogging(signalR.LogLevel.Trace)
    // .withAutomaticReconnect()
    .build();

StartConnection();

///////////////////////////////////////////////////////////////////////////
// Hub Built-In Methods

connection.onclose(() => Onclose());

connection.onreconnecting((error) => OnReConnecting(error));

connection.onreconnected(() => OnReConnected());

///////////////////////////////////////////////////////////////////////////
// Custom Methods

connection.on("OldMessages", OldMessages);

connection.on("WelcomeMessage", WelcomeMessage);

connection.on("LeftMessage",  LeftMessage);

connection.on("ReceiveMessage", ReceiveMessage);

connection.on("MessageDeleted", MessageDeleted);

connection.on("Timeout", (timeout) => PreventMessage());

connection.on("TimeOutFinished", (timeout) => PreventMessage(false));

connection.on("Banned", Banned);

connection.on("UnBanned", UnBanned);

// connection.on("UserBannedMessage", UserBannedMessage);

connection.on("PinMessage", PinMessage);

connection.on("UnPinMessage", UnPinMessage);

connection.on("CurrentUsers", CurrentUsers);

connection.on("CloseConnection", CloseConnection);

connection.on("RoomClosed", RoomClosed);
