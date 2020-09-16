<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="LEStatus.aspx.cs" Inherits="VOMS_ERP.Enquiries.LEStatus" EnableEventValidation="false"
     %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Local Enquiry Status"
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
                  <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                    border="0">
                    <thead>
                        <tr>
                            <th width="10%">
                                Enquiry No.
                            </th>
                            <th width="5%">
                                Date
                            </th>
                            <th width="10%">
                                Ref. No.
                            </th>
                            <th width="28%">
                                Subject
                            </th>
                            <th width="10%">
                                Supplier
                            </th>
                            <th width="05%">
                                Customer
                            </th>
                            <th width="20%">
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
                            <th colspan="3" align="left">
                            </th>
                            <th colspan="2" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Enqury No.
                            </th>
                            <th>
                                Date
                            </th>
                            <th>
                                Ref. No.
                            </th>
                            <th>
                                Subject
                            </th>
                            <th>
                                Supplier
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
                        </tr>
                    </tfoot>
                </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFLENo" runat="server" Value="" />
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFFENo" runat="server" Value="" />
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFSupplier" runat="server" Value="" />
                <asp:HiddenField ID="HFCustomer" runat="server" Value="" />
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
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
        .dataTables_scroll
        {
            overflow-x: auto;
    overflow-y: visible;
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

            oTable = $("[id$=gvItmMstr]").dataTable({
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
                "sAjaxSource": "WebService1.asmx/GetLItems",
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
                "sScrollXInner": "110%",
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
                    $('[id$=HFLENo]').val(Valuee);
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
                    $('[id$=HFSupplier]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFCustomer]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFStatus]').val(Valuee);
                }

            });

            /* Init the table */
            oTable = $("#gvItmMstr").dataTable();
        });


        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = LEStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres.contains('Success::')) {
                        oTable.fnDraw();
                        SuccessMessage(fres.replace('Success::', ''));
                    }
                    else if (fres.contains('Error::')) {
                        ErrorMessage(fres.replace('Error::', ''));
                        //ErrorMessage('Cannot Delete this Record, LQ already created so delete LQ.' + valddd.parentNode.parentNode.id + '.');
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
            } catch (e) {
                alert(e.Message);
            }
        }

        function mailsDetails(valddd) {
            try {

                window.location.replace("../Masters/EmailSend.aspx?LeID=" + valddd.parentNode.parentNode.id);
            } catch (e) {
                alert(e.Message);
            }
        }
        function EditDetails(valddd) {
            try {

                window.location.replace("../Enquiries/FloatEnquiry.Aspx?ID=" + valddd.parentNode.parentNode.id);
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
