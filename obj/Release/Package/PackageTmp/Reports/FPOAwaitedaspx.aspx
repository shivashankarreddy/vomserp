<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="FPOAwaitedaspx.aspx.cs" Inherits="VOMS_ERP.Reports.FPOAwaitedaspx" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Style="font-size: 15.5px;
                                            font-weight: bold" Text="Details Of FPO's Awaited" CssClass="bcTdTitleLabel"></asp:Label><div
                                                id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:ImageButton ID="btnexcel" runat="server" ImageUrl="../images/EXCEL.png" title="Export Excel"
                                OnClick="btnExcelExpt_Click"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="bcTdNewTable">
                <table id="FPOAwaited" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                    border="0">
                    <thead>
                        <tr>
                            <th id="ForeignQuotationId" runat="server" visible="false">
                            </th>
                            <th width="12%">
                                Enquiry No.
                            </th>
                            <th width="12%">
                                Enquiry Date
                            </th>
                            <th width="18%">
                                Quotation No.
                            </th>
                            <th width="12%">
                                Foreign Quotation Date
                            </th>
                            <th width="08%">
                                Total Amount($)
                            </th>
                            <th width="05%">
                                Customer
                            </th>
                            <th>
                                Options
                            </th>
                            <th>
                                Remarks
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="5">
                            </th>
                            <th colspan="4" align="left">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Enquiry No.
                            </th>
                            <th>
                               Enquiry Date
                            </th>
                            <th>
                                Quotation No.
                            </th>
                            <th>
                                Date
                            </th>
                            <th>
                                Total Amount
                            </th>
                            <th>
                                customer
                            </th>
                            <th>
                                Options
                            </th>
                            <th>
                                Remarks
                            </th>
                        </tr>
                    </tfoot>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFFENO" runat="server" Value="" />
                <asp:HiddenField ID="HFFEFrmDt" runat="server" Value="" />
                <asp:HiddenField ID="HFFEToDt" runat="server" Value="" />
                <asp:HiddenField ID="HFFQNO" runat="server" Value="" />
                <asp:HiddenField ID="HFFQFrmDt" runat="server" Value="" />
                <asp:HiddenField ID="HFFQToDt" runat="server" Value="" />
                <asp:HiddenField ID="HFFQAmount" runat="server" Value="" />
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFStat" runat="server" Value="" />
                <asp:HiddenField ID="HFRemarks" runat="server" Value="" />
            </td>
        </tr>
    </table>
    <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery.ui.theme.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery-ui.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/jquery.dataTables_themeroller.css" rel="stylesheet"
        type="text/css" />
    <script src="../JScript/Scripts/js/jquery.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/ZeroClipboard.js" type="text/javascript"></script>
    <script src="../JScript/media/js/TableTools.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script src="../JScript/jquery.jeditable.js" type="text/javascript"></script>
    <script src="../JScript/jquery.dataTables.editable.js" type="text/javascript"></script>
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
            /*          Main Functionality       */
            oTable = $('#FPOAwaited').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "ReportService.asmx/GetFPOAwaited",
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
                                    $("#FPOAwaited").show();
                                }
                    });
                },
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,

                //--- Dynamic Language---------
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search all columns:"
                },

                "oSearch": {
                    "sSearch": "",
                    "bRegex": false,
                    "bSmart": true
                }
            }).makeEditable({
                sUpdateURL: "FPOAwaitingHandler.ashx",
                "aoColumns": [null, null, null, null, null, null,
                            {
                                type: 'select',
                                indicator: 'Saving...',
                                loadtext: 'loading...',
                                data: "{'Select':'Select','PLACEDTOOTHERS':'PLACEDTOOTHERS', 'CANCEL':'CANCEL'}",                                
                                onblur: 'submit'
                            },
                            {
                                indicator: 'Saving...',
                                tooltip: 'DOUBLE CLICK ON THE RECORD TO EDIT AND CLICK ENTER TO SAVE',
                                loadtext: 'loading...',
                                type: 'textarea',
                                onblur: 'submit'
                            }]
            });

            $("#FPOAwaited").dataTable().columnFilter(
                {
                    //sPlaceHolder: "foot:before",
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" }
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;
                
                if (InDex == 0) {
                    $('[id$=HFFENO]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFEFrmDt]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFFEToDt]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFQNO]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFFQFrmDt]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFFQToDt]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFFQAmount]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFStat]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFRemarks]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#FPOAwaited').dataTable();
        });
    </script>
</asp:Content>
