<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="OpeningEPCopiesReport.aspx.cs" Inherits="VOMS_ERP.Reports.OpeningEPCopiesReport" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" style="font-size:15.5px;font-weight:bold"
                                        Text="STATEMENT OF PENDING EP COPIES / SHIPPING BILLS / ARE-1 FORMS"
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
                <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                <table id="gvOpeningEpCopies" class="widthFull fontsize10 displayNone" cellpadding="0"
                    cellspacing="0" border="0">
                    <thead>
                        <tr>
                         <%--   <th width="5%">
                                Sl.No.
                            </th>--%>
                            <th width="10%">
                                COMMERCIAL INVOICE No.
                            </th>
                            <th width="10%">
                                DATE
                            </th>
                            <th width="28%">
                                PROFORMA INVOICE No.
                            </th>
                            <th width="28">
                                DATE
                            </th>
                            <th width="10%">
                                AIRWAY BILL No. / BILL OF LADING No.
                            </th>
                            <th width="03%">
                                DATE
                            </th>
                            <th width="03%">
                                SHIPPING BILL No.
                            </th>
                            <th width="03%">
                                DATE
                            </th>
                            <th width="03%">
                                SHIPPING BILL STATUS
                            </th>
                            <th width="03%">
                                No. OF PAGES
                            </th>
                            <th width="03%">
                                No. OF ARE-1 FORMS
                            </th>
                            <th width="03%">
                                ARE-1 FORM STATUS
                            </th>
                            <th width="03%">
                                LOAD PORT
                            </th>
                            <th width="03%">
                                DISCHARGE PORT
                            </th>
                            <th width="03%">
                                CHA AGENT
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="9">
                            </th>
                            <th colspan="3" align="left">
                            </th>
                            <th colspan="3" align="right">
                            </th>
                        </tr>
                        <tr>
                          <%--  <th width="1%">
                            </th>--%>
                            <th width="10%">
                                COMMERCIAL INVOICE No.
                            </th>
                            <th width="03%">
                                DATE
                            </th>
                            <th width="10%">
                                PROFORMA INVOICE No.
                            </th>
                            <th width="03%">
                                DATE
                            </th>
                            <th width="15%">
                                AIRWAY BILL No. / BILL OF LADING No.
                            </th>
                            <th width="03%">
                                DATE
                            </th>
                            <th width="05%">
                                SHIPPING BILL No.
                            </th>
                            <th width="03%">
                                DATE
                            </th>
                            <th width="05%">
                                SHIPPING BILL STATUS
                            </th>
                            <th width="05%">
                                No. OF PAGES
                            </th>
                            <th width="05%">
                                No. OF ARE-1 FORMS
                            </th>
                            <th width="05%">
                                ARE-1 FORM STATUS
                            </th>
                            <th width="05%">
                                LOAD PORT
                            </th>
                            <th width="05%">
                                DISCHARGE PORT
                            </th>
                            <th width="05%">
                                CHA AGENT
                            </th>
                        </tr>
                    </tfoot>
                </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFCOMMERCIAL_INVOICE_No" runat="server" Value="" />
                <asp:HiddenField ID="HFCOMMERCIAL_INVOICE_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFCOMMERCIAL_INVOICE_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPROFORMA_INVOICE_No" runat="server" Value="" />
                <asp:HiddenField ID="HFPROFORMA_INVOICE_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFPROFORMA_INVOICE_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFAWB_BL_No" runat="server" Value="" />
                <asp:HiddenField ID="HFAWB_BL_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFAWB_BL_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_no" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSb_Staus" runat="server" Value="" />
                <asp:HiddenField ID="HFNo_Pages" runat="server" Value="" />
                <asp:HiddenField ID="HFNo_ARE_Forms" runat="server" Value="" />
                <asp:HiddenField ID="HFARE_Form_Status" runat="server" Value="" />
                <asp:HiddenField ID="HFLoad_Port" runat="server" Value="" />
                <asp:HiddenField ID="HFDischarge_Port" runat="server" Value="" />
                <asp:HiddenField ID="HFCha_Agent" runat="server" Value="" />
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

            oTable = $("[id$=gvOpeningEpCopies]").dataTable({
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
                "sAjaxSource": "ReportService.asmx/GetOpeningEPCopies",
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
                                    $("#gvOpeningEpCopies").show();
                                }
                    });
                }
            });

            $("#gvOpeningEpCopies").dataTable().columnFilter(
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
                                    { "type": "text" },
                                    { "type": "text"}]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;
                if (InDex == 0) {
                    $('[id$=HFCOMMERCIAL_INVOICE_No]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFCOMMERCIAL_INVOICE_FromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFCOMMERCIAL_INVOICE_ToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFPROFORMA_INVOICE_No]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFPROFORMA_INVOICE_FromDate]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFPROFORMA_INVOICE_ToDate]').val(Valuee);
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
                    $('[id$=HFSb_no]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFSb_FromDate]').val(Valuee);
                }
                else if (InDex == 11) {
                    $('[id$=HFSb_ToDate]').val(Valuee);
                }
                else if (InDex == 12) {
                    $('[id$=HFSb_Staus]').val(Valuee);
                }
                else if (InDex == 13) {
                    $('[id$=HFNo_Pages]').val(Valuee);
                }
                else if (InDex == 14) {
                    $('[id$=HFNo_ARE_Forms]').val(Valuee);
                }
                else if (InDex == 15) {
                    $('[id$=HFARE_Form_Status]').val(Valuee);
                }
                else if (InDex == 16) {
                    $('[id$=HFLoad_Port]').val(Valuee);
                }
                else if (InDex == 17) {
                    $('[id$=HFDischarge_Port]').val(Valuee);
                }
                else if (InDex == 18) {
                    $('[id$=HFCha_Agent]').val(Valuee);
                }

            });

            /* Init the table */
            oTable = $("#gvOpeningEpCopies").dataTable();
        });


    </script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        function SuccessMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Success">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }
        function ErrorMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Error">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }
    </script>
</asp:Content>
