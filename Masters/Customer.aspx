<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="Customer.aspx.cs" Inherits="VOMS_ERP.Masters.Customer" EnableEventValidation="false" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Customer" CssClass="bcTdTitleLabel"></asp:Label><div
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
                                        <span id="lblOrg" class="bcLabel">Name of Organization<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtorgnm" onchange="SearchCmpnyNm()" ValidationGroup="D"
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
                                        <span id="lblTelPh" class="bcLabel">Telephone:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txttpn" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onblur="return CheckT_M();" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblMobile" class="bcLabel">Mobile<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtmbl" ValidationGroup="D" MaxLength="12" CssClass="bcAsptextbox"
                                            onkeypress="return isNumberKey(event)" onblur="return CheckT_M();CheckT_F();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpem" class="bcLabel">Primary Email<font color="red" size="2"><b>*</b></font>:</span>
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
                                        <span id="lblFax" class="bcLabel">Fax1:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfx" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onkeypress="return isNumberKey(event)" onblur="return CheckT_F();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblCity" class="bcLabel">Fax2:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfx2" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onkeypress="return isNumberKey(event)" onblur="return CheckT_F1();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Shipping Port:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtShpingPortNm" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="150"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Region<font color="red" size="2"><b>*</b></font></span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlRegion" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Country" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        &nbsp;
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
                                        <span id="lblbcuntry" class="bcLabel">Billing Country<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:UpdateDropDownList('ddlblngCntry', 'Customer.Aspx', 'ddlblngSt')"--%>
                                        <asp:DropDownList runat="server" ID="ddlblngCntry" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Country" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown1" runat="server" Category="Country"
                                            TargetControlID="ddlblngCntry" PromptText="Select Country" PromptValue="0" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCountrydropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblbstate" class="bcLabel">Billing State:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:UpdateDropDownList('ddlblngSt', 'Customer.Aspx', 'ddlBilngCty');getval('hdfdblngSt', 'ddlblngSt')"--%>
                                        <asp:DropDownList runat="server" ID="ddlblngSt" CssClass="bcAspdropdown" Enabled="false">
                                            <asp:ListItem Text="Select State" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown2" runat="server" Category="State" TargetControlID="ddlblngSt"
                                            ParentControlID="ddlblngCntry" PromptText="Select State" PromptValue="0" LoadingText="Loading States..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindStatedropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBCity" class="bcLabel">Billing City:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:getval('hdfdblngCty', 'ddlBilngCty')"--%>
                                        <asp:DropDownList runat="server" ID="ddlBilngCty" CssClass="bcAspdropdown" Enabled="false">
                                            <asp:ListItem Text="Select City" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown3" runat="server" Category="City" TargetControlID="ddlBilngCty"
                                            ParentControlID="ddlblngSt" PromptText="Select City" PromptValue="0" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCityropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblBSt" class="bcLabel">Billing Street<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtblngstrt" ValidationGroup="D" MaxLength="200"
                                            onkeypress="return noSplChar(event)" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpin" class="bcLabel">Billing PIN/PO Box:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtblngpb" onkeypress="return noSplChar(event)" ValidationGroup="D"
                                            CssClass="bcAsptextbox" MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Shipping Country:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:UpdateDropDownList('ddlshpngCntry', 'Customer.Aspx', 'ddlshpngSt')"--%>
                                        <asp:DropDownList runat="server" ID="ddlshpngCntry" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Country" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown4" runat="server" Category="Country"
                                            TargetControlID="ddlshpngCntry" PromptText="Select Country" PromptValue="0" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCountrydropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblshpstate" class="bcLabel">Shipping State:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:UpdateDropDownList('ddlshpngSt', 'Customer.Aspx', 'ddlshpngCty');getval('hdfdShpngSt', 'ddlshpngSt')"--%>
                                        <asp:DropDownList runat="server" ID="ddlshpngSt" CssClass="bcAspdropdown" Enabled="false">
                                            <asp:ListItem Text="Select State" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown5" runat="server" Category="State" TargetControlID="ddlshpngSt"
                                            ParentControlID="ddlshpngCntry" PromptText="Select State" PromptValue="0" LoadingText="Loading States..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindStatedropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Shipping City:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:getval('hdfdshpngCty', 'ddlshpngCty');"--%>
                                        <asp:DropDownList runat="server" ID="ddlshpngCty" CssClass="bcAspdropdown" Enabled="false">
                                            <asp:ListItem Text="Select City" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown6" runat="server" Category="City" TargetControlID="ddlshpngCty"
                                            ParentControlID="ddlshpngSt" PromptText="Select City" PromptValue="0" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCityropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblshpstr" class="bcLabel">Shipping Street:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtshpngstrt" onkeypress="return noSplChar(event)"
                                            ValidationGroup="D" MaxLength="200" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblshpin" class="bcLabel">Shipping PIN/PO Box:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtshpngpb" onkeypress="return noSplChar(event)"
                                            ValidationGroup="D" CssClass="bcAsptextbox" MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">Contact Person :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtKindAttn" onkeypress="return noSplChar(event)"
                                            ValidationGroup="D" CssClass="bcAsptextbox" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Assigned To :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlAsgnedUsr" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Assigned Name" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Currency :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCurrency" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Currency" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" class="bcTdNewTable">
                                        <asp:HiddenField runat="server" ID="hdfdCustmrID" />
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
                                            <asp:GridView runat="server" ID="gvcustomer" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                                Width="100%" OnRowCommand="gvcustomer_RowCommand" OnRowDataBound="gvcustomer_RowDataBound"
                                                OnPreRender="gvcustomer_PreRender">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Org. Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblorgName" runat="server" Text='<%#Eval("OrgName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Bussi. Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblBussName" runat="server" Text='<%#Eval("BussName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Tele Phone/Mobile">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblphonembl" runat="server" Text='<%# ShowNmbrs(Eval("Phone"), Eval("Mobile")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Primary E-Mail">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblprieml" runat="server" Text='<%#Eval("PriEmail") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Customer Code" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCustcd" runat="server" Text='<%#Eval("ID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Customer Code" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCmpnyId" runat="server" Text='<%#Eval("CompanyId") %>'></asp:Label>
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
    <script src="../JScript/jquery.tablePagination.0.1.js" type="text/javascript"></script>
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
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvcustomer]").dataTable({
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
            oTable = $("[id$=gvcustomer]").dataTable();
        });

        function SearchCmpnyNm() {
            try {
                var value1 = $('[id$=txtorgnm]').val();
                var result = Customer.CheckCustomerName(value1.trim(), 'OrgName');
                if (!result.value) {
                    ErrorMessage('Organization Name already Exists');
                    $('[id$=txtorgnm]').val('');
                    $('[id$=txtorgnm]').focus();
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }
        function SearchBsNm() {

            try {
                var value1 = $('[id$=txtbsnm]').val();
                var result = Customer.CheckCustomerName(value1.trim(), 'BussName');
                if (!result.value) {
                    ErrorMessage('Business Name already Exists');
                    $('[id$=txtbsnm]').val('');
                    $('[id$=txtbsnm]').focus();
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }

        function Myvalidations() {

            var res = $('[id$=txtorgnm]').val();
            var res1 = $('[id$=txtbsnm]').val();
            var res2 = $('[id$=txtmbl]').val();
            var res3 = $('[id$=txtpem]').val();
            var res4 = $('[id$=ddlblngCntry]').val();
            var res6 = $('[id$=txtblngstrt]').val();
            var res8 = $('[id$=txttpn]').val();
            var res9 = $('[id$=txtfx]').val();
            var res10 = $('[id$=txtfx2]').val();
            var res11 = $('[id$=txtpem]').val();
            var res12 = $('[id$=txtsem]').val();
            var res13 = $('[id$=ddlRegion]').val();
            if (res.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Organisation Name is Required.</span>');
                $('[id$=txtorgnm]').focus();
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
                $('[id$=divMyMessage]').append('<span class="Error">Mobile Number is Required.</span>');
                $('[id$=txtmbl]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res2.trim() == res8.trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Mobile and Telephone No should not be same.</span>');
                $('[id$=txtmbl]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (res3.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Primary E-Mail is Required.</span>');
                $('[id$=txtpem]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res13.trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Region is Required.</span>');
                $('[id$=ddlRegion]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res11.trim() == res12.trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Primary Email and Secondary Email should not be same.</span>');
                $('[id$=txtsem]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res9.trim() == res2.trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Mobile and FAX1 should not be same.</span>');
                $('[id$=txtfx]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }

            else if (res10.trim() == res2.trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Mobile and FAX2 should not be same.</span>');
                $('[id$=txtfx2]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res9.trim() != '' && res8.trim() != '' && res9.trim() == res8.trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Telephone and FAX1 should not be same.</span>');
                $('[id$=txtfx]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }

            else if (res8.trim() != '' && res10.trim() != '' && res10.trim() == res8.trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Telephone and FAX2 should not be same.</span>');
                $('[id$=txtfx2]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res9.trim() != '' && res10.trim() != '' && res9.trim() == res10.trim()) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">FAX1 and FAX2 should not be same.</span>');
                $('[id$=txtfx2]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res4.trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Billing Country is Required.</span>');
                $('[id$=ddlblngCntry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (res6.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Billing Street is Required.</span>');
                $('[id$=txtblngstrt]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }

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
                //$('[id$=txtmbl]').val('');txtfx2
                ErrorMessage('Mobile No. and Fax No. should not be same.');
            }
        }
        function CheckT_F1() {
            var Telephone = $('[id$=txtfx]').val();
            var Mobile = $('[id$=txtfx2]').val();
            if (Telephone != '' && Mobile != '' && Telephone == Mobile) {
                $('[id$=txtfx2]').val('');
                ErrorMessage('Fax No. - 2 and Fax No. - 1 should not be same.');
            }
        }
        function CheckT_E() {
            var Telephone = $('[id$=txtpem]').val();
            var Mobile = $('[id$=txtsem]').val();
            if (Telephone != '' && Mobile != '' && Telephone == Mobile) {
                $('[id$=txtsem]').val('');
                ErrorMessage('Primary and Secondry E-Mails should not be same.');
            }
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
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
       
    </script>
</asp:Content>
