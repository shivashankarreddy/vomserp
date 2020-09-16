<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="VOMS_ERP.Login" %>

<meta http-equiv="Expires" content="0">
<meta http-equiv="Pragma" content="no-cache">
<meta http-equiv="Cache-Control" content="no-cache">
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Johnnie Walker</title>
    <link rel="stylesheet" type="text/css" href="css/style.css" />
    <link rel="stylesheet" type="text/css" href="css/login.css" />

    <%--<script src="JScript/AnJs.js" type="text/javascript"></script>--%>

    <script src="JScript/jquery-1.8.2.min.js" type="text/javascript"></script>

    <script src="JScript/JScript.js" type="text/javascript"></script>

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
    
    </script>

    <script type="text/javascript">
        function showfrgtpwddiv() {
            $('#logindiv').hide();
            $('#frgtpwddiv').show();
            $('#useremail').val('').focus();
        }
        function showlogindiv() {
            $('#logindiv').show();
            $('#frgtpwddiv').hide();


            if ($('#username').val() == '') {
                $('#username').focus();
            } else {
                $('#password').val('').focus();
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
<body id="login" onpageshow="if (event.persisted) noBack();" onunload="" onload="javascript:noBack();">
    <div class="login_min-header">
        <div class="login_header-one">
            <div id="banner" style="height: 75px;">
                <div class="logo">
                    <a href="Login.aspx">Volta</a>
                </div>
                <div class="head_dis" style="display: none;">
                    <span>Volta Online Management System</span>
                </div>
                <div class="export_house" style="display: none;">
                    <span>Export House </span>
                </div>
                <div class="date" style="display: block;">
                    <span>Date :

                        <script type="text/javascript">
					<!--
                            var currentTime = new Date()
                            var month = currentTime.getMonth() + 1
                            var day = currentTime.getDate()
                            var year = currentTime.getFullYear()

                            document.write(day + "/" + month + "/" + year)
					//-->
                        </script>

                        Time :

                        <script type="text/javascript">
					<!--
                            var currentTime = new Date()
                            var hours = currentTime.getHours()
                            var minutes = currentTime.getMinutes()
                            document.write((hours > 12 ? hours - 12 : hours) + ":" + (minutes < 10 ? "0" + minutes : minutes) + " ")
                            document.write(hours > 11 ? "PM" : "AM")
					//-->

                        </script>

                    </span>
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
                <asp:HiddenField runat="server" ID="hdfldID" />
                <div class="left_img_admin">
                </div>
                <div id="login-form-wrap">
                    <div id="page-heading">
                    </div>
                    <div id="av-flash-block">
                    </div>
                    <div id="logindiv" style="margin-top: 100px;" >
                        <fieldset>
                            <br />
                            <asp:Label ID="loginerrormsg" runat="server" Text=" " ForeColor="Red" Font-Bold="true"></asp:Label>
                            <br />
                            <label for="username" style="font-size: 14px; font-weight: bold; font-family: Verdana;
                                text-transform: uppercase; color: #3F525F;">
                                User mail-ID</label>
                            <asp:TextBox ID="txtUser" runat="server" TabIndex="1" ></asp:TextBox>
                            <%--<p ng-bind = "TxtUs" style="color: #3F525F;"></p>--%>
                            <div id="password-group">
                                <label style="font-size: 14px; font-weight: bold; font-family: Verdana; text-transform: uppercase;
                                    color: #3F525F;" for="password">
                                    Password</label>
                                <asp:TextBox ID="txtPass" TextMode="Password" runat="server" TabIndex="2"></asp:TextBox><br />
                                <a style="float: left; margin-left: 195px; top: 15px; color: #1C6FAF; font-size: 11px;
                                    font-weight: bold; font-family: Verdana;" class="login-forgot-link" onclick="showfrgtpwddiv()"
                                    href="javascript:">Forgot Password?</a>
                            </div>
                            <br />
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/CImage.aspx" Visible="false" />
                            <br />
                            <asp:TextBox ID="txtimgcode" runat="server" Visible="false"></asp:TextBox>
                            <div class="loginbtndiv">
                                <a href="javascript:">
                                    <asp:Button ID="btnLogin" runat="server" CssClass="lgnbtn" Text="Login" ValidationGroup="submit"
                                        OnClick="btnSubmit_Click" /></a></div>
                            <div id="remember-me-group" style="display: none;">
                                <div class="dijitReset">
                                    <input type="checkbox" class="dijitReset" name="remember-me" value="1" id="remember-me"
                                        tabindex="0" /></div>
                                <label style="color: #666; font-size: 12px; font-family: Verdana;" class="inline-label"
                                    for="remember-me">
                                    Keep me signed in</label>
                            </div>
                        </fieldset>
                    </div>
                    <br />
                    <div id="frgtpwddiv" style="display: None;">
                        <%--<form method="POST" action="">--%>
                        <fieldset id="forgot-password">
                            <legend>password reset</legend>
                            <br />
                            <br />
                            <div id="divMyMessage" style="top: 177px; right: 300px;" runat="server" align="right"
                                class="formError1" />
                            <label class="" for="username">
                                your email</label>
                            <asp:TextBox ID="txtUserMail" runat="server" MaxLength="50" onblur="validateEmail(this);"></asp:TextBox><br />
                            <label class="" for="lblSQtn1" id="lblSQtn1">
                                Question 1</label>
                            <asp:TextBox ID="txtASQtn1" runat="server" onblur="validateAnswer(this, 1);"></asp:TextBox><br />
                            <label class="" for="lblSQtn2" id="lblSQtn2">
                                Question 2</label>
                            <asp:TextBox ID="txtASQtn2" runat="server" onblur="validateAnswer(this, 2);"></asp:TextBox><br />
                            <asp:Button ID="btnReSet" class="lgnbtn" Text="ReSet Password" runat="server" OnClick="btnReSet_Click" />
                            <%--onclick="btnReSet_Click"--%>
                            <a class="form-link" onclick="showlogindiv()" href="javascript:">« Login</a>
                        </fieldset>
                        <%--</form>--%>
                    </div>
                </div>
                <br class="clear" />
            </div>
            <br class="clear" />
        </div>
    </div>

    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        function Myvalidations() {
            if ($('#frgtpwddiv').show()) {
                if ($('[id$=txtUserMail]').val().trim() == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">User Name(E-Mail) is Required.</span>');
                    $('[id$=txtUserMail]').focus();
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    return false;
                }
                else if ($('[id$=txtASQtn1]').val().trim() == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Security Question 1 Answer is Required.</span>');
                    $('[id$=txtASQtn1]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
                else if ($('[id$=txtASQtn2]').val().trim() == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Security Question 2 Answer is Required.</span>');
                    $('[id$=txtASQtn2]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        // E-Mail Validation
        function validateEmail(emailField) {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            if (reg.test(emailField.value) == false) {
                SuccessMessage('Invalid E-Mail Format');
                return false;
            }
            else {
                var obj1 = emailField.value;
                var result = Login.CheckEmail(obj1);
                if (result.value == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">User Name/E-Mail is not Matched.</span>');
                    $('[id$=txtUserMail]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
                else {
                    var res = result.value.split('~')
                    $('[id$=hdfldID]').val(res[0]);
                    $('[id$=lblSQtn1]').html(res[1]);
                    $('[id$=lblSQtn2]').html(res[2]);

                }
            }
            return true;
        }
        function validateAnswer(ansField, i) {
            if (ansField.value != '') {
                var res = $('[id$=hdfldID]').val().split('#');
                if ((ansField.value).trim() == res[i].trim()) {
                    return true;
                }
                else {
                    ansField.focus();
                    ansField.value = '';
                    ErrorMessage('Your Answer is not Matched.')
                    return false;
                }
                return true;
            }
            return false;
        }
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

        document.getElementById('txtUser').focus();
    </script>

    </form>
</body>
</html>
