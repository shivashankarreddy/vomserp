<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Gdn.aspx.cs" Inherits="VOMS_ERP.Logistics.Gdn" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Goods Dispatch Note(GDN)"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
                                        <span id="Span17" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <span id="Span3" class="bcLabel">Dispatch Instructions No. <font color="red" size="2">
                                            <b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlDspchInst" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlDspchInst_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span34" class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" Enabled="false"
                                            CssClass="bcAspMultiSelectListBox"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lblpos" SelectionMode="Multiple" Enabled="false"
                                            CssClass="bcAspMultiSelectListBox"></asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Dispatch Destination <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtDspchDstn" CssClass="bcAsptextbox" Visible="false"></asp:TextBox>
                                        <asp:DropDownList runat="server" ID="ddlDspchDstn" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Supplier Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtSuplr" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                        <asp:HiddenField runat="server" ID="hdfSuplrID" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Transporter Name :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTnptrNm" onkeypress="return isTransporterName(event)"
                                            CssClass="bcAsptextbox" MaxLength="100"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Dispatch Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRcvdDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">L/R No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtWBNo" MaxLength="75" onkeypress="return isWayBill(event)"
                                            onchange="javascript:return CheckWayBillNo();" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">L/R Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtWBDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Truck No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTkNo" onkeypress="return isAlphaNumaricSpace(event)"
                                            MaxLength="13" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
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
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Gross Weight <font color="red" size="2"><b>*</b></font>
                                            : Kgs </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtGW" onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            MaxLength="15" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">Net Weight <font color="red" size="2"><b>*</b></font>
                                            : Kgs </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtNW" onchange="return CheckWeights(event)" onkeyup="extractNumber(this,3,true);"
                                            onkeypress="return blockNonNumbers(this, event, true, false);" CssClass="bcAsptextbox"
                                            MaxLength="15"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span18" class="bcLabel">Freight :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlFrt" CssClass="bcAspdropdown" AutoPostBack="false"
                                            onchange="ShowModelPop();">
                                            <%--OnSelectedIndexChanged="ddlFrt_SelectedIndexChanged"--%>
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="To-Pay" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Pre-Paid" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField runat="server" ID="hdfempty" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span12" class="bcLabel">DC No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtDcNo" MaxLength="75" onkeypress="return isWayBill(event)"
                                            onchange="javascript:return CheckDcNo();" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span14" class="bcLabel">DC No. Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtDcDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span20" class="bcLabel">Condition of Goods :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlGdsCndtn" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Good" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Bad" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Breakage" Value="3"></asp:ListItem>
                                            <%--<asp:ListItem Text="" Value="4"></asp:ListItem>--%>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span15" class="bcLabel">Invoice No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtInvNo" MaxLength="75" onkeypress="return isWayBill(event)"
                                            onchange="javascript:return CheckInvNo();" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span16" class="bcLabel">Invoice Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtInvDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2" style="display: none;">
                                        <div visible="false" runat="server" id="dvpmmdm">
                                            <span id="Span19" class="bcLabel">Payment Mode <font color="red" size="2"><b>*</b></font>
                                                :</span>
                                            <asp:DropDownList runat="server" ID="ddlPmtMd" CssClass="bcAspdropdown">
                                                <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Cash" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Cheque" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Remarks :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtremarks" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">ARE-1 Forms & Additional Documents: </span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtAdtnlDocs" TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="49%">
                                                        <span id="Span13" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
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
                    <tr style="background-color: Gray; font-style: normal; color: White;">
                        <td colspan="6">
                            <b>&nbsp;&nbsp;Item Details</b>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            <div style="min-height: 150px; max-height: 250px; overflow: auto;">
                                <asp:GridView runat="server" ID="gvGDN" AutoGenerateColumns="false" RowStyle-CssClass="rounded-corner"
                                    EmptyDataRowStyle-CssClass="rounded-corner" CssClass="rounded-corner" HeaderStyle-CssClass="rounded-corner"
                                    AlternatingRowStyle-CssClass="rounded-corner" BorderWidth="0px" Width="100%"
                                    ShowFooter="true" EmptyDataText="No Records To Display...!" EmptyDataRowStyle-HorizontalAlign="Center"
                                    GridLines="None" OnRowDataBound="gvGDN_RowDataBound" OnPreRender="gvGDN_PreRender"
                                    AllowPaging="true" PageSize="100">
                                    <AlternatingRowStyle CssClass="rounded-corner"></AlternatingRowStyle>
                                    <%-- OnRowCommand="gvGDN_RowCommand"--%>
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox runat="server" ID="chkbhdr" OnCheckedChanged="chkbhdr_OnCheckedChanged"
                                                    AutoPostBack="true" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkbitm" OnCheckedChanged="chkbitm_OnCheckedChanged" Checked='<%#bool.Parse(Eval("IsItemCheck").ToString())%>'
                                                    AutoPostBack="true"/>
                                                <asp:HiddenField ID="hfFSNo" runat="server" Value='<%# Eval("Sno") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="IDs" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemDtlsID" runat="server" Text='<%# Eval("StockItemsID") %>'></asp:Label>
                                                <asp:Label ID="lblItemID" runat="server" Text='<%# Eval("ItemId") %>'></asp:Label>
                                                <asp:Label ID="lblLclPoID" runat="server" Text='<%# Eval("LocalPOId") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="S.No.">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                                <asp:Label ID="lblSerialNo" runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblFooterPaging" runat="server" Text=""></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Desc" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="txtItmDesc" TextMode="MultiLine" CssClass="bcAsptextbox"
                                                    Text='<%# Eval("Description") %>' onblur="ReSizeTXT(this.id)" onfocus="ExpandTXT(this.id)"></asp:TextBox>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_OnSelectedIndexChanged">
                                                    <asp:ListItem Text="25" Value="25" />
                                                    <asp:ListItem Text="50" Value="50" />
                                                    <asp:ListItem Text="100" Value="100" Selected="True" />
                                                    <asp:ListItem Text="200" Value="200" />
                                                </asp:DropDownList>
                                            </FooterTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Part No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFullName" runat="server" Text='<%# Eval("PartNumber") %>' />
                                                <asp:HiddenField ID="hfHSCode" runat="server" Value='<%# Eval("HSCode") %>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Button runat="server" ID="btnPrevious" Text="Previous" OnClick="btnPrevious_Click"
                                                    Style="width: 90px" />
                                            </FooterTemplate>
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
                                            <FooterTemplate>
                                                <asp:Button runat="server" ID="btnNext" Text="Next" OnClick="btnNext_Click" Style="width: 90px" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Actual Qty" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dispatch Qty" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="txtCrntQty" CssClass="bcAsptextbox" Width="50px"
                                                    Text='<%# Eval("DspchQty") %>' AutoPostBack="true" onkeyup="this.value=this.value.replace(/[^0-9 \.]/g,'');"
                                                    OnTextChanged="txtRcvdQty_TextChanged"></asp:TextBox>
                                            </ItemTemplate>
                                             <%--onkeyup="extractNumber(this,2,false);"
                                                    onkeypress="return blockNonNumbers(this, event, true, false);"--%>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="GDN/GRN Rcvd Qty" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRcvdQty" runat="server" Text='<%# Eval("ReceivedQty") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rmng Qty" ItemStyle-HorizontalAlign="Center">
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
                            <span id="Span10" class="bcLabel">Payment Mode <font color="red" size="2"><b>*</b></font>
                                :</span>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlPmtPop" CssClass="bcAspdropdown" onchange="DisplayChanges()">
                                <%--onselectedindexchanged="ddlPmtPop_SelectedIndexChanged"--%>
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
                                            <span id="Span22" class="bcLabel">
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
                                            <span id="Span23" class="bcLabel">Cheque No. <font color="red" size="2"><b>*</b></font>
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
                                            <span id="Span24" class="bcLabel">Cheque Date <font color="red" size="2"><b>*</b></font>
                                                :</span>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" MaxLength="10" Width="50%" CssClass="bcAsptextbox" ID="txtChqDt"></asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Span25" class="bcLabel">NEFT/RTGS Code :</span>
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
                <input id="btnCancel" type="button" value="Cancel" onclick="$find('ModalPopupExtender1').hide(); return false;" />
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
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
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

        function GetProformaInvNo() {
            var refNo = $('[id$=txtpmIn]').val();
            var result = IomForm.GetProformaInvNo(refNo);
            if (result.value == false) {
                $('[id$=txtpmIn]').val('');
                $('[id$=txtpmIn]').focus();
                ErrorMessage('This Proforma Invoice No. is already in Use');
            }
        }

        function CheckActualQty(id, rstval, ActlQty) {
            try {
                var Rslt = confirm('Dispatch Quantity is more than Actual Quantity. Do you want to proceed?');
                if (Rslt) {
                    //$('[id$=' + rstval + ']').text('0');
                }
                else {
                    $('[id$=' + rstval + ']').text($('[id$=' + ActlQty + ']').text());
                    $('[id$=' + id + ']').val('0');
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtRcvdDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
                //,
                // maxDate: dateToday
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
        });

        function chkAllCheckbox(obj) {
            var gv = $("#<%=gvGDN.ClientID %> input");
            var parentTr = obj.parentNode.parentNode;
            var txtScore = document.getElementById(obj.id);

//            var index = txtid.parentNode.parentNode.sectionRowIndex; // Get the corresponding index 
            var grid = $find("<%=gvGDN.ClientID %>");
            var MasterTable = grid.get_masterTableView();
            var radTextbox = MasterTable.get_dataItems()[index].findControl("chkbitm"); //accessing RadControls 
            alert(radTextbox.get_value());

            for (var i = 0; i < gv.length; i++) {
                //var node = gv[i];
                //if (node.type != 'checkbox') continue;
                if (gv[i].type == 'checkbox') {
                    gv[i].checked = obj.checked;
                }
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
        function isWayBill(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 32 && charCode != 47 && charCode != 45 &&
            (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function isTransporterName(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 32 && charCode != 38 && charCode != 46 &&
            (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
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
            //            var charCode = (evt.which) ? evt.which : event.keyCode;
            //            if (charCode > 31 && (charCode < 48 || charCode > 57))
            //                return false;
            //            return true;
        }
        function isDecimalKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
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

        function uploadComplete() {
            var result = Gdn.AddItemListBox();
            var getDivGDNItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGDNItems).html(result.value);
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
                    var result = Gdn.DeleteItemListBox($('#lbItems').val());
                    var getDivGDNItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivGDNItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
        });


        $('#lnkAdd').click(function () {
            //if ($('#lbItems').val() != "") {
            var result = Gdn.AddItemListBox();
            var getDivGDNItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGDNItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            // }
        });

        function Myvalidations() {
            try {
                if ($('[id$=gvGDN]').length != 0)
                    var GdnItems = $('[id$=gvGDN]')[0].rows.length;
                if (($('[id$=ddlDspchInst]').val()).trim() == '00000000-0000-0000-0000-000000000000') {

                    ErrorMessage('Dispatch Instructions Number is Required.');
                    $('[id$=ddlDspchInst]').focus();

                    return false;
                } //ddlRefno
                else if ($('[id$=lbfpos]').val() == null) {

                    ErrorMessage('Frn PO Number is Required.');
                    $('[id$=lbfpos]').focus();

                    return false;
                }
                else if ($('[id$=lblpos]').val() == null) {

                    ErrorMessage('Lcl PO Number is Required.');
                    $('[id$=lblpos]').focus();

                    return false;
                }
                else if ($('[id$=ddlDspchDstn]').length > 0 && ($('[id$=ddlDspchDstn]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Dispatch Destination is Required.');
                    $('[id$=ddlDspchDstn]').focus();
                    return false;
                }
                else if ($('[id$=txtDspchDstn]').length > 0 && ($('[id$=txtDspchDstn]').val()).trim() == '') {
                    ErrorMessage('Dispatch Destination is Required.');
                    $('[id$=txtDspchDstn]').focus();
                    return false;
                }
                else if (($('[id$=txtSuplr]').val()).trim() == '') {

                    ErrorMessage('Supplier Name is Required.');
                    $('[id$=txtSuplr]').focus();

                    return false;
                }
                //            else if (($('[id$=txtTnptrNm]').val()).trim() == '') {
                //                
                //                ErrorMessage('Transporter Name is Required.');
                //                $('[id$=txtTnptrNm]').focus();
                //                
                //                return false;
                //            }
                else if (($('[id$=txtRcvdDt]').val()).trim() == '') {

                    ErrorMessage('Dispatch Date is Required.');
                    $('[id$=txtRcvdDt]').focus();

                    return false;
                }
                //            else if (($('[id$=txtWBNo]').val()).trim() == '') {
                //                
                //                ErrorMessage('Way Bill Number is Required.');
                //                $('[id$=txtWBNo]').focus();
                //                
                //                return false;
                //            }

                //                else if (($('[id$=txtWBDt]').val()).trim() == '') {
                //                    
                //                    ErrorMessage('Way Bill Date is Required.');
                //                    $('[id$=txtWBDt]').focus();
                //                    
                //                    return false;
                //                }
                if (($('[id$=txtWBNo]').val()).trim() != '') {
                    if (($('[id$=txtWBDt]').val()).trim() == '') {

                        ErrorMessage('L/R Date is Required.');
                        $('[id$=txtWBDt]').focus();

                        return false;
                    }
                }
                //                if (($('[id$=txtTkNo]').val()).trim() == '') {
                //                    
                //                    ErrorMessage('Truck Number is Required.');
                //                    $('[id$=txtTkNo]').focus();
                //                    
                //                    return false;
                //                }
                if (($('[id$=ddlPkngTp]').val()).trim() == '00000000-0000-0000-0000-000000000000') {

                    ErrorMessage('Packing Type is Required.');
                    $('[id$=ddlPkngTp]').focus();

                    return false;
                }
                else if (($('[id$=txtPkngNo]').val()).trim() == '' || ($('[id$=txtPkngNo]').val()).trim() == '0') {

                    ErrorMessage('Number of Packages is Required.');
                    $('[id$=txtPkngNo]').focus();

                    return false;
                }
                else if (($('[id$=txtGW]').val()).trim() == '') {

                    ErrorMessage('Gross Weight is Required.');
                    $('[id$=txtGW]').focus();

                    return false;
                }
                else if (($('[id$=txtNW]').val()).trim() == '') {

                    ErrorMessage('Net Weight is Required.');
                    $('[id$=txtNW]').focus();

                    return false;
                }
                //                else if (($('[id$=ddlFrt]').val()).trim() == '0') {

                //                    ErrorMessage('Freight is Required.');
                //                    $('[id$=ddlFrt]').focus();

                //                    return false;
                //                }
                //            else if (($('[id$=txtDcNo]').val()).trim() == '') {
                //                
                //                ErrorMessage('Delivery Challana Number is Required.');
                //                $('[id$=txtDcNo]').focus();
                //                
                //                return false;
                //            }
                //            else if (($('[id$=txtDcDt]').val()).trim() == '') {
                //                
                //                ErrorMessage('Delivery Challana Date is Required.');
                //                $('[id$=txtDcDt]').focus();
                //                
                //                return false;
                //            }
                //            else if (($('[id$=txtInvNo]').val()).trim() == '') {
                //                
                //                ErrorMessage('Invoice Number is Required.');
                //                $('[id$=txtInvNo]').focus();
                //                
                //                return false;
                //            }
                //            else if (($('[id$=txtInvDt]').val()).trim() == '') {
                //                
                //                ErrorMessage('Invoice Date is Required.');
                //                $('[id$=txtInvDt]').focus();
                //                
                //                return false;
                //            }
                //                else if (($('[id$=ddlPmtMd]').val()).trim() == '0') {
                //                    
                //                    ErrorMessage('Payment Mode is Required.');
                //                    $('[id$=ddlPmtMd]').focus();
                //                    
                //                    return false;
                //                }
                //            else if (($('[id$=txtArefmsNo]').val()).trim() == '') {
                //                
                //                ErrorMessage('No. of ARE-1 Forms is Required.');
                //                $('[id$=txtArefmsNo]').focus();
                //                
                //                return false;
                //            }
                //                else if ($("#ctl00_ContentPlaceHolder1_chkblAREfms input:checked").length == 0) {
                //                    
                //                    ErrorMessage('At Least one ARE-1 Form is Required.');
                //                    $('[id$=chkblAREfms]').focus();
                //                    
                //                    return false;
                //                }
                if (GdnItems > 0) {
                    if (GdnItems == 1) {

                        ErrorMessage('No Items to Save.');
                        $('[id$=gvLpoItems]').focus();

                        return false;
                    }
                    else {
                        var select = 0;
                        for (var i = 2; i <= GdnItems; i++) {
                            var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                            var chkbval = GetClientID(chkbx + "_chkbitm").attr("id");
                            var CrntQty = GetClientID(chkbx + "_txtCrntQty").attr("id");
                            if (chkbval == undefined) {
                                continue;
                            }
                            else if ($('#' + chkbval)[0].checked) {
                                if (parseInt($('[id$=' + CrntQty + ']').val()) > 0)
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
                else if (GdnItems <= 0) {

                    ErrorMessage('No Item to Save.');
                    $('[id$=gvLpoItems]').focus();

                    return false;
                }
                if ($('[id$=DivComments]').css("visibility") == "visible") {
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
                if (($('[id$=txtAmt]').val()).trim() == '' || ($('[id$=txtAmt]').val()).trim() == '0' || ($('[id$=txtAmt]').val()).trim() == '0.00') {

                    if (($('[id$=txtAmt]').val()).trim() == '0') {
                        ErrorMessage('Amount shouldnot be Zero.');
                    }
                    if (($('[id$=txtAmt]').val()).trim() == '0.00') {
                        ErrorMessage('Amount shouldnot be Zero.');
                    }
                    else
                        ErrorMessage('Amount is Required.');
                    $('[id$=txtAmt]').focus();


                    aflag = false;
                }
                else {

                    $("#ctl00_ContentPlaceHolder1_Panel1").hide();    //$('[id$=ModalPopupExtender1]').hide();
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
                    $("#ctl00_ContentPlaceHolder1_Panel1").hide();    //$('[id$=ModalPopupExtender1]').hide();
                    $("#ctl00_ContentPlaceHolder1_ModalPopupExtender1_backgroundElement").hide();
                }
            }
            else {
                //                $("#ctl00_ContentPlaceHolder1_Panel1").hide();
                //                $("#ctl00_ContentPlaceHolder1_ModalPopupExtender1_backgroundElement").hide();
                aflag = true;
            }
            return aflag;
        }
        function CancelSelection() {
            document.getElementById("ctl00_ContentPlaceHolder1_ddlFrt").value = "0";
            $('[id$=ddlPmtPop]').val("0");
            $('[id$=txtAmt]').val("");
            $('[id$=txtChqno]').val("");
            $('[id$=txtChqDt]').val("");
            $('[id$=txtRtgsCd]').val("");
            //$find('ModalPopupExtender1').hide();
            //document.getElementById("ctl00_ContentPlaceHolder1_ModalPopupExtender1").hide();
            $find('ctl00_ContentPlaceHolder1_ModalPopupExtender1').hide();
        }
        function ShowModelPop() {
            if (($('[id$=ddlFrt]').val()).trim() == '1')
                $find('ctl00_ContentPlaceHolder1_ModalPopupExtender1').show();
            else
                $find('ctl00_ContentPlaceHolder1_ModalPopupExtender1').hide();
        }


        function CheckWayBillNo() {
            var waybillno = $('[id$=txtWBNo]').val();
            var result = Gdn.CheckWayBillNo1(waybillno);
            if (result.value == false) {
                $("#<%=txtWBNo.ClientID%>")[0].value = '';
                ErrorMessage('L/R NUMBER Exists.');
                $("#<%=txtWBNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function CheckDcNo() {
            var dcno = $('[id$=txtDcNo]').val();
            var result = Gdn.CheckDcNo1(dcno);
            if (result.value == false) {
                $("#<%=txtDcNo.ClientID%>")[0].value = '';
                ErrorMessage('Dc No. Exists.');
                $("#<%=txtDcNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function CheckInvNo() {
            var invno = $('[id$=txtInvNo]').val();
            var result = Gdn.CheckInvNo1(invno);
            if (result.value == false) {
                $("#<%=txtInvNo.ClientID%>")[0].value = '';
                ErrorMessage('Invoice No. Exists.');
                $("#<%=txtInvNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }



    </script>
</asp:Content>
