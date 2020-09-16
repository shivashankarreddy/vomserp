<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Iom2FormStatus.aspx.cs" Inherits="VOMS_ERP.Logistics.Iom2FormStatus"
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="IOM Template for Logistics Status"
                                            CssClass="bcTdTitleLabel"></asp:Label>
                                            <div id="divMyMessage" runat="server" align="center"
                                                class="formError1" style="margin-right: 25%;" />
                                    </td>
                                    <td align="right">
                                        <div runat="server" id="dvexport" >
                                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                                class="item_top_icons" title="Export Excel"  OnClick="btnExcelExpt_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                     <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                                        <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                                            border="0">
                                            <thead>
                                                <tr>
                                                    <th width="14%">
                                                        Reference Number
                                                    </th>
                                                    <th width="8%">
                                                        IOM Date
                                                    </th>
                                                    <th width="8%">
                                                        Customer Name
                                                    </th>
                                                    <th width="08%">
                                                        Subject
                                                    </th>
                                                    <th width="15%">
                                                        FPO Number(s)
                                                    </th>
                                                    <th width="15%">
                                                        LPO Number(s)
                                                    </th>
                                                    <th width="8%">
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
                                                    <th colspan="4" align="left">
                                                    </th>
                                                </tr>
                                                <tr>
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
                                        <asp:HiddenField ID="HFRefNo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFIOMFrmDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFIOMToDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFCust" runat="server" Value="" />
                                        <asp:HiddenField ID="HFSub" runat="server" Value="" />
                                        <asp:HiddenField ID="HFFPO" runat="server" Value="" />
                                        <asp:HiddenField ID="HFLPO" runat="server" Value="" />
                                        <asp:HiddenField ID="HFStatus" runat="server" Value="" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/Scripts/js/jquery.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/ZeroClipboard.js" type="text/javascript"></script>
    <script src="../JScript/media/js/TableTools.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script type="text/javascript">

//        $(window).load(function () {
//            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
//        });

        /// not using
        function Myvalidations() {
            var res = $('[id$=txtCstmrNm]').val();
            var res1 = $('[id$=txtFrmDt]').val();
            if (res.trim() == '' && res1.trim() == '') {
                ErrorMessage('Supplier Name Or Dates are Required.');
                $('[id$=txtSuplrNm]').focus();
                return false;
            }
            else {
                return true;
            }
        }
    </script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            /*          Main Functionality       */
            oTable = $('#gvItmMstr').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "CT1WebService1.asmx/GetIOMLOGItems",
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
                /* Init the table */
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
                                      null, null
                                    ]
                });
            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFRefNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFIOMFrmDt]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFIOMToDt]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFSub]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFFPO]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFLPO]').val(Valuee);
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
                    var result = Iom2FormStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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

        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = Iom2FormStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Logistics/Iom2Form.Aspx?ID=" + valddd.parentNode.parentNode.id);
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }
        $(window).load(function () {
            $("#clickExcel").click(function () {
                var gvItmMstr = $('#gvItmMstr').html();
                window.open('data:application/vnd.ms-excel,' + $('#gvItmMstr').html());
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
