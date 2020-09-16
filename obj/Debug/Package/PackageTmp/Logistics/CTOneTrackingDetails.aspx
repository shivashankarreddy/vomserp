<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CTOneTrackingDetails.aspx.cs" Inherits="VOMS_ERP.Logistics.CTOneTrackingDetails" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        
        <tr>
            <td class="bcTdNewTable" align="center" colspan="3">
                <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true"
                    HasCrystalLogo="False" HasToggleGroupTreeButton="False" Height="50px" PrintMode="ActiveX"
                    ToolPanelView="None" Width="350px" EnableDatabaseLogonPrompt="False" EnableParameterPrompt="False"
                    HasDrilldownTabs="False" HasDrillUpButton="False" HasPrintButton="False" HasSearchButton="False"
                    HasToggleParameterPanelButton="False" HasZoomFactorList="False" />
            </td>
        </tr>
        <%--<tr>
            <td align="left" colspan="2">
                <table width="100%">
                    <tr>
                        <td style="width: 20%">
                            &nbsp;
                        </td>
                        <td align="left" class="NoUnderLine" style="width: 30%">
                            <asp:LinkButton runat="server" ID="lbtnBack" Text="Back">
                            </asp:LinkButton>
                        </td>
                        <td align="center" class="NoUnderLine" style="width: 50%">
                            <asp:LinkButton runat="server" ID="lbtnCntnu" Text="">
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>--%>
    </table>
</asp:Content>