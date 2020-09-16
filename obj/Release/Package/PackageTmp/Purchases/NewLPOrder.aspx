<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="NewLPOrder.aspx.cs" Inherits="VOMS_ERP.Purchases.NewLPOrder" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table  class="MainTable" align="center">
        <tr>
            <td>
                <table>
                    <tr>
                        <td class="bcTdNewTable">
                            <table style="width: 98%;" align="center">
                                <tr class="bcTRTitleRow">
                                    <td class="bcTdTitleLeft" align="left" colspan="6">
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Local Purchase Order"
                                                        CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                            class="formError1" />
                                                </td>
                                                <td style="text-align: right;" colspan="6">
                                                    <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                                        fields are Mandatory</span>
                                                </td>
                                            </tr>
                                        </table>
                                </tr>
                            </table>
                        </td>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                    align="center">
                    <tr>
                        <td>
                            <span class="bcLabel">Customer Name :<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="bcAspdropdown" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:HiddenField ID="HFID" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Foreign Purchase Order No.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox ID="ListBoxFPO" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ddlFpoNo_SelectedIndexChanged">
                            </asp:ListBox>
                            <%--<asp:DropDownList runat="server" ID="ddlFpoNo" CssClass="bcAspdropdown" Height="22px"
                                OnSelectedIndexChanged="ddlFpoNo_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="0" Text="Select FPO"></asp:ListItem>
                            </asp:DropDownList>--%>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Department<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlRsdby" CssClass="bcAspdropdown">
                                <asp:ListItem Value="0" Text="Select Departmet"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Foreign Enquiry No.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox ID="ListBoxFEO" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ddlfenq_SelectedIndexChanged">
                            </asp:ListBox>
                            <%--<asp:DropDownList runat="server" ID="ddlfenq" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlfenq_SelectedIndexChanged"
                                Enabled="false">
                                <asp:ListItem Value="0" Text="Select F-Enquiry"></asp:ListItem>
                                <asp:ListItem Value="1" Text="DCGL-875"></asp:ListItem>
                            </asp:DropDownList>--%>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Supplier Category:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlsuplrctgry" CssClass="bcAspdropdown">
                                <asp:ListItem Value="0" Text="Select Category"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <span class="bcLabel">Supplier Name<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlSuplr" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlSuplr_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Text="Select Supplier" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <span class="bcLabel">Local Quotation No.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox ID="ListBoxLPO" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ddlLQuoteNo_SelectedIndexChanged">
                            </asp:ListBox>
                            <%-- <asp:DropDownList runat="server" ID="ddlLQuoteNo" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlLQuoteNo_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Text="Select Lcl Quotation" Value="0"></asp:ListItem>
                            </asp:DropDownList>--%>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Local PurchaseOrder No.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtLpono" CssClass="bcAsptextbox" onchange="CheckDuplicates()"></asp:TextBox>
                            <%--Enabled="false"--%>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">LPO Date<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:HiddenField ID="hfFPODt" runat="server" />
                            <asp:TextBox runat="server" ID="txtLpoDt" onchange="ChangeDueDate();" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">LPO Due Date<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtLpoDueDt" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Subject<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtsubject" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Important Instructions: </span>
                        </td>
                        <td class="bcTdnormal" colspan="4">
                            <asp:TextBox runat="server" ID="txtimpinst" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                Style="width: 525px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="bcTdnormal">
                            <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                <table width="100%">
                                    <tr>
                                        <td align="left" width="44%">
                                            <span id="Span2" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                        </td>
                                        <td align="center">
                                            <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                TextMode="MultiLine" Rows="4"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdNewTable" colspan="6">
                            <%--<div style="min-height: 100px; max-height: 350px; overflow: auto;">
                            <asp:GridView runat="server" ID="gvLpoItems" Width="100%" RowStyle-CssClass="rounded-corner"
                                EmptyDataRowStyle-CssClass="rounded-corner" CssClass="rounded-corner" HeaderStyle-CssClass="rounded-corner"
                                AlternatingRowStyle-CssClass="rounded-corner" BorderWidth="0px" GridLines="None"
                                AutoGenerateColumns="false" OnRowDataBound="gvLpoItems_RowDataBound" ShowFooter="true"
                                OnPreRender="gvLpoItems_PreRender">
                                <AlternatingRowStyle CssClass="rounded-corner"></AlternatingRowStyle>
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox runat="server" ID="HdrChkbx" OnCheckedChanged="HdrChkbx_OnCheckedChanged"
                                                AutoPostBack="true" />
                                            <%--onclick='SelectAll(this)'--%>
                            <%--            </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="ItmChkbx" OnCheckedChanged="ItmChkbx_OnCheckedChanged"
                                                AutoPostBack="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="S.No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex+1 %>
                                            <asp:HiddenField ID="hfFESNo" runat="server" Value='<%# Eval("FESNo") %>'></asp:HiddenField>
                                            <asp:HiddenField ID="HfItemID" runat="server" Value='<%# Eval("ItemId") %>'></asp:HiddenField>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item Description" ItemStyle-Width="150px" HeaderStyle-Width="150px">
                                        <ItemTemplate>
                                            <div class='expanderR'>
                                                <asp:Label ID="lblItemDesc" runat="server" Text='<%# Eval("ItemDesc") %>' ToolTip='<%# Eval("ItemDesc") %>' />
                                            </div>
                                            <asp:Label ID="lblItemID" runat="server" Visible="false" Text='<%# Eval("ItemId") %>' />
                                            <asp:Label ID="lblItemDetailsID" runat="server" Visible="false" Text='<%# Eval("ItemDetailsId") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPOrderId" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLPOrderId" runat="server" Text='0' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Part No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrtNo" runat="server" Text='<%# Eval("PartNumber") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Specifications">
                                        <ItemTemplate>
                                            <div class='expanderR'>
                                                <asp:Label ID="lblSpec" runat="server" Text='<%# Eval("Specifications") %>' ToolTip='<%# Eval("Specifications") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Make">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMake" runat="server" Text='<%# Eval("Make") %>' />
                                            <asp:HiddenField ID="HFExdtPrcnt" runat="server" Value='<%# Eval("ExDutyPercentage") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txtQty" Width="80px" CssClass="bcAsptextbox" Text='<%#Eval("qty") %>'
                                                onkeyup="extractNumber(this,2,false);" onblur="extractNumber(this,2,false);"
                                                onkeypress="return blockNonNumbers(this, event, true, false);" OnTextChanged="txtQty_TextChanged"
                                                AutoPostBack="true"></asp:TextBox>
                                            <asp:HiddenField runat="server" ID="HF_RCVDQty5" Value="0" />
                                            <asp:HiddenField runat="server" ID="HF_ActualQty5" Value='0' />
                                            <asp:Label ID="lblQty" runat="server" Text='<%# Eval("qty") %>' Visible="false" />
                                            <%--<asp:HiddenField runat="server" ID="hfQTYsum" Value='<%# Eval("QTYsum") %>' />--%>
                            <%-- </ItemTemplate>
                                        <FooterTemplate>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Unit Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUnitName" runat="server" Text='<%# Eval("UnitName") %>' />
                                            <asp:Label ID="lblUnitID" runat="server" Visible="false" Text='<%# Eval("UNumsId") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            Total
                                        </FooterTemplate>
                                        <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                        <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate" HeaderStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"
                                        ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRate" runat="server" Text='<%# Eval("rate") %>' />
                                            <asp:TextBox runat="server" ID="txtRate" Width="80px" CssClass="bcAsptextbox" Text='<%#Eval("rate") %>'
                                                onkeyup="extractNumber(this,2,false);" onblur="extractNumber(this,2,false);"
                                                onkeypress="return blockNonNumbers(this, event, true, false);" onchange="CheckRate(this);"
                                                OnTextChanged="txtQty_TextChanged" AutoPostBack="true" Visible="false"></asp:TextBox>
                                            <asp:HiddenField runat="server" ID="HF_CngPrice" Value='<%#Eval("rate") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="LblAmo" runat="server" Text="Amount :"></asp:Label>
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="left"></FooterStyle>
                                        <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount" HeaderStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"
                                        ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("totalAmt") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label runat="server" ID="lbltmnt"></asp:Label>
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                        <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ex-Dt(%)" Visible="true">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExdtPrcnt" runat="server" Text='<%# Eval("ExDutyPercentage") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Disc(%)" Visible="true">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDscntPrcnt" runat="server" Text='<%# Eval("DiscountPercentage") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            Grand
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                        <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Net-Rate" Visible="true">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNRate" runat="server" Text='<%# Eval("QPrice") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="LblTotAmount" runat="server" Text="Total :"></asp:Label>
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                        <FooterStyle HorizontalAlign="Left"></FooterStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Remarks" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRmrks" runat="server" Text='<%# Eval("Remarks") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"
                                        ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("totalAmt") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label runat="server" ID="lblTotl"></asp:Label>
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                        <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataRowStyle CssClass="rounded-corner"></EmptyDataRowStyle>
                                <HeaderStyle CssClass="rounded-corner"></HeaderStyle>
                                <RowStyle CssClass="rounded-corner"></RowStyle>
                            </asp:GridView>
                        </div>--%>
                            <div id="divLPOItems" runat="server" style="overflow: auto; width: 100%; max-height: 250px;
                                min-height: 200px;">
                            </div>
                        </td>
                    </tr>
                    <tr style="background-color: Gray; font-style: normal; color: White;">
                        <td colspan="6">
                            Setting Terms
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdNewTable" colspan="6">
                            <table style="width: 100%; overflow: auto;">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Drawing Approvals: </span>
                                        <asp:CheckBox runat="server" ID="ChkbDrwngAprls" Text="" onclick='CHeck("ChkbDrwngAprls", "dvDrwngAprls")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvDrwngAprls" style="display: none;">
                                            <span class="bcLabel">Remainder Time <font color="red" size="2"><b>*</b></font>:</span>
                                            <asp:TextBox runat="server" ID="txtDrwngAprls" CssClass="bcAsptextbox" onblur="extractNumber(this,0,false);"
                                                onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                Width="23px" MaxLength="2" onfocus="this.select()" onMouseUp="return false" onchange="CheckDP(this.id)"></asp:TextBox>
                                            <span class="bcLabel">Week(s)</span></div>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Inspection: </span>
                                        <asp:CheckBox runat="server" ID="ChkbInspcn" Text="" onclick='CHeck("ChkbInspcn", "dvInsptn")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvInsptn" style="display: none;">
                                            <span class="bcLabel">Remainder Time <font color="red" size="2"><b>*</b></font>:</span>
                                            <asp:TextBox runat="server" ID="txtInsptn" CssClass="bcAsptextbox" onblur="extractNumber(this,0,false);"
                                                onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                Width="23px" MaxLength="2" onfocus="this.select()" onMouseUp="return false" onchange="CheckDP(this.id)"></asp:TextBox>
                                            <span class="bcLabel">Week(s)</span></div>
                                    </td>
                                    <td class="bcTdnormal" style="visibility: hidden;">
                                        <span runat="server" id="ECEA_spn_txt" class="bcLabel">Excise Duty Exemption Applicable:
                                        </span>
                                        <asp:CheckBox runat="server" ID="ChkbCEEApl" Text="" onclick='CHeck("ChkbCEEApl", "dvCEEApl")'
                                            CssClass="bcCheckBoxList" Visible="false" Enabled="false" />
                                    </td>
                                    <td style="visibility: hidden;">
                                        <div id="dvCEEApl" runat="server" style="display: none;">
                                            <span runat="server" class="bcLabel">Remainder Time <font color="red" size="2"><b>*</b></font>:</span>
                                            <asp:TextBox runat="server" ID="txtCEEApl" CssClass="bcAsptextbox" onblur="extractNumber(this,0,false);"
                                                onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                Width="23px" MaxLength="2" onfocus="this.select()" onMouseUp="return false" onchange="CheckDP(this.id)"></asp:TextBox>
                                            <span class="bcLabel">Week(s)</span>
                                        </div>
                                        <asp:HiddenField ID="hdfldCstmr" runat="server" Value="0" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr style="background-color: Gray; font-style: normal; color: White;">
                        <td colspan="6">
                            Terms & Conditions
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdNewTable" colspan="8">
                            <table style="width: 100%; overflow: auto;">
                                <tr>
                                    <td>
                                        <span id="lblED" runat="server" class="bcLabel">CGST:</span>
                                        <asp:CheckBox runat="server" ID="chkExdt" Text=" " onclick='CHeck("chkExdt", "dvExdt")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvExdt" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnExdt" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtExdt" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                onchange="CalculateExDuty();" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                Style="text-align: right;" Width="35px" MaxLength="5"></asp:TextBox></div>
                                    </td>
                                    <td style="display: none">
                                        <span id="Span1" class="bcLabel">Sale Tax:</span>
                                        <asp:CheckBox runat="server" ID="chkSltx" Text=" " onclick='CHeck("chkSltx", "dvSltx")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td style="display: none">
                                        <div id="dvSltx" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnSltx" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtSltx" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                onfocus="this.select()" onMouseUp="return false" Width="35px" Style="text-align: right;"
                                                MaxLength="5"></asp:TextBox></div>
                                    </td>
                                    <%--Modified by Satya :: For GST Implementation :: START--%>
                                    <td>
                                        <span id="Span5" runat="server" class="bcLabel">SGST:</span>
                                        <asp:CheckBox runat="server" ID="chkSGST" Text=" " onclick='CHeck("chkSGST", "dvSGST")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvSGST" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnSGST" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtSGST" MaxLength="5" onblur="extractNumber(this,2,false);"
                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                onkeypress="return blockNonNumbers(this, event, true, false);" onchange="CalculateExDuty();"
                                                Style="text-align: right;" Width="40px" CssClass="bcAsptextbox"></asp:TextBox></div>
                                    </td>
                                    <td>
                                        <span id="Span6" runat="server" class="bcLabel">IGST:</span>
                                        <asp:CheckBox runat="server" ID="chkIGST" Text=" " onclick='CHeck("chkIGST", "dvIGST")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvIGST" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnIGST" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtIGST" MaxLength="5" onblur="extractNumber(this,2,false);"
                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                onkeypress="return blockNonNumbers(this, event, true, false);" onchange="CalculateExDuty();"
                                                Style="text-align: right;" Width="40px" CssClass="bcAsptextbox"></asp:TextBox></div>
                                    </td>
                                    <%--Modified by Satya :: For GST Implementation :: END--%>
                                    <td>
                                        <span id="lblPck" class="bcLabel">Packing:</span>
                                        <asp:CheckBox runat="server" ID="chkPkng" Text=" " onclick='CHeck("chkPkng", "dvPkng")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvPkng" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnPkng" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtPkng" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                onkeypress="return blockNonNumbers(this, event, true, false);" Style="text-align: right;"
                                                Width="35px" MaxLength="5"></asp:TextBox></div>
                                    </td>
                                    <td>
                                        <span id="lblDis" class="bcLabel">Discount:</span>
                                        <asp:CheckBox runat="server" ID="chkDsnt" Text=" " onclick='CHeckForLpo("chkDsnt", "dvDsnt")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvDsnt" runat="server" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnDsnt" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Rs" Value="1"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtDsnt" Text="0" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                onchange="CalculateExDuty();" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                Style="text-align: right;" Width="35px" MaxLength="10"></asp:TextBox></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="8">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Price Basis<font color="red" size="2"><b>*</b></font>:</span>
                                                </td>
                                                <td class="bcTdnormal" style="margin: 0px; width: 183px">
                                                    <asp:DropDownList runat="server" ID="ddlPrcBsis" CssClass="bcAspdropdown">
                                                        <asp:ListItem Text="Select Price Basis" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox runat="server" class="bcAsptextboxmulti" onfocus="ExpandTXT(this.id)"
                                                        onblur="ReSizeTXT(this.id)" Style="height: 20px; width: 150px;" ID="txtPriceBasis"
                                                        TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Delivery Period<font color="red" size="2"><b>*</b></font>:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtDlvry" CssClass="bcAsptextbox" onchange="ChangeDueDate()"
                                                        onfocus="this.select()" onMouseUp="return false" Width="23px" MaxLength="2" onkeyup="extractNumber(this, 0, false);"
                                                        onblur="extractNumber(this, 0, false); chkDlvrPeriod()"></asp:TextBox><span class="bcLabel">Weeks</span>
                                                </td>
                                                <td>
                                                </td>
                                                <td colspan="2">
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="10">
                                        Payment Terms :
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="10" class="bcTdNewTable">
                                        <center>
                                            <div style="overflow: auto; width: 35%;" id="divPaymentTerms" runat="server">
                                            </div>
                                        </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="10" class="bcTdNewTable">
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="UserAccordion" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane3" runat="server">
                                                        <Header>
                                                            <a href="#" class="href">Attachments</a></Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel2" runat="server" Width="98%">
                                                                <table>
                                                                    <tr>
                                                                        <td colspan="3">
                                                                            <asp:Label ID="lblstatus" runat="server" Style="font-family: Arial; font-size: small;"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td valign="top">
                                                                            <ajax:AsyncFileUpload ID="AsyncFileUpload1" runat="server" OnClientUploadError="uploadError"
                                                                                OnClientUploadComplete="uploadComplete" OnClientUploadStarted="uploadStarted"
                                                                                UploaderStyle="Modern" CompleteBackColor="LightGreen" UploadingBackColor="Yellow"
                                                                                ThrobberID="ThrobberImg" OnUploadedComplete="FileUploadComplete" CssClass="FileUploadClass" />
                                                                            <asp:Image runat="server" ID="ThrobberImg" ImageUrl="~/images/uploadingImage.gif"
                                                                                AlternateText="loading" />
                                                                        </td>
                                                                        <td valign="top">
                                                                            <div id="divListBox" runat="server" width="221px">
                                                                            </div>
                                                                        </td>
                                                                        <td valign="top">
                                                                            <a id="lnkdelete" href="javascript:void(0)">Delete Item</a>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </Content>
                                                    </ajax:AccordionPane>
                                                </Panes>
                                            </ajax:Accordion>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="10" class="bcTdNewTable" align="right">
                                        <span><a href="javascript:void(0)" id="lbtnATConditions" title="Add Addtional Terms & Conditions"
                                            onclick="fnOpen()" class="bcAlink">Additional Terms & Conditions </a></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="bcTdNewTable" colspan="6">
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
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/date.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery.blockUI.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            if ($("[id$=rbtnDsnt]").find(":checked").val() == 1) {
                $("[id$=txtDsnt]").width(100);
            }
            else {
                $("[id$=txtDsnt]").width(35);
            }
        });
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $('[id$=rbtnExdt]').change(function () {
            if ($("[id$=rbtnExdt]").find(":checked").val() == 1) {
                $("[id$=txtExdt]").width(100);
                $('[id$=txtExdt]').val(0);
            }
            else {
                $("[id$=txtExdt]").width(35);
                $('[id$=txtExdt]').val(0);
            }
        });
        $('[id$=rbtnSltx]').change(function () {
            if ($("[id$=rbtnSltx]").find(":checked").val() == 1) {
                $("[id$=txtSltx]").width(100);
                $('[id$=txtSltx]').val(0);
            }
            else {
                $("[id$=txtSltx]").width(35);
                $('[id$=txtSltx]').val(0);
            }
        });
        $('[id$=rbtnPkng]').change(function () {
            if ($("[id$=rbtnPkng]").find(":checked").val() == 1) {
                $("[id$=txtPkng]").width(100);
                $('[id$=txtPkng]').val(0);
            }
            else {
                $("[id$=txtPkng]").width(35);
                $('[id$=txtPkng]').val(0);
            }
        });
        $('[id$=rbtnDsnt]').change(function () {
            if ($("[id$=rbtnDsnt]").find(":checked").val() == 1) {
                $("[id$=txtDsnt]").width(100);
                $('[id$=txtDsnt]').val(0);

            }
            else {
                $("[id$=txtDsnt]").width(35);
                $('[id$=txtDsnt]').val(0);
            }
        });

        function CHeckForLpo(ckid, dvid, SelVal) {
            try {
                if (SelVal == 1)
                    $('[id$=txtDsnt]').width(100);
                else
                    $('[id$=txtDsnt]').width(35);
                var ChkBox = document.getElementById("ctl00_ContentPlaceHolder1_" + ckid);
                if (ChkBox.checked == true) {
                    $('[id$=' + dvid + ']').css("display", "block");
                    $('div[id*=' + dvid + '] input[type=text]').focus();
                }
                else {
                    $('[id$=' + dvid + ']').css("display", "none");
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }

        function GetQueryStringParams(sParam) {
            var sPageURL = window.location.search.substring(1);
            var sURLVariables = sPageURL.split('&');
            for (var i = 0; i < sURLVariables.length; i++) {
                var sParameterName = sURLVariables[i].split('=');
                if (sParameterName[0] == sParam) {
                    return sParameterName[1];
                }
            }
        }



        function CheckRate(txtRate) {
            try {
                var txtRateval = document.getElementById(txtRate.id).value;
                var ClientID = txtRate.id.split("txtRate");
                if (!txtRateval || 0 === txtRateval.length || parseFloat(txtRateval) == 0) {
                    ErrorMessage("Rate Can't be Zero/Empty");
                }
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }

        $('[id$=txtDsnt]').keypress(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtDsnt]').val() + sv;
            var DcstChk = $("[id$=rbtnDsnt]").find(":checked").val();
            if (discount > 99.99 && DcstChk == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Discount Cannot be Grater than 99.99%</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('[id$=txtDsnt]').val('0');
                return false;
            }
        });
        $('[id$=txtExdt]').keypress(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtExdt]').val() + sv;
            var ExChk = $("[id$=rbtnExdt]").find(":checked").val();
            if (discount > 99.99 && ExChk == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">CGST Cannot be Greater than 99.99%</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('[id$=txtExdt]').val('0');
                return false;
            }
        });
        $('[id$=txtSGST]').keypress(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtSGST]').val() + sv;
            var ExChk = $("[id$=rbtnSGST]").find(":checked").val();
            if (discount > 99.99 && ExChk == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">SGST Cannot be Greater than 99.99%</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('[id$=txtSGST]').val('0');
                return false;
            }
        });
        $('[id$=txtIGST]').keypress(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtIGST]').val() + sv;
            var ExChk = $("[id$=rbtnIGST]").find(":checked").val();
            if (discount > 99.99 && ExChk == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">IGST Cannot be Greater than 99.99%</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('[id$=txtIGST]').val('0');
                return false;
            }
        });
        $('[id$=txtPkng]').keypress(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtPkng]').val() + sv;
            var PkgChk = $("[id$=rbtnPkng]").find(":checked").val();
            if (discount > 99.99 && PkgChk == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Packing Cannot be Grater than 99.99%</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('[id$=txtPkng]').val('0');
                return false;
            }
        });
        $('[id$=txtSltx]').keypress(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtSltx]').val() + sv;
            var SltxChk = $("[id$=rbtnSltx]").find(":checked").val();
            if (discount > 99.99 && SltxChk == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Sales Tax Cannot be Grater than 99.99%</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('[id$=txtSltx]').val('0');
                return false;
            }
        });

        $('[id$=txtDlvry]').keyup(function () {
            if ($('[id$=ChkbDrwngAprls]').is(':checked') && $('[id$=txtDlvry]').val() > 0) {
                var Approval = $('[id$=txtDrwngAprls]').val();
                if (parseInt($('[id$=txtDlvry]').val()) < parseInt(Approval)) {
                    $('[id$=txtDlvry]').val('0');
                    ErrorMessage('Delivery period cannot be less than Drawing Approval.');
                }
            }
            if ($('[id$=ChkbInspcn]').is(':checked') && $('[id$=txtDlvry]').val() > 0) {
                var Approval = $('[id$=txtInsptn]').val();
                if (parseInt($('[id$=txtDlvry]').val()) < parseInt(Approval)) {
                    $('[id$=txtDlvry]').val('0');
                    ErrorMessage('Delivery period cannot be less than Inspection Approval.');
                }
            }
            if ($('[id$=ChkbCEEApl]').is(':checked') && $('[id$=txtDlvry]').val() > 0) {
                var Approval = $('[id$=txtCEEApl]').val();
                if (parseInt($('[id$=txtDlvry]').val()) < parseInt(Approval)) {
                    $('[id$=txtDlvry]').val('0');
                    ErrorMessage('Delivery period cannot be less than ExciseDuty Approval.');
                }
            }
        });
    </script>
    <script type="text/javascript">

        function CheckDuplicates() {
            try {
                var LpoNmbr = $('[id$=txtLpono]').val();
                var result = NewLPOrder.CheckDuplicateLPOs(LpoNmbr);
                if (result.value) {
                    ErrorMessage('LPO Number already Exists');
                    $('[id$=txtLpono]').focus(0, 0);
                    $('[id$=txtLpono]').val('');
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }

        function chkDlvrPeriod() {
            var Currentweeks = $('[id$=txtDlvry]').val();
            if (Currentweeks == 0) {
                ErrorMessage('Delivery Period should not be Zero');
                $('[id$=txtDlvry]').focus(0, 0);
                $('[id$=txtFpoDuedt]').val('');
            }
        }

        function CheckDP(CntrlID) {
            var CntrolVal = $('[id$=' + CntrlID + ']').val();
            var Dlvry = $('[id$=txtDlvry]').val();
            if (parseInt(CntrolVal) > parseInt(Dlvry)) {
                $('[id$=' + CntrlID + ']').val('0');
                $('[id$=' + CntrlID + ']').focus();
                ErrorMessage('Remainder Time should be less than Delivery period.');
            }
        }

        function ChangeDueDate() {
            var Currentweeks = $('[id$=txtDlvry]').val();
            if (Currentweeks == 0) {
                $('[id$=txtDlvry]').focus(0, 0);
                $('[id$=txtLpoDueDt]').val('');
            }
            else {
                var strdate = $('[id$=txtLpoDt]').val();
                var strdate1 = strdate.split('-');
                strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
                strdate = new Date(strdate.replace(/-/g, "/"));
                strdate.setDate(strdate.getDate() + (Currentweeks * 7));
                var month = strdate.getMonth() + 1;
                $('[id$=txtLpoDueDt]').val(("0" + strdate.getDate()).slice(-2) + '-' + ("0" + month).slice(-2) + '-' + strdate.getFullYear());
            }
        }

        function CheckQtys(Sno) {
            try {
                var txtQtyval = $('#txtQuantity' + Sno).val();
                var txtRate = $('#HF_CngPrice' + Sno).val();
                var Hflbltmnt = $('#Hflbltmnt').val();
                var HflblTotl = $('#HflblTotl').val();
                var hfCurrentPage = $('[id$=hfCurrentPage]').val();
                var ddlRowsChanged = $('[id$=ddlRowsChanged]').val();
                var HF_CngPrice = $('#HF_CngPrice' + Sno).val();
                var result = NewLPOrder.CheckQtyAdd(Sno, Hflbltmnt, HflblTotl, HF_CngPrice, txtQtyval, hfCurrentPage, ddlRowsChanged);
                var C_D = result.value;

                //                Hflbltmnt = parseFloat(C_D[0]);
                //                HflblTotl = parseFloat(C_D[1]);
                $('#HflblTotl').val(Hflbltmnt);
                $('#Hflbltmnt').val(HflblTotl);
                //                document.getElementById(Sno.id).value;
                //                var ClientID = Sno.id.split("txtQty");
                if (!txtQtyval || 0 === txtQtyval.length || parseFloat(txtQtyval) == 0) {
                    document.getElementById(txtQty.id).value = $('[id$=' + ClientID[0] + 'Hfd_MQty]').val();
                    ErrorMessage("Quantity Can't be Zero/Empty");
                }
                else if (GetQueryStringParams('ID') != "") {
                    if (parseFloat($('#HF_RCVDQty' + Sno).val()) != 0 &&
                        (parseFloat(txtQtyval) > parseFloat($('#HF_RCVDQty' + Sno).val()))) {
                        ErrorMessage("Quantity Exceeds FPO Quantity/Part Quantity is already released with another PO");
                    }
                    else if (parseFloat(txtQtyval) > parseFloat($('#HF_ActualQty' + Sno).val())) {
                        ErrorMessage("Quantity Exceeds FPO Quantity");
                    }
                }
                else if (parseFloat(txtQtyval) > parseFloat($('#HF_RCVDQty' + Sno).val())) {
                    var RmngQTY = (parseFloat($('#HF_RCVDQty' + Sno).val()) - parseFloat($('#HF_RCVDQty' + Sno).val()));
                    document.getElementById(txtQtyval).focus();
                    alert("Quantity Exceeds FPO Quantity/Part Quantity is already released with another PO");
                }
                //                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                //                $('#' + getDivFEItems).html(result.value);

                var results = NewLPOrder.AddNewRow(Sno, txtQtyval, HF_CngPrice);
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                $('#' + getDivFEItems).html(results.value);
                CalculateExDuty(); 
                Expnder();
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }



        function getPaymentValues(RNo) {
            var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(RNo))).attr("id");
            var PercAmt = $('#' + txtPercAmt).val();
            var txtDesc = GetClientID("txtDesc" + (parseInt(RNo))).attr("id");
            var Desc = $('#' + txtDesc).val();
            if (PercAmt != '0' && PercAmt != '' && Desc != '') {
                var result = NewLPOrder.PaymentAddItem(RNo, PercAmt, Desc);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);
                if ($('[id$=HfMessage]').val() != '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">' + $('[id$=HfMessage]').val() + '</span>');
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    $('[id$=' + txtPercAmt + ']').focus();
                }
                else
                    $('[id$=' + txtDesc + ']').focus();
            }
            else {
                $("#<%=divMyMessage.ClientID %> span").remove();
                if (PercAmt == '') {
                    $('[id$=divMyMessage]').append('<span class="Error">Payment Percentage is Required.</span>');
                }
                else if (PercAmt == '0') {
                    $('[id$=divMyMessage]').append('<span class="Error">Payment Percentage cannot be Zero.</span>');
                }
                else if (PercAmt > 100) {
                    ErrorMessage('Percentage Cannot Exceed 100');
                    $('#' + txtPercAmt).val('0');
                    $('[id$=' + txtPercAmt + ']').focus();
                }
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('#' + txtPercAmt).focus();
            }
            $('#' + txtDesc).focus();
        }

        function doConfirmPayment(id) {
            if (confirm("Are you sure you want to Delete Payment?")) {
                var result = NewLPOrder.PaymentDeleteItem(id);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);
            }
            else {
                return false;
            }
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function SpclChars(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 44 && charCode != 45 && charCode != 46 && charCode != 47 && charCode != 58
            && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function Myvalidations() {
            var TotPayAmt = 0;
            var PaymentRCount = $('#tblPaymentTerms tbody tr').length;
            var LPOrderItems = $('#tblItems tbody tr').length;

            if (($('[id$=ddlCustomer]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Customer Name is required');
                $('[id$=ddlCustomer]').focus();
                return false;
            }
            else if ($('#ctl00_ContentPlaceHolder1_ListBoxFPO :selected').length == 0) {
                ErrorMessage('Frn Purchase Order No. is Required.');
                $('[id$=ListBoxFPO]').focus();
                return false;
            }
            else if (($('[id$=ddlRsdby]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Raiseby Dept. is Required.');
                $('[id$=ddlRsdby]').focus();
                return false;
            }
            else if ($('#ctl00_ContentPlaceHolder1_ListBoxFEO :selected').length == 0) {
                ErrorMessage('Frn Enquiry No. is Required.');
                $('[id$=ListBoxFEO]').focus();
                return false;
            }
            else if (($('[id$=ddlSuplr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Supplier is Required.');
                $('[id$=ddlSuplr]').focus();
                return false;
            }
            else if (($('[id$=txtLpono]').val()).trim() == '') {
                ErrorMessage('Lcl Purchase Order Number is Required.');
                $('[id$=txtLpono]').focus();
                return false;
            }
            else if (($('[id$=txtLpoDt]').val()).trim() == '') {
                ErrorMessage('Lcl Purchase Order Date is Required.');
                $('[id$=txtLpoDt]').focus();
                return false;
            }
            else if (($('[id$=txtLpoDueDt]').val()).trim() == '') {
                ErrorMessage('Lcl Purchase Order Due Date is Required.');
                $('[id$=txtLpoDueDt]').focus();
                return false;
            }

            else if ($('#ctl00_ContentPlaceHolder1_ListBoxLPO :selected').length == 0) {
                ErrorMessage('Lcl Quotation No. is Required.');
                $('[id$=ListBoxLPO]').focus();
                return false;
            }
            else if (($('[id$=txtsubject]').val()).trim() == '') {
                ErrorMessage('Subject is Required.');
                $('[id$=txtsubject]').focus();
                return false;
            }
            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            if (LPOrderItems > 0) {
                //                if (LPOrderItems == 0) {
                //                    ErrorMessage('No Items to Save.');
                //                    $('[id$=gvLpoItems]').focus();
                //                    return false;
                //                }
                //                else {
                var select = 0;
                for (var i = 1; i <= LPOrderItems; i++) {
                    var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                    var chkbval = $('#ckhChaild' + i).is(':checked');
                    if (chkbval == true) {
                        select = select + 1;
                    }
                }
                if (select == 0) {
                    ErrorMessage('Select At Least One Item.');
                    $('[id$=gvLpoItems]').focus();
                    return false;
                }
                var result = NewLPOrder.ValidateRowsBeforeSave();
                if (result.value.includes("ERROR::")) {
                    ErrorMessage(result.value.replace("ERROR::", ""));
                    return false;
                }

                //}
            }
            else if (LPOrderItems == 0) {
                ErrorMessage('No Items to Save.');
                $('[id$=gvLpoItems]').focus();
                return false;
            }
            if ($('[id$=ChkbDrwngAprls]')[0].checked == true) {
                var aDrawingAp = ($('[id$=txtDrwngAprls]').val()).trim();
                var aDel = ($('[id$=txtDlvry]').val()).trim();
                if (($('[id$=txtDrwngAprls]').val()).trim() == '') {
                    ErrorMessage('Drawing Approvals Remainder Time is Required.');
                    $('[id$=txtDrwngAprls]').focus();
                    return false;
                }
                else if (parseInt(aDrawingAp) == 0) {
                    ErrorMessage('Drawing Approvals Remainder Time cannot be Zero.');
                    $('[id$=txtDrwngAprls]').focus();
                    return false;
                }
                else if (parseInt(aDrawingAp) > parseInt(aDel)) {
                    ErrorMessage('Drawing Approvals Remainder Time should less than Delivery Period.');
                    $('[id$=txtDrwngAprls]').focus();
                    return false;
                }
            }
            if ($('[id$=ChkbInspcn]')[0].checked == true) {
                if (($('[id$=txtInsptn]').val()).trim() == '') {
                    ErrorMessage('Inspection Remainder Time is Required.');
                    $('[id$=txtInsptn]').focus();
                    return false;
                }
                if (($('[id$=txtInsptn]').val()).trim() == '0') {
                    ErrorMessage('Inspection Remainder Time cannot be zero.');
                    $('[id$=txtInsptn]').focus();
                    return false;
                }
                else if (parseInt(($('[id$=txtInsptn]').val()).trim()) > parseInt(aDel)) {
                    ErrorMessage('Inspection Remainder Time should less than Delivery Period.');
                    $('[id$=txtInsptn]').focus();
                    return false;
                }
            }
            //            if ($('[id$=ChkbCEEApl]')[0].checked == true) {
            //                if (($('[id$=txtCEEApl]').val()).trim() == '') {
            //                    ErrorMessage('CE-Excemption Remainder Time is Required.');
            //                    $('[id$=txtCEEApl]').focus();
            //                    return false;
            //                }
            //                if (($('[id$=txtCEEApl]').val()).trim() == '0') {
            //                    ErrorMessage('CE-Excemption Remainder Time is Required.');
            //                    $('[id$=txtCEEApl]').focus();
            //                    return false;
            //                }
            //                else if (parseInt(($('[id$=txtCEEApl]').val()).trim()) > parseInt(aDel)) {
            //                    ErrorMessage('CE-Excemption Remainder Time should less than Delivery Period.');
            //                    $('[id$=txtCEEApl]').focus();
            //                    return false;
            //                }
            //            }
            if ($('[id$=chkSltx]')[0].checked == true) {
                if (($('[id$=txtSltx]').val()).trim() == '0') {
                    ErrorMessage('Sales Tax is Required.');
                    $('[id$=txtSltx]').focus();
                    return false;
                }
            }
            if ($('[id$=chkPkng]')[0].checked == true) {
                if (($('[id$=txtPkng]').val()).trim() == '0') {
                    ErrorMessage('Packing is Required.');
                    $('[id$=txtPkng]').focus();
                    return false;
                }
            }
            if (($('[id$=ddlPrcBsis]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Price Basis is Required.');
                $('[id$=ddlPrcBsis]').focus();
                return false;
            }
            else if (($('[id$=txtPriceBasis]').val()).trim() == '') {
                ErrorMessage('Price Basis Location is Required.');
                $('[id$=txtPriceBasis]').focus();
                return false;
            }
            else if (($('[id$=txtDlvry]').val()).trim() == '' || $('[id$=txtDlvry]').val() == '0') {
                ErrorMessage('Delivery Period is Required.');
                $('[id$=txtDlvry]').focus();
                return false;
            }
            if (PaymentRCount > 0) {
                for (var k = 1; k <= PaymentRCount; k++) {
                    var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(k))).attr("id");
                    var PercAmt = $('#' + txtPercAmt).val();
                    var txtDesc = GetClientID("txtDesc" + (parseInt(k))).attr("id");
                    var Desc = $('#' + txtDesc).val();
                    TotPayAmt += parseFloat(PercAmt);
                    if (PercAmt == '' || PercAmt == '0' || Desc == '') {
                        var message = '';
                        if (PercAmt == '')
                            message = 'Payment Is Required';
                        else if (PercAmt == '0')
                            message = 'Payment Cannot be Zero';
                        else if (Desc == '')
                            message = 'Description Is Required';
                        ErrorMessage(message + ' of SNo : ' + k);
                        return false;
                        break;
                    }
                }
            }
            if (PaymentRCount == 0 || TotPayAmt < 100) {
                ErrorMessage('Payment has to be 100%.');
                return false;
            }
            if ($('[id$=chkExdt]')[0] != undefined) {
                if ($('[id$=chkExdt]')[0].checked == true) {
                    if (($('[id$=txtExdt]').val()).trim() == '0') {
                        ErrorMessage('CGST is Required.');
                        $('[id$=txtExdt]').focus();
                        return false;
                    }
                }
            }
            if ($('[id$=chkSGST]')[0] != undefined) {
                if ($('[id$=chkSGST]')[0].checked == true) {
                    if (($('[id$=txtSGST]').val()).trim() == '0') {
                        ErrorMessage('SGST is Required.');
                        $('[id$=txtSGST]').focus();
                        return false;
                    }
                }
            }
            if ($('[id$=chkIGST]')[0] != undefined) {
                if ($('[id$=chkIGST]')[0].checked == true) {
                    if (($('[id$=txtIGST]').val()).trim() == '0') {
                        ErrorMessage('IGST is Required.');
                        $('[id$=txtIGST]').focus();
                        return false;
                    }
                }
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


        function fnSetValues() {
            var iHeight = 500;
            var iWidth = 1000;
            var sFeatures = "dialogHeight: " + iHeight + "px; dialogWidth: " + iWidth + "px;";
            return sFeatures;
        }
        function fnOpen(id, rowIndex) {
            var sFeatures = fnSetValues();
            var w = 1000;
            var h = 550;
            var x = (screen.width / 2) - (w / 2);
            var y = (screen.height / 2) - (h / 2);
            var LQuoteID = $('[id$=ListBoxLPO]').val();
            var LQID = LQuoteID.toString().split(',');
            var ED = $('[id$=HFID]').val();
            if (ED == '00000000-0000-0000-0000-000000000000') {
                window.open("../Masters/TermsNConditions.aspx?TAr=LQ/LPO&LclQuteID=" + LQID[0] + "&LPO=true", "9491E7DC-C54E-432D-9694-D044F4DDECAF", "width=1000,height=500, top=" + y + ", left=" + x + "");
            }
            else {
                window.open("../Masters/TermsNConditions.aspx?TAr=LQ/LPO&LclQuteID=" + ED + "&LPO=false", "9491E7DC-C54E-432D-9694-D044F4DDECAF", "width=1000,height=500, top=" + y + ", left=" + x + "");
            }
            //            window.showModalDialog("../Masters/TermsNConditions.aspx?TAr=LQ/LPO&LclQuteID=" + LQID[0], "509", sFeatures);
            //            select * from EnumMaster e where e.Description = 'LQ/LPO'
        }

        function ExpandTXT(CtrlID) {
            $('#' + CtrlID).animate({ "height": "75px" }, "slow");
            $('#' + CtrlID).slideDown("slow");
        }

        function ReSizeTXT(CtrlID) {
            $('#' + CtrlID).animate({ "height": "20px" }, "slow");
            $('#' + CtrlID).slideDown("slow");
        }
        
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            CHeck('ChkbCEEApl', 'dvCEEApl');
            $('div.expanderR').expander({
                slicePoint: 30,
                expandPrefix: '...',
                collapseTimer: 5000,
                userCollapseText: 'read less'
            });
        });
        function Expnder() {
            $('div.expanderR').expander();
        }

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });

        function uploadComplete() {
            var result = NewLPOrder.AddItemListBox();
            var getDivLEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivLEItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
                var AsyncFileUpload = $("#<%=AsyncFileUpload1.ClientID%>")[0];
                var txts = AsyncFileUpload.getElementsByTagName("input");
                for (var i = 0; i < txts.length; i++) {
                    txts[i].value = "";
                    txts[i].style.backgroundColor = "transparent";
                }
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                SuccessMessage('File Uploaded Successfully.');
                /* Clear Content */
                var AsyncFileUpload = $("#<%=AsyncFileUpload1.ClientID%>")[0];
                var txts = AsyncFileUpload.getElementsByTagName("input");
                for (var i = 0; i < txts.length; i++) {
                    txts[i].value = "";
                    txts[i].style.backgroundColor = "transparent";
                }
            }
        }
        function uploadError() {
            ErrorMessage('File upload Failed.');
        }
        function uploadStarted() {
            SuccessMessage('File Uploaded Started.');
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = NewLPOrder.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    // SuccessMessage('Selected Attachment Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';

                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
            Expnder();
        });

        $('#lnkAdd').click(function () {
            var result = NewLPOrder.AddItemListBox();
            var getDivFEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
        });

        function CheckAllBoxs() {
            var DcstChk = $('#ckhMain').is(':checked')
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var ddlRowsChanged = $('[id$=ddlRowsChanged]').val();
            var result = NewLPOrder.CheckAllBoxs(DcstChk, hfCurrentPage, ddlRowsChanged);
            var getDivFEItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
            var resultaaa = NewLPOrder.ChecErrorMsg();
            if (resultaaa.value != '')
                ErrorMessage(resultaaa.value);

        }

        function CheckIndividualBoxs(Sno) {
            var DcstChk = $('#ckhChaild' + Sno).is(':checked')
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var ddlRowsChanged = $('[id$=ddlRowsChanged]').val();
            var result = NewLPOrder.CheckIndividualBoxs(DcstChk, Sno, hfCurrentPage, ddlRowsChanged);
            var getDivFEItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
            var resultaaa = NewLPOrder.ChecErrorMsg();
            if (resultaaa.value != '')
                ErrorMessage(resultaaa.value);
        }

        // Paging Start

        function NextPage() {
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var ddlRowsChanged = parseInt($('[id$=ddlRowsChanged]').val());
            if (ddlRowsChanged == 'NAN' || ddlRowsChanged <= 0) {
                if (ddlRowsChanged == 'NAN')
                    ErrorMessage('Items to Display cannot be Empty');
                else
                    ErrorMessage('Items to Display cannot be 0(Zero)');
                $('[id$=hfCurrentPage]').focus();
                return false;
            }
            else {
                var getDivFEItems = GetClientID("divLPOItems").attr("id");

                $('div.test').block({
                    message: '<h1>Processing</h1>',
                    css: { border: '3px solid #a00' }
                });

                //                $.blockUI({ message: 'Just a moment...</h1>',
                //                    fadeOut: 20
                //                });

                var result = NewLPOrder.NextPage(hfCurrentPage, ddlRowsChanged);
                $('#' + getDivFEItems).html(result.value);
                Expnder();

                $('div.test').unblock();
                //                $.unblock();
            }
        };

        function PrevPage() {
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var ddlRowsChanged = parseInt($('[id$=ddlRowsChanged]').val());
            if (ddlRowsChanged == 'NAN' || ddlRowsChanged <= 0) {
                if (ddlRowsChanged == 'NAN')
                    ErrorMessage('Items to Display cannot be Empty');
                else
                    ErrorMessage('Items to Display cannot be 0(Zero)');
                $('[id$=hfCurrentPage]').focus();
                return false;
            }
            else {
                var result = NewLPOrder.PrevPage(hfCurrentPage, ddlRowsChanged);
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                Expnder();
            }
        };

        function RowsChanged() {
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var ddlRowsChanged = $('[id$=ddlRowsChanged]').val();
            var result = NewLPOrder.RowsChanged(hfCurrentPage, ddlRowsChanged);
            var getDivFEItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        //Paging END

        function CalculateExDuty() {
            var ExDutyAmt = $('[id$=txtExdt]').val();
            var ExChk = $("[id$=rbtnExdt]").find(":checked").val();

            if (ExDutyAmt > 99.99 && ExChk == 0) {

                ErrorMessage('CGST Cannot be Grater than 99.99%');

                $('[id$=txtExdt]').val('0');
                ExDutyAmt = 0;
            }

            var SGSTAmt = $('[id$=txtSGST]').val();
            var SGSTChk = $("[id$=rbtnSGST]").find(":checked").val();
            if (SGSTAmt > 99.99 && SGSTChk == 0) {

                ErrorMessage('SGST Cannot be Grater than 99.99%');

                $('[id$=txtSGST]').val('0');
                SGSTAmt = 0;
            }
            var IGSTAmt = $('[id$=txtIGST]').val();
            var IGSTChk = $("[id$=rbtnIGST]").find(":checked").val();
            if (IGSTAmt > 99.99 && IGSTChk == 0) {

                ErrorMessage('IGST Cannot be Grater than 99.99%');

                $('[id$=txtIGST]').val('0');
                IGSTAmt = 0;
            }
            var DiscountAmt = $('[id$=txtDsnt]').val();


            var DcstChk = $("[id$=rbtnDsnt]").find(":checked").val();
            if (DiscountAmt > 99.99 && DcstChk == 0) {

                ErrorMessage('Discount Cannot be Grater than 99.99%');

                $('[id$=txtDsnt]').val('0');
                DiscountAmt = 0;

            }
            var SalesTaxAmt = '0';

            var PackingAmt = $('[id$=txtPkng]').val();
            var PkgChk = $("[id$=rbtnPkng]").find(":checked").val();
            if (PackingAmt > 99.99 && PkgChk == 0) {

                ErrorMessage('Packing Amount Cannot be Grater than 99.99%');

                $('[id$=txtPkng]').val('0');
                PackingAmt = 0;
            }
            var AdsnlCgrs = $('[id$=txtSltx]').val();
            var AddChk = $("[id$=rbtnSltx]").find(":checked").val();

            if (AdsnlCgrs > 99.99 && AddChk == 0) {

                ErrorMessage('Sale Tax Cannot be Grater than 99.99%');

                $('[id$=txtSltx]').val('0');
                AdsnlCgrs = 0;
            }
            var ExDuty = $('[id$=chkExdt]').is(':checked');
            var SGST = $('[id$=chkSGST]').is(':checked');
            var IGST = $('[id$=chkIGST]').is(':checked');
            var Discount = $('[id$=chkDsnt]').is(':checked');
            var SalesTax = false;
            var Packing = $('[id$=chkPkng]').is(':checked');
            var AdsnlChrg = $('[id$=chkACgs]').is(':checked');
            if (ExDutyAmt == '') {
                ExDutyAmt = 0;
                $('[id$=txtExdt]').val('0');
            }
            if (SGSTAmt == '') {
                SGSTAmt = 0;
                $('[id$=txtSGST]').val('0');
            }
            if (IGSTAmt == '') {
                IGSTAmt = 0;
                $('[id$=txtIGST]').val('0');
            }
            if (DiscountAmt == '') {
                DiscountAmt = 0;
                $('[id$=txtDsnt]').val('0');
            }
            if (PackingAmt == '') {
                PackingAmt = 0;
                $('[id$=txtPkng]').val('0');
            }
            if (AdsnlCgrs == '') {
                AdsnlCgrs = 0;
                $('[id$=txtAdtnChrgs]').val('0');
            }
            //            var resss = NewLPOrder.GetExduty(ExDutyAmt, SGSTAmt, IGSTAmt, DiscountAmt, PackingAmt, SalesTaxAmt, AdsnlCgrs, Discount, ExDuty, SGST, IGST, SalesTax,
            //            Packing, AdsnlChrg, DcstChk, ExChk, SGSTChk, IGSTChk, PkgChk, AddChk);
            var res = NewLPOrder.CalculateExDuty(ExDutyAmt, SGSTAmt, IGSTAmt, DiscountAmt, PackingAmt, SalesTaxAmt, AdsnlCgrs, Discount, ExDuty, SGST, IGST, SalesTax,
            Packing, AdsnlChrg, DcstChk, ExChk, SGSTChk, IGSTChk, PkgChk, AddChk);

            var getDivLQItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivLQItems).html(res.value);
            Expnder();
            ItemWiseDsct();
            ItemWiseExciseDuty();
        }

        function ItemWiseDsct() {
            var DiscountAmt = $('[id$=txtDsnt]').val();
            var DcstChk = $("[id$=rbtnDsnt]").find(":checked").val();
            var RwCunt = $("#tblItems > tbody > tr").length;
            if (DcstChk == 0 && DiscountAmt != 0) {
                for (var i = 1; i <= RwCunt; i++) {
                    //var RwDscuntVal = $('[id$=txtDiscount' + i + ']'); //.val()
                    //RwDscuntVal.readOnly = true;
                    $('[id$=txtDiscount' + i + ']').attr("disabled", "disabled")//.attr('readonly', 'readonly');
                }
            }
        }

        function ItemWiseExciseDuty() {
            var ExciseAmt = $('[id$=txtExdt]').val();
            //var ExChk = $("[id$=chkExdt]").find(":checked").val();
            var ExChk = $("[id$=rbtnExdt]").find(":checked").val();

            var SGSTAmt = $('[id$=txtSGST]').val();
            var SGSTChk = $("[id$=rbtnSGST]").find(":checked").val();

            var IGSTAmt = $('[id$=txtIGST]').val();
            var IGSTChk = $("[id$=rbtnIGST]").find(":checked").val();

            var RwCunt = $("#tblItems > tbody > tr").length;
            if ((ExChk == 0 && ExciseAmt != 0) || (SGSTChk == 0 && SGSTAmt != 0) || (IGSTChk == 0 && IGSTAmt != 0)) {
                for (var i = 1; i <= RwCunt; i++) {
                    $('[id$=txtPercent' + i + ']').attr("disabled", "disabled");
                }
            }
        }
    </script>
</asp:Content>
