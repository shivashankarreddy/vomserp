<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="WeekReport.aspx.cs" Inherits="VOMS_ERP.Reports.WeekReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<style type = "text/css">
.aligntable
{
    display: block; /* width:35.8% !important; */
    margin: 2px 0px 0px 8px;
            
}
</style>
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" style="font-size:15.5px;font-weight:bold" Text="WEEKLY REPORT" CssClass="bcTdTitleLabel"></asp:Label><div
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
                                        <span class="bcLabelright">Report Type <font color="red" size="2"><b>*</b></font>:
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlReportType" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Enquiry Report" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="FPO Report" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Enquiry Pending" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Invoice Pending" Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
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
                            <table width="100%" >
                                <tr>
                                    <td>
                                    <div class="aligntable" id="aligntbl" style="margin-left: 21px !important;">
                                        <div style="min-height: 200px; max-height: 500px; overflow: auto;">
                                            <asp:GridView runat="server" ID="GvWklyRpt" Width="100%" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="true" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                EmptyDataText="No Records are Exists" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                                AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" 
                                                Caption="Weekly Reports" >
                                            </asp:GridView>
                                        </div>
                                        </div>
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

    <script type="text/javascript">

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 84 + "px");
        });
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

    <script type="text/javascript">
        $(document).ready(function () {

            $("[id$=GvWklyRpt]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search critera",
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
