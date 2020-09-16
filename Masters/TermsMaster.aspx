<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="TermsMaster.aspx.cs" Inherits="VOMS_ERP.Masters.TermsMaster"  %>

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
        function Myvalidations() {
            var res = $('[id$=txtTermNm]').val();
            var res1 = $('[id$=txtTrmVlu]').val();
            if (res.trim() == '') {
                ErrorMessage('Terms Name is Required.');
                $('[id$=txtTermNm]').focus();
                return false;
            }
            else if (res1 == '') {
                ErrorMessage('Term Value/Description is Required.');
                $('[id$=txtTrmVlu]').focus();
                return false;
            }
            else if (($('[id$=ddlArea]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Select Term Area.');
                $('[id$=ddlArea]').focus();
                return false;
            }
            else {
                return true;
            }
        }
    </script>
    <asp:HiddenField runat="server" ID="hdfdTermMstrID" />
    <asp:HiddenField runat="server" ID="hdnDsptn" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Terms Master" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
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
                                        <div style="border: 0px solid #9CB5CB; float: left; background: #F5F4F4; padding: 5px;
                                            width: 98%; margin: 5px; height: 99%;">
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Label13" class="bcLabelright">Terms Name <font color="red" size="2"><b>*</b></font>
                                                        :</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <span>
                                                        <asp:TextBox runat="server" ID="txtTermNm" ValidationGroup="D" class="bcAsptextbox"
                                                            MaxLength="500" onchange="SearchDesc()" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 ]/g,'');"></asp:TextBox>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span2" class="bcLabelright">Terms Value<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <span>
                                                        <asp:TextBox runat="server" ID="txtTrmVlu" ValidationGroup="D" class="bcAspMultiSelectListBox"
                                                            MaxLength="500" TextMode="MultiLine"></asp:TextBox>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span3" class="bcLabelright">Term Area<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:DropDownList runat="server" ID="ddlArea" CssClass="bcAspdropdown">
                                                        <asp:ListItem Text="Select Term Area" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row" style="text-align: center; width: 98%; display: none;">
                                                <div style="text-align: right; width: 49%;">
                                                    <span id="Span1" class="bcLabelright">Term Type<font color="red" size="2"><b>*</b></font>:</span></div>
                                                <div style="text-align: left; width: 50%;">
                                                    <asp:DropDownList runat="server" ID="ddlIpTP" CssClass="bcAspdropdown">
                                                        <asp:ListItem Text="Select Input Type" Value="0"></asp:ListItem>
                                                        <asp:ListItem Text="Text Box" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="Drop Down List" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="Text Area" Value="3"></asp:ListItem>
                                                    </asp:DropDownList>
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
                        <td colspan="3" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:GridView runat="server" ID="gvTmsMstr" AutoGenerateColumns="False" Width="100%"
                                            RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            OnRowCommand="gvTmsMstr_RowCommand" OnRowDataBound="gvTmsMstr_RowDataBound" OnPreRender="gvTmsMstr_PreRender">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Term Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbltrmNm" runat="server" Text='<%#Eval("Description") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Term Value/Description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVle" runat="server" Text='<%#Eval("Value") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Term Area">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrmAra" runat="server" Text='<%#Eval("TermArea") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Terms Master" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrmMstr" runat="server" Text='<%#Eval("ID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                    Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Remove"
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
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvTmsMstr]").dataTable({
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
                    "sSearch": "Search :"
                },

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
            oTable = $("[id$=gvTmsMstr]").dataTable();
        });

        function SearchDesc() {
            var value1 = $('[id$=txtTermNm]').val();
            var filterValue = $(this).attr('href')
            oTable.fnFilter("^" + value1 + "$", 1, true);
            if (oTable.fnSettings().fnRecordsDisplay() > 0) {
                ErrorMessage('Term Name Already Exist');
                $('[id$=txtTermNm]').val('');
            }
            else
                oTable.fnFilter('', 1);
        }
		
    </script>
</asp:Content>
