<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Grn.aspx.cs" Inherits="VOMS_ERP.Logistics.Grn" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Goods Receipt Note"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
                                        <span id="Span13" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <span id="Span3" class="bcLabel">Customer Name <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCstmr" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlCstmr_SelectedIndexChanged">
                                            <asp:ListItem Text="-- Select --" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span10" class="bcLabel">Is Dispatch Instructions:
                                            <asp:CheckBox runat="server" ID="chkbDpchInst" CssClass="chkbgrp" onclick="ChangeDisplay(this.id, 'dvDpchInst')" /></span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <div id="dvDpchInst" style="display: none;">
                                            <asp:DropDownList runat="server" ID="ddlDpchInst" CssClass="bcAspdropdown" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlDpchInst_SelectedIndexChanged">
                                                <asp:ListItem Text="-- Select --" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span22" class="bcLabel">Is GDN:
                                            <asp:CheckBox runat="server" ID="chkbGdnNmbr" CssClass="chkbgrp" onclick="ChangeDisplay(this.id, 'dvGdnNmbr')" /></span>
                                    </td>
                                    <td align="left">
                                        <div id="dvGdnNmbr" style="display: none;">
                                            <asp:DropDownList runat="server" ID="ddlGDspchNote" CssClass="bcAspdropdown" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlGDspchNote_SelectedIndexChanged">
                                                <asp:ListItem Text="-- Select --" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span34" class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" Enabled="false"
                                            CssClass="bcAspMultiSelectListBox" AutoPostBack="true" OnSelectedIndexChanged="lbfpos_SelectedIndexChanged">
                                        </asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Supplier Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlSuplr" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlSuplr_SelectedIndexChanged">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lblpos" SelectionMode="Multiple" Enabled="false"
                                            CssClass="bcAspMultiSelectListBox" AutoPostBack="true" OnSelectedIndexChanged="lblpos_SelectedIndexChanged">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Place of Receipt <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPlcRcpt" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Transporter Name <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTnptrNm" onkeypress="return isTransporterName(event)"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Received Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRcvdDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">L/R No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtWBNo" onkeypress="return isWayBill(event)" onchange="javascript:return CheckWayBillNo();"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">L/R Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtWBDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Truck No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTkNo" onkeypress="return isAlphaNumaricSpace(event)"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Packing Type <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPkngTp" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">No. of Packages <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPkngNo" onkeyup="extractNumber(this,0,true);"
                                            onkeypress="return blockNonNumbers(this, event, true, true);" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Gross Weight : Kgs <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtGW" onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">Net Weight : Kgs <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtNW" onchange="return CheckWeights(event)" onkeyup="extractNumber(this,3,true);"
                                            onkeypress="return blockNonNumbers(this, event, true, false);" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span12" class="bcLabel">DC No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtDcNo" onchange="javascript:return CheckDcNo();"
                                            onkeypress="return isWayBill(event)" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span14" class="bcLabel">DC No. Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtDcDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span18" class="bcLabel">Freight <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlFrt" CssClass="bcAspdropdown" AutoPostBack="false"
                                            onchange="ShowModelPop();">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="To-Pay" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Pre-Paid" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField runat="server" ID="hdfempty" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span15" class="bcLabel">Invoice No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtInvNo" onchange="javascript:return CheckInvNo();"
                                            onkeypress="return isWayBill(event)" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span16" class="bcLabel">Invoice Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtInvDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span20" class="bcLabel">Condition of Goods <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlGdsCndtn" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Good" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Bad" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Breakage" Value="3"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="display: none;">
                                        <span id="Span19" class="bcLabel">Payment Mode <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal" style="display: none;">
                                        <asp:DropDownList runat="server" ID="ddlPmtMd" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Cash" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Cheque" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                   <td class="bcTdnormal">
                                        <span id="Span26" class="bcLabel">Location <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlLocation" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                      <td class="bcTdnormal">
                                        <span id="Span17" class="bcLabel">Box Name <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtBoxname" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                 <tr>
                                  
                                    <td class="bcTdnormal">
                                        <span id="Span21" class="bcLabel">Dimensions<font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                       <asp:TextBox runat="server" ID="txtDimen" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                      <td class="bcTdnormal">
                                        <span class="bcLabel">Inspection Remarks :</span>
                                    </td>
                                    <td align="left" colspan="3">
                                        <asp:TextBox runat="server" ID="txtremarks" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                            Width="557px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="43%">
                                                        <span id="Span29" class="bcLabel">Comments <font color="red" size="2"><b>*</b></font>
                                                            :</span>
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
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            <div style="width: 100%">
                                <ajax:Accordion ID="Accordion1" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                    HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                    FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                    FramesPerSecond="40" RequireOpenedPane="false">
                                    <Panes>
                                        <ajax:AccordionPane ID="AccordionPane1" runat="server">
                                            <Header>
                                                <a href="#" class="href">CT-1 & ARE-1 DETAILS</a> &nbsp;<asp:Image runat="server"
                                                    ID="Image1" AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif"
                                                    Visible="false" />
                                            </Header>
                                            <Content>
                                                <asp:Panel ID="Panel3" runat="server" Width="100%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td colspan="6" class="bcTdNewTable">
                                                                <center>
                                                                    <div style="overflow: auto; width: 100%;" id="divCT1Dtls" runat="server">
                                                                    </div>
                                                                </center>
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
                    <tr style="background-color: Gray; font-style: normal; color: White;">
                        <td colspan="6">
                            <b>&nbsp;&nbsp;Item Details</b>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            <div style="min-height: 150px; max-height: 250px; overflow: auto;">
                                <asp:GridView runat="server" ID="gvGRN" AutoGenerateColumns="false" RowStyle-CssClass="rounded-corner"
                                    EmptyDataRowStyle-CssClass="rounded-corner" CssClass="rounded-corner" HeaderStyle-CssClass="rounded-corner"
                                    AlternatingRowStyle-CssClass="rounded-corner" BorderWidth="0px" Width="100%"
                                    ShowFooter="true" EmptyDataText="No Records To Display...!" OnPreRender="gvGRN_PreRender"
                                    OnRowDataBound="gvGRN_RowDataBound" EmptyDataRowStyle-HorizontalAlign="Center"
                                    OnRowCommand="gvGRN_RowCommand" GridLines="None" AllowPaging="True" PageSize="100">
                                    <AlternatingRowStyle CssClass="rounded-corner"></AlternatingRowStyle>
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox runat="server" ID="chkbhdr" OnCheckedChanged="chkbhdr_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%--<asp:CheckBox runat="server" ID="chkbitm" Checked='<%# Eval("check") %>' OnCheckedChanged="chkbitm_CheckedChanged"
                                                    AutoPostBack="true" />--%>
                                                    <asp:CheckBox runat="server" ID="chkbitm" Checked='<%#bool.Parse(Eval("check").ToString())%>' OnCheckedChanged="chkbitm_CheckedChanged"
                                                    AutoPostBack="true" />
                                                <asp:HiddenField ID="hfFSNo" runat="server" Value='<%# Eval("Sno") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="IDs" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemDtlsID" runat="server" Text='<%# Eval("StockItemsId") %>'></asp:Label>
                                                <asp:Label ID="lblItemID" runat="server" Text='<%# Eval("ItemId") %>'></asp:Label>
                                                <asp:Label ID="lblLclPoID" runat="server" Text='<%# Eval("LocalPOId") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="S.No.">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                                <asp:Label ID="lblSerialNo" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Desc">
                                            <FooterTemplate>
                                                <asp:Label ID="lblFooterPaging" runat="server" Text=""></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="txtItmDesc" TextMode="MultiLine" CssClass="bcAsptextbox"
                                                    Text='<%# Eval("Description") %>' onblur="ReSizeTXT(this.id)" onfocus="ExpandTXT(this.id)"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Part No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFullName" runat="server" Text='<%# Eval("PartNumber") %>' />
                                                <asp:HiddenField ID="hfHSCode" runat="server" Value='<%# Eval("HSCode") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Specifications" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpec" runat="server" Text='<%# Eval("Specifications") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="HSCCode">
                                            <ItemTemplate>
                                                <%--<asp:Label ID="lblHscCode" runat="server" Text='<%# Eval("HSCode") %>' />--%>
                                                <asp:TextBox runat="server" ID="txtHscCode" CssClass="bcAsptextbox" Width="50px" AutoPostBack="true"
                                                    Text='<%# Eval("HSCode") %>' OnTextChanged="txtHscCode_TextChanged" ></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Make">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMk" runat="server" Text='<%# Eval("Make") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Actual Qty" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Arrived Qty" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="HFDsptchQty" runat="server" Value='<%# Eval("DspchQty") %>' />
                                                <asp:TextBox runat="server" ID="txtCrntQty" CssClass="bcAsptextbox" Width="50px"
                                                    Text='<%# Eval("DspchQty") %>' AutoPostBack="true" onkeyup="extractNumber(this,2,false);"
                                                    onkeypress="return blockNonNumbers(this, event, true, false);" OnTextChanged="txtRcvdQty_TextChanged"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <FooterTemplate>
                                                <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_OnSelectedIndexChanged">
                                                    <asp:ListItem Text="25" Value="25" />
                                                    <asp:ListItem Text="50" Value="50" />
                                                    <asp:ListItem Text="100" Value="100" Selected="True" />
                                                    <asp:ListItem Text="200" Value="200" />
                                                </asp:DropDownList>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="GDN/GRN Rcvd Qty" ItemStyle-HorizontalAlign="Center">
                                            <FooterTemplate>
                                                <asp:Button ID="btnPrev" Text="Previous" OnClick="btnPrevious_Click" runat="server"
                                                    Width="90px" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblRcvdQty" runat="server" Text='<%# Eval("ReceivedQty") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rmng Qty" ItemStyle-HorizontalAlign="Center">
                                            <FooterTemplate>
                                                <asp:Button ID="btnNext" Text="Next" OnClick="btnNext_Click" runat="server" Width="90px" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblRmngQty" runat="server" Text='<%# Eval("RemainingQty") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Units" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("UnitNm") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataRowStyle HorizontalAlign="Center" CssClass="rounded-corner"></EmptyDataRowStyle>
                                    <HeaderStyle CssClass="rounded-corner"></HeaderStyle>
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="rounded-corner"></RowStyle>
                                </asp:GridView>
                            </div>
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
                                                <a href="#" class="href">Attachments</a> &nbsp;<asp:Image runat="server" ID="imgAtchmt"
                                                    AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif" Visible="false" />
                                            </Header>
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
                        <td align="center" colspan="6">
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
    </table>
    <ajax:ModalPopupExtender ID="ModalPopupExtender1" runat="server" CancelControlID="btnCancel"
        TargetControlID="hdfempty" OnOkScript="Validations()" OnCancelScript="CancelSelection()"
        PopupControlID="Panel1" PopupDragHandleControlID="PopupHeader" Drag="true" BackgroundCssClass="ModalPopupBG">
    </ajax:ModalPopupExtender>
    <asp:Panel ID="Panel1" Style="display: none; top: 120px;" runat="server">
        <div class="Popup">
            <div class="PopupHeader" id="PopupHeader">
                Freight Charges</div>
            <div class="PopupBody">
                <p>
                    All * Fields are Mandatory</p>
                <table>
                    <tr>
                        <td>
                            <span id="Span23" class="bcLabel">Payment Mode <font color="red" size="2"><b>*</b></font>
                                :</span>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlPmtPop" CssClass="bcAspdropdown" onchange="DisplayChanges()">
                                <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Cash" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Cheque" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="dvcash" style="display: none;">
                                <table>
                                    <tr>
                                        <td>
                                            <span id="Span24" class="bcLabel">
                                                <asp:Label ID="LblAmount" runat="server" Text="Amount "></asp:Label><font color="red"
                                                    size="2"><b>*</b></font>:</span>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" MaxLength="10" Width="50%" onkeypress="return isNumberKey(event)"
                                                CssClass="bcAsptextbox" ID="txtAmt"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="dvchque" style="display: none;">
                                <table>
                                    <tr>
                                        <td>
                                            <span id="Span25" class="bcLabel">Cheque No. <font color="red" size="2"><b>*</b></font>
                                                :</span>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" MaxLength="10" Width="50%" onkeypress="return isNumberKey(event)"
                                                CssClass="bcAsptextbox" ID="txtChqno"></asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Span27" class="bcLabel">Cheque Date <font color="red" size="2"><b>*</b></font>
                                                :</span>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" MaxLength="10" Width="50%" CssClass="bcAsptextbox" ID="txtChqDt"></asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Span28" class="bcLabel">NEFT/RTGS Code :</span>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" MaxLength="10" Width="50%" onkeypress="return isAlphaNumaric(event)"
                                                CssClass="bcAsptextbox" ID="txtRtgsCd"></asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="Controls">
                <input id="btnOkay" type="button" onclick="return Validations()" value="Done" />
                <input id="btnCancel" type="button" value="Cancel" />
            </div>
        </div>
    </asp:Panel>
    <style type="text/css">
        .ModalPopupBG
        {
            background-color: #666699;
            filter: alpha(opacity=50);
            opacity: 0.7;
        }
        .Popup
        {
            min-width: 50%;
            max-width: 99%;
            min-height: 45%;
            max-height: 85%;
            background: white;
            padding: 5px 5px 5px 5px;
            border: solid 1px blue;
            margin: 10%;
        }
        .PopupHeader
        {
            font-size: 2em;
            font-family: Arial;
        }
        .PopupBody
        {
            font-size: 1em;
            font-family: Times New Roman;
        }
    </style>
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

        function ExpandTXT(obj) {
            $('#' + obj).animate({ "height": "75px" }, "slow");
            $('#' + obj).slideDown("slow");
        }

        function ReSizeTXT(obj) {
            $('#' + obj).animate({ "height": "20px" }, "slow");
            $('#' + obj).slideDown("slow");
        }

        function GetProformaInvNo() {
            var refNo = $('[id$=txtpmIn]').val();
            var result = IomForm.GetProformaInvNo(refNo);
            if (result.value == false) {
                $('[id$=txtpmIn]').val('');
                $('[id$=txtpmIn]').focus();
                ErrorMessage('This Proforma Invoice No. is already in Use');
            }
        }

        $(document).ready(function () {
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
            var dateToday = new Date();
            $('[id$=txtRcvdDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                maxDate: dateToday
            });
            $('[id$=txtWBDt]').datepicker({
                maxDate: dateToday,
                dateFormat: 'dd-mm-yy'
            });
            $('[id$=txtDcDt]').datepicker({
                maxDate: dateToday,
                dateFormat: 'dd-mm-yy'
            });
            $('[id$=txtInvDt]').datepicker({
                maxDate: dateToday,
                dateFormat: 'dd-mm-yy'
            });
            $('[id$=txtChqDt]').datepicker({
                maxDate: dateToday,
                dateFormat: 'dd-mm-yy'
            });
            DatePickerAdd();
        });


        function DatePickerAdd() {
            try {
                var dateToday = new Date();
                $('.DatePicker').datepicker({
                    maxDate: dateToday,
                    dateFormat: 'dd-mm-yy'
                });
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }

        function CheckWeights(evt) {
            if ((parseFloat(($('[id$=txtGW]').val()).trim())) < (parseFloat(($('[id$=txtNW]').val()).trim()))) {
                ErrorMessage('Net Weight should not be greaterthan Gross Weight.');
                $('[id$=txtNW]').val('');
                $('[id$=txtNW]').focus();
                return false;
            }
            else
                return true;
        }

        function chkAllCheckbox(obj) {
            var gv = $("#<%=gvGRN.ClientID %> input");
            for (var i = 0; i < gv.length; i++) {
                if (gv[i].type == 'checkbox') {
                    gv[i].checked = obj.checked;
                }
            }
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57)) {
                return false;
            }

            if (charCode == 46 && el.value.indexOf(".") !== -1) {
                return false;
            }

            return true;
        }
        // E-Mail Validation
        function validateEmail(emailField) {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            if (emailField.value == '') {
                return true;
            }
            else if (reg.test(emailField.value) == false) {
                emailField.value = '';
                emailField.focus();
                alert('invalid Email-ID');
                return false;
            }
            return true;
        }
        function isWayBill(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 32 && charCode != 47 && charCode != 45 &&
            (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function isTransporterName(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            //            if (charCode != 8 && charCode != 32 && charCode != 38 && charCode != 46 &&
            //            (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
            if (charCode != 32 && charCode != 8 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function isAlphaNumaric(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && (charCode < 48 || charCode > 57) &&
            (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function isAlphaNumaricSpace(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 32 && (charCode < 48 || charCode > 57) &&
            (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function isDecimalKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function uploadComplete() {
            var result = Grn.AddItemListBox();
            var getDivGRNDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGRNDItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                $get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If U need Upload New File.";
            }
            /* Clear Content */
            var AsyncFileUpload = $("#<%=AsyncFileUpload1.ClientID%>")[0];
            var txts = AsyncFileUpload.getElementsByTagName("input");
            for (var i = 0; i < txts.length; i++) {
                txts[i].value = "";
                txts[i].style.backgroundColor = "transparent";
            }
        }
        function uploadError() {
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload Failed.";
        }
        function uploadStarted() {
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload started.";
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != "") {
                if (confirm("Are you sure you want to delete the item")) {
                    var result = Grn.DeleteItemListBox($('#lbItems').val());
                    var getDivGRNDItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivGRNDItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
        });


        $('#lnkAdd').click(function () {
            var result = Grn.AddItemListBox();
            var getDivGRNDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGRNDItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
        });


        function CheckActualQty(id, rstval, ActualQty, PrevousQty) {

            try {
                var Rslt = confirm('Arrived Quantity is more than Actual Quantity. Do you want to proceed?');
                if (Rslt) {
                    // $('#' + rstval + '').text('0');
                }
                else {
                    $('#' + id + '').val($('#' + ActualQty + '').val());    
                    $('#' + ActualQty + '').text(PrevousQty);
                    $('#' + rstval + '').text(0);
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }

        function DisplayChanges() {
            if (($('[id$=ddlPmtPop]').val()).trim() == '1') {
                $('[id$=dvcash]').show();
                $('[id$=dvchque]').hide();
            }
            else if (($('[id$=ddlPmtPop]').val()).trim() == '2') {
                $('[id$=dvchque]').show();
                $('[id$=dvcash]').hide();
            }
            else {
                $('[id$=dvcash]').hide();
                $('[id$=dvchque]').hide();
            }
            $('[id$=ddlPmtMd]').val($('[id$=ddlPmtPop]').val());
        }


        function Myvalidations() {

            try {
                if ($('[id$=gvGRN]').length != 0)
                    var GrnItems = $('[id$=gvGRN]')[0].rows.length;

                if (($('[id$=ddlCstmr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Customer Name is Required.');
                    $('[id$=ddlCstmr]').focus();
                    return false;
                }

                if ($('[id$=chkbDpchInst]')[0].checked == true) {
                    if (($('[id$=ddlDpchInst]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Dispatch Instructions is Required.');
                        $('[id$=ddlDpchInst]').focus();
                        return false;
                    }
                }

                if ($('[id$=chkbGdnNmbr]')[0].checked == true) {
                    if (($('[id$=ddlGDspchNote]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('GDN Number is Required.');
                        $('[id$=ddlGDspchNote]').focus();
                        return false;
                    }
                }

                if ($('[id$=lbfpos]').val() == null) {
                    ErrorMessage('Frn PO Number is Required.');
                    $('[id$=lbfpos]').focus();
                    return false;
                }
                else if (($('[id$=ddlSuplr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Supplier Name is Required.');
                    $('[id$=ddlSuplr]').focus();
                    return false;
                }
                else if ($('[id$=lblpos]').val() == null) {
                    ErrorMessage('Lcl PO Number is Required.');
                    $('[id$=lblpos]').focus();
                    return false;
                }
                else if (($('[id$=ddlPlcRcpt]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Place of Receipt is Required.');
                    $('[id$=ddlPlcRcpt]').focus();
                    return false;
                }
                else if (($('[id$=txtTnptrNm]').val()).trim() == '') {
                    ErrorMessage('Transporter Name is Required.');
                    $('[id$=txtTnptrNm]').focus();
                    return false;
                }
                if (($('[id$=txtRcvdDt]').val()).trim() == '') {
                    ErrorMessage('Received Date is Required.');
                    $('[id$=txtRcvdDt]').focus();
                    return false;
                }

                else if (($('[id$=ddlPkngTp]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Packing Type is Required.');
                    $('[id$=ddlPkngTp]').focus();
                    return false;
                }

                else if (($('[id$=txtPkngNo]').val()).trim() == '' || ($('[id$=txtPkngNo]').val()).trim() == '0') {
                    ErrorMessage('Number of Packages is Required.');
                    $('[id$=txtPkngNo]').focus();
                    return false;
                }
                else if (($('[id$=txtPkngNo]').val()).trim() == 0) {
                    ErrorMessage('Packing Type can`t be Zero.');
                    $('[id$=txtPkngNo]').focus();
                    return false;
                }
                else if (($('[id$=txtGW]').val()).trim() == '' || (parseFloat(($('[id$=txtGW]').val()).trim()) == 0)) {
                    ErrorMessage('Gross Weight is Required.');
                    $('[id$=txtGW]').focus();
                    return false;
                }
                else if (($('[id$=txtNW]').val()).trim() == '' || (parseFloat(($('[id$=txtNW]').val()).trim()) == 0)) {
                    ErrorMessage('Net Weight is Required.');
                    $('[id$=txtNW]').focus();
                    return false;
                }
                else if (($('[id$=ddlFrt]').val()).trim() == '0') {
                    ErrorMessage('Freight is Required.');
                    $('[id$=ddlFrt]').focus();
                    return false;
                }
                //                else if (($('[id$=txtDcNo]').val()).trim() == '') {
                //                    ErrorMessage('Delivery Challana Number is Required.');
                //                    $('[id$=txtDcNo]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtDcDt]').val()).trim() == '') {
                //                    ErrorMessage('Delivery Challana Date is Required.');
                //                    $('[id$=txtDcDt]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtInvNo]').val()).trim() == '') {
                //                    ErrorMessage('Invoice Number is Required.');
                //                    $('[id$=txtInvNo]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtInvDt]').val()).trim() == '') {
                //                    ErrorMessage('Invoice Date is Required.');
                //                    $('[id$=txtInvDt]').focus();
                //                    return false;
                //                }
                else if (($('[id$=ddlGdsCndtn]').val()).trim() == '0') {
                    ErrorMessage('Condition of Goods is Required.');
                    $('[id$=ddlGdsCndtn]').focus();
                    return false;
                }
                else if (($('[id$=ddlLocation]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Location is Required.');
                    $('[id$=ddlLocation]').focus();
                    return false;
                }
                else if (($('[id$=txtBoxname]').val()).trim() == '') {
                    ErrorMessage('Box Name is Required.');
                    $('[id$=txtBoxname]').focus();
                    return false;
                }
                else if (($('[id$=txtDimen]').val()).trim() == '') {
                    ErrorMessage('Dimensions is Required.');
                    $('[id$=txtDimen]').focus();
                    return false;
                }
                else if ($('[id$=DivComments]').css("visibility") == "visible") {
                    if (($('[id$=txtComments]').val()).trim() == '') {
                        ErrorMessage('Comment is Required.');
                        $('[id$=txtComments]').focus();
                        return false;
                    }
                }
                //                else if (($('[id$=ddlPmtMd]').val()).trim() == '0') {
                //                    ErrorMessage('<span class="Error">Payment Mode is Required.');
                //                    $('[id$=ddlPmtMd]').focus();
                //                    return false;
                //                }

                var RowCount = $('#tblCT1Dtls tbody tr').length;
                if ((parseInt(RowCount) / 3) > 0) {
                    var CT1IDD = $('#ddl1').val();
                    if (CT1IDD != '0') {
                        var RCount = (parseInt(RowCount) / 3);
                        for (var i = 1; i <= RCount; i++) {
                            var ddlCT1No = GetClientID("ddl" + (parseInt(i))).attr("id");
                            var CT1ID = $('#' + ddlCT1No).val();
                            var CT1No = $('#' + ddlCT1No + ' option:selected').text();
                            var txtDate = GetClientID("txtDate" + (parseInt(i))).attr("id");
                            var CT1Date = $('#' + txtDate).val();
                            var txtValue = GetClientID("txtValue" + (parseInt(i))).attr("id");
                            var CT1Value = $('#' + txtValue).val();

                            var txtARE1No = GetClientID("txtARE1No" + (parseInt(i))).attr("id");
                            var ARE1No = $('#' + txtARE1No).val();
                            var txtAREValue = GetClientID("txtARE1Value" + (parseInt(i))).attr("id");
                            var ARE1Value = $('#' + txtAREValue).val();

                            var chkbFrmWht = GetClientID("chkbARE1FormWht" + (parseInt(i))).attr("id");
                            var FrmWht = $('#' + chkbFrmWht)[0].checked;
                            var chkbFrmBff = GetClientID("chkbARE1FormBff" + (parseInt(i))).attr("id");
                            var FrmBff = $('#' + chkbFrmBff)[0].checked;
                            var chkbFrmBle = GetClientID("chkbARE1FormBle" + (parseInt(i))).attr("id");
                            var FrmBle = $('#' + chkbFrmBle)[0].checked;
                            var chkbFrmGrn = GetClientID("chkbARE1FormGrn" + (parseInt(i))).attr("id");
                            var FrmGrn = $('#' + chkbFrmGrn)[0].checked;
                            var chkbFrmPnk = GetClientID("chkbARE1FormPnk" + (parseInt(i))).attr("id");
                            var FrmPnk = $('#' + chkbFrmPnk)[0].checked;

                            var ARE1Forms = FrmWht + ',' + FrmBff + ',' + FrmBle + ',' + FrmGrn + ',' + FrmPnk;

                            if (CT1ID == 0) {
                                ErrorMessage('Select CT-1 No. in Row No-' + i);
                                $('#' + ddlCT1No).focus();
                                return false;
                            }
                            else if (ARE1No == '' || ARE1No == "0") {
                                ErrorMessage('ARE1 No. can not be Empty/Zero(0) in Row No-' + i);
                                $('#' + txtARE1No).focus();
                                return false;
                            }
                            else if (FrmWht != true && FrmBff != true && FrmBle != true && FrmGrn != true && FrmPnk != true) {
                                ErrorMessage('select atleast one ARE1 form in Row No-' + i);
                                return false;
                            }
                            else if (ARE1Value == '' || ARE1Value == '0') {
                                ErrorMessage('ARE-1 Value can not be Empty/Zero(0) in Row No-' + i);
                                $('#' + txtAREValue).val(0);
                                $('#' + txtAREValue).focus();
                                return false;
                            }
                            else if (parseFloat(ARE1Value) > parseFloat(CT1Value)) {
                                ErrorMessage('ARE-1 Value should be less than CT-1 Value in Row No-' + i);
                                $('#' + txtAREValue).focus();
                                return false;
                            }
                        }
                    }
                }

                if ($('[id$=DivComments]').css("visibility") == "visible") {
                    if (($('[id$=txtComments]').val()).trim() == '') {
                        ErrorMessage('Comment is Required.');
                        $('[id$=txtComments]').focus();
                        return false;
                    }
                }
                if (GrnItems > 0) {
                    if (GrnItems == 1) {
                        ErrorMessage('No Items to Save.');
                        $('[id$=gvLpoItems]').focus();
                        return false;
                    }
                    else {
                        var select = 0;
                        for (var i = 2; i <= GrnItems; i++) {
                            var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                            var chkbval = GetClientID(chkbx + "_chkbitm").attr("id");
                            var CrntQty = GetClientID(chkbx + "_txtCrntQty").attr("id");
                            if (chkbval == undefined) {
                                continue;
                            }
                            else if ($('#' + chkbval)[0].checked) {
                                if (parseFloat($('[id$=' + CrntQty + ']').val()) > 0)
                                    select = select + 1;
                                else {
                                    ErrorMessage('Dispatch Quantity should not be Zero(0) at Item ' + (i - 1) + '.');
                                    $('[id$=' + CrntQty + ']').focus();
                                    return false;
                                    break;
                                }
                            }
                        }
                        if (select == 0) {
                            ErrorMessage('Select At Least One Item.');
                            $('[id$=gvLpoItems]').focus();
                            return false;
                        }
                    }
                }
                else if (GrnItems <= 0) {
                    ErrorMessage('No Item to Save.');
                    $('[id$=gvLpoItems]').focus();
                    return false;
                }
                else {
                    return true;
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }


        function Validations() {
            var aflag = true;
            if (($('[id$=ddlPmtPop]').val()).trim() == '0') {
                ErrorMessage('Payment Mode is Required.');
                $('[id$=ddlPmtPop]').focus();
                aflag = false;
            }
            if (($('[id$=ddlPmtPop]').val()).trim() == '1') {
                if (($('[id$=txtAmt]').val()).trim() == '') {
                    ErrorMessage('Amount is Required.');
                    $('[id$=txtAmt]').focus();
                    aflag = false;
                }
                else if (($('[id$=txtAmt]').val()).trim() == '0') {
                    ErrorMessage('Amount cannot be Zero.');
                    $('[id$=txtAmt]').focus();
                    aflag = false;
                }
                else {
                    $("#ctl00_ContentPlaceHolder1_Panel1").hide();
                    $("#ctl00_ContentPlaceHolder1_ModalPopupExtender1_backgroundElement").hide();
                }
            }
            else if (($('[id$=ddlPmtPop]').val()).trim() == '2') {
                if (($('[id$=txtChqno]').val()).trim() == '') {
                    ErrorMessage('Cheque Number is Required.');
                    $('[id$=txtChqno]').focus();
                    aflag = false;
                }
                else if (($('[id$=txtChqDt]').val()).trim() == '') {
                    ErrorMessage('Cheque Date is Required.');
                    $('[id$=txtChqDt]').focus();
                    aflag = false;
                }
                else {
                    $("#ctl00_ContentPlaceHolder1_Panel1").hide();
                    $("#ctl00_ContentPlaceHolder1_ModalPopupExtender1_backgroundElement").hide();
                }
            }
            else {
                aflag = true;
            }
            return aflag;
        }

        function CancelSelection() {
            $('[id$=ddlFrt]').val("0");
            $('[id$=ddlPmtPop]').val("0");
            $('[id$=txtAmt]').val("");
            $('[id$=txtChqno]').val("");
            $('[id$=txtChqDt]').val("");
            $('[id$=txtRtgsCd]').val("");
        }
        function ShowModelPop() {
            if (($('[id$=ddlFrt]').val()).trim() == '1')
                $find('ctl00_ContentPlaceHolder1_ModalPopupExtender1').show();
            else
                $find('ctl00_ContentPlaceHolder1_ModalPopupExtender1').hide();
        }

        function ChangeDisplay(chkbCntrl, dvCntrl) {
            if ($('[id$=' + chkbCntrl + ']')[0].checked) {
                //$(".chkbgrp").attr("checked", false);
                if (chkbCntrl == 'ctl00_ContentPlaceHolder1_chkbDpchInst') {
                    $('[id$=chkbGdnNmbr]')[0].checked = false;
                    $('[id$=dvGdnNmbr]').hide();
                }
                else if (chkbCntrl == 'ctl00_ContentPlaceHolder1_chkbGdnNmbr') {
                    $('[id$=chkbDpchInst]')[0].checked = false;
                    $('[id$=dvDpchInst]').hide();
                }
                $('[id$=' + dvCntrl + ']').show();
            }
            else {
                $('[id$=' + dvCntrl + ']').hide();
                window.location = "Grn.aspx";
            }
        }

        var message = "Sorry, Right Click is disabled.";
        //this will register click funtion for all the mousedown operations on the page
        document.onmousedown = click;
        function click(e) {
            //for Internet Explore..’2′ is for right click of mouse
            if (event.button == 2) {
                alert(message);
                return false;
            }
            //for other browsers like Netscape 4 etc..
            if (e.which == 3) {
                alert(message);
                return false;
            }
        }
        function DisableRightClk(e) {
            //for Internet Explore..’2′ is for right click of mouse
            if (event.button == 2) {
                alert(message);
                return false;
            }
            //for other browsers like Netscape 4 etc..
            if (e.which == 3) {
                alert(message);
                return false;
            }
        }





        function CheckWayBillNo() {
            var waybillno = $('[id$=txtWBNo]').val();
            var result = Grn.CheckWayBillNo(waybillno);
            if (result.value == false) {
                $("#<%=txtWBNo.ClientID%>")[0].value = '';
                ErrorMessage('L/R No. Exists.');
                $("#<%=txtWBNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function CheckDcNo() {
            var dcno = $('[id$=txtDcNo]').val();
            var result = Grn.CheckDcNo(dcno);
            if (result.value == false) {
                $("#<%=txtDcNo.ClientID%>")[0].value = '';
                ErrorMessage('DC Number Exists.');
                $("#<%=txtDcNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function CheckInvNo() {
            var invno = $('[id$=txtInvNo]').val();
            var result = Grn.CheckInvNo(invno);
            if (result.value == false) {
                $("#<%=txtInvNo.ClientID%>")[0].value = '';
                ErrorMessage('Invoice Number Exists.');
                $("#<%=txtInvNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function doConfirmCT1Dtls(id) {
            try {
                if (confirm("Are you sure you want to Delete CT-1?")) {
                    var result = Grn.DeleteCT1Dtls(id);
                    var DvCT1Dtls = GetClientID("divCT1Dtls").attr("id");
                    $('#' + DvCT1Dtls).html(result.value);
                }
                else {
                    return false;
                }
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }

        function CT1No_Change(RNO) {
            //var ddlCT1No = GetClientID("ddl" + (parseInt(RNo))).attr("id");
            var CT1ID = $('#ddl' + RNO + ' option:selected').val();
            if (CT1ID != '0') {
                var result = Grn.CT1No_Change(RNO, CT1ID);
                var DvCT1Dtls = GetClientID("divCT1Dtls").attr("id");
                $('#' + DvCT1Dtls).html(result.value);

                DatePickerAdd();
            }
            else {
                ErrorMessage('Select CT1 Number in RowNo - ' + RNO);
                $('#ddl' + RNO).focus();
            }
        }

        function AreOne_Value(RNO) {
            var CT1ID = $('#ddl' + RNO + ' option:selected').val();
            var AREVal = $('#txtARE1Value' + RNO).val();
            var HDFNAREVal = $('#hfnAREval' + RNO).val();

            if (CT1ID != '0' && AREVal != '' && AREVal != '0') {
                var result = Grn.AreOne_Value(RNO, CT1ID, AREVal, HDFNAREVal);
                var DvCT1Dtls = GetClientID("divCT1Dtls").attr("id");
                $('#' + DvCT1Dtls).html(result.value);
                if ($('#HfMessage').val() != "")
                    ErrorMessage($('#HfMessage').val());
                DatePickerAdd();
            }
            else if (CT1ID == '0') {
                ErrorMessage('Select CT1 Number in RowNo - ' + RNO);
                $('#ddl' + RNO).focus();
            }
            else if (AREVal == '0') {
                ErrorMessage('Are-1 value cannot be zero in RowNo - ' + RNO);
                $('#txtARE1Value' + RNO).focus();
            }
            else if (AREVal == '') {
                ErrorMessage('Are-1 value cannot be empty in RowNo - ' + RNO);
                $('#txtARE1Value' + RNO).focus();
            }
            else
                ErrorMessage('Fill all the details properly in RowNo - ' + RNO);
        }

        function AreOne_ValueTemp(RNO) {
            try {
                var AREVal = $('#txtARE1Value' + RNO).val();
                var HDFNAREVal = $('#hfnAREval' + RNO).val();

                if (AREVal == '') {
                    ErrorMessage('Are-1 value cannot be empty in RowNo - ' + RNO);
                    $('#txtARE1Value' + RNO).focus();
                }
                else if (AREVal != '') {
                    //$('#hfnAREval' + RNO).val(AREVal);

                    //                    var result = Grn.AreOne_ValueTemp(RNO, CT1ID, AREVal, HDFNAREVal);
                    //                    var DvCT1Dtls = GetClientID("divCT1Dtls").attr("id");
                    //                    $('#' + DvCT1Dtls).html(result.value);
                    //                    if ($('#HfMessage').val() != "")
                    //                        ErrorMessage($('#HfMessage').val());
                    //                    DatePickerAdd();
                }
                else
                    ErrorMessage('Fill all the details properly in RowNo - ' + RNO);
            }
            catch (Error) {
                ErrorMessage(Error.Message);
            }
        }


        function AddCT1Dtls(RNo) {
            try {
                var ddlCT1No = GetClientID("ddl" + (parseInt(RNo))).attr("id");
                var CT1ID = $('#' + ddlCT1No).val();
                var CT1No = $('#' + ddlCT1No + ' option:selected').text();
                var txtDate = GetClientID("txtDate" + (parseInt(RNo))).attr("id");
                var CT1Date = $('#' + txtDate).val();
                var txtValue = GetClientID("txtValue" + (parseInt(RNo))).attr("id");
                var CT1Value = $('#' + txtValue).val();

                var txtARE1No = GetClientID("txtARE1No" + (parseInt(RNo))).attr("id");
                var ARE1No = $('#' + txtARE1No).val();
                var txtARE1Date = GetClientID("txtAREDate" + (parseInt(RNo))).attr("id");
                var ARE1Date = $('#' + txtARE1Date).val();
                var txtAREValue = GetClientID("txtARE1Value" + (parseInt(RNo))).attr("id");
                var ARE1Value = $('#' + txtAREValue).val();


                var chkbFrmWht = GetClientID("chkbARE1FormWht" + (parseInt(RNo))).attr("id");
                var FrmWht = $('#' + chkbFrmWht)[0].checked;
                var chkbFrmBff = GetClientID("chkbARE1FormBff" + (parseInt(RNo))).attr("id");
                var FrmBff = $('#' + chkbFrmBff)[0].checked;
                var chkbFrmBle = GetClientID("chkbARE1FormBle" + (parseInt(RNo))).attr("id");
                var FrmBle = $('#' + chkbFrmBle)[0].checked;
                var chkbFrmGrn = GetClientID("chkbARE1FormGrn" + (parseInt(RNo))).attr("id");
                var FrmGrn = $('#' + chkbFrmGrn)[0].checked;
                var chkbFrmPnk = GetClientID("chkbARE1FormPnk" + (parseInt(RNo))).attr("id");
                var FrmPnk = $('#' + chkbFrmPnk)[0].checked;

                var ARE1Forms = FrmWht + ',' + FrmBff + ',' + FrmBle + ',' + FrmGrn + ',' + FrmPnk;

                var result = Grn.AddCT1Dtls(RNo, CT1ID, CT1No, CT1Date, CT1Value, ARE1No, ARE1Date, ARE1Value, ARE1Forms, 'ChangeRow');
                var DvCT1Dtls = GetClientID("divCT1Dtls").attr("id");
                $('#' + DvCT1Dtls).html(result.value);

                DatePickerAdd();
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }
        //NewCT1Dtls
        function NewCT1Dtls(RNo) {
            try {
                var ddlCT1No = GetClientID("ddl" + (parseInt(RNo))).attr("id");
                var CT1ID = $('#' + ddlCT1No).val();
                var CT1No = $('#' + ddlCT1No + ' option:selected').text();
                var txtDate = GetClientID("txtDate" + (parseInt(RNo))).attr("id");
                var CT1Date = $('#' + txtDate).val();
                var txtValue = GetClientID("txtValue" + (parseInt(RNo))).attr("id");
                var CT1Value = $('#' + txtValue).val();

                var txtARE1No = GetClientID("txtARE1No" + (parseInt(RNo))).attr("id");
                var ARE1No = $('#' + txtARE1No).val();
                var txtARE1Date = GetClientID("txtAREDate" + (parseInt(RNo))).attr("id");
                var ARE1Date = $('#' + txtARE1Date).val();
                var txtAREValue = GetClientID("txtARE1Value" + (parseInt(RNo))).attr("id");
                var ARE1Value = $('#' + txtAREValue).val();

                var chkbFrmWht = GetClientID("chkbARE1FormWht" + (parseInt(RNo))).attr("id");
                var FrmWht = $('#' + chkbFrmWht)[0].checked;
                var chkbFrmBff = GetClientID("chkbARE1FormBff" + (parseInt(RNo))).attr("id");
                var FrmBff = $('#' + chkbFrmBff)[0].checked;
                var chkbFrmBle = GetClientID("chkbARE1FormBle" + (parseInt(RNo))).attr("id");
                var FrmBle = $('#' + chkbFrmBle)[0].checked;
                var chkbFrmGrn = GetClientID("chkbARE1FormGrn" + (parseInt(RNo))).attr("id");
                var FrmGrn = $('#' + chkbFrmGrn)[0].checked;
                var chkbFrmPnk = GetClientID("chkbARE1FormPnk" + (parseInt(RNo))).attr("id");
                var FrmPnk = $('#' + chkbFrmPnk)[0].checked;

                var ARE1Forms = FrmWht + ',' + FrmBff + ',' + FrmBle + ',' + FrmGrn + ',' + FrmPnk;

                if (CT1ID == 0) {
                    ErrorMessage('Select CT-1 No. in Row No-' + RNo);
                    $('#' + ddlCT1No).focus();
                    return false;
                }
                else if (ARE1No == '' || ARE1No == "0") {
                    ErrorMessage('ARE1 No. can not be Empty/Zero(0) in Row No-' + RNo);
                    $('#' + txtARE1No).focus();
                    return false;
                }
                else if (FrmWht != true && FrmBff != true && FrmBle != true && FrmGrn != true && FrmPnk != true) {
                    ErrorMessage('select atleast one ARE1 form in Row No-' + RNo);
                    return false;
                }
                else if (ARE1Value == '' || ARE1Value == '0') {
                    ErrorMessage('ARE-1 Value can not be Empty/Zero(0) in Row No-' + RNo);
                    $('#' + txtAREValue).val(0);
                    $('#' + txtAREValue).focus();
                    return false;
                }
                else {
                    var result = Grn.NewCT1Dtls(RNo, CT1ID, CT1No, CT1Date, CT1Value, ARE1No, ARE1Date, ARE1Value, ARE1Forms);
                    var DvCT1Dtls = GetClientID("divCT1Dtls").attr("id");
                    $('#' + DvCT1Dtls).html(result.value);
                }
                DatePickerAdd();
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }

        function doConfirmARE1Dtls(id) {
            try {
                if (confirm("Are you sure you want to Delete ARE-1?")) {
                    var result = Grn.DeleteARE1Dtls(id);
                    var DvARE1Dtls = GetClientID("divARE1Dtls").attr("id");
                    $('#' + DvARE1Dtls).html(result.value);
                }
                else {
                    return false;
                }
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }

        function AddARE1Dtls(RNo) {
            try {
                var txtARE1No = GetClientID("txtARE1No" + (parseInt(RNo))).attr("id");
                var ARE1No = $('#' + txtARE1No).val();
                var txtValue = GetClientID("txtARE1Value" + (parseInt(RNo))).attr("id");
                var ARE1Value = $('#' + txtValue).val();

                var chkbFrmWht = GetClientID("chkbARE1FormWht" + (parseInt(RNo))).attr("id");
                var FrmWht = $('#' + chkbFrmWht)[0].checked;
                var chkbFrmBff = GetClientID("chkbARE1FormBff" + (parseInt(RNo))).attr("id");
                var FrmBff = $('#' + chkbFrmBff)[0].checked;
                var chkbFrmBle = GetClientID("chkbARE1FormBle" + (parseInt(RNo))).attr("id");
                var FrmBle = $('#' + chkbFrmBle)[0].checked;
                var chkbFrmGrn = GetClientID("chkbARE1FormGrn" + (parseInt(RNo))).attr("id");
                var FrmGrn = $('#' + chkbFrmGrn)[0].checked;
                var chkbFrmPnk = GetClientID("chkbARE1FormPnk" + (parseInt(RNo))).attr("id");
                var FrmPnk = $('#' + chkbFrmPnk)[0].checked;

                var ARE1Forms = FrmWht + ',' + FrmBff + ',' + FrmBle + ',' + FrmGrn + ',' + FrmPnk;

                if (ARE1Value == '') {
                    ErrorMessage('ARE-1 Value can not be Zero(0) in Row No-' + RNo);
                    $('#' + txtValue).focus();
                    $('#' + txtValue).val(0);
                }
                else {
                    var result = Grn.AddARE1Dtls(RNo, ARE1No, ARE1Value, ARE1Forms, 'ChangeRow');
                    var DvARE1Dtls = GetClientID("divARE1Dtls").attr("id");
                    $('#' + DvARE1Dtls).html(result.value);
                }
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }


        function NewARE1Dtls(RNo) {
            try {
                var txtARE1No = GetClientID("txtARE1No" + (parseInt(RNo))).attr("id");
                var ARE1No = $('#' + txtARE1No).val();
                var txtValue = GetClientID("txtARE1Value" + (parseInt(RNo))).attr("id");
                var ARE1Value = $('#' + txtValue).val();

                var chkbFrmWht = GetClientID("chkbARE1FormWht" + (parseInt(RNo))).attr("id");
                var FrmWht = $('#' + chkbFrmWht)[0].checked;
                var chkbFrmBff = GetClientID("chkbARE1FormBff" + (parseInt(RNo))).attr("id");
                var FrmBff = $('#' + chkbFrmBff)[0].checked;
                var chkbFrmBle = GetClientID("chkbARE1FormBle" + (parseInt(RNo))).attr("id");
                var FrmBle = $('#' + chkbFrmBle)[0].checked;
                var chkbFrmGrn = GetClientID("chkbARE1FormGrn" + (parseInt(RNo))).attr("id");
                var FrmGrn = $('#' + chkbFrmGrn)[0].checked;
                var chkbFrmPnk = GetClientID("chkbARE1FormPnk" + (parseInt(RNo))).attr("id");
                var FrmPnk = $('#' + chkbFrmPnk)[0].checked;

                var ARE1Forms = FrmWht + ',' + FrmBff + ',' + FrmBle + ',' + FrmGrn + ',' + FrmPnk;
                //                var chkbForms = GetClientID("txtForms" + (parseInt(RNo))).attr("id");
                //                var ARE1Forms = $('#' + txtValue).val();
                if (ARE1No == '' || ARE1Value == '' || ARE1Value == '0' || ARE1Forms.indexOf("true") == -1) {
                    ErrorMessage('Fill all ARE-1 Details in Row No-' + RNo);
                    $('#' + txtARE1No).focus();
                }
                else {
                    var result = Grn.AddARE1Dtls(RNo, ARE1No, ARE1Value, ARE1Forms, 'NewRow');

                    var DvARE1Dtls = GetClientID("divARE1Dtls").attr("id");
                    $('#' + DvARE1Dtls).html(result.value);
                }
                DatePickerAdd();
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }
        
    </script>
</asp:Content>
