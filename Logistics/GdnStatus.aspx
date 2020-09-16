<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="GdnStatus.aspx.cs" Inherits="VOMS_ERP.Logistics.GdnStatus" EnableEventValidation="false"
     %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%--<%@ Register Src="UC_GDN_Approve.ascx" TagName="UC_GDN_Approve" TagPrefix="uc1" %>--%>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Goods Dispatch Note(GDN) Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" style="margin-right: 10%;" />
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
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                                            <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                                                border="0">
                                                <thead>
                                                    <tr>
                                                        <th width="08%">
                                                            Dispatch Inst No.
                                                        </th>
                                                        <th width="08%">
                                                            Dispatch Date
                                                        </th>
                                                        <th width="10%">
                                                            FPO Number(s)
                                                        </th>
                                                        <th width="10%">
                                                            LPO Number(s)
                                                        </th>
                                                        <th width="10%">
                                                            Supplier Name
                                                        </th>
                                                        <th width="10%">
                                                            Invoice Number
                                                        </th>
                                                        <th width="8%">
                                                            Invoice Date
                                                        </th>
                                                        <th width="10%">
                                                            Ref. GDN
                                                        </th>
                                                        <th width="8%">
                                                            Status
                                                        </th>
                                                        <th width="02%">
                                                            Edit
                                                        </th>
                                                        <th width="02%">
                                                            Delete
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody class="tbody">
                                                </tbody>
                                                <tfoot>
                                                    <tr>
                                                        <th style="text-align: right" colspan="5">
                                                        </th>
                                                        <th colspan="3" align="left">
                                                        </th>
                                                        <th colspan="3" align="right">
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th>
                                                            Dispatch Inst No.
                                                        </th>
                                                        <th>
                                                            Dispatch Date
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
                                                            Ref. GDN
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
                                        <asp:HiddenField ID="HFDisInsNo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFDspFrmDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFDspToDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFLPONo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFSuplrNm" runat="server" Value="" />
                                        <asp:HiddenField ID="HFInvcNo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFInvFrmDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFInvToDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFRefGDN" runat="server" Value="" />
                                        <asp:HiddenField ID="HFStatus" runat="server" Value="" />
                                    </td>
                                </tr>
                            </table>
                            <%--<uc1:UC_GDN_Approve ID="IsApprove" runat="server" OnButtonClicked="OnButtonClicked" />--%>
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

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 84 + "px");
        });


        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });


        /// not using
    </script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
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
                "sAjaxSource": "CT1WebService1.asmx/GetGDNItems",
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

            $("#gvItmMstr").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                      null, null
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFDisInsNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFDspFrmDt]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFDspToDt]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFLPONo]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFSuplrNm]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFInvcNo]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFInvFrmDt]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFInvToDt]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFRefGDN]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#gvItmMstr').dataTable();
        });

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = GdnStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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

        function UpdateStatus(valddd, MailID) {
            try {

                var result = GdnStatus.UpdateStatus(valddd.parentNode.parentNode.id, 0, MailID.toString());
                var fres = result.value;
                if (fres == 'Success') {
                    //DataTableLoad();
                    oTable.fnDraw();
                    SuccessMessage('Approved Successfully');
                }
                else {
                    ErrorMessage(fres);
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }

        function EditDetails(valddd, App, CreatedBy, IsCust) {
            try {
                if (App != "Approved") {
                    var result = GdnStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres == 'Success') {
                        window.location.replace("../Logistics/Gdn.Aspx?ID=" + valddd.parentNode.parentNode.id);
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
                else {
                    alert("Cannot Edit when Approved.");
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
