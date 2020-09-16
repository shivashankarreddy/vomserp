<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FPODetails_Amndmnt.aspx.cs" Inherits="VOMS_ERP.Purchases.FPODetails_Amndmnt" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" colspan="2" align="center">
                <CR:CrystalReportViewer ID="FrnPurchaseOrderDtls" runat="server" AutoDataBind="true"
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
                <div style="padding-right: 20%; display: block;">
                    <asp:LinkButton runat="server" ID="lbtnCntnu" Text="Continue with LOCAL PURCHASE ORDER" Visible ="false">
                    </asp:LinkButton>
                </div>
            </td>
        </tr>
    </table>

</asp:Content>
