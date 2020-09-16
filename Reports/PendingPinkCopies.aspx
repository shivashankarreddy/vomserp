<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="PendingPinkCopies.aspx.cs" Inherits="VOMS_ERP.Reports.PendingPinkCopies" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" style="font-size:15.5px;font-weight:bold" Text="Status of Pending Pink Copies"
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
                <div class="aligntable" id="aligntbl" style="padding:0px 0px 0px 16px;">
                <table id="gvPendngPnkCpies" class="widthFull fontsize10 displayNone" cellpadding="0"
                    cellspacing="0" border="0">
                    <thead>
                        <tr>
                            <th width="07%">
                                FPO No.
                            </th>
                            <th width="06%">
                                CT-1 No.
                            </th>
                            <th width="10%">
                                CT-1 DATE
                            </th>
                            <th width="06">
                                CT-1 FORM ISSUED AT MUMBAI / HYDERABAD
                            </th>
                            <th width="10%">
                                ARE-1 No.
                            </th>
                             <th width="10%">
                                ARE-1DATE
                            </th>
                            <th width="06%">
                                S.B. No.
                            </th>
                            <th width="13%">
                                S.B. DATE
                            </th>
                            <th width="14%">
                                INV No.
                            </th>
                             <th width="13%">
                               CHA AGENT
                            </th>
                            <th width="08%">
                                SUPPLIER  NAME
                            </th><th width="08%">
                                AMOUNT (Rs.) 
                            </th><th width="08%">
                                 STATUS 
                            </th>
                            <th width="08%">
                                ARE-1
                            </th>
                            <th width="08%">
                                ARE-2
                            </th>
                            <th width="08%">
                                PINK
                            </th>
                            <th width="08%">
                                Ex. Inv. No.
                            </th>
                            <th width="08%">
                                Ex. Inv. DATE
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="6">
                            </th>
                            <th colspan="3" align="left">
                            </th>
                            <th colspan="3" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th width="10%">
                                FPO No.
                            </th>
                            <th width="08%">
                                CT-1 No.
                            </th>
                            <th width="15%">
                               CT-1 DATE
                            </th>
                             <th width="06">
                                CT-1 FORM ISSUED AT MUMBAI / HYDERABAD
                            </th>
                            <th width="08%">
                                ARE-1 No.
                            </th>
                            <th width="10%">
                                ARE-1 DATE
                            </th>
                            <th width="10%">
                                S.B. No.
                            </th>
                            <th width="10%">
                                S.B. DATE
                            </th>
                            <th width="15%">
                                INV No.
                            </th>
                             <th width="13%">
                               CHA AGENT
                            </th>
                            <th width="15%">
                                SUPPLIER  NAME
                            </th>
                            <th width="10%">
                               AMOUNT (Rs.) 
                            </th>
                            <th width="08%">
                                 STATUS 
                            </th>
                            <th width="08%">
                            </th>
                            <th width="08%">
                            </th>
                            <th width="08%">
                            </th>
                            <th width="08%">
                                Ex. Inv. No.
                            </th>
                            <th width="08%">
                               Ex. Inv. DATE
                            </th>
                        </tr>
                    </tfoot>
                </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFCT1No" runat="server" Value="" />
                <asp:HiddenField ID="HFct1_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFct1_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFCT1FORMISSDMUMHYD" runat="server" Value="" />
                <asp:HiddenField ID="HFARE1No" runat="server" Value="" />
                <asp:HiddenField ID="HFAre_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFAre_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFSBNo" runat="server" Value="" />
                <asp:HiddenField ID="HFSB_FromDATE" runat="server" Value="" />
                <asp:HiddenField ID="HFSB_ToDATE" runat="server" Value="" />
                <asp:HiddenField ID="HFINVNo" runat="server" Value="" />
                <asp:HiddenField ID="HFCHAAgent" runat="server" Value="" />
                <asp:HiddenField ID="HFSUPPLIERNAME" runat="server" Value="" />
                <asp:HiddenField ID="HFAMOUNT" runat="server" Value="" />
                <asp:HiddenField ID="HFSTATUS" runat="server" Value="" />
                <asp:HiddenField ID="HFARE1" runat="server" Value="" />
                <asp:HiddenField ID="HFARE2" runat="server" Value="" />
                <asp:HiddenField ID="HFPINK" runat="server" Value="" />
                <asp:HiddenField ID="HFExInvNo" runat="server" Value="" />
                <asp:HiddenField ID="HFEx_FromDATE" runat="server" Value="" />
                <asp:HiddenField ID="HFEx_ToDATE" runat="server" Value="" />
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

            oTable = $("[id$=gvPendngPnkCpies]").dataTable({
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
                "sAjaxSource": "ReportService.asmx/GetPendngPnkCopes",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                //"bDeferRender": true,

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
//                "sScrollXInner": "100%",
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
                                    $("#gvPendngPnkCpies").show();
                                }
                    });
                }
            });
            $("#gvPendngPnkCpies").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "text" },
                                    {  "type": "date-range"},
                                    { "type": "text" },
                                    {  "type": "text" },
                                    { "type": "date-range" }, 
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" }, 
                                    { "type": "text" },
                                    {"type": "text" },
                                    { "type": "text" },
                                    null,
                                    null,
                                    null,
                                    { "type": "text" }, { "type": "date-range"}]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFCT1No]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFct1_FromDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFct1_ToDate]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFCT1FORMISSDMUMHYD]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFARE1No]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFAre_FromDate]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFAre_ToDate]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFSBNo]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFSB_FromDATE]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFSB_ToDATE]').val(Valuee);
                }
                else if (InDex == 11) {
                    $('[id$=HFINVNo]').val(Valuee);
                }
                else if (InDex == 12) {
                    $('[id$=HFCHAAgent]').val(Valuee);
                }else if (InDex == 13) {
                    $('[id$=HFSUPPLIERNAME]').val(Valuee);
                }else if (InDex == 14) {
                    $('[id$=HFAMOUNT]').val(Valuee);
                }else if (InDex == 15) {
                    $('[id$=HFSTATUS]').val(Valuee);
                }else if (InDex == 16) {
                    $('[id$=HFExInvNo]').val(Valuee);
                }else if (InDex == 17) {
                    $('[id$=HFEx_FromDATE]').val(Valuee);
                }else if (InDex == 18) {
                    $('[id$=HFEx_ToDATE]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $("#gvPendngPnkCpies").dataTable();
        });
    </script>
</asp:Content>
