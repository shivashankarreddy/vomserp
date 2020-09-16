<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true"
    CodeBehind="VerbalLPO_Customer.aspx.cs" Inherits="VOMS_ERP.Customer_Access.VerbalLPO_Customer" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Local Purchase Order Verbal"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="6">
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
            <td class="bcTdNewTable">
                <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                    align="center">
                    <tr>
                        <%--<td class="bcTdnormal">
                            <span class="bcLabel">Foreign Purchase Order No.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox ID="ListBoxFPO" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ddlFpoNo_SelectedIndexChanged">
                            </asp:ListBox>
                        </td>--%>
                        <td>
                            <span class="bcLabel">Customer Name :<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="bcAspdropdown" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
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
                        </td>
                        <td class="bcTdnormal">
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
                            <asp:DropDownList runat="server" ID="ddlSuplr" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlSuplr_SelectedIndexChanged">
                                <asp:ListItem Text="Select Supplier" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                        </td>
                        <td class="bcTdnormal">
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Local PurchaseOrder No.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtLpono" CssClass="bcAsptextbox" onchange="CheckDuplicates()"
                                Enabled="false"></asp:TextBox>
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
                            <div id="divLPOItems" class="aligntable" runat="server" style="margin-left: 10px !important;">
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
                                            <span class="bcLabel">Day(s)</span></div>
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
                                            <span class="bcLabel">Day(s)</span></div>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span runat="server" id="ECEA_spn_txt" class="bcLabel">Excise Duty Exemption Applicable:
                                        </span>
                                        <asp:CheckBox runat="server" ID="ChkbCEEApl" Text="" onclick='CHeck("ChkbCEEApl", "dvCEEApl")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvCEEApl" runat="server" style="display: none;">
                                            <span id="Span1" runat="server" class="bcLabel">Remainder Time <font color="red"
                                                size="2"><b>*</b></font>:</span>
                                            <asp:TextBox runat="server" ID="txtCEEApl" CssClass="bcAsptextbox" onblur="extractNumber(this,0,false);"
                                                onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                Width="23px" MaxLength="2" onfocus="this.select()" onMouseUp="return false" onchange="CheckDP(this.id)"></asp:TextBox>
                                            <span class="bcLabel">Day(s)</span>
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
                        <td class="bcTdNewTable" colspan="6">
                            <table style="width: 100%; overflow: auto;">
                                <tr>
                                    <td>
                                        <span id="lblED" runat="server" class="bcLabel">Excise Duty:</span>
                                        <asp:CheckBox runat="server" ID="chkExdt" Text=" " onclick='CHeckForLpo("chkExdt", "dvExdt")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvExdt" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnExdt" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtExdt" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                onkeypress="return blockNonNumbers(this, event, true, false);" Style="text-align: right;"
                                                Width="35px" MaxLength="5" onchange="CalculateExDuty();"></asp:TextBox></div>
                                    </td>
                                    <td>
                                        <span id="Span2" class="bcLabel">Sale Tax:</span>
                                        <asp:CheckBox runat="server" ID="chkSltx" Text=" " onclick='CHeckForLpo("chkSltx", "dvSltx")'
                                            CssClass="bcCheckBoxList" />
                                    </td>
                                    <td>
                                        <div id="dvSltx" style="display: none;">
                                            <asp:RadioButtonList runat="server" ID="rbtnSltx" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="%" Value="0" Selected="True"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtSltx" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                onfocus="this.select()" onMouseUp="return false" Width="35px" Style="text-align: right;"
                                                MaxLength="5" onchange="CalculateExDuty();"></asp:TextBox></div>
                                    </td>
                                    <td>
                                        <span id="lblPck" class="bcLabel">Packing:</span>
                                        <asp:CheckBox runat="server" ID="chkPkng" Text=" " onclick='CHeckForLpo("chkPkng", "dvPkng")'
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
                                                Width="35px" MaxLength="5" onchange="CalculateExDuty();"></asp:TextBox></div>
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
                                            </asp:RadioButtonList>
                                            <asp:TextBox runat="server" ID="txtDsnt" Text="0" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                onfocus="this.select()" onMouseUp="return false" onkeyup="extractNumber(this,2,false);"
                                                onkeypress="return blockNonNumbers(this, event, true, false);" Style="text-align: right;"
                                                Width="35px" MaxLength="10" onchange="CalculateExDuty();"></asp:TextBox></div>
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
                                                        onblur="extractNumber(this, 0, false); chkDlvrPeriod()"></asp:TextBox><span class="bcLabel">Days</span>
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
                                    <td colspan="8">
                                        Payment Terms :
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="8" class="bcTdNewTable">
                                        <center>
                                            <div style="overflow: auto; width: 35%;" id="divPaymentTerms" runat="server">
                                            </div>
                                        </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="8" class="bcTdNewTable">
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
                                    <td colspan="8" class="bcTdNewTable" align="right">
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
                                            <asp:HiddenField ID="HFSelectedVal" runat="server" />
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
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/date.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            if ($("[id$=rbtnDsnt]").find(":checked").val() == 1) {
                $("[id$=txtDsnt]").width(100);
            }
            else {
                $("[id$=txtDsnt]").width(35);
            }
            ItemWiseDsct();
            ItemWiseExciseDuty();
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
                    $('div[id*=' + dvid + '] input[type=text]').val('0');
                    CalculateExDuty();
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
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

        function CheckQty(txtQty) {
            try {
                var txtQtyval = document.getElementById(txtQty.id).value;
                var ClientID = txtQty.id.split("txtQty");
                if (!txtQtyval || 0 === txtQtyval.length || parseFloat(txtQtyval) == 0) {
                    document.getElementById(txtQty.id).value = $('[id$=' + ClientID[0] + 'Hfd_MQty]').val();
                    ErrorMessage("Quantity Can't be Zero/Empty");
                }
                else if (GetQueryStringParams('ID') != "") {
                    if (parseFloat($('[id$=' + ClientID[0] + 'HF_RCVDQty]').val()) != 0 &&
                        (parseFloat($('[id$=' + txtQty.id + ']').val()) > parseFloat($('[id$=' + ClientID[0] + 'HF_RCVDQty]').val()))) {
                        ErrorMessage("Quantity Exceeds FPO Quantity/Part Quantity is already released with another PO");
                    }
                    else if (parseFloat($('[id$=' + txtQty.id + ']').val()) > parseFloat($('[id$=' + ClientID[0] + 'HF_ActualQty]').val())) {
                        ErrorMessage("Quantity Exceeds FPO Quantity");
                    }
                }
                else if (parseFloat($('[id$=' + txtQty.id + ']').val()) > parseFloat($('[id$=' + ClientID[0] + 'HF_RCVDQty]').val())) {
                    var RmngQTY = (parseFloat($('[id$=' + ClientID[0] + 'HF_RCVDQty]').val()) - parseFloat($('[id$=' + ClientID[0] + 'HF_RCVDQty]').val()));
                    document.getElementById(txtQty.id).focus();
                    alert("Quantity Exceeds FPO Quantity/Part Quantity is already released with another PO");
                }
            }
            catch (Error) {
                ErrorMessage(Error);
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
                $('[id$=divMyMessage]').append('<span class="Error">Excise Duty Cannot be Grater than 99.99%</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                $('[id$=txtExdt]').val('0');
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
                var result = VerbalLPO_Customer.CheckDuplicateLPOs(LpoNmbr);
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

        function getPaymentValues(RNo) {
            var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(RNo))).attr("id");
            var PercAmt = $('#' + txtPercAmt).val();
            var txtDesc = GetClientID("txtDesc" + (parseInt(RNo))).attr("id");
            var Desc = $('#' + txtDesc).val();
            if (PercAmt != '0' && PercAmt != '' && Desc != '') {
                var result = VerbalLPO_Customer.PaymentAddItem(RNo, PercAmt, Desc);
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
                var result = VerbalLPO_Customer.PaymentDeleteItem(id);
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

        //        function Myvalidations() {
        //            var TotPayAmt = 0;
        //            var PaymentRCount = $('#tblPaymentTerms tbody tr').length;
        //            var LPOrderItems = ($('[id$=tblItems]').length > 0) ? $('[id$=tblItems]')[0].rows.length : 0;

        //            if (($('[id$=ddlCustomer]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
        //                ErrorMessage('Customer Name is required');
        //                $('[id$=ddlCustomer]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=ddlRsdby]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
        //                ErrorMessage('Department is Required.');
        //                $('[id$=ddlRsdby]').focus();
        //                return false;
        //            } 
        //            else if (($('[id$=ddlsuplrctgry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
        //                ErrorMessage('Supplier Category is Required.');
        //                $('[id$=ddlsuplrctgry]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=ddlSuplr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
        //                ErrorMessage('Supplier is Required.');
        //                $('[id$=ddlSuplr]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=txtLpono]').val()).trim() == '') {
        //                ErrorMessage('Lcl Purchase Order Number is Required.');
        //                $('[id$=txtLpono]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=txtLpoDt]').val()).trim() == '') {
        //                ErrorMessage('Lcl Purchase Order Date is Required.');
        //                $('[id$=txtLpoDt]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=txtLpoDueDt]').val()).trim() == '') {
        //                ErrorMessage('Lcl Purchase Order Due Date is Required.');
        //                $('[id$=txtLpoDueDt]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=txtsubject]').val()).trim() == '') {
        //                ErrorMessage('Subject is Required.');
        //                $('[id$=txtsubject]').focus();
        //                return false;
        //            }
        //            else if ($('[id$=DivComments]').css("visibility") == "visible") {
        //                if (($('[id$=txtComments]').val()).trim() == '') {
        //                    ErrorMessage('Comment is Required.');
        //                    $('[id$=txtComments]').focus();
        //                    return false;
        //                }
        //            }
        //            if (LPOrderItems > 0) {
        //                if (LPOrderItems == 1) {
        //                    ErrorMessage('No Items to Save.');
        //                    $('[id$=gvLpoItems]').focus();
        //                    return false;
        //                }
        //                else {
        //                    var select = 0;
        //                    for (var i = 1; i < LPOrderItems; i++) {
        //                        if (i > 1 || LPOrderItems < i - 1) {
        //                            var j = i - 1;
        //                            var txtPrice = GetClientID("txtPrice" + j).attr("id");
        //                            var ItemCheck = GetClientID("HFStatus" + j).attr("id");
        //                            var ItmPrice = $('#' + txtPrice).val();
        //                            var ItmCheckedrnot = $('#' + ItemCheck).val();
        //                            var chkbx = "ckhChaild" + j; // if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
        //                            var chkbval = GetClientID(chkbx + "_ItmChkbx").attr("id");
        //                            if ($('#' + chkbx)[0].checked) {
        //                                select = select + 1;
        //                            }
        //                            if (ItmPrice == "0" && ItmCheckedrnot == "True") {
        //                                $('#' + txtPrice).focus();
        //                                ErrorMessage('Price Cannot Be Zero.');
        //                                return false;
        //                            }
        //                        }
        //                    }
        //                    if (select == 0) {
        //                        ErrorMessage('Select At Least One Item.');
        //                        $('[id$=gvLpoItems]').focus();
        //                        return false;
        //                    }
        //                }
        //            }
        //            else if (LPOrderItems == 0) {
        //                ErrorMessage('No Items to Save.');
        //                $('[id$=gvLpoItems]').focus();
        //                return false;
        //            }
        //            else if (Price == '' || Price == '.' || Price == '0') {
        //                var txtPrice = GetClientID("txtPrice" + (parseInt(obj))).attr("id");
        //                $('#' + txtPrice).val('0.00');
        //                $('#' + txtPrice).focus();

        //                ErrorMessage('Price Cannot Be Empty.');

        //            }
        //            if ($('[id$=ChkbDrwngAprls]')[0].checked == true) {
        //                var aDrawingAp = ($('[id$=txtDrwngAprls]').val()).trim();
        //                var aDel = ($('[id$=txtDlvry]').val()).trim();
        //                if (($('[id$=txtDrwngAprls]').val()).trim() == '') {
        //                    ErrorMessage('Drawing Approvals Remainder Time is Required.');
        //                    $('[id$=txtDrwngAprls]').focus();
        //                    return false;
        //                }
        //                else if (parseInt(aDrawingAp) == 0) {
        //                    ErrorMessage('Drawing Approvals Remainder Time cannot be Zero.');
        //                    $('[id$=txtDrwngAprls]').focus();
        //                    return false;
        //                }
        //                else if (parseInt(aDrawingAp) > parseInt(aDel)) {
        //                    ErrorMessage('Drawing Approvals Remainder Time should less than Delivery Period.');
        //                    $('[id$=txtDrwngAprls]').focus();
        //                    return false;
        //                }
        //            }
        //            if ($('[id$=ChkbInspcn]')[0].checked == true) {
        //                if (($('[id$=txtInsptn]').val()).trim() == '') {
        //                    ErrorMessage('Inspection Remainder Time is Required.');
        //                    $('[id$=txtInsptn]').focus();
        //                    return false;
        //                }
        //                if (($('[id$=txtInsptn]').val()).trim() == '0') {
        //                    ErrorMessage('Inspection Remainder Time cannot be zero.');
        //                    $('[id$=txtInsptn]').focus();
        //                    return false;
        //                }
        //                else if (parseInt(($('[id$=txtInsptn]').val()).trim()) > parseInt(aDel)) {
        //                    ErrorMessage('Inspection Remainder Time should less than Delivery Period.');
        //                    $('[id$=txtInsptn]').focus();
        //                    return false;
        //                }
        //            }
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
        //            if (($('[id$=ddlPrcBsis]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
        //                ErrorMessage('Price Basis is Required.');
        //                $('[id$=ddlPrcBsis]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=txtPriceBasis]').val()).trim() == '') {
        //                ErrorMessage('Price Basis Location is Required.');
        //                $('[id$=txtPriceBasis]').focus();
        //                return false;
        //            }
        //            else if (($('[id$=txtDlvry]').val()).trim() == '' || $('[id$=txtDlvry]').val() == '0') {
        //                ErrorMessage('Delivery Period is Required.');
        //                $('[id$=txtDlvry]').focus();
        //                return false;
        //            }
        //            if (PaymentRCount > 0) {
        //                for (var k = 1; k <= PaymentRCount; k++) {
        //                    var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(k))).attr("id");
        //                    var PercAmt = $('#' + txtPercAmt).val();
        //                    var txtDesc = GetClientID("txtDesc" + (parseInt(k))).attr("id");
        //                    var Desc = $('#' + txtDesc).val();
        //                    TotPayAmt += parseFloat(PercAmt);
        //                    if (PercAmt == '' || PercAmt == '0' || Desc == '') {
        //                        var message = '';
        //                        if (PercAmt == '')
        //                            message = 'Payment Is Required';
        //                        else if (PercAmt == '0')
        //                            message = 'Payment Cannot be Zero';
        //                        else if (Desc == '')
        //                            message = 'Description Is Required';
        //                        ErrorMessage(message + ' of SNo : ' + k);
        //                        return false;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (PaymentRCount == 0 || TotPayAmt < 100) {
        //                ErrorMessage('Payment has to be 100%.');
        //                return false;
        //            }
        //            return true;
        //        }
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
        //        function fnOpen() {
        //            var sFeatures = fnSetValues();
        //            //window.showModalDialog("../Masters/TermsNConditions.aspx", "509", sFeatures);
        //            window.showModalDialog("../Masters/TermsNConditions.aspx?TAr=General", "508", sFeatures);
        //        }

        //        function fnOpen(id, rowIndex) {
        //            var sFeatures = fnSetValues();
        //            window.showModalDialog("../Customer Access/AddItem_PI.aspx", "", sFeatures);
        //            BindGridView(rowIndex);
        //        }

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
            var result = VerbalLPO_Customer.AddItemListBox();
            var getDivLEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivLEItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                SuccessMessage('File Uploaded Successfully.');
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
            ErrorMessage('File upload Failed.');
        }
        function uploadStarted() {
            SuccessMessage('File Uploaded Started.');
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = VerbalLPO_Customer.DeleteItemListBox($('#lbItems').val());
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
            var result = VerbalLPO_Customer.AddItemListBox();
            var getDivFEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
        });
    </script>
    <script type="text/javascript">
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
            var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
            var ExDuty = (txtExDuty == undefined ? "0" : $('#' + txtExDuty).val()); if (txtExDuty == '') { txtExDuty = 0.00; ExDuty = 0; }
            var ChkChaild = GetClientID("ckhChaild" + (parseInt(obj))).attr("id");
            var Chaild = $('#' + ChkChaild).is(':checked');
            //            var HFStatus = GetClientID("HFStatus" + (parseInt(obj))).attr("id");
            var Status = "false";

            var CHKExDuty = 0; var CHKDiscount = 0; var CHKSalesTax = 0; var CHKPacking = 0; var CHKAdChrgs = 0;

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
            var txtDsnt = $('[id$=txtDsnt]').val().trim();
            var txtSlsTax = 0;
            var txtPkng = $('[id$=txtPkng]').val().trim();
            var txtAdchrgs = 0; // $('[id$=txtAdtnChrgs]').val().trim();

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
            if (Discount == "")
                Discount = "0.00";
            if (ExDuty == "")
                ExDuty = "0.00";
            if (Discount > 100) {
                ErrorMessage('Discount Percentage Cannot Exceed 100');
                Discount = "0.00";
            }
            if (ExDuty > 100) {
                ErrorMessage('Ex-Duty Percentage Cannot Exceed 100');
                ExDuty = "0.00";
            }
            var returnVal = "";
            if (returnVal != '')
                obj3 = returnVal;
            if (obj3 == null || descrip == null)
                obj3 = 0;
            if (obj3 != '0' && descrip != '') {
                if (Discount == '100' || Discount == '100.00') {
                    if (confirm("Are you sure you want to Give " + Discount + "% Discount ?")) {
                        var result = VerbalLPO_Customer.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, Status);
                        var getDivLQItems = GetClientID("divLPOItems").attr("id");
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
                    var result = VerbalLPO_Customer.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, Status);
                    var getDivLQItems = GetClientID("divLPOItems").attr("id");
                    $('#' + getDivLQItems).html(result.value);
                    if (qty == "0" && Chaild == true) {
                        ErrorMessage('This Item was already raised in another VerbalLPO.');
                    }
                    //                     if (!$('#' + ChkChaild).is(':checked')) {
                    //                            ErrorMessage("This Item was not selected in FPO.");
                    //                            return false;
                    //                        }
                }
            }
            else if (obj3 == '0' && descrip != '') {
                if (Discount == '100' || Discount == '100.00') {
                    if (confirm("Are you sure you want to Give " + Discount + "% Discount ?")) {
                        var result = VerbalLPO_Customer.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, Status);
                        var getDivLQItems = GetClientID("divLPOItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                    }
                    else {
                        var result = VerbalLPO_Customer.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, Status);
                        var getDivLQItems = GetClientID("divLPOItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                        //                        if (!$('#' + ChkChaild).is(':checked')) {
                        //                            ErrorMessage("This Item Was Not Selected in FPO.");
                        //                            return false;
                        //                        }
                    }
                }
                else {
                    if ((Discount != '' && Discount != '.') && (Price != '' && Price != '.') && (ExDuty != '' && ExDuty != '.') && (qty != '' && qty != '.')) {

                        var result = VerbalLPO_Customer.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, Status);
                        var getDivLQItems = GetClientID("divLPOItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                        if (parseFloat(ExDuty) != 0) {
                            CHKExDuty = 0.00;
                            $('[id$=chkExdt]').attr("disabled", true);
                            $('[id$=chkExdt]').attr('checked', false);
                            $('[id$=txtExdt]').val("0");
                            $('[id$=dvExdt]').hide();
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
                            var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
                            $('#' + txtExDuty).val('0.00');
                            $('[id$=' + txtExDuty + ']').focus();

                            ErrorMessage('ExDutyPercentage Cannot Be Empty.');

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
                    var result = VerbalLPO_Customer.SaveChanges(obj - 1, obj, obj3, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, Status);
                    var getDivLQItems = GetClientID("divLPOItems").attr("id");
                    $('#' + getDivLQItems).html(result.value);
                }
                else {
                    var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
                    $('#' + txtDiscount).val('0.00');

                    ErrorMessage('Item Descreption is Required.');

                }
            }

            if ($('[id$=chkExdt]').is(':checked') || $('[id$=chkDsnt]').is(':checked')) {
                CalculateExDuty();
            }
            var FlagExDuty = $('[id$=HFExDuty]').val();
            if (FlagExDuty == 0) {
                $('[id$=chkExdt]').removeAttr("disabled");
            } else {
                $('[id$=chkExdt]').attr("disabled", true);
                $('[id$=chkExdt]').attr('checked', false);
                $('[id$=txtExdt]').val("0");
                $('[id$=dvExdt]').hide();
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
                var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
                var ExDuty = (txtExDuty == undefined ? "0" : $('#' + txtExDuty).val());
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
                var txtAdchrgs = 0;
                var Status = "";
                //$('[id$=txtAdtnChrgs]').val().trim();

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

                //                if (returnVal != '')
                //                    obj3 = returnVal;
                //                if (obj3 == null || descrip == null)
                //                    obj3 = 0;
                if ((Discount == 0) && (txtExdt == 0) && (txtDsnt == 0) && (txtPkng == 0) && (txtAdchrgs == 0) && (ExDuty == 0) && (Price != 0) && (qty != 0) && (obj3 == 0)) {
                    var result = VerbalLPO_Customer.SaveChangesWhileRateChange(obj - 1, obj, 0, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild);
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
                        var result = VerbalLPO_Customer.SaveChanges(obj - 1, obj, 0, 0, 0, spec, make, qty, Price, Amount, Rmrks, Discount, UnitID, ExDuty, CHKExDuty, CHKDiscount, CHKSalesTax, CHKPacking, CHKAdChrgs, Chaild, Status);
                        var getDivLQItems = GetClientID("divLPOItems").attr("id");
                        $('#' + getDivLQItems).html(result.value);
                    }
                    else {
                        if (Discount == '' || Discount == '.') {
                            var txtDiscount = GetClientID("txtDiscount" + (parseInt(obj))).attr("id");
                            $('#' + txtDiscount).val('0.00');
                            $('#' + txtDiscount).focus();

                            ErrorMessage('Discount Cannot Be Empty.');

                        }
                        else if (Price == '' || Price == '.' || Price == '0') {
                            var txtPrice = GetClientID("txtPrice" + (parseInt(obj))).attr("id");
                            $('#' + txtPrice).val('0.00');
                            $('#' + txtPrice).focus();

                            ErrorMessage('Price Cannot Be Empty.');

                        }
                        else if (ExDuty == '' || ExDuty == '.') {
                            var txtExDuty = GetClientID("txtPercent" + (parseInt(obj))).attr("id");
                            $('#' + txtExDuty).val('0.00');
                            $('[id$=' + txtExDuty + ']').focus();

                            ErrorMessage('ExDutyPercentage Cannot Be Empty.');

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
            catch (Error) {
                ErrorMessage(Error.Message);
            }
        }

        function ItemWiseDsct() {
            var DiscountAmt = $('[id$=txtDsnt]').val();
            var DcstChk = $("[id$=rbtnDsnt]").find(":checked").val();
            var RwCunt = $("#tblItems > tbody > tr").length;
            if (DcstChk == 0 && DiscountAmt != 0) {
                for (var i = 1; i <= RwCunt; i++) {
                    $('[id$=txtDiscount' + i + ']').attr("disabled", "disabled");
                }
            }
        }

        function ItemWiseExciseDuty() {
            var ExciseAmt = $('[id$=txtExdt]').val();
            var ExChk = $("[id$=rbtnExdt]").find(":checked").val();
            var RwCunt = $("#tblItems > tbody > tr").length;
            if (ExChk == 0 && ExciseAmt != 0) {
                for (var i = 1; i <= RwCunt; i++) {
                    $('[id$=txtPercent' + i + ']').attr("disabled", "disabled");
                }
            }
        }

        function CalculateExDuty() {
            var ExDutyAmt = $('[id$=txtExdt]').val();
            var ExChk = $("[id$=rbtnExdt]").find(":checked").val();
            if (ExDutyAmt > 99.99 && ExChk == 0) {

                ErrorMessage('Excise Duty Cannot be Grater than 99.99%');

                $('[id$=txtExdt]').val('0');
                ExDutyAmt = 0;
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
            var Discount = $('[id$=chkDsnt]').is(':checked');
            var SalesTax = false;
            var Packing = $('[id$=chkPkng]').is(':checked');
            var AdsnlChrg = $('[id$=chkACgs]').is(':checked');
            if (ExDutyAmt == '') {
                ExDutyAmt = 0;
                $('[id$=txtExdt]').val('0');
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
            var res = VerbalLPO_Customer.CalculateExDuty(ExDutyAmt, DiscountAmt, PackingAmt, SalesTaxAmt, AdsnlCgrs, Discount, ExDuty, SalesTax,
            Packing, AdsnlChrg, DcstChk, ExChk, PkgChk, AddChk);
            var getDivLQItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivLQItems).html(res.value);
            Expnder();
            ItemWiseDsct();
            ItemWiseExciseDuty();
        }
    </script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });

        function uploadComplete() {
            var result = VerbalLPO_Customer.AddItemListBox();
            var getDivLEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivLEItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                SuccessMessage('File Uploaded Successfully.');
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
            ErrorMessage('File upload Failed.');
        }
        function uploadStarted() {
            SuccessMessage('File Uploaded Started.');
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = VerbalLPO_Customer.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    //SuccessMessage('Selected Attachment Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';

                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
            Expnder();
        });


        $('#lnkAdd').click(function () {
            var result = VerbalLPO_Customer.aspx.AddItemListBox();
            var getDivFEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
        });

        $(document).ready(function () {
            $('div.expanderR').expander();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }

        function chcked(CheckedID, UncheckedID) {
            if ($('[id$=' + CheckedID + ']')[0].checked) {
                $('[id$=' + UncheckedID + ']').attr("checked", false);
            }
        }

        var dateToday = new Date();
        $('[id$=txtReceivedDate]').datepicker({
            maxDate: dateToday,
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
            , showAnim: 'blind'
            //, showButtonPanel: true
        });

        function chkDlvrPeriod() {
            var Currentweeks = $('[id$=txtDlvry]').val();
            if (Currentweeks == 0) {
                ErrorMessage('Delivery Period should not be Zero');
                $('[id$=txtDlvry]').focus(0, 0);
                $('[id$=txtFpoDuedt]').val('');
            }
        }
        function ChangeDueDate() {
            var Currentweeks = $('[id$=txtDlvry]').val();
            if (Currentweeks == "") {
                $('[id$=txtDlvry]').focus(0, 0);
                $('[id$=txtFpoDuedt]').val('');
            }
            else {
                var strdate = $('[id$=txtReceivedDate]').val();
                var strdate1 = strdate.split('-');
                strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
                strdate = new Date(strdate.replace(/-/g, "/"));
                strdate.setDate(strdate.getDate() + (Currentweeks * 7));
                var month = strdate.getMonth() + 1;
                $('[id$=txtFpoDuedt]').val(("0" + strdate.getDate()).slice(-2) + '-' + ("0" + month).slice(-2) + '-' + strdate.getFullYear());
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

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function doConfirmPayment(id) {
            if (confirm("Are you sure you want to Delete Payment?")) {
                var result = VerbalLPO_Customer.PaymentDeleteItem(id);
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
                var result = VerbalLPO_Customer.PaymentAddItem(RNo, PercAmt, Desc);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);
                if ($('[id$=HfMessage]').val() != '') {

                    ErrorMessage('' + $('[id$=HfMessage]').val() + '');

                    $('[id$=' + txtPercAmt + ']').focus();
                }
                else
                    $('[id$=' + txtDesc + ']').focus();
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
        function doConfirm(id) {
            if (confirm("Are you sure you want to Delete?")) {
                var result = VerbalLPO_Customer.DeleteItem(id);
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                Expnder();
            }
            else {
                return false;
            }
        }

        function doConfirmRegret(id) {
            if (confirm("Are you sure you want to Regret?")) {
                var fenqid = $('[id$=ddlfenq] :selected').val();
                var result = VerbalLPO_Customer.RegretItem(id, fenqid);
                if (result.value == true) {

                    $('[id$=divMyMessage]').append('<span class="Success">Item Regretted Successfully.');


                    var result1 = VerbalLPO_Customer.BindGridView(0);
                    var getDivFEItems = GetClientID("divLPOItems").attr("id");
                    $('#' + getDivFEItems).html(result1.value);
                    Expnder();
                }
                else {

                    ErrorMessage('Error while Regretting.');

                }
            }
            else {
                return false;
            }
        }

        function AlertError() {
            alert('You donot have permissions to Regret, Contact your Admin.');
        }

        function AddItemColumn(id, isnew) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var ddlUnits = GetClientID("ddlU" + (parseInt(id))).attr("id");
            var Units = $('#' + ddlUnits).val();
            var txtSpec = GetClientID("txtSpecifications" + (parseInt(id))).attr("id");
            var spec = $('#' + txtSpec).val();
            var txtMake = GetClientID("txtMake" + (parseInt(id))).attr("id");
            var Make = $('#' + txtMake).val();
            var txtQty = GetClientID("txtQuantity" + (parseInt(id))).attr("id");
            var Qty = $('#' + txtQty).val();
            var txtrt = GetClientID("txtRate" + (parseInt(id))).attr("id");
            var Rate = $('#' + txtrt).val();
            var txtPrtNo = GetClientID("txtPrtNo" + (parseInt(id))).attr("id");
            var PrtNo = $('#' + txtPrtNo).val();
            var txtRmrks = GetClientID("txtRemarks" + (parseInt(id))).attr("id");
            var Rmrks = $('#' + txtRmrks).val();

            if (sv == undefined && PrtNo == '' && spec == '' && Make == '' && Qty == '' && Rate == 0) {

                ErrorMessage('Fill all the Details.');

                if (ddlUnits != undefined) {
                    $("#" + ddlUnits + " option[value='0']").attr("selected", "selected");
                }
            }
            else if (sv == undefined && PrtNo == undefined && spec == '' && Make == '' && Qty == '' && Rate == 0) {

                ErrorMessage('Fill all the Details..');

                if (ddlUnits != undefined) {
                    $("#" + ddlUnits + " option[value='0']").attr("selected", "selected");
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
                if (Rate == undefined || Rate == '0')
                    Rate = 0;
                var result = VerbalLPO_Customer.AddNewRow(id, sv, PrtNo, spec, Make, Qty, Rate, Units, Rmrks, isnew);
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
            }
            Expnder();
        }

        function DisplayRefFPO() {
            if ($('[id$=chkbIRO]')[0].checked) {
                $('[id$=spnRfpolbl]')[0].style.display = "block";
                $('[id$=spnRfpoddl]')[0].style.display = "block";
            }
            else {
                $('[id$=spnRfpolbl]')[0].style.display = "none";
                $('[id$=spnRfpoddl]')[0].style.display = "none";
                window.location = "VerbalLPO_Customer.aspx";
            }
            var result = VerbalLPO_Customer.ChkBoxRptdFPOMode($('[id$=chkbIRO]')[0].checked);
        }
        function FillItemDRP(id) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = VerbalLPO_Customer.FillItemDRP(id, sv);
            var getDivFEItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        function FillUnitDRP(id) {
            var ddlCat = GetClientID("ddlU" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = VerbalLPO_Customer.FillUnitDRP(id, sv);
            var getDivFEItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        function Myvalidations() {
            var PaymentRCount = $('#tblPaymentTerms tbody tr').length;
            var TotalAmt = 0;
            var rowCount = $('#tblItems tbody tr').length;
            var count = 0;
            var Price = $('#txtPrice').val(); if (Price == '') { Price = 0.00; }

            if (($('[id$=ddlCustomer]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Customer Name is required');
                $('[id$=ddlCustomer]').focus();
                return false;
            }
            else if (($('[id$=ddlRsdby]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Department is Required.');
                $('[id$=ddlRsdby]').focus();
                return false;
            }
            else if (($('[id$=ddlsuplrctgry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Supplier Category is Required.');
                $('[id$=ddlsuplrctgry]').focus();
                return false;
            }
            else if (($('[id$=ddlSuplr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Supplier is Required.');
                $('[id$=ddlSuplr]').focus();
                return false;
            }
            //            else if (($('[id$=txtLpono]').val()).trim() == '') {
            //                ErrorMessage('Local Purchase Order Number is Required.');
            //                $('[id$=txtLpono]').focus();
            //                return false;
            //            }
            else if (($('[id$=txtLpoDt]').val()).trim() == '') {
                ErrorMessage('Local Purchase Order Date is Required.');
                $('[id$=txtLpoDt]').focus();
                return false;
            }
            else if (($('[id$=txtLpoDueDt]').val()).trim() == '') {
                ErrorMessage('Local Purchase Order Due Date is Required.');
                $('[id$=txtLpoDueDt]').focus();
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
            else if (Price == '' || Price == '.' || Price == '0') {
                var txtPrice = GetClientID("txtPrice" + (parseInt(obj))).attr("id");
                $('#' + txtPrice).val('0.00');
                $('#' + txtPrice).focus();

                ErrorMessage('Price Cannot Be Empty.');

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
            if ($('[id$=ChkbCEEApl]')[0].checked == true) {
                if (($('[id$=txtCEEApl]').val()).trim() == '') {
                    ErrorMessage('CE-Excemption Remainder Time is Required.');
                    $('[id$=txtCEEApl]').focus();
                    return false;
                }
                if (($('[id$=txtCEEApl]').val()).trim() == '0') {
                    ErrorMessage('CE-Excemption Remainder Time is Required.');
                    $('[id$=txtCEEApl]').focus();
                    return false;
                }
                else if (parseInt(($('[id$=txtCEEApl]').val()).trim()) > parseInt(aDel)) {
                    ErrorMessage('CE-Excemption Remainder Time should less than Delivery Period.');
                    $('[id$=txtCEEApl]').focus();
                    return false;
                }
            }
            //VerbalLPO_Customer.FillItemGrid
            var result = VerbalLPO_Customer.ValidateItems();
            if (result.value.indexOf("ERROR::") >= 0) {
                var val = result.value.split('ERROR::');
                ErrorMessage(val[1]);
                return false;
            }


            //            var rowCount = $('#tblItems tbody tr').length;
            //            if (rowCount == 1) {
            //                for (var i = 1; i <= rowCount; i++) {
            //                    var units = $('#ddlUnits' + '' + i).val();
            //                    var qty = $('#txtQty' + '' + i).val();
            //                    var ItDesc = $('#ddl' + '' + i).val();
            //                    if (ItDesc != 0) {
            //                        if ($('#ddlUnits' + '' + i).val() == '0' || $('#txtQty' + '' + i).val() == '0' || $('#txtQty' + '' + i).val() == '') {
            //                            //alert('sample : ' + $('#ddlUnits' + '' + i).val());
            //                            var message = '';
            //                            if (qty == '')
            //                                message = 'Quantity is Required';
            //                            else if (qty == '0')
            //                                message = 'Quantity Cannot Be Zero';
            //                            else if (units == 0)
            //                                message = 'Units is Required'; ErrorMessage(message + ' of SNo : ' + i + '.');
            //                            return false;
            //                            break;
            //                        }
            //                    }
            //                    else {
            //                        ErrorMessage('Please Select atleast 1 Item.');
            //                        return false;
            //                        break;
            //                    }
            //                }
            //            }
            //            else if (rowCount > 1) {
            //                for (var i = 1; i <= rowCount; i++) {
            //                    var units = $('#ddlUnits' + '' + i).val();
            //                    var qty = $('#txtQty' + '' + i).val();
            //                    var ItDesc = $('#ddl' + '' + i).val();
            //                    if (ItDesc != 0) {
            //                        if (units == 0 || qty == 0 || qty == '') {
            //                            var message = '';
            //                            if (qty == '') {
            //                                message = 'Quantity is Required';
            //                                $('#txtQty' + '' + i).focus();
            //                            }
            //                            else if (qty == '0') {
            //                                message = 'Quantity Cannot Be Zero';
            //                                $('#txtQty' + '' + i).focus();
            //                            }
            //                            else if (units == 0) {
            //                                message = 'Units is Required';
            //                                $('#ddlUnits' + '' + i).focus();
            //                            }
            //                            ErrorMessage(message + ' of SNo : ' + i + '.');
            //                            return false;
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //            else if (rowCount == 0) {
            //                ErrorMessage('No Rows to save.');
            //                return false;
            //            }

            if ($('[id$=ddlPrcBsis]').val() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Price Basis is Required.');
                $('[id$=ddlPrcBsis]').focus();
                return false;
            }
            else if (($('[id$=txtPriceBasis]').val()).trim() == '') {
                ErrorMessage('Price Basis Location is Required.');
                $('[id$=txtPriceBasis]').focus();
                return false;
            }
            else if ($('[id$=txtDlvry]').val() == '' || $('[id$=txtDlvry]').val() == '0') {
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
                    TotalAmt = parseFloat(TotalAmt) + parseFloat(PercAmt);
                    if (PercAmt == '' || PercAmt == '0' || Desc == '') {
                        //alert('sample : ' + $('#ddlUnits' + '' + i).val());
                        var message = '';
                        if (PercAmt == '')
                            message = 'Payment Is Required';
                        else if (PercAmt == '0')
                            message = 'Payment Cannot be Zero';
                        else if (Desc == '')
                            message = 'Description Is Required';
                        ErrorMessage('' + message + ' of SNo : ' + k + '.');
                        //$('[id$=txtPercAmt' + k - 1 + ']').focus();
                        return false;
                        break;
                    }
                }
                if (PaymentRCount == 0 || TotalAmt < 100) {
                    ErrorMessage('Total Payment percentage should be 100.');
                    return false;
                }
            }
            else if (PaymentRCount <= 0) {
                ErrorMessage('Payment Terms is Required.');
                return false;
            }
            else {
                return true;
            }
        }
        //        function fnOpen(id, rowIndex) {
        //            var sFeatures = fnSetValues();
        //            window.showModalDialog("../Enquiries/AddItems.aspx", "", sFeatures);
        //            BindGridView(rowIndex);
        //        }

        function SpclChars(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 44 && charCode != 45 && charCode != 46 && charCode != 47 && charCode != 58
            && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function fnSetValues() {
            var iHeight = 500;
            var iWidth = 1000;
            var sFeatures = "dialogHeight: " + iHeight + "px; dialogWidth: " + iWidth + "px;";
            return sFeatures;
        }
        function fnOpen_TC() {
            var sFeatures = fnSetValues();
            window.showModalDialog("../Masters/TermsNConditions.aspx?TAr=General", "508", sFeatures);
        }

        function CheckLPOOrderNo() {
            var enqNo = $('[id$=txtLpoNo]').val().trim();
            var result = VerbalLPO_Customer.CheckLPOOrderNo(enqNo);
            if (result.value == false) {
                $('#txtLpoNo').val('').focus();
                ErrorMessage('LPO Order Number Exists.');
                return false;
            }
        }

        function CheckItem(ID) {
            var IsChecked = $('#ckhChaild' + ID).is(':checked');
            var result = VerbalLPO_Customer.CheckItem(IsChecked, ID);
            $('#tblItems').html('');
            $('#tblItems').html(result.value);
            Expnder();
        }

        var oTable;
        $(document).ready(function () {
            DesignGrid();
        });

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 84 + "px");
        });

        function DesignGrid() {
            $("[id$=tblItems]").dataTable({
                "bScrollCollapse": true,
                "bPaginate": false,
                "bSort": false,
                "bFilter": false,
                "bInfo": false,
                "bJQueryUI": true,
                "bAutoWidth": false,
                "sScrollY": "750px",
                "sScrollX": "100%"
                //"sScrollXInner": "100%"

            });
            oTable = $('#tblItems').dataTable();
        }

        function UpdateSelectedItem() {
            var RowID = 0;
            var ItemID = $('#ddl' + RowID).val();
            var PartNo = $('#txtPartNo' + RowID).val();
            var Spec = $('#txtSpec' + RowID).val();
            var Make = $('#txtMake' + RowID).val();
            var Qty = $('#txtQuantity' + RowID).val();
            if (Qty != '') {
                Qty = parseFloat(Qty);
            }
            var UnitID = $('#ddlUnits' + RowID).val();
            var Rate = $('#txtRate' + RowID).val();
            if (Rate == '') {
                Rate = 0;
                Rate = parseFloat(Rate);
            }
            var Remarks = $('#txtRemarks' + RowID).val();

            if (ItemID != '00000000-0000-0000-0000-000000000000' && (Qty != 0 || Qty != '') && UnitID != 0 && Rate != 0) {
                var selVal = $('[id$=HFSelectedVal]').val();
                var result = VerbalLPO_Customer.UpdateSelectedItem(selVal, ItemID, PartNo, Spec, Make, Qty, UnitID, Rate, Remarks);
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                DesignGrid();
            }
            else {
                if (ItemID == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('ItemDescription is Required.');
                    $('#ddl' + RowID).focus();
                }
                else if (Qty == '') {
                    ErrorMessage('Quantity cannot be empty.');
                    $('#txtQuantity' + RowID).focus();
                }
                else if (Qty == 0) {
                    ErrorMessage('Quantity cannot be zero.');
                    $('#txtQuantity' + RowID).focus();
                    $('#txtQuantity' + RowID).val('');
                }
                else if (UnitID == 0) {
                    ErrorMessage('Unit is Required.');
                    $('#ddlUnits' + RowID).focus();
                }
                else if (Rate == '' || parseFloat(Rate) == 0) {
                    ErrorMessage('Amount cannot be Empty/zero.');
                    $('#txtRate' + RowID).focus();
                    $('#txtRate' + RowID).val('');
                }
            }
        }

        function SelectDDLunits(ID) {
            var DDLText = "No(s)";
            var DDLVal = "" + $("#ddlUnits0 option:contains('" + DDLText + "')").attr('value') + "";
            $('#ddlUnits' + ID).val(DDLVal);
        }

        //        function FillSpec_ItemDesc(ID) {
        //            var ItemID = $('#ddl' + ID).val();
        //            var result = VerbalLPO_Customer.FillSpec_ItemDesc(ItemID);
        //            var arr = result.value.split('&@&');
        //            $('#txtPartNo' + ID).val(arr[0]);
        //            $('#txtSpec' + ID).val(arr[1]);
        //            SelectDDLunits(ID);
        //        }

        function FillSpec_ItemDesc(ID) {
            var ItemID = $('#ddl' + ID).val();
            var result = VerbalLPO_Customer.FillSpec_ItemDesc(ItemID);
            var arr = result.value.split('&@&');
            //$('#Item' + ID).val(arr[0]);
            $('#ddl' + ID).val(arr[0]);
            $('#txtPartNo' + ID).val(arr[2]);
            $('#txtSpec' + ID).val(arr[3]);
            $('[id$=HFIsSubItem]').val(arr[4]);
            //$('#ddlUnits' + ID).val('103');
            SelectDDLunits(ID);
        }

        function AddItemRow(RowID) {
            var ItemID = $('#ddl' + RowID).val();
            var PartNo = $('#txtPartNo' + RowID).val();
            var Spec = $('#txtSpec' + RowID).val();
            var Make = $('#txtMake' + RowID).val();
            var Qty = $('#txtQuantity' + RowID).val();
            var UnitID = $('#ddlUnits' + RowID).val();
            var Rate = $('#txtRate' + RowID).val();
            var Remarks = $('#txtRemarks' + RowID).val();
            if (Rate == '') {
                Rate = 0;
            }


            if (ItemID != '00000000-0000-0000-0000-000000000000' && (parseFloat(Qty) != 0 && Qty != '') && UnitID != 0 && (parseFloat(Rate) != 0 && Rate != '')) {
                var result = VerbalLPO_Customer.FillItemGrid(RowID, ItemID, PartNo, Spec, Make, Qty, UnitID, Rate, Remarks);
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                // $('#tblItems tbody').html('');
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                //$('#tblItems tbody').append(result.value);

                var Count = (Number($('#tblItems tbody tr').length));
                $('#btnDel' + Count).focus();
                RowID = 0;
                $('#ddl' + RowID + ' > option[value=' + ItemID + ']').remove();
                $('#ddl' + RowID).val('0');
                $('#txtPartNo' + RowID).val('');
                $('textarea#txtSpec' + RowID).val('');
                $('#txtMake' + RowID).val('');
                $('#txtQuantity' + RowID).val('');
                $('#txtRate' + RowID).val('');
                $('#txtRemarks' + RowID).val('');
                SelectDDLunits(RowID);
                DesignGrid();
            }
            else {
                if (ItemID == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('ItemDescription is Required.');
                    $('#ddl' + RowID).focus();
                }
                else if (Qty == '') {
                    ErrorMessage('Quantity cannot be empty.');
                    $('#txtQuantity' + RowID).focus();
                }
                else if (parseFloat(Qty) == 0) {
                    ErrorMessage('Quantity cannot be zero.');
                    $('#txtQuantity' + RowID).focus();
                    $('#txtQuantity' + RowID).focus('');
                }
                else if (UnitID == 0) {
                    ErrorMessage('Unit is Required.');
                    $('#ddlUnits' + RowID).focus();
                }
                else if (Rate == '' || parseFloat(Rate) == 0) {
                    ErrorMessage('Amount cannot be Empty/zero.');
                    $('#txtRate' + RowID).focus();
                    $('#txtRate' + RowID).val('');
                }
            }
        }

        var returnVal = "";
        var flag = 0;
        function fnOpen(id) {
            returnVal = window.showModalDialog("../Customer Access/AddItem_PI.aspx", "Add Item",
            "dialogHeight:680px; dialogWidth:980px; dialogLeft:150; dialogright:150; dialogTop:150; ");
            if (returnVal != null) {
                var rtnVal = returnVal.split(',');
                if (rtnVal[1].trim() == "") {
                    $('#ddl' + id).val(rtnVal[0].toLowerCase());
                    FillSpec_ItemDesc(rtnVal[0].toLowerCase());
                }
                else {
                    var result = VerbalLPO_Customer.NewItemAdded();
                    var getDivFEItems = GetClientID("divFPOItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    $('#ddl' + id).val(rtnVal[0]);
                    FillSpec_ItemDesc(id);
                    DesignGrid();
                }
                returnVal = "";
            }
            else
                returnVal = "";
        }


        function FillItemGrid(obj1, obj2) {

            var obj3 = $('#hfItemID' + (parseInt(obj2) + 1)).val();
            var ddlUnit = GetClientID("ddlUnits" + (parseInt(obj2) + 1)).attr("id");
            var obj5 = $('#' + ddlUnit).val();
            var txtspec = GetClientID("txtSpecification" + (parseInt(obj2) + 1)).attr("id");
            var spec = $('#' + txtspec).val();
            var txtmake = GetClientID("txtMake" + (parseInt(obj2) + 1)).attr("id");
            var make = $('#' + txtmake).val();
            var txtqty = GetClientID("txtQty" + (parseInt(obj2) + 1)).attr("id");
            var qty = $('#' + txtqty).val();
            var lblPNo = GetClientID("lblPartNo" + (parseInt(obj2) + 1)).attr("id");
            var PNo = $('#' + lblPNo).text();
            if (obj3 == '') {
                obj3 = 0;
            }

            if (obj3 == '' && flag == '') {
                ErrorMessage('ItemDescription is Required.');
            }
            else {
                if ((qty != '' && qty != '.')) {
                    var result = VerbalLPO_Customer.FillItemGrid(obj2, obj1, obj3, 0, 274, obj5, spec, make, qty, PNo, returnVal);
                    var getDivFEItems = GetClientID("divLPOItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);

                    if ($('#' + txtqty).val() == '' || $('#' + txtqty).val() == '0') {
                        $('#' + txtqty).focus();
                        ErrorMessage('Quantity is required.');
                    }
                    else {
                        $('#' + ddlUnit).focus();
                    }
                }
                else if (qty == '' && obj3 == 0) {
                    var result = VerbalLPO_Customer.FillItemGrid(obj2, obj1, obj3, 0, 274, obj5, spec, make, qty, PNo, returnVal);
                    var getDivFEItems = GetClientID("divLPOItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                }
                else {
                    if (qty == '' || qty == '.') {
                        var txtqty = GetClientID("txtQty" + (parseInt(obj2) + 1)).attr("id");
                        $('#' + txtqty).focus();
                        ErrorMessage('Quantity Cannot Be Empty.');
                    }
                }
                Expnder();
            }
        }

        function doConfirmm(id) {
            var ItmId = $('#hfItemID' + (parseInt(id) + 1)).val();
            //var Cst = VerbalLPO_Customer.CheckStat(ItmId, id);
            //if (Cst.value == true) {
            if (confirm("Are you sure you want to Continue?")) {
                var result = VerbalLPO_Customer.DeleteItems(ItmId);
                var getDivFEItems = GetClientID("divLPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
            }
            else {
                ErrorMessage('This Item is Used By another Transection.');
            }
            // Expnder();
            DesignGrid();
        }

        function DeleteItem(obj1) {
            var result = VerbalLPO_Customer.DeleteItem(obj1);
            var getDivFEItems = GetClientID("divLPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
            DesignGrid();
        }

        function AddItemColumn(obj1, obj2) {
            var obj3 = $('#hfItemID' + (parseInt(obj2) + 1)).val();
            var ddl1 = GetClientID("ddl" + (parseInt(obj2) + 2)).attr("id");
            var obj4 = $('#' + ddl1).val();
            if (obj3 != '') {
                if (obj3 != '' && obj3 > 0) {
                    var result = VerbalLPO_Customer.AddItemColumn(obj2 + 1, obj1, obj3);
                    var getDivFEItems = GetClientID("divLPOItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                }
                else if (obj3 != 'undefined' && obj3 == 0) {
                    alert('Please Select an Item'); $('#' + ddl1).focus();
                }
                $('[id$=ddl' + (obj1 + 1) + ']').focus();
            }
            else { alert('Please Select an Item'); $('#' + ddl).focus(); }
            Expnder();
        }

        function CancelEdit() {
            $('#btnaddItem').show();
            $('#btnEditItem').hide();
            $('#btnCancel').hide();

            var RowID = 0;
            $('#lblEditID' + RowID).text('');
            $('#ddl' + RowID).val(0);
            $('#txtPartNo' + RowID).val('');
            $('#txtSpec' + RowID).val('');
            $('#txtMake' + RowID).val('');
            $('#txtQuantity' + RowID).val('');
            $('#txtRate' + RowID).val('');
            $('#txtRemarks' + RowID).val('');
            SelectDDLunits(RowID);
        }

        function EditSelectedItem(RowID) {
            var SelID = Number(RowID) + 1;
            var ItemID = $('#hfItemID' + SelID).val();
            var result = VerbalLPO_Customer.EditItemRow(SelID, ItemID);
            var arr = result.value.split('&@&');

            RowID = 0;
            var select = document.getElementById("ddl" + RowID);
            var option = document.createElement('option');
            option.text = arr[1];
            option.value = ItemID;
            var HfVal = SelID + "," + ItemID;
            $('[id$=HFSelectedVal]').val(HfVal);
            if ($("#ddl" + RowID + " option[value=" + ItemID.trim() + "]").length == 0) {
                select.add(option, 0);
            }
            $('#btnaddItem').hide();
            $('#btnEditItem').show();
            $('#btnCancel').show();

            $('#lblEditID' + RowID).text(SelID);
            $('#ddl' + RowID).val(ItemID.trim());
            $('#txtPartNo' + RowID).val(arr[2].trim());
            $('#txtSpec' + RowID).val(arr[3]);
            $('#txtMake' + RowID).val(arr[4]);
            $('#txtQuantity' + RowID).val(arr[5].trim());
            $('#ddlUnits' + RowID).val(arr[6].trim());
            $('#txtRate' + RowID).val(arr[7].trim());
            $('#txtRemarks' + RowID).val(arr[8].trim());
        }     
    </script>
</asp:Content>
