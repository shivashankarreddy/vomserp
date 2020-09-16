<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="SevottamDetails.aspx.cs" Inherits="VOMS_ERP.Logistics.SevottamDetails"  %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" align="center" colspan="2">
                 <CR:CrystalReportViewer ID="SevottamDtls" runat="server" AutoDataBind="true"
                    HasCrystalLogo="False" HasToggleGroupTreeButton="False" Height="50px" PrintMode="ActiveX"
                    ToolPanelView="None" Width="350px" EnableDatabaseLogonPrompt="False" EnableParameterPrompt="False"
                    HasDrilldownTabs="False" HasDrillUpButton="False" HasPrintButton="False" HasSearchButton="False"
                    HasToggleParameterPanelButton="False" HasZoomFactorList="False" />
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                <table width="100%">
                    <tr>
                        <td style="width: 20%">
                            &nbsp;
                        </td>
                        <td align="left" class="NoUnderLine" style="width: 80%">
                            <asp:LinkButton runat="server" ID="lbtnBack" Text="Back">
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
