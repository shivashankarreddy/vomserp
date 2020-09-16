<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="INVsforBRCsPending.aspx.cs" Inherits="VOMS_ERP.Reports.INVsforBRCsPending" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" style="font-size:15.5px;font-weight:bold"
                                        Text="LIST OF INVOICES FOR WHICH BRC's ARE PENDING - AS ON Dt."
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
                <div class="aligntable" id="aligntbl" style="padding:0px 0px 0px 16px;">
                <table id="gvINVsforBRCsPending" class="widthFull fontsize10 displayNone" cellpadding="0"
                    cellspacing="0" border="0">
                    <thead>
                        <tr>
                            <th width="07%">
                                CInvoice No.
                            </th>
                            <th width="06%">
                                CInvoice Date
                            </th>
                             <th width="07%">
                                Packing List No.
                            </th>
                            <th width="06%">
                                Packing List Date
                            </th>
                            <th width="10%">
                                Airway Bill No. / Bill of Lading No.
                            </th>
                            <th width="03%">
                                Airway Bill / Bill of Lading Date
                            </th>
                            <th width="10%">
                                S.Bill No.
                            </th>
                            <th width="06">
                                S.Bill Date
                            </th>
                            <th width="10%">
                                Port
                            </th>
                             <th width="10%">
                                CIF Amount (USD)
                            </th>
                            <th width="06%">
                                Freight (USD)
                            </th>
                            <th width="13%">
                                FOB Vaule (USD)
                            </th>
                            <th width="14%">
                                Name of The Party
                            </th>
                             <th width="13%">
                               To Port
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="5">
                            </th>
                            <th colspan="3" align="left">
                            </th>
                            <th colspan="3" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th width="10%">
                                Invoice No.
                            </th>
                            <th width="08%">
                                Invoice Date
                            </th>
                             <th width="07%">
                                Packing List No.
                            </th>
                            <th width="06%">
                                Packing List Date
                            </th>
                            <th width="10%">
                                Airway Bill No. / Bill of Lading No.
                            </th>
                            <th width="03%">
                                Airway Bill / Bill of Lading Date
                            </th>
                            <th width="15%">
                                S.Bill No.
                            </th>
                            <th width="08%">
                                S.Bill Date
                            </th>
                            <th width="10%">
                                Port
                            </th>
                            <th width="10%">
                                CIF Amount (USD)
                            </th>
                            <th width="10%">
                                Freight (USD)
                            </th>
                            <th width="15%">
                                FOB Value (USD)
                            </th>
                            <th width="15%">
                                Name of The Party
                            </th>
                            <th width="10%">
                               To Port
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
                <asp:HiddenField ID="HFPROFORMA_INVOICE_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPROFORMA_INVOICE_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPACKING_No" runat="server" Value="" />
                <asp:HiddenField ID="HFPKNG_LIST_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPKNG_LIST_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFAWB_BL_No" runat="server" Value="" />
                <asp:HiddenField ID="HFAWB_BL_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFAWB_BL_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSBILL_No" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPORT" runat="server" Value="" />
                <asp:HiddenField ID="HFCIF_AMOUNT" runat="server" Value="" />
                <asp:HiddenField ID="HFFRIEGHT" runat="server" Value="" />
                <asp:HiddenField ID="HFFOB_VALUE" runat="server" Value="" />
                <asp:HiddenField ID="HFNAME_OF_THE_PARTY" runat="server" Value="" />
                <asp:HiddenField ID="HFTO_PORT" runat="server" Value="" />
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
            $(".aligntable").width($(window).width() - 88 + "px");
        });
        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=gvINVsforBRCsPending]").dataTable({
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
                "sAjaxSource": "ReportService.asmx/INVsforBRCsPending",
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
                                    $("#gvINVsforBRCsPending").show();
                                }
                    });
                }
            });
            $("#gvINVsforBRCsPending").dataTable().columnFilter(
                {
                    "aoColumns": [{ "type": "text" },
                                    { "type": "date-range" },
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
                                    { "type": "text"}]
                }); //

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFINVOICE_No]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFPROFORMA_INVOICE_FromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFPROFORMA_INVOICE_ToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFPACKING_No]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFPKNG_LIST_FromDate]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFPKNG_LIST_ToDate]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFAWB_BL_No]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFAWB_BL_FromDate]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFAWB_BL_ToDate]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFSBILL_No]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFSb_FromDate]').val(Valuee);
                }
                else if (InDex == 11) {
                    $('[id$=HFSb_ToDate]').val(Valuee);
                }
                else if (InDex == 12) {
                    $('[id$=HFPORT]').val(Valuee);
                }
                else if (InDex == 13) {
                    $('[id$=HFCIF_AMOUNT]').val(Valuee);
                }
                else if (InDex == 14) {
                    $('[id$=HFFRIEGHT]').val(Valuee);
                }
                else if (InDex == 15) {
                    $('[id$=HFFOB_VALUE]').val(Valuee);
                }
                else if (InDex == 16) {
                    $('[id$=HFNAME_OF_THE_PARTY]').val(Valuee);
                }
                else if (InDex == 17) {
                    $('[id$=HFTO_PORT]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $("#gvINVsforBRCsPending").dataTable();
        });
    </script>
</asp:Content>
