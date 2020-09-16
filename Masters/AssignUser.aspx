<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    MaintainScrollPositionOnPostback="true" CodeBehind="AssignUser.aspx.cs" Inherits="VOMS_ERP.Masters.AssignUser"
     %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Assign Users" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span6" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Label13" class="bcLabelright">Category <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCtgry" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlCtgry_SelectedIndexChanged">
                                            
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabelright">User ID <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlUsers" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlUsers_SelectedIndexChanged">
                                            
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabelright">Designation <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:Label runat="server" ID="lblDsgntn" Text="" Visible="False" CssClass="bcLabel"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlDsgntn" CssClass="bcAspdropdown">
                                           
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabelright">Password <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPwd" CssClass="bcAsptextbox" TextMode="Password"
                                            MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabelright">Confirm Password <font color="red" size="2"><b>
                                            *</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCfmPwd" CssClass="bcAsptextbox" TextMode="Password"
                                            MaxLength="50" onchange="return ConfirmPassword()"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="SpnRole" class="bcLabelright">
                                            <asp:Label ID="lblRoleType" runat="server" Visible="false">Role<font color="red" size="2"><b>*</b></font>:</asp:Label>
                                        </span><span id="Span5" class="bcLabelright">
                                            <asp:Label ID="lblCustomer" runat="server" Visible="false">Company Name<font color="red" size="2"><b>*</b></font>:</asp:Label>
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--<asp:Label runat="server" ID="lblRole" Text="" Visible="False" CssClass="bcLabel"></asp:Label>--%>
                                        <asp:DropDownList runat="server" ID="ddlRole" Visible="false" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Role" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:ListBox ID="LBCustomer" runat="server" AutoPostBack="true" SelectionMode="Multiple"
                                            CssClass="bcAspMultiSelectListBox" Visible="false"></asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" align="right">
                                        <center>
                                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                                <tbody>
                                                    <tr valign="middle">
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div1" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnsave_Click" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div2" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnclear" OnClientClick="Javascript:clearAll()"
                                                                    Text="Clear" OnClick="btnclear_Click" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div3" class="bcButtonDiv">
                                                                <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </center>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;<asp:HiddenField runat="server" ID="hdnfldCngPwd" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:GridView runat="server" ID="gvUsers" AutoGenerateColumns="False" RowStyle-CssClass="bcGridViewRowStyle"
                                            EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                            PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                            AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" DataKeyNames="ID"
                                            Width="100%" OnRowCommand="gvUsers_RowCommand" OnRowDataBound="gvUsers_RowDataBound"
                                            OnRowDeleting="gvUsers_RowDeleting" OnPreRender="gvUsers_PreRender">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No.">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ID" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ID" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUsrID" runat="server" Text='<%# Eval("UserID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Category">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCtype" runat="server" Text='<%# Eval("ContactType") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="User ID" DataField="EmailID" HeaderStyle-Width="470px" />
                                                <asp:BoundField HeaderText="Designation" DataField="DesignationID" HeaderStyle-Width="270px" />
                                                <asp:BoundField HeaderText="Role" DataField="RoleID" HeaderStyle-Width="270px" />
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                    Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" Visible="true" />
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Delete"
                                                    Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
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
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
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
        $(document).ready(function () {
            $("[id$=gvUsers]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search:"
                },
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
        });       
    </script>
    <script type="text/javascript">

        function ConfirmPassword() {
            if (($('[id$=txtPwd]').val()).trim() != ($('[id$=txtCfmPwd]').val()).trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Confirm Password Mis-Match.</span>');
                $('[id$=txtCfmPwd]').val('');
                $('[id$=txtCfmPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else
                return true;
        }

        function Myvalidations() {

            if (($('[id$=ddlCtgry]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Category is Required.</span>');
                $('[id$=ddlCtgry]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            if (($('[id$=ddlUsers]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">User ID is Required.</span>');
                $('[id$=ddlUsers]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            if (($('[id$=btnSave]').val()).trim() == 'Save') {
                if (($('[id$=ddlDsgntn]').val()).trim() == '0') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Designation is Required.</span>');
                    $('[id$=ddlDsgntn]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
            }
            if (($('[id$=txtPwd]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Passwored is Required.</span>');
                $('[id$=txtPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            if (($('[id$=txtCfmPwd]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Confirm password is Required.</span>');
                $('[id$=txtCfmPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            if (($('[id$=btnSave]').val()).trim() == 'Save') {
                if (($('[id$=ddlRole]').val()).trim() == '0') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Role is Required.</span>');
                    $('[id$=ddlRole]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
            }
            if (($('[id$=txtPwd]').val()).trim() != ($('[id$=txtCfmPwd]').val()).trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Confirm Password Mis-Match.</span>');
                $('[id$=txtCfmPwd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            return true;
        }

    </script>
</asp:Content>
