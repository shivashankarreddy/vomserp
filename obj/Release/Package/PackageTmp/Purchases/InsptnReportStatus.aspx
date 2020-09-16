<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="InsptnReportStatus.aspx.cs" Inherits="VOMS_ERP.Purchases.InsptnReportStatus"
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblInsptn" runat="server" Text="Inspection Report Status"
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
                <table id="gvInsptnStatus" class="widthFull fontsize10 displayNone" cellpadding="0"
                    cellspacing="0" border="0">
                    <thead>
                        <tr>
                            <th width="15%">
                                Inspection Ref.No.
                            </th>
                            <th width="15%">
                                Inspection Date
                            </th>
                            <th width="15%">
                                Inspector
                            </th>
                            <th width="15%">
                                Third Party Inspector
                            </th>
                            <th width="15%">
                                Contact Person
                            </th>
                            <th width="15%">
                                Contact Number
                            </th>
                            <th width="15%">
                                Contact Address
                            </th>
                            <th width="15%">
                                Inspection Details
                            </th>
                            <th width="15">
                                Stage
                            </th>
                            <th>
                                Ready/Replan Date
                            </th>
                            <th>
                                Status
                            </th>
                            <th width="08%">
                                E
                            </th>
                            <th width="08%">
                                D
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="9">
                            </th>
                            <th colspan="4" align="left">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Inspection Ref.No.
                            </th>
                            <th>
                                Inspection Date
                            </th>
                            <th>
                                Inspector
                            </th>
                            <th>
                                Third Party Inspector
                            </th>
                            <th>
                                Contact Person
                            </th>
                            <th>
                                Contact Number
                            </th>
                            <th>
                                Contact Address
                            </th>
                            <th>
                                Inspection Details
                            </th>
                            <th>
                                Stage
                            </th>
                            <th>
                                Ready/Replan Date
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
            <td>
                <asp:HiddenField ID="HFInsptnRefNo" runat="server" Value="" />
                <asp:HiddenField ID="HFInsFDate" runat="server" Value="" />
                <asp:HiddenField ID="HFInsTDate" runat="server" Value="" />
                <asp:HiddenField ID="HFInspector" runat="server" Value="" />
                <asp:HiddenField ID="HFTDInspector" runat="server" Value="" />
                <asp:HiddenField ID="HFCntPersn" runat="server" Value="" />
                <asp:HiddenField ID="HFConNum" runat="server" Value="" />
                <asp:HiddenField ID="HFConAddrs" runat="server" Value="" />
                <asp:HiddenField ID="HFInsDtls" runat="server" Value="" />
                <asp:HiddenField ID="HFStage" runat="server" Value="" />
                <asp:HiddenField ID="HFReplanFDt" runat="server" Value="" />
                <asp:HiddenField ID="HFReplanTDt" runat="server" Value="" />
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
            </td>
        </tr>
    </table>
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media_ColVis/css/ColVisAlt.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/media/js/TableTools.js" type="text/javascript"></script>
    <script src="../JScript/media/js/ZeroClipboard.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/ZeroClipboard.js" type="text/javascript"></script>
    <script src="../JScript/media/js/TableTools.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script type="text/javascript">

        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=gvInsptnStatus]").dataTable({
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
                "sAjaxSource": "Purchases_WebService.asmx/GetInspItems",
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
                                    $("#gvInsptnStatus").show();
                                }
                    });
                }
            });
            $("#gvInsptnStatus").dataTable().columnFilter(
                {
                    "aoColumns": [
                                   { "type": "text" },
                                   { "type": "date-range" },
                                   { "type": "text" },
                                   { "type": "text" },
                                   { "type": "text" },
                                   { "type": "text" },
                                   { "type": "text" },
                                   { "type": "text" },
                                   { "type": "text" },
                                   { "type": "date-range" },
                                   { "type": "text" },
                                   null, 
                                   null
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFInsptnRefNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFInsFDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFInsTDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFInspector]').val(Valuee);
                } else if (InDex == 4) {
                    $('[id$=HFTDInspector]').val(Valuee);
                } else if (InDex == 5) {
                    $('[id$=HFCntPersn]').val(Valuee);
                } else if (InDex == 6) {
                    $('[id$=HFConNum]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFConAddrs]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFInsDtls]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFStage]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFReplanFDt]').val(Valuee);
                }
                else if (InDex == 11) {
                    $('[id$=HFReplanTDt]').val(Valuee);
                }
                else if (InDex == 12) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvInsptnStatus").dataTable();
        });


        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = InsptnReportStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Purchases/InspectionReport.Aspx?ID=" + valddd.parentNode.parentNode.id);
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
                    var result = InsptnReportStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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



        //        $(window).load(function () {
        //            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        //        });

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


        //        function changedate() {
        //            var strdateEnqDT = $('[id$=txtFrmDt]').val();
        //            var strdateEnqDT1 = strdateEnqDT.split('-');
        //            strdateEnqDT = (strdateEnqDT1[1] + '-' + strdateEnqDT1[0] + '-' + strdateEnqDT1[2]);
        //            strdateEnqDT = new Date(strdateEnqDT.replace(/-/g, "/"));
        //            $('[id$=txtToDt]').datepicker('option', {
        //                minDate: new Date(strdateEnqDT),
        //                maxDate: dateToday,
        //                dateFormat: 'dd-mm-yy',
        //                changeMonth: true,
        //                changeYear: true
        //            });
        //        }



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

        /// not using
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
</asp:Content>
