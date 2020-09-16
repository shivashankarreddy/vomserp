<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Ready_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched.aspx.cs"
    Inherits="VOMS_ERP.Reports.Ready_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" style="font-size:13.3px;font-weight:bold"
                                        Text="STATEMENT OF CARGO - READY/PENDING/CUSTOMS CLEARANCE/CONTAINERS STUFFING/YET TO BE DISPATCHED - AS ON Dt."
                                            CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="right">
                                        <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                            title="Export Excel" OnClick="btnExcelExpt_Click"></asp:ImageButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div class="aligntable" id="aligntbl">
                    <table id="gvReady_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched" class="widthFull fontsize10 displayNone"
                        cellpadding="0" cellspacing="0" border="0">
                        <thead>
                            <tr>
                               <%-- <th width="02%">
                                    Sl.No.
                                </th>--%>
                                <th width="10%">
                                    Proforma Invoice No.
                                </th>
                                <th width="10%">
                                    Date
                                </th>
                                <th width="06%">
                                    Type of Consignment
                                </th>
                                <th width="07%">
                                    Amount in USD FOB
                                </th>
                                <th width="06%">
                                    Freight
                                </th>
                                <th width="10%">
                                    Cost and Freight
                                </th>
                                <th width="05%">
                                    POL
                                </th>
                                <th width="05%">
                                    POD
                                </th>
                                <th width="06">
                                    No of Pkgs
                                </th>
                                <th width="10%">
                                    Gross Weight in KGS
                                </th>
                                <th width="10%">
                                    Net Weight in KGS
                                </th>
                                <th width="10%">
                                    Shipping bill No
                                </th>
                                <th width="10%">
                                    Date
                                </th>
                                <th width="10%">
                                    Container No
                                </th>
                                <th width="10%">
                                    B/L No or AWB No
                                </th>
                                <th width="10%">
                                    Date
                                </th>
                                <th width="10%">
                                    Contact Person
                                </th>
                                <th width="15%">
                                    Remarks
                                </th>
                                <th width="10%">
                                    VESSEL details
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                        <tfoot>
                            <tr>
                                <th style="text-align: right" colspan="8">
                                </th>
                                <th colspan="5" align="left">
                                </th>
                                <th colspan="5" align="right">
                                </th>
                            </tr>
                            <tr>
                                <%--<th width="1%">
                                </th>--%>
                                <th width="10%">
                                    Proforma Invoice No.
                                </th>
                                <th width="06%">
                                    Date
                                </th>
                                <th width="08%">
                                    Type of Consignment
                                </th>
                                <th width="07%">
                                    Amount in USD FOB
                                </th>
                                <th width="06%">
                                    Freight
                                </th>
                                <th width="10%">
                                    Cost and Freight
                                </th>
                                <th width="03%">
                                    POL
                                </th>
                                <th width="15%">
                                    POD
                                </th>
                                <th width="08%">
                                    No of Pkgs
                                </th>
                                <th width="10%">
                                    Gross Weight in KGS
                                </th>
                                <th width="10%">
                                    Net Weight in KGS
                                </th>
                                <th width="10%">
                                    Shipping bill No
                                </th>
                                <th width="06%">
                                    Date
                                </th>
                                <th width="15%">
                                    Container No
                                </th>
                                <th width="15%">
                                    B/L No or AWB No
                                </th>
                                <th width="06%">
                                    Date
                                </th>
                                <th width="10%">
                                    Contact Person
                                </th>
                                <th width="10%">
                                    Remarks
                                </th>
                                <th width="10%">
                                    VESSEL details
                                </th>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFINVOICE_No" runat="server" Value="" />
                <asp:HiddenField ID="HFINVOICE_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFINVOICE_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFType_of_Consignment" runat="server" Value="" />
                <asp:HiddenField ID="HFAmount_in_USD_FOB" runat="server" Value="" />
                <asp:HiddenField ID="HFFreight" runat="server" Value="" />
                <asp:HiddenField ID="HFCost_and_Freight" runat="server" Value="" />
                <asp:HiddenField ID="HFPOL" runat="server" Value="" />
                <asp:HiddenField ID="HFPOD" runat="server" Value="" />
                <asp:HiddenField ID="HFNo_of_Pkgs" runat="server" Value="" />
                <asp:HiddenField ID="HFGross_Weight" runat="server" Value="" />
                <asp:HiddenField ID="HFNet_Weight" runat="server" Value="" />
                <asp:HiddenField ID="HFShpngBl_No" runat="server" Value="" />
                <asp:HiddenField ID="HFShpngBl_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFShpngBl_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFContainer_No" runat="server" Value="" />
                <asp:HiddenField ID="HFBL_No_AWB_No" runat="server" Value="" />
                <asp:HiddenField ID="HFBL_No_AWB_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFBL_No_AWB_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFContact_Person" runat="server" Value="" />
                <asp:HiddenField ID="HFRemarks" runat="server" Value="" />
                <asp:HiddenField ID="HFVESSEL_Details" runat="server" Value="" />
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery.ui.theme.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery-ui.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/jquery.dataTables_themeroller.css" rel="stylesheet"
        type="text/css" />
    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <style type="text/css">
        .ui-datepicker-calendar tr, .ui-datepicker-calendar td, .ui-datepicker-calendar td a, .ui-datepicker-calendar th
        {
            font-size: inherit;
        }
        div.ui-datepicker
        {
            font-size: 12px;
        }
        .ui-datepicker-title span
        {
            font-size: 12px;
        }
        
        .my-style-class input[type=text]
        {
            color: green;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 84 + "px");
        });


        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=gvReady_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                "iDisplayLength": 100,
                "aaSorting": [],
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "ReportService.asmx/Ready_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                //"bDeferRender": true,

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                //"sScrollXInner": "100%",
                "bScrollCollapse": true,

                "fnServerData": function (sSource, aoData, fnCallback) {
                    $.ajax({
                        "dataType": 'json',
                        "contentType": "application/json; charset=utf-8",
                        "type": "GET",
                        "url": sSource,
                        "data": aoData,
                        "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#gvReady_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched").show();
                                }
                    });
                }
            });
            $("#gvReady_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched").dataTable().columnFilter(
                {
                    "aoColumns": [{ "type": "text" }, { "type": "date-range" },
                                    { "type": "text" }, { "type": "text" }, 
                                    { "type": "text" }, { "type": "text" },
                                    { "type": "text" }, { "type": "text" },
                                    { "type": "text" }, { "type": "text" },
                                    { "type": "text" }, { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" }, { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" }, { "type": "text" },
                                    { "type": "text"}]
                }); //

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFINVOICE_No]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFINVOICE_FromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFINVOICE_ToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFType_of_Consignment]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFAmount_in_USD_FOB]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFFreight]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFCost_and_Freight]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFPOL]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFPOD]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFNo_of_Pkgs]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFGross_Weight]').val(Valuee);
                }
                else if (InDex == 11) {
                    $('[id$=HFNet_Weight]').val(Valuee);
                }
                else if (InDex == 12) { ///////////
                    $('[id$=HFShpngBl_No]').val(Valuee);
                }
                else if (InDex == 13) {
                    $('[id$=HFShpngBl_FromDate]').val(Valuee);
                }
                else if (InDex == 14) {
                    $('[id$=HFShpngBl_ToDate]').val(Valuee);
                }
                else if (InDex == 15) {
                    $('[id$=HFContainer_No]').val(Valuee);
                }
                else if (InDex == 16) {
                    $('[id$=HFBL_No_AWB_No]').val(Valuee);
                }
                else if (InDex == 17) {
                    $('[id$=HFBL_No_AWB_FromDate]').val(Valuee);
                }
                else if (InDex == 18) {
                    $('[id$=HFBL_No_AWB_ToDate]').val(Valuee);
                }
                else if (InDex == 19) {
                    $('[id$=HFContact_Person]').val(Valuee);
                }
                else if (InDex == 20) {
                    $('[id$=HFRemarks]').val(Valuee);
                }
                else if (InDex == 21) {
                    $('[id$=HFVESSEL_Details]').val(Valuee);
                }

            });

            /* Init the table */
            oTable = $("#gvReady_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched").dataTable();
        });
    </script>
</asp:Content>
