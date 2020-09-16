<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UC_Action.ascx.cs" Inherits="VOMS_ERP.Logistics.UC_Action" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Panel runat="server" ID="pnlPopup" BorderStyle="Solid" CssClass="PopUpStyle"
    BorderWidth="3px">
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="pnlPopup" BackgroundCssClass="modalBackground"
        TargetControlID="hf1">
    </asp:ModalPopupExtender>    
    <table width="100%">
    <tr>
    <td align="right">
    <asp:HiddenField ID="hf1" runat="server" />
    <asp:ImageButton ID="btnClose" runat="server" 
            ImageUrl="~/img/closeButton_normal.gif" onclick="btnClose_Click" />
    </td>
    </tr>
    </table>
    <table width="100%">
        <tr>
            <td colspan="3">
                <center>
                    <h3>
                        Do you want to Re-applicable this CT-1 Form</h3>
                </center>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <center>
                    <h3>(OR)</h3>
                        <asp:HiddenField ID="HFPinvID" runat="server" />
                        <asp:HiddenField ID="HFCT1ID" runat="server" />
                </center>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <center>
                    <h3>
                        Terminate</h3>
                </center>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="SetStatus(Cancel)"
                    OnClick="btnCancel_Click" />
            </td>
            <td align="center">
                <asp:Button ID="btnReApplicable" runat="server" Text="Re-Applicable" OnClientClick="SetStatus(ReApplicable)"
                    OnClick="btnReApplicable_Click" />
            </td>
            <td align="center">
                <asp:Button ID="btnTerminate" runat="server" Text="Terminate" OnClientClick="SetStatus(Terminate)"
                    OnClick="btnTerminate_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
