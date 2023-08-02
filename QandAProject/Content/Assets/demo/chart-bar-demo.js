// Set new default font family and font color to mimic Bootstrap's default styling
let loadingContainer2 = document.createElement("div")
loadingContainer2.className = "loader"
loadingContainer2.id = "bar-loading"
let loading2 = document.createElement("div")
loading2.className = "justify-content-center jimu-primary-loading"
loadingContainer2.appendChild(loading2)
let container2 = document.getElementById("bar-chart")
container2.appendChild(loadingContainer2)
$.ajax({
    url: "/Sales/GetMonthySales",
    dataType: "json",
    data: {year:-1},
    method: "POST",
    success: function (result) {
        generateChart(result)
    }
})

function generateChart(data) {
    $("#myBarChart").remove()
    $("#bar-chart").append(`<canvas id="myBarChart" width="100%" height="40"></canvas>`)
    let parsedData = JSON.parse(data)
    Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
    Chart.defaults.global.defaultFontColor = '#292b2c';

    // Bar Chart Example
    var ctx = document.getElementById("myBarChart");
    var myLineChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: parsedData.months,
            datasets: [{
                label: "Revenue",
                backgroundColor: "rgba(2,117,216,1)",
                borderColor: "rgba(2,117,216,1)",
                data: parsedData.values,
            }],
        },
        options: {
            scales: {
                xAxes: [{
                    time: {
                        unit: 'month'
                    },
                    gridLines: {
                        display: false
                    },
                    ticks: {
                        maxTicksLimit: 6
                    }
                }],
                yAxes: [{
                    ticks: {
                        min: 0,
                        max: Math.ceil(Math.max(...parsedData.values)),
                        maxTicksLimit: 5
                    },
                    gridLines: {
                        display: true
                    }
                }],
            },
            legend: {
                display: false
            }
        }
    });
    let chart2 = document.getElementById("bar-chart")
    chart2.removeChild(document.getElementById("bar-loading"))
}
var dp = $("#datepicker").datepicker({
    format: "yyyy",
    viewMode: "years",
    minViewMode: "years",
    autoclose: true //to close picker once year is selected
});

dp.on('changeYear', function (e) {
    container2.appendChild(loadingContainer2)
    var selectedDate = e.date;
    $.ajax({
        url: "/Sales/GetMonthySales",
        dataType: "json",
        data: { year: selectedDate.getFullYear()},
        method: "POST",
        success: function (result) {
            generateChart(result)
        }
    })
});