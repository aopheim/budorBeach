/////////
var tempValues = [];
var humidityValues = [];
var pressureValues = [];
var timeStamps = [];
var chartModel;

function generateGlobalConfig(data, title){
    return globalConfig = {
        type: 'line',
        data: data,
        options: {
            elements: {
                point: {
                    hitRadius: 10,
                    hoverRadius: 10,
                    radius: 0
                }
            },
            hoverMode: 'index',
            legend: {
                display: false
            },
            maintainAspectRatio: true,
            responsive: true,
            scales: {
                xAxes: [
                    {
                        gridLines: {
                            display: false
                        },
                        ticks: {
                            callback: function(label, index, labels){
                                var unixTime = Date.parse(label);
                                var date = new Date(unixTime);

                                var hours = date.getHours();
                                hours = ("0" + hours).slice(-2);

                                var minutes = date.getMinutes();
                                minutes = ("0" + minutes).slice(-2);


                                return hours + ':' + minutes;
                            }
                        }
                    }],
                yAxes: [
                    {
                        gridLines: {
                            display: true,
                            borderDash: [10, 10]
                        }
                    }]
            },
            stacked: false,
            title: {
                display: true,
                text: title
            },
            tooltips: {
                callbacks: {
                    title: function(tooltipItems, data) {
                        var currentTitle = tooltipItems[0].xLabel;
                        var unixTimeInUtc = Date.parse(currentTitle);
                        return new Date(unixTimeInUtc).toLocaleString("no-NO");
                    }
                }
            }
        }
    };
}

function showChart() {
    tempValues = chartModel.temperatureReadings;
    humidityValues = chartModel.humidityReadings;
    pressureValues = chartModel.pressureReadings;
    timeStamps = chartModel.measuredAt;

    var tempData = {
        labels: timeStamps,
        datasets: [
            {
                fill: false,
                data: tempValues,
                label: 'Temperatur',
                borderColor: "#3e95cd",
            }
        ]
    };

    var humidityData = {
        labels: timeStamps,
        datasets: [
            {
               data: humidityValues,
               label: 'Relativ luftfuktighet (%)',
               borderColor: "#8e5ea2",
               fill: false
            },
        ]
    };

    var pressureData = {
        labels: timeStamps,
        datasets: [
            {
               data: pressureValues,
               label: 'Trykk (hPa)',
               borderColor: "#3cba9f",
               fill: false
            }
        ]
    };

    var tempCtx = document.getElementById('tempChart').getContext('2d');
    var humidityCtx = document.getElementById('humidityChart').getContext('2d');
    var pressureCtx = document.getElementById('pressureChart').getContext('2d');

    if (window.tempLine || window.humLine || window.pressureLine) {
        window.tempLine.destroy();
        window.humLine.destroy();
        window.pressureLine.destroy();
    }

    window.tempLine = new Chart(tempCtx, generateGlobalConfig(tempData, 'Temperatur [°C]'));
    window.humLine = new Chart(humidityCtx, generateGlobalConfig(humidityData, 'Relativ luftfuktighet [%]'));
    window.pressureLine = new Chart(pressureCtx, generateGlobalConfig(pressureData, 'Trykk [hPa]'));
}
    
function getChartData(hours) {
    return fetch('./Index?handler=SensorReadings&cutOffHours=' + hours,
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
            showChart();
        });
}

$("#chartDropDown").change(function() {
    var dropDown = document.getElementById("chartDropDown");
    var hours = dropDown.options[dropDown.selectedIndex].value;
    getChartData(hours);
});

getChartData(12);