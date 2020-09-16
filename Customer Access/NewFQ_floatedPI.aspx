<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true"
    CodeBehind="NewFQ_floatedPI.aspx.cs" Inherits="VOMS_ERP.Customer_Access.NewFQ_floatedPI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Foreign Quotation"
                                            CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                    align="center">
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Customer<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlcustomer" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlcustomer_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Foreign Enquiry Number<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox runat="server" ID="lblEnquiry" CssClass="bcAspMultiSelectListBox" AutoPostBack="true"
                                SelectionMode="Multiple" OnSelectedIndexChanged="lblEnquiry_SelectedIndexChanged">
                            </asp:ListBox>
                            <%--<asp:DropDownList runat="server" ID="ddlfenq" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlfenq_SelectedIndexChanged">
                                <asp:ListItem Text="--Select Enquiry No--" Value="0"></asp:ListItem>
                            </asp:DropDownList>--%>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Foreign Quotation No. <font color="red" size="2"><b>*</b></font>
                                :</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtfquoteno" CssClass="bcAsptextbox" onchange="CheckEnquiryNo();"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Subject<font color="red" size="2"><b>*</b></font>: </span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtsubject" CssClass="bcAsptextbox" onkeypress="return isSomeSplChar(event)"></asp:TextBox>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Project/Department Name: </span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddldept" CssClass="bcAspdropdown">
                                <asp:ListItem Value="0" Text="Select Departmet"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Quotation Date<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:HiddenField ID="hfFeReceivedDt" runat="server" Value="" />
                            <asp:TextBox runat="server" ID="txtdt" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal" valign="top">
                            <span class="bcLabel">Important Instructions: </span>
                        </td>
                        <td class="bcTdnormal" colspan="3">
                            <asp:TextBox runat="server" ID="txtimpinst" TextMode="MultiLine" Height="48px" Width="179px"
                                CssClass="bcAsptextboxmulti" Style="width: 550px;"></asp:TextBox>
                        </td>
                        <td class="bcTdnormal" colspan="2">
                            <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                <table width="100%">
                                    <tr>
                                        <td align="right" width="44%">
                                            <span class="bcLabel">Comments <font color="red" size="2"><b>*</b></font>: </span>
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
                        <td colspan="6">
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
                        <td class="bcTdnormal" colspan="6">
                            <div style="min-height: 150px; max-height: 250px; overflow: auto;">
                                <asp:GridView runat="server" ID="gvFquoteItems" Width="100%" RowStyle-CssClass="rounded-corner"
                                    GridLines="None" EmptyDataRowStyle-CssClass="rounded-corner" CssClass="rounded-corner"
                                    AllowPaging="true" HeaderStyle-CssClass="rounded-corner" AlternatingRowStyle-CssClass="rounded-corner"
                                    PageSize="100" ShowFooter="true" BorderWidth="0px" AutoGenerateColumns="false"
                                    OnRowDataBound="gvFquoteItems_RowDataBound" OnPreRender="gvFquoteItems_PreRender">
                                    <AlternatingRowStyle CssClass="rounded-corner"></AlternatingRowStyle>
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox runat="server" ID="Chkbxall" AutoPostBack="true" OnCheckedChanged="Chkbxall_OnCheckedChanged" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="ItmChkbx" AutoPostBack="true" OnCheckedChanged="ItmChkbx_OnCheckedChanged" />
                                                <asp:HiddenField ID="hfIsChecked" runat="server" Value='<%# Eval("IsChecked") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="S.No." ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Desc" ControlStyle-Width="300px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemDesc" runat="server" Text='<%# Eval("ItemDesc") %>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblFooterPaging" runat="server" Text=""></asp:Label>
                                            </FooterTemplate>
                                            <ControlStyle Width="300px"></ControlStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Part No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFullName" runat="server" Text='<%# Eval("PartNumber") %>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_OnSelectedIndexChanged">
                                                    <asp:ListItem Text="25" Value="25" />
                                                    <asp:ListItem Text="50" Value="50" />
                                                    <asp:ListItem Text="100" Value="100" Selected="True" />
                                                    <asp:ListItem Text="200" Value="200" />
                                                </asp:DropDownList>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Specifications">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDept" runat="server" Text='<%# Eval("Specifications") %>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Button runat="server" ID="btnPrevious" Text="Previous" OnClick="btnPrevious_Click"
                                                    Style="width: 90px" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Make">
                                            <ItemTemplate>
                                                <asp:Label ID="lblContact" runat="server" Text='<%# Eval("Make") %>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Button runat="server" ID="btnNext" Text="Next" OnClick="btnNext_Click" Style="width: 90px" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Units" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("UnitName") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField FooterStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"
                                            HeaderStyle-HorizontalAlign="left" HeaderStyle-Width="50">
                                            <HeaderTemplate>
                                                <asp:Label ID="lblhdrRt" runat="server" Text=""></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="lblRate" runat="server" Text='<%# Eval("rate") %>' OnTextChanged="lblRate_TextChanged"
                                                    onfocus='this.select()' AutoPostBack="true" onMouseUp='return false' class="bcAsptextbox"
                                                    MaxLength='18' onkeypress='return blockNonNumbers(this, event, true, false);'
                                                    Style="text-align: right !important; width: 50px;" />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                Total :
                                            </FooterTemplate>
                                            <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                            <HeaderStyle HorizontalAlign="Left" Width="50px"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField FooterStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"
                                            HeaderStyle-HorizontalAlign="left">
                                            <HeaderTemplate>
                                                <asp:Label ID="lblDiscount" runat="server" Text="Discount"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDiscount" runat="server" Text='<%# Eval("DiscountPercentage") %>'
                                                    OnTextChanged="txtDicount_TextChanged" onfocus='this.select()' AutoPostBack="true"
                                                    onMouseUp='return false' class="bcAsptextbox" MaxLength='18' onkeypress='return blockNonNumbers(this, event, true, false);'
                                                    Style="text-align: right !important; width: 50px;" />&nbsp;&nbsp;&nbsp;&nbsp;
                                            </ItemTemplate>
                                            <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField FooterStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"
                                            HeaderStyle-HorizontalAlign="left">
                                            <HeaderTemplate>
                                                <asp:Label ID="lblExciseDuty" runat="server" Text="ExciseDuty"></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtExciseDuty" runat="server" OnTextChanged="txtExDuty_TextChanged"
                                                    Text='<%# Eval("ExDutyPercentage") %>' onfocus='this.select()' AutoPostBack="true"
                                                    onMouseUp='return false' class="bcAsptextbox" MaxLength='18' onkeypress='return blockNonNumbers(this, event, true, false);'
                                                    Style="text-align: right !important; width: 50px;" />
                                            </ItemTemplate>
                                            <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="RoundOff">
                                            <HeaderTemplate>
                                                <asp:DropDownList ID="ddlRateChangeHeader" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRateChangeHeader_OnSelectedIndexChanged">
                                                    <asp:ListItem Text="--Select--" Value="0" />
                                                    <asp:ListItem Text="1 Decimals" Value="1" />
                                                    <asp:ListItem Text="2 Decimals" Value="2" />
                                                    <asp:ListItem Text="0 Decimals" Value="3" />
                                                </asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlRateChange" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRateChange_OnSelectedIndexChanged"
                                                    SelectedValue='<%# Bind("RoundOff") %>'>
                                                    <asp:ListItem Text="--Select--" Value="0" />
                                                    <asp:ListItem Text="1 Decimals" Value="1" />
                                                    <asp:ListItem Text="2 Decimals" Value="2" />
                                                    <asp:ListItem Text="0 Decimals" Value="3" />
                                                </asp:DropDownList>
                                                <asp:HiddenField ID="hfSV" runat="server" Value="" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField FooterStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"
                                            HeaderStyle-HorizontalAlign="Right">
                                            <HeaderTemplate>
                                                <asp:Label ID="lblhdrAmt" runat="server" Text=""></asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblmailID" runat="server" Text='<%# Eval("Amount") %>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label runat="server" ID="lbltmnt"></asp:Label>
                                            </FooterTemplate>
                                            <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                            <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataRowStyle CssClass="rounded-corner"></EmptyDataRowStyle>
                                    <HeaderStyle CssClass="rounded-corner"></HeaderStyle>
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="rounded-corner"></RowStyle>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr style="background-color: Gray; font-style: normal; color: White;">
                        <td colspan="6" class="bcTdNewTable">
                            Terms &amp; Conditions
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Price Basis<font color="red" size="2"><b>*</b></font>: </span>
                        </td>
                        <td class="bcTdnormal" colspan="2">
                            <asp:DropDownList runat="server" ID="ddlPrcBsis" CssClass="bcAspdropdown">
                                <asp:ListItem Text="Select Price Basis" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <asp:TextBox runat="server" CssClass="bcAsptextboxRight" onkeypress='return SpclChars(event);'
                                ID="txtPriceBasis"></asp:TextBox>
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Delivery Period<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtDlvry" CssClass="bcAsptextbox" Width="50px" onkeyup="extractNumber(this, 0, false);"
                                onblur="extractNumber(this, 0, false); chkDlvrPeriod()" onfocus="this.select()"
                                onMouseUp="return false" MaxLength="2"></asp:TextBox>Weeks
                        </td>
                    </tr>
                    <tr style="background-color: Gray; font-style: normal; color: White;">
                        <td colspan="6" class="bcTdNewTable">
                            Payment Terms:<%--<span class="bcLabel" style="font-size: small; font-style: italic"></span>--%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            <center>
                                <div style="overflow: auto; width: 35%;" id="divPaymentTerms" runat="server">
                                </div>
                            </center>
                        </td>
                    </tr>
                    <tr style="background-color: Gray; font-style: normal; color: White;">
                        <td colspan="6" class="bcTdNewTable">
                            Estimation
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdNewTable1">
                            <asp:Label ID="lblTtlAmt" CssClass="bcLabel" runat="server" Text="Total Amount(Rs):"></asp:Label>
                        </td>
                        <td class="bcTdNewTable1">
                            <asp:TextBox runat="server" ID="txtTotalAmount" CssClass="bcAsptextbox" onkeypress='return isNumberKey(event)'
                                Font-Bold="True"></asp:TextBox>
                        </td>
                        <%--  <td class="bcTdNewTable1">
                            <span class="bcLabel">Fob + Margin(%)<font color="red" size="2"><b>*</b></font>:</span>
                        </td>--%>
                        <%-- <td>
                            <asp:TextBox visible="false" runat="server" ID="txtMargin" class="positive" onblur="extractNumber(this,2,false);"
                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                onkeypress="return blockNonNumbers(this, event, true, false);" MaxLength="5"
                                AutoPostBack="True" CssClass="bcAsptextbox" OnTextChanged="txtMargin_TextChanged"></asp:TextBox>
                        </td>--%>
                        <td class="bcTdNewTable1">
                            <span class="bcLabel">Conversion Rate<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdNewTable1">
                            <asp:TextBox runat="server" ID="txtCvsRt" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                onkeypress="return blockNonNumbers(this, event, true, false);" MaxLength="5"
                                AutoPostBack="True" OnTextChanged="txtCvsRt_TextChanged"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable" align="right">
                            <span><a href="javascript:void(0)" id="lbtnATConditions" title="Add Addtional Terms & Conditions"
                                onclick="fnOpen()" class="bcAlink">Additional Terms & Conditions </a></span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
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
                                                    <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
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
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <style type="text/css">
        tr.even:hover td, #Compare_Range_wrapper tr.even:hover .dataTable DTFC_Cloned
        {
            background-color: #CBCBCB;
        }
        tr.odd:hover td, #Compare_Range_wrapper tr.odd:hover .dataTable DTFC_Cloned
        {
            background-color: #CBCBCB;
        }
    </style>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });

        function CheckEnquiryNo() {
            var enqNo = $('[id$=txtfquoteno]').val().trim();
            var result = NewFQuotation.CheckEnquiryNo(enqNo);
            if (result.value == false) {
                $('[id$=txtfquoteno]').focus();
                $('[id$=txtfquoteno]').val('');

                ErrorMessage('Quotation Number Exists.');

                return false;
            }
        }

        function chkAllCheckbox(obj) {
            var gv = $("#<%=gvFquoteItems.ClientID %> input");
            for (var i = 0; i < gv.length; i++) {
                if (gv[i].type == 'checkbox') {
                    gv[i].checked = obj.checked;
                }
            }
        }

        $(".positive").numeric({ negative: false }, function () { alert("No negative values"); this.value = ""; this.focus(); });

        function isSomeSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) &&
            (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false; //charCode != 32 &&
            return true;
        }

        $('[id$=txtMargin]').keyup(function (e) {
            var margin = $('[id$=txtMargin]').val();
            if (margin > 99.99) {

                ErrorMessage('Margin Cannot be Grater than 99.99%');

                $('[id$=txtMargin]').val('0');
                return false;
            }
        });

        function removeDes(me) {
            var txt = me.value;
            if (txt.indexOf(".") > -1 && txt.split(".")[1].length > 2) {
                var substr = txt.split(".")[1].substring(0, 2);
                me.value = txt.split(".")[0] + "." + substr;
            }
        }

        function CalculatetoDollar() {
            var result = NewFQuotation.ConverttoDollar();
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
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

        function doConfirmPayment(id) {
            if (confirm("Are you sure you want to Delete Payment?")) {
                //DeleteItem(id);
                var result = NewFQuotation.PaymentDeleteItem(id);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);
            }
            else {
                return false;
            }
        }

        function getPaymentValues(RNo) {
            var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(RNo))).attr("id");
            var PercAmt = $('#' + txtPercAmt).val();
            var txtDesc = GetClientID("txtDesc" + (parseInt(RNo))).attr("id");
            var Desc = $('#' + txtDesc).val();
            if (PercAmt != '0' && PercAmt != '' && Desc != '') {
                var result = NewFQ_floatedPI.PaymentAddItem(RNo, PercAmt, Desc);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);

                if ($('[id$=HfMessage]').val() != '')//HfMessage
                {
                    ErrorMessage($('[id$=HfMessage]').val());
                }
            }
            else {
                if (PercAmt == '') {
                    ErrorMessage('Payment Percentage is Required.');
                }
                else if (PercAmt == '0') {
                    ErrorMessage('Payment Percentage cannot be Zero.');
                }
                else if (PercAmt > 100) {
                    ErrorMessage('Percentage Cannot Exceed 100');
                    $('#' + txtPercAmt).val('0');
                    $('[id$=' + txtPercAmt + ']').focus();
                }
                $('#' + txtPercAmt).focus();
            }
            $('#' + txtDesc).focus();
        }

        function chkDlvrPeriod() {
            var Currentweeks = $('[id$=txtDlvry]').val();
            if (Currentweeks == '0' || Currentweeks == '') {
                ErrorMessage('Delivery Period should not be Zero, or Empty');
                $('[id$=txtDlvry]').focus(0, 0);
            }
        }

        function Myvalidations() {
            var TotPayAmt = 0;
            var FQItems = ($('[id$=gvFquoteItems]').length > 0) ? $('[id$=gvFquoteItems]')[0].rows.length : 0;
            var PaymentRCount = $('#tblPaymentTerms tbody tr').length;
            var atxtDlvry = $('[id$=txtDlvry]').val();
            if (($('[id$=ddlcustomer]').val()).trim() == '0') {
                ErrorMessage(' Customer is Required.');
                $('[id$=ddlcustomer]').focus();
                return false;
            }
            else if ($('[id$=lblEnquiry]').val() == null || $('[id$=lblEnquiry]').val() == '') {
                ErrorMessage('Foreign Enquiry Number is Required.');
                $('[id$=lblEnquiry]').focus();
                return false;
            }
            else if (($('[id$=txtfquoteno]').val()).trim() == '') {
                ErrorMessage('Foreign Quotation Number is Required.');
                $('[id$=txtlquotno]').focus();
                return false;
            }
            else if (($('[id$=txtsubject]').val()).trim() == '') {
                ErrorMessage('Subject is Required.');
                $('[id$=txtsubject]').focus();
                return false;
            }
            else if (($('[id$=ddldept]').val()).trim() == '0') {

                ErrorMessage('Department is Required.');
                $('[id$=ddllenq]').focus();

                return false;
            }
            else if (($('[id$=txtdt]').val()).trim() == '') {

                ErrorMessage('Quotation Date is Required.');
                $('[id$=txtdt]').focus();

                return false;
            }

            else if (FQItems == 0) {

                ErrorMessage('No Items to Save.');
                $('[id$=gvFquoteItems]').focus();

                return false;
            }
            else if (FQItems == 1) {

                ErrorMessage('No Items to Save.');
                $('[id$=gvFquoteItems]').focus();

                return false;
            }

            //            for (var i = 0; i < FQItems; i++) {
            //            else if (($('[id$=lblRate]').val()).trim() == '' || ($('[id$=lblRate]').val()).trim() == '0.00' || ($('[id$=lblRate]').val()).trim() == '0') {
            //                ErrorMessage('Rate is Required.');
            //                //$('[id$=lblRate]').focus();
            //                return false;       
            //        }
            //         
            //        }

            else if (($('[id$=ddlPrcBsis]').val()).trim() == '0') {

                ErrorMessage('Price Basis is Required.');
                $('[id$=ddlPrcBsis]').focus();

                return false;
            }
            else if (($('[id$=txtPriceBasis]').val()).trim() == '') {

                ErrorMessage('Price Basis Location is Required.');
                $('[id$=txtPriceBasis]').focus();

                return false;
            }
            else if (($('[id$=txtDlvry]').val()).trim() == '') {
                ErrorMessage('Delivery Period is Required.');
                $('[id$=txtDlvry]').focus();

                return false;
            }
            else if (atxtDlvry == '0' || atxtDlvry == '') {

                ErrorMessage('Delivery Period should not be Zero OR Empty.');
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
                        //alert('sample : ' + $('#ddlUnits' + '' + i).val());
                        var message = '';
                        if (PercAmt == '')
                            message = 'Payment Percentage Is Required';
                        else if (PercAmt == '0')
                            message = 'Payment Percentage Cannot be Zero';
                        else if (Desc == '')
                            message = 'Description Is Required';

                        ErrorMessage(message + ' of SNo : ' + k + '.');

                        return false;
                        break;
                    }
                }
                if (PaymentRCount == 0) {

                    ErrorMessage('Total Price percentage should be 100.');

                    return false;
                }
            }
            if (PaymentRCount == 0 || TotPayAmt < 100) {

                ErrorMessage('Total Price percentage should be 100.');

                return false;
            }
            //            if (($('[id$=txtMargin]').val()).trim() == '') {

            //                ErrorMessage('Margin is Required.');
            //                $('[id$=txtMargin]').focus();

            //                return false;
            //            }
            else if (($('[id$=txtCvsRt]').val()).trim() == '') {

                ErrorMessage('Conversion rate is Required.');
                $('[id$=txtCvsRt]').focus();

                return false;
            }
            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()) == "") {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            else {
                return true;
            }
        }


        function fnSetValues() {
            var iHeight = 500;
            var iWidth = 1000;
            var sFeatures = "dialogHeight: " + iHeight + "px; dialogWidth: " + iWidth + "px;";
            return sFeatures;
        }
        function fnOpen(id, rowIndex) {
            var sFeatures = fnSetValues();
            window.showModalDialog("../Masters/TermsNConditions.aspx?TAr=General", "508", sFeatures); //../Enquiries/AddItems.aspx
            //FillItemGrid(id, rowIndex);
        }


        function uploadComplete() {
            var result = NewFQuotation.AddItemListBox();
            var getDivLEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivLEItems).html(result.value);
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
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = NewFQuotation.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';

                    //SuccessMessage('Selected Attachment Deleted Successfully.');
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
        });

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
