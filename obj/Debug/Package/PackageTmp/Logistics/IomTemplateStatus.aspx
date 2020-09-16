<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="IomTemplateStatus.aspx.cs" Inherits="VOMS_ERP.Logistics.IomTemplateStatus" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="IOM Template Status"
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
                                    class="item_top_icons" title="Export Excel" Style="width: 15px; height: 16px"
                                    OnClick="btnExcelExpt_Click" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table id="gvIOMTmplt" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                                border="0">
                                <thead>
                                    <tr>
                                        <%--<th width="08%">
                                            S.No
                                        </th>--%>
                                        <th width="08%">
                                            Reference Number
                                        </th>
                                        <th width="08%">
                                            IOM Date
                                        </th>
                                        <th width="05%">
                                            Customer Name
                                        </th>
                                        <th width="15%">
                                            Supplier Name
                                        </th>
                                        <th width="8%">
                                            Subject
                                        </th>
                                        <th width="5">
                                            FPO Number(s)
                                        </th>
                                        <th width="5">
                                            LPO Number(s)
                                        </th>
                                        <th width="5">
                                            Status
                                        </th>
                                        <th width="04%">
                                            Edit
                                        </th>
                                        <th width="03%">
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
                                        <th colspan="4" align="right">
                                        </th>
                                    </tr>
                                    <tr>
                                        <%--<th>
                                            &nbsp;
                                        </th>--%>
                                        <th>
                                            Reference Number
                                        </th>
                                        <th>
                                            IOM Date
                                        </th>
                                        <th>
                                            Customer Name
                                        </th>
                                        <th>
                                            Supplier Name
                                        </th>
                                        <th>
                                            Subject
                                        </th>
                                        <th>
                                            FPO Number(s)
                                        </th>
                                        <th>
                                            LPO Number(s)
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
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:HiddenField ID="HFRefNmbr" runat="server" Value="" />
                            <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                            <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                            <asp:HiddenField ID="HFCstnrNm" runat="server" Value="" />
                            <asp:HiddenField ID="HFSuplrNm" runat="server" Value="" />
                            <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                            <asp:HiddenField ID="HFFpoNmbrs" runat="server" Value="" />
                            <asp:HiddenField ID="HFLpoNmbrs" runat="server" Value="" />
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
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script type="text/javascript">
        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);
            oTable = $("[id$=gvIOMTmplt]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                "aaSorting": [[0, "asc"]],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "CT1WebService1.asmx/GetIOMItems",
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
                                    $("#gvIOMTmplt").show();
                                }
                    });
                }
            });

            $("#gvIOMTmplt").dataTable().columnFilter(
                {
                    "aoColumns": [{ "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
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
                    $('[id$=HFRefNmbr]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFCstnrNm]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFSuplrNm]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFSubject]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFFpoNmbrs]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFLpoNmbrs]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#gvIOMTmplt').dataTable();
        });

        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                //                var result = IomTemplateStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                //                var fres = result.value;
                //                if (fres == 'Success') {
                window.location.replace("../Logistics/IomForm.Aspx?ID=" + valddd.parentNode.parentNode.id);
                //                }
                //                else {
                //                    ErrorMessage(fres);
                //                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = IomTemplateStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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
        //        function exportToExcel() {
        //            var strCopy = document.getElementById("detailsTable").innerHTML;
        //            window.clipboardData.setData("Text", strCopy);
        //            var objExcel = new ActiveXObject("Excel.Application");
        //            objExcel.visible = true;

        //            var objWorkbook = objExcel.Workbooks.Add;
        //            var objWorksheet = objWorkbook.Worksheets(1);
        //            objWorksheet.Paste;
        //        }



        $(window).load(function () {
            $("#clickExcel").click(function () {
                var gvIOMTmplt = $('#gvIOMTmplt').html();
                window.open('data:application/vnd.ms-excel,' + $('#gvIOMTmplt').html());
            });
        });


        //        jQuery(document).ready(function($) {

        //            // Define any icon actions before calling the toolbar 
        //            $('.toolbar-icons a').on('click', function(event) {
        //                event.preventDefault();
        //            });

        //            $('.settings-button').toolbar({ content: '#user-options', position: 'top', hideOnClick: true });
        //            //$('#normal-button').toolbar({ content: '#user-options', position: 'Left' });
        //            //				$('#normal-button-small').toolbar({content: '#user-options-small', position: 'top', hideOnClick: true});
        //            //				$('#button-left').toolbar({content: '#user-options', position: 'left'});
        //            //				$('#button-right').toolbar({content: '#user-options', position: 'right'});
        //            //				$('#link-toolbar').toolbar({content: '#user-options', position: 'top'});
        //        });

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

        //        function changedate() {
        //            var strdateDT = $('[id$=txtFrmDt]').val();
        //            var strdateDT1 = strdateDT.split('-');
        //            strdateDT = (strdateDT1[1] + '-' + strdateDT1[0] + '-' + strdateDT1[2]);
        //            strdateDT = new Date(strdateDT.replace(/-/g, "/"));
        //            $('[id$=txtToDt]').datepicker('option', {
        //                minDate: new Date(strdateDT),
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
