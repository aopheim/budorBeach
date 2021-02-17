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
        document.getElementById("temperatureInDegreesC").innerHTML = model["temperatureInDegreesC"];
        document.getElementById("relativeHumidityInPercent").innerHTML = model["relativeHumidityInPercent"];
        document.getElementById("pressureInhPa").innerHTML = model["pressureInhPa"];
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
