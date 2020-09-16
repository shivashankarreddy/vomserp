<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="VOMS_ERP.Masters.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="../JScript/Angularjs/angular.js" type="text/javascript"> </script>
    <script src="../JScript/Angularjs/chart.js" type="text/javascript"></script>
    <script src="../JScript/Angularjs/angular-chart.js" type="text/javascript"></script>
    <script src="../JScript/Angularjs/angular-chart.min.js" type="text/javascript"></script>
    <script src="../JScript/Master/Dashboard.js" type="text/javascript"></script>
    <script src="../JScript/Highcharts/highcharts.js" type="text/javascript"></script>
    <script src="../JScript/Highcharts/drilldown.js" type="text/javascript"></script>
    <%--<script src="../JScript/Highcharts/heatmap.js" type="text/javascript"></script>
    <script src="../JScript/Highcharts/exporting.js" type="text/javascript"></script>--%>
    <%--
<div ng-app="DB"  ng-controller="DashBoard" style="width:500px" >
  <canvas class="chart chart-bar" chart-data="data" chart-labels="labels" 
    chart-colors="colors" chart-series="series"
    chart-options="options"  ></canvas> 
</div>--%>
    <table class="MainTable" align="center">
        <tr class="bcTRTitleRow">
            <td class="bcTdTitleLeft" align="left">
                <table width="100%">
                    <tr>
                        <td>
                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" CssClass="bcTdTitleLabel"></asp:Label><div
                                id="div1" runat="server" align="center" class="formError1" />
                        </td>
                        <td colspan="4" style="text-align: left;" align="left">
                            <span id="Span1" class="bcLabelcenter">
                                <asp:Label runat="server" ID="Label2"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%-- <tr>
            <td class="bcTdnormal">
                <div id="full_dashbord" class="full_dashbord" style="margin-top: 10px; margin-bottom: 20px;">


                </div>
            </td>
        </tr>--%>
        <tr>
            <td colspan="2" align="center">
                <%--<asp:Label ID="lblHead" runat="server" Text="Johnnie Walker" CssClass="bcTdTitleLabel"></asp:Label>--%>
            </td>
        </tr>
        <tr width="100%">
            <td>
                <div ng-app="DB" ng-controller="C_PDR" style="width: 100%">
                    <table width="100%">
                        <tr>
                            <td>
                                <div id="DrillDown_BarChart" class="scroll" style="width: 100%; overflow: auto">
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
