<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ShpngBilDtls.aspx.cs" Inherits="VOMS_ERP.Invoices.ShpngBilDtls" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Shipping Bill Details"
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
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Proforma Invoice No. <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPrfmaInvcNo" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required" autofocus="autofocus" AutoPostBack="true" OnSelectedIndexChanged="ddlPrfmaInvcNo_SelectedIndexChanged">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">LEO No.:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtLeoNo" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="4" MaxLength="50" required="required"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">LEO Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtLeoDt" CssClass="bcAsptextbox DatePicker" TabIndex="6"
                                            MaxLength="12" required="required"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Shipping Bill No. <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtShpngBlNo" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" onchange="javascript:return CheckSHPBNo();" TabIndex="8" MaxLength="50" required="required"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Shipping Bill Date <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtShpngBlDt" CssClass="bcAsptextbox DatePicker"
                                            TabIndex="10" MaxLength="12" required="required"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">CHA :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlChaMstr" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">State of Origin :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtStOrgn" onkeypress='return isNoSplChar(event);'
                                            CssClass="bcAsptextbox" TabIndex="14" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="bcLabelright">SB Status :</span>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList runat="server" ID="rbtnepcpy" TabIndex="138" RepeatDirection="Horizontal">
                                            <%--AutoPostBack="true" OnSelectedIndexChanged="rbtnDBK_SelectedIndexChanged1"--%>
                                            <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td>
                                        <span class="bcLabelright">ARE Status :</span>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList runat="server" ID="rbtnArests" TabIndex="138" RepeatDirection="Horizontal">
                                            <%--AutoPostBack="true" OnSelectedIndexChanged="rbtnDBK_SelectedIndexChanged1"--%>
                                            <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="UserAccordion" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false" TabIndex="16">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane3" runat="server">
                                                        <Header>
                                                            <a href="#" class="href">Factory Sealed</a>
                                                        </Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel2" runat="server" Width="100%">
                                                                <table style="width: 100%;">
                                                                    <tr style="background-color: Gray; font-style: normal; color: White;">
                                                                        <td colspan="6">
                                                                            <b>&nbsp;&nbsp;Address of Stuffing</b>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">LoFSP No. :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" onkeypress='return isSomeSplChars(event);' ID="txtLofspNo"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">LoFSP Date :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtLofspDt" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">Date of Stuffing :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtStfngDt" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">File No. :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" onkeypress='return isSomeSplChars(event);' ID="txtFileNo"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">File Date :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtFileDt" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">No. of Containers :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtCntrsNo" onkeypress='return isNumberKey(event);'
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">Type of Containers :</span>
                                                                        </td>
                                                                        <td>
                                                                            <%--<asp:TextBox runat="server" ID="txtCntrsTp" CssClass="bcAsptextbox"></asp:TextBox>--%>
                                                                            <asp:DropDownList runat="server" ID="ddlCntrstp" CssClass="bcAspdropdown">
                                                                                <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">Range :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" onkeypress='return isSomeSplChars(event);' ID="txtRange"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">Division :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" onkeypress='return isSomeSplChars(event);' ID="txtDvsn"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">Commissionerate :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtCmsnrate" onkeypress='return isSomeSplChars(event);'
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="background-color: Gray; font-style: normal; color: White;">
                                                                        <td colspan="6">
                                                                            <b>&nbsp;&nbsp;Container Details</b>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="6" class="bcTdNewTable" style="width: 99%">
                                                                            <center>
                                                                                <%--<div style="overflow: auto; width: 35%;" id="divCntrDtls" runat="server">
                                                                                </div>--%>
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
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Porting of Loading :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPrtLdng" CssClass="bcAspdropdown" required="required">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Port of Discharge :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPrtDscrg" CssClass="bcAspdropdown" required="required">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Country of
                                            <br />
                                            Destination :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPlcFnlDstn" CssClass="bcAspdropdown" required="required">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Country of Origin :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPlcOrgGds" CssClass="bcAspdropdown" required="required">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Total Packages :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtTtlPkgs" onkeypress='return isNumberKey(event);'
                                            CssClass="bcAsptextbox" TabIndex="38" MaxLength="10" required="required"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Loose Packets :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtLsePkgs" onkeypress='return isNumberKey(event);'
                                            CssClass="bcAsptextbox" TabIndex="40" MaxLength="10" required="required"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Gross Weight(KGs) :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtGrsWt" onblur="return CheckWeights(event);" onkeyup="extractNumber(this,3,true);"
                                            onkeypress="return blockNonNumbers(this, event, true, false);" CssClass="bcAsptextbox"
                                            TabIndex="42" MaxLength="50" required="required"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Net Weight(KGs) :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtNtWt" CssClass="bcAsptextbox" onblur="return CheckWeights(event);"
                                            onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            TabIndex="44" MaxLength="10" required="required"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright" id="SpnFOB" runat="server"></span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFobVal" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            TabIndex="46" MaxLength="10" required="required"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Rotation No. :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRtnNo" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="48" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Rotation Date :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRtnDt" CssClass="bcAsptextbox DatePicker" TabIndex="50"
                                            MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Nature of Cargo :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtNtrCrg" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="52" MaxLength="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">No. of Containers :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtNCntrs" CssClass="bcAsptextbox" onkeypress='return isNumberKey(event);'
                                            TabIndex="54" MaxLength="50" required="required"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">RBI Waiver No. :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRbiWvNo" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="56" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Date :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtRbiWvDt" CssClass="bcAsptextbox DatePicker" TabIndex="58"
                                            MaxLength="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="Accordion1" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false" TabIndex="60">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane1" runat="server">
                                                        <Header>
                                                            <a href="#" class="href">DBK</a>
                                                        </Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel1" runat="server" Width="100%">
                                                                <table style="width: 100%;">
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright" id="SpnTODB" runat="server"></span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtTDBck" onblur="extractNumber(this,2,false);" onkeyup="extractNumber(this,2,false);"
                                                                                onkeypress="return blockNonNumbers(this, event, true, false);" CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright" id="SpnSTF" runat="server"></span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtSrvcTxRfnd" onblur="extractNumber(this,2,false);"
                                                                                onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">DBK Amount Received:</span>
                                                                            <%--<span class="bcLabelright">DBK Scroll No. :</span>--%>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtDbkAmountReceived" CssClass="bcAsptextbox" onkeypress='return isNumberKey(event);'></asp:TextBox>
                                                                            <%--<asp:TextBox runat="server" ID="txtDBKScrlNo" onkeypress='return isNumberKey(event);'
                                                                                CssClass="bcAsptextbox"></asp:TextBox>--%>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">Date :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtDbkScrlDt" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">EP Copy Receipt Status :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtDbkEPCRstat" onkeypress='return isSomeSplChars(event);'
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">LEO Date :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtLeoDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">Exam Mark ID :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtExmMrkID" onkeypress='return isSomeSplChars(event);'
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">Mark Date :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtMrkDt" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">Bank A/c No. :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtBnkAcNo" onkeypress='return isNumberKey(event);'
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">Amount Remitted Date :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtAmntRmtdDt" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">Amount Remitted Remarks :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtRemarks" TextMode="MultiLine" onkeypress='return isSomeSplChars(event); isAmount(event); '
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">DBK :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:RadioButtonList runat="server" ID="rbtnDBK" TabIndex="138" RepeatDirection="Horizontal"
                                                                                AutoPostBack="true" OnSelectedIndexChanged="rbtnDBK_SelectedIndexChanged1">
                                                                                <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                                                                <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </td>
                                                                        <td>
                                                                            
                                                                            <asp:Label ID="lblScrl" runat="server" Text="Bank Scroll No.:" class="bcLabelright"></asp:Label>
                                                                            <%--<asp:Label ID="lblDBKAmountReceived" runat="server" Text="DBK Amount Received:" class="bcLabelright"></asp:Label>--%>
                                                                            <asp:Label ID="lblrmks" runat="server" Text="Custom Query:" class="bcLabelright" Visible="false"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <%--<asp:TextBox runat="server" ID="txtDbkAmountReceived" CssClass="bcAsptextbox"></asp:TextBox>--%>
                                                                            <asp:TextBox runat="server" ID="txtbnkscrlno" CssClass="bcAsptextbox"></asp:TextBox>
                                                                            <asp:TextBox runat="server" ID="txtremksdbk" CssClass="bcAsptextbox" Visible="false"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblDbkYRemarks" runat="server" Text="Remarks:" class="bcLabelright"></asp:Label>
                                                                            <asp:Label ID="lblActn" runat="server" Text="Action:" class="bcLabelright" Visible="false"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtDbkYRemarks" TextMode="MultiLine" CssClass="bcAsptextbox"></asp:TextBox>
                                                                            <asp:TextBox runat="server" ID="txtAction" CssClass="bcAsptextbox" Visible="false"></asp:TextBox>
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
                                        <b>&nbsp;&nbsp;Invoice Details</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright"> Invoice Value (₹) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtInvcInr" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox" TabIndex="80" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright"><asp:Label ID="LblAmount" runat="server" Text="Invoice Value "></asp:Label> <font color="red" size="2"><b> *</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtInvcUsd" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox" TabIndex="82" MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright"><asp:Label ID="LblFOBVal" Text="FOB Value " runat="server"></asp:Label><font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFobValInr" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox" TabIndex="84" MaxLength="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Proforma Invoice No. <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtPfrmInvcNo" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="86" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Proforma Invoice Date <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtPfrmInvcDt" CssClass="bcAsptextbox DatePicker"
                                            TabIndex="88" MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Nat. of Con :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtNtCn" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" required="required" TabIndex="90" MaxLength="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Foreign Currency(Inv):</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFCrInv" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox" TabIndex="92" onchange='CalculateInvoiceInrValue();'
                                            MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Exchange Rate :</span>
                                    </td>
                                    <td align="left">
                                        <span class="bcLabel">1.00 USD =</span>
                                        <asp:TextBox runat="server" ID="txtExcngRt" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox" TabIndex="94" onchange='CalculateInvoiceInrValue();'
                                            MaxLength="10" Width="100px"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright" id="SpnInr" runat="server"></span>
                                    </td>
                                    <td align="left">
                                        <span id="InvcInrVal" runat="server" class="bcLabel"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table class="rounded-corner" style="width: 100%;">
                                            <thead>
                                                <td style="height: 25px" class="rounded-First">
                                                    &nbsp;
                                                </td>
                                                <td style="height: 25px">
                                                    <span class="bcLabel">Rate</span>
                                                </td>
                                                <td style="height: 25px">
                                                    <span class="bcLabel">Currency</span>
                                                </td>
                                                <td style="height: 25px" class="rounded-Last">
                                                    <span class="bcLabel">Amount</span>
                                                </td>
                                            </thead>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">Insurance</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtInsrncRt" onblur="extractNumber(this,2,false); CalculateAmount('ddlInsrncCrncy', 'txtInsrncRt', 'txtInsrncAmnt');"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="96" MaxLength="12"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="ddlInsrncCrncy" onchange="CalculateAmount('ddlInsrncCrncy', 'txtInsrncRt', 'txtInsrncAmnt');"
                                                        CssClass="bcAspdropdown" TabIndex="98">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtInsrncAmnt" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="100" MaxLength="12"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">Freight</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtFrtRt" onblur="extractNumber(this,2,false); CalculateAmount('ddlFrtCrncy', 'txtFrtRt', 'txtFrtAmnt');"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="102" MaxLength="12"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="ddlFrtCrncy" onchange="CalculateAmount('ddlFrtCrncy', 'txtFrtRt', 'txtFrtAmnt');"
                                                        CssClass="bcAspdropdown" TabIndex="104">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtFrtAmnt" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="106" MaxLength="12"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">Discount</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtDscntRt" onblur="extractNumber(this,2,false); CalculateAmount('ddlDscntCrncy', 'txtDscntRt', 'txtDscntAmnt');"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="108" MaxLength="12"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="ddlDscntCrncy" onchange="CalculateAmount('ddlDscntCrncy', 'txtDscntRt', 'txtDscntAmnt');"
                                                        CssClass="bcAspdropdown" TabIndex="110">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtDscntAmnt" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="112" MaxLength="12"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">Commission</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtCmsnRt" onblur="extractNumber(this,2,false);CalculateAmount('ddlCmsnCrncy', 'txtCmsnRt', 'txtCmsnAmnt');"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="114" MaxLength="12"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="ddlCmsnCrncy" onchange="CalculateAmount('ddlCmsnCrncy', 'txtCmsnRt', 'txtCmsnAmnt');"
                                                        CssClass="bcAspdropdown" TabIndex="116">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtCmsnAmnt" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="118" MaxLength="12"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">Other Deductions</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtOtrDtcnsRt" onblur="extractNumber(this,2,false);CalculateAmount('ddlOtrDtcnsCrncy', 'txtOtrDtcnsRt', 'txtOtrDtcnsAmnt');"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="120" MaxLength="12"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="ddlOtrDtcnsCrncy" onchange="CalculateAmount('ddlOtrDtcnsCrncy', 'txtOtrDtcnsRt', 'txtOtrDtcnsAmnt');"
                                                        CssClass="bcAspdropdown" TabIndex="122">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtOtrDtcnsAmnt" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="124" MaxLength="12"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">Packing Charges</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtPkngChrgsRt" onblur="extractNumber(this,2,false); CalculateAmount('ddlPkngChrgsCrncy', 'txtPkngChrgsRt', 'txtPkngChrgsAmnt');"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="126" MaxLength="12"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="ddlPkngChrgsCrncy" onchange="CalculateAmount('ddlPkngChrgsCrncy', 'txtPkngChrgsRt', 'txtPkngChrgsAmnt');"
                                                        CssClass="bcAspdropdown" TabIndex="128">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtPkngChrgsAmnt" onblur="extractNumber(this,2,false);"
                                                        onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        CssClass="bcAsptextbox" TabIndex="130" MaxLength="12"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">Nature of Payment</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtNtrPmnt" onkeypress='return isSomeSplChars(event);'
                                                        CssClass="bcAsptextbox" TabIndex="132" MaxLength="12"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <span class="bcLabel">Period of Payment</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtPrdPmnt" onkeypress='return isNumberKey(event);'
                                                        CssClass="bcAsptextbox" TabIndex="134" MaxLength="12"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <span class="bcLabelright">FTP Mentioned or Not?</span>
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList runat="server" ID="rbtnFtpMntn" TabIndex="136" RepeatDirection="Horizontal">
                                                        <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                            </tr>
                                            <tfoot>
                                                <td class="rounded-foot-left">
                                                </td>
                                                <td colspan="2">
                                                </td>
                                                <td class="rounded-foot-right">
                                                </td>
                                            </tfoot>
                                        </table>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        <b>&nbsp;&nbsp;Documents Attached</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Invoices :</span>
                                    </td>
                                    <td align="left">
                                        <asp:RadioButtonList runat="server" ID="rbtnInvcAtchmnt" TabIndex="138" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Packing List :</span>
                                    </td>
                                    <td align="left">
                                        <asp:RadioButtonList runat="server" ID="rbtnPkngLstAtchmnt" TabIndex="140" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">SDF Declaration :</span>
                                    </td>
                                    <td align="left">
                                        <asp:RadioButtonList runat="server" ID="rbtnSdfDclrtnAtchmnt" TabIndex="142" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright" style="text-align: right;">Appendix III with 4A Declaration
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:RadioButtonList runat="server" ID="rbtnApdx4ADclrAtchmnt" TabIndex="142" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">LET Export Date :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtLetExptDt" CssClass="bcAsptextbox DatePicker"
                                            TabIndex="144" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Officer of Custom :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtOfcrCstm" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="146" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">Date of Shipment :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtShpmntDt" CssClass="bcAsptextbox DatePicker" TabIndex="148"
                                            MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block; height: 29px;">
                                        <span class="bcLabelright">Vessel Name :</span>
                                    </td>
                                    <td align="left" style="height: 29px">
                                        <asp:TextBox runat="server" ID="txtVslNm" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="150" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="height: 29px">
                                        <span class="bcLabelright">Voyage No. :</span>
                                    </td>
                                    <td align="left" style="height: 29px">
                                        <asp:TextBox runat="server" ID="txtVygNo" onkeypress='return isSomeSplChars(event);'
                                            CssClass="bcAsptextbox" TabIndex="152" MaxLength="50"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="41%">
                                                        <span id="Span6" class="bcLabelright">Comments<font color="red" size="2"><b>*</b></font>:</span>
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
                                    <td colspan="6">
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="Accordion2" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false" TabIndex="160">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane2" runat="server">
                                                        <Header>
                                                            <a href="#" class="href">DEPB Details</a>
                                                        </Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel3" runat="server" Width="98%">
                                                                <table style="width: 100%;">
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <span class="bcLabelright">Total FOB Value Declared by Exporter for DEPB Items :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtExptrDEPBItems" onblur="extractNumber(this,2,false);"
                                                                                onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            &nbsp; USD
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <span class="bcLabelright">Total FOB Value Declared by Exporter for Non-DEPB Items :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtExptrNonDEPBItems" onblur="extractNumber(this,2,false);"
                                                                                onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            &nbsp; USD
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <span class="bcLabelright">Customs Accepted Total FOB Value for DEPB Items :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtCstmrAcptTFobValDEPBItms" onblur="extractNumber(this,2,false);"
                                                                                onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            &nbsp; USD
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span class="bcLabelright">DEPB Lic :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtDepbLicNmbr" onkeypress='return isSomeSplChars(event);'
                                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <span class="bcLabelright">DEPB Lic Date :</span>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox runat="server" ID="txtDepbLicDate" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
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
                                    <td colspan="6" class="bcTdNewTable">
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="Accordion3" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane4" runat="server">
                                                        <Header>
                                                            <a href="#" class="href">Attachments</a> &nbsp;<asp:Image runat="server" ID="imgAtchmt"
                                                                AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif" Visible="false" />
                                                        </Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel4" runat="server" Width="98%">
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
                                                        <div id="Div4" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" TabIndex="180" ID="btnSave" Text="Save" OnClick="btnSave_Click"
                                                                formvalidate="True" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div5" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" TabIndex="182" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div6" class="bcButtonDiv">
                                                            <a href="../Masters/Home.aspx" title="Exit" tabindex="184" class="bcAlink" onclick="javascript:Exit()">
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
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });
    </script>
    <script type="text/javascript">

        $(document).ready(function () {
            var dateToday = new Date();
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

        function CalculateAmount(CrncyVal, RateValue, AmountVal) {
            try {
                if ($('[id$=' + CrncyVal + '] option:selected').text() == 'INR')
                    $('[id$=' + AmountVal + ']').val((($('[id$=txtExcngRt]').val()) * ($('[id$=' + RateValue + ']').val())).toFixed(2));
                else if ($('[id$=' + CrncyVal + '] option:selected').text() == 'USD')                
                    $('[id$=' + AmountVal + ']').val($('[id$=' + RateValue + ']').val());
                else if ($('[id$=' + CrncyVal + '] option:selected').text() == '')
                    $('[id$=' + AmountVal + ']').val($('[id$=' + RateValue + ']').val());
                else
                    $('[id$=' + AmountVal + ']').val('0');
            }
            catch (Error) {
                ErrorMessage(Error);
            }
        }


        function CheckWeights(evt) {
            if ((parseFloat(($('[id$=txtGrsWt]').val()).trim())) < (parseFloat(($('[id$=txtNtWt]').val()).trim()))) {
                ErrorMessage('Net Weight should not be greaterthan Gross Weight.');
                $('[id$=txtNtWt]').val('');
                $('[id$=txtNtWt]').focus();
                return false;
            }
            else
                return true;
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) //&& charCode != 46 
                return false;
            return true;
        }
        function isAmount(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
                return false;
            return true;
        }

        function isNoSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if ((charCode < 33 || charCode > 47) && (charCode < 58 || charCode > 64) && (charCode < 91 || charCode > 96) && (charCode < 123 || charCode > 126))
                return true;
            return false;
        }

        function isSomeSplChars(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function ValidateAmount(AmountField) {
            var reg = /^[1-9]\d*(((,\d{3}){1})?(\.\d{0,3})?)$/;
            if (AmountField.value == '') {
                return true;
            }
            else if (reg.test(AmountField.value) == false) {
                ErrorMessage('Invalid Amount');
                AmountField.value = '';
                AmountField.focus();
                return false;
            }
            return true;
        }

        function CheckSHPBNo() {
            var enqNo = $('[id$=txtShpngBlNo]').val();
            var result = ShpngBilDtls.CheckSHPBNo(enqNo);
            if (result.value == false) {
                $("#<%=txtShpngBlNo.ClientID%>")[0].value = '';
                ErrorMessage('Shipping Bill No. Number Exists.');
                $("#<%=txtShpngBlNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function CalculateInvoiceInrValue() {
            try {
                var FCrInv = $('[id$=txtFCrInv]').val();
                var ExcngRt = $('[id$=txtExcngRt]').val();
                $('[id$=InvcInrVal]').text(FCrInv * ExcngRt);
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }

        function doConfirmCntrDtls(id) {
            try {
                if (confirm("Are you sure you want to Delete Container Details?")) {
                    var result = ShpngBilDtls.CntrDtlsDeleteItem(id);
                    var getdivCntrDtls = GetClientID("divCntrDtls").attr("id");
                    $('#' + getdivCntrDtls).html(result.value);
                }
                else {
                    return false;
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }

        function SaveChanges(RNo) {
            try {

                var ddlCntrType = GetClientID("ddl" + (parseInt(RNo))).attr("id");
                var CntrType = $('#' + ddlCntrType).val();
                var txtCntrNo = GetClientID("txtCntrNo" + (parseInt(RNo))).attr("id");
                var CntrNo = $('#' + txtCntrNo).val();
                var txtSize = GetClientID("txtSize" + (parseInt(RNo))).attr("id");
                var Size = $('#' + txtSize).val();
                var txtSealNo = GetClientID("txtSealNo" + (parseInt(RNo))).attr("id");
                var SealNo = $('#' + txtSealNo).val();
                var txtDate = GetClientID("txtDate" + (parseInt(RNo))).attr("id");
                var Date = $('#' + txtDate).val();
                //if (CntrNo != '' && Size != '' && SealNo != '' && Date != '' && CntrType != 0) {
                var result = ShpngBilDtls.CntrDtlsAddItem(RNo, CntrNo, CntrType, Size, SealNo, Date, false);
                var getdivCntrDtls = GetClientID("divCntrDtls").attr("id");
                $('#' + getdivCntrDtls).html(result.value);
                //}
                //                else {
                //                    $("#<%=divMyMessage.ClientID %> span").remove();
                //                    if (CntrType == '0') {
                //                        ErrorMessage('Container Type is Required.');
                //                        $('#' + ddlCntrType).focus();
                //                    }
                //                    else if (CntrNo == '') {
                //                        ErrorMessage('Container Number is Required.');
                //                        $('#' + txtCntrNo).focus();
                //                    }
                //                    else if (Size == '') {
                //                        ErrorMessage('Container Size is Required.');
                //                        $('#' + txtSize).focus();
                //                    }
                //                    else if (SealNo == '') {
                //                        ErrorMessage('Container Seal Number is Required.');
                //                        $('#' + txtSealNo).focus();
                //                    }
                //                    else if (Date == '') {
                //                        ErrorMessage('Date is Required.');
                //                        $('#' + txtDate).focus();
                //                    }
                //                }
                DatePickerAdd();
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }

        function AddNewRow(RNo) {
            try {

                var ddlCntrType = GetClientID("ddl" + (parseInt(RNo))).attr("id");
                var CntrType = $('#' + ddlCntrType).val();
                var txtCntrNo = GetClientID("txtCntrNo" + (parseInt(RNo))).attr("id");
                var CntrNo = $('#' + txtCntrNo).val();
                var txtSize = GetClientID("txtSize" + (parseInt(RNo))).attr("id");
                var Size = $('#' + txtSize).val();
                var txtSealNo = GetClientID("txtSealNo" + (parseInt(RNo))).attr("id");
                var SealNo = $('#' + txtSealNo).val();
                var txtDate = GetClientID("txtDate" + (parseInt(RNo))).attr("id");
                var Date = $('#' + txtDate).val();
                if ((CntrNo != '' || CntrNo != '0') && (Size != '' || Size != '0') && SealNo != '' && Date != '' && CntrType != '00000000-0000-0000-0000-000000000000') {
                    var result = ShpngBilDtls.CntrDtlsAddItem(RNo, CntrNo, CntrType, Size, SealNo, Date, true);
                    var getdivCntrDtls = GetClientID("divCntrDtls").attr("id");
                    $('#' + getdivCntrDtls).html(result.value);
                }
                else {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    if (CntrType == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Container Type is Required.');
                        $('#' + ddlCntrType).focus();
                    }
                    else if ((CntrNo == '' || CntrNo == '0')) {
                        ErrorMessage('Container Number is Required.');
                        $('#' + txtCntrNo).focus();
                    }
                    else if ((Size == '' || Size == '0')) {
                        ErrorMessage('Container Size is Required.');
                        $('#' + txtSize).focus();
                    }
                    else if (SealNo == '') {
                        ErrorMessage('Container Seal Number is Required.');
                        $('#' + txtSealNo).focus();
                    }
                    else if (Date == '') {
                        ErrorMessage('Date is Required.');
                        $('#' + txtDate).focus();
                    }
                }
                DatePickerAdd();
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
            DatePickerAdd();
        }

        function uploadComplete() {
            var result = ShpngBilDtls.AddItemListBox();
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
            //$get("<%=lblstatus.ClientID%>").innerHTML = "File upload Failed.";
            ErrorMessage('File upload Failed.');
        }
        function uploadStarted() {
            //$get("<%=lblstatus.ClientID%>").innerHTML = "File upload started.";
            SuccessMessage('File Uploaded Started.');
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                if (confirm("Are you sure you want to delete the item")) {
                    var result = ShpngBilDtls.DeleteItemListBox($('#lbItems').val());
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
            //if ($('#lbItems').val() != "") {
            var result = ShpngBilDtls.AddItemListBox();
            var getDivGRNDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGRNDItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            // }
        });


        //        function isNumberKey(evt) {
        //            var charCode = (evt.which) ? evt.which : event.keyCode;
        //            if (charCode > 31 && (charCode < 48 || charCode > 57)) //&& charCode != 46 
        //                return false;
        //            return true;
        //        }
        //        function isAlphaKey(evt) {
        //            var charCode = (evt.which) ? evt.which : event.keyCode;
        //            if (charCode != 32 && charCode != 8 && (charCode < 65 || charCode > 91) && (charCode < 97 || charCode > 122))
        //                return false;
        //            return true;
        //        }
        //        function isAmount(evt) {
        //            var charCode = (evt.which) ? evt.which : event.keyCode;
        //            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
        //                return false;
        //            return true;
        //        }
        //        function isOrgName(evt) {
        //            var charCode = (evt.which) ? evt.which : event.keyCode;
        //            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 46 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
        //                return false;
        //            return true;
        //        }




        function MyValue(Cntrl, Type, Message) {
            try {
                if (Type == 'int') {
                    if (($('[id$=' + Cntrl + ']').val()).trim() == '0') {
                        ErrorMessage(Message);
                        return false;
                    }
                    else
                        return true;
                }
                else if (Type == 'string') {
                    if (($('[id$=' + Cntrl + ']').val()).trim() == '') {
                        ErrorMessage(Message);
                        return false;
                    }
                    else
                        return true;
                }
                else if (Type == 'stringint') {
                    if (($('[id$=' + Cntrl + ']').val()).trim() == '') {
                        ErrorMessage(Message);
                        return false;
                    }
                    else if (($('[id$=' + Cntrl + ']').val()).trim() == '0') {
                        ErrorMessage(Message);
                        return false;
                    }
                    else
                        return true;
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }
        function Myvalidations() {
            try {
                if (($('[id$=ddlPrfmaInvcNo]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Shipment Proforma Invoice Number is Required.');
                    $('[id$=ddlPrfmaInvcNo]').focus();
                    return false;
                }
                else if (($('[id$=txtShpngBlNo]').val()).trim() == '') {
                    ErrorMessage('Shipping Bill Number is Required.');
                    $('[id$=txtShpngBlNo]').focus();
                    return false;
                }
                else if ($('[id$=txtShpngBlDt]').val() == '') {
                    ErrorMessage('Shipping Bill Date is Required.');
                    $('[id$=txtShpngBlDt]').focus();
                    return false;
                }
                else if (($('[id$=txtInvcInr]').val()).trim() == '') {
                    ErrorMessage('Invoice Value is Required.');
                    $('[id$=txtInvcInr]').focus();
                    return false;
                }
                else if ($('[id$=txtInvcUsd]').val() == '') {
                    ErrorMessage('Invoice Value($) is Required.');
                    $('[id$=txtInvcUsd]').focus();
                    return false;
                }
                else if (($('[id$=txtFobValInr]').val()).trim() == '') {
                    ErrorMessage('FOB Value is Required.');
                    $('[id$=txtFobValInr]').focus();
                    return false;
                }
                else if ($('[id$=txtPfrmInvcNo]').val() == '') {
                    ErrorMessage('Proforma Invoice Number is Required.');
                    $('[id$=txtPfrmInvcNo]').focus();
                    return false;
                }
                else if ($('[id$=txtPfrmInvcDt]').val() == '') {
                    ErrorMessage('Proforma Invoice Date is Required.');
                    $('[id$=txtPfrmInvcDt]').focus();
                    return false;
                }
                if ($('[id$=DivComments]').css("visibility") == "visible") {
                    if (($('[id$=txtComments]').val()).trim() == '') {
                        ErrorMessage('Comment is Required.');
                        $('[id$=txtComments]').focus();
                        return false;
                    }
                }
                return true;
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }




        //        function Myvalidations() {

        //            try {

        //                MyValue('ddlPrfmaInvcNo', 'int', 'Select Proforma Invoice Number');
        //                MyValue('txtLeoNo', 'string', 'Select LEO Number');
        //                MyValue('txtLeoDt', 'string', 'Select LEO Date');





        //                //                 = ddlChaMstr.SelectedIndex = ddlPrtLdng.SelectedIndex = ddlPrtDscrg.SelectedIndex = -1;
        //                //                ddlPlcFnlDstn.SelectedIndex = ddlPlcOrgGds.SelectedIndex = ddlInsrncCrncy.SelectedIndex = ddlFrtCrncy.SelectedIndex = -1;
        //                //                ddlDscntCrncy.SelectedIndex = ddlCmsnCrncy.SelectedIndex = ddlOtrDtcnsCrncy.SelectedIndex = ddlPkngChrgsCrncy.SelectedIndex = -1;

        //                //                rbtnFtpMntn.SelectedIndex = rbtnInvcAtchmnt.SelectedIndex = rbtnPkngLstAtchmnt.SelectedIndex = rbtnSdfDclrtnAtchmnt.SelectedIndex = -1;
        //                //                rbtnApdx4ADclrAtchmnt.SelectedIndex = -1;

        //                //                 =  = txtShpngBlNo.Text = txtShpngBlDt.Text = txtStOrgn.Text = txtLofspNo.Text = "";
        //                //                txtLofspDt.Text = txtStfngDt.Text = txtFileNo.Text = txtFileDt.Text = txtCntrsNo.Text = txtCntrsTp.Text = "";
        //                //                txtRange.Text = txtDvsn.Text = txtCmsnrate.Text = txtTtlPkgs.Text = txtLsePkgs.Text = txtGrsWt.Text = txtNtWt.Text = "";
        //                //                txtFobVal.Text = txtRtnNo.Text = txtRtnDt.Text = txtNtrCrg.Text = txtNCntrs.Text = txtRbiWvNo.Text = txtRbiWvDt.Text = "";
        //                //                txtTDBck.Text = txtSrvcTxRfnd.Text = txtDBKScrlNo.Text = txtDbkScrlDt.Text = txtDbkEPCRstat.Text = txtLeoDate.Text = "";
        //                //                txtExmMrkID.Text = txtMrkDt.Text = txtBnkAcNo.Text = txtAmntRmtdDt.Text = txtRemarks.Text = txtInvcInr.Text = "";
        //                //                txtInvcUsd.Text = txtFobValInr.Text = txtPfrmInvcNo.Text = txtPfrmInvcDt.Text = txtNtCn.Text = txtFCrInv.Text = "";
        //                //                txtExcngRt.Text = txtInsrncRt.Text = txtInsrncAmnt.Text = txtFrtRt.Text = txtFrtAmnt.Text = txtDscntRt.Text = "";
        //                //                txtDscntAmnt.Text = txtCmsnRt.Text = txtCmsnAmnt.Text = txtOtrDtcnsRt.Text = txtOtrDtcnsAmnt.Text = txtPkngChrgsRt.Text = "";
        //                //                txtPkngChrgsAmnt.Text = txtNtrPmnt.Text = txtPrdPmnt.Text = txtLetExptDt.Text = txtOfcrCstm.Text = txtShpmntDt.Text = txtVslNm.Text = "";
        //                //                txtVygNo.Text = txtExptrDEPBItems.Text = txtExptrNonDEPBItems.Text = txtCstmrAcptTFobValDEPBItms.Text = txtDepbLicNmbr.Text = "";
        //                //                txtDepbLicDate.Text = "";



        //                return true;



        //            }
        //            catch (Error) {
        //                ErrorMessage(Error.message);
        //                return false;
        //            }
        //        }

    </script>
</asp:Content>
