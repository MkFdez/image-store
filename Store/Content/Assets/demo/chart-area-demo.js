loadAreaChart()
function loadAreaChart() {
    // Set new default font family and font color to mimic Bootstrap's default styling
    
    let loadingContainer = document.createElement("div")
    loadingContainer.className = "loader"
    loadingContainer.id = "area-loading"
    let loading = document.createElement("div")
    loading.className = "justify-content-center jimu-primary-loading"
    loadingContainer.appendChild(loading)
    let container = document.getElementById("area-chart")
    container.appendChild(loadingContainer)
    $.ajax({
        type: "POST",
        url: "/Sales/GetDailySales",
        dataType: "json",
        data: { month: -1, year: -1 },
        success: function (result) {
            renderChart(result)
        }
    })
    function renderChart(data) {
        $("#myAreaChart").remove()
        $("#area-chart").append(`<canvas id="myAreaChart" width="100 % " height="40"></canvas>`)
        let parsedData = JSON.parse(data)
        Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
        Chart.defaults.global.defaultFontColor = '#292b2c';
        document.getElementById("DailySalesCount").innerHTML = "    "+parsedData.values.length
        document.getElementById("DailySalesAmount").innerHTML = "    $"+parsedData.total

        // Area Chart Example
        var ctx = document.getElementById("myAreaChart");
        var myLineChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: parsedData.dates,
                datasets: [{
                    label: "Sales",
                    lineTension: 0.3,
                    backgroundColor: "rgba(2,117,216,0.2)",
                    borderColor: "rgba(2,117,216,1)",
                    pointRadius: 5,
                    pointBackgroundColor: "rgba(2,117,216,1)",
                    pointBorderColor: "rgba(255,255,255,0.8)",
                    pointHoverRadius: 5,
                    pointHoverBackgroundColor: "rgba(2,117,216,1)",
                    pointHitRadius: 50,
                    pointBorderWidth: 2,
                    data: parsedData.values,
                }],
            },
            options: {
                scales: {
                    xAxes: [{
                        time: {
                            unit: 'date'
                        },
                        gridLines: {
                            display: false
                        },
                        ticks: {
                            maxTicksLimit: 7
                        }
                    }],
                    yAxes: [{
                        ticks: {
                            min: 0,
                            max: Math.max(...parsedData.values) == 0 ? 1 : Math.ceil(Math.max(...parsedData.values)),
                            maxTicksLimit: 5
                        },
                        gridLines: {
                            color: "rgba(0, 0, 0, .125)",
                        }
                    }],
                },
                legend: {
                    display: false
                }
            }
        });
        let chart = document.getElementById("area-chart")
        chart.removeChild(document.getElementById("area-loading"))

    }

    var dpm = $("#datepickermonth").datepicker({
        format: "mm-yyyy",
        viewMode: "months",
        minViewMode: "months",
        autoclose: true //to close picker once year is selected
    });

    dpm.on('changeMonth', function (e) {
        container.appendChild(loadingContainer)
        var selectedDate = e.date;
        $.ajax({
            type: "POST",
            url: "/Sales/GetDailySales",
            dataType: "json",
            data: { month: selectedDate.getMonth() + 1, year: selectedDate.getFullYear() },
            success: function (result) {
                renderChart(result)
            }
        })
    });
}