<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CommercialInvoiceStatus.aspx.cs" Inherits="VOMS_ERP.Invoices.CommercialInvoiceStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Commercial Invoice Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td align="right">
                                        <%-- <asp:ImageButton ID="btnWordExpt" runat="server" ImageUrl="../images/word.png" class="item_top_icons"
                                                title="Export Word" OnClick="btnWordExpt_Click" Visible="false" />
                                            <asp:ImageButton ID="btnPdfExpt" runat="server" ImageUrl="../images/pdf.png" class="item_top_icons"
                                                title="Export PDF" OnClick="btnPdfExpt_Click" Visible="false"></asp:ImageButton>--%>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <div runat="server" id="dvexport">
                                <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                    class="item_top_icons" title="Export Excel" OnClick="btnExcelExpt_Click" Style="width: 16px;
                                    height: 14px;" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table id="gvCommercialInvoice" class="widthFull fontsize10 displayNone" cellpadding="0"
                                cellspacing="0" border="0">
                                <thead>
                                    <tr>
                                        <th width="20%">
                                            Commercial Invoice No.
                                        </th>
                                        <th width="8%">
                                            C.Invoice Date
                                        </th>
                                        <th width="13%">
                                            P.Invoice No.
                                        </th>
                                        <th width="12%">
                                            P.Invoice Date
                                        </th>
                                        <th width="08%">
                                            Vessel No.
                                        </th>
                                        <th width="3%">
                                            SB No.
                                        </th>
                                        <th width="3%">
                                            BL No.
                                        </th>
                                        <th width="7%">
                                            E
                                        </th>
                                        <th width="7%">
                                            D
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
                                            Commercial Invoice No.
                                        </th>
                                        <th>
                                            C.Invoice Date
                                        </th>
                                        <th>
                                            P.Invoice No.
                                        </th>
                                        <th>
                                            P.Invoice Date
                                        </th>
                                        <th>
                                            Vessel No.
                                        </th>
                                        <th>
                                            SB No.
                                        </th>
                                        <th>
                                            BL No.
                                        </th>
                                        <th>
                                        </th>
                                        <th>
                                        </th>
                                    </tr>
                                </tfoot>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:HiddenField ID="HFCINNo" runat="server" Value="" />
                            <asp:HiddenField ID="HFCFrmDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFCToDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFPinNo" runat="server" Value="" />
                            <asp:HiddenField ID="HFPFrDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFPToDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFVessNo" runat="server" Value="" />
                            <asp:HiddenField ID="HFSbNo" runat="server" Value="" />
                            <asp:HiddenField ID="HFBLNo" runat="server" Value="" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <%--    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>--%>
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

        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=gvCommercialInvoice]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                "aaSorting": [],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "CommercialInvoiceService.asmx/GetCINVStatus",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
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
                                    $("#gvCommercialInvoice").show();
                                }
                    });
                }
            });
            $("#gvCommercialInvoice").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" }

                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFCINNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFCFrmDt]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFCToDt]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFPinNo]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFPFrDt]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFPToDt]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFVessNo]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFSbNo]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFBLNo]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvCommercialInvoice").dataTable();
        });

        function EditDetails(valddd, CreatedBy, IsCust, PinvID) {
            try {
                var result = CommercialInvoiceStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Invoices/CommercialInvoice.aspx?ID=" + PinvID);  //valddd.parentNode.parentNode.id);
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust, PinvID) {

            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = CommercialInvoiceStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust, PinvID);
                    var fres = result.value;
                    if (fres == 'Success') {
                        oTable.fnDraw();
                        SuccessMessage("Deleted Successfully");
                    }
                    else if (fres == 'Error') {
                        ErrorMessage("Cannot Delete this Record, Another Transaction is using this record");
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
            } catch (e) {
                alert(e.Message);
            }
        }

        //        $(window).load(function () {
        //            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        //        });

    </script>
</asp:Content>
