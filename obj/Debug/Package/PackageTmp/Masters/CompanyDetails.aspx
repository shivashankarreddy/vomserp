<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CompanyDetails.aspx.cs" Inherits="VOMS_ERP.Masters.CompanyDetails"
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Company Details" CssClass="bcTdTitleLabel"></asp:Label><div
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
                        <td colspan="6">
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblOrg" class="bcLabel">Name of Organization <font color="red" size="2"><b>
                                            *</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onkeypress="return isOrgName(event);" onchange="SearchCmpnyNm()"--%>
                                        <asp:DropDownList runat="server" ID="ddlorgnm" ValidationGroup="D" CssClass="bcAsptextbox"
                                            OnSelectedIndexChanged="ddlorgnm_SelectedIndexChanged" runat="server" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBName" class="bcLabel">Business Name <font color="red" size="2"><b></b>
                                        </font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" Enabled="false" ID="txtbsnm" ValidationGroup="D" MaxLength="100"
                                            ReadOnly="false" Text="" CssClass="bcAsptextbox" onkeypress="return isOrgName(event);"
                                            Height="17px"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblTelPh" class="bcLabel">Telephone:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txttpn" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onkeypress="return isNumberKey(event)" onblur="return CheckT_M();"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblMobile" class="bcLabel">Mobile <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtmbl" ValidationGroup="D" MaxLength="12" CssClass="bcAsptextbox"
                                            onkeypress="return isNumberKey(event)" onblur="return CheckT_M();CheckT_F();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpem" class="bcLabel">Primary Email<font color="red" size="2"><b> *</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtpem" ValidationGroup="D" MaxLength="70" CssClass="bcAsptextbox"
                                            onblur="return validateEmail(this);CheckT_E();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblSEM" class="bcLabel">Secondary Email:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtsem" ValidationGroup="D" MaxLength="70" CssClass="bcAsptextbox"
                                            onblur="return validateEmail(this);CheckT_E();"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblFax" class="bcLabel">Fax :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfx" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onkeypress="return isNumberKey(event)" onblur="return CheckT_F();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblCity" class="bcLabel">Contact Person <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCntctPrsn" onkeypress="return isAlphaKey(event);"
                                            ValidationGroup="D" CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Alternate Contact Person :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtAltCntctPrsn" onkeypress="return isAlphaKey(event);"
                                            ValidationGroup="D" CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Email CC :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtEmailCC" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                            TextMode="MultiLine" Rows="4" onblur="return validateMultipleEmailsCommaSeparated(this);"></asp:TextBox>
                                    </td>
                                    <td colspan="6">
                                        &nbsp;<asp:HiddenField runat="server" ID="hdfdCmpDtlsID" />
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        Address Information
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblbcuntry" class="bcLabel">Country <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCntry" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlCntry_SelectedIndexChanged">
                                            <asp:ListItem Text="Select Country" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblbstate" class="bcLabel">State <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlSt" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlSt_SelectedIndexChanged">
                                            <asp:ListItem Text="Select State" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBCity" class="bcLabel">City <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:getval('hdfdblngCty', 'ddlBilngCty')"--%>
                                        <asp:DropDownList runat="server" ID="ddlCty" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select City" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblBSt" class="bcLabel">Street <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtstrt" ValidationGroup="D" MaxLength="200" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9# ]/g'');"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">Area:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtArea" ValidationGroup="D" MaxLength="200" onkeyup="this.value=this.value.replace(/[^a-zA-Z# ]/g,'');"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpin" class="bcLabel">PIN/PO Box:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtpb" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="10" onkeyup="this.value=this.value.replace(/[^0-9]/g,'');"></asp:TextBox>
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
                                                                <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
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
                    <tr>
                        <td colspan="6">
                            <div>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:GridView runat="server" ID="gvCmpnyDtls" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                                Width="100%" OnRowCommand="gvCmpnyDtls_RowCommand" OnRowDataBound="gvCmpnyDtls_RowDataBound"
                                                OnPreRender="gvCmpnyDtls_PreRender">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Org. Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblorgName" runat="server" Text='<%#Eval("CompanyName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Contact Person">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCntctPrsn" runat="server" Text='<%# String.Format("{0}/{1}", Eval("ContactPrsn"), Eval("AltContactPrsn")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Tele Phone/Mobile">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblphonembl" runat="server" Text='<%# String.Format("{0}/{1}", Eval("Telephone"), Eval("AltContact")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="E-Mail">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblprieml" runat="server" Text='<%# String.Format("{0}/{1}", Eval("PMail"), Eval("SMail"))  %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Address">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddrs" runat="server" Text='<%# String.Format("{0}/{1}/{2}", Eval("Address1"), Eval("City"), Eval("State"))  %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Customer Code" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCmpnyDtls" runat="server" Text='<%#Eval("ID") %>'></asp:Label>
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
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_filter
        {
            visibility: visible !important;
        }
    </style>
    <script type="text/javascript">
        function validateMultipleEmailsCommaSeparated(email) {
            var result = email.value.split(",");
            for (var i = 0; i < result.length; i++) {
                var reg = /^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[,]{0,1}\s*)+$/;
                if (reg.test(result[i]) == false) {
                    email.value = '';
                    email.focus();
                    ErrorMessage('Invalid Email-ID');
                    return false;
                }
            }
            return true;
        }

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvCmpnyDtls]").dataTable({
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
            oTable = $("[id$=gvCmpnyDtls]").dataTable();
        });

        function CheckT_M() {
            var Telephone = $('[id$=txttpn]').val();
            var Mobile = $('[id$=txtmbl]').val();
            if (Telephone != '' && Mobile != '' && Telephone == Mobile) {
                //$('[id$=txttpn]').val('');
                $('[id$=txtmbl]').val('');
                ErrorMessage('Telephone No. and Mobile No. should not be same.');
            }
        }
        function CheckT_F() {
            var Telephone = $('[id$=txtfx]').val();
            var Mobile = $('[id$=txtmbl]').val();
            if (Telephone != '' && Mobile != '' && Telephone == Mobile) {
                $('[id$=txtfx]').val('');
                //$('[id$=txtmbl]').val('');
                ErrorMessage('Mobile No. and Fax No. should not be same.');
            }
        }
        function CheckT_E() {
            var Telephone = $('[id$=txtpem]').val();
            var Mobile = $('[id$=txtsem]').val();
            if (Telephone != '' && Mobile != '' && Telephone == Mobile) {
                //$('[id$=txtpem]').val('');
                $('[id$=txtsem]').val('');
                ErrorMessage('Primary and Secondry E-Mails should not be same.');
            }
        }

        function SearchCmpnyNm() {
            var value1 = $('[id$=txtorgnm]').val();
            oTable.fnFilter(value1, 1);
            if ($('[id$=gvCmpnyDtls] >tbody >tr >td').length > 1) {
                ErrorMessage('Company Name already Exists');
                $('[id$=txtorgnm]').val('');
            }
            oTable.fnFilter('', 1);
        }

        function Myvalidations() {
            if (($('[id$=ddlorgnm]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Orginazation Name is Required.</span>');
                $('[id$=ddlorgnm]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtbsnm]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Business Name is Required.</span>');
                $('[id$=txtbsnm]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtmbl]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Mobile Number is Required.</span>');
                $('[id$=txtmbl]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if ((($('[id$=txtmbl]').val()).trim() != '') && ($('[id$=txttpn]').val().trim() != '')
            && ($('[id$=txtmbl]').val()).trim() == $('[id$=txttpn]').val().trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Mobile and Telephone Number should not be same.</span>');
                $('[id$=txtmbl]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtpem]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Primary E-Mail is Required.</span>');
                $('[id$=txtpem]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtpem]').val()).trim() == $('[id$=txtsem]').val().trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Primary Email and Secondary Email should not be same.</span>');
                $('[id$=txtpem]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if ((($('[id$=txtmbl]').val()).trim() != '') && ($('[id$=txtfx]').val().trim() != '')
            && ($('[id$=txtmbl]').val()).trim() == $('[id$=txtfx]').val().trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Mobile and Fax should not be same.</span>');
                $('[id$=txtmbl]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtCntctPrsn]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Contact Person Name is Required.</span>');
                $('[id$=txtCntctPrsn]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if ((($('[id$=txtCntctPrsn]').val()).trim() != '') && ($('[id$=txtAltCntctPrsn]').val().trim() != '')
            && ($('[id$=txtCntctPrsn]').val()).trim() == $('[id$=txtAltCntctPrsn]').val().trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Contact person and Alternate Contact Person should not be same.</span>');
                $('[id$=txtmbl]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlCntry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Billing Country is Required.</span>');
                $('[id$=ddlCntry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlSt]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Billing State is Required.</span>');
                $('[id$=ddlSt]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlCty]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Billing City is Required.</span>');
                $('[id$=ddlCty]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtstrt]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Billing Street is Required.</span>');
                $('[id$=txtstrt]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }            
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function isOrgName(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 46 &&
            (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function isAlphaKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 32 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
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
