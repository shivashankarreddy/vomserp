<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CTOneGeneration.aspx.cs" Inherits="VOMS_ERP.CTOneGeneration" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="CT-1 Generation" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
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
            <td>
                <div style="width: 100%">
                    <asp:HiddenField ID="HselectedItems" runat="server" Value="" />
                    <asp:HiddenField ID="HFSupplierAdd" runat="server" Value="" />
                    <asp:HiddenField ID="HFhypothecation" runat="server" Value="" />
                    <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                        align="center">
                        <tr style="background-color: Gray; font-style: normal; color: White;">
                            <td colspan="6">
                                Proforma Invoice
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">IOM Ref No.<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:DropDownList runat="server" ID="ddlIOMRefNo" CssClass="bcAspdropdown" Height="22px"
                                    OnSelectedIndexChanged="ddlIOMRefNo_OnSelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:HiddenField ID="HFPrfINVID" runat="server" Value="" />
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Customer<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtCustomer" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                                <asp:HiddenField runat="server" ID="HFCustID" />
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Date<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtDate" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Supplier Category.<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlCategory" CssClass="bcAspdropdown" Enabled="false">
                                    <asp:ListItem Value="0" Text="Select Catagory"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Supplier<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:HiddenField ID="HFSuplierID" runat="server" />
                                <asp:TextBox runat="server" ID="txtSupplierNm" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Proforma Invoice <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtProfInv" CssClass="bcAsptextbox" ReadOnly="True"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">FPO <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:ListBox runat="server" ID="ListBoxFPO" mulitple="mulitple" CssClass="bcAspMultiSelectListBox">
                                </asp:ListBox>
                            </td>
                            <td>
                                <span class="bcLabel">LPO <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:ListBox runat="server" ID="ListBoxLPO" mulitple="mulitple" CssClass="bcAspMultiSelectListBox">
                                </asp:ListBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Important Instructions <font color="red" size="2"><b>*</b></font>
                                    :</span>
                            </td>
                            <td class="bcTdnormal" colspan="3">
                                <asp:TextBox ID="txtImpInstructions" runat="server" class="bcAsptextboxmulti" TextMode="MultiLine"
                                    ReadOnly="True"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="bcTdNewTable" align="center">
                                <asp:HiddenField runat="server" ID="hfGT_Sub" Value="0" />
                                <asp:HiddenField runat="server" ID="hfRT_Sub" Value="0" />
                                <asp:HiddenField runat="server" ID="hfExT_Sub" Value="0" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="bcTdNewTable">
                                <div id="DivItemDetails" runat="server" style="max-height: 250px; min-height: 200px;
                                    overflow: auto; width: 100%;">
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="bcTdNewTable">
                                <div id="DvTerms" runat="server" visible="false">
                                    <table style="width: 100%; overflow: auto;">
                                        <tr style="background-color: Gray; font-style: normal; color: White;">
                                            <td colspan="6">
                                                <b>Terms & Conditions</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span id="lblDis" class="bcLabel">Discount:</span>
                                                <asp:CheckBox runat="server" ID="chkDsnt" Text=" " onclick='CHeck("chkDsnt", "dvDsnt"); Calculate();'
                                                    CssClass="bcCheckBoxList" />
                                            </td>
                                            <td>
                                                <div id="dvDsnt" style="display: none;">
                                                    <asp:TextBox runat="server" ID="txtDsnt" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        Width="35px" MaxLength="5" onchange="Calculate();"></asp:TextBox>%</div>
                                            </td>
                                            <td>
                                                <span id="lblPck" class="bcLabel">Packing:</span>
                                                <asp:CheckBox runat="server" ID="chkPkng" Text=" " onclick='CHeck("chkPkng", "dvPkng"); Calculate();'
                                                    CssClass="bcCheckBoxList" />
                                            </td>
                                            <td>
                                                <div id="dvPkng" style="display: none;">
                                                    <asp:TextBox runat="server" ID="txtPkng" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        Width="35px" MaxLength="5" onchange="Calculate();"></asp:TextBox>%</div>
                                            </td>
                                            <td>
                                                <span id="lblED" class="bcLabel">Excise Duty:</span>
                                                <asp:CheckBox runat="server" ID="chkExdt" Text=" " onclick='CHeck("chkExdt", "dvExdt"); Calculate();'
                                                    CssClass="bcCheckBoxList" />
                                            </td>
                                            <td>
                                                <div id="dvExdt" style="display: none;">
                                                    <asp:TextBox runat="server" ID="txtExdt" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        Width="35px" MaxLength="5" onchange="Calculate();"></asp:TextBox>%</div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                        align="center">
                        <tr style="background-color: Gray; font-style: normal; color: White;">
                            <td colspan="6">
                                Ex-Details
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Central Ex-Duty Amt<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtCEDAmt" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                    onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Ex-Reg No<font color="red" size="2"><b></b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtExRegNo" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">ECC No<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtECCNo" CssClass="bcAsptextbox" onchange="CheckECC();"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Range<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtRange" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Division<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDivision" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Commissionerate<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtCommissioneRate" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Range Address<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtVRange" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Division Address<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtVDivision" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Commissionerate Address<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtVCommissionerate" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Duty Drawback S.No<font color="red" size="2"><b></b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtDutyDrawBackNo" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Material Dispatch Date :</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDispatchMdt" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Material Gatepass & Invoice<font color="red" size="2"><b></b></font>:</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtMaterlGatePass" runat="server" Height="60px" class="bcAsptextboxmulti"
                                    Style="width: 180px;" TextMode="MultiLine"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Ex-Tariff Heading No's<font color="red" size="2"><b> </b></font>
                                    :</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtHeadingNumbers" runat="server" Height="60px" class="bcAsptextboxmulti"
                                    Style="width: 180px;" TextMode="MultiLine"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel"><span lang="EN-US">Mfr. Factory Address</span><font color="red"
                                    size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtFactryAdd" Height="60px" class="bcAsptextboxmulti"
                                    Style="width: 180px;" TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Hypothecation<font color="red" size="2"><b></b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:DropDownList runat="server" ID="ddlHypothecation" CssClass="bcAspdropdown" Height="22px"
                                    OnSelectedIndexChanged="ddlHypothecation_OnSelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Mfr. Units<font color="red" size="2"><b></b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:DropDownList runat="server" ID="ddlUnits" CssClass="bcAspdropdown" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlUnits_SelectedIndexChanged">
                                    <asp:ListItem Text="Select Unit" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">CT-1 Draft Ref. No. <font color="red" size="2"><b></b></font>:</span>
                            </td>
                            <td class="bcTdnormal" style="width: 215px">
                                <asp:TextBox runat="server" ID="txtCT1DrftRefNo" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Location<font color="red" size="2"><b>*</b></font> :</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:DropDownList runat="server" ID="ddlLocation" CssClass="bcAspdropdown" Height="22px">
                                    <asp:ListItem Value="0" Text="Select Location"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td colspan="2" class="bcTdnormal">
                                <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                    <table width="100%">
                                        <tr>
                                            <td align="right" style="width: 39%">
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
                    </table>
                    <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc;
                        display: none;" align="center">
                        <tr style="background-color: Gray; font-style: normal; color: White;">
                            <td colspan="6">
                                CT-1
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Bond Balance Value :</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtBondBalance" MaxLength="12" CssClass="bcAsptextbox"
                                    onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                            </td>
                            <td>
                                <span class="bcLabel">Internal Ref. No.<font color="red" size="2"><b>*</b></font> :</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtInternalRefNo" CssClass="bcAsptextbox" onchange="GetCTOneInternalRefNo()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">CT-1 value<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal" style="width: 215px">
                                <asp:TextBox runat="server" ID="txtCtOneVal" CssClass="bcAsptextbox" onkeyup="extractNumber(this,2,false);"
                                    MaxLength="12" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Ref No:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtRefNo" CssClass="bcAsptextbox" MaxLength="180"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Ref Date:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtRefDate" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">No.Of ARE Forms:</span>
                            </td>
                            <td class="bcTdnormal" style="width: 215px">
                                <asp:TextBox runat="server" ID="txtNoOfAREForms" CssClass="bcAsptextbox" onblur="extractNumber(this,0,false);"
                                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Sevottom Ref.No.:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtSevRefNo" CssClass="bcAsptextbox"></asp:TextBox>
                            </td>
                            <td colspan="2" class="bcTdnormal">
                            </td>
                        </tr>
                    </table>
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
            <td>
                <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                    align="center">
                    <tr>
                        <td class="bcTdNewTable" colspan="6">
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="bcTdNewTable" colspan="6">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
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
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        function CalculateMax(CtrlID) {
            if ($('[id$=' + CtrlID + ']').val() > 99.99) {
                ErrorMessage('Value Cannot be greater than 99.99%');
                $('[id$=' + CtrlID + ']').val('0');
                $('[id$=' + CtrlID + ']').focus();
            }
        }


        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });        
    </script>
    <script type="text/javascript">
        function Close_Supplier(msg) {
            alert(msg);
            window.location = "../Masters/Home.aspx?NP=no";
        }

        $(window).unload(function () { alert("Bye now!"); });

        function GetCTOneInternalRefNo() {
            var refNo = $('[id$=txtInternalRefNo]').val();
            var result = CTOneGeneration.GetCTOneInternalRefNo(refNo);
            if (result.value == false) {
                $('[id$=txtInternalRefNo]').val('');
                $('[id$=txtInternalRefNo]').focus();
                ErrorMessage('This Internal Reference No. is already in Use');
            }
        }

        function CheckECC() {
            var refNo = $('[id$=txtECCNo]').val();
            var result = CTOneGeneration.CheckECC(refNo);
            if (result.value == false) {
                $('[id$=txtECCNo]').val('');
                $('[id$=txtECCNo]').focus();
                ErrorMessage('This ECC-No. is already in Use');
            }
        }

        function CheckQty(CtrlID) {
            var refNo = $('[id$=' + CtrlID + ']').val();
            var result = CTOneGeneration.GetCTOneInternalRefNo(refNo);
            if (result.value == false) {
                $('[id$=txtInternalRefNo]').val('');
                $('[id$=txtInternalRefNo]').focus();
                ErrorMessage('This Internal Reference No. is already in Use');
            }
        }

        $('[id$=txtNoOfAREForms]').keyup(function (e) {
            var AreForms = parseInt($('[id$=txtNoOfAREForms]').val());
            if (AreForms > 5) {
                $('[id$=txtNoOfAREForms]').val('');
                ErrorMessage('ARE Forms cannot be greater than 5.');
            }
        });


        function ClearAll() {
            this.form.reset();
            return false;
        }

        var dateToday = new Date();
        $('[id$=txtRefDate]').datepicker({
            maxDate: dateToday,
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true,
            maxDate: dateToday
        });

        $('[id$=txtDispatchMdt]').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        });

        $('[id$=txtDsnt]').keyup(function (e) {
            var discount = $('[id$=txtDsnt]').val();
            if (discount > 99.99) {
                ErrorMessage('Discount Cannot be Grater than 99.99%');
                $('[id$=txtDsnt]').val('');
                return false;
            }
        });
        $('[id$=txtDsnt]').blur(function (e) {
            var discount = $('[id$=txtDsnt]').val();
            if (discount == '') {
                $('[id$=txtDsnt]').focus();
                ErrorMessage('Discount Cannot be Empty');
                $('[id$=txtDsnt]').val('');
                return false;
            }
        });
        //txtExdt
        $('[id$=txtExdt]').keyup(function (e) {
            var discount = $('[id$=txtExdt]').val();
            if (discount > 99.99) {
                ErrorMessage('Excise Duty Cannot be Grater than 99.99%');
                $('[id$=txtExdt]').val('');
                return false;
            }
        });
        $('[id$=txtExdt]').blur(function (e) {
            if ($('[id$=txtExdt]').val() == '') {
                $('[id$=txtExdt]').val('');
                $('[id$=txtExdt]').focus();
                ErrorMessage('Excise Duty Cannot be Empty');
                return false;
            }
        });

        $('[id$=txtPkng]').keyup(function (e) {
            var discount = $('[id$=txtPkng]').val();
            if (discount > 99.99) {
                ErrorMessage('Packing Cannot be Grater than 99.99%');
                $('[id$=txtPkng]').val('');
                return false;
            }
        });
        $('[id$=txtPkng]').blur(function (e) {
            if ($('[id$=txtPkng]').val() == '') {
                $('[id$=txtPkng]').focus();
                ErrorMessage('Packing Cannot be Empty');
                $('[id$=txtPkng]').val('');
                return false;
            }
        });

        function uploadComplete() {
            var result = CTOneGeneration.AddItemListBox();
            var getDivctoneItems = GetClientID("divListBox").attr("id");
            $('#' + getDivctoneItems).html(result.value);
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
                    var result = CTOneGeneration.DeleteItemListBox($('#lbItems').val());
                    var getDivctoneItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivctoneItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
        });
        
    </script>
    <script type="text/javascript">
        function ExpandTXT(obj) {
            $('#txtDesc-Spec' + '' + obj).animate({ "height": "75px" }, "slow");
            $('#txtDesc-Spec' + '' + obj).slideDown("slow");
        }

        function ReSizeTXT(obj) {
            $('#txtDesc-Spec' + '' + obj).animate({ "height": "20px" }, "slow");
            $('#txtDesc-Spec' + '' + obj).slideDown("slow");
        }

        function ExpandTXTt(obj) {
            $('#txtDesc-Spec' + obj + 'a' + obj).animate({ "height": "75px" }, "slow");
            $('#txtDesc-Spec' + obj + 'a' + obj).slideDown("slow");
        }

        function ReSizeTXTt(obj) {
            $('#txtDesc-Spec' + obj + 'a' + obj).animate({ "height": "20px" }, "slow");
            $('#txtDesc-Spec' + obj + 'a' + obj).slideDown("slow");
        }

        $(document).ready(function () {
            $('div.expanderR').expander();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }

        $(".fnOpen").live('click', function () {
            var ClickRID = $(this).closest('tr').attr('id');
            var aray = ClickRID.split('a');
            var RowIndex = $(this).closest('td').parent()[0].sectionRowIndex;
            var res = fnOpen1(aray[0], aray[1], ClickRID);
            if (res != "")
                Add_Sub_Itms1(aray[0], aray[1], ClickRID, RowIndex, false);
        });

        function fnOpen1(TRID, RowID, ClickRowID) {
            returnVal = window.showModalDialog("../Enquiries/AddItems.aspx", "Add Item",
            "dialogHeight:680px; dialogWidth:980px; dialogLeft:150; dialogright:150; dialogTop:150; ");
            if (returnVal != null) {
                var rtnVal = returnVal.split(',');
                if (rtnVal[1].trim() == "") {
                    $('#hfSubItemID' + ClickRowID).val(rtnVal[0]);
                    GetItemDesc_Spec(rtnVal[0], ClickRowID);
                }
                else {
                    $('#hfSubItemID' + ClickRowID).val(rtnVal[0]);
                    GetItemDesc_Spec(rtnVal[0], ClickRowID);
                }
                returnVal = "";
            }
            else
                returnVal = "";
            return returnVal;
        }

        function GetItemDesc_Spec(ItmID, id) {
            var rslt = CTOneGeneration.GetItemDesc_Spec(ItmID);
            $("[id$=txtDesc-Spec" + id + "]").val(rslt.value);
        }

        function Calculate() {
            var DscntAll = $('[id$=txtDsnt]').val();
            var PkngAll = $('[id$=txtPkng]').val();
            var ExseAll = $('[id$=txtExdt]').val();
            var ChkDsnt = $('[id$=chkDsnt]').is(':checked');
            var ChkPkng = $('[id$=chkPkng]').is(':checked');
            var ChkExse = $('[id$=chkExdt]').is(':checked');

            if (ChkDsnt) {
                if (DscntAll == '') {
                    DscntAll = 0;
                    $('[id$=txtDsnt]').val('0')
                }
            }
            else {
                DscntAll = 0;
                $('[id$=txtDsnt]').val('0')
            }
            if (ChkPkng) {
                if (PkngAll == '') {
                    PkngAll = 0;
                }
            }
            else {
                PkngAll = 0;
                $('[id$=txtPkng]').val('0');
            }
            if (ChkExse) {
                if (ExseAll == '') {
                    ExseAll = 0;
                }
            }
            else {
                ExseAll = 0;
                $('[id$=txtExdt]').val('0');
            }
            var result;
            if (DscntAll == '' || PkngAll == '' || ExseAll == '' || ChkDsnt || ChkPkng || ChkExse) {
                result = CTOneGeneration.Calculations(ExseAll, DscntAll, PkngAll, ChkDsnt, ChkExse, ChkPkng);
                var getDivLQItems = GetClientID("DivItemDetails").attr("id");
                $('#' + getDivLQItems).html(result.value);
                footerValues();
                CheckUncheckAll();
            }
        }

        function UpdateItems(RNo) {
            var HDFNSNo = GetClientID("HFESNo" + (parseInt(RNo))).attr("id");
            var SNo = $('#' + HDFNSNo).val();
            var spec = GetClientID("txtDesc-Spec" + (parseInt(RNo))).attr("id");
            var txtSpec = $('#' + spec).val().replace('"', '#@%');
            var qty = GetClientID("txtQuantity" + (parseInt(RNo))).attr("id");
            var txtqty = $('#' + qty).val();
            var Dscnt = GetClientID("txtDscnt" + (parseInt(RNo))).attr("id");
            var txtDscnt = $('#' + Dscnt).val();
            var ExDuty = GetClientID("txtExDuty" + (parseInt(RNo))).attr("id");
            var txtExDuty = $('#' + ExDuty).val();
            var PackingPercent = GetClientID("txtPkingPercentage" + (parseInt(RNo))).attr("id");
            var txtPckngPrcnt = $('#' + PackingPercent).val();
            var HSCode = GetClientID("txtHSCode" + (parseInt(RNo))).attr("id");
            var txtHSCode = $('#' + HSCode).val();
            var DscntAll = $('[id$=txtDsnt]').val();
            var PkngAll = $('[id$=txtPkng]').val();
            var ExseAll = $('[id$=txtExdt]').val();
            var ItemStatus = GetClientID("ddlStatus" + (parseInt(RNo))).length > 0 ? GetClientID("ddlStatus" + (parseInt(RNo))).attr("id") : "";
            if (ItemStatus != "")
                var ddlStatus = $('#' + ItemStatus).val();
            else
                var ddlStatus = "";
            var ChkDsnt = $('[id$=chkDsnt]').is(':checked');
            var ChkPkng = $('[id$=chkPkng]').is(':checked');
            var ChkExse = $('[id$=chkExdt]').is(':checked');
            if (txtSpec == '' || txtqty == '' || txtExDuty == '' || txtPckngPrcnt == '' || txtDscnt == '' || txtHSCode == '') {
                if (txtSpec == '') {
                    $('[id$=txtDesc-Spec' + RNo + ']').focus();
                    ErrorMessage('Specification Cannot be Empty.');
                }
                else if (txtqty == '') {
                    $('[id$=txtQuantity' + RNo + ']').focus();
                    $('[id$=txtQuantity' + RNo + ']').val(0);
                    ErrorMessage('Quantity Cannot be Empty.');
                }
                else if (txtDscnt == '') {
                    $('[id$=txtDscnt' + RNo + ']').focus();
                    $('[id$=txtDscnt' + RNo + ']').val(0);
                    ErrorMessage('Discount Cannot be Empty.');
                }
                else if (txtExDuty == '') {
                    $('[id$=txtExDuty' + RNo + ']').focus();
                    $('[id$=txtExDuty' + RNo + ']').val(0);
                    ErrorMessage('Excise Duty Cannot be Empty.');
                }
                else if (txtPckngPrcnt == '') {
                    $('[id$=txtPkingPercentage' + RNo + ']').focus();
                    $('[id$=txtPkingPercentage' + RNo + ']').val(0);
                    ErrorMessage('Packing Percent Cannot be Empty.');
                }
                else if (txtHSCode == '') {
                    $('[id$=txtHSCode' + RNo + ']').focus();
                    ErrorMessage('HSCode Cannot be Empty.');
                }

            }
            else {
                if (DscntAll == '' || PkngAll == '' || ExseAll == '' || ChkDsnt || ChkPkng || ChkExse) {
                    if (ChkDsnt) {
                        if (DscntAll == '') {
                            DscntAll = 0;
                        }
                    }
                    else {
                        DscntAll = 0;
                    }
                    if (ChkPkng) {
                        if (PkngAll == '') {
                            PkngAll = 0;
                        }
                    }
                    else {
                        PkngAll = 0;
                    }
                    if (ChkExse) {
                        if (ExseAll == '') {
                            ExseAll = 0;
                        }
                    }
                    else {
                        ExseAll = 0;
                    }

                }
                var result;
                if (ChkDsnt || ChkPkng || ChkExse) {
                    result = CTOneGeneration.SaveChanges(RNo, SNo, txtSpec, txtqty, txtExDuty, txtPckngPrcnt, txtDscnt, txtHSCode, ddlStatus);
                    result = CTOneGeneration.Calculations(ExseAll, DscntAll, PkngAll, ChkDsnt, ChkExse, ChkPkng);
                }
                else {
                    result = CTOneGeneration.SaveChanges(RNo, SNo, txtSpec, txtqty, txtExDuty, txtPckngPrcnt, txtDscnt, txtHSCode, ddlStatus);
                }
                var getDivLQItems = GetClientID("DivItemDetails").attr("id");
                $('#' + getDivLQItems).html(result.value);

                if ($('[id$=hdfErrMsg]').val() != '')
                    ErrorMessage($('[id$=hdfErrMsg]').val());

                footerValues();
                CheckUncheckAll();

            }
            calculate_Footer();
        }

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
                var getDivFEItems = GetClientID("DivItemDetails").attr("id");
                var result = CTOneGeneration.NextPage(hfCurrentPage, ddlRowsChanged);
                $('#' + getDivFEItems).html(result.value);
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
                var result = CTOneGeneration.PrevPage(hfCurrentPage, ddlRowsChanged);
                var getDivFEItems = GetClientID("DivItemDetails").attr("id");
                $('#' + getDivFEItems).html(result.value);
            }
        };

        function RowsChanged() {
            var hfCurrentPage = $('[id$=hfCurrentPage]').val();
            var ddlRowsChanged = $('[id$=ddlRowsChanged]').val();
            var result = CTOneGeneration.RowsChanged(hfCurrentPage, ddlRowsChanged);
            var getDivFEItems = GetClientID("DivItemDetails").attr("id");
            $('#' + getDivFEItems).html(result.value);
        }

        function CheckUncheckAll() {
            if ($('[id$=hdfDscntAll]').val() == 'False') {
                $('[id$=chkDsnt]')[0].checked = false;
                $('[id$=txtDsnt]').val('0');
                CHeck('chkDsnt', 'dvDsnt');
                $('[id$=chkDsnt]').attr("disabled", true);
            } else {
                $('[id$=chkDsnt]').removeAttr("disabled"); //chkDsnt
                CHeck('chkDsnt', 'dvDsnt');
            }
            if ($('[id$=hdfExseAll]').val() == 'False') {
                $('[id$=chkExdt]')[0].checked = false;
                $('[id$=txtExdt]').val('0');
                $('[id$=chkExdt]').attr("disabled", true);
                CHeck('chkExdt', 'dvExdt');
            } else {
                $('[id$=chkExdt]').removeAttr("disabled"); //chkDsnt
                CHeck('chkExdt', 'dvExdt');
            }
            if ($('[id$=hdfPkngAll]').val() == 'False') {
                $('[id$=chkPkng]')[0].checked = false;
                $('[id$=txtPkng]').val('0');
                $('[id$=chkPkng]').attr("disabled", true);
                CHeck('chkPkng', 'dvPkng');
            } else {
                $('[id$=chkPkng]').removeAttr("disabled"); //chkDsnt
                CHeck('chkPkng', 'dvPkng');
            }

            if ($('#hdfDscntAll_Sub').length > 0) {
                if ($('[id$=hdfDscntAll_Sub]').val() == 'False') {
                    $('[id$=chkDsnt]')[0].checked = false;
                    $('[id$=txtDsnt]').val('0');
                    CHeck('chkDsnt', 'dvDsnt');
                    $('[id$=chkDsnt]').attr("disabled", true);
                } else {
                    $('[id$=chkDsnt]').removeAttr("disabled"); //chkDsnt
                    CHeck('chkDsnt', 'dvDsnt');
                }
            }
            if ($('#hdfExseAll_Sub').length > 0) {
                if ($('[id$=hdfExseAll_Sub]').val() == 'False') {
                    $('[id$=chkExdt]')[0].checked = false;
                    $('[id$=txtExdt]').val('0');
                    $('[id$=chkExdt]').attr("disabled", true);
                    CHeck('chkExdt', 'dvExdt');
                } else {
                    $('[id$=chkExdt]').removeAttr("disabled"); //chkDsnt
                    CHeck('chkExdt', 'dvExdt');
                }
            }
            if ($('#hdfPkngAll_Sub').length > 0) {
                if ($('[id$=hdfPkngAll_Sub]').val() == 'False') {
                    $('[id$=chkPkng]')[0].checked = false;
                    $('[id$=txtPkng]').val('0');
                    $('[id$=chkPkng]').attr("disabled", true);
                    CHeck('chkPkng', 'dvPkng');
                } else {
                    $('[id$=chkPkng]').removeAttr("disabled"); //chkDsnt
                    CHeck('chkPkng', 'dvPkng');
                }
            }
        }

        function footerValues() {
            var examount = 0;
            examount = $('[id$=hdfExDutyAmt]').val();
            $('[id$=txtCtOneVal]').val(examount);
            $('[id$=txtCEDAmt]').val(examount);
            $('[id$=txtHeadingNumbers]').val($('[id$=hdfHSCode]').val());
        }

        function CheckChaild(RNo) {
            var ChkStatus = false;
            if ($('[id$=ckhChaild' + RNo + ']').is(':checked')) {
                ChkStatus = true;
            }

            var result = CTOneGeneration.ChkBoxChecked(RNo, ChkStatus);
            var getDivLQItems = GetClientID("DivItemDetails").attr("id");
            $('#' + getDivLQItems).html(result.value);
            footerValues();

        }

        function CheckHeader() {
            var ChkStatus = false;
            if ($('[id$=ckhHeader]').is(':checked')) {
                ChkStatus = true;
            }
            var result = CTOneGeneration.CheckHeader(ChkStatus);
            var getDivLQItems = GetClientID("DivItemDetails").attr("id");
            $('#' + getDivLQItems).html(result.value);
            footerValues();
        }     
    </script>
    <script type="text/javascript">

        function Myvalidations() {

            var rowCount = $('#rounded-corner tbody tr').length;
            var count = 0;
            var notSelected = "";
            var ExDutyPercent = $('[id$=txtExdt]').val();
            if (ExDutyPercent == undefined)
                ExDutyPercent = 0;
            for (var i = 1; i <= rowCount; i++) {

                var qty = $('#txtQuantity' + '' + i).val();
                var Hid = $('#HItmID' + '' + i).val(); //HItmID
                var ExDuty = $('#txtExDuty' + '' + i).val();
                var HsCode = $('#txtHSCode' + '' + i).val();

                var isChecked = $('#ckhChaild' + i).is(':checked');
                if (!isChecked) {
                    notSelected += ',' + Hid;
                    count = count + 1;
                    $('[id$=HselectedItems]').val(notSelected);
                }
                if (isChecked) {

                    if (qty == 0 || qty == '' || (ExDuty == 0 && ExDutyPercent == 0) || HsCode == '') {
                        var message = '';
                        if (HsCode == 0) {
                            message = 'HSCode Cannot Be Empty in Row : ' + i;
                            $('#txtHSCode' + '' + i).focus();
                        }
                        else if (qty == '') {
                            message = 'Quantity is Required in Row : ' + i;
                            $('#txtQuantity' + '' + i).focus();
                        }
                        else if (qty == '0') {
                            message = 'Quantity Cannot Be Zero in Row : ' + i;
                            $('#txtQuantity' + '' + i).focus();
                        }
                        else if (ExDuty == 0 && ExDutyPercent == 0) {
                            message = 'If CheckBox is checked Ex-Duty has to be entered either in that Row : ' + i + ' or in Terms & Conditions.';
                            $('#txtExDuty' + '' + i).focus();
                        }
                        ErrorMessage(message);
                        return false;
                        break;
                    }
                }
            }

            if (rowCount > 0) {
                var result = NewFPOrder.ValidateRowsBeforeSave();
                if (result.value.includes("ERROR::")) {
                    ErrorMessage(result.value.replace("ERROR::", ""));
                    return false;
                }
            }
            if (($('[id$=ddlIOMRefNo]').val()).trim() == '00000000-0000-0000-0000-000000000000' && ($('[id$=HFPrfINVID]').val()).trim() == '') {
                ErrorMessage('Please Select IOM Ref No./Proforma INVOICE ID');
                return false;
            }
            else if (($('[id$=txtCustomer]').val()).trim() == '') {
                $('[id$=txtCustomer]').focus();
                ErrorMessage('Customer is Required.');
                return false;
            }
            else if (($('[id$=txtDate]').val()).trim() == '') {
                $('[id$=txtDate]').focus();
                ErrorMessage('Date is Required.');
                return false;
            }
            else if (($('[id$=txtSupplierNm]').val()).trim() == '') {
                $('[id$=txtSupplierNm]').focus();
                ErrorMessage('Supplier Name is Required.');
                return false;
            }
            else if (($('[id$=txtCEDAmt]').val()).trim() == 0) {
                $('[id$=txtCEDAmt]').focus();
                ErrorMessage('Central Excise Duty Amount is Required.');
                return false;
            }

            else if (($('[id$=txtECCNo]').val()).trim() == '') {
                $('[id$=txtECCNo]').focus();
                ErrorMessage('ECC No is Required.');
                return false;
            }
            else if (($('[id$=txtRange]').val()).trim() == '') {
                $('[id$=txtRange]').focus();
                ErrorMessage('Range is Required.');
                return false;
            }
            else if (($('[id$=txtDivision]').val()).trim() == '') {
                $('[id$=txtDivision]').focus();
                ErrorMessage('Division is Required.');
                return false;
            }
            else if (($('[id$=txtCommissioneRate]').val()).trim() == '') {
                $('[id$=txtCommissioneRate]').focus();
                ErrorMessage('Commissionerate is Required.');
                return false;
            }
            else if (($('[id$=txtVRange]').val()).trim() == '') {
                $('[id$=txtVRange]').focus();
                ErrorMessage('Range Address is Required.');
                return false;
            }
            else if (($('[id$=txtVDivision]').val()).trim() == '') {
                $('[id$=txtVDivision]').focus();
                ErrorMessage('Division Address is Required.');
                return false;
            }
            else if (($('[id$=txtVCommissionerate]').val()).trim() == '') {
                $('[id$=txtVCommissionerate]').focus();
                ErrorMessage('Commissionerate Address is Required.');
                return false;
            }
            //            else if (($('[id$=txtDispatchMdt]').val()).trim() == '') {
            //                $('[id$=txtDispatchMdt]').focus();
            //                ErrorMessage(' Material Dispatch Date is Required.');
            //                return false;
            //            }

            else if (($('[id$=txtHeadingNumbers]').val()).trim() == '') {
                $('[id$=txtHeadingNumbers]').focus();
                ErrorMessage('Ex-Tariff Heading Nos is Required.');
                return false;
            }
            else if (($('[id$=txtFactryAdd]').val()).trim() == '') {
                $('[id$=txtFactryAdd]').focus();
                ErrorMessage('Manufacturing Factory Address is Required.');
                return false;
            }
            else if (($('[id$=ddlLocation]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $('[id$=ddlLocation]').focus();
                ErrorMessage('Please Select Location');
                return false;
            }
            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Comment is Required.</span>');
                    $('[id$=txtComments]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
            }
            if (rowCount == 0) {
                ErrorMessage('There are No Items.');
                return false;
            }
            else if (rowCount == count) {
                ErrorMessage('Minimum One Item Should be selected.');
                return false;
            }
            else {
                var result = CTOneGeneration.Validate_SubItem_BeforeSaving();
                if (result.value != "") {
                    ErrorMessage(result.value);
                    return false;
                }
                else
                    return true;
            }
        }
    </script>
    <script type="text/javascript">

        $('[src*=expand]').live('click', function (e) {
            e.preventDefault();
            var TRid = $(this).attr('id');
            var LPOID = $("[id$=HFLPOID" + TRid + "]").val();
            var ItemID = $("[id$=HFItemID" + TRid + "]").val();
            if ($('[id$=ckhChaild' + TRid + ']').is(':checked')) {
                var NewTbl = CTOneGeneration.Expand_LPOs(LPOID, ItemID, TRid);

                $(this).closest("tr").after(NewTbl.value);

                $(this).attr("src", "../images/collapse.png");
                $("[id$=btnExpand" + TRid + "]").prop('title', 'Collapse');
            }
            else {
                ErrorMessage('To expand you need to check this item.');
            }
            $('[id$=hfGT_Sub]').val($('#hfGT_sub').val());
            $('[id$=hfRT_Sub]').val($('#hfRateT_sub').val());
            $('[id$=hfExT_Sub]').val($('#hfExDtT_sub').val());
            calculate_Footer();
        });
        $('[src*=collapse]').live('click', function (e) {
            e.preventDefault();
            var TRid = $(this).attr('id');
            $(this).attr("src", "../images/expand.png");

            $(".DEL" + TRid).remove();

            $("[id$=btnExpand" + TRid + "]").prop('title', 'Expand');
        });

        function savechanges1(TrID, SNo, x) {
            var ClickRID = TrID + 'a' + SNo;
            var RowIndex = $(x).closest('td').parent()[0].rowIndex - 1;
            Add_Sub_Itms1(TrID, SNo, ClickRID, RowIndex, false);
        }

        $(".addrow").live('click', function () {

            var RowIndex = $(this).closest('td').parent()[0].sectionRowIndex;
            var ClickRID = $(this).closest('tr').attr('id');
            var aray = ClickRID.split('a');
            Add_Sub_Itms1(aray[0], aray[1], ClickRID, RowIndex, true);
        });

        function Add_Sub_Itms1(TrID, RowID, ClickRID, RowIndex, IsAdd) {

            var FESNo = $('#HFESNo' + TrID).val();
            var LPOID = $('#HFLPOID' + TrID).val();
            var ItmDetailsID = $('#HItmID' + TrID).val();
            var ParentItemID = $('#HFItemID' + TrID).val();
            var SubRowID = $('#lblSubSNo' + ClickRID).text();
            var ItemID = $('#hfSubItemID' + ClickRID).val();
            var LPONo = "";
            var ItmDesc = $('#txtDesc-Spec' + ClickRID).val();
            var HSCode = $('#txtHSCode' + ClickRID).val();
            var Qty = $('#txtQuantity' + ClickRID).val();
            var Rate = $('#txtRate' + ClickRID).val();
            var Disc = $('#txtDscnt' + ClickRID).val();
            var Pkng = $('#txtPking' + ClickRID).val();
            var exduty = $('#txtExDuty' + ClickRID).val();
            var UnitsID = $('#ddlUnits' + ClickRID).val();
            var DiscPercent = $('[id$=txtDsnt]').val();
            var PkngPercent = $('[id$=txtPkng]').val();
            var ExDutyPercent = $('[id$=txtExdt]').val();

            var DscntAll = $('[id$=txtDsnt]').val();
            var PkngAll = $('[id$=txtPkng]').val();
            var ExseAll = $('[id$=txtExdt]').val();
            var ChkDsnt = $('[id$=chkDsnt]').is(':checked');
            var ChkPkng = $('[id$=chkPkng]').is(':checked');
            var ChkExse = $('[id$=chkExdt]').is(':checked');

            if (DscntAll == '' || PkngAll == '' || ExseAll == '' || ChkDsnt || ChkPkng || ChkExse) {
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
            }

            if (IsAdd != true)
                IsAdd = false;
            if (IsAdd == true) {
                if (ItemID.trim() == "" || ItemID.trim() == 0) {
                    ErrorMessage('Enter Item to add new row.');
                    return false;
                }
                else if (ItmDesc.trim() == "") {
                    ErrorMessage('Item Description is required.');
                    return false;
                }
                else if (HSCode.trim() == "") {
                    ErrorMessage('Hs-Code is required.');
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
                else if (Pkng.trim() == "") {
                    ErrorMessage('Item Packing cannot be empty.');
                    return false;
                }
                else if (exduty.trim() == "") {
                    ErrorMessage('Item Ex-Duty cannot be empty.');
                    return false;
                }
                else if (UnitsID.trim() == "" || UnitsID.trim() == 0) {
                    ErrorMessage('Item Unit is required.');
                    return false;
                }
                else {
                    var res;
                    res = CTOneGeneration.Add_Sub_Itms(RowID, TrID, SubRowID, LPOID, ParentItemID, ItemID, FESNo, LPONo, ItmDetailsID, ItmDesc, HSCode, Qty, Rate, Disc, Pkng, exduty, UnitsID, DiscPercent, PkngPercent, ExDutyPercent, IsAdd);
                    if (ChkDsnt || ChkPkng || ChkExse) {
                        res = CTOneGeneration.Calculations_Sub(ExseAll, DscntAll, PkngAll, ChkDsnt, ChkExse, ChkPkng, LPOID, LPONo, ParentItemID, TrID, IsAdd);
                    }
                    $(".DEL" + TrID).remove();
                    $('#rounded-corner > tbody > tr').eq((RowIndex - RowID)).after(res.value);
                }
            }
            else {
                var res;
                res = CTOneGeneration.Add_Sub_Itms(RowID, TrID, SubRowID, LPOID, ParentItemID, ItemID, FESNo, LPONo, ItmDetailsID, ItmDesc, HSCode, Qty, Rate, Disc, Pkng, exduty, UnitsID, DiscPercent, PkngPercent, ExDutyPercent, IsAdd);
                if (ChkDsnt || ChkPkng || ChkExse) {
                    res = CTOneGeneration.Calculations_Sub(ExseAll, DscntAll, PkngAll, ChkDsnt, ChkExse, ChkPkng, LPOID, LPONo, ParentItemID, TrID, IsAdd);
                }
                $(".DEL" + TrID).remove();
                $('#rounded-corner > tbody > tr').eq((RowIndex - RowID)).after(res.value);
            }
            calculate_Footer();

            CheckUncheckAll();
        }

        function calculate_Footer() {
            if ($('#hfGT_sub').length > 0 && $('#hfRateT_sub').length > 0 && $('#hfExDtT_sub').length > 0) {
                $('[id$=hfGT_Sub]').val($('#hfGT_sub').val());
                $('[id$=hfRT_Sub]').val($('#hfRateT_sub').val());
                $('[id$=hfExT_Sub]').val($('#hfExDtT_sub').val());
            }

            var FgT = parseFloat($('#hfGt_Foot').val());
            var SubgT = parseFloat($('[id$=hfGT_Sub]').val());
            $('#lblGTotal').text(parseFloat(FgT + SubgT).toFixed(2));

            var ExDutyAmt = parseFloat($('#hdfExDutyAmt').val());
            var SubExDutyAmt = parseFloat($('[id$=hfExT_Sub]').val());
            $('#lblExDuty').text(parseFloat(ExDutyAmt + SubExDutyAmt).toFixed(2));

            var FRateT = parseFloat($('#hfTotalRate_Foot').val());
            var SubRate = parseFloat($('[id$=hfRT_Sub]').val());
            $('#lblRateTotal').text(parseFloat(FRateT + SubRate).toFixed(2));

        }

        function Delete_SubItem(TrID, SNo, x) {
            if (confirm("Are you sure you want to Delete?")) {
                var ClickRID = $(x).closest('tr').attr('id');
                var RowIndex = $(x).closest('td').parent()[0].rowIndex - 1;
                var LPOID = $('#HFLPOID' + TrID).val();
                var ParentItemID = $('#HFItemID' + TrID).val();
                var ItemID = $('#hfSubItemID' + ClickRID).val();
                var res = CTOneGeneration.Delete_SubItem(LPOID, ParentItemID, TrID, SNo, ItemID);
                $(".DEL" + TrID).remove();
                $('#rounded-corner > tbody > tr').eq((RowIndex - SNo)).after(res.value);
                calculate_Footer();
                CheckUncheckAll();
            }
        }
    </script>
</asp:Content>
