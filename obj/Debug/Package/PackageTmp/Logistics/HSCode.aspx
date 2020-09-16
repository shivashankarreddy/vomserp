<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HSCode.aspx.cs" Inherits="VOMS_ERP.Logistics.HSCode" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Volta Impex Pvt Ltd</title>
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/messages.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery-ui-1.9.1.custom.min.css" rel="stylesheet" type="text/css" />

    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        var SentValue = window.my_special_setting;
        $('[id$=txtHSCode]').val = window.my_special_setting;
    </script>

    <script type="text/javascript">
        function Exit() {
            window.close();
        }

        function SendSelectedVal() {
            window.returnValue = $('#HFSelectedVal').val(); window.close();
        }
    </script>    
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table class="MainTable" align="center">
            <tr>
                <td class="bcTdNewTable">
                    <table style="width: 98%;" align="center">
                        <tr class="bcTRTitleRow">
                            <td class="bcTdTitleLeft" align="left" colspan="6">
                            <asp:HiddenField ID="HFSelectedVal" runat="server" />
                                <table width="100%">
                                    <tr>
                                        <td>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="HSCode" CssClass="bcTdTitleLabel"></asp:Label><div
                                                id="divMyMessage" runat="server" align="center" class="formError1" />
                                        </td>
                                        <td style="text-align: right;" colspan="6">
                                            <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                                fields are Mandatory</span>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="width: 100%">
                        <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                            align="center">
                            <tr>
                                <td class="bcTdnormal">
                                    <span class="bcLabel">Item Description.<font color="red" size="2"><b>*</b></font>:</span>
                                </td>
                                <td class="bcTdnormal">
                                    <asp:DropDownList runat="server" ID="ddlItmDescription" CssClass="bcAspdropdown" Visible="false"
                                        Height="22px" Enabled="false" OnSelectedIndexChanged="ddlItmDescription_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:TextBox runat="server" ID="txtSpec" TextMode="MultiLine" CssClass="bcAsptextboxmulti" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td class="bcTdnormal">
                                    <span class="bcLabel">HSCode<font color="red" size="2"><b>*</b></font>:</span>
                                </td>
                                <td class="bcTdnormal">
                                    <asp:TextBox runat="server" ID="txtHSCode" CssClass="bcAsptextbox"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                        align="center">
                        <tr>
                            <td class="bcTdNewTable" colspan="6">
                            </td>
                        </tr>
                        <tr>
                            <td align="center" class="bcTdNewTable" colspan="6">
                                <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                    <tbody>
                                        <tr align="center" valign="middle">
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div1" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClientClick="javascript:validations()"
                                                        OnClick="btnSave_Click" />
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div2" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnClear" OnClientClick="Javascript:clearAll()"
                                                        Text="Clear" />
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div3" class="bcButtonDiv">
                                                    <a title="Exit" class="bcAlink" onclick="javascript:Exit()">Exit </a>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
