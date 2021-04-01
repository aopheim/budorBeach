/////////
var tempValues = [];
var humidityValues = [];
var pressureValues = [];
var timeStamps = [];
var chartModel;



function showChart() {
    tempValues = chartModel.temperatureReadings;
    humidityValues = chartModel.humidityReadings;
    pressureValues = chartModel.pressureReadings;
    timeStamps = chartModel.measuredAt;

    console.log(tempValues);
    console.log(timeStamps);
    var chartData = {
        labels: timeStamps,
        datasets: [
            {
                data: tempValues,
                label: 'Temperatur',
                borderColor: "#3e95cd",
                fill: false
            },
            {
                data: humidityValues,
                label: 'Relativ luftfuktighet (%)',
                borderColor: "#8e5ea2",
                fill: false
            },
            {
                data: pressureValues,
                label: 'Trykk (hPa)',
                borderColor: "#3cba9f",
                fill: false
            }
        ]
    };
    console.log(chartData);
    let config = {
        type: 'line',
        data: chartData,
        options: {
            responsive: true,
            hoverMode: 'index',
            stacked: false,
            title: {
                display: true,
                text: 'Data fra siste 12 timer'
            }
        }
    };
    console.log('config:', config);
    var ctx = document.getElementById('sensorChart').getContext('2d');
    window.myLine = new Chart(ctx, config);
}
    
function getChartData() {
    return fetch('./Index?handler=SensorReadings',
            {
                method: 'get',
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8'
                }
            })
        .then(function(response) {
            if (response.ok) {
                return response.text();
            } else {
                console.log('no response');
            }
        })
        .then(function(text) {
            try {
                return JSON.parse(text);
            } catch (err) {
                console.log(err, 'Method Not Found');
            }
        })
        .then(function(responseJson) {
            chartModel = responseJson;
            console.log(chartModel);
            showChart();
        });
}

getChartData();