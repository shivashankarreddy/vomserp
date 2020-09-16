<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="DutyDrawBackAmountReport.aspx.cs" Inherits="VOMS_ERP.Reports.DutyDrawBackAmountReport" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" style="font-size:15.5px;font-weight:bold" Text="DUTY DRAWBACK AMOUNT"
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
                                            title="Export Excel" OnClick="btnExcelExpt_Click1"></asp:ImageButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div class="aligntable" id="aligntbl">
                <table id="gvDutyDBAMTRPT" class="widthFull fontsize10 displayNone" cellpadding="0"
                    cellspacing="0" border="0">
                    <thead>
                        <tr>
                            <%--<th width="02%">
                                Sl.No.
                            </th>--%>
                            <th width="07%">
                                COMMERCIAL INVOICE No.
                            </th>
                            <th width="06%">
                                DATE
                            </th>
                            <th width="07%">
                                PROFORMA INVOICE No.
                            </th>
                            <th width="06%">
                                DATE
                            </th>
                            <th width="10%">
                                S.BILL No.
                            </th>
                            <th width="06">
                                DATE
                            </th>
                            <th width="10%">
                                LOAD PORT
                            </th>
                             <th width="10%">
                                DISCHARGE PORT
                            </th>
                            <th width="06%">
                                CHA AGENT
                            </th>
                            <th width="13%">
                                Duty Draw Back Amount as per S.B.
                            </th>
                            <th width="14%">
                                Duty Draw Back Amount received
                            </th>
                             <th width="13%">
                               REMARKS AS PER ICEGATE / CUSTOM'S QUERY 
                            </th>
                            <th width="08%">
                                ACTION TAKEN / TO BE TAKEN
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="7">
                            </th>
                            <th colspan="5" align="left">
                            </th>
                            <th colspan="3" align="right">
                            </th>
                        </tr>
                        <tr>
                           <%-- <th width="1%">
                            </th>--%>
                            <th width="07%">
                                COMMERCIAL INVOICE No.
                            </th>
                            <th width="10%">
                                DATE
                            </th>
                            <th width="10%">
                                PROFORMA INVOICE No.
                            </th>
                            <th width="10%">
                                DATE
                            </th>
                            <th width="15%">
                                S.BILL No.
                            </th>
                            <th width="10%">
                                DATE
                            </th>
                            <th width="10%">
                                LOAD PORT
                            </th>
                            <th width="10%">
                                DISCHARGE PORT
                            </th>
                            <th width="10%">
                                CHA AGENT
                            </th>
                            <th width="15%">
                                Duty Draw Back Amount as per S.B.
                            </th>
                            <th width="15%">
                                Duty Draw Back Amount received
                            </th>
                            <th width="10%">
                               REMARKS AS PER ICEGATE / CUSTOM'S QUERY 
                            </th>
                            <th width="08%">
                                ACTION TAKEN / TO BE TAKEN
                            </th>
                        </tr>
                    </tfoot>
                </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFCOMMERCIAL_INVOICENO" runat="server" Value="" />
                <asp:HiddenField ID="HFCOMMERCIAL_InvFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFCOMMERCIAL_InvToDate" runat="server" Value="" />

                <asp:HiddenField ID="HFINVOICE_No" runat="server" Value="" />
                <asp:HiddenField ID="HFPROFORMA_INVOICE_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPROFORMA_INVOICE_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSBILL_No" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPORTOF_SHIPMENT" runat="server" Value="" />
                <asp:HiddenField ID="HFPORTOF_DISCH" runat="server" Value="" />
                <asp:HiddenField ID="HFCHA_AGENT" runat="server" Value="" />
                <asp:HiddenField ID="HFDUTY_DB_AMT_SB" runat="server" Value="" />
                <asp:HiddenField ID="HFDUTY_DB_AMTRECVD" runat="server" Value="" />
                <asp:HiddenField ID="HFREMARKS" runat="server" Value="" />
                <asp:HiddenField ID="HFACTION_DETAILS" runat="server" Value="" />
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

         oTable = $("[id$=gvDutyDBAMTRPT]").dataTable({
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
             "sAjaxSource": "ReportService.asmx/GetDutyDBAMT",
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
                                    $("#gvDutyDBAMTRPT").show();
                                }
                 });
             }
         });
         $("#gvDutyDBAMTRPT").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text"}]
                });

         $("tfoot input").change(function (i) {
             var InDex = $("tfoot input").index(this);
             var Valuee = this.value;

            if (InDex == 0) {
                $('[id$=HFCOMMERCIAL_INVOICENO]').val(Valuee);
             }
             else if (InDex == 1) {
                 $('[id$=HFCOMMERCIAL_InvFromDate]').val(Valuee);
             }
             else if (InDex == 2) {
                 $('[id$=HFCOMMERCIAL_InvToDate]').val(Valuee);
             }
             else if (InDex == 3) {
                 $('[id$=HFINVOICE_No]').val(Valuee);
             }
             else if (InDex == 4) {
                 $('[id$=HFPROFORMA_INVOICE_FromDate]').val(Valuee);
             }
             else if (InDex == 5) {
                 $('[id$=HFPROFORMA_INVOICE_ToDate]').val(Valuee);
             }
             else if (InDex == 6) {
                 $('[id$=HFSBILL_No]').val(Valuee);
             }
             else if (InDex == 7) {
                 $('[id$=HFSb_FromDate]').val(Valuee);
             }
             else if (InDex == 8) {
                 $('[id$=HFSb_ToDate]').val(Valuee);
             }
             else if (InDex == 9) {
                 $('[id$=HFPORTOF_SHIPMENT]').val(Valuee);
             }
             else if (InDex == 10) {
                 $('[id$=HFPORTOF_DISCH]').val(Valuee);
             }
             else if (InDex == 11) {
                 $('[id$=HFCHA_AGENT]').val(Valuee);
             }
             else if (InDex == 12) {
                 $('[id$=HFDUTY_DB_AMT_SB]').val(Valuee);
             }
             else if (InDex == 13) {
                 $('[id$=HFDUTY_DB_AMTRECVD]').val(Valuee);
             }
             else if (InDex == 14) {
                 $('[id$=HFREMARKS]').val(Valuee);
             }
             else if (InDex == 15) {
                 $('[id$=HFACTION_DETAILS]').val(Valuee);
             }
         });

         /* Init the table */
         oTable = $("#gvDutyDBAMTRPT").dataTable();
     });
    </script>
</asp:Content>
