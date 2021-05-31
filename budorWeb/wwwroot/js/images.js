function getImagesForDay(dateAsString) {
    return fetch('./Index?handler=ImagesForDay&selectedDateAsString=' + dateAsString,
        {
            method: 'get',
            headers: {
                'Content-Type': 'application/json;charset=UTF-8'
            }
        });
}

$("#dateSelector").change(function () {
    var dropDown = document.getElementById("dateSelector");
    var dateAsString = dropDown.options[dropDown.selectedIndex].value;
    getImagesForDay(dateAsString);
});

getChartData(12);