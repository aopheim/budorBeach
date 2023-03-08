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

connection.on("SendSensorReading",
    (model) => {
        document.getElementById("temperatureInDegreesC").innerHTML = round(model["temperatureInDegreesC"], 1) + "°C";
        document.getElementById("relativeHumidityInPercent").innerHTML = round(model["relativeHumidityInPercent"], 1) + "%";
        document.getElementById("pressureInhPa").innerHTML = round(model["pressureInhPa"], 1) + "hPa";

        var unixTimeInUtc = Date.parse(model["measuredAtUtc"]);
        var date = new Date(unixTimeInUtc);
        document.getElementById("lastUpdatedAt").innerHTML = date.toLocaleString("no-NO");
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

function round(value, precision) {
    var multiplier = Math.pow(10, precision || 0);
    return Math.round(value * multiplier) / multiplier;
}
