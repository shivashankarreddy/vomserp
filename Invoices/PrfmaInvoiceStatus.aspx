<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="PrfmaInvoiceStatus.aspx.cs" EnableEventValidation="false" Inherits="VOMS_ERP.Invoices.PrfmaInvoiceStatus" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Proforma Invoice Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" style="margin-right: 25%;" />
                                    </td>
                                    <td align="right">
                                        <div runat="server" id="dvexport">
                                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                                Visible="false" class="item_top_icons" title="Export Word" OnClick="btnExcelExpt_Click" />
                                        </div>
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
            <div class="aligntable" id="aligntbl" style="margin-right: 10px !important;">
                <table id="Pinvstatus" cellpadding="0" cellspacing="0" border="0" class="display">
                    <thead>
                        <tr>
                            <th width="5%">
                                Shipment Invoice No.
                            </th>
                            <th width="5%">
                                P. Invoice No.
                            </th>
                            <th width="5%">
                                P. Invoice Date
                            </th>
                            <th width="25%">
                                Terms of Delivery & Payments
                            </th>
                            <th width="10%">
                                Customer Name
                            </th>
                            <th width="3%">
                                Ref. FPOs
                            </th>
                            <th width="3%">
                                Shipment Planning No.
                            </th>
                            <th width="10%">
                                Status
                            </th>
                            <th width="3%">
                                E
                            </th>
                            <th width="3%">
                                D
                            </th>
                        </tr>
                    </thead>
                    <tbody class="tbody">
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="5">
                            </th>
                            <th colspan="3" align="left">
                            </th>
                            <th colspan="2" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Shipment Invoice No.
                            </th>
                            <th>
                                P. Invoice No.
                            </th>
                            <th>
                                P. Invoice Date
                            </th>
                            <th>
                                Terms of Delivery & Payments
                            </th>
                            <th>
                                Customer Name
                            </th>
                            <th>
                                Ref. FPOs
                            </th>
                            <th>
                                Shipment Planning No.
                            </th>
                            <th>
                                Status
                            </th>
                            <th>
                                &nbsp;
                            </th>
                            <th>
                                &nbsp;
                            </th>
                        </tr>
                    </tfoot>
                </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFShpINVNo" runat="server" Value="" />
                <asp:HiddenField ID="HFPInvNo" runat="server" Value="" />
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFTrmsDelPaymnt" runat="server" Value="" />
                <asp:HiddenField ID="HFCustNm" runat="server" Value="" />
                <asp:HiddenField ID="HFFPO" runat="server" Value="" />
                <asp:HiddenField ID="HFShpPlngNo" runat="server" Value="" />
                <asp:HiddenField ID="HFStat" runat="server" Value="" />
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
            $(".aligntable").width($(window).width() - 50 + "px");
        });

        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=Pinvstatus]").dataTable({
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
                "sAjaxSource": "InvoicesWebService1.asmx/GetPInvStatus",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                //"bDeferRender": true,

                "fnInitComplete": function (oSettings, json) {
                    for (var i = 0, iLen = oSettings.aoData.length; i < iLen; i++) {
                        oSettings.aoData[i].nTr.className += " " + oSettings.aoData[i]._aData[0];
                    }
                },


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
                                    $("#Pinvstatus").show();
                                }
                    });
                }
            });

            $("#Pinvstatus").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                     { "type": "text" },
                                      null, null
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFShpINVNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFPInvNo]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFFromDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFTrmsDelPaymnt]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFCustNm]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFFPO]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFShpPlngNo]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFStat]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#Pinvstatus').dataTable();
        });

        /// not using
        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = PrfmaInvoiceStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Invoices/PrfmaInvoice.aspx?ID=" + valddd.parentNode.parentNode.id);
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust, CompanyId) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = PrfmaInvoiceStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust, CompanyId);
                    var fres = result.value;
                    if (fres.contains('Success::')) {
                        oTable.fnDraw();
                        SuccessMessage(fres.replace('Success::', ''));
                    }
                    else if (fres.contains('Error::')) {
                        ErrorMessage(fres.replace('Error::', ''));
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
            } catch (e) {
                alert(e.Message);
            }
        }
    </script>
    <script type="text/jscript">
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
