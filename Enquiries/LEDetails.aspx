<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="LEDetails.aspx.cs" Inherits="VOMS_ERP.Enquiries.LEDetails"  %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <center>
        <table class="MainTable" align="center">
            <tr>
                <td class="bcTdNewTable" align="center" colspan="2">
                    <cr:crystalreportviewer id="CrystalReportViewer1" runat="server" autodatabind="true"
                        grouptreestyle-showlines="False" hascrystallogo="False" hasdrilldowntabs="False"
                        hasdrillupbutton="False" haspagenavigationbuttons="True" hasprintbutton="False"
                        hastogglegrouptreebutton="False" hastoggleparameterpanelbutton="False" 
                        toolpanelview="None" EnableDatabaseLogonPrompt="False" />
                </td>
            </tr>
            <tr>
                <td align="right" class="NoUnderLine" style="width: 18%">
                    <asp:LinkButton runat="server" ID="lbtnBack" Text="Back">
                    </asp:LinkButton>
                </td>
                <td align="right">
                    <div style="padding-right: 20%;" class="NoUnderLine">
                        <asp:LinkButton runat="server" ID="lbtnCntnu" Text="Continue with LOCAL QUOTATION">
                        </asp:LinkButton>
                    </div>
                   
                </td>
            </tr>
        </table>
    </center>
</asp:Content>
