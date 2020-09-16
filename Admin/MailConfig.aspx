<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="MailConfig.aspx.cs" Inherits="VOMS_ERP.Admin.MailConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <div id="divMessage">
                </div>
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Email Configuration"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;">
                                        <span id="Span6" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;" colspan="3" class="bcTdnormal">
                            <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                width: 98%; margin: 5px; height: 99%;">
                                <div id="dvenmtp" class="row" style="text-align: center; width: 98%; display: none;">
                                    <asp:HiddenField ID="HFEditID" runat="server" Value="" />
                                </div>
                                <div id="dvenmDesc" class="row" style="text-align: center; width: 98%;">
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span3" class="bcLabelright">Company Name<font color="red" size="2"><b>*</b></font>:</span></div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:DropDownList ID="ddlCompanyID" runat="server" CssClass="bcAspdropdown" AutoPostBack="true">
                                                <asp:ListItem Text="--Select--" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp; </span>
                                    </div>
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span7" class="bcLabelright">Domain Name<font color="red" size="2"><b>*</b></font>:</span></div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtDomainName" class="bcAsptextbox"></asp:TextBox>
                                            &nbsp; </span>
                                    </div>
                                    <div id="enmPrnt" style="text-align: right; display: none; width: 49%;">
                                        <span id="Span2" class="bcLabelright"><font color="red" size="2"><b>*</b></font>:</span>
                                        <asp:Label runat="server" class="bcLabelright" ID="lblPrntTxt" Text=""></asp:Label></div>
                                    <div id="enmPrntddl" style="text-align: left; display: none; width: 50%;">
                                        <span></span>
                                    </div>
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span4" class="bcLabelright"><font color="red" size="2"><b>*</b></font>:</span>
                                        <asp:Label runat="server" class="bcLabelright" ID="lblPort" Text="Port No"></asp:Label>
                                    </div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtPortNo" class="bcAsptextbox"></asp:TextBox>
                                            &nbsp; </span>
                                    </div>
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span1" class="bcLabelright">:</span>
                                        <asp:Label runat="server" class="bcLabelright" ID="Label1" Text="Enable SSL"></asp:Label>
                                    </div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                        <asp:CheckBox runat="server" ID="chkIsSSL" />
                                        </span>
                                    </div>
                                    <div style="text-align: left; width: 98%;">
                                        <span>&nbsp; </span>
                                    </div>
                                    <div style="text-align: center; width: 98%;">
                                        <center>
                                        </center>
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
                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click"></asp:LinkButton>
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" Text="Clear" />
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
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="bcTdNewTable">
                            <asp:GridView runat="server" ID="gvDomains" AutoGenerateColumns="False" RowStyle-CssClass="bcGridViewRowStyle"
                                EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" DataKeyNames="ID"
                                Width="100%" OnPreRender="gvDomains_PreRender" OnRowCommand="gvDomains_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="S.No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                        <%--<HeaderStyle Width="10px" />--%>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Domain / IP" DataField="DomainName" />
                                    <asp:BoundField HeaderText="Port" DataField="Port" />
                                    <asp:BoundField HeaderText="SSL" DataField="IsSSL" />
                                    <asp:BoundField HeaderText="Company Name" DataField="CompanyName" />
                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                        Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" Visible="true" />
                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Delete"
                                        Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" Visible="false" />
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="bcTdNewTable">
                            <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                width: 98%; margin: 5px; height: 99%;">
                                <div id="Div4" class="row" style="text-align: center; width: 98%; display: None;">
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span5" class="bcLabelright">Enum Type/Enum Master:</span></div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtSearch" class="autosuggest" CssClass="bcAsptextbox"
                                                MaxLength="100"></asp:TextBox>
                                        </span>
                                    </div>
                                </div>
                            </div>
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

    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("[id$=gvDomains]").dataTable({
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

        function Myvalidations() {

            if (($('[id$=ddlCompanyID]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Company is Required.</span>');
                $('[id$=ddlCompanyID]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }            
            if (($('[id$=txtDomainName]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Domain is Required.</span>');
                $('[id$=txtDomainName]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            if (($('[id$=txtPortNo]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Port is Required.</span>');
                $('[id$=txtPort]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
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
    </script>
</asp:Content>
