<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequestForCngePwd.aspx.cs"
    Inherits="VOMS_ERP.Masters.RequestForCngePwd" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Volta Impex Pvt Ltd</title>
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
    <link rel="stylesheet" type="text/css" href="../css/login.css" />
    <link rel="stylesheet" type="text/css" href="../css/messages.css" />
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_filter
        {
            visibility: visible !important;
        }
    </style>
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
    <script type="text/javascript" language="javascript">
        var k = 0;
        function showHideMessage() {
            var selectedEffect = 'blind';
            var options = {};
            if (k == 0) {
                document.getElementById("spnShowHide").innerHTML = 'Hide Message';
                document.getElementById("imgShowHide").src = 'Images/sort_asc.gif';
                document.getElementById("box-title").style.display = "";
                k = 1;
            }
            else {
                document.getElementById("spnShowHide").innerHTML = 'Show Message';
                document.getElementById("imgShowHide").src = 'Images/sort_desc.gif';
                document.getElementById("box-title").style.display = "none";
                k = 0;
            }
            $("#box-title").toggle(selectedEffect, options, 500);
        }
        function UseNewPWD() {
            var CrntPWD = $("#<%=txtCrntPwd.ClientID%>")[0].value;
            var NewPWD = $("#<%=txtNwPwd.ClientID%>")[0].value;
            if (CrntPWD == NewPWD) {
                $("#<%=txtNwPwd.ClientID%>").focus();
                ErrorMessage('Don&#39;t Use Current password as New Password.');
                $('[id$=txtNwPwd]').val('');
                $('[id$=txtNwPwd]').focus();
                return false;
            }
        }
        function CheckPwd(PwdFld) {
            var obj1 = ($('[id$=hdnfldpwd]').val()).trim();
            var obj2 = PwdFld.value;
            var result = RequestForCngePwd.ComparePWd(obj2, obj1);
            if (result.value == false) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Current Password is not Matched.</span>');
                $('[id$=txtCrntPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            return true;
        }
        function Myvalidations() {
            if (($('[id$=txtCrntPwd]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Current Password is Required.</span>');
                $('[id$=txtCrntPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtNwPwd]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">New Password is Required.</span>');
                $('[id$=txtNwPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtCfmPwd]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Confirm Password is Required.</span>');
                $('[id$=txtCfmPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtCfmPwd]').val()).trim() != ($('[id$=txtNwPwd]').val()).trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Confirm Password Mis-Matched.</span>');
                $('[id$=txtCfmPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlSQtn1]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Security Question 1 is Required.</span>');
                $('[id$=ddlSQtn1]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtAnsQ1]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Answer 1 is Required.</span>');
                $('[id$=txtAnsQ1]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlSQtn2]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Security Question 2 is Required.</span>');
                $('[id$=ddlSQtn2]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtAnsQ2]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Answer 2 is Required.</span>');
                $('[id$=txtAnsQ2]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 106px;
            height: 78px;
        }
    </style>
</head>
<body id="login">
    <div class="login_min-header">
        <div class="login_header-one">
            <div id="banner" style="height: 75px;">
                <div class="logo">
                    <a href="#">Volta</a>
                </div>
                <div class="head_dis" style="display: none;">
                    <span>Volta Online Management System</span>
                </div>
                <div class="export_house" style="display: none;">
                    <span>Export House </span>
                </div>
            </div>
        </div>
    </div>
    <br />
    <form id="form2" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
    <div class="main_login_div">
        <div id="login-box">
            <div id="content">
                <asp:HiddenField runat="server" ID="hdnfldpwd" />
                <table style="width: 95%; height: 98%; vertical-align: middle;" align="center">
                    <tr>
                        <td class="bcTdNewTable">
                            <table style="width: 98%; vertical-align: middle" align="center">
                                <tr>
                                    <td class="bcTdTitleSpaceRow" colspan="6">
                                        <br />
                                    </td>
                                </tr>
                                <tr class="bcTRTitleRow">
                                    <td class="bcTdTitleLeft" align="left" colspan="6">
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Change Password" CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" style="vertical-align: middle;">
                                        </div>
                                        <div class="formError1" id="formError1" align="right" />
                                    </td>
                                </tr>
                                <tr style="width: 100%">
                                    <td colspan="6" style="text-align: right; font-size: 12px; font-family: Verdana;">
                                        All <font color="red" size="2"><b>*</b></font> fields are Mandatory
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" class="bcTdNewTable">
                                        <div style="border: 5px solid #9CB5CB; float: left; width: 100%; margin: 0px; height: 350px;">
                                            <div class="row" style="text-align: center; width: 97%; padding: 5px; height: 20px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Label13" class="bcLabelright">User Name<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <span style="font-size: 16px; font-family: Verdana;">
                                                        <asp:Label runat="server" ID="lblUsrID" Text="User Namek"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; padding: 5px 0px 10px 0px;
                                                margin-top: 10px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span1" class="bcLabelright">Current Password<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtCrntPwd" ValidationGroup="D" TextMode="Password"
                                                        class="bcAsptextbox" MaxLength="50" onblur="CheckPwd(this);"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row" style="height: 30px">
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; padding: 5px 0px 10px 0px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span2" class="bcLabelright">New Password<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtNwPwd" ValidationGroup="D" class="bcAsptextbox"
                                                        MaxLength="50" TextMode="Password" onchange="return UseNewPWD()"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row" style="height: 30px">
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; padding: 5px 0px 10px 0px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span3" class="bcLabelright">Confirm Password<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtCfmPwd" ValidationGroup="D" class="bcAsptextbox"
                                                        TextMode="Password" MaxLength="50"></asp:TextBox><%--onchange="ConformPWD()"--%>
                                                </div>
                                            </div>
                                            <div class="row" style="height: 30px">
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; padding: 5px 0px 10px 0px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span4" class="bcLabelright">Security Question 1<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:DropDownList runat="Server" ID="ddlSQtn1" onchange="javascript:RemoveDropDownList('ddlSQtn1', 'RequestForCngePwd.aspx', 'ddlSQtn2')">
                                                        <asp:ListItem Text="Select Question" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row" style="height: 20px">
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; padding: 5px 0px 10px 0px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span6" class="bcLabelright">Answer<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtAnsQ1" ValidationGroup="D" class="bcAsptextbox"
                                                        MaxLength="200"></asp:TextBox><%--onchange="ConformPWD()"--%>
                                                </div>
                                            </div>
                                            <div class="row" style="height: 30px">
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; padding: 5px 0px 10px 0px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span5" class="bcLabelright">Security Question 2<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:DropDownList runat="Server" ID="ddlSQtn2">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row" style="height: 20px">
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; padding: 5px 0px 10px 0px;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span7" class="bcLabelright">Answer<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtAnsQ2" ValidationGroup="D" class="bcAsptextbox"
                                                        MaxLength="200"></asp:TextBox><%--onchange="ConformPWD()"--%>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" colspan="3">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle">
                                                        <div class="loginbtndiv">
                                                            <a href="javascript:">
                                                                <asp:Button ID="btnSave" runat="server" CssClass="lgnbtn" ToolTip="Submit" Text="Submit"
                                                                    ValidationGroup="submit" OnClick="btnSave_Click" /></a>
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle">
                                                        <div class="loginbtndiv">
                                                            <a href="javascript:">
                                                                <asp:Button ID="btnClear" runat="server" CssClass="lgnbtn" ToolTip="Clear" Text="Clear"
                                                                    OnClick="btnClear_Click" /></a>
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle">
                                                        <div class="loginbtndiv">
                                                            <a href="javascript:">
                                                                <asp:Button ID="btnExit" runat="server" CssClass="lgnbtn" ToolTip="SignOut" Text="SignOut"
                                                                    OnClick="btnExit_Click" /></a>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" class="bcTdNewTable">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
