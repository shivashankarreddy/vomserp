<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="VOMS_ERP.Masters.ChangePassword"
     %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr class="bcTRTitleRow">
            <td class="bcTdNewTable">
                <table style="width: 100%;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Change Password" CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
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
                <table style="width: 100%; background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                    <tr>
                        <td class="bcTdNewTable">
                            <table style="width: 100%; background-color: #F5F4F4; vertical-align: top" align="center">
                                <tr>
                                    <td colspan="3" class="bcTdNewTable">
                                        <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                            width: 98%; margin: 5px; height: 99%;">
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Label13" class="bcLabelright">User Name<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <span>
                                                        <asp:DropDownList runat="server" ID="ddlUsrNm" CssClass="bcAspdropdown">
                                                            <%--<asp:ListItem Text="Select User" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="kumar@gmail.com" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="ram@ymail.com" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="raj@hotmail.com" Value="3"></asp:ListItem>--%>
                                                        </asp:DropDownList>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span1" class="bcLabelright">Current Password<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtCrntPwd" ValidationGroup="D" TextMode="Password"
                                                        class="bcAsptextbox" onchange="CheckMailID()" MaxLength="50"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span2" class="bcLabelright">New Password<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtNwPwd" ValidationGroup="D" class="bcAsptextbox"
                                                        MaxLength="50" TextMode="Password" onchange="return UseNewPWD()"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span3" class="bcLabelright">Confirm Password<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:TextBox runat="server" ID="txtCfmPwd" ValidationGroup="D" class="bcAsptextbox"
                                                        TextMode="Password" MaxLength="50"></asp:TextBox><%--onchange="ConformPWD()"--%>
                                                </div>
                                            </div>
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
                                                            <asp:LinkButton runat="server" ID="btnChange" Text="Change" OnClick="btnChange_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div2" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                                            <%--OnClientClick="Javascript:clearAll()"--%>
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
                                <tr>
                                    <td colspan="3" class="bcTdNewTable">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" class="bcTdNewTable">
                                        <center>
                                            <%--                               <asp:GridView runat="server" ID="gvdept" AutoGenerateColumns="False" 
                                    RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                    PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                    CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle">
                                    <Columns>
                                        <asp:TemplateField HeaderText="S.No.">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                                <asp:Label ID="lblSerialNo" runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="10px" />
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Department Name" DataField="Department Name" HeaderStyle-Width="170px" />
                                        <asp:TemplateField HeaderText="Id" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblID" Text='<%#Eval("Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                            Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Delete"
                                            Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                    </Columns>
                                </asp:GridView>--%>
                                        </center>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        // This is to check Current Password
        function CheckMailID() {
            $.ajax({
                type: "POST",
                url: "ChangePassword.aspx/CheckPwd",
                data: '{PWD: "' + $("#<%=txtCrntPwd.ClientID%>")[0].value + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                    alert(response);
                }
            });
            function OnSuccess(response) {
                if (response != null) {
                    if (response.d == "False") {
                        ErrorMessage('Invalid Current Password.');
                        $("#<%=txtCrntPwd.ClientID%>")[0].value = '';
                        $("#<%=txtCrntPwd.ClientID%>")[0].focus();
                    }
                }
            }
        }


        // This is used to check ConformPWD
        function ConformPWD() {
            var NewPWD = $("#<%=txtNwPwd.ClientID%>")[0].value;
            var NewCPWD = $("#<%=txtCfmPwd.ClientID%>")[0].value;
            if (NewPWD != NewCPWD) {
                $("#<%=txtCfmPwd.ClientID%>")[0].value = '';
                $("#<%=txtCfmPwd.ClientID%>")[0].focus();
                alert('New Password Didnot Match.');
            }
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

        function Myvalidations() {
            if ($('[id$=txtCrntPwd]').val() != $('[id$=txtCrntPwd]').val()) {
                ErrorMessage('Does Not Match.');
                $('[id$=txtCfmPwd]').val('');
                $('[id$=txtCfmPwd]').focus();
                return false;
            }
            if ($('[id$=txtCrntPwd]').val().trim() == '') {
                ErrorMessage('Current password is Required.');
                $('[id$=txtCrntPwd]').focus();
                return false;
            }
            if ($('[id$=txtNwPwd]').val().trim() == '') {
                ErrorMessage('New Password is Required.');
                $('[id$=txtNwPwd]').focus();
                return false;
            }
            if ($('[id$=txtCfmPwd]').val().trim() == '') {
                ErrorMessage('New Conform Password is Required.');
                $('[id$=txtCfmPwd]').focus();
                return false;
            }
            if ($('[id$=txtNwPwd]').val() != $('[id$=txtCfmPwd]').val()) {
                ErrorMessage('New Password Does Not Match.');
                $('[id$=txtCfmPwd]').val('');
                $('[id$=txtCfmPwd]').focus();
                return false;
            }
            else {
                return true;
            }
        }
    </script>
</asp:Content>
