<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="RqstCEDtlsStatus.aspx.cs" Inherits="VOMS_ERP.Logistics.RqstCEDtlsStatus"
     %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Request for Proforma Invoice Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" align="right">
                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                title="Export Excel" OnClick="btnExcelExpt_Click"></asp:ImageButton>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                                border="0">
                                <thead>
                                    <tr>
                                        <th width="5%">
                                            Customer
                                        </th>
                                        <th width="15%">
                                            FPO Number(s)
                                        </th>
                                        <th width="23%">
                                            LPO Number(s)
                                        </th>
                                        <th width="20%">
                                            Supplier Name
                                        </th>
                                        <th width="10">
                                            Request Number
                                        </th>
                                        <th width="08%">
                                            Request Date
                                        </th>
                                        <th width="05%">
                                            Status
                                        </th>
                                        <th width="05%">
                                            Edit
                                        </th>
                                        <th width="05%">
                                            Delete
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
                                        <th colspan="2" align="right">
                                        </th>
                                    </tr>
                                    <tr>
                                        <th>
                                            Customer
                                        </th>
                                        <th>
                                            FPO Number(s)
                                        </th>
                                        <th>
                                            LPO Number(s)
                                        </th>
                                        <th>
                                            Supplier Name
                                        </th>
                                        <th>
                                            Request Number
                                        </th>
                                        <th>
                                            Request Date
                                        </th>
                                        <th>
                                            Status
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
                        <td colspan="6">
                            <asp:HiddenField ID="HFCust" runat="server" Value="" />
                            <asp:HiddenField ID="HFFPOs" runat="server" Value="" />
                            <asp:HiddenField ID="HFLPOs" runat="server" Value="" />
                            <asp:HiddenField ID="HFSupplier" runat="server" Value="" />
                            <asp:HiddenField ID="HFRqstNumber" runat="server" Value="" />                            
                            <asp:HiddenField ID="HFRqstFromDT" runat="server" Value="" />
                            <asp:HiddenField ID="HFRqstToDT" runat="server" Value="" />
                            <asp:HiddenField ID="HFStatus" runat="server" Value="" />
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
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/date.js" type="text/javascript"></script>
    <script src="../JScript/daterangepicker.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
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

        function Myvalidations() {
            var res = $('[id$=txtSuplrNm]').val();
            var res1 = $('[id$=txtFrmDt]').val();
            if (res.trim() == '' && res1.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Supplier Name Or Dates are Required.</span>');
                $('[id$=txtSuplrNm]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }

    </script>
    <script type="text/javascript">

        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);
            oTable = $("[id$=gvItmMstr]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aaSorting": [[0, "asc"]],
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                //"aLengthMenu": [[5000, -1], [5000, "All"]],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "../Logistics/LogisticsWS.asmx/GetPISItems",
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
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                      null, null
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFPOs]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFLPOs]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFSupplier]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFRqstNumber]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFRqstFromDT]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFRqstToDT]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $("#gvItmMstr").dataTable();
        });

        function Delet(valddd) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = RqstCEDtlsStatus.DeleteRecord(valddd.parentNode.parentNode.id);
                    var fres = result.value;
                    if (Number(fres) == 0) {
                        oTable.fnDraw();
                        SuccessMessage('Deleted Successfully');
                    }
                    else {
                        ErrorMessage('Cannot Delete this Record, IOM Created for this Reference' + valddd.parentNode.parentNode.id + '.');
                    }
                }
            } catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd) {
            try {

                window.location.replace("../Logistics/RqstCEDtls.Aspx?ID=" + valddd.parentNode.parentNode.id);
            } catch (e) {
                alert(e.Message);
            }
        }

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
