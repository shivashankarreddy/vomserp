<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UC_GDN_Approve.ascx.cs"
    Inherits="VOMS_ERP.Logistics.UC_GDN_Approve" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Panel runat="server" ID="pnlPopup" BackColor="#F4E4CF" BorderColor="#B7B7B7"
    BorderStyle="Solid" BorderWidth="3px" CssClass="PopUpStyle">
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="pnlPopup"
        BackgroundCssClass="modalBackground" TargetControlID="hf1">
    </asp:ModalPopupExtender>
    <table width="100%">
        <tr>
            <td align="right">
                <asp:ImageButton ID="btnClose" runat="server" ImageUrl="~/img/closeButton_normal.gif"
                    OnClick="btnClose_Click" />
                <asp:HiddenField ID="hf1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <div id="divMyMessage1" runat="server" align="center" class="formError1" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td colspan="2">
                <center>
                    <h3>
                        Do you want to Reject this GDN if so Please Comment</h3>
                </center>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center" valign="middle">
                Comment
                <asp:Label ID="lblStar" runat="server" Text="*" ToolTip="Field is Mandatory" Font-Bold="true"
                    ForeColor="Red"></asp:Label>
                :&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" CssClass="bcAsptextboxmulti" ToolTip="Field is Mandatory"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                &nbsp;&nbsp;
                <asp:Button ID="btnTerminate" runat="server" Text="Reject" OnClientClick="return PopUpValidation()"
                    OnClick="btnTerminate_Click" />
            </td>
        </tr>
    </table>

    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        function PopUpValidation() {
            if (($('[id$=txtComment]').val()).trim() == '') {
                ErrorMessage1('Comment is required.');
                return false;
            }
            else
                return true;
        }

        function ErrorMessage1(msg) {
            $("#<%=divMyMessage1.ClientID %> span").remove();
            $('[id$=divMyMessage1]').append('<span class="Error">' + msg + '</span>');
            $('[id$=divMyMessage1]').fadeTo(2000, 1).fadeOut(3000);
        }
    </script>

</asp:Panel>
