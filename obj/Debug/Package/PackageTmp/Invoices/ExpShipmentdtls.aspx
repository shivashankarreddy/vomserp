<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ExpShipmentdtls.aspx.cs" Inherits="VOMS_ERP.Invoices.ExpShipmentdtls" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="ESD" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 100%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Export Shipment Details Report"
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
                                <table>
                                    <tr>
                                        <td class="bcTdnormal">
                                            <span class="bcLabel">Commercial Invoice No :<font color="red" size="2"><b>
                                                                    *</b></font></span>
                                        </td>
                                        <td class="bcTdnormal">
                                            <asp:DropDownList runat="server" ID="ddlCommInvNo" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlCommInvNo_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
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
                                                            <a href="#" class="href">Details</a> &nbsp;<asp:Image runat="server" ID="imgAtchmt"
                                                                AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif" Visible="false" />
                                                        </td>
                                                        <td align="center">
                                                            <asp:Label ID="lblBlink" runat="server" Text="Click here" CssClass="blinkytext"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </Header>
                                            <Content>
                                                <asp:Panel ID="Panel2" runat="server" Width="100%">
                                                    <table width="100%" style="background-color: #F5F4F4; padding: 5px;">
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span class="bcLabel">Performa Invoice No :<font color="red" size="2" ><b>
                                                                    *</b></font>:</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtPerfInvNo" CssClass="bcAsptextbox " Enabled = "false">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span5" class="bcLabel ">Performa Invoice Date <font color="red" size="2"><b>
                                                                    </b></font>:</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtdate" CssClass="bcAsptextbox DatePicker" TabIndex="6" Enabled = "false">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span7" class="bcLabel">Mode of Shipment<font color="red" size="2"><b></b></font>
                                                                    :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtModeofShpt" CssClass="bcAsptextbox" Enabled = "false">
                                                                </asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span id="Span10" class="bcLabel">FOB Value in USD<font color="red" size="2"><b></b></font>
                                                                    : </span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtFOBValUSD" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                                    onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);" Enabled = "false">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span13" class="bcLabel">Freight & Ins<font color="red" size="2"><b></b></font>
                                                                    : </span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtFrtIns" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                                    onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);" >
                                                                </asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span15" class="bcLabel">CFR/CIF Value<font color="red" size="2"><b></b></font>
                                                                    :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtCFRCIFVal" CssClass="bcAsptextbox" Enabled = "false">
                                                                </asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span id="Span8" class="bcLabel">Port of Loading :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtPrtld" CssClass="bcAsptextbox" Enabled = "false"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span9" class="bcLabel">Port of Discharge :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtPrtDis" CssClass="bcAsptextbox" Enabled = "false"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span11" class="bcLabel">No.of packages :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtnoofpcks" CssClass="bcAsptextbox" onkeypress='return isNumberKey(event);' ></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span id="Span12" class="bcLabel">NET Weight in KGS <font color="red" size="2"><b></b></font>
                                                                    : </span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtnetwgtkgs" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                                    onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);" ></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span class="bcLabel">Gross Weight in KGS :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" CssClass="bcAsptextboxRight" ID="txtGrosswgtkgs" onblur="extractNumber(this,2,false);"
                                                                    onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);" ></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span1" class="bcLabel">Shipping Bill No :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtshpbllno" CssClass="bcAsptextbox" Enabled = "false"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span id="Span16" class="bcLabel">Container No :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtContNo" CssClass="bcAsptextbox" Enabled = "false"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span17" class="bcLabel">Vessel details ETA/ETD :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtvessdetETAETD" CssClass="bcAsptextbox" Enabled = "false"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span18" class="bcLabel">Particulars of BL/AWB :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="TxtPartBLAWB" CssClass="bcAsptextbox" Enabled = "false"></asp:TextBox>
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
                            <table width="100%" style="background-color: #F5F4F4; padding: 5px; border: solid 1px #ccc">
                                <tr>
                                    <td valign="top">
                                        <span id="lblCustName" class="bcLabel">Consignee name<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td valign="top">
                                        <asp:TextBox runat="server" ID="TxtConsigneename" CssClass="bcAsptextbox" onkeypress="return isAlphabet(event)"></asp:TextBox>
                                    </td>
                                    <td valign="top">
                                        <span id="lblGRN" class="bcLabel" style="display: inline-block; width: 11em; float: left">
                                            BIVAC/Pre-Shipt INSP.Details/Status: </span>
                                    </td>
                                    <td valign="top">
                                        <asp:TextBox runat="server" ID="TxtBivac" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td valign="top">
                                        <span id="lblGDN" class="bcLabel" style="display: inline-block; width: 11em; float: left">
                                            Supplier/Cargo Description <font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td valign="top">
                                        <asp:TextBox runat="server" ID="TxtSupplier" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <span id="Span14" class="bcLabel" style="display: inline-block; width: 11em; float: left">
                                            Date of Cargo Carting at CFS <font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="TxtDateCargo" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel" style="display: inline-block; width: 11em;">CUSTS.Exam.Status
                                            /Compltn.Dt<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="Txtcuststatus" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="lblImpInstructions" class="bcLabel" style="display: inline-block; width: 13em;
                                            float: left">Container Stuffing Date:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtContStuffing" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">Particulars
                                            of ECTN/URN/ID F No.<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="ParticularsofEctnUrn" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">ECTN Request
                                            Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtEctnReqDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">ECTN Invoice
                                            Received Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtECTNInvReceviedDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">ECTN Payment
                                            Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtECTNPayStatusdate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">ECTN No.
                                            Recevied Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtECTNNoRecDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BL Payment
                                            Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="BLPayStatusDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BL Approved
                                            Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtblappDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">Bl Release
                                            Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtblrelstatusDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BL Received
                                            Date at HYD.Office<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtblrecDateHyd" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">Commercial
                                            Invoice Details<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtCommInvDet" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">Certf.of
                                            origin By FAPCCI Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtcertOrigFAPCCI" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">Status
                                            of Commercial Invoice<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtstatusCommInv" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">DOC./Details
                                            Requested From Consignee<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtDetailsreqConsignee" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BL/AWB
                                            (1st print) applied & received on<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtBlAWBapprec" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">RFI/FDI
                                            form requested on<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRFIFDIreqon" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">RFI/FDI
                                            form received on<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtRFIFDIrecon" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BIVAC/
                                            Pre-shipment inspection request on<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtBivacinsreq" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BIVAC/
                                            Pre-shipment inspection completed on<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtBivacinscompldon" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">Export
                                            Invoice courier details<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="InvCourierDet" CssClass="bcAsptextbox "></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BL Invoice
                                            Recevied Date<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="BlInvRecDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">Container
                                            Stuffing Status<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtContStuffStat" CssClass="bcAsptextbox "></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">ETCN Payment
                                            Status<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtETCNPayStat" CssClass="bcAsptextbox "></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BL Payment
                                            Status<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtBLpayStat" CssClass="bcAsptextbox "></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">BL Release
                                            Status<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtBLRelStat" CssClass="bcAsptextbox "></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <span class="bcLabel" style="display: inline-block; width: 11em; float: left">AV / CoC
                                            (certificate) forwarded to Consignee on<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtAVCOCConigon" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel">Remarks<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRemarks" CssClass="bcAsptextbox "></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
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
                                                    <asp:LinkButton runat="server" ID="btnclear" Text="Clear" OnClick="btnclear_Click" />
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(document).ready(function () {
                var dateToday = new Date();
                DatePickerAdd();
            });


            function DatePickerAdd() {
                try {
                    var dateToday = new Date();
                    $('.DatePicker').datepicker({
                        dateFormat: 'dd-MM-yy',
                        maxDate: dateToday,

                        changeMonth: true,
                        changeYear: true,
                        monthNames: ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"]
                    });
                }
                catch (Error) {
                    ErrorMessage(Error);
                }
            }
        })
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) //&& charCode != 46 
                return false;
            return true;
        }
        function isAlphabet(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function Myvalidations() {
            try {
                if (($('[id$=ddlCommInvNo]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Commercial Invoice Number is Required.');
                    $('[id$=ddlCommInvNo]').focus();
                    return false;
                } else if (($('[id$=txtPerfInvNo]').val()).trim() == '') {
                    ErrorMessage('Performa Invoice Number is Required.');
                    $('[id$=txtPerfInvNo]').focus();
                    return false;

                }
                return true;
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }

    </script>
</asp:Content>
