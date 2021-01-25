const connection = new signalR.HubConnectionBuilder()
    .withUrl("/budorhub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
var hubProxy = $.connection.budorHub.

function start() {
    try {
        connection.start().done(function () {
            console.log("SignalR Connected.");

            try {
                console.log("Trying invoke");
                connection.invoke("SendMessageToAllClients", "this is my sent message");
            } catch (err) {
                console.log(err);
            }

            connection.on("ReceiveMessage",
                (message) => {
                    console.log("Received message");
                    console.log(message);
                });
        }
        );
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};


connection.onclose(start);
// Start the connection.
start()

