var DB = angular.module("DB", ['chart.js']);
var S_Res;
 

angular.element(document).ready(function () {
//    angular.bootstrap(document, ['DB']);

});

DB.controller("C_PDR", function ($scope, $http) {

    $http({
        method: 'POST',
        url: "Dashboard.aspx/GetChartData",
        datatype: "JSON",
        headers:
        {
            "content-Type": "application/json"
        },
        data: {}
    }).then(function (Success) {

        if (typeof (angular.fromJson(angular.fromJson(Success.data.d))) == "object") {
            S_Res = angular.fromJson(Success.data.d);
            $scope.RenderCharts();
        }
        else {
            ErrorMessage(angular.fromJson(Success.data.d));
        }

    }, function (Error) {
        console.log('Error::' + Error.data.d);
    });

     

    $scope.RenderCharts = function () {

        var DrillDownChart_Monthly = Highcharts.chart('DrillDown_BarChart',
        {
            lang:
            {
                drillUpText: '◁ Back'
            },
            chart:
            {
                type: 'column',
                events:
                {
                    drilldown: function (e) {
                        var T = ''
                    }
                },
                 height: 340
            },

            title:
            {
                //                align: 'left',
                text: 'Status Reports From '+ GetDateFormat(new Date(),-7)+ ' To '+ GetDateFormat(new Date(),0)
            },
            subtitle:
            {
                text: ''
            },
            xAxis:
            {
                //categories: S_Res.Yearly_Barchart_Axis_PD,
                crosshair: true,
                type: 'category',
                scrollbar: {
                    enabled: true
                },
                min: 0, 
            },
            yAxis:
            {
                min: 0,
               title: {
                    text: 'Count',
                    align: 'low'
                }
            },
            tooltip:
            {
                headerFormat: '<span style="font-size:10px">{point.key}</span>',
                pointFormat: '<p><span style="color:{series.color};padding:0">{series.name}: </span>' + '<span style="padding:0"><b>{point.y:.1f}</b></span></p>',
                footerFormat: '',
                shared: true,
                useHTML: true
            },
            plotOptions: {
            series: {
            allowPointSelect: true,
            dataLabels:
                    {
                        enabled: true
                    },
                    cursor: 'pointer'
                    ,point: {
                    events: {
                        click: function () {
                        location.href =this.options.Link;
                        }
                    }
                    }
                },
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                },
                column:
                {
                    pointPadding: 0.2,
                    borderWidth: 0
                } 
            },
             
            series: S_Res.Yearly_Barchart,
            drilldown:
            {
                series: S_Res.DrillDown_Data
            }
        });
         
    }

});

 function GetDateFormat(d,s) { 
      d.setDate(d.getDate()+s);
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join('-');
}
