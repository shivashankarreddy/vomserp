<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    MaintainScrollPositionOnPostback="true" CodeBehind="Contact.aspx.cs" Inherits="VOMS_ERP.Masters.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../JScript/jquery.1.4.2.js"></script>
    <style type="text/css">
        .dataTables_filter
        {
            visibility: visible !important;
        }
    </style>
    <script type="text/javascript">
        // This is to check Primary MailID 
        function CheckMailID(mailid) {
            $.ajax({
                type: "POST",
                url: "Contact.aspx/CheckMailID",
                data: '{MailID: "' + $("#<%=txtpem.ClientID%>")[0].value + '" }',
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
                        ErrorMessage('Mail-ID Exists');
                        $("#<%=txtpem.ClientID%>")[0].value = '';
                        $("#<%=txtpem.ClientID%>")[0].focus();
                    }
                }
            }
        }

        // This is to check First Name
        //        function CheckFirstName() {
        //            $.ajax({
        //                type: "POST",
        //                url: "Contact.aspx/CheckFirstName",
        //                data: '{FirstName: "' + $("#<%=txtfname.ClientID%>")[0].value + '" }',
        //                contentType: "application/json; charset=utf-8",
        //                dataType: "json",
        //                success: OnSuccess,
        //                failure: function (response) {
        //                    alert(response);
        //                }
        //            });
        //            function OnSuccess(response) {
        //                if (response != null) {
        //                    if (response.d == "False") {
        //                        ErrorMessage('FirstName is Taken.');
        //                        $("#<%=txtfname.ClientID%>")[0].value = '';
        //                        $("#<%=txtfname.ClientID%>")[0].focus();
        //                    }
        //                }
        //            }
        //        }

        // This is to check Last Name
        //        function CheckLastName() {
        //            $.ajax({
        //                type: "POST",
        //                url: "Contact.aspx/CheckLastName",
        //                data: '{LastName: "' + $("#<%=txtlname.ClientID%>")[0].value + '" }',
        //                contentType: "application/json; charset=utf-8",
        //                dataType: "json",
        //                success: OnSuccess,
        //                failure: function (response) {
        //                    alert(response);
        //                }
        //            });
        //            function OnSuccess(response) {
        //                if (response != null) {
        //                    if (response.d == "False") {
        //                        ErrorMessage('LastName is Taken.');
        //                        $("#<%=txtlname.ClientID%>")[0].value = '';
        //                        $("#<%=txtlname.ClientID%>")[0].focus();
        //                    }
        //                }
        //            }
        //        }

        // This is to check Alias Name
        function CheckAlias() {
            $.ajax({
                type: "POST",
                url: "Contact.aspx/CheckAlias",
                data: '{AliasName: "' + $("#<%=txtalsNm.ClientID%>")[0].value + '" }',
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
                        ErrorMessage('Alias Name is Taken.');
                        $("#<%=txtalsNm.ClientID%>")[0].value = '';
                        $("#<%=txtalsNm.ClientID%>")[0].focus();
                    }
                }
            }
        }
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
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Users" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
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
                                    <td class="bcTdnormal">
                                        <span id="lblfName" class="bcLabel">First Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <%--onchange="CheckFirstName()"--%>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfname" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onkeypress="return NameVal(event);" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblLName" class="bcLabel">Last Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <%--onchange="CheckLastName()"--%>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtlname" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onkeypress="return NameVal(event);" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblaName" class="bcLabel">Alias Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <%--onchange="CheckAlias()"--%>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtalsNm" onkeypress="return NameVal(event);" ValidationGroup="D"
                                            MaxLength="3" CssClass="bcAsptextbox" style='text-transform:uppercase'></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Designation<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlDesignationID" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Department<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddldept" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lbltelph" class="bcLabel">Telephone Number:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txttpn" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onkeypress="return isNumberKey(event)" onblur="return CheckT_M();" MaxLength="12"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblMobileNo" class="bcLabel">Mobile Number<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtmbl" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onkeypress="return isNumberKey(event)" onblur="return CheckT_M(); CheckT_F();"
                                            MaxLength="12"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Fax Number:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfx" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onkeypress="return isNumberKey(event)" onblur="return CheckT_F();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpem" class="bcLabel">Primary Email<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <%--SearchEmail();--%>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtpem" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onblur="return validateEmail(this); CheckT_E();" onchange="return CheckMailID(this);"
                                            MaxLength="70"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblSEM" class="bcLabel">Secondary Email:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtsem" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onblur="validateEmail(this);" MaxLength="70"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblcontyp" class="bcLabel">Contact Type<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlcncttp" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlcncttp_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="--Select Contact Type--"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblRole" class="bcLabel">
                                            <asp:Label ID="lblRoleType" runat="server" Text="" Visible="false">Role<font color="red" size="2"><b>*</b></font>:</asp:Label>
                                            <asp:Label ID="lblCompany" runat="server" Text="" Visible="false">Supplier Name<font color="red" size="2"><b>*</b></font>:</asp:Label>
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlrole" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" visible="false">
                                        <span id="lblCmpnNme" class="bcLabel">
                                            <asp:Label ID="lblCmpnNm" runat="server" Visible="false" Text="Company Name:">Company Name<font color="red" size="2"><b>*</b></font>:</asp:Label>
                                        </span>
                                    </td>
                                    <td class="bcTdnormal" visible="false">
                                        <asp:DropDownList runat="server" ID="ddlCmpnyNm" Visible="false" AutoPostBack ="true" 
                                            CssClass="bcAspdropdown" 
                                            onselectedindexchanged="ddlCmpnyNm_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" visible="false">
                                        <span id="lblCust" class="bcLabel">
                                            <asp:Label ID="lblCustomer" runat="server" Visible="false" Text="Customer List :">Customer List<font color="red" size="2"><b>*</b></font>:</asp:Label>
                                        </span>
                                    </td>
                                    <td class="bcTdnormal" visible="false">
                                        <asp:ListBox runat="server" ID="lbcustomer" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox">
                                        </asp:ListBox>
                                    </td>
                                    <td colspan="2">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" align="right" class="bcTdNewTable">
                                        <center>
                                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                                <tbody>
                                                    <tr valign="middle">
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div1" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClientClick="javascript:validations()"
                                                                    OnClick="btnSave_Click" />
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
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:GridView runat="server" ID="gvcontact" RowStyle-CssClass="bcGridViewRowStyle"
                                            EmptyDataText="No records Found" RowStyle-VerticalAlign="Bottom" AutoGenerateColumns="False"
                                            EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            Width="100%" DataKeyNames="ID" OnRowCommand="gvcontact_RowCommand" OnRowDeleting="gvcontact_RowDeleting"
                                            OnRowDataBound="gvcontact_RowDataBound" OnPreRender="gvcontact_PreRender" EnableTheming="True"
                                            TabIndex="-1">
                                            <RowStyle VerticalAlign="Bottom" CssClass="bcGridViewRowStyle"></RowStyle>
                                            <EmptyDataRowStyle CssClass="bcGridViewEmptyDataRowStyle"></EmptyDataRowStyle>
                                            <Columns>
                                                <asp:TemplateField HeaderText="ID" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSNO" runat="server" Text='<%# Eval("ID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Full Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFullName" runat="server" Text='<%# Eval("FullName") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Department">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDept" runat="server" Text='<%# Eval("Department") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Mobile/Phone">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblContact" runat="server" Text='<%# Eval("Contact") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="E-MailID">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblmailID" runat="server" Text='<%# Eval("MailID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                    Text="Modify" ShowHeader="true" HeaderStyle-Width="20px"></asp:ButtonField>
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
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        var oTable;


        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvcontact]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                //"bStateSave": true,

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
            oTable = $("[id$=gvcontact]").dataTable();
        });
        //-------------      -----------

        //        function SearchEmail() {
        //            var value1 = $('[id$=txtpem]').val();
        //            oTable.fnFilter(value1, 4);
        //            if ($('[id$=gvcontact] >tbody >tr').length >= 1) {
        //                ErrorMessage('Primary E-Mail ID already Exists');
        //                $('[id$=txtpem]').val('');
        //            }
        //            oTable.fnFilter('', 4);
        //        }

        function CheckT_M() {
            var Telephone = $('[id$=txttpn]').val();
            var Mobile = $('[id$=txtmbl]').val();
            if (Telephone != '' && Mobile != '' && Telephone == Mobile) {
                $('[id$=txtmbl]').val('');
                ErrorMessage('Telephone No. and Mobile No. should not be same.');
            }
        }
        function CheckT_F() {
            var Telephone = $('[id$=txtfx]').val();
            var Mobile = $('[id$=txtmbl]').val();
            if (Mobile == "") {
                $('[id$=txtfx]').val('');
                ErrorMessage('Mobile No. Cannot be empty.');
            }
            else if (Telephone != '' && Mobile != '' && Telephone == Mobile) {
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

        function fnClickRedraw() {
            $(document).ready(function () {
                //oTable = $('table#ctl00_ContentPlaceHolder1_gvcontact').dataTable({ "bStateSave": true });                
            });
        };

        function NameVal(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) //charCode != 46 &&
                return false;
            return true;
        }

        // E-Mail Validation
        function validateEmail(emailField) {
            var reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            if (emailField.value == '') {
                return true;
            }
            if (reg.test(emailField.value) == false) {
                emailField.value = '';
                emailField.focus();
                ErrorMessage('Invalid Email-ID');
                return false;
            }
            return true;
        }

        function Myvalidations() {

            var res = $('[id$=txtfname]').val();
            var res1 = $('[id$=txtlname]').val();
            var res2 = $('[id$=txtalsNm]').val();
            var res3 = $('[id$=ddlDesignationID]').val();
            var res4 = $('[id$=ddldept]').val();
            var res5 = $('[id$=txtmbl]').val();
            var res6 = $('[id$=txtpem]').val();
            var res7 = $('[id$=ddlcncttp]').val();
            var res8 = $('[id$=ddlrole]').val();
            var res9 = $('[id$=ddlCmpnyNm]').val();
            if (res.trim() == '') {
                ErrorMessage('First Name is Required.');
                $('[id$=txtfname]').focus();
                return false;
            }
            else if (res1.trim() == '') {
                ErrorMessage('Last Name is Required.');
                $('[id$=txtlname]').focus();
                return false;
            }
            else if (res2.trim() == '') {
                ErrorMessage('Alias Name is Required.');
                $('[id$=txtalsNm]').focus();
                return false;
            }
            else if (res3.trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Designation is Required.');
                $('[id$=ddlDesignationID]').focus();
                return false;
            }
            else if (res4.trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Department is Required.');
                $('[id$=ddldept]').focus();
                return false;
            }
            else if (res5.trim() == '') {
                ErrorMessage('Mobile Number is Required.');
                $('[id$=txtmbl]').focus();
                return false;
            }
            else if ((($('[id$=txtmbl]').val()).trim() != '') && ($('[id$=txtfx]').val().trim() != '')
            && ($('[id$=txtmbl]').val()).trim() == $('[id$=txtfx]').val().trim()) {
                ErrorMessage('Mobile and Fax should not be same.');
                $('[id$=txtmbl]').focus();
                return false;
            }
            else if ((($('[id$=txtmbl]').val()).trim() != '') && ($('[id$=txttpn]').val().trim() != '') &&
            ($('[id$=txtmbl]').val()).trim() == $('[id$=txttpn]').val().trim()) {
                ErrorMessage('Mobile and Telephone Number should not be same.');
                $('[id$=txtmbl]').focus();
                return false;
            }
            else if (res6.trim() == '') {
                ErrorMessage('Primary E-Mail is Required.');
                $('[id$=txtpem]').focus();
                return false;
            }
            else if (($('[id$=txtpem]').val()).trim() == $('[id$=txtsem]').val().trim()) {
                ErrorMessage('Primary Email and Secondary Email should not be same.');
                $('[id$=txtpem]').focus();
                return false;
            }
            else if (res7.trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Contact Type is Required.');
                $('[id$=ddlcncttp]').focus();
                return false;
            }
            else if (res8.trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Role/Company Name is Required.');
                $('[id$=ddlrole]').focus();
                return false;
            }
            else if (res9.trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Company Name is Required.');
                $('[id$=ddlCmpnyNm]').focus();
                return false;
            }
        }

    </script>
</asp:Content>
