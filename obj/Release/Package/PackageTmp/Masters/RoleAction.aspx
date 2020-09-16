<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="RoleAction.aspx.cs" Inherits="VOMS_ERP.Masters.RoleAction"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hdfldbtnvalue" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Role Action" CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
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
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td width="12%">
                                        &nbsp;
                                    </td>
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <span id="Label13" class="bcLabelright">Role<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlTmMbrNm" CssClass="bcAspdropdown" onchange="javascript:SelectOne('ddlTmMbrNm','ddlTmNm');">
                                        </asp:DropDownList>
                                    </td>
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <span id="Span1" class="bcLabelright">Team<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlTmNm" CssClass="bcAspdropdown" onchange="javascript:SelectOne('ddlTmNm', 'ddlTmMbrNm');">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <center>
                                <table width="70%">
                                    <tr>
                                        <td>
                                            <asp:GridView runat="server" ID="gvRlActn" AutoGenerateColumns="False" RowStyle-CssClass="bcGridViewRowStyle"
                                                EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                                DataKeyNames="ID" PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain"
                                                HeaderStyle-CssClass="bcGridViewHeaderStyle" Width="100%" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                                OnRowDataBound="gvRlActn_RowDataBound" OnPreRender="gvRlActn_PreRender">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." HeaderStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Screen ID" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:HiddenField runat="server" ID="hdfldsrnid1" Value='<% #Eval("ID") %>' />
                                                            <asp:Label runat="server" ID="lblScreenID" Text='<%#Eval("ID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Screen/Page Name" HeaderStyle-Width="70px">
                                                        <ItemTemplate>
                                                            <asp:HiddenField runat="server" ID="hdfldsrnid" Value='<% #Eval("ID") %>' />
                                                            <asp:Label runat="server" ID="lblScreenNm" Text='<%#Eval("ScreenName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            Permissions [<input type="checkbox" id="all" value="NIL" onclick="SelectAll(this)">Select
                                                            All]
                                                        </HeaderTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
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
                                                    <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click"></asp:LinkButton>
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div2" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="LinkButton2" Text="Clear" onclick="LinkButton2_Click" />
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div3" class="bcButtonDiv">
                                                    <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit</a>
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
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvRlActn]").dataTable({
                "aLengthMenu": [[-1], ["All"]],
                "iDisplayLength": -1,
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
        function Myvalidations() {

            if (($('[id$=ddlTmMbrNm]').val()).trim() == '00000000-0000-0000-0000-000000000000' && ($('[id$=ddlTmNm]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Select Role/Team.</span>');
                $('[id$=ddlTmMbrNm]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }
        function SelectOne(Selectedone, Deselectone) {
            var obj = document.getElementById("ctl00_ContentPlaceHolder1_" + Selectedone).value;
            var obj1 = Selectedone == 'ddlTmMbrNm' ? 'RoleID' : 'GroupID'; var empty = 0;
            var grid = document.getElementById("ctl00_ContentPlaceHolder1_gvRlActn");
            var result = RoleAction.GetScreens(obj, obj1);
            if (result.value != null) {
                for (var i = 1; i < grid.rows.length; i++) {
                    for (var k = 0; k < result.value.Tables[0].Rows.length; k++) {
                        var scrnID = grid.rows[i].getElementsByTagName('input');
                        if (scrnID[0].type == "hidden") {
                            if (scrnID[0].value == result.value.Tables[0].Rows[k].ScreenID) {
                                var checkTr = result.value.Tables[0].Rows[k].PermissionID.split(',');
                                var cntls = grid.rows[i].getElementsByTagName('input');
                                var idval = i < 9 ? "0" + [i + 1] : i + 1;
                                for (var j = 0; j < cntls.length; j++) {
                                    if (cntls[j].type == "checkbox") {
                                        cntls[j].checked = ("ctl00_ContentPlaceHolder1_gvRlActn_ctl" + idval + "_" + checkTr[j - 1].toLowerCase()) == cntls[j].id ? true : false;
                                        empty = 1;
                                    }
                                }
                            }
                        }
                    }
                    if (empty != 1) {
                        var cntls = grid.rows[i].getElementsByTagName('input');
                        for (var j = 0; j < cntls.length; j++) {
                            if (cntls[j].type == "checkbox")
                                cntls[j].checked = false;
                            empty = 0;
                        }
                    }
                    else
                        empty = 0;
                }
                $('[id$=btnSave]').html('Update'); $('[id$=hdfldbtnvalue]').val('Update');
                document.getElementById("ctl00_ContentPlaceHolder1_" + Deselectone).value = 0;
                return true;
            }
            else {
                for (var i = 01; i < grid.rows.length; i++) {
                    var cntls = grid.rows[i].getElementsByTagName('input');
                    for (var j = 0; j < cntls.length; j++) {
                        if (cntls[j].type == "checkbox")
                            cntls[j].checked = false;
                    }
                }
                $('[id$=btnSave]').html('Save'); $('[id$=hdfldbtnvalue]').val('Save');
                document.getElementById("ctl00_ContentPlaceHolder1_" + Deselectone).value = 0;
                return false;
            }
        }
        function ClearAllChkb() {
            var inputs = document.getElementsByTagName('input');
            for (var k = 0; k < inputs.length; k++) {
                var input = inputs[k]
                if (input.type != 'checkbox') continue;
                input.checked = false;
            }
        }
       
    </script>
</asp:Content>
