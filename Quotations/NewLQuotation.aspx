<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="NewLQuotation.aspx.cs" Inherits="VOMS_ERP.Quotations.NewLQuotation" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="HselectedItems" runat="server" Value="" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblTitle" runat="server" Text="Local Quotation" CssClass="bcTdTitleLabel"></asp:Label><div
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
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td>
                                        <span id="lblCust" class="bcLabel">Customer Name <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlcustomer" AutoPostBack="true" Enabled="true"
                                            CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlcustomer_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="HFID" runat="server" Value="" />
                                    </td>
                                    <td>
                                        <span id="lblFEN" class="bcLabel">Foreign Enquiry No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlfenq" AutoPostBack="true" CssClass="bcAspdropdown"
                                            OnSelectedIndexChanged="ddlfenq_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <span id="lblLEN" class="bcLabel">Local Enquiry No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddllenq" AutoPostBack="true" CssClass="bcAspdropdown"
                                            OnSelectedIndexChanged="ddllenq_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Select L-Enquiry"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span id="lblSupplier" class="bcLabel">Supplier Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lblsupplier" Text="SUPPLIER ORGANIZATION NAME" CssClass="bcLabel"></asp:Label>
                                        <asp:Label runat="server" ID="lblsupplierID" Text="" Visible="false" CssClass="bcLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <span id="lblQN" class="bcLabel">Quotation No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtlquotno" CssClass="bcAsptextbox" MaxLength="350"
                                            onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9 \-&_/.:]/g,'');" onchange="CheckEnquiryNo();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="Span3" class="bcLabel">LQ Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtqdt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span id="lblQDt" class="bcLabel">LQ Entry Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:HiddenField ID="hfLeIssueDt" runat="server" Value="" />
                                        <asp:TextBox runat="server" ID="txtdt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="lblSub" class="bcLabel">Subject<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtsubject" CssClass="bcAsptextbox" onkeyup="this.value=this.value.replace(/[^a-zA-Z ]/g,'');"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="lblDeptName" class="bcLabel">Project/Department Name</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddldept" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Departmet"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span id="lblImpI" class="bcLabel">Important Instructions:</span>
                                    </td>
                                    <td colspan="3">
                                        <asp:TextBox runat="server" ID="txtimpinst" TextMode="MultiLine" Height="48px" Width="179px"
                                            CssClass="bcAsptextboxmulti" Style="width: 550px;"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="42%">
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
                                    <td style="overflow: auto;" colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <div id="divLQItems" runat="server" style="overflow: auto; width: 100%; max-height: 250px;
                                                        min-height: 200px;">
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr style="background-color: Gray; font-style: normal; color: White;">
                                                <td colspan="6">
                                                    <b>Terms & Conditions</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="6">
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td colspan="6">
                                                                <table width="100%">
                                                                    <tr>
                                                                        <td width="20px" valign="top">
                                                                            <span id="lblDis" class="bcLabel">Discount:</span>
                                                                            <asp:CheckBox runat="server" ID="chkDsnt" Text=" " onclick='LqCHeck("chkDsnt", "dvDsnt", "txtDsnt")'
                                                                                CssClass="bcCheckBoxList" />
                                                                        </td>
                                                                        <td width="30px">
                                                                            <div id="dvDsnt" style="display: none;">
                                                                                <asp:RadioButtonList runat="server" ID="rbtnDsnt" RepeatDirection="Horizontal">
                                                                                    <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                                <asp:TextBox runat="server" ID="txtDsnt" MaxLength="10" onblur="extractNumber(this,2,false);"
                                                                                    onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                                                    onkeypress="return blockNonNumbers(this, event, true, false);" onchange="CalculateExDuty();"
                                                                                    Style="text-align: right;" Width="40px" CssClass="bcAsptextbox"></asp:TextBox></div>
                                                                        </td>
                                                                        <td width="25px" valign="top">
                                                                            <span id="lblED" runat="server" class="bcLabel">CGST:</span>
                                                                            <asp:CheckBox runat="server" ID="chkExdt" Text=" " onchange="ItemWiseExciseDuty()"
                                                                                onclick='LqCHeck("chkExdt", "dvExdt", "txtExdt")' CssClass="bcCheckBoxList" />
                                                                        </td>
                                                                        <td width="30px">
                                                                            <div id="dvExdt" style="display: none;">
                                                                                <asp:RadioButtonList runat="server" ID="rbtnExdt" RepeatDirection="Horizontal">
                                                                                    <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                                <asp:TextBox runat="server" ID="txtExdt" MaxLength="5" onblur="extractNumber(this,2,false);"
                                                                                    onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                                                    onkeypress="return blockNonNumbers(this, event, true, false);" onchange="CalculateExDuty();"
                                                                                    Style="text-align: right;" Width="40px" CssClass="bcAsptextbox"></asp:TextBox></div>
                                                                        </td>
                                                                        <%--Modified by Satya :: For GST Implementation :: START--%>
                                                                        <td width="25px" valign="top">
                                                                            <span id="Span5" runat="server" class="bcLabel">SGST:</span>
                                                                            <asp:CheckBox runat="server" ID="chkSGST" Text=" " onchange="ItemWiseExciseDuty()"
                                                                                onclick='LqCHeck("chkSGST", "dvSGST", "txtSGST")' CssClass="bcCheckBoxList" />
                                                                        </td>
                                                                        <td width="30px">
                                                                            <div id="dvSGST" style="display: none;">
                                                                                <asp:RadioButtonList runat="server" ID="rbtnSGST" RepeatDirection="Horizontal">
                                                                                    <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                                <asp:TextBox runat="server" ID="txtSGST" MaxLength="5" onblur="extractNumber(this,2,false);"
                                                                                    onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                                                    onkeypress="return blockNonNumbers(this, event, true, false);" onchange="CalculateExDuty();"
                                                                                    Style="text-align: right;" Width="40px" CssClass="bcAsptextbox"></asp:TextBox></div>
                                                                        </td>
                                                                        <td width="25px" valign="top">
                                                                            <span id="Span6" runat="server" class="bcLabel">IGST:</span>
                                                                            <asp:CheckBox runat="server" ID="chkIGST" Text=" " onchange="ItemWiseExciseDuty()"
                                                                                onclick='LqCHeck("chkIGST", "dvIGST", "txtIGST")' CssClass="bcCheckBoxList" />
                                                                        </td>
                                                                        <td width="30px">
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
                                                                        <td width="20px" valign="top">
                                                                            <span id="lblPck" class="bcLabel">Packing:</span>
                                                                            <asp:CheckBox runat="server" ID="chkPkng" Text=" " onclick='LqCHeck("chkPkng", "dvPkng", "txtPkng")'
                                                                                CssClass="bcCheckBoxList" />
                                                                        </td>
                                                                        <td width="30px">
                                                                            <div id="dvPkng" style="display: none;">
                                                                                <asp:RadioButtonList runat="server" ID="rbtnPkng" RepeatDirection="Horizontal">
                                                                                    <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                                <asp:TextBox runat="server" ID="txtPkng" MaxLength="5" onblur="extractNumber(this,2,false);"
                                                                                    onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                                                    onkeypress="return blockNonNumbers(this, event, true, false);" onchange="CalculateExDuty();"
                                                                                    Style="text-align: right;" Width="40px" CssClass="bcAsptextbox"></asp:TextBox></div>
                                                                        </td>
                                                                        <td width="40px" valign="top">
                                                                            <span id="Span1" class="bcLabel" style="display: Block;">Additional Charges:</span>
                                                                            <asp:CheckBox runat="server" ID="chkACgs" Text=" " onclick='LqCHeck("chkACgs", "dvAdtnChgs", "txtAdtnChrgs")'
                                                                                CssClass="bcCheckBoxList" />
                                                                        </td>
                                                                        <td width="30px">
                                                                            <div id="dvAdtnChgs" style="display: none;">
                                                                                <asp:RadioButtonList runat="server" ID="rbtnAdtnChrgs" RepeatDirection="Horizontal">
                                                                                    <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                                <asp:TextBox runat="server" ID="txtAdtnChrgs" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                                                    onfocus="this.select()" onMouseUp="return false" onchange="CalculateExDuty();"
                                                                                    onkeyup="extractNumber(this,2,false);" onblur="extractNumber(this,2,false);"
                                                                                    Style="text-align: right;" Width="40px" CssClass="bcAsptextbox"></asp:TextBox></div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="6">
                                                                <table style="width: 100%; overflow: auto;">
                                                                    <tr>
                                                                        <td class="bcTdnormal">
                                                                            <span class="bcLabel">Price Basis<font color="red" size="2"><b>*</b></font>: </span>
                                                                        </td>
                                                                        <td class="bcTdnormal">
                                                                            <asp:DropDownList runat="server" ID="ddlPriceBasis" CssClass="bcAspdropdown">
                                                                                <asp:ListItem Text="- Select -" Value="0"></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                            &nbsp;&nbsp;
                                                                            <asp:TextBox runat="server" CssClass="bcAsptextboxRight" onkeypress='return SpclChars(event);'
                                                                                ID="txtPriceBasis" onkeyup="this.value=this.value.replace(/[^a-zA-Z ]/g,'');"></asp:TextBox>
                                                                        </td>
                                                                        <td class="bcTdnormal">
                                                                            <span class="bcLabel">Delivery Period<font color="red" size="2"><b>*</b></font>:</span>
                                                                        </td>
                                                                        <td class="bcTdnormal">
                                                                            <asp:TextBox runat="server" ID="txtDlvry" CssClass="bcAsptextbox" Width="23px" MaxLength="2"
                                                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this, 0, false);"
                                                                                onblur="extractNumber(this, 0, false);" onchange="chkDlvrPeriod()"></asp:TextBox>&nbsp;&nbsp;
                                                                            <font style="font-family: Verdana; font-size: 12px;">Weeks</font>
                                                                        </td>
                                                                        <td colspan="4">
                                                                            &nbsp;
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr style="background-color: Gray; font-style: normal; color: White;">
                                                            <td colspan="6">
                                                                Payment Terms :
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="6" align="center">
                                                                <table style="width: 35%; text-align: center; overflow: auto;">
                                                                    <tr>
                                                                        <td align="center">
                                                                            <div style="overflow: auto; width: 98%; text-align: center" id="divPaymentTerms"
                                                                                runat="server">
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="6" class="bcTdNewTable" align="right">
                                                                <span><a href="javascript:void(0)" id="lbtnATConditions" title="Add Addtional Terms & Conditions"
                                                                    onclick="fnOpen()" class="bcAlink">Additional Terms & Conditions </a></span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" class="bcTdNewTable">
                                                    <div style="width: 100%">
                                                        <asp:Accordion ID="UserAccordion" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                            HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                            FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                            FramesPerSecond="40" RequireOpenedPane="false">
                                                            <Panes>
                                                                <asp:AccordionPane ID="AccordionPane3" runat="server">
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
                                                                                        <asp:AsyncFileUpload ID="AsyncFileUpload1" runat="server" OnClientUploadError="uploadError"
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
                                                                </asp:AccordionPane>
                                                            </Panes>
                                                        </asp:Accordion>
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
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });
        $(document).ready(function () {
            Expnder();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }

        $(document).ready(function () {
            if ($("[id$=rbtnDsnt]").find(":checked").val() == 1) {
                $("[id$=txtDsnt]").width(100);
            }
            else {
                $("[id$=txtDsnt]").width(35);
            }
        });

        $('[id$=rbtnDsnt]').change(function () {
            if ($("[id$=rbtnDsnt]").find(":checked").val() == 1) {
                //var myW = 100;
                $("[id$=txtDsnt]").width(100);
                $('[id$=txtDsnt]').val(0);
            }
            else {
                $("[id$=txtDsnt]").width(35);
                $('[id$=txtDsnt]').val(0);
            }
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
        $('[id$=rbtnAdtnChrgs]').change(function () {
            if ($("[id$=rbtnAdtnChrgs]").find(":checked").val() == 1) {
                $("[id$=txtAdtnChrgs]").width(100);
                $('[id$=txtAdtnChrgs]').val(0);
            }
            else {
                $("[id$=txtAdtnChrgs]").width(35);
                $('[id$=txtAdtnChrgs]').val(0);
            }
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            if ($('[id$=hfLeIssueDt]').val() != '') {
                var strdateEnqDT = $('[id$=hfLeIssueDt]').val();
                var strdateEnqDT1 = strdateEnqDT.split('-');
                strdateEnqDT = (strdateEnqDT1[1] + '-' + strdateEnqDT1[0] + '-' + strdateEnqDT1[2]);
                strdateEnqDT = new Date(strdateEnqDT.replace(/-/g, "/"));
                var dateToday = new Date();
                $('[id$=txtqdt]').datepicker({
                    maxDate: dateToday,
                    minDate: strdateEnqDT,
                    dateFormat: 'dd-mm-yy',
                    changeMonth: true,
                    changeYear: true
                });
            }
        });

        $('[id$=txtDsnt]').keyup(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtDsnt]').val(); // +sv;
            var DcstChk = $("[id$=rbtnDsnt]").find(":checked").val();
            var GrndTot = $('[id$=lblGTAmt]').html();
            var GrnTot = $.trim(GrndTot);
            if (parseFloat(discount) > parseFloat(GrnTot) && DcstChk == 1) {
                ErrorMessage('Discount Exceeds Grand Total');
                $('[id$=txtDsnt]').val('0');
                return false;
            }
            if (discount > 99.99 && DcstChk == 0) {

                ErrorMessage('Discount Cannot be Grater than 99.99%');

                $('[id$=txtDsnt]').val('0');
                return false;
            }
            else {
                var numericReg = /^\d{2}(\.\d)?$/;
                if (!numericReg.test(discount)) {
                }
            }
        });
        $('[id$=txtExdt]').keyup(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtExdt]').val(); // +sv;
            var ExChk = $("[id$=rbtnExdt]").find(":checked").val();
            if (discount > 99.99 && ExChk == 0) {

                ErrorMessage('CGST Cannot be Grater than 99.99%');

                $('[id$=txtExdt]').val('0');
                return false;
            }
        });

        $('[id$=txtSGST]').keyup(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtSGST]').val(); // +sv;
            var ChkSGST = $("[id$=rbtnSGST]").find(":checked").val();
            if (discount > 99.99 && ChkSGST == 0) {

                ErrorMessage('SGST Cannot be Grater than 99.99%');

                $('[id$=txtSGST]').val('0');
                return false;
            }
        });

        $('[id$=txtIGST]').keyup(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtIGST]').val(); // +sv;
            var ChkIGST = $("[id$=rbtnIGST]").find(":checked").val();
            if (discount > 99.99 && ChkIGST == 0) {

                ErrorMessage('IGST Cannot be Grater than 99.99%');

                $('[id$=txtIGST]').val('0');
                return false;
            }
        });

        $('[id$=txtPkng]').keyup(function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            var sv = String.fromCharCode(code);
            var discount = $('[id$=txtPkng]').val(); // +sv;
            var PkgChk = $("[id$=rbtnPkng]").find(":checked").val();
            if (discount > 99.99 && PkgChk == 0) {

                ErrorMessage('Packing Cannot be Grater than 99.99%');

                $('[id$=txtPkng]').val('0');
                return false;
            }
        });


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

        function ExpandTXT(obj) {
            $('#txtSpecifications' + '' + obj).animate({ "height": "75px" }, "slow");
            $('#txtSpecifications' + '' + obj).slideDown("slow");
        }

        function ReSizeTXT(obj) {
            $('#txtSpecifications' + '' + obj).animate({ "height": "22px" }, "slow");
            $('#txtSpecifications' + '' + obj).slideDown("slow");
        }

        function CheckExDuty(obj) {
            var ExDuty = 0;
            if ($('[id$=txtPercent]').length > 0) {
                var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id"); //txtPercent
                ExDuty = $('#' + txtExDuty).val();
            }
            var ExChk = $("[id$=rbtnExdt]").find(":checked").val();
            if (ExDuty > 99.99 && ExChk == 0) {

                ErrorMessage('CGST Cannot be Grater than 99.99%');

                $('[id$=' + txtExDuty + ']').val('0');
                $('[id$=' + txtExDuty + ']').focus();
            }
            else if (ExDuty == '') {
                $('[id$=' + txtExDuty + ']').focus();
                $('[id$=' + txtExDuty + ']').val('0');
                ErrorMessage('CGST cannot be Empty.');
            }
        }

        function CheckDiscount(obj) {
            var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id"); //txtPercent
            var Discount = $('#' + txtDiscount).val();
            var DcstChk = $("[id$=rbtnDsnt]").find(":checked").val();
            if (Discount > 99.99 && DcstChk == 0) {

                ErrorMessage('Discount Cannot be Grater than 99.99%');

                $('[id$=' + txtDiscount + ']').val('0');
            }
            else if (Discount == '') {
                $('[id$=' + txtDiscount + ']').focus();
                $('[id$=' + txtDiscount + ']').val('0');
                ErrorMessage('Discount cannot be Empty.');
            }
        }

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
            //            var RwCunt = $("#tblItems > tbody > tr").length;
            //            if (DcstChk == 0 && DiscountAmt != 0) {
            //                for (var i = 1; i <= RwCunt; i++) {
            //                    //var RwDscuntVal = $('[id$=txtDiscount' + i + ']'); //.val()
            //                    //RwDscuntVal.readOnly = true;
            //                    $('[id$=txtDiscount' + i + ']').attr("disabled", "disabled")//.attr('readonly', 'readonly');
            //                }
            //            }
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
            var AdsnlCgrs = $('[id$=txtAdtnChrgs]').val();
            var AddChk = $("[id$=rbtnAdtnChrgs]").find(":checked").val();

            if (AdsnlCgrs > 99.99 && AddChk == 0) {

                ErrorMessage('Additional Charges Cannot be Grater than 99.99%');

                $('[id$=txtAdtnChrgs]').val('0');
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
            var res = NewLQuotation.CalculateExDuty(ExDutyAmt, SGSTAmt, IGSTAmt, DiscountAmt, PackingAmt, SalesTaxAmt, AdsnlCgrs, Discount, ExDuty, SGST, IGST, SalesTax,
            Packing, AdsnlChrg, DcstChk, ExChk, SGSTChk, IGSTChk, PkgChk, AddChk);
            var getDivLQItems = GetClientID("divLQItems").attr("id");
            $('#' + getDivLQItems).html(res.value);
            Expnder();
            ItemWiseDsct();
            ItemWiseExciseDuty();
        }

        function numbersonly(myfield, e, dec) {
            var key;
            var keychar;

            if (window.event)
                key = window.event.keyCode;
            else if (e)
                key = e.which;
            else
                return true;
            keychar = String.fromCharCode(key);

            if ((key == null) || (key == 0) || (key == 8) ||
            (key == 9) || (key == 13) || (key == 27))
                return true;

            else if ((("0123456789.").indexOf(keychar) > -1))
                return true;

            else if (dec && (keychar == ".")) {
                myfield.form.elements[dec].focus();
                return false;
            }
            else
                return false;
        }
        function dotplaced(myfield) {
            if (myfield.indexOf(".") === -1) { return false; } return true;
        }
    </script>
    <script type="text/javascript">

        function scroll2Row(rowID) {
            var trHeight = $('#tblItems tbody tr').eq(0).height();
            var scroll = parseInt(rowID - 2) * trHeight;
            $('#ctl00_ContentPlaceHolder1_divLQItems').scrollTop(scroll);
        }

        function FillItemGrid(obj1, obj2) {
            var ddl = GetClientID("ddl" + (parseInt(obj2) + 1)).attr("id");
            var obj3 = $('#' + ddl).val();
            var ddlCat = GetClientID("ddlCategory" + (parseInt(obj2) + 1)).attr("id");
            var obj4 = $('#' + ddlCat).val();
            var ddlUnit = GetClientID("ddlU" + (parseInt(obj2) + 1)).attr("id");
            var obj5 = $('#' + ddlUnit).val();
            var txtspec = GetClientID("txtSpecification" + (parseInt(obj2) + 1)).attr("id");
            var spec = $('#' + txtspec).val();
            var txtmake = GetClientID("txtMake" + (parseInt(obj2) + 1)).attr("id");
            var make = $('#' + txtmake).val();
            var txtqty = GetClientID("txtQty" + (parseInt(obj2) + 1)).attr("id");
            var qty = $('#' + txtqty).val();
            var lblPNo = GetClientID("lblPartNo" + (parseInt(obj2) + 1)).attr("id");
            var PNo = $('#' + lblPNo).text();
            var txtPrice = GetClientID("txtPrice" + (parseInt(obj2) + 1)).attr("id");
            var Price = $('#' + txtPrice).val(); if (Price == '') { Price = 0; }
            var lblAmount = GetClientID("lblAmount" + (parseInt(obj2) + 1)).attr("id");
            var Amount = $('#' + lblAmount).text();
            if (obj3 == null) {
                obj3 = 0;
            }
            var ddlRowsChanged = parseInt($('[id$=ddlRowsChanged]').val());
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var result = NewLQuotation.FillItemGrid(obj2, obj1, obj3, 0, 0, obj5, spec, make, qty, PNo, Price, Amount, hfCurrentPage, ddlRowsChanged);
            var getDivLQItems = GetClientID("divLQItems").attr("id");
            $('#' + getDivLQItems).html(result.value);
            Expnder();
        }

        function DeleteItem(obj1) {
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var result = NewLQuotation.DeleteItem(obj1, hfCurrentPage);
            var getDivLQItems = GetClientID("divLQItems").attr("id");
            $('#' + getDivLQItems).html(result.value);
            Expnder();
        }
        function FillItemDRP(id) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var result = NewLQuotation.FillItemDRP(id, sv, hfCurrentPage);
            var getDivLQItems = GetClientID("divLQItems").attr("id");
            $('#' + getDivLQItems).html(result.value);
            Expnder();
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
                    && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function AmountCalulate(obj2) {
            Expnder();
        }

        function DiscountEnable(obj2) {
            var resDiscount = 0;
            $('[id$=tblItems tbody tr td:nth-child(12) input[type=text]]').each(function () {
                var value1 = $(this).val();
                if (value1 != '0.00' && value1 != '0' && value1 != '') {
                    resDiscount = 1;
                    alert('Discount : ' + resDiscount);
                }
            });
            if (resDiscount == 0) {
                $('[id$=chkDsnt]').removeAttr("disabled");
            } else {
                $('[id$=chkDsnt]').attr("disabled", true);
            }
            Expnder();
        }

        function AddItemColumn(id) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var ddlUnits = GetClientID("ddlUnits" + (parseInt(id))).attr("id");
            var Units = $('#' + ddlUnits).val();
            var txtSpec = GetClientID("txtSpecifications" + (parseInt(id))).attr("id");
            var spec = $('#' + txtSpec).val();
            var txtMake = GetClientID("txtMake" + (parseInt(id))).attr("id");
            var Make = $('#' + txtMake).val();
            var txtQty = GetClientID("txtQuantity" + (parseInt(id))).attr("id");
            var Qty = $('#' + txtQty).val();
            var txtPrtNo = GetClientID("txtPrtNo" + (parseInt(id))).attr("id");
            var PrtNo = $('#' + txtPrtNo).val();
            var txtPrice = GetClientID("txtPrice" + (parseInt(id))).attr("id");
            var Price = $('#' + txtPrice).val();
            var spnAmnt = GetClientID("spnAmount" + (parseInt(id))).attr("id");
            var Amnt = $('#' + spnAmnt).text();
            var txtRmrks = GetClientID("txtRemarks" + (parseInt(id))).attr("id");
            var Rmrks = $('#' + txtRmrks).val();
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
            if ((sv == undefined || sv == '0') || (Qty == '' || Qty == '0' || Qty == '0.00') || (Price == '' || Price == '0' || print == '0.00') || (Units == '' || Units == '0' || Units == undefined)) {
                var errorMsg = '';
                var flag = 0;
                if (sv == '0') {
                    flag = 1;
                    errorMsg = 'Error With Item Description.';
                    $('#' + ddlCat).focus();
                }
                else if (Qty == '' || Qty == '0' || Qty == '0.00') {
                    flag = 1;
                    errorMsg = 'Error With Quantity.';
                    $('#' + txtQty).focus();
                }
                else if (Units == '0') {
                    flag = 1;
                    errorMsg = 'Error With Units.';
                    $("#" + ddlUnits + " option[value='0']").attr("selected", "selected");
                }
                else if (Price == '' || Price == '0' || Price == '0.00') {
                    flag = 1;
                    errorMsg = 'Error With Price.';
                    $('#' + txtPrice).focus();
                }
                if (flag == 1) {

                    ErrorMessage('' + errorMsg + '');

                }
                else {
                    if (sv == undefined)
                        sv = '';
                    if (PrtNo == undefined)
                        PrtNo = '';
                    if (Units == undefined)
                        Units = '';
                    var result = NewLQuotation.AddNewRow(id, sv, PrtNo, spec, Make, Qty, Units, Price, Amnt, Rmrks, hfCurrentPage, ddlRowsChanged);
                    var getDivLQItems = GetClientID("divLQItems").attr("id");
                    $('#' + getDivLQItems).html(result.value);
                }
            }
            else if (sv != undefined && sv == 0) {

                ErrorMessage('Select Item Description.');

            }
            else {
                if (sv == undefined)
                    sv = '';
                if (PrtNo == undefined)
                    PrtNo = '';
                if (Units == undefined)
                    Units = '';
                var result = NewLQuotation.AddNewRow(id, sv, PrtNo, spec, Make, Qty, Units, Price, Amnt, Rmrks, hfCurrentPage, ddlRowsChanged);
                var getDivLQItems = GetClientID("divLQItems").attr("id");
                $('#' + getDivLQItems).html(result.value);
            }
            Expnder();
        }

        function FillItemsAll00(obj) {
            var OverAllExiseDuty = $('[id$=txtExdt]').val().trim();
            var OverAllDiscount = $('[id$=txtDsnt]').val().trim();
            if (OverAllExiseDuty != '0' && OverAllExiseDuty != '0.00' && OverAllExiseDuty != '' && OverAllExiseDuty != '.') {
            }
        }

        function fnSetValues1() {
            var iHeight = 300;
            var iWidth = 1000;
            var sFeatures = "dialogHeight: " + iHeight + "px; dialogWidth: " + iWidth + "px;";
            return sFeatures;
        }

        var returnVal = "";
        var flag = 0;
        function fnOpenItems(id, rowIndex) {
            returnVal = window.showModalDialog("../Enquiries/AddItems.aspx", "Add Item", "dialogHeight:680px; dialogWidth:940px; dialogLeft:150; dialogright:150; dialogTop:150; ");
            if (returnVal != null) {
                flag = 1;
                FillItemsAll(id);
                returnVal = "";
            }
            else
                returnVal = "";
        }

        function FillItemsAll(obj) {
            var ddl = GetClientID("ddl" + (parseInt(obj))).attr("id");
            var obj3 = (ddl == undefined ? '' : $('#' + ddl).val());
            var des = GetClientID("lbldescip" + (parseInt(obj))).attr("id");
            var descrip = $('#' + des).text();
            var ddlCat = GetClientID("ddlCategory" + (parseInt(obj))).attr("id");
            var obj4 = $('#' + ddlCat).val();
            var txtspec = GetClientID("txtSpecifications" + (parseInt(obj))).attr("id");
            var spec = $('#' + txtspec).val();
            var txtmake = GetClientID("txtMake" + (parseInt(obj))).attr("id");
            var make = $('#' + txtmake).val();
            var txtqty = GetClientID("txtQuantity" + (parseInt(obj))).attr("id");
            var qty = $('#' + txtqty).val();
            var ddlU = GetClientID("ddlUnits" + (parseInt(obj))).attr("id");
            var UnitID = $('#' + ddlU).val(); if (UnitID == undefined) { UnitID = 0; }
            var txtPrice = GetClientID("txtPrice" + (parseInt(obj))).attr("id");
            var Price = $('#' + txtPrice).val(); if (Price == '') { Price = 0.00; }
            var lblAmount = GetClientID("spnAmount" + (parseInt(obj))).attr("id");
            var Amount = $('#' + lblAmount).text(); if (Amount == '') { Amount = 0.00; }
            var txtRmrks = GetClientID("txtRemarks" + (parseInt(obj))).attr("id");
            var Rmrks = $('#' + txtRmrks).val();
            var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
            var Discount = $('#' + txtDiscount).val();
            var ExDuty = 0;
            if ($('[id$=txtPercent]').length > 0) {
                var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
                ExDuty = (txtExDuty == undefined ? "0" : $('#' + txtExDuty).val()); if (txtExDuty == '') { txtExDuty = 0.00; ExDuty = 0; }
            }
            var ChkChaild = GetClientID("ckhChaild" + (parseInt(obj))).attr("id");
            var Chaild = $('#' + ChkChaild).is(':checked');

            var CHKExDuty = 0; var CHKDiscount = 0; var CHKSalesTax = 0; var CHKPacking = 0; var CHKAdChrgs = 0;

            //            var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
            //            var ExDuty = (txtSGST == undefined ? "0" : $('#' + txtSGST).val()); if (txtSGST == '') { txtSGST = 0.00; ExDuty = 0; }



            var RwCunt = $("#tblItems > tbody > tr").length;
            for (var i = 1; i <= RwCunt; i++) {
                var RwDscuntVal = $('[id$=txtDiscount' + i + ']').val();
                if (RwDscuntVal != 0) {
                    $('[id$=rbtnDsnt]').find("input[value='0']").attr("checked", "checked");
                    $('[id$=chkDsnt]').attr("disabled", true);
                    $('[id$=chkDsnt]').attr('checked', false);
                    $('[id$=txtDsnt]').val("0");
                    $('[id$=dvDsnt]').hide();
                }
            }

            var txtExdt = $('[id$=txtExdt]').val().trim();
            var txtSGST = $('[id$=txtSGST]').val().trim();
            var txtIGST = $('[id$=txtIGST]').val().trim();

            var txtDsnt = $('[id$=txtDsnt]').val().trim();
            var txtSlsTax = 0;
            var txtPkng = $('[id$=txtPkng]').val().trim();
            var txtAdchrgs = $('[id$=txtAdtnChrgs]').val().trim();

            if ($('[id$=chkExdt]').is(':checked') && txtExdt != '') {
                CHKExDuty = txtExdt;
            }

            if ($('[id$=chkDsnt]').is(':checked') && txtDsnt != '') {
                CHKDiscount = txtDsnt;
            }
            if ($('[id$=chkSltx]').is(':checked') && txtSlsTax != '') {
                CHKSalesTax = txtSlsTax;
            }
            if ($('[id$=chkPkng]').is(':checked') && txtPkng != '') {
                CHKPacking = txtPkng;
            }
            if ($('[id$=chkACgs]').is(':checked') && txtAdchrgs != '') {
                CHKAdChrgs = txtAdchrgs;
            }
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

            if (returnVal != '')
                obj3 = returnVal;
            if (obj3 == null || descrip == null)
                obj3 = 0;
            if (obj3 != '0' && descrip != '') {
                if (Discount == '100' || Discount == '100.00') {
                    if (confirm("Are you sure you want to Give " + Discount + "% Discount ?")) {
                        var result = NewLQuotation.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                        var getDivLQItems = GetClientID("divLQItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                    }
                    else {
                        alert(obj3 + ',  ' + descrip + ' Discount1 : ' + Discount);
                        var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
                        $('#' + txtDiscount).focus();
                        $('#' + txtDiscount).val('0.00');
                    }
                }
                else {
                    var result = NewLQuotation.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                    var getDivLQItems = GetClientID("divLQItems").attr("id");
                    $('#' + getDivLQItems).html(result.value);
                }
            }
            else if (obj3 == '0' && descrip != '') {
                if (Discount == '100' || Discount == '100.00') {
                    if (confirm("Are you sure you want to Give " + Discount + "% Discount ?")) {
                        var result = NewLQuotation.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                        var getDivLQItems = GetClientID("divLQItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                    }
                    else {
                        var result = NewLQuotation.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                        var getDivLQItems = GetClientID("divLQItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                    }
                }
                else {
                    if ((Discount != '' && Discount != '.') && (Price != '' && Price != '.') && (ExDuty != '' && ExDuty != '.') && (qty != '' && qty != '.')) {

                        var result = NewLQuotation.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                        var getDivLQItems = GetClientID("divLQItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                        if (parseFloat(ExDuty) != 0) {
                            CHKExDuty = 0.00;
                            $('[id$=chkExdt]').attr("disabled", true);
                            $('[id$=chkExdt]').attr('checked', false);
                            $('[id$=txtExdt]').val("0");
                            $('[id$=dvExdt]').hide();

                            $('[id$=chkSGST]').attr("disabled", true);
                            $('[id$=chkSGST]').attr('checked', false);
                            $('[id$=txtSGST]').val("0");
                            $('[id$=dvSGST]').hide();

                            $('[id$=chkIGST]').attr("disabled", true);
                            $('[id$=chkIGST]').attr('checked', false);
                            $('[id$=txtIGST]').val("0");
                            $('[id$=dvIGST]').hide();
                        }
                        CalculateExDuty();
                    }
                    else {
                        if (Discount == '' || Discount == '.') {
                            var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
                            $('#' + txtDiscount).val('0.00');
                            $('#' + txtDiscount).focus();

                            ErrorMessage('Discount Cannot Be Empty.');

                        }
                        else if (Price == '' || Price == '.') {
                            var txtPrice = GetClientID("txtPrice" + (parseInt(obj))).attr("id");
                            $('#' + txtPrice).val('0.00');
                            $('#' + txtPrice).focus();

                            ErrorMessage('Price Cannot Be Empty.');

                        }
                        else if (ExDuty == '' || ExDuty == '.') {
                            if ($('[id$=txtPercent]').length > 0) {
                                var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
                                $('#' + txtExDuty).val('0.00');
                                $('[id$=' + txtExDuty + ']').focus();
                                ErrorMessage('ExDutyPercentage Cannot Be Empty.');
                            }
                        }
                        else if (qty == '' || qty == '.') {
                            var txtqty = GetClientID("txtQuantity" + (parseInt(obj))).attr("id");
                            $('#' + txtqty).val('0');
                            $('#' + txtqty).focus();

                            ErrorMessage('Quantity Cannot Be Empty.');

                        }
                    }
                }
            }
            else {
                if (flag == 1) {
                    var result = NewLQuotation.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                    var getDivLQItems = GetClientID("divLQItems").attr("id");
                    $('#' + getDivLQItems).html(result.value);
                }
                else {
                    var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
                    $('#' + txtDiscount).val('0.00');

                    ErrorMessage('Item Descreption is Required.');

                }
            }

            if ($('[id$=chkExdt]').is(':checked') || $('[id$=chkDsnt]').is(':checked') || $('[id$=chkSGST]').is(':checked') || $('[id$=chkIGST]').is(':checked')) {
                CalculateExDuty();
            }
            var FlagExDuty = $('[id$=HFExDuty]').val();
            if (FlagExDuty == 0) {
                $('[id$=chkExdt]').removeAttr("disabled");
                $('[id$=chkSGST]').removeAttr("disabled");
                $('[id$=chkIGST]').removeAttr("disabled");
            } else {
                $('[id$=chkExdt]').attr("disabled", true);
                $('[id$=chkExdt]').attr('checked', false);
                $('[id$=txtExdt]').val("0");
                $('[id$=dvExdt]').hide();

                $('[id$=chkSGST]').attr("disabled", true);
                $('[id$=chkSGST]').attr('checked', false);
                $('[id$=txtSGST]').val("0");
                $('[id$=dvSGST]').hide();

                $('[id$=chkIGST]').attr("disabled", true);
                $('[id$=chkIGST]').attr('checked', false);
                $('[id$=txtIGST]').val("0");
                $('[id$=dvIGST]').hide();
            }
            var FlagDiscount = $('[id$=HFDiscount]').val();
            if (FlagDiscount == 0) {
                $('[id$=chkDsnt]').removeAttr("disabled");
            } else {
                $('[id$=rbtnDsnt]').find("input[value='0']").attr("checked", "checked");
                $('[id$=chkDsnt]').attr("disabled", true);
                $('[id$=chkDsnt]').attr('checked', false);
                $('[id$=txtDsnt]').val("0");
                $('[id$=dvDsnt]').hide();
            }
            Expnder();
            scroll2Row(obj);
        }

        function doConfirm(id) {
            if (confirm("Are you sure you want to Continue?")) {
                var hfCurrentPage = $('[id$=hfCurrentPage]').val();
                var result = NewLQuotation.DeleteItem(id, hfCurrentPage);
                var getDivLQItems = GetClientID("divLQItems").attr("id");
                $('#' + getDivLQItems).html(result.value);
                Expnder();
            }
            else {
                return false;
            }
        }

        function doConfirmPayment(id) {
            if (confirm("Are you sure you want to Delete Payment?")) {
                var result = NewLQuotation.PaymentDeleteItem(id);
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
                var result = NewLQuotation.PaymentAddItem(RNo, PercAmt, Desc);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);
                if ($('[id$=HfMessage]').val() != '') {
                    ErrorMessage($('[id$=HfMessage]').val());
                    $('[id$=' + txtPercAmt + ']').focus();
                }
                else {
                    $('[id$=' + txtDesc + ']').focus();
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
            if (Currentweeks == 0) {
                ErrorMessage('Delivery Period should not be Zero');
                $('[id$=txtDlvry]').focus(0, 0);
            }
        }

        function uploadComplete() {
            var result = NewLQuotation.AddItemListBox();
            var getDivLQItems = GetClientID("divListBox").attr("id");
            $('#' + getDivLQItems).html(result.value);
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
                $get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If You need Upload New File.";
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
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload Failed.";
        }
        function uploadStarted() {
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload started.";
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = NewLQuotation.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
            Expnder();
        });


        function Myvalidations() {
            try {
                var IsExciseDuty = false;
                var PaymentRCount = $('#tblPaymentTerms tbody tr').length;
                var atxtDlvry = $('[id$=txtDlvry]').val();
                var TotalAmt = 0;
                //$(".RA").hide();
                //                var TRid = $(this).attr('id');
                //                $(this).attr("src", "../images/expand.png");
                //                //$(".DEL" + TRid).remove();
                //                $("[id$=btnExpand" + TRid + "]").prop('title', 'Expand');

                var rowCount = $('#tblItems tbody tr').length;
                if (($('[id$=ddlcustomer]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Customer is Required.');
                    $('[id$=ddlcustomer]').focus();
                    return false;
                }
                else if (($('[id$=ddlfenq]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Foreign Enquiry Number is Required.');
                    $('[id$=ddlfenq]').focus();
                    return false;
                }
                else if (($('[id$=ddllenq]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Local Enquiry Number is Required.');
                    $('[id$=ddllenq]').focus();
                    return false;
                }
                else if (($('[id$=txtlquotno]').val()).trim() == '') {

                    ErrorMessage('Local Quotation Number is Required.');
                    $('[id$=txtlquotno]').focus();

                    return false;
                }
                else if (($('[id$=txtdt]').val()).trim() == '') {

                    ErrorMessage('Local Quotation Date is Required.');
                    $('[id$=txtdt]').focus();

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
                        $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                        return false;
                    }
                }
                if ($('[id$=tblItems]')[0].rows.length > '0') {
                    var tbl = $('[id$=tblItems]')[0];
                    var rowCount = $('[id$=tblItems]')[0].rows.length; var selectone = 0;
                    //                    if (rowCount > 1) {
                    //                        for (var i = 0; i <= rowCount - 4; i++) {
                    //                            if (i == 0) {

                    //                            }
                    //                            else {
                    //                                if ($('[id$=txtPercent' + i + ']').val() != undefined)
                    //                                    if ($('[id$=txtPercent' + i + ']').val().trim() != 0) {
                    //                                        IsExciseDuty = true;
                    //                                    }
                    //                                if ($('[id$=ckhChaild' + i + ']')[0].checked) {
                    //                                    if ($('[id$=txtQuantity' + i + ']').val().trim() == '' || $('[id$=txtQuantity' + i + ']').val().trim() == 0) {

                    //                                        ErrorMessage('Item ' + i + ' Quantity is Required.');
                    //                                        $('[id$=txtQuantity' + i + ']').focus();

                    //                                        return false;
                    //                                    }
                    //                                    else {
                    //                                        if ($('[id$=txtPrice' + i + ']').val().trim() == '' || $('[id$=txtPrice' + i + ']').val().trim() == 0.00 || $('[id$=txtPrice' + i + ']').val().trim() == 0) {

                    //                                            ErrorMessage('Item ' + i + ' Price is Required.');
                    //                                            $('[id$=txtPrice' + i + ']').focus();

                    //                                            return false;
                    //                                        }
                    //                                        else
                    //                                            selectone = 1;
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                        if (selectone == 0) {

                    //                            ErrorMessage('Select at Least one Item.');
                    //                            $('[id$=tblItems]').focus();

                    //                            return false;
                    //                        }
                    //                    }
                    //                    else {
                    //                        ErrorMessage('No Items to Save.');
                    //                        $('[id$=tblItems]').focus();
                    //                        return false;
                    //                    }
                    var result = NewLQuotation.ValidateRowsBeforeSave();
                    if (result.value.includes("ERROR::")) {
                        ErrorMessage(result.value.replace("ERROR::", ""));
                        return false;
                    }
                    if ($('[id$=chkDsnt]')[0].checked == true) {
                        if (($('[id$=txtDsnt]').val()).trim() == '0') {
                            ErrorMessage('Discount Amount is Required.');
                            $('[id$=txtDsnt]').focus();
                            return false;
                        }
                    }
                    if ($('[id$=chkExdt]')[0] != undefined) {
                        if ($('[id$=chkExdt]')[0].checked == true) {
                            if (($('[id$=txtExdt]').val()).trim() == '0') {
                                ErrorMessage('CGST is Required.');
                                $('[id$=txtExdt]').focus();
                                return false;
                            }
                            else {
                                IsExciseDuty = true;
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
                    if ($('[id$=chkPkng]')[0].checked == true) {
                        if (($('[id$=txtPkng]').val()).trim() == '0') {
                            ErrorMessage('Packing is Required.');
                            $('[id$=txtPkng]').focus();
                            return false;
                        }
                    }
                    if ($('[id$=chkACgs]')[0].checked == true) {
                        if (($('[id$=txtAdtnChrgs]').val()).trim() == '0') {
                            ErrorMessage('Additional Charges is Required.');
                            $('[id$=txtAdtnChrgs]').focus();
                            return false;
                        }
                    }
                    var tbl = $('[id$=tblItems]')[0];
                }
                if ($('[id$=tblItems]')[0].rows.length == '0') {
                    ErrorMessage('No Items to Save.');
                    $('[id$=tblItems]').focus();
                    return false;
                }
                else if (($('[id$=ddlPriceBasis]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Price Basis is Required.');
                    $('[id$=ddlPriceBasis]').focus();
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
                else if (atxtDlvry == '0') {
                    ErrorMessage('Delivery Period should not be Zero.');
                    $('[id$=txtDlvry]').focus();
                    return false;
                }
                else if (PaymentRCount > 0) {
                    for (var k = 1; k <= PaymentRCount; k++) {
                        var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(k))).attr("id");
                        var PercAmt = $('#' + txtPercAmt).val();
                        var txtDesc = GetClientID("txtDesc" + (parseInt(k))).attr("id");
                        var Desc = $('#' + txtDesc).val();
                        TotalAmt = Number(TotalAmt) + Number(PercAmt);
                        if (PercAmt == '' || PercAmt == '0' || Desc == '') {
                            var message = '';
                            if (PercAmt == '')
                                message = 'Payment Percentage Is Required';
                            else if (PercAmt == '0')
                                message = 'Payment Percentage Cannot be Zero';
                            else if (Desc == '')
                                message = 'Description Is Required';

                            ErrorMessage('' + message + ' of SNo : ' + k + '.');

                            $('[id$=' + txtPercAmt + ']').focus();
                            return false;
                            break;
                        }
                    }
                    if (PaymentRCount == 0 || TotalAmt < 100) {
                        ErrorMessage('Total Payment percentage should be 100.');
                        return false;
                    }
                    else if (TotalAmt < 100) {
                        ErrorMessage('Payment has to be 100%.');
                        return false;
                    }
                    //                    else {
                    //                        if (IsExciseDuty) {
                    //                            return confirm('You have added Excise Duty* in Quotation. \n Are you sure to continue with ?');
                    //                        }
                    //                    }
                }
                else if (PaymentRCount <= 0) {
                    ErrorMessage('Payment has to be 100%.');
                    return false;
                }

                var result = NewLQuotation.Validate_SubItem_BeforeSaving();
                if (result.value != "") {
                    ErrorMessage(result.value);
                    return false;
                }

            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
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
                var getDivFEItems = GetClientID("divLQItems").attr("id");

                $('div.test').block({
                    message: '<h1>Processing</h1>',
                    css: { border: '3px solid #a00' }
                });

                //                $.blockUI({ message: 'Just a moment...</h1>',
                //                    fadeOut: 20
                //                });

                var result = NewLQuotation.NextPage(hfCurrentPage, ddlRowsChanged);
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
                var result = NewLQuotation.PrevPage(hfCurrentPage, ddlRowsChanged);
                var getDivFEItems = GetClientID("divLQItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                Expnder();
            }
        };

        function RowsChanged() {
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var ddlRowsChanged = $('[id$=ddlRowsChanged]').val();
            var result = NewLQuotation.RowsChanged(hfCurrentPage, ddlRowsChanged);
            var getDivFEItems = GetClientID("divLQItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        //Paging END


        function fnSetValues() {
            var iHeight = 500;
            var iWidth = 1000;
            var sFeatures = "dialogHeight: " + iHeight + "px; dialogWidth: " + iWidth + "px;";
            return sFeatures;
        }
        function fnOpen(id, rowIndex) {
            var sFeatures = fnSetValues();
            var retVals = { id1: 508, id2: 513 };
            var w = 1000;
            var h = 550;
            var x = (screen.width / 2) - (w / 2);
            var y = (screen.height / 2) - (h / 2);
            var ED = $('[id$=HFID]').val();
            if (ED == '00000000-0000-0000-0000-000000000000') {
                window.open("../Masters/TermsNConditions.aspx?TAr=LQ/LPO&LclQuteID=" + ED + "&LQ=true", "508", "width=1000,height=500,toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, top=" + y + ", left=" + x + "");
            }
            else {
                window.open("../Masters/TermsNConditions.aspx?TAr=LQ/LPO&LclQuteID=" + ED + "&LQ=false", "508", "width=1000,height=500,toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, top=" + y + ", left=" + x + "");
            }
        }
        
    </script>
    <script type="text/javascript">

        function isNumberKey1(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if ((charCode > 31 || charCode < 46) && charCode == 47 && (charCode < 48 || charCode > 57))
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

        function LqCHeck(ckid, dvid, txtBox) {
            var ChkBox = document.getElementById("ctl00_ContentPlaceHolder1_" + ckid);
            var txtval = document.getElementById("ctl00_ContentPlaceHolder1_" + txtBox);
            if (ChkBox.checked == true) {
                document.getElementById(dvid).style.display = 'block';
            }
            else {
                document.getElementById(dvid).style.display = 'none';
                document.getElementById(txtval.id).value = "0";
                CalculateExDuty();
            }
        }

        function QtyCheck(pqty, qty) {
            var qtyAppnd = pqty + "a" + qty;
            var CQty = 0, TcQty = 0;
            var PQty = $('#txtPrice' + pqty).val()
            var ClsCount = $('.DEL' + pqty).length;
            for (i = 1; i <= ClsCount; i++) {
                if (parseFloat($('#txtRate' + qtyAppnd).val()) != 0) {
                    CQty = parseFloat(CQty) + parseFloat(($('#txtRate' + pqty + "a" + i).val()));
                }
            }
            if (CQty > PQty) {
                confirm("Sub-Item Value is more than Parent Item");

            }
        }
    </script>
    <script type="text/javascript">
        function CheckEnquiryNo() {
            var enqNo = $('[id$=txtlquotno]').val();
            var result = NewLQuotation.CheckEnquiryNo(enqNo);
            if (result.value == false) {
                $("#<%=txtlquotno.ClientID%>")[0].value = '';
                ErrorMessage('Quotation Number exists.');
                $("#<%=txtlquotno.ClientID%>")[0].focus();
                return false;
            }
        }
    </script>
    <script type="text/jscript">
        $('[src*=expand]').live('click', function (e) {
            e.preventDefault();
            var TRid = $(this).attr('id');
            var ItemID = $("[id$=HItmID" + TRid + "]").val();
            if ($('[id$=ckhChaild' + TRid + ']').is(':checked')) {
                var NewTbl = NewLQuotation.Expand_LPOs(ItemID, TRid);
                $(this).closest("tr").after(NewTbl.value);
                $(this).attr("src", "../images/collapse.png");
                $("[id$=btnExpand" + TRid + "]").prop('title', 'Collapse');
            }
            else {
                ErrorMessage('To expand you need to check this item.');
            }
        });

        $('[src*=collapse]').live('click', function (e) {
            e.preventDefault();
            var TRid = $(this).attr('id');
            $(this).attr("src", "../images/expand.png");
            $(".DEL" + TRid).remove();
            $("[id$=btnExpand" + TRid + "]").prop('title', 'Expand');
        });

        function fnOpen1(TRID, RowID, ClickRowID) {
            var returnVall = window.showModalDialog("../Enquiries/AddItems.aspx", "Add Item",
            "dialogHeight:680px; dialogWidth:980px; dialogLeft:150; dialogright:150; dialogTop:150; ");
            if (returnVall != null) {
                var rtnVal = returnVall.split(',');
                if (rtnVal[1].trim() == "") {
                    $('#hfSubItemID' + ClickRowID).val(rtnVal[0]);
                    GetItemDesc_Spec(rtnVal[0], ClickRowID);
                }
                else {
                    $('#hfSubItemID' + ClickRowID).val(rtnVal[0]);
                    GetItemDesc_Spec(rtnVal[0], ClickRowID);
                }
            }
            else
                returnVall = "";
            return returnVall;
        }

        function GetItemDesc_Spec(ItmID, id) {
            var rslt = NewLQuotation.GetItemDesc_Spec(ItmID, id);
            var aray = rslt.value.split('^~^,');
            $("[id$=lblItemDesc" + id + "]").text(aray[0]);
            $("[id$=lblPartNo" + id + "]").text(aray[1]);
            $("[id$=txtDesc-Spec" + id + "]").val(aray[2]);
        }

        $(".fnOpen").live('click', function () {
            var ClickRID = $(this).closest('tr').attr('id');
            var aray = ClickRID.split('a');
            var RowIndex = $(this).closest('td').parent()[0].sectionRowIndex;
            var res1 = fnOpen1(aray[0], aray[1], ClickRID);
            if (res1 != "")
                Add_Sub_Itms1(aray[0], aray[1], ClickRID, RowIndex, false);
        });

        $(".addrow").live('click', function () {
            var RowIndex = $(this).closest('td').parent()[0].sectionRowIndex;
            var ClickRID = $(this).closest('tr').attr('id');
            var aray = ClickRID.split('a');
            Add_Sub_Itms1(aray[0], aray[1], ClickRID, RowIndex, true);
        });

        function savechanges1(TrID, SNo, x) {
            var ClickRID = TrID + 'a' + SNo;
            var RowIndex = $(x).closest('td').parent()[0].rowIndex - 1;
            Add_Sub_Itms1(TrID, SNo, ClickRID, RowIndex, false);
        }

        function Add_Sub_Itms1(TrID, RowID, ClickRID, RowIndex, IsAdd) {
            var ParentItemID = $('#HItmID' + TrID).val();
            var SubRowID = $('#lblSubSNo' + ClickRID).text();
            var ItemID = $('#hfSubItemID' + ClickRID).val();
            var ItmDesc = $('#lblItemDesc' + ClickRID).text();
            var Spec = $("[id$=txtDesc-Spec" + ClickRID + "]").val();
            var PNo = $("[id$=lblPartNo" + ClickRID + "]").text();
            var Make = $("[id$=txtMake" + ClickRID + "]").val();
            var Qty = $('#txtQuantity' + ClickRID).val();
            var Rate = $('#txtRate' + ClickRID).val();
            var Disc = $('#txtDscnt' + ClickRID).val();
            var exduty = $('#txtExDuty' + ClickRID).val();
            var UnitID = $('#ddlUnits' + ClickRID).val();
            var Remarks = $('#txtRemarks' + ClickRID).val();
            var DiscPercent = $('[id$=txtDsnt]').val();
            var PkngPercent = $('[id$=txtPkng]').val();
            var ExDutyPercent = $('[id$=txtExDuty' + ClickRID + ']').val();
            var DscntAll = $('[id$=txtDsnt]').val();
            var PkngAll = $('[id$=txtPkng]').val();
            var ExseAll = $('[id$=txtExdt]').val();
            var AddChrgsAll = $('[id$=txtAdtnChrgs]').val();
            var ChkDsnt = $('[id$=chkDsnt]').is(':checked');
            var ChkPkng = $('[id$=chkPkng]').is(':checked');
            var ChkExse = $('[id$=chkExdt]').is(':checked');
            var ChkAddChrgs = $('[id$=chkACgs]').is(':checked');

            if (DscntAll == '' || PkngAll == '' || ExseAll == '' || AddChrgsAll == '' || ChkDsnt || ChkPkng || ChkExse || ChkAddChrgs) {
                if (ChkDsnt) {
                    if (DscntAll == '') { DscntAll = 0; }
                }
                else { DscntAll = 0; }

                if (ChkPkng) {
                    if (PkngAll == '') { PkngAll = 0; }
                }
                else { PkngAll = 0; }

                if (ChkExse) {
                    if (ExseAll == '') { ExseAll = 0; }
                }
                else { ExseAll = 0; }

                if (ChkAddChrgs) {
                    if (AddChrgsAll == '') { AddChrgsAll = 0; }
                }
                else { AddChrgsAll = 0; }
            }

            if (IsAdd != true)
                IsAdd = false;
            if (IsAdd == true) {
                if (ItemID.trim() == "" || ItemID.trim() == 0) {
                    ErrorMessage('Select Item to add new row.');
                    return false;
                }
                else if (ItmDesc.trim() == "") {
                    ErrorMessage('Item Description is required.');
                    return false;
                }

                else if (Qty.trim() == "" || Qty.trim() == 0) {
                    ErrorMessage('Item Quantity cannot be empty or zero.');
                    return false;
                }
                else if (Rate.trim() == "" || Rate.trim() == 0) {
                    ErrorMessage('Item Rate cannot be empty or zero.');
                    return false;
                }
                else if (Disc.trim() == "") {
                    ErrorMessage('Item Discount cannot be empty.');
                    return false;
                }
                else if (exduty.trim() == "") {
                    ErrorMessage('Item Ex-Duty cannot be empty.');
                    return false;
                }
                else if (UnitID.trim() == "" || UnitID.trim() == 0) {
                    ErrorMessage('Item Unit is required.');
                    return false;
                }
                else {
                    var res;
                    res = NewLQuotation.Add_Sub_Itms(RowID, TrID, SubRowID, ParentItemID, ItemID, ItmDesc, Spec, PNo, Make, Qty, Rate, Disc, exduty, UnitID, Remarks,
                    DiscPercent, PkngPercent, ExDutyPercent, DscntAll, PkngAll, ExseAll, AddChrgsAll, ChkDsnt, ChkPkng, ChkExse, ChkAddChrgs, IsAdd);

                    $(".DEL" + TrID).remove();
                    $('#tblItems > tbody > tr').eq((RowIndex - RowID)).after(res.value);
                }
            }
            else {
                var res;
                res = NewLQuotation.Add_Sub_Itms(RowID, TrID, SubRowID, ParentItemID, ItemID, ItmDesc, Spec, PNo, Make, Qty, Rate, Disc, exduty, UnitID, Remarks,
                    DiscPercent, PkngPercent, ExDutyPercent, DscntAll, PkngAll, ExseAll, AddChrgsAll, ChkDsnt, ChkPkng, ChkExse, ChkAddChrgs, IsAdd);

                $(".DEL" + TrID).remove();
                $('#tblItems > tbody > tr').eq((RowIndex - RowID)).after(res.value);
            }

            $('[id$=lblTotalAmt]').text($('[id$=HFTotalAmt_S]').val());
            $('[id$=lblGTAmt]').text($('[id$=HFGTAmt_S]').val());

            var FlagExDuty_S = $('[id$=HFExDuty_S]').val();
            var FlagExDuty = $('[id$=HFExDuty]').val();
            if (FlagExDuty_S == 0 && FlagExDuty == 0) {
                $('[id$=chkExdt]').removeAttr("disabled");
                $('[id$=chkSGST]').removeAttr("disabled");
                $('[id$=chkIGST]').removeAttr("disabled");
            } else {
                $('[id$=chkExdt]').attr("disabled", true);
                $('[id$=chkExdt]').attr('checked', false);
                $('[id$=txtExdt]').val("0");
                $('[id$=dvExdt]').hide();

                $('[id$=chkSGST]').attr("disabled", true);
                $('[id$=chkSGST]').attr('checked', false);
                $('[id$=txtSGST]').val("0");
                $('[id$=dvSGST]').hide();

                $('[id$=chkIGST]').attr("disabled", true);
                $('[id$=chkIGST]').attr('checked', false);
                $('[id$=txtIGST]').val("0");
                $('[id$=dvIGST]').hide();
            }

            var FlagDiscount_S = $('[id$=HFDiscount_S]').val();
            var FlagDiscount = $('[id$=HFDiscount]').val();
            if (FlagDiscount_S == 0 && FlagDiscount == 0) {
                $('[id$=chkDsnt]').removeAttr("disabled");
            } else {
                $('[id$=rbtnDsnt]').find("input[value='0']").attr("checked", "checked");
                $('[id$=chkDsnt]').attr("disabled", true);
                $('[id$=chkDsnt]').attr('checked', false);
                $('[id$=txtDsnt]').val("0");
                $('[id$=dvDsnt]').hide();
            }

        }

        function Delete_SubItem(TrID, SNo, x) {
            if (confirm("Are you sure you want to Delete?")) {
                var ClickRID = $(x).closest('tr').attr('id');
                var RowIndex = $(x).closest('td').parent()[0].rowIndex - 1;
                var ParentItemID = $('#HItmID' + TrID).val();
                var ItemID = $('#hfSubItemID' + ClickRID).val();
                var res = NewLQuotation.Delete_SubItem(ParentItemID, TrID, SNo, ItemID);
                $(".DEL" + TrID).remove();
                $('#tblItems > tbody > tr').eq((RowIndex - SNo)).after(res.value);
            }
        }



        function CalculateTotalAmount(obj) {

            try {

                var ddl = GetClientID("ddl" + (parseInt(obj))).attr("id");
                var obj3 = $('#' + ddl).val();
                var des = GetClientID("lbldescip" + (parseInt(obj))).attr("id");
                var descrip = $('#' + des).text();
                var ddlCat = GetClientID("ddlCategory" + (parseInt(obj))).attr("id");
                var obj4 = $('#' + ddlCat).val();
                var txtspec = GetClientID("txtSpecifications" + (parseInt(obj))).attr("id");
                var spec = $('#' + txtspec).val();
                var txtmake = GetClientID("txtMake" + (parseInt(obj))).attr("id");
                var make = $('#' + txtmake).val();
                var txtqty = GetClientID("txtQuantity" + (parseInt(obj))).attr("id");
                var qty = $('#' + txtqty).val();
                var ddlU = GetClientID("ddlUnits" + (parseInt(obj))).attr("id");
                var UnitID = $('#' + ddlU).val(); if (UnitID == undefined) { UnitID = 0; }
                var txtPrice = GetClientID("txtPrice" + (parseInt(obj))).attr("id");
                var Price = $('#' + txtPrice).val(); if (Price == '') { Price = 0.00; }
                var lblNetRate = GetClientID("spnNetRate" + (parseInt(obj))).attr("id");
                var NetRate = $('#' + lblAmount).text(); if (NetRate == '') { NetRate = 0.00; }
                var lblAmount = GetClientID("spnAmount" + (parseInt(obj))).attr("id");
                var Amount = $('#' + lblAmount).text(); if (Amount == '') { Amount = 0.00; }
                var txtRmrks = GetClientID("txtRemarks" + (parseInt(obj))).attr("id");
                var Rmrks = $('#' + txtRmrks).val();
                var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
                var Discount = $('#' + txtDiscount).val();
                var ExDuty = 0;
                if ($('[id$=txtPercent]').length > 0) {
                    var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
                    ExDuty = (txtExDuty == undefined ? "0" : $('#' + txtExDuty).val());
                }

                var lblGrandTotal = GetClientID("spnGrandTotal" + (parseInt(obj))).attr("id");
                var GrandTotal = $('#' + lblGrandTotal).text(); if (GrandTotal == '') { GrandTotal = 0.00; }
                var ChkChaild = GetClientID("ckhChaild" + (parseInt(obj))).attr("id");
                var Chaild = $('#' + ChkChaild).is(':checked');

                var lblTotalAmt = GetClientID("lblTotalAmt").attr("id");
                var TotalAmount = $('#' + lblTotalAmt).text(); if (TotalAmount == '') { TotalAmount = 0.00; }
                var lblGrandAmount = GetClientID("lblGTAmt").attr("id");
                var GrandAmount = $('#' + lblGrandAmount).text(); if (GrandAmount == '') { GrandAmount = 0.00; }

                var CHKExDuty = 0, CHKDiscount = 0, CHKSalesTax = 0, CHKPacking = 0, CHKAdChrgs = 0, txtSlsTax = 0;

                var txtExdt = $('[id$=txtExdt]').val().trim();
                var txtDsnt = $('[id$=txtDsnt]').val().trim();
                var txtPkng = $('[id$=txtPkng]').val().trim();
                var txtAdchrgs = $('[id$=txtAdtnChrgs]').val().trim();

                if ($('[id$=chkExdt]').is(':checked') && txtExdt != '') {
                    CHKExDuty = txtExdt;
                }
                if ($('[id$=chkDsnt]').is(':checked') && txtDsnt != '') {
                    CHKDiscount = txtDsnt;
                }
                if ($('[id$=chkSltx]').is(':checked') && txtSlsTax != '') {
                    CHKSalesTax = txtSlsTax;
                }
                if ($('[id$=chkPkng]').is(':checked') && txtPkng != '') {
                    CHKPacking = txtPkng;
                }
                if ($('[id$=chkACgs]').is(':checked') && txtAdchrgs != '') {
                    CHKAdChrgs = txtAdchrgs;
                }
                var hfCurrentPage = $('[id$=hfCurrentPage]').val();
                var ddlRowsChanged = $('[id$=ddlRowsChanged]').val();
                if (returnVal != '')
                    obj3 = returnVal;
                if (obj3 == null || descrip == null)
                    obj3 = 0;
                if ((Discount == 0) && (txtExdt == 0) && (txtDsnt == 0) && (txtPkng == 0) && (txtAdchrgs == 0) && (ExDuty == 0) && (Price != 0) && (qty != 0) && (obj3 == 0)) {
                    var result = NewLQuotation.SaveChangesWhileRateChange(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                    if (result.value) {
                        $('#' + lblNetRate).text(parseFloat(Price).toFixed(2));
                        $('#' + lblAmount).text((qty * Price).toFixed(2));
                        $('#' + lblGrandTotal).text((qty * Price).toFixed(2));

                        $('#' + lblTotalAmt).text(parseFloat((qty * Price) + ((parseFloat(TotalAmount.replace(',', '')) - parseFloat(Amount.replace(',', ''))))).toFixed(2));
                        $('#' + lblGrandAmount).text(parseFloat((qty * Price) + ((parseFloat(GrandAmount.replace(',', '')) - parseFloat(GrandTotal.replace(',', ''))))).toFixed(2));
                    }
                }
                else {
                    if ((Discount != '' && Discount != '.') && (Price != '' && Price != '.') && (ExDuty != '' && ExDuty != '.') && (qty != '' && qty != '.')) {
                        var result = NewLQuotation.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, hfCurrentPage, ddlRowsChanged);
                        var getDivLQItems = GetClientID("divLQItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                        CalculateExDuty();
                    }
                    else {
                        if (Discount == '' || Discount == '.') {
                            var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
                            $('#' + txtDiscount).val('0.00');
                            $('#' + txtDiscount).focus();

                            ErrorMessage('Discount Cannot Be Empty.');

                        }
                        else if (Price == '' || Price == '.') {
                            var txtPrice = GetClientID("txtPrice" + (parseInt(obj))).attr("id");
                            $('#' + txtPrice).val('0.00');
                            $('#' + txtPrice).focus();

                            ErrorMessage('Price Cannot Be Empty.');

                        }
                        else if (ExDuty == '' || ExDuty == '.') {
                            if ($('[id$=txtPercent]').length > 0) {
                                var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
                                $('#' + txtExDuty).val('0.00');
                                $('[id$=' + txtExDuty + ']').focus();
                                ErrorMessage('ExDutyPercentage Cannot Be Empty.');
                            }
                        }
                        else if (qty == '' || qty == '.') {
                            var txtqty = GetClientID("txtQuantity" + (parseInt(obj))).attr("id");
                            $('#' + txtqty).val('0');
                            $('#' + txtqty).focus();

                            ErrorMessage('Quantity Cannot Be Empty.');

                        }
                    }
                }
                scroll2Row(obj);
            }
            catch (Error) {
                ErrorMessage(Error.Message);
            }
        }

    </script>
</asp:Content>
