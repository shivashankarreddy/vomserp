<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="DspchInstRpt.aspx.cs" Inherits="VOMS_ERP.Logistics.DspchInstRpt"
    %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" align="center" colspan="2">
                <CR:CrystalReportViewer ID="DispatchInstructionsDtls" runat="server" AutoDataBind="true"
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
                        <td align="right" class="NoUnderLine" style="width: 18%">
                            <asp:LinkButton runat="server" ID="lbtnBack" Text="Back">
                            </asp:LinkButton>
                        </td>
                        <td align="right">
                            <div style="padding-right: 20%; display: block;">
                                <asp:LinkButton runat="server" ID="lbtnCntnu">
                                </asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbContinueGRN">
                                </asp:LinkButton>
                            </div>
                        </td>
                        <td align="right">
                            <div style="padding-right: 20%; display: block;">
                                

                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
