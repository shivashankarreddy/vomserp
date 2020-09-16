<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="AirWayBillStatus.aspx.cs" Inherits="VOMS_ERP.Invoices.AirWayBillStatus"
    EnableEventValidation="false"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Status of AirWayBill"
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
            <td align="right">
                <div runat="server" id="dvexport">
                    <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                        class="item_top_icons" Style="width: 15px !important; height: 15px !important;"
                        title="Export Excel" OnClick="btnExcelExpt_Click" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <table width="98%" align="center">
                    <tr>
                        <%--<td>
                            <div id="left_part2" style="width: 25%;">
                                <div class="form2" style="width: 100%;">
                                    <div class="add_enquiry" style="width: 100%; padding: 0 0 0 0;">
                                        <table border="0" cellpadding="0" cellspacing="0" class="top_heading" style="width: 100%;
                                            padding: 0 0 0 0;">
                                            <tr>
                                                <th>
                                                    <span class="search" style="width: 100%; padding: 0,0,0,0;">Search</span>
                                                </th>
                                            </tr>
                                        </table>
                                        <div>
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td>
                                                        <span id="Span1" class="bcLabel">From : </span>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" ID="txtFrmDt" Width="75px" onchange="changedate(this.id);"
                                                            CssClass="bcAsptextbox"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <span id="Span2" class="bcLabel">To :</span>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" ID="txtToDt" Width="75px" CssClass="bcAsptextbox"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <span id="Span3" class="bcLabelright">Customer:</span>
                                                    </td>
                                                    <td colspan="4">
                                                        <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="bcAspdropdown" Enabled="False">
                                                            <asp:ListItem Value="0" Text="-- Select --"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <span id="Span4" class="bcLabelright">Supplier:</span>
                                                    </td>
                                                    <td colspan="4">
                                                        <asp:DropDownList runat="server" ID="ddlSupplier" CssClass="bcAspdropdown" Enabled="false">
                                                            <asp:ListItem Value="0" Text="-- Select --"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        &nbsp;
                                                    </td>
                                                    <td class="bcTdButton" colspan="3">
                                                        <div id="Div4" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="right_part2" style="min-height: 20%; max-height: 40%; width: 74%;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:GridView runat="server" ID="GVAirWayBill" AutoGenerateColumns="false" RowStyle-CssClass="bcGridViewRowStyle"
                                                EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                                PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                                AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" EmptyDataText="No Records To Display...!"
                                                DataKeyNames="AWBID" OnPreRender="GVAirWayBill_PreRender" OnRowCommand="GVAirWayBill_RowCommand"
                                                OnRowDataBound="GVAirWayBill_RowDataBound">
                                                <Columns>
                                                    <%--<asp:TemplateField HeaderText="S.No." ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:HyperLinkField HeaderText="AirWayBill No." DataTextField="AWBNumber" DataNavigateUrlFields="AWBID"
                                                        DataNavigateUrlFormatString="AirWayBillDetails.Aspx?ID={0}" />
                                                    <asp:TemplateField HeaderText="Customers">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCustNm" runat="server" Text='<%# Eval("Customers") %>'></asp:Label>
                                                            <asp:HiddenField ID="hfCreatedBy" runat="server" Value='<%# Eval("CreatedBy") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Proforma Invoice No.s">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblProformaInv" runat="server" Text='<%# Eval("ShpngPrfmaInvcNmbr") %>'></asp:Label><%--ProformaINVIDs
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Freight">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFreight" runat="server" Text='<%# Eval("Freight") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Place Of Delivery">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPlaceOfDelivery" runat="server" Text='<%# Eval("PlaceOfDelivery") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                        Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Remove"
                                                        Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>--%>
                    </tr>
                </table>
                <table id="tblAirWayBill" cellpadding="0" cellspacing="0" border="0" class="display">
                    <thead>
                        <tr>
                            <th width="10%">
                                AirWayBill No.
                            </th>
                            <th width="5%">
                                Customers
                            </th>
                            <th width="10%">
                                Proforma Invoice No.s
                            </th>
                            <th width="05%">
                                Freight
                            </th>
                            <th width="10%">
                                Place Of Delivery
                            </th>
                            <th width="3%">
                                E
                            </th>
                            <th width="3%">
                                D
                            </th>
                        </tr>
                    </thead>
                    <tbody class="tbody">
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="3">
                            </th>
                            <th colspan="2" align="left">
                            </th>
                            <th colspan="2" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                AirWayBill No.
                            </th>
                            <th>
                                Customers
                            </th>
                            <th>
                                Proforma Invoice No.s
                            </th>
                            <th>
                                Freight
                            </th>
                            <th>
                                Place Of Delivery
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
                <asp:HiddenField ID="HFAWBNo" runat="server" Value="" />
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFPrfrmInvNo" runat="server" Value="" />
                <asp:HiddenField ID="HFFreight" runat="server" Value="" />
                <asp:HiddenField ID="HFPlcOfDelvry" runat="server" Value="" />
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
        var oTable;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            /*          Main Functionality       */
            oTable = $('#tblAirWayBill').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "AirwayBillWS.asmx/GetAirWayBillStat",
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
                                    $("#tblAirWayBill").show();
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

            $("#tblAirWayBill").dataTable().columnFilter(
                {
                    "aoColumns": [{ "type": "text" },
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
                    $('[id$=HFAWBNo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFPrfrmInvNo]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFreight]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFPlcOfDelvry]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#tblAirWayBill').dataTable();
        });


        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = AirWayBillStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Invoices/AirWayBill.aspx?ID=" + valddd.parentNode.parentNode.id);
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
                    var result = AirWayBillStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust,CompanyId);
                    var fres = result.value;
                    if (fres.contains('Success::')) {
                        oTable.fnDraw();
                        SuccessMessage(fres.replace('Success::', ''));
                    }
                    else if (fres.contains('Error::')) {
                        ErrorMessage(fres.replace('Error::', ''));
                        //ErrorMessage('Cannot Delete this Record, LE already created so delete LE/ Error while Deleting ' + valddd.parentNode.parentNode.id + '.');
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
            $("[id$=GVAirWayBill]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search :"
                },

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
        });       
    </script>
</asp:Content>
