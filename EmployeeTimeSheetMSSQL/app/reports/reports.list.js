var ReportModule = function () {
    var initializeDataAndChart = function (criteria, timePeriod, elementID) {
        var data = {
            Criteria: criteria,
            TimePeriod: timePeriod
        };

        $.ajax({
            url: "/Report/GetAnalyticsAsync/",
            type: "POST",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json; charset=utf-8"
        }).done(function (data, status) {
            if (data.Success) {
                var dataHorizontal = data.Data.X;
                var dataVertical = data.Data.Y;
                initializeChart(elementID, dataHorizontal, dataVertical);
            }
        }).fail(function () {
            toastr.error("There was an error getting your analytics.");
        });
        
    };

    function number_format(number, decimals, dec_point, thousands_sep) {
        // *     example: number_format(1234.56, 2, ',', ' ');
        // *     return: '1 234,56'
        number = (number + '').replace(',', '').replace(' ', '');
        var n = !isFinite(+number) ? 0 : +number,
            prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
            sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
            dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
            s = '',
            toFixedFix = function (n, prec) {
                var k = Math.pow(10, prec);
                return '' + Math.round(n * k) / k;
            };
        // Fix for IE parseFloat(0.55).toFixed(0) = 0;
        s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
        if (s[0].length > 3) {
            s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
        }
        if ((s[1] || '').length < prec) {
            s[1] = s[1] || '';
            s[1] += new Array(prec - s[1].length + 1).join('0');
        }
        return s.join(dec);
    }

    // Bar Chart Example
    var initializeChart = function (chartElementID, dataHorizontal, dataVertical) {        
        var myBarChart = new Chart(chartElementID, {
            type: 'bar',
            data: {
                labels: dataHorizontal,
                datasets: [{
                    label: "Work Item",
                    backgroundColor: "#4e73df",
                    hoverBackgroundColor: "#2e59d9",
                    borderColor: "#4e73df",
                    data: dataVertical
                }]
            },
            options: {
                maintainAspectRatio: false,
                layout: {
                    padding: {
                        left: 10,
                        right: 25,
                        top: 25,
                        bottom: 0
                    }
                },
                scales: {
                    xAxes: [{
                        time: {
                            unit: 'hour'
                        },
                        gridLines: {
                            display: false,
                            drawBorder: false
                        },
                        ticks: {
                            maxTicksLimit: 6
                        },
                        maxBarThickness: 25
                    }],
                    yAxes: [{
                        ticks: {
                            min: 0,
                            max: 100,
                            maxTicksLimit: 5,
                            padding: 10,                            
                            callback: function (value, index, values) {
                                return number_format(value) + 'h';
                            }
                        },
                        gridLines: {
                            color: "rgb(234, 236, 244)",
                            zeroLineColor: "rgb(234, 236, 244)",
                            drawBorder: false,
                            borderDash: [2],
                            zeroLineBorderDash: [2]
                        }
                    }],
                },
                legend: {
                    display: false
                },
                tooltips: {
                    titleMarginBottom: 10,
                    titleFontColor: '#6e707e',
                    titleFontSize: 14,
                    backgroundColor: "rgb(255,255,255)",
                    bodyFontColor: "#858796",
                    borderColor: '#dddfeb',
                    borderWidth: 1,
                    xPadding: 15,
                    yPadding: 15,
                    displayColors: false,
                    caretPadding: 10,
                    callbacks: {
                        label: function (tooltipItem, chart) {                            
                            return 'logged ' + number_format(tooltipItem.yLabel) + 'h';
                        }
                    }
                }
            }
        });
    };

    var registerEvents = function () {
        $("#timePeriodWorkItem").on("change", function () {
            var timePeriod = $(this).val();
            if (timePeriod !== "") {
                initializeDataAndChart("AnalyticsByWorkItem", timePeriod, "workItemChart");
            }
        });

        $("#timePeriodEmployeeWorkItem").on("change", function () {
            var timePeriod = $(this).val();
            if (timePeriod !== "") {
                initializeDataAndChart("AnalyticsByEmployeeWorkItem", timePeriod, "workItemEmployeeChart");
            }
        });
    };

    return {
        Init: function () {
            registerEvents();
            initializeDataAndChart("AnalyticsByWorkItem", "WEEK", "workItemChart");
            initializeDataAndChart("AnalyticsByEmployeeWorkItem", "WEEK", "workItemEmployeeChart");
        }
    };

}();