<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="eBRCStatus.aspx.cs" Inherits="VOMS_ERP.Invoices.eBRCStatus" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="E-BRC Status" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <div runat="server" id="dvexport">
                                <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                    class="item_top_icons" style="width:15px !important;height:15px !important" title="Export Excel" OnClick="btnExcelExpt_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
                <table id="tblE_BRC" cellpadding="0" cellspacing="0" border="0" class="display">
                    <thead>
                        <tr>
                            <th width="10%">
                                Shipping Bill No.
                            </th>
                            <th width="5%">
                                Date
                            </th>
                            <th width="10%">
                                Shipping Bill Port
                            </th>
                            <th width="05%">
                                BRC No
                            </th>
                            <th width="10%">
                                BRC Status
                            </th>
                            <th width="10%">
                                BRC Utilisation Status
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
                            <th style="text-align: right" colspan="4">
                            </th>
                            <th colspan="2" align="left">
                            </th>
                            <th colspan="2" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Shipping Bill No.
                            </th>
                            <th>
                                Date
                            </th>
                            <th>
                                Shipping Bill Port
                            </th>
                            <th>
                                BRC No
                            </th>
                            <th>
                                BRC Status
                            </th>
                            <th>
                                BRC Utilisation Status
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
                <asp:HiddenField ID="HFShppngBllNo" runat="server" Value="" />
                <asp:HiddenField ID="HFFrmDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFShppngBillPort" runat="server" Value="" />
                <asp:HiddenField ID="HFBRCNo" runat="server" Value="" />
                <asp:HiddenField ID="HFBrcStat" runat="server" Value="" />
                <asp:HiddenField ID="HFBrcUtil" runat="server" Value="" />

                <%--<asp:HiddenField ID="HFBRCStatus" runat="server" Value="" />
                <asp:HiddenField ID="HFBRCUtlstionStats" runat="server" Value="" />--%>
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
        var oTable;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            /*          Main Functionality       */
            oTable = $('#tblE_BRC').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "EBRCWS.asmx/GetEBRCStat",
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
                                    $("#tblE_BRC").show();
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

            $("#tblE_BRC").dataTable().columnFilter(
                {
                    "aoColumns": [{ "type": "text" },
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
                    $('[id$=HFShppngBllNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFrmDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFShppngBillPort]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFBRCNo]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFBrcStat]').val(Valuee);
                } 
                else if (InDex == 6) {
                    $('[id$=HFBrcUtil]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#tblE_BRC').dataTable();
        });


        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = eBRCStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Invoices/e-BRC Details.Aspx?ID=" + valddd.parentNode.parentNode.id);
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = eBRCStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres.contains('Success::')) {
                        oTable.fnDraw();
                        SuccessMessage(fres.replace('Success::', ''));
                    }
                    else if (fres.contains('Error::')) {
                        ErrorMessage(fres.replace('Error::', ''));
                        //ErrorMessage('Cannot Delete this Record, LE already created so delete LE/ Error while Deleting ' + valddd.parentNode.parentNode.id + '.');
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
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtFrmDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtToDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });


        /// not using
        function Myvalidations() {
            var res = $('[id$=txtCustomerNm]').val();
            var res1 = $('[id$=txtFrmDt]').val();
            if (res.trim() == '' && res1.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer Name Or Dates are Required.</span>');
                $('[id$=txtCustomerNm]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }
    </script>
    <script type="text/javascript">
        $("[id$=gvBRCDtls]").dataTable({
            "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            "iDisplayLength": 10,
            "aaSorting": [[0, "asc"]],
            "bJQueryUI": true,
            "bAutoWidth": false,
            "bProcessing": true,
            "sPaginationType": "full_numbers",
            "oLanguage": {
                "sZeroRecords": "There are no Records that match your search criteria",
                "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                "sInfoEmpty": "Showing 0 to 0 of 0 records",
                "sInfoFiltered": "(filtered from _MAX_ total records)",
                "sSearch": "Search :"
            },
            "sScrollY": "250px",
            "sScrollX": "100%",
            "sScrollXInner": "100%",
            "bScrollCollapse": true
        });

        function SearchStatus(valu) {
            var value1 = valu.toString();
            oTable.fnFilter(value1, 5);
        }
        
    </script>
</asp:Content>
