<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="StmtOfCargoSales_CommercialINVPrepared.aspx.cs"
    Inherits="VOMS_ERP.Reports.StmtOfCargoSales_CommercialINVPrepared" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" style="font-size:15.5px;font-weight:bold" Text="STATEMENT OF CARGO - SALES / COMMERCIAL INVOICE PREPARED"
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
                <table id="gvDutyDBAMTRPT" class="widthFull fontsize10 displayNone" cellpadding="0"
                    cellspacing="0" border="0">
                    <thead>
                        <tr>
                          
                            <th width="08%">
                                Export Invoice No.
                            </th>
                            <th width="05%">
                                Export Invoice Date
                            </th>
                            <th width="04%">
                                Mode of Shipment
                            </th>
                            <th width="08%">
                                FOB Value
                            </th>
                            <th width="08">
                                Freight
                            </th>
                            <th width="08%">
                                CIF Value
                            </th>
                            <th width="08%">
                                POL
                            </th>
                            <th width="08%">
                                POD
                            </th>
                            <th width="05%">
                                No. of Pkgs
                            </th>
                            <th width="06%">
                                Gross Weight
                            </th>
                            <th width="06%">
                                Net Weight
                            </th>
                            <th width="10%">
                                SB No.
                            </th>
                            <th width ="06%">
                                SB Date
                            </th>
                            <th width = "08%">
                                Container Nos.
                            </th>
                            <th width = "12%">
                                BL / AWB No. Date
                            </th>
                            <th width = "12%">
                                Commercial Invoice No.
                            </th>
                            <th width = "06%">
                                Commercial Invoice Date.
                            </th>                            
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="9">
                            </th>
                            <th colspan="5" align="left">
                            </th>
                            <th colspan="3" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Export Invoice No.
                            </th>
                            <th>
                                Export Invoice Date
                            </th>
                            <th>
                                Mode of Shipment
                            </th>
                            <th>
                                FOB Value
                            </th>
                            <th>
                                Freight
                            </th>
                            <th>
                                CIF Value
                            </th>
                            <th>
                                POL 
                            </th>
                            <th>
                                POD
                            </th>
                            <th>
                                No. of Pkgs
                            </th>
                            <th>
                                Gross Weight
                            </th>
                            <th>
                                Net Weight
                            </th>
                            <th>
                               SB No. 
                            </th>
                            <th>
                                SB Date
                            </th>
                            <th>
                                Container Nos.
                            </th>
                            <th>
                            </th>
                            <th>
                             Commercial Invoice No.
                            </th>
                            <th>
                            Commercial Invoice Date.
                            </th>
                        </tr>
                    </tfoot>
                </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFExpInvNo" runat="server" Value="" />
                <asp:HiddenField ID="HFExp_INVOICE_FromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFExp_INVOICE_ToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFModeofshp" runat="server" Value="" />
                <asp:HiddenField ID="HFFOBVal" runat="server" Value="" />
                <asp:HiddenField ID="HFFreigh" runat="server" Value="" />
                <asp:HiddenField ID="HFCIFVal" runat="server" Value="" />
                <asp:HiddenField ID="HFPOL" runat="server" Value="" />
                <asp:HiddenField ID="HFPOD" runat="server" Value="" />
                <asp:HiddenField ID="HFNoPkgs" runat="server" Value="" />
                <asp:HiddenField ID="HFGrsWght" runat="server" Value="" />
                <asp:HiddenField ID="HFNetWght" runat="server" Value="" />
                <asp:HiddenField ID="HFSbno" runat="server" Value="" />
                <asp:HiddenField ID="HFSB_FrmDt" runat="server" Value="" />
                <asp:HiddenField ID="HFSB_ToDt" runat="server" Value="" />
                <asp:HiddenField ID="HFContNo" runat="server" Value="" />
                <asp:HiddenField ID="HFCommInvNo" runat="server" Value="" />
                <asp:HiddenField ID="HFComInv_FrmDt" runat="server" Value="" />
                <asp:HiddenField ID="HFComInv_ToDt" runat="server" Value="" />
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
                "sAjaxSource": "ReportService.asmx/CARGO_SALES_COMMERCIAL_INVOICE_PREPARED",
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

            $('.dataTables_filter input').unbind('keypress keyup').bind('keypress keyup', function (e) {
                $('[id$=HFRawSearch]').val($(this).val());
                oTable.fnFilter($(this).val());
            });

                        $("#gvDutyDBAMTRPT").dataTable().columnFilter(
                            {
                                "aoColumns": [
                                                { "type": "text" },
                                                { "type": "date-range" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "text" },
                                                { "type": "date-range" },
                                                { "type": "text" },
                                                null,
                                                { "type": "text" },
                                                { "type": "date-range" }
                                                ]
                            });

                        $("tfoot input").change(function (i) {
                            var InDex = $("tfoot input").index(this);
                            var Valuee = this.value;

                            if (InDex == 0) {
                                $('[id$=HFExpInvNo]').val(Valuee);
                            }
                            else if (InDex == 1) {
                                $('[id$=HFExp_INVOICE_FromDate]').val(Valuee);
                            }
                            else if (InDex == 2) {
                                $('[id$=HFPROFORMA_INVOICE_ToDate]').val(Valuee);
                            }
                            else if (InDex == 3) {
                                $('[id$=HFModeofshp]').val(Valuee);
                            }
                            else if (InDex == 4) {
                                $('[id$=HFFOBVal]').val(Valuee);
                            }
                            else if (InDex == 5) {
                                $('[id$=HFFreigh]').val(Valuee);
                            }
                            else if (InDex == 6) {
                                $('[id$=HFCIFVal]').val(Valuee);
                            }
                            else if (InDex == 7) {
                                $('[id$=HFPOL]').val(Valuee);
                            }
                            else if (InDex == 8) {
                                $('[id$=HFPOD]').val(Valuee);
                            }
                            else if (InDex == 9) {
                                $('[id$=HFNoPkgs]').val(Valuee);
                            }
                            else if (InDex == 10) {
                                $('[id$=HFGrsWght]').val(Valuee);
                            }
                            else if (InDex == 11) {
                                $('[id$=HFNetWght]').val(Valuee);
                            }
                            else if (InDex == 12) {
                                $('[id$=HFSbno]').val(Valuee);
                            }
                             else if (InDex == 13) {
                                $('[id$=HFSB_FrmDt]').val(Valuee);
                            }
                             else if (InDex == 14) {
                                $('[id$=HFSB_ToDt]').val(Valuee);
                            }
                             else if (InDex == 15) {
                                $('[id$=HFContNo]').val(Valuee);
                            }
                             else if (InDex == 16) {
                                $('[id$=HFCommInvNo]').val(Valuee);
                            }
                             else if (InDex == 17) {
                                $('[id$=HFComInv_FrmDt]').val(Valuee);
                            }
                             else if (InDex == 18) {
                                $('[id$=HFComInv_ToDt]').val(Valuee);
                            }
                        });
            /* Init the table */
            oTable = $("#gvDutyDBAMTRPT").dataTable();
        });
    </script>
</asp:Content>
