<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Company.aspx.cs" Inherits="VOMS_ERP.Admin.Company" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Company" CssClass="bcTdTitleLabel"></asp:Label><div
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
                                        <span id="lblOrg" class="bcLabel">Company Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCompany" onchange="SearchCmpnyNm()" ValidationGroup="D"
                                            MaxLength="150" onkeypress="return isOrgName(event)" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBName" class="bcLabel">Business Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtbsnm" onchange="SearchBsNm()" ValidationGroup="D"
                                            MaxLength="100" onkeypress="return isOrgName(event)" CssClass="bcAsptextbox"
                                            Height="17px"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblTelPh" class="bcLabel">User-ID<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtUsrId" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onchange="SearchUsrId()" MaxLength="50" onblur="return validateEmail(this);"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblFax" class="bcLabel">Password :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" TextMode="Password" ID="txtPswrd" ValidationGroup="D"
                                            CssClass="bcAsptextbox" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblMobile" class="bcLabel">Contact No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtmbl" ValidationGroup="D" MaxLength="12" CssClass="bcAsptextbox"
                                            onkeypress="return isNumberKey(event)" onblur="return CheckT_M();CheckT_F();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Address :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtAdd" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="500"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblCity" class="bcLabel">Country<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCmpnyCntry" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Country" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <ajax:CascadingDropDown ID="CascadingDropDown1" runat="server" Category="Country"
                                            TargetControlID="ddlCmpnyCntry" PromptText="Select Country" PromptValue="0" LoadingText="Loading Countries..."
                                            ServicePath="../Masters/cascadingdataservice.asmx" ServiceMethod="BindCountrydropdown">
                                        </ajax:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">State<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCmpnySt" CssClass="bcAspdropdown" Enabled="false">
                                            <asp:ListItem Text="Select State" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <ajax:CascadingDropDown ID="CascadingDropDown2" runat="server" Category="State" TargetControlID="ddlCmpnySt"
                                            ParentControlID="ddlCmpnyCntry" PromptText="Select State" PromptValue="0" LoadingText="Loading States..."
                                            ServicePath="../Masters/cascadingdataservice.asmx" ServiceMethod="BindStatedropdown">
                                        </ajax:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBSt" class="bcLabel">City<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCmpnyCty" CssClass="bcAspdropdown" Enabled="false">
                                            <asp:ListItem Text="Select City" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <ajax:CascadingDropDown ID="CascadingDropDown3" runat="server" Category="City" TargetControlID="ddlCmpnyCty"
                                            ParentControlID="ddlCmpnySt" PromptText="Select City" PromptValue="0" LoadingText="Loading Cities..."
                                            ServicePath="../Masters/cascadingdataservice.asmx" ServiceMethod="BindCityropdown">
                                        </ajax:CascadingDropDown>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblpin" class="bcLabel">P.B. Number :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCmnpypbn" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblshpstr" class="bcLabel">Contact Person :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCntctPersn" onkeypress="return isOrgName(event)"
                                            ValidationGroup="D" MaxLength="200" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblshpin" class="bcLabel">Company Title<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCmpnyTtl" onkeypress="return isOrgName(event)"
                                            ValidationGroup="D" CssClass="bcAsptextbox" MaxLength="100"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Company Currency<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 70px;">
                                        <asp:DropDownList runat="server" ID="ddlCurrency" CssClass="bcAspdropdown" Enabled="true">
                                            <asp:ListItem Text="Select Currency" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:HiddenField runat="server" ID="hdfdCmpnyID" />
                                        <asp:HiddenField runat="server" ID="hdfdCompnyLogo" />
                                        <span id="Span5" class="bcLabel">Company Logo<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 50px;">
                                        <asp:FileUpload runat="server" ID="ImgCmpLogo" onchange="previewFile()" />
                                    </td>
                                    <td>
                                        <asp:Image ID="CmpnyLogo1" Style="width: 50px; height: 50px" runat="server"></asp:Image>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">User Type<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 70px;">
                                        <asp:DropDownList runat="server" ID="UserType" CssClass="bcAspdropdown" 
                                            Enabled="true">
                                            <asp:ListItem  Value="0">-- Select --</asp:ListItem>
                                            <asp:ListItem  Value="1">Vendor</asp:ListItem>
                                            <asp:ListItem  Value="2">Customer</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Region<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 70px;">
                                        <asp:DropDownList runat="server" ID="ddlRegion" CssClass="bcAspdropdown" 
                                            Enabled="true">
                                            <asp:ListItem  Value="0">-- Select --</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" class="bcTdNewTable">
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
                        <td>
                            <asp:GridView runat="server" ID="gvCompany" RowStyle-CssClass="bcGridViewRowStyle"
                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                Width="100%" OnRowCommand="gvCompany_RowCommand" OnRowDataBound="gvCompany_RowDataBound"
                                OnPreRender="gvCompany_PreRender">
                                <HeaderStyle CssClass="headerCamera" />
                                <Columns>
                                    <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Company Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblorgName" runat="server" Text='<%#Eval("CompanyName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="10px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Business Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblBussName" runat="server" Text='<%#Eval("BusinessName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="10px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mobile">
                                        <ItemTemplate>
                                            <asp:Label ID="lblphonembl" runat="server" Text='<%# ShowNmbrs(Eval("PhoneNumber")) %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="10px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Primary E-Mail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblprieml" runat="server" Text='<%#Eval("UserID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="10px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Code" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCmpnyId" runat="server" Text='<%#Eval("CompanyID") %>'></asp:Label>
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.tablePagination.0.1.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">

        function previewFile() {
            var preview = document.querySelector('#<%=CmpnyLogo1.ClientID %>');
            var file = document.querySelector('#<%=ImgCmpLogo.ClientID %>').files[0];
            var reader = new FileReader();

            reader.onloadend = function () {
                preview.src = reader.result;
            }

            if (file) {
                reader.readAsDataURL(file);
            } else {
                preview.src = "";
            }
        }



        function SearchCmpnyNm() {
            try {
                var value1 = $('[id$=txtCompany]').val();
                var result = Company.CheckCustomerName(value1, 'CompanyName');
                if (!result.value) {
                    ErrorMessage('Company Name already Exists');
                    $('[id$=txtCompany]').val('');
                    $('[id$=txtCompany]').focus();
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }
        function SearchBsNm() {

            try {
                var value1 = $('[id$=txtbsnm]').val();
                var result = Company.CheckCustomerName(value1, 'OrgName');
                if (!result.value) {
                    ErrorMessage('Business Name already Exists');
                    $('[id$=txtbsnm]').val('');
                    $('[id$=txtbsnm]').focus();
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }
        function SearchUsrId() {

            try {
                var value1 = $('[id$=txtUsrId]').val();
                var result = Company.CheckCustomerName(value1, 'UsrId');
                if (!result.value) {
                    ErrorMessage('UserId already Exists');
                    $('[id$=txtUsrId]').val('');
                    $('[id$=txtUsrId]').focus();
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) //charCode != 46 &&
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
                alert('Invalid Email-ID');
                return false;
            }
            return true;
        }

        function Myvalidations() {

            var res = $('[id$=txtCompany]').val();
            var res1 = $('[id$=txtbsnm]').val();
            var res2 = $('[id$=txtUsrId]').val();
            var res3 = $('[id$=txtPswrd]').val();
            var res4 = $('[id$=txtmbl]').val();
            var res6 = $('[id$=ddlCmpnyCntry]').val();
            var res7 = $('[id$=ddlCmpnySt]').val();
            var res10 = $('[id$=ddlCmpnyCty]').val();
            var res8 = $('[id$=txtCmpnyTtl]').val();
            var res9 = $('[id$=ImgCmpLogo]').val();
            var res11 = $('[id$=hdfdCmpnyID]').val();
            var res12 = $('[id$=UserType]').val();
            if (res.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Company Name is Required.</span>');
                $('[id$=txtCompany]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (res1.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Business Name is Required.</span>');
                $('[id$=txtbsnm]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res2.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">User-ID is Required.</span>');
                $('[id$=txtUsrId]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res3.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Password is Required.</span>');
                $('[id$=txtPswrd]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            //            else if (res2.trim() != '') {
            //                var re = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,}/;
            //                if (!(re.test(res3))) {
            //                    $("#<%=divMyMessage.ClientID %> span").remove();
            //                    $('[id$=divMyMessage]').append('<span class="Error">Password must be Six Characters(at least one number, one lowercase and one uppercase letter) is Required.</span>');
            //                    $('[id$=txtPswrd]').focus();
            //                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
            //                    return false;
            //                }

            //            }
            else if (res4.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Mobile is Required.</span>');
                $('[id$=txtmbl]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }

            else if (res6.trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Country is Required.</span>');
                $('[id$=ddlCmpnyCntry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }

            else if (res7.trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">State is Required.</span>');
                $('[id$=ddlCmpnySt]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res10.trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">City is Required.</span>');
                $('[id$=ddlCmpnyCty]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res8.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Company Title is Required.</span>');
                $('[id$=txtCmpnyTtl]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res9.trim() == '' && res11.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Company Logo is Required.</span>');
                $('[id$=ImgCmpLogo]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res12.trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">User Type is Required.</span>');
                $('[id$=UserType]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res9.trim() != '') {
                var ext = res9.substring(res9.lastIndexOf('.') + 1);
                if (ext == "gif" || ext == "GIF" || ext == "JPEG" || ext == "jpeg" || ext == "jpg" || ext == "JPG" || ext == "PNG" || ext == "png") {
                    return true;
                }
                else {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Only Image Files are Upload(*.JPG,*.PNG,*.JPEG).</span>');
                    $('[id$=ImgCmpLogo]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                } 
            }
            
            else {
                return true;
            }
        }

        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvCompany]").dataTable({
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
            }); //
            oTable = $("[id$=gvCompany]").dataTable();
        });

    </script>
</asp:Content>
