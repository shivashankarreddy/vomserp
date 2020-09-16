<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true" CodeBehind="FQStatusCustomer.aspx.cs" Inherits="VOMS_ERP.Customer_Access.FQStatusCustomer" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Foreign Quotation Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../images/EXCEL.png"
                                title="Export Excel" OnClick="btnExcelExpt_Click"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="bcTdNewTable">
                <table id="FQStatus" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                    border="0">
                    <thead>
                        <tr>
                            <th id="ForeignQuotationId" runat="server" visible="false">
                            </th>
                            <th width="20%">
                                Quotation No.
                            </th>
                            <th width="08%">
                                Date
                            </th>
                            <th width="08%">
                                Ref. Enquiry No.
                            </th>
                            <th width="18%">
                                Subject
                            </th>
                            <th width="02%">
                                Vendor/Supplier
                            </th>
                            <th width="15%">
                                Status
                            </th>
                            <th width="03%">
                                M
                            </th>
                            <th width="03%">
                                E
                            </th>
                            <th width="03%">
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
                            <th colspan="2" align="left">
                            </th>
                            <th colspan="4" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Quotation Number
                            </th>
                            <th>
                                Date
                            </th>
                            <th>
                                Ref. Enquiry No.
                            </th>
                            <th>
                                Subject
                            </th>
                            <th>
                                Customer
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
                <asp:HiddenField ID="HFFQNo" runat="server" Value="" />
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFRefFENO" runat="server" Value="" />
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
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

            oTable = $('#FQStatus').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "PI_Webservice.asmx/GetFQItems",
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
                                    $("#FQStatus").show();
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

            $("#FQStatus").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                      null, null, null
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFFQNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFToDate]').val(Valuee);
                }

                else if (InDex == 3) {
                    $('[id$=HFRefFENO]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFSubject]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#FQStatus').dataTable();
        });

        function mailsDetails(valddd, IsCust) {
            try {
                if (IsCust == 0)
                    window.location.replace("../Masters/EmailSend.aspx?CFqID=" + valddd.parentNode.parentNode.id);
                else
                    ErrorMessage('You do not have permissions to send Mail.');
            } catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd, CreatedBy, IsCust, Status) {
            try {

                if (Status == 60) {
                    ErrorMessage("Cannot Edit this Record LPO is Released")
                }
                else {
                    if (IsCust == 0) {
                        var result = FQStatusCustomer.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust, Status);
                        var fres = result.value;
                        if (fres == 'Success') {
                            window.location.replace("../Customer Access/NewFQ_floatedPI.aspx?ID=" + valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                        }
                        else {
                            ErrorMessage(fres);
                        }
                    }
                    else
                        ErrorMessage('You do not have permissions to Edit.');
                }
            } catch (e) {
                alert(e.Message);
            }

        }

        function Delet(valddd, CreatedBy, IsUser) {
            try {
                if (IsUser == 0) {
                    if (confirm("Are you sure you want to Delete?")) {
                        var result = FQStatusCustomer.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsUser);
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
                }
                else
                    ErrorMessage('You do not have permissions to Delete.');
            } catch (e) {
                alert(e.Message);
            }
        }
    </script>
    <script type="text/javascript">
        var dateToday = new Date();
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
