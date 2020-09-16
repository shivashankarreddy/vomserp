<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermsNConditions.aspx.cs"
    Inherits="VOMS_ERP.Masters.TermsNConditions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Volta Impex Pvt Ltd</title>
    <link rel="stylesheet" type="text/css" href="~/css/style.css" />
    <link href="~/css/messages.css" rel="stylesheet" type="text/css" />
</head>
<body onload="GetTArea()">
    <form id="form1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Terms & Conditions"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td colspan="3" class="bcTdNewTable">
                                        <div style="width: 900px; height: 300px; overflow: auto;">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td class="bcTdnormal">
                                                        <asp:GridView runat="server" ID="gvTmsCndtns" AutoGenerateColumns="False" Width="100%"
                                                            RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                                            OnRowDataBound="gvTmsCndtns_RowDataBound">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Select">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox runat="server" ID="ChkbItm" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="S.No.">
                                                                    <ItemTemplate>
                                                                        <%# Container.DataItemIndex+1 %>
                                                                        <asp:Label ID="lblSerialNo" runat="server"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Title">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblttl" runat="server" Text='<%#Eval("Title") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Description">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtdecrp" TextMode="MultiLine" Height="44px" Width="625px" runat="server"
                                                                            MaxLength="500" Text='<%#Eval("Description") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Terms Master" Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTrmID" runat="server" Text='<%#Eval("TermsID") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10px" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="3">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div1" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div2" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div3" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnExit" Text="Exit" PostBackUrl="~/Masters/Home.aspx"
                                                                OnClientClick="javascrip:self.close();" OnClick="btnExit_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField runat="server" ID="hdfldTArea" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="~/JScript/JScript.js" type="text/javascript"></script>
    <script src="~/JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="~/JScript/jquery.1.4.2.js" type="text/javascript"></script>
    <script src="~/JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="~/JScript/jquery.json-2.3.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_filter
        {
            visibility: visible !important;
        }
    </style>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        function SuccessMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Success">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }
        function ErrorMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Error">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }
    </script>
    <script type="text/javascript">
        function Myvalidations() {

            //            var Tcs = $('[id$=gvTmsCndtns]')[0].rows.length
            //            if (Tcs == 0) {
            //                $("#<%=divMyMessage.ClientID %> span").remove();
            //                $('[id$=divMyMessage]').append('<span class="Error">No Items to Save.</span>');
            //                $('[id$=gvTmsCndtns]').focus();
            //                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            //                return false;
            //            }
            //            else if (Tcs > 0) {
            //                if (Tcs == 1) {
            //                    $("#<%=divMyMessage.ClientID %> span").remove();
            //                    $('[id$=divMyMessage]').append('<span class="Error">No Items to Save.</span>');
            //                    $('[id$=gvTmsCndtns]').focus();
            //                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            //                    return false;
            //                }
            //                else {
            //                    var select = 0;
            //                    for (var i = 2; i <= Tcs; i++) {
            //                        var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
            //                        var chkbval = GetClientID(chkbx + "_ChkbItm").attr("id");
            //                        if ($('#' + chkbval)[0].checked) {
            //                            select = 1;
            //                        }
            //                    }
            //                    if (select == 0) {
            //                        $("#<%=divMyMessage.ClientID %> span").remove();
            //                        $('[id$=divMyMessage]').append('<span class="Error">Select At Least One Item.</span>');
            //                        $('[id$=gvTmsCndtns]').focus();
            //                        $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            //                        return false;
            //                    }
            //                    else
            //                        return true;
            //                }
            //            }

        }
        function GetTArea() {
            var TArea = window.dialogArguments;
            document.getElementById("hdfldTArea").value = TArea;
        }
    </script>
    </form>
</body>
</html>
