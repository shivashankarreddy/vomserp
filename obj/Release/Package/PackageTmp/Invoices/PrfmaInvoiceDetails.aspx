<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="PrfmaInvoiceDetails.aspx.cs" Inherits="VOMS_ERP.Invoices.PrfmaInvoiceDetails" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td>
                <asp:ImageButton ID="btnWordExpt" runat="server" ImageUrl="../images/Word1.png" class="item_top_icons"
                    title="Export Excel" CausesValidation="False"  OnClick="btnWordExpt_Click">
                </asp:ImageButton>
            </td>
        </tr>
        <tr>
            <td class="bcTdNewTable" align="center" colspan="2">
                <CR:CrystalReportViewer ID="ProformaInvoiceDetails" runat="server" AutoDataBind="true"
                    GroupTreeStyle-ShowLines="False" HasCrystalLogo="False" HasDrilldownTabs="False"
                    HasDrillUpButton="False" HasPageNavigationButtons="True" HasPrintButton="False"
                    HasToggleGroupTreeButton="False" HasToggleParameterPanelButton="False" ToolPanelView="None" />
            </td>
        </tr>
        <tr>
            <td align="right" class="NoUnderLine" style="width: 18%">
                <asp:LinkButton runat="server" ID="lbtnBack" Text="Back">
                </asp:LinkButton>
            </td>
            <td align="right">
                <div style="padding-right: 20%;" class="NoUnderLine">
                    <asp:LinkButton runat="server" ID="lbBtnContinue" Text="Continue to PackingList">
                    </asp:LinkButton>
                </div>
            </td>
        </tr>
        <tr style="display: none;">
            <td>
                <asp:Repeater ID="rptTBodyList" runat="server">
                    <HeaderTemplate>
                        <div class="aligntable" id="aligntabl1">
                            <table id="tblBodyList" cellpadding="0" cellspacing="0" border="0" class="display">
                                <thead>
                                    <tr>
                                        <th colspan="3">
                                            Description of Goods
                                        </th>
                                        <th>
                                            Quantity
                                        </th>
                                        <th>
                                            Unit Rate($)
                                        </th>
                                        <th>
                                            Amount($)
                                        </th>
                                    </tr>
                                </thead>
                                <tbody class="tbody">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="<%# Eval("asno") %>">
                            <td colspan="3" align="left">
                                <%# Eval("aDesc")%>
                            </td>
                            <td>
                                <%# Eval("aQty")%>
                            </td>
                            <td>
                                <%# Eval("aRate")%>
                            </td>
                            <td>
                                <%# Eval("aAmount")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody> </table></div>
                    </FooterTemplate>
                </asp:Repeater>
            </td>
        </tr>
    </table>
</asp:Content>
