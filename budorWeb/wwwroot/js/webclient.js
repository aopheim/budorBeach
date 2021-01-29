const connection = new signalR.HubConnectionBuilder()
    .withUrl("/budorhub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

start();

connection.on("ConsoleLogMessage",
    (message) => {
        console.log(message);
    });

async function start() {
    try {
        await connection.start();
        console.assert(connection.state === signalR.HubConnectionState.Connected);
    } catch (err) {
        console.assert(connection.state === signalR.HubConnectionState.Disconnected);
        console.log(err);
        setTimeout(() => start(), 5000);
    }
}

async function sendMessageToClient() {
    try {
        await connection.invoke("SendMessageToAllClients", "Message from client");
    } catch (err) {
        console.error(err);
    }
}
