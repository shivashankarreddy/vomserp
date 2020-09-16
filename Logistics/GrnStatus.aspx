<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="GrnStatus.aspx.cs" Inherits="VOMS_ERP.Logistics.GrnStatus" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Goods Receipt Note(GRN) Status"
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
                                    class="item_top_icons" title="Export Excel" Style="width: 15px; height: 16px;"
                                    OnClick="btnExcelExpt_Click" />
                            </div>
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
                                                        <th width="18%">
                                                            GRN No.
                                                        </th>
                                                        <th width="15%">
                                                            Ref. Nmbr
                                                        </th>
                                                        <th width="08%">
                                                            Received Date
                                                        </th>
                                                        <th width="08%">
                                                            FPO Number(s)
                                                        </th>
                                                        <th width="15%">
                                                            LPO Number(s)
                                                        </th>
                                                        <th width="15%">
                                                            Supplier Name
                                                        </th>
                                                        <th width="8%">
                                                            Invoice Number
                                                        </th>
                                                        <th width="8%">
                                                            Invoice Date
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
                                                        <th style="text-align: right" colspan="4">
                                                        </th>
                                                        <th colspan="3" align="left">
                                                        </th>
                                                        <th colspan="3" align="right">
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th>
                                                            GRN No.
                                                        </th>
                                                        <th>
                                                            Ref. Nmbr
                                                        </th>
                                                        <th>
                                                            Received Date
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
                                                            Invoice Number
                                                        </th>
                                                        <th>
                                                            Invoice Date
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
                                        <asp:HiddenField ID="HFGRNNo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFRefNmbr" runat="server" Value="" />
                                        <asp:HiddenField ID="HFRecvdFrmDte" runat="server" Value="" />
                                        <asp:HiddenField ID="HFRecvdToDte" runat="server" Value="" />
                                        <asp:HiddenField ID="HFFPONum" runat="server" Value="" />
                                        <asp:HiddenField ID="HFLPONum" runat="server" Value="" />
                                        <asp:HiddenField ID="HFSuppNm" runat="server" Value="" />
                                        <asp:HiddenField ID="HFInvNm" runat="server" Value="" />
                                        <asp:HiddenField ID="HFInvFrmDte" runat="server" Value="" />
                                        <asp:HiddenField ID="HFInvToDte" runat="server" Value="" />
                                    </td>
                                </tr>
                            </table>
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
            $(".aligntable").width($(window).width() - 84 + "px");
        });

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
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
    <script type="text/javascript">

        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);
            myfunction();
        });

        function myfunction() {
            oTable = $("#gvItmMstr").dataTable({
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
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "CT1WebService1.asmx/GetGRNItems",
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
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" }, null, null
                                   ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFGRNNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFRefNmbr]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFRecvdFrmDte]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFRecvdToDte]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFFPONum]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFLPONum]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFSuppNm]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFInvNm]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFInvFrmDte]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFInvToDte]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvItmMstr").dataTable();
        }


        /* Init the table */
        oTable = $("#gvItmMstr").dataTable();
        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = GrnStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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
                var result = GrnStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Logistics/Grn.Aspx?ID=" + valddd.parentNode.parentNode.id);
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
