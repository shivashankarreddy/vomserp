<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" EnableEventValidation="false" CodeBehind="BillPaymentApprovalStatus.aspx.cs"
    Inherits="VOMS_ERP.Accounts.BillPaymentApprovalStatus" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Bill Payment Approval Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" style="margin-right: 5%;" />
                                    </td>
                                    <td align="right">
                                        <div runat="server" id="dvexport">
                                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                                class="item_top_icons" title="Export Excel" OnClick="btnExcelExpt_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table id="tblShippingBillStat" cellpadding="0" cellspacing="0" border="0" class="display">
                                <thead>
                                    <tr>
                                        <th width="15%">
                                            Approval Ref. No.
                                        </th>
                                        <th width="5%">
                                            Supplier Name
                                        </th>
                                        <th width="15%">
                                            LPO Number
                                        </th>
                                        <th width="5%">
                                            Invoice No
                                        </th>
                                        <th width="10%">
                                            Payment Type
                                        </th>
                                        <th width="5%">
                                            Payment Date
                                        </th>
                                        <th width="5%">
                                            Amount
                                        </th>
                                        <th width="5%">
                                            Cheque No
                                        </th>
                                        <th width="5%">
                                            Cheque Date
                                        </th>
                                        <th width="5%">
                                            Bank
                                        </th>
                                        <th width="5%">
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
                                        <th colspan="5" align="left">
                                        </th>
                                        <th colspan="3" align="right">
                                        </th>
                                    </tr>
                                    <tr>
                                        <th>
                                            Approval Ref. No.
                                        </th>
                                        <th>
                                            Supplier Name
                                        </th>
                                        <th>
                                            LPO Number
                                        </th>
                                        <th>
                                        </th>
                                        <th>
                                        </th>
                                        <th>
                                            Payment Date
                                        </th>
                                        <th>
                                            Amount
                                        </th>
                                        <th>
                                            Cheque No
                                        </th>
                                        <th>
                                            Cheque Date
                                        </th>
                                        <th>
                                            Bank
                                        </th>
                                        <th>
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
                            <asp:HiddenField ID="HFRefNo" runat="server" Value="" />
                            <asp:HiddenField ID="HFSuplrNm" runat="server" Value="" />
                            <asp:HiddenField ID="HFLPO" runat="server" Value="" />
                            <asp:HiddenField ID="HFInvNmbr" runat="server" Value="" />
                            <asp:HiddenField ID="HFPymntTp" runat="server" Value="" />
                            <asp:HiddenField ID="HFPymntFDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFPymntToDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFAmt" runat="server" Value="" />
                            <asp:HiddenField ID="HFChqNo" runat="server" Value="" />
                            <asp:HiddenField ID="HFChqFDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFChqTDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFBank" runat="server" Value="" />
                            <asp:HiddenField ID="HFStat" runat="server" Value="" />
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
                "sAjaxSource": "BillpmntService.asmx/GetBPAStatus",
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
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                     null, null]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;



                if (InDex == 0) {
                    $('[id$=HFRefNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFSuplrNm]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFLPO]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFInvNmbr]').val(Valuee);
                }
                 else if (InDex == 4) {
                     $('[id$=HFPymntTp]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFPymntFDt]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFPymntToDt]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFAmt]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFChqNo]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFChqFDt]').val(Valuee);

                }
                else if (InDex == 10) {
                    $('[id$=HFChqTDt]').val(Valuee);

                }
                else if (InDex == 11) {
                    $('[id$=HFBank]').val(Valuee);

                }
                else if (InDex == 12) {
                    $('[id$=HFStat]').val(Valuee);

                }

            });

            /* Init the table */
            oTable = $('#tblShippingBillStat').dataTable();
        });

        function EditDetails(valddd, CreatedBy, IsCust, Stat) {
            try {

                if (Stat == 0) {
                    ErrorMessage('Edit Not Permitted for Rejected Bill Payment');
                }
                else if (Stat == 2) {
                    ErrorMessage('Edit Not Permitted for Reverse Bill Payment/Check Lost');
                }
                else {
                    var result = BillPaymentApprovalStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust, Stat);
                    var fres = result.value;
                    if (fres == 'Success') {
                        window.location.replace("../Accounts/BillPaymentApproval.Aspx?ID=" + valddd.parentNode.parentNode.id);
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
            }

            catch (e) {
                alert(e.Message);
            }
        }


        function Delet(valddd, CreatedBy, IsCust, Stat) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = BillPaymentApprovalStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust, Stat);
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


        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

    </script>
    <%--<script type="text/javascript">
        $(document).ready(function () {
            $("[id$=gvBillPayment]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                //"sDom": 'r<"H"lf><"datatable-scroll"t><"F"ip>',
                //"sDom": 'T<"clear">lfrtip',
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search :"
                },

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
            //            oTable = $("[id$=gvRced]").dataTable();
            //        });
        });
        function SearchStatus(valu) {
            var value1 = valu.toString();
            oTable.fnFilter(value1, 5);
        }
        
    </script>--%>
</asp:Content>
