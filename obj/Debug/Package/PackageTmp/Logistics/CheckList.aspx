<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CheckList.aspx.cs" Inherits="VOMS_ERP.Logistics.CheckList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="MainTable" align="center">
                <tr>
                    <td class="bcTdNewTable">
                        <table style="width: 98%; vertical-align: top;" align="center">
                            <tr class="bcTRTitleRow">
                                <td class="bcTdTitleLeft" align="left" colspan="6">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="SHIPMENT PLANNING"
                                                    CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                        class="formError1" />
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
                                <td colspan="6" class="bcTdNewTable">
                                    <div style="width: 100%">
                                        <ajax:Accordion ID="UserAccordion" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                            HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                            FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                            FramesPerSecond="40" RequireOpenedPane="false">
                                            <Panes>
                                                <ajax:AccordionPane ID="AccordionPane3" runat="server">
                                                    <Header>
                                                        <table width="100%">
                                                            <tr>
                                                                <td width="7%">
                                                                    <a href="#" class="href">Header</a> &nbsp;<asp:Image runat="server" ID="imgAtchmt"
                                                                        AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif" Visible="false" />
                                                                </td>
                                                                <td align="center">
                                                                    <asp:Label ID="lblBlink" runat="server" Text="Click here" CssClass="blinkytext"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </Header>
                                                    <Content>
                                                        <asp:Panel ID="Panel2" runat="server" Width="98%">
                                                            <table>
                                                                <tr>
                                                                    <td class="bcTdnormal">
                                                                        <span class="bcLabel">Notify :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:DropDownList runat="server" ID="ddlNtfy" CssClass="bcAspdropdown">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span5" class="bcLabel">Country of Origin of Goods<font color="red" size="2"><b>*</b></font>
                                                                            :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:DropDownList runat="server" ID="ddlPlcOrgGds" CssClass="bcAspdropdown">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span7" class="bcLabel">Country of Final Destination<font color="red" size="2"><b>*</b></font>
                                                                            :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:DropDownList runat="server" ID="ddlPlcFnlDstn" CssClass="bcAspdropdown">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span10" class="bcLabel">Port Of Loading<font color="red" size="2"><b>*</b></font>
                                                                            : </span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:DropDownList runat="server" ID="ddlPrtLdng" CssClass="bcAspdropdown">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span13" class="bcLabel">Port Of Discharge<font color="red" size="2"><b>*</b></font>
                                                                            : </span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:DropDownList runat="server" ID="ddlPrtDscrg" CssClass="bcAspdropdown">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span15" class="bcLabel">Place of Delivery<font color="red" size="2"><b>*</b></font>
                                                                            :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:DropDownList runat="server" ID="ddlPlcDlry" CssClass="bcAspdropdown">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span8" class="bcLabel">Pre-Carriage by :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:TextBox runat="server" ID="txtPCrBy" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span9" class="bcLabel">Place of receipt by pre-Carrier :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:TextBox runat="server" ID="txtPlcRcptPCr" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span11" class="bcLabel">Vessel / Flight No. :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:TextBox runat="server" ID="txtVslFlt" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span12" class="bcLabel">Terms Of Delivery and Payment <font color="red"
                                                                            size="2"><b>*</b></font>: </span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:TextBox runat="server" ID="txtTrmDlryPmnt" TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span class="bcLabel">Incoterm :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <table>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:DropDownList runat="server" ID="ddlIncoTrm" CssClass="bcAspdropdown">
                                                                                        <asp:ListItem Text="Select Price Basis" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:TextBox runat="server" CssClass="bcAsptextboxRight" ID="txtPriceBasis"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <span id="Span1" class="bcLabel">Other References :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:TextBox runat="server" ID="txtOtrRfs" TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <td class="bcTdnormal">
                                                                        <span id="Span16" class="bcLabel"> END USE CODE :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:TextBox runat="server" ID="txtEndUseCode"  CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                     <td class="bcTdnormal">
                                                                        <span id="Span17" class="bcLabel"> ADD INTEGRATED TAX :</span>
                                                                    </td>
                                                                    <td class="bcTdnormal">
                                                                        <asp:TextBox runat="server" ID="txtAddIntegratedTax"  CssClass="bcAsptextbox" Onchange =  "javascript:getinttaxValues();" ></asp:TextBox>
                                                                    </td>
         
                                                                <tr>
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
                                <td>
                                    <table width="100%" style="background-color: #F5F4F4; padding: 5px; border: solid 1px #ccc">
                                        <tr>
                                            <td valign="top">
                                                <span id="lblCustName" class="bcLabel">Name of Customer<font color="red" size="2"><b>*</b></font>:</span>
                                            </td>
                                            <td valign="top">
                                                <asp:ListBox ID="ListBoxCustomer" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                                    CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ListBoxCustomer_SelectedIndexChanged">
                                                </asp:ListBox>
                                            </td>
                                            <td valign="top">
                                                <span id="lblGRN" class="bcLabel">Goods Dispatch Note(GDN) <font color="red" size="2">
                                                    <b></b></font>:</span>
                                            </td>
                                            <td valign="top">
                                                <asp:ListBox ID="ListBoxGDN" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                                    CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ListBoxGDN_SelectedIndexChanged">
                                                </asp:ListBox>
                                            </td>
                                            <td valign="top">
                                                <span id="lblGDN" class="bcLabel">Goods Receipt Note(GRN) <font color="red" size="2">
                                                    <b>*</b></font>:</span>
                                            </td>
                                            <td valign="top">
                                                <asp:ListBox ID="ListBoxGRN" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                                    CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ListBoxGRN_SelectedIndexChanged">
                                                </asp:ListBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <span id="Span14" class="bcLabel">Verbal FPOs <font color="red" size="2"><b>*</b></font>:</span>
                                            </td>
                                            <td>
                                                <asp:ListBox ID="lstbxVerbalFPOIDs" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                                    CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="lstbxVerbalFPOIDs_SelectedIndexChanged">
                                                </asp:ListBox>
                                            </td>
                                            <td class="bcTdnormal">
                                                <span class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                            </td>
                                            <td align="left">
                                                <asp:ListBox runat="server" ID="lbfpos" Enabled="false" SelectionMode="Multiple"
                                                    CssClass="bcAspMultiSelectListBox" AutoPostBack="true"></asp:ListBox>
                                            </td>
                                            <td>
                                                <span id="lblImpInstructions" class="bcLabel">Important Instructions:</span>
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtImpInstructions" CssClass="bcAsptextboxmulti"
                                                    TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="bcLabel">Shipment Mode<font color="red" size="2"><b>*</b></font>:</span>
                                            </td>
                                            <td>
                                                <asp:RadioButtonList ID="rbtnshpmnt" runat="server" RepeatDirection="Horizontal"
                                                    ForeColor="#000000" Font-Size="11px" font-family="Arial" AutoPostBack="True"
                                                    OnSelectedIndexChanged="rbtnshpmnt_SelectedIndexChanged">
                                                    <%-- <asp:ListItem Text="By Air" Value="F180B3B3-25A8-4ED6-8459-CFA232A9970B"></asp:ListItem>
                                                    <asp:ListItem Text="By Sea" Value="65D23EF5-DE0A-492A-A81A-1FAF9A4A4CC5" Selected="True"></asp:ListItem>--%>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td align="right">
                                                <span id="Span3" class="bcLabel"><span class="wikiContent">Shipment Planning Ref No</span>.
                                                    :</span>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox runat="server" ID="txtRefNo" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td>
                                                <span id="Span6" class="bcLabel">Supplier Invoice :</span>
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtSupInv" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="bcTdnormal">
                                                <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                                    <table width="100%">
                                                        <tr>
                                                            <td align="right" width="40%">
                                                                <span id="Span2" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                                    TextMode="MultiLine" Rows="4"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="9">
                                                <asp:Panel runat="server" ID="pnlVerbalFPO">
                                                    <table width="100%">
                                                        <%--<td colspan="6">
                                                <asp:Panel runat="server" ID="pnlVerbalFPO">
                                                    <table width="100%">
                                                        <tr style="background-color: Gray; font-style: normal; color: White;">
                                                            <td colspan="9">
                                                                <b>&nbsp;&nbsp;Verbal FPO Details</b>
                                                            </td>
                                                        </tr>
                                                        <tr style="font-size: small; font-weight: bold; color: Red;">
                                                            <td>
                                                                Supplier
                                                            </td>
                                                            <td>
                                                                No Of Pkgs
                                                            </td>
                                                            <td>
                                                                Verbal FPO No.
                                                            </td>
                                                            <td>
                                                                Godown Receipt No
                                                            </td>
                                                            <td>
                                                                Covered
                                                                <br />
                                                                Under ARE-1
                                                            </td>
                                                            <td>
                                                                Net Weight Kgs
                                                            </td>
                                                            <td>
                                                                Gr Weight Kgs
                                                            </td>
                                                            <td>
                                                                Remarks
                                                            </td>
                                                            <td>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:DropDownList ID="ddlVerbalSupplier" runat="server">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox runat="server" ID="txtNoofPkgs" onkeyup="extractNumber(this,0,true)"
                                                                    onkeypress="return blockNonNumbers(this, event, true, true)" Width="80px"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblFpoNosVerbal" runat="server" Text="."></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox runat="server" ID="txtVerbalGodownReceiptNo"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox runat="server" ID="chkVerbalAreOne" />
                                                            </td>
                                                            <td>
                                                                <asp:TextBox runat="server" ID="txtVerbalNetWeight" onkeyup="extractNumber(this,0,true)"
                                                                    onkeypress="return blockNonNumbers(this, event, true, true)" Width="80px"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox runat="server" ID="txtVerbalGrWeight" onkeyup="extractNumber(this,0,true)"
                                                                    onkeypress="return blockNonNumbers(this, event, true, true)" onchange="return CheckWeights(event)"
                                                                    Width="80px"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox runat="server" ID="txtRemarks" TextMode="MultiLine" Text=""></asp:TextBox>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Button runat="server" ID="btnVerbalFPO" Text="ADD Verbal FPO" Class="Verbal"
                                                                    OnClientClick="return Validation()" Width="125PX" Height="50px" OnClick="btnVerbalFPO_Click" />
                                                            </td>
                                                        </tr>
                                        <tr>
                                            <td>
                                                <div id="divVerbalDetails" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                        </table> </asp:Panel> </td> --%>
                                                        <tr>
                                                            <td colspan="9">
                                                                <div id="divVerbalDetails" runat="server" width="100%">
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <asp:Button runat="server" ID="btnVerbalFPO" Text="ADD Verbal FPO" Class="Verbal"
                                                                    OnClientClick="return final()" Width="125PX" Height="50px" OnClick="btnVerbalFPO_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="6" class="bcTdNewTable" align="right">
                                                <asp:Button ID="btnExport" runat="server" Text="Export" Visible="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <div id="divGrnDetails" runat="server" width="100%">
                                                </div>
                                            </td>
                                        </tr>
                                        <tr style="background-color: Gray; font-style: normal; color: White;">
                                            <td colspan="6">
                                                <b>&nbsp;&nbsp;List Details</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="6" style="min-height: 1%; max-height: 10%; overflow: auto;">
                                                <asp:Panel ID="panelContainer" runat="server" Width="100%" ScrollBars="Auto">
                                                    <asp:GridView runat="server" ID="gvCheckedList" AutoGenerateColumns="false" Width="100%"
                                                        RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                        PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                        FooterStyle-CssClass="bcGridViewHeaderStyle" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                                        AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" ShowFooter="true"
                                                        OnRowDataBound="gvCheckedList_RowDataBound">
                                                        <RowStyle CssClass="bcGridViewRowStyle"></RowStyle>
                                                        <HeaderStyle CssClass="fixedHeader " />
                                                        <EmptyDataRowStyle CssClass="bcGridViewEmptyDataRowStyle"></EmptyDataRowStyle>
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="S.No." ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSNO" runat="server" Text='<%# Eval("SNo") %>'></asp:Label>
                                                                    <asp:HiddenField ID="hfCustID" runat="server" Value='<%# Eval("CustomerID") %>'>
                                                                    </asp:HiddenField>
                                                                    <asp:HiddenField ID="hfGDNID" runat="server" Value='<%# Eval("GDNID") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hfGRNID" runat="server" Value='<%# Eval("GRNID") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hfSupplierID" runat="server" Value='<%# Eval("SupplierID") %>'>
                                                                    </asp:HiddenField>
                                                                    <asp:HiddenField ID="hfFPOs" runat="server" Value='<%# Eval("FPOs") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hfaType" runat="server" Value='<%# Eval("aType") %>'></asp:HiddenField>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Pkg.Nos" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPkgNo" runat="server" Text='<%# Eval("PkgNos") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="name of the Supplier & Place">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSupplierAndPlace" runat="server" Text='<%# Eval("SupplierNm") %>' />
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalText" runat="server" Text="Totals : " />
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="No of Pkgs" FooterStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPkgs" runat="server" Text='<%# Eval("NoOfPkgs") %>' />
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalPkgs" runat="server" Text="0" />
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="FPO No.">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblFPONo" runat="server" Text='<%# Eval("FPONOs") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Pkgs. to be collected from Transport Godown LR No. / VIPL Godown Receipt No.">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLRGodownNo" runat="server" Text='<%# Eval("LR_GodownNo") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Covered under ARE-1 if TRUE, details & availability ARE-1">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSpec" runat="server" Text='<%# Eval("IsARE1") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Net Weight Kgs" FooterStyle-HorizontalAlign="Right"
                                                                ItemStyle-HorizontalAlign="Right">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblNetWeight" runat="server" Text='<%# Eval("NetWeight") %>' />
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalNetWeight" runat="server" Text="0.00" />
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Gr Weight Kgs" FooterStyle-HorizontalAlign="Right"
                                                                ItemStyle-HorizontalAlign="Right">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblGrWeight" runat="server" Text='<%# Eval("GrWeight") %>' />
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalGrWeight" runat="server" Text="0.00" />
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Remarks">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtRemarks" TextMode="MultiLine" Text='<%# Eval("Remarks") %>'
                                                                        CssClass="bcAsptextboxmulti"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <FooterStyle Font-Bold="True" />
                                                        <PagerStyle HorizontalAlign="Center" CssClass="bcGridViewPagerStyle"></PagerStyle>
                                                        <HeaderStyle CssClass="bcGridViewHeaderStyle"></HeaderStyle>
                                                        <AlternatingRowStyle CssClass="bcGridViewAlternatingRowStyle"></AlternatingRowStyle>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="6" class="bcTdNewTable" style="height: 25px">
                                                &nbsp;
                                                <table runat="server" id="CheckListGrid_Export" style="width: 100%;">
                                                </table>
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
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        var IsSub = true;
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
    </script>
    <script type="text/javascript">
        function GetSeaOrAir(CtrlID) {
            var val = $('#' + CtrlID + ' input:checked').val();
            var res = (val == 303 ? 'Air' : 'Sea');
            var result = CheckList.GetCount(val);
            var txtval = $('[id$=txtRefNo]').val().split("/");
            $('[id$=txtRefNo]').val(txtval[0] + '/' + res + '/' + result.value + '/' + txtval[3]);
        }

        function CheckWeights(evt) {
            if ((parseFloat(($('[id$=txtVerbalGrWeight]').val()).trim())) < (parseFloat(($('[id$=txtVerbalNetWeight]').val()).trim()))) {
                ErrorMessage('Net Weight should not be greater than Gross Weight.');
                $('[id$=txtVerbalNetWeight]').val('');
                $('[id$=txtVerbalGrWeight]').val('');
                $('[id$=txtVerbalNetWeight]').focus();
                return false;
            }
            else
                return true;
        }

        function CheckGrossWeights() {
            if ((parseFloat(($('[id$=txtVerbalGrWeight]').val()).trim())) < (parseFloat(($('[id$=txtVerbalNetWeight]').val()).trim()))) {
                ErrorMessage('Net Weight should not be greater than Gross Weight.');
                $('[id$=txtVerbalNetWeight]').val('');
                $('[id$=txtVerbalGrWeight]').val('');
                $('[id$=txtVerbalNetWeight]').focus();
                return false;
            }
            else
                return true;
        }

        function OpenAccordion() {
            $find('ctl00_ContentPlaceHolder1_UserAccordion_AccordionExtender').set_SelectedIndex(0);
        }

        function Myvalidations() {
            if (($('[id$=ddlPlcOrgGds]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Country of origin of goods is Required.');
                $('[id$=ddlPlcOrgGds]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=ddlPlcFnlDstn]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Country of Final Destination is Required.');
                $('[id$=ddlPlcFnlDstn]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=ddlPrtLdng]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Port of Loading is Required.');
                $('[id$=ddlPrtLdng]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=ddlPrtDscrg]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Port of Discharge is Required.');
                $('[id$=ddlPrtDscrg]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=ddlPlcDlry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Place of Delivery is Required.');
                $('[id$=ddlPlcDlry]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=txtTrmDlryPmnt]').val()).trim() == '') {
                ErrorMessage('Terms Of Delivery and Payment is Required.');
                $('[id$=txtTrmDlryPmnt]').focus();
                OpenAccordion();
                return false;
            }
            else if ($("#<%=gvCheckedList.ClientID %> tr").length != undefined) {
                var GvCheckedListItems = $("#<%=gvCheckedList.ClientID %> tr").length;
                if ($('#ctl00_ContentPlaceHolder1_ListBoxCustomer :selected').length == 0) {
                    ErrorMessage('Customer is Required.');
                    $('[id$=ListBoxCustomer]').focus();
                    return false;
                }
                else if ($('#ctl00_ContentPlaceHolder1_ListBoxGRN :selected').length == 0
                    && $('#ctl00_ContentPlaceHolder1_ListBoxGDN :selected').length == 0
                    && $('#ctl00_ContentPlaceHolder1_lstbxVerbalFPOIDs :selected').length == 0) {
                    ErrorMessage('GDN/GRN/Verbal FPO is Required.');
                    return false;
                }

                if ($('#ctl00_ContentPlaceHolder1_lstbxVerbalFPOIDs :selected').length != 0) {

                    //                    if (($('[id$=ddlVerbalSupplier]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    //                        ErrorMessage('Supplier is Required.');
                    //                        $('[id$=ddlVerbalSupplier]').focus();
                    //                        return false;
                    //                    }
                    //                    else if (($('[id$=txtNoofPkgs]').val()).trim() == '') {
                    //                        ErrorMessage('No of packages are Required.');
                    //                        $('[id$=txtNoofPkgs]').focus();
                    //                        return false;
                    //                    }
                    //                    else if (($('[id$=txtVerbalGodownReceiptNo]').val()).trim() == '') {
                    //                        ErrorMessage('Godown Receipt No is Required.');
                    //                        $('[id$=txtVerbalGodownReceiptNo]').focus();
                    //                        return false;
                    //                    }
                    //                    else if (($('[id$=txtVerbalNetWeight]').val()).trim() == '' || parseFloat($('[id$=txtVerbalNetWeight]').val()) <= 0) {
                    //                        ErrorMessage('Netweight is Required.');
                    //                        $('[id$=txtVerbalNetWeight]').focus();
                    //                        return false;
                    //                    }
                    //                    else if (($('[id$=txtVerbalGrWeight]').val()).trim() == '' || parseFloat($('[id$=txtVerbalGrWeight]').val()) <= 0) {
                    //                        ErrorMessage('Gross weight is Required.');
                    //                        $('[id$=txtVerbalGrWeight]').focus();
                    //                        return false;
                    //                    }
                }

                if ($('#ctl00_ContentPlaceHolder1_rbtnshpmnt :checked').length == 0) {
                    ErrorMessage('ShipmentMode is Required.');
                    return false;
                }
                else if (GvCheckedListItems < 3) {
                    ErrorMessage('No Rows to Save.');
                    return false;
                }
                else if ($('[id$=DivComments]').css("visibility") == "visible") {
                    if (($('[id$=txtComments]').val()).trim() == '') {
                        ErrorMessage('Comment is Required.');
                        $('[id$=txtComments]').focus();
                        return false;
                    }
                }
                else {
                    return true;
                }
            }
            else {
                ErrorMessage('No Items to save');
                return false;
            }
        }

        function Validation() {
            if ($('#ctl00_ContentPlaceHolder1_lstbxVerbalFPOIDs :selected').length != 0) {

                //                if (($('[id$=ddlVerbalSupplier]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                //                    ErrorMessage('Supplier is Required.');
                //                    $('[id$=ddlVerbalSupplier]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtNoofPkgs]').val()).trim() == '') {
                //                    ErrorMessage('No of packages are Required.');
                //                    $('[id$=txtNoofPkgs]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtVerbalGodownReceiptNo]').val()).trim() == '') {
                //                    ErrorMessage('Godown Receipt No is Required.');
                //                    $('[id$=txtVerbalGodownReceiptNo]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtVerbalNetWeight]').val()).trim() == '' || parseFloat($('[id$=txtVerbalNetWeight]').val()) <= 0) {
                //                    ErrorMessage('Netweight is Required.');
                //                    $('[id$=txtVerbalNetWeight]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtVerbalGrWeight]').val()).trim() == '' || parseFloat($('[id$=txtVerbalGrWeight]').val()) <= 0) {
                //                    ErrorMessage('Gross weight is Required.');
                //                    $('[id$=txtVerbalGrWeight]').focus();
                //                    return false;
                //                }
                //                else
                //                    return true;
            }
        }

        function Count() {
            var items = "";
            var selected = $("[id*=lstbxVerbalFPOIDs] option:selected");
            selected.each(function () {
                items += $(this).val() + ",";
                var result = CheckList.Count(items);
                var getdivVERFPO = GetClientID("divVerbalDetails").attr("id");
                $('#' + getdivVERFPO).html(result.value);
            });

        }

        function SaveChanges(RNo) {
            try {

                var ddlSuplr = GetClientID("ddlSuppliers" + (parseInt(RNo))).attr("id");
                var Supplier = $('#' + ddlSuplr).val();
                var txtPkgs = GetClientID("txtNoPkgs" + (parseInt(RNo))).attr("id");
                var Pkgs = $('#' + txtPkgs).val();
                var txtGRN = GetClientID("txtGRNNo" + (parseInt(RNo))).attr("id");
                var GRNNo = $('#' + txtGRN).val();
                var txtAre = GetClientID("txtCoveredUnderARE" + (parseInt(RNo))).attr("id");
                var Are = $('#' + txtAre).is(':checked');
                var txtNetWei = GetClientID("txtNetWt" + (parseInt(RNo))).attr("id");
                var NetWeight = $('#' + txtNetWei).val();
                var txtGrossW = GetClientID("txtGrsWt" + (parseInt(RNo))).attr("id");
                var GrossWeight = $('#' + txtGrossW).val();
                var txtRem = GetClientID("txtRemarks" + (parseInt(RNo))).attr("id");
                var Remarks = $('#' + txtRem).val();
                var FPONum = GetClientID("lblFpoNo" + (parseInt(RNo))).attr("id");
                var FPONumber = $('#' + FPONum).text();

                var items = "";
                var selected = $("[id*=lstbxVerbalFPOIDs] option:selected");
                selected.each(function () {
                    if (items != "") {
                        items += ",";
                    }
                    items += $(this).val();
                    //var result = CheckList.Count(items);
                    // var getdivVERFPO = GetClientID("divVerbalDetails").attr("id");
                    //$('#' + getdivVERFPO).html(result.value);
                });
                if (parseFloat(NetWeight) != 0 && parseFloat(GrossWeight) != 0) {
                    if (parseFloat(GrossWeight) < parseFloat(NetWeight)) {
                        GrossWeight = '';
                        NetWeight = '';
                        $('#' + txtNetWei).val(0);
                        $('#' + txtGrossW).val(0);
                        ErrorMessage('GrossWeight Should be Greater than NetWeight');
                        return false;
                    }
                }
                var result = CheckList.VerbalFPOAdd(RNo, Supplier, Pkgs, GRNNo, Are, NetWeight, GrossWeight, Remarks, items, FPONumber);

                var getdivVERFPO = GetClientID("divVerbalDetails").attr("id");
                $('#' + getdivVERFPO).html(result.value);
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }

        function final() {
            var Rcount = $('#tblVerbalDetails >tbody >tr').length;
            for (var i = 0; i < Rcount; i++) {
                IsSub = true;
                AddNewRow(parseInt(i) + 1);
                if (IsSub == false)
                    return false;
            }

        }

        function AddNewRow(RNo) {
            try {

                var ddlSuplr = GetClientID("ddlSuppliers" + (parseInt(RNo))).attr("id");
                var Supplier = $('#' + ddlSuplr).val();
                var txtPkgs = GetClientID("txtNoPkgs" + (parseInt(RNo))).attr("id");
                var Pkgs = $('#' + txtPkgs).val();
                var txtGRN = GetClientID("txtGRNNo" + (parseInt(RNo))).attr("id");
                var GRNNo = $('#' + txtGRN).val();
                var txtAre = GetClientID("txtCoveredUnderARE" + (parseInt(RNo))).attr("id");
                //var ChkBox = $j("[id$=chbDocTypes]").is(':checked');
                var Are = $('#' + txtAre).is(':checked');
                var txtNetWei = GetClientID("txtNetWt" + (parseInt(RNo))).attr("id");
                var NetWeight = $('#' + txtNetWei).val();
                var txtGrossW = GetClientID("txtGrsWt" + (parseInt(RNo))).attr("id");
                var GrossWeight = $('#' + txtGrossW).val();
                var txtRem = GetClientID("txtRemarks" + (parseInt(RNo))).attr("id");
                var Remarks = $('#' + txtRem).val();

                if ((Pkgs != '0') && (GRNNo != '') && parseFloat(NetWeight) != 0 && parseFloat(GrossWeight) != 0 && Supplier != '00000000-0000-0000-0000-000000000000') {
                    var Rcounts = $('#tblVerbalDetails >tbody >tr').length;
                    if (Rcounts == RNo) {
                        var result = CheckList.AddtoGrid();
                        // var getdivVERFPO = GetClientID("divVerbalDetails").attr("id");
                        // $('#' + getdivVERFPO).html(result.value);
                    }
                }
                else {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    if (Supplier == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Supplier is Required.');
                        IsSub = false;
                        $('#' + ddlSuplr).focus();
                    }
                    else if ((Pkgs == '' || Pkgs == '0')) {
                        ErrorMessage('No of Package is Required.');
                        IsSub = false;
                        $('#' + txtPkgs).focus();
                    }
                    else if ((GRNNo == '')) {
                        ErrorMessage('GRN No is Required.');
                        IsSub = false;
                        $('#' + txtGRN).focus();
                    }
                    else if (NetWeight == '' || NetWeight == '0') {
                        ErrorMessage('Net Weight is Required.');
                        IsSub = false;
                        $('#' + txtNetWei).focus();
                    }
                    else if (GrossWeight == '' || GrossWeight == '0') {
                        ErrorMessage('Gross Weight is Required.');
                        IsSub = false;
                        $('#' + txtGrossW).focus();
                    }
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }

        function getinttaxValues() {
            var PercAmt = $('[id$=txtAddIntegratedTax]').val();
                if (PercAmt == '') {
                    ErrorMessage('Payment Percentage is Required');
                }
                else if (PercAmt == '0') {
                     ErrorMessage('Payment Percentage cannot be Zero');
                }
                else if (PercAmt > 100) {
                    ErrorMessage('Percentage Cannot Exceed 100');
                    $('[id$=txtAddIntegratedTax]').val('0');
                    $('[id$=txtAddIntegratedTax]').focus();
                }
              

            }
           
        
    </script>
</asp:Content>
