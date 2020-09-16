<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="TeamMaster.aspx.cs" Inherits="VOMS_ERP.Masters.TeamMaster"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Team/Group Master"
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
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <span id="Label13" class="bcLabelright">Team Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTmNm" ValidationGroup="D" class="bcAsptextbox"
                                            MaxLength="150" onkeypress="return CheckSpeclChars(event);" onchange="CheckTeamName();"></asp:TextBox>
                                            <%--SearchTeamNm();--%>
                                    </td>
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <span id="Span1" class="bcLabelright">Team Lead<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlTmLead" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <font color="red" size="4"><b></b></font><span id="Span3" class="bcLabelright">Parent
                                            Name : </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlTmNm" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="6" class="bcTdNewTable">
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
                                        <asp:GridView runat="server" ID="gvTmMstr" AutoGenerateColumns="False" RowStyle-CssClass="bcGridViewRowStyle"
                                            EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                            PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                            AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" DataKeyNames="TeamID"
                                            Width="100%" OnRowCommand="gvTmMstr_RowCommand" OnRowDeleting="gvTmMstr_RowDeleting"
                                            OnRowDataBound="gvTmMstr_RowDataBound" OnPreRender="gvTmMstr_PreRender" EmptyDataText="No Records to Display">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." HeaderStyle-Width="10px">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSNO" runat="server" Text='<%# Eval("TeamID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Team Name" DataField="TeamName" HeaderStyle-Width="170px" />
                                                <asp:BoundField HeaderText="Team Lead" DataField="TeamLead" HeaderStyle-Width="170px" />
                                                <asp:BoundField HeaderText="Parent Team" DataField="ParentTeamID" HeaderStyle-Width="170px" />
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                    Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Delete"
                                                    Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
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
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvTmMstr]").dataTable({
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
            oTable = $("[id$=gvTmMstr]").dataTable();
        });
        function SearchTeamNm() {
            var value1 = $('[id$=txtTmNm]').val();
            oTable.fnFilter(value1, 1);
            if ($('[id$=gvTmMstr] >tbody >tr >td').length > 1) {
                ErrorMessage('Team Name already Exists');
                $('[id$=txtTmNm]').val('');
            }
            oTable.fnFilter('', 1);
        }          
    </script>
    <script type="text/javascript">
        function Myvalidations() {
            if (($('[id$=txtTmNm]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Team Name is Required.</span>');
                $('[id$=txtTmNm]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlTmLead]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Team Lead is Required.</span>');
                $('[id$=ddlTmLead]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }

        function CheckSpeclChars(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 38 && charCode != 45 &&
            (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        // This is to check TeamName
        function CheckTeamName() {
            $.ajax({
                type: "POST",
                url: "TeamMaster.aspx/CheckTeamName",
                data: '{TeamName: "' + $("#<%=txtTmNm.ClientID%>")[0].value + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                    ErrorMessage(response);
                }
            });
            function OnSuccess(response) {
                if (response != null) {
                    if (response.d == "False") {
                        ErrorMessage('Team Name Exists');
                        $("#<%=txtTmNm.ClientID%>")[0].value = '';
                        $("#<%=txtTmNm.ClientID%>")[0].focus();
                    }
                }
            }
        }
    </script>
</asp:Content>
