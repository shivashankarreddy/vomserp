<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CVMapping.aspx.cs" Inherits="VOMS_ERP.Admin.CVMapping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Customer Vendor Mapping"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Vendors<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox ID="ListBoxVendor" runat="server" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                Rows="10"></asp:ListBox>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Customers<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox ID="ListBoxCustomers" runat="server" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                Rows="10"></asp:ListBox>
                        </td>
                    </tr>
                </table>
                <tr>
                    <td align="center" class="bcTdNewTable" colspan="6">
                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                            <tbody>
                                <tr align="center" valign="middle">
                                    <td align="center" valign="middle" class="bcTdButton">
                                        <div id="Div1" class="bcButtonDiv">
                                            <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                                        </div>
                                    </td>
                                    <td align="center" valign="middle" class="bcTdButton">
                                        <div id="Div2" class="bcButtonDiv">
                                            <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/date.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">

        function Myvalidations() {

            if (($('[id$=ListBoxVendor]').val() == null)) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Vendor Name is Required.</span>');
                $('[id$=lbsuplrs]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            if (($('[id$=ListBoxCustomers]').val() == null)) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer Name is Required.</span>');
                $('[id$=lbsuplrs]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }

        }
    </script>
</asp:Content>
