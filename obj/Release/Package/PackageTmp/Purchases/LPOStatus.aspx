<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="LPOStatus.aspx.cs" Inherits="VOMS_ERP.Purchases.LPOStatus" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Local Purchase Order Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="bcTdNewTable">
                <table width="98%">
                    <tr>
                        <td align="right">
                            <asp:ImageButton ID="btnExcelExpt" OnClick="btnExcelExpt_Click" runat="server" ImageUrl="../images/EXCEL.png"
                                title="Export Excel"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
                <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                    <table id="gvLpoStatus" class="widthFull fontsize10 displayNone" cellpadding="0"
                        cellspacing="0" border="0">
                        <thead>
                            <tr>
                                <th width="30%">
                                    Order No.
                                </th>
                                <th width="08%">
                                    Date
                                </th>
                                <th width="15%">
                                    Ref. FPO No.
                                </th>
                                <th width="08%">
                                    Subject
                                </th>
                                <th width="08%">
                                    Supplier
                                </th>
                                <th width="08%">
                                    Status
                                </th>
                                <th width="05%">
                                    Drawing
                                </th>
                                <th width="05%">
                                    Inspection
                                </th>
                                <th width="05%">
                                    Amd
                                </th>
                                <th width="03%">
                                    Cancel
                                </th>
                                <th width="03%">
                                    M
                                </th>
                                <th width="03%">
                                    Amd
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
                                <th colspan="5" align="left">
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
                                    Ref. FPO No.
                                </th>
                                <th>
                                    Subject
                                </th>
                                <th>
                                    Supplier
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
                                <th>
                                </th>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFLPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFSupplier" runat="server" Value="" />
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
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
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

            oTable = $("[id$=gvLpoStatus]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_", //of _TOTAL_ records
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
                "sAjaxSource": "Purchases_WebService.asmx/GetLPOItems",

                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
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
                                    $("#gvLpoStatus").show();
                                }
                    });
                }
            });
            $("#gvLpoStatus").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                      null, null, null, null, null, null
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFLPONo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFSubject]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFSupplier]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvLpoStatus").dataTable();
        });


        function mailsDetails(valddd) {
            try {

                window.location.replace("../Masters/EmailSend.aspx?LpoID=" + valddd.parentNode.parentNode.id);
            } catch (e) {
                alert(e.Message);
            }
        }

        function Reapply(valddd, CreatedBy, IsCust, IsVerbalLPO, Status) {
            try {
                var Result = '';
                if (Status != "101") {
                    if (confirm("Are you sure to Cancel the LPO.?")) {
                        if (Status <= "60") {
                            Result = LPOStatus.Cancel(valddd.parentNode.parentNode.id, CreatedBy, IsCust, IsVerbalLPO);
                            oTable.fnDraw();
                            if (Result.value == "Success") {
                                SuccessMessage("LPO Sucessfully Cancelled");
                            }
                            else
                                ErrorMessage("Unable to cancel this reocrd");
                        }
                        else
                            ErrorMessage("Unable to cancel this reocrd");
                    }
                }
                else
                    ErrorMessage("LPO already Cancelled");
            }
            catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd, CreatedBy, IsCust, IsVerbal, Status) {
            try {
                var result = LPOStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    if (IsVerbal == false) {
                        if (Status >= 70) {
                            ErrorMessage("Unable to Edit GRN Processed for this reocrd");
                        }
                        else
                            window.location.replace("../Purchases/NewLPOrder.Aspx?ID=" + valddd.parentNode.parentNode.id);
                    }
                    else {
                        window.location.replace("../Purchases/NewLPOrderVerbal.Aspx?ID=" + valddd.parentNode.parentNode.id);
                    }
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        function Ammendement(valddd, CreatedBy, IsCust, IsVerbal) {
            try {
                var result = LPOStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    if (IsVerbal == false)
                        window.location.replace("../Purchases/NewLPOrder.Aspx?ID=" + valddd.parentNode.parentNode.id + "&IsAm=True");
                    else {
                        ErrorMessage('Ammendement is not implemented for verbal LPO');
                    }
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
                    var result = LPOStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres.includes('Success::')) {
                        oTable.fnDraw();
                        SuccessMessage(fres.replace('Success::', ''));
                    }
                    else if (fres.includes('Error::')) {
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

        $(window).load(function () {
            $("#clickExcel").click(function () {
                var gvLpoStatus = $('#gvLpoStatus').html();
                window.open('data:application/vnd.ms-excel,' + $('#gvLpoStatus').html());
            });
        });

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });

    </script>
    <script type="text/javascript">

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtfromdt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txttodt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
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
