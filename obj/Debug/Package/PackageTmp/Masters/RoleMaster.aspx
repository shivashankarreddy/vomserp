<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="RoleMaster.aspx.cs" Inherits="VOMS_ERP.Masters.RoleMaster"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_filter
        {
            visibility: visible !important;
        }
    </style>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
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
    <table style="width: 98%; height: 98%; vertical-align: top; background-color: #F5F4F4;"
        align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr>
                        <td class="bcTdTitleSpaceRow" colspan="3">
                            <br />
                        </td>
                    </tr>
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Role Master" CssClass="bcTdTitleLabel"></asp:Label>
                            <div id="divMyMessage" runat="server" align="center" class="formError1" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                        <td style="text-align: right;">
                            All <font color="red" size="4"><b>*</b></font> fields are Mandatory
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;" class="bcTdnormal">
                            <font color="red" size="2"><b>*</b></font><span id="Label13" class="bcLabelright">Role
                                Name:</span>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtRole" class="bcAsptextbox" MaxLength="150"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClientClick="javascript:validations()"
                                                    OnClick="btnSave_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" OnClientClick="Javascript:clearAll()"
                                                    Text="Clear" />
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
                        <td colspan="3" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="bcTdNewTable">
                            <div id="dvEnmMstr" class="DivGridSize">
                                <center>
                                    <asp:GridView runat="server" ID="gvRole" AutoGenerateColumns="False" RowStyle-CssClass="bcGridViewRowStyle"
                                        EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                        PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                        AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle">
                                        <Columns>
                                            <asp:TemplateField HeaderText="S.No.">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex+1 %>
                                                    <asp:Label ID="lblSerialNo" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Width="10px" />
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="Role Name" DataField="Department Name" HeaderStyle-Width="170px" />
                                            <asp:TemplateField HeaderText="Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblID" Text='<%#Eval("Id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                            <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Delete"
                                                Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                        </Columns>
                                    </asp:GridView>
                                </center>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
