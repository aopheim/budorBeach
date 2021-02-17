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

connection.on("ReceiveCurrentSensorReading",
    (model) => {
        console.log('Received model: ', model);
        console.log(model.MeasuredAtUtc);
        document.getElementById("latestSensorReading").innerHTML = model["measuredAtUtc"];
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
