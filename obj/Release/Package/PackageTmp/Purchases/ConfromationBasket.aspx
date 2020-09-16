<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="ConfromationBasket.aspx.cs" Inherits="VOMS_ERP.Purchases.ConfromationBasket"
    %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Foreign Quotation Confirmation Basket"
                                            CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="6">
                                        All <font color="red" size="4"><b>*</b></font>fields are Mandatory
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                    align="center">
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Name of Customer<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlcustmr" class="bcAspdropdown" OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Value="0" Text="Select Customer"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Enquiry Number<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlfenqy" class="bcAspdropdown" Visible="false"
                                AutoPostBack="true">
                                <asp:ListItem Value="0" Text="Select Enquiry Number"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ListBox runat="server" ID="Lstfenqy" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                OnSelectedIndexChanged="ddlfenqy_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal" colspan="6">
                            <center>
                                <asp:GridView runat="server" ID="gvConformBskt" RowStyle-CssClass="bcGridViewRowStyle"
                                    EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                    PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                    AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%">
                                </asp:GridView>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" align="right" class="bcTdNewTable">
                            <center>
                                <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                    <tbody>
                                        <tr valign="middle">
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div1" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnSave" Text="Confirm" OnClick="btnSave_Click" />
                                                    <%--OnClientClick="javascript:Myvalidations()"--%>
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div2" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div3" class="bcButtonDiv">
                                                    <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        function Myvalidations() {
            var ConformBskt = 0;
            if ($('[id$=gvConformBskt]').length > 0)
                ConformBskt = $('[id$=gvConformBskt]')[0].rows.length;

            if (($('[id$=ddlcustmr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer is Required.</span>');
                $('[id$=ddlcustmr]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if ($('[id$=Lstfenqy]').val() == null) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Enquiry is Required.</span>');
                $('[id$=Lstfenqy]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (ConformBskt <= 1) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">No Items to Confirm.</span>');
                $('[id$=gvConformBskt]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }
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
