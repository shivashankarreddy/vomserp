<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CToneReApplie.aspx.cs" Inherits="VOMS_ERP.Logistics.CToneReApplie" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RE-APPLICABLE/TERMINATE</title>
    <script type="text/javascript" src="../JScript/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" language="javascript">
        function CloseWindow() {
            window.close();
        }

        $(document).ready(function () {
            var qs = (function (a) {
                if (a == "") return {};
                var b = {};
                for (var i = 0; i < a.length; ++i) {
                    var p = a[i].split('=');
                    if (p.length != 2) continue;
                    b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
                }
                return b;
            })(window.location.search.substr(1).split('&'));
            $('#HFPinvID').val(qs["PinvID"].toString());
            $('#HFCT1ID').val(qs["CT1"].toString());

        });
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
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
    </form>
</body>
</html>
