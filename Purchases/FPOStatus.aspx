<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="FPOStatus.aspx.cs" Inherits="VOMS_ERP.Purchases.FPOStatus" EnableEventValidation="false" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Foreign Purchase Order Status"
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
                <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                    border="0">
                    <thead>
                        <tr>
                            <th width="20%">
                                Order No.
                            </th>
                            <th width="8%">
                                Date
                            </th>
                            <th width="13%">
                                Ref. Enquiry No.
                            </th>
                            <th width="12%">
                                Subject
                            </th>
                            <th width="08%">
                                Value($)
                            </th>
                            <th width="05%">
                                Customer
                            </th>
                            <th width="15%">
                                Status
                            </th>
                            <th width="3%">
                                Amend Report
                            </th>
                            <th width="3%">
                                Cancel
                            </th>
                            <th width="3%">
                                M
                            </th>
                            <th width="3%">
                                Amend E
                            </th>
                            <th width="3%">
                                E
                            </th>
                            <th width="3%">
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
                            <th colspan="4" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Order No.
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
                                Value($)
                            </th>
                            <th>
                                Customer
                            </th>
                            <th>
                                Status
                            </th>
                            <th>
                            </th>
                            <th>
                            </th>
                            <th>
                            </th>
                            <th>
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
                <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFFENo" runat="server" Value="" />
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFFPValue" runat="server" Value="" />
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
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

        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=gvItmMstr]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_", // of _TOTAL_ records
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
                "sAjaxSource": "Purchases_WebService.asmx/GetFPOItems",
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
                                    $("#gvItmMstr").show();
                                }
                    });
                }
            });
            $("#gvItmMstr").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
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
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFENo]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFSubject]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFFPValue]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvItmMstr").dataTable();
        });

        function Reapply(valddd, CreatedBy, IsCust, IsVerbalLPO, Status) {
            try {
                if (IsCust == 1) {
                    var result = FPOStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres == 'Success') {
                        var Result = '';
                        if (Status != "100") {
                            if (confirm("Are you sure to Cancel the FPO.?")) {
                                if (Status <= "50") {
                                    Result = FPOStatus.Cancel(valddd.parentNode.parentNode.id, CreatedBy, IsCust, IsVerbalLPO);
                                    oTable.fnDraw();
                                    if (Result.value == "Success") {
                                        SuccessMessage("FPO Sucessfully Cancelled");
                                    }
                                    else
                                        ErrorMessage("Unable to cancel this record");
                                }
                                else
                                    ErrorMessage("Unable to cancel this record");
                            }
                        }
                        else
                            ErrorMessage("FPO already Cancelled");
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
                else
                    ErrorMessage('You do not have permissions for Cancilation.');
            }
            catch (e) {
                alert(e.Message);
            }
        }

        function mailsDetails(valddd, IsCust) {
            try {
                if (IsCust == 1)
                    window.location.replace("../Masters/EmailSend.aspx?FpoID=" + valddd.parentNode.parentNode.id);
                else
                    ErrorMessage('You do not have permissions to send Mail.');
            } catch (e) {
                alert(e.Message);
            }
        }

        function Ammendement(valddd, CreatedBy, IsCust, IsVerbal) {
            try {
                if (IsCust == 1) {
                    var result = FPOStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres == 'Success') {
                        if (IsVerbal == 0)
                            window.location.replace("../Purchases/NewFPOrder.aspx?ID=" + valddd.parentNode.parentNode.id + "&IsAm=True");
                        else
                            window.location.replace("../Purchases/NewFPOrderVerbal.Aspx?ID=" + valddd.parentNode.parentNode.id + "&IsAm=True");

                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
                else
                    ErrorMessage('You do not have permissions for Amendment.');
            } catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd, CreatedBy, IsCust, IsVerbal) {
            try {
                if (IsCust == 1) {
                    //if (StatusTypeId == 'FPOReceived') {
                    var result = FPOStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres == 'Success') {
                        if (IsVerbal == 0)
                            window.location.replace("../Purchases/NewFPOrder.Aspx?ID=" + valddd.parentNode.parentNode.id);
                        else
                            window.location.replace("../Purchases/NewFPOrderVerbal.Aspx?ID=" + valddd.parentNode.parentNode.id);
                    }

                    else {
                        ErrorMessage(fres);
                    }
                    //}
                    //                else {
                    //                    ErrorMessage('Unable to Edit, LPO is Released') 
                    //                }
                }
                else
                    ErrorMessage('You do not have permissions for Edit.');
            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (IsCust == 1) {
                    var result = FPOStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres == 'Success') {
                        if (confirm("Are you sure you want to Delete?")) {
                            var result = FPOStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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
                    }

                    else {
                        ErrorMessage(fres);
                    }
                }
                else
                    ErrorMessage('You do not have permissions for Delete.');
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
    <script type="text/javascript">
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
    </script>
</asp:Content>
