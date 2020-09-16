<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="WeeklyEnquiryReport.aspx.cs" Inherits="VOMS_ERP.Reports.WeeklyEnquiryReport" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Style="font-size: 15.5px;
                                            font-weight: bold" Text="WEEKLY ENQUIRY REPORT" CssClass="bcTdTitleLabel"></asp:Label><div
                                                id="divMyMessage" runat="server" align="center" class="formError1" style="margin-right: 5%;" />
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
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 5px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Customer Name :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlcustomer" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">From Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFrmDt" CssClass="bcAsptextbox" MaxLength="12"
                                            onchange="changedate(this.id);" Width="80px"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">To Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtToDt" CssClass="bcAsptextbox" MaxLength="12" Width="80px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="8">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div4" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="lbtnSearch" Text="Search" OnClick="lbtnSearch_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div5" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="lbtnClear" Text="Clear" OnClick="lbtnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div6" class="bcButtonDiv">
                                                            <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>&nbsp;
                                                        </div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
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
                                        <div style="min-height: 200px; max-height: 500px; overflow: auto;">
                                            <asp:Repeater ID="GvWklyRpt" runat="server">
                                                <HeaderTemplate>
                                                    <table id="tblCountryList" cellpadding="0" cellspacing="0" border="0" class="display"
                                                        style="font-family: Arial;">
                                                        <thead style="font-style: bold;">
                                                            <tr>
                                                                <%-- <th style="display:none;">
                                                                    CreatedDate
                                                                </th>--%>
                                                                <th>
                                                                    Enquiry Number
                                                                </th>
                                                                <th>
                                                                    Enquiry Date
                                                                </th>
                                                                <th>
                                                                    Description
                                                                </th>
                                                                <th>
                                                                    FERecv Date
                                                                </th>
                                                                <th>
                                                                    Quotation in US $
                                                                </th>
                                                                <th>
                                                                    Quotation Submitted On
                                                                </th>
                                                                <th>
                                                                    LPODT_Supplier
                                                                </th>
                                                                <th>
                                                                    Status
                                                                </th>
                                                                <th style="width: 380px !important;">
                                                                    Remarks
                                                                </th>
                                                                <th>Created By</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody class="tbody">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr id="<%# Eval("Foreign Enquiry ID") %>">
                                                        <%--<td style="display:none;">
                                                            <%# Eval("CreatedDate")%>
                                                        </td>--%>
                                                        <td>
                                                            <%# Eval("Enquiry Number")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Enquiry Date")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Subject")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("FERecv Date")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Final Amt")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Quotation Date")%>
                                                            <td>
                                                                <%# Eval("LPODT")%>
                                                            </td>
                                                            <td>
                                                                <%# Eval("Status")%>
                                                            </td>
                                                            <td>
                                                                <%# Eval("Remarks")%>
                                                            </td>
                                                            <td>
                                                                <%# Eval("CreatedBy")%>
                                                            </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </tbody> </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblLegend" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/jquery.jeditable.js" type="text/javascript"></script>
    <script src="../JScript/jquery.dataTables.editable.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            var data = "";
            data = $('[id$=hfData]').val();
            $('#tblCountryList').dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search all columns:"
                }
            }).makeEditable({
                sUpdateURL: "WeeklyEnquiryReportHandler.ashx",
                "aoColumns": [null, null, null, null, null, null,null,
                {
                    indicator: 'Saving...',
                    tooltip: 'DOUBLE CLICK ON THE RECORD TO EDIT AND CLICK ENTER TO SAVE',
                    loadtext: 'loading...',
                    onblur: 'submit'
                },
                {
                    indicator: 'Saving...',
                    tooltip: 'DOUBLE CLICK ON THE RECORD TO EDIT AND CLICK ENTER TO SAVE',
                    loadtext: 'loading...',
                    type: 'textarea',
                    onblur: 'submit'
                }]
            });
        });

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


    </script>
</asp:Content>
