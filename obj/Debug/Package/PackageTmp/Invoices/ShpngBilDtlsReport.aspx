<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ShpngBilDtlsReport.aspx.cs" Inherits="VOMS_ERP.Invoices.ShpngBilDtlsReport"  %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" align="center" colspan="2">
                <CR:CrystalReportViewer ID="ShippingBillDtls" runat="server" AutoDataBind="true"
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
                    <asp:LinkButton runat="server" ID="lbBtnContinue" Text="">
                    </asp:LinkButton>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
