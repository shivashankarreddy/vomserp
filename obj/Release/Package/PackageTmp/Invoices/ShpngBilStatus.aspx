<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ShpngBilStatus.aspx.cs" Inherits="VOMS_ERP.Invoices.ShpngBilStatus" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Shipping Bill Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <div runat="server" id="dvexport">
                                <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                    class="item_top_icons" title="Export Excel" Height="15" Width="16" OnClick="btnExcelExpt_Click" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table id="tblShippingBillStat" cellpadding="0" cellspacing="0" border="0" class="display">
                                <thead>
                                    <tr>
                                        <th width="15%">
                                            Shipping Bill No.
                                        </th>
                                        <th width="5%">
                                            Shipping Bill Date
                                        </th>
                                        <th width="15%">
                                            P Invoice Number
                                        </th>
                                        <th width="5%">
                                            P Invoice Date
                                        </th>
                                        <th width="10%">
                                            Port of Loading
                                        </th>
                                        <th width="5%">
                                            Port of Discharge
                                        </th>
                                        <th width="5%">
                                            Country of Destination
                                        </th>
                                        <th width="5%">
                                            Country of Origin
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
                                        <th colspan="2" align="left">
                                        </th>
                                        <th colspan="3" align="right">
                                        </th>
                                    </tr>
                                    <tr>
                                        <th>
                                            Shipping Bill No.
                                        </th>
                                        <th>
                                            Shipping Bill Date
                                        </th>
                                        <th>
                                            P Invoice Number
                                        </th>
                                        <th>
                                            P Invoice Date
                                        </th>
                                        <th>
                                            Port of Loading
                                        </th>
                                        <th>
                                            Port of Discharge
                                        </th>
                                        <th>
                                            Country of Destination
                                        </th>
                                        <th>
                                            Country of Origin
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
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:HiddenField ID="HFShippingBillNo" runat="server" Value="" />
                            <asp:HiddenField ID="HFShippingBillFromDate" runat="server" Value="" />
                            <asp:HiddenField ID="HFShippingBillToDate" runat="server" Value="" />
                            <asp:HiddenField ID="HFPrfmaInvoiceNmbr" runat="server" Value="" />
                            <asp:HiddenField ID="HFPrfmaInvoiceFromDate" runat="server" Value="" />
                            <asp:HiddenField ID="HFPrfmaInvoiceToDate" runat="server" Value="" />
                            <asp:HiddenField ID="HFPrtLoading" runat="server" Value="" />
                            <asp:HiddenField ID="HFPrtDischarge" runat="server" Value="" />
                            <asp:HiddenField ID="HFCntryDestination" runat="server" Value="" />
                            <asp:HiddenField ID="HFCntryOrigine" runat="server" Value="" />
                        </td>
                    </tr>
                </table>
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
            $(".aligntable").width($(window).width() - 114 + "px");
        });


        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        //        $(document).ready(function () {
        //            var dateToday = new Date();
        //            $('[id$=txtFrmDt]').datepicker({
        //                dateFormat: 'dd-mm-yy',
        //                changeMonth: true,
        //                changeYear: true,
        //                maxDate: dateToday
        //            });
        //            $('[id$=txtToDt]').datepicker({
        //                dateFormat: 'dd-mm-yy',
        //                changeMonth: true,
        //                changeYear: true,
        //                maxDate: dateToday
        //            });
        //        });

        //        $("[id$=GvShpngBil]").dataTable({
        //            "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
        //            "iDisplayLength": 10,
        //            "aaSorting": [[0, "desc"]],
        //            "bJQueryUI": true,
        //            "bAutoWidth": false,
        //            "bProcessing": true,
        //            "sPaginationType": "full_numbers",

        //            "oLanguage": {
        //                "sZeroRecords": "There are no Records that match your search criteria",
        //                "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
        //                "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
        //                "sInfoEmpty": "Showing 0 to 0 of 0 records",
        //                "sInfoFiltered": "(filtered from _MAX_ total records)",
        //                "sSearch": "Search :"
        //            },

        //            //Scrolling--------------
        //            "sScrollY": "250px",
        //            "sScrollX": "100%",
        //            //"sScrollXInner": "100%",
        //            "bScrollCollapse": true
        //        });
        //        function SearchStatus(valu) {
        //            var value1 = valu.toString();
        //            oTable.fnFilter(value1, 5);
        //        }
        
    </script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            /*          Main Functionality       */
            oTable = $('#tblShippingBillStat').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "ShpngBillStatWebService.asmx/GetShippingBillStat",
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
                                    $("#tblOscarNominees").show();
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
            });

            $("#tblShippingBillStat").dataTable().columnFilter(
                {
                    "aoColumns": [{ "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    null, null]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFShippingBillNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFShippingBillFromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFShippingBillToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFPrfmaInvoiceNmbr]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFPrfmaInvoiceFromDate]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFPrfmaInvoiceToDate]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFPrtLoading]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFPrtDischarge]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFCntryDestination]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFCntryOrigine]').val(Valuee);

                }
            });

            /* Init the table */
            oTable = $('#tblShippingBillStat').dataTable();
        });

        function EditDetails(valddd, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID, CreatedBy, IsCust) {
            try {
                var result = ShpngBilStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    var encrypt = ShpngBilStatus.Encrypt(valddd.parentNode.parentNode.id, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID);
                    window.location.replace("../Invoices/ShpngBilDtls.aspx?RefIDs=" + encrypt.value);
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = ShpngBilStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID, CreatedBy, IsCust);
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
