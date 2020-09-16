<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="Mails.aspx.cs" Inherits="VOMS_ERP.Masters.Mails"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
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

        // E-Mail Validation
        function validateEmail(emailField) {
            var reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            if (reg.test(emailField.value) == false) {
                emailField.value = '';
                emailField.focus();
                alert('invalid Email-ID');
                return false;
            }
            return true;
        }        
    </script>
    <script type="text/javascript">
        function Myvalidations() {
            var res = $('[id$=txtfrom]').val();
            var res1 = $('[id$=txtpwd]').val();
            var res2 = $('[id$=txtto]').val();
            var res3 = $('[id$=txtsub]').val();

            if (res.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">From E-Mail is Required.</span>');
                $('[id$=txtfrom]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            if (res1.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Password is Required.</span>');
                $('[id$=txtpwd]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            if (res2.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">To E-Mail is Required.</span>');
                $('[id$=txtto]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            if (res3.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Subject is Required.</span>');
                $('[id$=txtsub]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }
    </script>
    <script type="text/javascript">
        function ErrorMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Error">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }
        function SuccessMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Success">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }
    </script>
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Mail Service" CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span7" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
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
                                    <td colspan="3">
                                        <div style="border: 0px solid #9CB5CB; float: left; background: #F5F4F4; padding: 5px;
                                            width: 98%; margin: 5px; height: 99%;">
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 34%;">
                                                    <span id="Label13" class="bcLabelright">From<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 65%;">
                                                    <span>
                                                        <asp:TextBox ID="txtfrom" runat="server" Width="30%" CssClass="bcAsptextbox" MaxLength="70"
                                                            onblur="validateEmail(this);"></asp:TextBox></span> <span>
                                                                <asp:RegularExpressionValidator ID="rev" runat="server" ErrorMessage=" Email Format"
                                                                    Display="Dynamic" ControlToValidate="txtfrom" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></span>
                                                    <span>
                                                        <asp:RequiredFieldValidator ID="rfv" runat="server" ControlToValidate="txtfrom" ValidationGroup="A"
                                                            ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator></span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 34%;">
                                                    <span id="Span1" class="bcLabelright">Password<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 65%;">
                                                    <asp:TextBox ID="txtpwd" runat="server" Width="30%" TextMode="Password" MaxLength="50"
                                                        CssClass="bcAsptextbox"></asp:TextBox>
                                                    <span>
                                                        <asp:RequiredFieldValidator ID="rfv1" runat="server" ControlToValidate="txtpwd" ValidationGroup="A"
                                                            ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator></span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 34%;">
                                                    <span id="Span2" class="bcLabelright">To<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 65%;">
                                                    <asp:TextBox ID="txtto" runat="server" Width="30%" MaxLength="150" CssClass="bcAsptextbox"
                                                        onblur="validateEmail(this);"></asp:TextBox>
                                                    <span>
                                                        <asp:RequiredFieldValidator ID="rfv2" runat="server" ControlToValidate="txtto" ValidationGroup="A"
                                                            ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator></span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 34%;">
                                                    <span id="Span3" class="bcLabelright">Add Cc :</span></div>
                                                <div style="text-align: left; width: 65%;">
                                                    <asp:TextBox ID="txtcc" runat="server" Width="30%" MaxLength="150" CssClass="bcAsptextbox"></asp:TextBox>
                                                    <span>
                                                        <asp:RegularExpressionValidator ID="rev2" runat="server" ErrorMessage=" Email Format"
                                                            ValidationGroup="A" ControlToValidate="txtcc" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 34%;">
                                                    <span id="Span4" class="bcLabelright">subject<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 65%;">
                                                    <asp:TextBox ID="txtsub" runat="server" MaxLength="250" Width="30%" Height="3%" CssClass="bcAsptextbox"></asp:TextBox></div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 34%;">
                                                    <span id="Span5" class="bcLabelright">Attach File(s) :</span></div>
                                                <div style="text-align: left; width: 65%;">
                                                    <asp:FileUpload ID="fileUpload1" runat="server" />
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; height: 30%;">
                                                <div style="text-align: right; width: 34%;">
                                                    <%--<span id="SpBody"></span>--%>
                                                    <span id="Span6" class="bcLabelright">Body :</span>
                                                </div>
                                                <div style="text-align: left; width: 65%; height: 88%">
                                                    <asp:TextBox ID="txtbody" runat="server"  Width="50%" TextMode="MultiLine" Height="100%"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <%--</div>--%>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="3">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div1" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnSend" Text="Send" OnClick="btnSend_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div2" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnClear" OnClientClick="Javascript:clearAll()"
                                                                Text="Clear" OnClick="btnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div3" class="bcButtonDiv">
                                                            <a href="../Masters/Home.aspx" title="Exit" class="bcAlink" onclick="javascript:Exit()">
                                                                Exit </a>
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
            </td>
        </tr>
    </table>
    <%--<script type="text/javascript">
        function Myvalidations() {
            var res = $('[id$=txtfrom]').val();
            if (res.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">From Mail is Required.</span>');
                $('[id$=txtfrom]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }

    </script>--%>
</asp:Content>
