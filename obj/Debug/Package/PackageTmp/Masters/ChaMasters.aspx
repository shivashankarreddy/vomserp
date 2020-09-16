<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="ChaMasters.aspx.cs" Inherits="VOMS_ERP.Masters.ChaMasters"
    %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="CHA Master" CssClass="bcTdTitleLabel"></asp:Label><div
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
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Code<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 176px">
                                        <asp:TextBox runat="server" ID="txtcode" ValidationGroup="D" MaxLength="500" CssClass="bcAsptextbox"
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z/ -]/g,'');" onchange="SearchCode()"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 139px">
                                        <asp:TextBox runat="server" ID="txtName" ValidationGroup="D" MaxLength="50" CssClass="bcAsptextbox"
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z/ -]/g,'');" onchange="SearchName()"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="height: 5px">
                                        <span class="bcLabel">Contact Person<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 176px; height: 5px">
                                        <asp:TextBox runat="server" ID="txtcontactperson" ValidationGroup="D" MaxLength="50"
                                            CssClass="bcAsptextbox" onkeyup="this.value=this.value.replace(/[^a-zA-Z/ -]/g,'');"></asp:TextBox>
                                    </td>
                                    <%--<td class="bcTdnormal">
                                        <span class="bcLabel">Address<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtAddress" ValidationGroup="D" CssClass="bcAsptextbox"
                                            Height="42px" TextMode="MultiLine" Width="183px"></asp:TextBox>
                                    </td>--%>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblbcuntry" class="bcLabel">Country<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlblngCntry" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Country" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown1" runat="server" Category="Country"
                                            TargetControlID="ddlblngCntry" PromptText="Select Country" PromptValue="0" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCountrydropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblbstate" class="bcLabel">State<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlblngSt" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select State" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown2" runat="server" Category="State" TargetControlID="ddlblngSt"
                                            ParentControlID="ddlblngCntry" PromptText="Select State" PromptValue="0" LoadingText="Loading States..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindStatedropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBCity" class="bcLabel">City<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlBilngCty" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select City" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown3" runat="server" Category="City" TargetControlID="ddlBilngCty"
                                            ParentControlID="ddlblngSt" PromptText="Select City" LoadingText="Loading Cities..."
                                            PromptValue="0" ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCityropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblBSt" class="bcLabel">Street<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtblngstrt" ValidationGroup="D" MaxLength="200"
                                            CssClass="bcAsptextbox" onkeyup="this.value=this.value.replace(/[^a-zA-Z/ -]/g,'');"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpin" class="bcLabel">PIN/PO Box:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtblngpb" onkeypress="return noSplChar(event)" ValidationGroup="D"
                                            MaxLength="10" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="height: 5px">
                                        <span class="bcLabel">Mobile Number<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 139px; height: 5px">
                                        <asp:TextBox runat="server" ID="txtmbl" ValidationGroup="D" onkeypress='return isNumberKey(event);'
                                            CssClass="bcAsptextbox" MaxLength="15"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">CHA Agent’s Licence number<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtAgentsLicence" ValidationGroup="D" MaxLength="200"
                                            CssClass="bcAsptextbox" onchange="SearchLicNo()"></asp:TextBox>
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
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
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
                        <td colspan="6">
                            <div>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:GridView runat="server" ID="chamaster" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                                Width="100%" OnRowCommand="chamaster_RowCommand" OnRowDataBound="chamaster_RowDataBound"
                                                OnPreRender="chamaster_PreRender">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                            <asp:Label ID="txtchaMstr" runat="server" Text='<%#Eval("ID") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Code">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtcode" runat="server" Text='<%#Eval("Code") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtName" runat="server" Text='<%#Eval("Name") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Contact Person">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtcontactperson" runat="server" Text='<%#Eval("ContactPrsn") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Mobile Number">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtmbl" runat="server" Text='<%#Eval("MobileNo") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="AgentsLicenceNo" DataField="AgentsLicenceNo" HeaderStyle-Width="30px" />
                                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                        Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Remove"
                                                        Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
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
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=chamaster]").dataTable({
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
            oTable = $("[id$=chamaster]").dataTable();
        });

        function SearchCode() {
            var value1 = $('[id$=txtcode]').val(); ;
            oTable.fnFilter("^" + value1 + "$", 1, true);
            if (oTable.fnSettings().fnRecordsDisplay() > 0) {
                ErrorMessage('Code Already Exist');
                $('[id$=txtcode]').val('');
            }
            else
                oTable.fnFilter('', 1);
        }
        function SearchName() {
            var value2 = $('[id$=txtName]').val(); ;
            oTable.fnFilter("^" + value2 + "$", 2, true);
            if (oTable.fnSettings().fnRecordsDisplay() > 0) {
                ErrorMessage('Name Already Exist');
                $('[id$=txtName]').val('');
            }
            else
                oTable.fnFilter('', 2);
        }
        function SearchLicNo() {
            var value3 = $('[id$=txtAgentsLicence]').val(); ;
            oTable.fnFilter("^" + value3 + "$", 3, true);
            if (oTable.fnSettings().fnRecordsDisplay() > 0) {
                ErrorMessage('Licence Number Already Exist');
                $('[id$=txtAgentsLicence]').val('');
            }
            else
                oTable.fnFilter('', 3);
        }
        function Myvalidations() {

            if (($('[id$=txtcode]').val()).trim() == '') {
                ErrorMessage('Code is Required.');
                $('[id$=txtcode]').focus();
                return false;
            }
            else if (($('[id$=txtName]').val()).trim() == '') {
                ErrorMessage('Name is Required.');
                $('[id$=txtName]').focus();
                return false;
            }
            else if (($('[id$=txtcontactperson]').val()).trim() == '') {
                ErrorMessage('Contact Person is Required.');
                $('[id$=txtcontactperson]').focus();
                return false;
            }
            else if (($('[id$=ddlblngCntry]').val()).trim() == '0') {
                ErrorMessage('Country is Required.');
                $('[id$=ddlblngCntry]').focus();
                return false;
            }
            else if (($('[id$=ddlblngSt]').val()).trim() == '0') {
                ErrorMessage('State is Required.');
                $('[id$=ddlblngSt]').focus();
                return false;
            }
            else if (($('[id$=ddlBilngCty]').val()).trim() == '0') {
                ErrorMessage('City is Required.');
                $('[id$=ddlBilngCty]').focus();
                return false;
            }
            else if (($('[id$=txtblngstrt]').val()).trim() == '') {
                ErrorMessage('Street is Required.');
                $('[id$=txtblngstrt]').focus();
                return false;
            }
            else if (($('[id$=txtmbl]').val()).trim() == '') {
                ErrorMessage('Mobile Number is Required.');
                $('[id$=txtmbl]').focus();
                return false;
            }
            else if (($('[id$=txtAgentsLicence]').val()).trim() == '') {
                ErrorMessage('Agent Licence Number is Required.');
                $('[id$=txtAgentsLicence]').focus();
                return false;
            }
            else {
                return true;
            }

        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function isAlphaKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && (charCode < 65 || charCode > 91) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function noSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 44 && charCode != 45 && charCode != 46 && charCode != 47 && charCode != 58 && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function isOrgName(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 46 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        // E-Mail Validation
        function validateEmail(emailField) {
            var reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            if (emailField.value == '') {
                return true;
            }
            else if (reg.test(emailField.value) == false) {
                emailField.value = '';
                emailField.focus();
                ErrorMessage('Invalid Email-ID');
                return false;
            }
            return true;
        }       
    </script>
</asp:Content>
