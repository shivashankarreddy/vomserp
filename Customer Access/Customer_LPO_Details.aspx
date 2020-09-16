<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true" CodeBehind="Customer_LPO_Details.aspx.cs" Inherits="VOMS_ERP.Customer_Access.Customer_LPO_Details" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" align="center" colspan="2">
                <CR:CrystalReportViewer ID="LclPurchaseOrderDtls" runat="server" AutoDataBind="true"
                    GroupTreeStyle-ShowLines="False" HasCrystalLogo="False" HasDrilldownTabs="False"
                    HasDrillUpButton="False" HasPageNavigationButtons="True" HasPrintButton="False"
                    HasToggleGroupTreeButton="False" HasToggleParameterPanelButton="False" ToolPanelView="None" />
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
            <asp:HiddenField ID="hfPath" runat="server" Value="RDLC" />
                <table width="100%">
                    <tr>
                        <td align="right" class="NoUnderLine" style="width: 18%">
                            <asp:LinkButton runat="server" ID="lbtnBack" Text="Back">
                            </asp:LinkButton>
                        </td>
                        <td align="right">
                            <div style="padding-right: 20%; display: block;">
                                <asp:LinkButton runat="server" ID="lbtnCntnu" Text="Continue with DISPATCH INSTRUCTIONS">
                                </asp:LinkButton>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
