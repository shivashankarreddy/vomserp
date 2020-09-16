<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ExciseLedger.aspx.cs" Inherits="VOMS_ERP.Logistics.ExciseLedger"
     EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <div id="divMessage">
                </div>
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Excise Ledger" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: centre;" class="bcTdnormal" colspan="3">
                            <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                width: 100%; margin: 5px; height: 100%;">
                                <div id="divSearch" class="row" style="text-align: center; width: 98%;">
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span3" class="bcLabelright">From Date<font color="red" size="2"><b></b></font>:</span>
                                    </div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtFrmDt" MaxLength="150" onchange="changedate();"
                                                class="bcAsptextbox"></asp:TextBox>
                                        </span>
                                    </div>
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span4" class="bcLabelright">To Date<font color="red" size="2"><b></b></font>:</span>
                                    </div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtToDt" MaxLength="150" class="bcAsptextbox"></asp:TextBox>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="6">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSend" Text="Search" OnClick="btnSend_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div3" class="bcButtonDiv">
                                                <a href="../Masters/Home.aspx" title="Exit" class="bcAlink" onclick="javascript:Exit()">
                                                    Exit </a>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel"><asp:Label ID="LblTotAmount" runat="server" Text="Total Bond Amount :"></asp:Label></span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTBAmt" CssClass="bcAsptextbox" Enabled="false"
                                            Text="5000.00"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Bond Amount as on Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtBndAmtDt" CssClass="bcAsptextbox" Enabled="false"
                                            Text="4200.00"></asp:TextBox>
                                    </td>
                                    <td colspan="2" style="width: 30%; text-align: right;">
                                        <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                            class="item_top_icons" title="Export Excel" OnClick="btnExcelExpt_Click" />
                                        <asp:ImageButton ID="btnWordExpt" runat="server" ImageUrl="../images/word.png" class="item_top_icons"
                                            title="Export Word" OnClick="btnWordExpt_Click" />
                                        <asp:ImageButton ID="btnPdfExpt" runat="server" ImageUrl="../images/pdf.png" class="item_top_icons"
                                            title="Export PDF" OnClick="btnPdfExpt_Click"></asp:ImageButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <div id="dvSevottam">
                                            <asp:GridView runat="server" ID="gvSevottam" AutoGenerateColumns="false" RowStyle-CssClass="bcGridViewRowStyle"
                                                EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                                PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                                AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" EmptyDataText="No Records To Display...!"
                                                OnPreRender="gvSevottam_PreRender">
                                                <AlternatingRowStyle CssClass="bcGridViewAlternatingRowStyle"></AlternatingRowStyle>
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." HeaderStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Date">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblDate" Text='<%#Eval("TDate") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Description">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblDescription" Text='<%#Eval("Description") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Debit(Rs)" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblDebit" Text='<%#GetFormatedNumber(Eval("Debit")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Credit(Rs)" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblCredit" Text='<%#GetFormatedNumber(Eval("Credit")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Sevottam ID" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblEnmMsterID" Text='<%#Eval("ID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%-- <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                            Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Remove"
                                            Text="Delete"  ShowHeader="true" HeaderStyle-Width="20px" />--%>
                                                </Columns>
                                                <EmptyDataRowStyle CssClass="bcGridViewEmptyDataRowStyle"></EmptyDataRowStyle>
                                                <RowStyle VerticalAlign="Bottom" CssClass="bcGridViewRowStyle"></RowStyle>
                                            </asp:GridView>
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
            //without passing class names.
            $("[id$=gvSevottam]").dataTable({
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
