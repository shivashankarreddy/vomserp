<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="SupplierUnits.aspx.cs" Inherits="VOMS_ERP.Masters.SupplierUnits"
    EnableEventValidation="false" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Supplier Units" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span6" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <%--new row added--%>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span10" class="bcLabel">Supplier<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlSuplr" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Supplier" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" colspan="4">
                                        &nbsp
                                    </td>
                                </tr>
                                <%--new row end--%>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblOrg" class="bcLabel">Name of Unit<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtorgnm" onchange="SearchCmpnyNm()" ValidationGroup="D"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBName" class="bcLabel">Business Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtbsnm" onchange="SearchBsNm()" ValidationGroup="D"
                                            CssClass="bcAsptextbox" MaxLength="100"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblTelPh" class="bcLabel">Telephone:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txttpn" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onkeypress="return isTelNo(event)" onchange="CheckT_M(this.id)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblMobile" class="bcLabel">Mobile <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtmbl" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="12" onkeypress="return isNumberKey(event)" onchange="CheckT_M(this.id)"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpem" class="bcLabel">Primary Email<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtpem" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="70" onblur="validateEmail(this);" onchange="GetPMailID();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblSEM" class="bcLabel">Secondary Email:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtsem" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="70" onblur="validateEmail(this);" onchange="CheckT_E()"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblFax" class="bcLabel">Fax1:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfx" ValidationGroup="D" CssClass="bcAsptextbox"
                                            MaxLength="15" onkeypress="return isNumberKey(event)" onchange="CheckT_F()"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblCity" class="bcLabel">Category<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlctgry" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Category"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Books"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblcont" class="bcLabel">Contact Person:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <table width="100%" style="border-width: 0px;">
                                            <tr>
                                                <td style="float: left;">
                                                    <asp:DropDownList ID="ddlHonorific" runat="server" CssClass="bcAspdropdown" Style="width: 60px;">
                                                        <asp:ListItem Text="Select" Value="Select"></asp:ListItem>
                                                        <asp:ListItem Text="Mr." Value="Mr."></asp:ListItem>
                                                        <asp:ListItem Text="Ms." Value="Ms."></asp:ListItem>
                                                        <asp:ListItem Text="Mrs." Value="Mrs."></asp:ListItem>
                                                        <asp:ListItem Text="Miss." Value="Miss."></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="float: left;">
                                                    <asp:TextBox runat="server" ID="txtCtPrsn" ValidationGroup="D" onkeypress="return isAlphaKey(event)"
                                                        CssClass="bcAsptextbox" MaxLength="93" Style="width: 115px;"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <%--new row Begin--%>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Range:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRange" ValidationGroup="D" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 ]/g,'');"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Division :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtDivision" ValidationGroup="D" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 ]/g,'');"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Commissionerate:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCommissionerate" ValidationGroup="D" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 ]/g,'');"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <%--new row Begin--%>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">Range Address :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRngAdrs" ValidationGroup="D" TextMode="MultiLine"
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 ]/g,'');" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span12" class="bcLabel">Division Address :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtDvsnAdrs" TextMode="MultiLine" ValidationGroup="D"
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 ]/g,'');" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span13" class="bcLabel">Commissionerate Address :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCmsnAdrs" TextMode="MultiLine" ValidationGroup="D"
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 ]/g,'');" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                </tr>
                                <%--new row end--%>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        Address Information
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblbcuntry" class="bcLabel">Billing Country<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:UpdateDropDownList('ddlblngCntry', 'Supplier.Aspx', 'ddlblngSt')"--%>
                                        <asp:DropDownList runat="server" ID="ddlblngCntry" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Country" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown1" runat="server" Category="Country"
                                            TargetControlID="ddlblngCntry" PromptText="Select Country" PromptValue="0" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCountrydropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblbstate" class="bcLabel">Billing State<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:UpdateDropDownList('ddlblngSt', 'Supplier.Aspx', 'ddlBilngCty');getval('hdfdblngSt', 'ddlblngSt')"--%>
                                        <asp:DropDownList runat="server" ID="ddlblngSt" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select State" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown2" runat="server" Category="State" TargetControlID="ddlblngSt"
                                            ParentControlID="ddlblngCntry" PromptText="Select State" PromptValue="0" LoadingText="Loading States..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindStatedropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBCity" class="bcLabel">Billing City<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:getval('hdfdblngCty', 'ddlBilngCty')"--%>
                                        <asp:DropDownList runat="server" ID="ddlBilngCty" CssClass="bcAspdropdown">
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
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9# ]/g,'');" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpin" class="bcLabel">Billing PIN/PO Box:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtblngpb" onkeypress="return noSplChar(event)" ValidationGroup="D"
                                            onkeyup="this.value=this.value.replace(/[^0-9]/g,'');" MaxLength="10" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Shipping Country:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--onchange="javascript:UpdateDropDownList('ddlshpngCntry', 'Supplier.Aspx', 'ddlshpngSt')"--%>
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
                                        <%--onchange="javascript:UpdateDropDownList('ddlshpngSt', 'Supplier.Aspx', 'ddlshpngCty');getval('hdfdShpngSt', 'ddlshpngSt')"--%>
                                        <asp:DropDownList runat="server" ID="ddlshpngSt" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select State" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown5" runat="server" Category="State" TargetControlID="ddlshpngSt"
                                            ParentControlID="ddlshpngCntry" PromptText="Select State" PromptValue="0" LoadingText="Loading States..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindStatedropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td style="width: 15%" class="bcTdNewTable">
                                        <span id="Span5" class="bcLabel">Shipping City:</span>
                                    </td>
                                    <td style="width: 15%" class="bcTdNewTable">
                                        <%--onchange="javascript:getval('hdfdshpngCty', 'ddlshpngCty')"--%>
                                        <asp:DropDownList runat="server" ID="ddlshpngCty" CssClass="bcAspdropdown">
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
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9# ]/g,'');" ValidationGroup="D"
                                            MaxLength="200" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblshpin" class="bcLabel">Shipping PIN/PO Box:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtshpngpb" onkeypress="return noSplChar(event)"
                                            onkeyup="this.value=this.value.replace(/[^0-9]/g,'');" ValidationGroup="D" MaxLength="10"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                       <td class="bcTdnormal">
                                        <span id="Span15" class="bcLabel">GST :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtGST" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span16" class="bcLabel">IEC No:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtIecNo" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        Bank Details
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblBank" class="bcLabel">Name of the Bank:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlBanks" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Bank" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Andhra Bank" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="State Bank" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Account Number:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtacntnbr" ValidationGroup="D" MaxLength="25" CssClass="bcAsptextbox"
                                            onkeypress="return isNumberKey(event)"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Branch Name:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtbrncNm" ValidationGroup="D" MaxLength="200" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">RTGS Code:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtrtgscd" ValidationGroup="D" MaxLength="50" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span14" class="bcLabel">Currency :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCurrency" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Currency" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:HiddenField runat="server" ID="hdfdsuplrID" />
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
                                                                <%--OnClientClick="Javascript:clearAll()"--%>
                                                                <asp:LinkButton runat="server" ID="LinkButton2" Text="Clear" OnClick="LinkButton2_Click" />
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
                                            <asp:GridView runat="server" ID="gvsupplier" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                                Width="100%" OnRowCommand="gvsupplier_RowCommand" OnRowDataBound="gvsupplier_RowDataBound"
                                                OnPreRender="gvsupplier_PreRender">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Unit Name">
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
                                                            <asp:Label ID="lblphonembl" runat="server" Text='<%# String.Format("{0}/{1}", Eval("Phone"), Eval("Mobile")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Primary E-Mail">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblprieml" runat="server" Text='<%#Eval("PriEmail") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Supplier">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblParentSupplier" runat="server" Text='<%#Eval("ParentSupplier") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Supplier Code" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblsuplrcd" runat="server" Text='<%#Eval("ID") %>'></asp:Label>
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
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvsupplier]").dataTable({
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
            oTable = $("[id$=gvsupplier]").dataTable();
        });


        function CheckT_M(ID) {
            var Telephone = $('[id$=txttpn]').val();
            var Mobile = $('[id$=txtmbl]').val();
            if (Telephone == Mobile) {
                ErrorMessage('Telephone No. and Mobile No. should not be same.');
                $('[id$=' + ID + ']').val('');
                $('[id$=' + ID + ']').focus();
            }
        }
        function CheckT_F() {
            var Telephone = $('[id$=txtfx]').val();
            var Mobile = $('[id$=txtmbl]').val();
            if (Telephone == Mobile) {
                $('[id$=txtfx]').val('');
                //$('[id$=txtmbl]').val('');
                ErrorMessage('Mobile No. and Fax No. should not be same.');
            }
        }
        function CheckT_E() {
            var Telephone = $('[id$=txtpem]').val();
            var Mobile = $('[id$=txtsem]').val();
            if (Telephone == Mobile) {
                //$('[id$=txtpem]').val('');
                $('[id$=txtsem]').val('');
                ErrorMessage('Primary and Secondry E-Mails should not be same.');
            }
        }

        function GetPMailID() {
            var refNo = $('[id$=txtpem]').val();
            oTable.fnFilter(refNo, 4);
            //var result = Supplier.GetPMailID(refNo);
            //if (result.value == false) {
            if ($('[id$=gvsupplier] >tbody >tr >td').length > 1) {
                $('[id$=txtpem]').val('');
                $('[id$=txtpem]').focus();
                ErrorMessage('This Primary Mail is already in Use');
            }
            else
                oTable.fnFilter('', 4);
        }

        function SearchCmpnyNm() {
            var value1 = $('[id$=txtorgnm]').val();
            oTable.fnFilter(value1, 1);
            if ($('[id$=gvsupplier] >tbody >tr >td').length > 1) {
                ErrorMessage('Supplier Unit Name already Exists');
                $('[id$=txtorgnm]').val('');
            }
            oTable.fnFilter('', 1);
        }
        function SearchBsNm() {
            var value1 = $('[id$=txtbsnm]').val();
            oTable.fnFilter(value1, 2);
            if ($('[id$=gvsupplier] >tbody >tr >td').length > 1) {
                ErrorMessage('Unit Business Name already Exists');
                $('[id$=txtbsnm]').val('');
            }
            oTable.fnFilter('', 2);
        }

        function Myvalidations() {
            if (($('[id$=ddlSuplr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Supplier is Required.');
                $('[id$=ddlSuplr]').focus();
                return false;
            }
            else if (($('[id$=txtorgnm]').val()).trim() == '') {
                ErrorMessage('Unit Name is Required.');
                $('[id$=txtorgnm]').focus();
                return false;
            }
            else if (($('[id$=txtbsnm]').val()).trim() == '') {
                ErrorMessage('Business Name is Required.');
                $('[id$=txtbsnm]').focus();
                return false;
            }
            else if (($('[id$=txtCtPrsn]').val()).trim() != '') {
                if (($('[id$=ddlHonorific]').val()).trim() == 'Select') {
                    ErrorMessage('Title is Required.');
                    $('[id$=ddlHonorific]').focus();
                    return false;
                }
            }
            if (($('[id$=txtmbl]').val()).trim() == '') {
                ErrorMessage('Mobile Number is Required.');
                $('[id$=txtmbl]').focus();
                return false;
            }
            else if (($('[id$=txtmbl]').val()).trim() != '' && ($('[id$=txttpn]').val()).trim() != '' &&
            ($('[id$=txtmbl]').val()).trim() == ($('[id$=txttpn]').val()).trim()) {
                ErrorMessage('Mobile and Telephone should not be same.');
                $('[id$=txtmbl]').focus();
                return false;
            }
            else if (($('[id$=txtpem]').val()).trim() == '') {
                ErrorMessage('Primary E-Mail is Required.');
                $('[id$=txtpem]').focus();
                return false;
            }
            else if (($('[id$=txtpem]').val()).trim() == $('[id$=txtsem]').val().trim()) {
                ErrorMessage('Primary Email and Secondary Email should not be same.');
                $('[id$=txtpem]').focus();
                return false;
            }
            else if (($('[id$=ddlctgry]').val()).trim() == '0') {
                ErrorMessage('Category is Required.');
                $('[id$=ddlctgry]').focus();
                return false;
            }
            else if (($('[id$=txtmbl]').val()).trim() != '' && ($('[id$=txtfx]').val()).trim() != '' &&
            ($('[id$=txtmbl]').val()).trim() == $('[id$=txtfx]').val().trim()) {
                ErrorMessage('Mobile and Fax1 should not be same.');
                $('[id$=txtmbl]').focus();
                return false;
            }
            //            else if (($('[id$=txttpn]').val()).trim() != '' && ($('[id$=txtfx]').val()).trim() != '' &&
            //            ($('[id$=txttpn]').val()).trim() == $('[id$=txtfx]').val().trim()) {
            //                ErrorMessage('Telephone and Fax1 should not be same.');
            //                $('[id$=txttpn]').focus();
            //                return false;
            //            }
            else if (($('[id$=ddlblngCntry]').val()).trim() == '0') {
                ErrorMessage('Billing Country is Required.');
                $('[id$=ddlblngCntry]').focus();
                return false;
            }
            else if (($('[id$=ddlblngSt]').val()).trim() == '0') {
                ErrorMessage('Billing State is Required.');
                $('[id$=ddlblngSt]').focus();
                return false;
            }
            else if (($('[id$=ddlBilngCty]').val()).trim() == '0') {
                ErrorMessage('Billing City is Required.');
                $('[id$=ddlBilngCty]').focus();
                return false;
            }
            else if (($('[id$=txtblngstrt]').val()).trim() == '') {
                ErrorMessage('Billing Street is Required.');
                $('[id$=txtblngstrt]').focus();
                return false;
            }
            else {
                return true;
            }
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function isAlphaKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 32 && charCode != 8 && (charCode < 65 && charCode < 97 || charCode > 90 && charCode > 122))
                return false;
            return true;
        }
        function noSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 44 && charCode != 45 && charCode != 46 && charCode != 47 && charCode != 58 && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function isTelNo(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode != 45 && charCode > 31
            && (charCode < 48 || charCode > 57))
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
        //        // E-Mail Validation
        //        function validateEmail(emailField) {
        //            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        //            if (emailField.value == '') {
        //                return true;
        //            }
        //            else if (reg.test(emailField.value) == false) {
        //                emailField.value = '';
        //                emailField.focus();
        //                alert('invalid Email-ID');
        //                return false;
        //            }
        //            return true;
        //        }      

    </script>
</asp:Content>
