<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="ManageUsers.aspx.cs" Inherits="VOMS_ERP.Masters.ManageUsers"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
            //without passing class names.
            $("[id$=gvUsrLst]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[1, "asc"]],
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
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Manage Users" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr class="bcTdNewTable">
                                    <td>
                                        <span id="Span1" class="bcLabelright">Category<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlCtgry" CssClass="bcAspdropdown" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlCtgry_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="--Select Category Type--"></asp:ListItem>
                                            <%--<asp:ListItem Value="1" Text="Volta"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="Customer"></asp:ListItem>
                                            <asp:ListItem Value="3" Text="Supplier"></asp:ListItem>--%>
                                        </asp:DropDownList>
                                    </td>
                                    <td colspan="3" style="width: 30%">
                                        &nbsp;
                                    </td>
                                    <td style="text-align: right;">
                                        <div style="width: 350px; height: 20px; text-align: right;">
                                            <span style="width: 320px; text-align: right;">
                                                <asp:Image ID="imgactive1" runat="server" ImageUrl="~/images/Userstatus_Green.PNG" />&nbsp;&nbsp;<font
                                                    style="font-family: Verdana; font-size: 12px;">Active</font> </span><span>
                                                        <asp:Image ID="imgdeact1" runat="server" ImageUrl="~/images/userstatus_icon.PNG" />&nbsp;&nbsp;<font
                                                            style="font-family: Verdana; font-size: 12px;">DeActive</font></span>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:GridView runat="server" ID="gvUsrLst" AutoGenerateColumns="False" RowStyle-CssClass="bcGridViewRowStyle"
                                            EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                            PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                            AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" DataKeyNames="ID"
                                            Width="100%" OnRowCommand="gvUsrLst_RowCommand" OnRowDataBound="gvUsrLst_RowDataBound"
                                            OnPreRender="gvUsrLst_PreRender">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No.">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="UserID" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Name" DataField="FullName" HeaderStyle-Width="170px" />
                                                <asp:TemplateField HeaderText="Role">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblRoleName" Text='<%#Eval("RoleName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status" HeaderStyle-Width="50px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("IsActive") %>' Visible="false"></asp:Label>
                                                        <asp:Image runat="server" ID="imgSmbl" ImageUrl='' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:ButtonField ButtonType="Link" CommandName="Active" Text="Active" ShowHeader="true"
                                                    HeaderStyle-Width="20px" />
                                                <asp:ButtonField ButtonType="Link" CommandName="Deactive" Text="Deactive" ShowHeader="true"
                                                    HeaderStyle-Width="20px" />
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
</asp:Content>
