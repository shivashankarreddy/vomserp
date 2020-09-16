<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="DrawingApprovalStatus.aspx.cs" Inherits="VOMS_ERP.Purchases.DrawingApprovalStatus"
     EnableEventValidation="false" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblDrwngAprvl" runat="server" Text="Drawing Approval Status"
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
                            <asp:ImageButton ID="btnExcelExpt" OnClick="btnExcelExpt_Click" runat="server" ImageUrl="../images/EXCEL.png" title="Export Excel">
                                        </asp:ImageButton>
                        </td>
                    </tr>
                </table>
                <table id="gvDRWNGStatus" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                    border="0">
                    <thead>
                        <tr>
                            <th width="15%">
                                Drawing Ref.No.
                            </th>
                            <th width="07%">
                               Customer Name
                            </th>
                            <th width="15%">
                               Supplier Name
                            </th>
                            <th width="15%">
                               FPO No.
                            </th>
                            <th width="15%">
                                LPO No.
                            </th>
                            <th width="15">
                              LPO Date
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
                            <th style="text-align: right" colspan="3">
                            </th>
                            <th colspan="5" align="left">
                            </th>
                            <%--<th colspan="3" align="right">
                            </th>--%>
                        </tr>
                        <tr>
                            <th>
                               Drawing Ref.No.
                            </th>
                            <th>
                                Customer Name
                            </th>
                            <th>
                                Supplier Name
                            </th>
                            <th>
                                 FPO No.
                            </th>
                            <th>
                                LPO No.
                            </th>
                            <th>
                                LPO Date
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
                <asp:HiddenField ID="HFDrwngRefNo" runat="server" Value="" />
                <asp:HiddenField ID="HFCustNm" runat="server" Value="" />
                <asp:HiddenField ID="HFSuplNm" runat="server" Value="" />
                <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFLPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFLPOFDt" runat="server" Value="" />
                <asp:HiddenField ID="HFLPOTDt" runat="server" Value="" />
            </td>
        </tr>
    </table>
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

            oTable = $("[id$=gvDRWNGStatus]").dataTable({
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
                "sAjaxSource": "Purchases_WebService.asmx/GetDrwngItems",
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
                                    $("#gvDRWNGStatus").show();
                                }
                    });
                }
            });
                    $("#gvDRWNGStatus").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                      null,null
                                    ]
                });
              
            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFDrwngRefNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFCustNm]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFSuplNm]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFLPONo]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFLPOFDt]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFLPOTDt]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvDRWNGStatus").dataTable();
        });
    
      function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = DrawingApprovalStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Purchases/DrawingApproval.Aspx?ID=" + valddd.parentNode.parentNode.id);
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust, CompanyId) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = DrawingApprovalStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust, CompanyId);
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
     
    </script>
</asp:Content>
