<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true"
    CodeBehind="FQDetails_Customer.aspx.cs" Inherits="VOMS_ERP.Customer_Access.FQDetails_Customer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" align="center" colspan="2">
                <CR:CrystalReportViewer ID="FQuotationDtls" runat="server" AutoDataBind="true" HasCrystalLogo="False"
                    HasToggleGroupTreeButton="False" Height="50px" PrintMode="ActiveX" ToolPanelView="None"
                    Width="350px" EnableDatabaseLogonPrompt="False" EnableParameterPrompt="False"
                    HasDrilldownTabs="False" HasDrillUpButton="False" HasPrintButton="False" HasSearchButton="False"
                    HasToggleParameterPanelButton="False" HasZoomFactorList="False" />
                <br />
            </td>
        </tr>
        <tr>
            <td align="right" class="NoUnderLine" style="width: 18%">
                <asp:LinkButton runat="server" ID="lbtnBack" Text="Back">
                </asp:LinkButton>
            </td>
            <td align="right">
                <div style="padding-right: 20%;">
                    <asp:LinkButton runat="server" ID="lbtnCntnu" Text="Continue with FOREIGN QUOTATION COMPARISON">
                    </asp:LinkButton>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
