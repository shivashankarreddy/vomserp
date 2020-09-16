<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="PrfmaInvoice.aspx.cs" Inherits="VOMS_ERP.Invoices.PrfmaInvoice" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Proforma Invoice" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span2" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <span id="Span3" style="float: left !important" class="bcLabelright">IS Bivac/Cotecna</span>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CHkBivac" runat="server" TabIndex="2" onchange="ShowBivac();" OnCheckedChanged="CHkBivac_CheckedChanged"
                                            AutoPostBack="True" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:HiddenField ID="hfPrevFPOIDs" runat="server" Value="" />
                                        <asp:HiddenField ID="hfIsEdit" runat="server" Value="False" />
                                        <asp:HiddenField ID="hfDirect" runat="server" Value="" />
                                        <span id="Span3" style="float: left !important" class="bcLabelright">Bivac Shipment
                                            Planning No.</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlBivacShpmntPlnngNo" CssClass="bcAspdropdown"
                                            Enabled="false" AutoPostBack="True" TabIndex="4" OnSelectedIndexChanged="ddlChkLst_SelectedIndexChanged1">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Shipment Planning No. <font color="red" size="2"><b>
                                            *</b></font> :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlChkLst" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlChkLst_SelectedIndexChanged1">
                                            <%--<asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>--%>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Customer Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlCstmr" CssClass="bcAspdropdown" AutoPostBack="false"
                                            OnSelectedIndexChanged="ddlCstmr_SelectedIndexChanged" Enabled="true">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span34" class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                            OnSelectedIndexChanged="lbfpos_SelectedIndexChanged" Enabled="False"></asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Proforma Invoice No. <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtPrfmInvce" CssClass="bcAsptextbox" Enabled="False"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Proforma Invoice Date <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtPIDate" CssClass="bcAsptextbox" Enabled="False"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Other References :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtOtrRfs" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                            Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Notify :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlNtfy" CssClass="bcAspdropdown" Enabled="false">
                                            <asp:ListItem Text="Select" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <%--<asp:TextBox runat="server" ID="txtNtfy" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>--%>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">Place of Origin of Goods <font color="red" size="2">
                                            <b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPlcOrgGds" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Place of Final Destination <font color="red" size="2">
                                            <b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPlcFnlDstn" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span10" class="bcLabel">Port Of Loading : </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPrtLdng" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span13" class="bcLabel">Port Of Discharge : </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPrtDscrg" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span15" class="bcLabel">Place of Delivery <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPlcDlry" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Pre-Carriage by :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPCrBy" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Place of receipt by pre-Carrier :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPlcRcptPCr" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Vessel / Flight No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtVslFlt" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">Freight Amount:
                                            <%--<font color="red" size="2"><b>*</b></font>--%>
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtFrieghtAmt" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            onkeyup="extractNumber(this,2,false);" onblur="extractNumber(this,2,false);"
                                            CssClass="bcAsptextbox" Enabled="true"></asp:TextBox>
                                        <%-- onkeypress="return blockNonNumbers(this, event, true, true)" 
                                        onkeyup="extractNumber(this,0,true)"onkeypress="return isNumberKey(event)"--%>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span12" class="bcLabel">Terms Of Delivery and Payment <font color="red"
                                            size="2"><b>*</b></font>: </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTrmDlryPmnt" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                            Enabled="false"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="51%">
                                                        <span id="Span6" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span14" class="bcLabel">&nbsp; </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        &nbsp;
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
                            <%--<div id="dvgroup" runat="server" style="display: block;">--%>
                            <asp:GridView runat="server" ID="gvPfrmaInvce" AutoGenerateColumns="false" Width="100%"
                                EmptyDataText="No Records To Display...!" OnRowDataBound="gvPfrmaInvce_RowDataBound"
                                OnPreRender="gvPfrmaInvce_PreRender">
                                <Columns>
                                    <asp:BoundField HeaderText="FPONmbr" DataField="FPONmbr"  />
                                    <asp:BoundField HeaderText="S.No." DataField="Sno" />
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <img id="imgExpand" runat="server" alt="" style="cursor: pointer" src="../images/Expand.png" />
                                            <div id="dvSubItems" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <HeaderTemplate>
                                            <asp:CheckBox runat="server" ID="chkbhdr" Checked="true" onClick="javascript:chkAllCheckbox(this);" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" Checked="true" ID="chkbitm" Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item Desc">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfIsSubItem" runat="server" Value="false" />
                                            <asp:Label ID="lblsItemDtlsID" runat="server" Text='<%# Eval("StockItemsId") %>'
                                                Visible="false"></asp:Label>
                                            <asp:Label ID="lblFrnPoID" runat="server" Text='<%# Eval("ItmForeignPOId") %>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblSerialNo" runat="server" Visible="false"></asp:Label>
                                            <asp:HiddenField ID="hfFESNo" runat="server" Value='<%# Eval("Sno") %>' />
                                            <asp:HiddenField ID="hfItemID" runat="server" Value='<%# Eval("ItemId") %>' />
                                            <asp:Label ID="lblItemDesc" runat="server" Text='<%# Eval("Description") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="HS-Code">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfFrnPoID" runat="server" Value='<%# Eval("ItmForeignPOId") %>' />
                                            <%--<asp:Label ID="lblHSCode" runat="server" Text='<%# Eval("HSCode") %>' />--%>
                                            <asp:TextBox ID="txtHSCode" runat="server" Text='<%# Eval("HSCode") %>'  AutoPostBack="true" OnTextChanged="txtHscCode_TextChanged" MaxLength="10"
                                                Style="width: 100px; text-align: right;" onkeyup="extractNumber(this,2,false);" 
                                                onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Part No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPartno" runat="server" Text='<%# Eval("PartNumber") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Specifications" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSpec" runat="server" Text='<%# Eval("Specifications") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Make">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMk" runat="server" Text='<%# Eval("Make") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("DspchQty") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Units" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUnits" runat="server" Text='<%# Eval("UnitNm") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate($)" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRate" runat="server" Text='<%# Eval("Rate") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount($)" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("Amount")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.rowGrouping.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <%--<style type="text/css">
        .subclass
        {
           width: 115px;
        }
    </style>--%>
    <script type="text/javascript">
        //        $("[src*=Expand]").live("click", function () {

        //            var table = "<table border='1'><tr><td>Row 1, Column 1</td><td>Row 1, Column 2</td></tr><tr><td>Row 2, Column 1</td><td>Row 2, Column 2</td></tr></table>";

        //            var id = $(this).closest('tr')[0].cells[2].children[0].id;
        //            var FPOID = $("#" + id).val();

        //            var hfFESNo = $(this).closest('tr')[0].cells[1].children[1].id;
        //            var FESNo = $("#" + hfFESNo).val();

        //            var PItmID = $(this).closest('tr')[0].cells[1].children[2].id;
        //            var ParentID = $("#" + PItmID).val();
        //            var result = PrfmaInvoice.BindSubItems(ParentID, FPOID, FESNo);

        //            //$(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>");
        //            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + result.value + "</td></tr>")
        //            $(this).attr("src", "../images/collapse.png");
        //        });
        //        $("[src*=collapse]").live("click", function () {
        //            $(this).attr("src", "../images/Expand.png");
        //            $(this).closest("tr").next().remove();
        //        });

        function SaveChanges(ParentID, RID, Cntrl, IsADD) {
            //dvSubItems
            var fsno = RID;
            var Decimal_ = 1;
            if (RID.indexOf(".") >= 0) {
                fsno = RID.split('.')[0];
                //Decimal_ = RID.split('.')[1];
                RID = RID.replace(".", "\\.");
            }
            var ItemID = $("#hfItemID" + RID).val();
            var ParentItemID = ParentID;

            var id = $(Cntrl).closest('table').parent().parent()[0].previousSibling.cells[2].children[0].id;
            var FPOID = $("#" + id).val();

            var FESNO = "" + fsno + "";
            var SNO = "" + RID + "";

            var ItmDesc = $("#txtDescription" + RID).val();
            var HsCode = $("#txtHsCode" + RID).val();
            var PartNo = $("#txtPartNo" + RID).val();
            var Make = $("#txtMake" + RID).val();
            var Qty = $("#txtQuantity" + RID).val();
            var Units = $("#ddlUnits" + RID).val();
            var Rate = $("#txtRate" + RID).val();
            if (IsADD == "true" && ItmDesc != "" && Qty == 0 && Rate == 0) {
                if (ItmDesc == "")
                    ErrorMessage('Item Description is required.');
                else if (qty == 0)
                    ErrorMessage('Quantity is required.');
                else if (Rate == 0)
                    ErrorMessage('Rate is required.');
            }
            else {
                var rslt = PrfmaInvoice.SaveChanges(ItemID, ParentItemID, FPOID, FESNO, SNO, ItmDesc, HsCode, PartNo, Make, Qty, Units, Rate, IsADD);

                var RowIndex = $(Cntrl).closest('table').parent().parent().index();
                $(Cntrl).closest("table").parent().parent().remove();
                $('[id$=gvPfrmaInvce] > tbody > tr').eq(parseInt(RowIndex) - parseInt(Decimal_)).after("<tr><td></td><td colspan = '999'>" + rslt.value + "</td></tr>");
            }
        }

        function Delete_SubItem(RID, Cntrl, ItemID) {
            if (confirm("Do you want to Delete this Row...?")) {
                var PItmID = $(Cntrl).closest('table').parent().parent()[0].previousSibling.cells[1].children[2].id;
                var ParentID = $("#" + PItmID).val();

                var id = $(Cntrl).closest('table').parent().parent()[0].previousSibling.cells[2].children[0].id;
                var FPOID = $("#" + id).val();

                var rslt = PrfmaInvoice.Delete_SubItem(RID, ItemID, ParentID, FPOID);

                var RowIndex = $(Cntrl).closest('table').parent().parent().index();
                $(Cntrl).closest("table").parent().parent().remove();
                $('[id$=gvPfrmaInvce] > tbody > tr').eq(parseInt(RowIndex) - 1).after("<tr><td></td><td colspan = '999'>" + rslt.value + "</td></tr>");
            }
        }

    </script>
    <script type="">
        var oTable = null;
        $(document).ready(function () {
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
            oTable = $('[id$=gvPfrmaInvce]').dataTable(
            {
                "aLengthMenu": [[-1], ["All"]],
                "iDisplayLength": -1,
                "aaSorting": [[0, 'asc']],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search :"
                },

                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true

                //                , "drawCallback": function (settings) {
                //                    
                //                    var api = this.api();
                //                    var rows = api.rows({ page: 'current' }).nodes();
                //                    var last = null;

                //                    api.column(0, { page: 'current' }).data().each(function (group, i) {
                //                        if (last !== group) {
                //                            $(rows).eq(i).before('<tr class="group"><td colspan="8">' + group + 'VARA@</td></tr>');
                //                            last = group;
                //                        }
                //                    });
                //                }

            }).rowGrouping({ iGroupingColumnIndex: 0,
                sGroupingColumnSortDirection: "asc",
                iGroupingOrderByColumnIndex: 1,
                //sGroupBy: "letter",
                //                fnOnGrouped: function () {
                //                    alert($(this).find('td'));
                ////                    $(this).find('td').append($('<span />', { 'class': 'rowCount-grid' }).append($('<b />', { 'text': 101 })));
                //                },
                bHideGroupingColumn: true
            });
            GridRowCount();
        });

        var ControlID = 0;
        function GridRowCount() {
            $('span.rowCount-grid').remove();
            $('input.expandedOrCollapsedGroup').remove();
            $('.dataTables_wrapper').find('.dataTables_filter').append($('<input />', { 'type': 'button', 'class': 'expandedOrCollapsedGroup collapsed', 'value': 'Expanded All Group' }));
            $('.dataTables_wrapper').find('[id|=group-id]').each(function () {
                var rowCount = $(this).nextUntil('[id|=group-id]').length;
                var IsChecked = '';
                ControlID++;
                var iid = $(this).closest('tr').next('tr')[0].cells[1].children[0].id;
                var FPOIDVal = "'" + $("#" + iid).val() + "'";
                var PrevFPOIDs = $('[id$=hfPrevFPOIDs]').val();
                var IsEdit = $('[id$=hfIsEdit]').val();
                if (IsEdit == "True" && PrevFPOIDs != '' && PrevFPOIDs.toLowerCase().indexOf(FPOIDVal.toLowerCase()) >= 0) {
                    IsChecked = "checked";

                    //alert(PrevFPOIDs + " :: " + FPOIDVal);
                }
                if ($('[id$=hfDirect]').val() == "9") {
                    IsChecked = "checked"; 
                }
                var Controls = '<input ' + IsChecked + ' type="checkbox" id="Grpchkbx' + ControlID + '" onchange="checkFPOGroup(this,' + ControlID + ');"/>';
                Controls += '<input type="hidden" id="hfFPO' + ControlID + '" value="' + $(this).find('td').html() + '"/>';
                $(this).find('td').prepend($('<span />', { 'class': 'rowCount-grid' })
                .prepend(Controls + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'));
                if ($('[id$=hfDirect]').val() == "9") {
                    checkFPOGroupNew(this, ControlID);
                }
            });
            $('.expandedOrCollapsedGroup').live('click', function () {
                if ($(this).hasClass('collapsed')) {
                    //$(this).addClass('expanded').removeClass('collapsed').val('Collapse All Group').parents('.dataTables_wrapper').find('.collapsed-group').trigger('click');
                }
                else {
                    $(this).addClass('collapsed').removeClass('expanded').val('Expanded All Group').parents('.dataTables_wrapper').find('.expanded-group').trigger('click');
                }
            });
        };


        $('[id$=gvPfrmaInvce] tbody').on('click', 'tr', function () {
            //var data = oTable.row(this).data();
            //alert('You clicked on ' + data[0] + '\'s row');
            //            alert('A');
        });

        function checkFPOGroupNew(cntrl, ID) {

            var id = $(cntrl).closest('tr').next('tr')[0].cells[2].children[0].id;
            var FPOID = $("#" + id).val();
            var fpoNo = $('[id$=hfFPO' + ID + ']').val();
            var result = PrfmaInvoice.CheckBoxChecked(true, fpoNo, FPOID);
            if (result.value != "") {
                if (result.value == "already checked previously")
                    $("#" + cntrl.id).attr('checked', false);
                ErrorMessage(result.value);
            }
        }

        function checkFPOGroup(cntrl, ID) {

            var id = $(cntrl).closest('tr').next('tr')[0].cells[2].children[0].id;
            var FPOID = $("#" + id).val();
            var fpoNo = $('[id$=hfFPO' + ID + ']').val();
            var result = PrfmaInvoice.CheckBoxChecked(cntrl.checked, fpoNo, FPOID);
            if (result.value != "") {
                if (result.value == "already checked previously")
                    $("#" + cntrl.id).attr('checked', false);
                ErrorMessage(result.value);
            }
        }

        //        function getParameterByName(name, url) {
        //            if (!url) url = window.location.href;
        //            name = name.replace(/[\[\]]/g, "\\$&");
        //            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        //                results = regex.exec(url);
        //            if (!results) return null;
        //            if (!results[2]) return '';
        //            return decodeURIComponent(results[2].replace(/\+/g, " "));
        //        }
    </script>
    <script type="text/javascript">

        function ShowBivac() {
            if ($('[id$=CHkBivac]')[0].checked) {
                $('[id$=ddlBivacShpmntPlnngNo]').removeAttr('disabled', 'disabled');
                $('[id$=ddlChkLst]').attr('disabled', 'disabled');
            }
            else {
                $('[id$=ddlBivacShpmntPlnngNo]').attr('disabled', 'disabled');
                $('[id$=ddlChkLst]').removeAttr('disabled', 'disabled');
                ClrInputs();
            }
        }

        function ClrInputs() {
            try {
                $('[id$=ddlBivacShpmntPlnngNo]').val('0');
                $('[id$=ddlCstmr]').val('0');
                $('[id$=lbfpos]').empty();
                $('[id$=txtPrfmInvce]').val('');
                $('[id$=txtPIDate]').val('');
                $('[id$=txtOtrRfs]').val('');
                $('[id$=ddlNtfy]').val('0');
                $('[id$=ddlPlcOrgGds]').val('0');
                $('[id$=ddlPlcFnlDstn]').val('0');
                $('[id$=ddlPrtLdng]').val('0');
                $('[id$=ddlPrtDscrg]').val('0');
                $('[id$=ddlPlcDlry]').val('0');
                $('[id$=txtPCrBy]').val('');
                $('[id$=txtPlcRcptPCr]').val('');
                $('[id$=txtVslFlt]').val('');
                $('[id$=txtTrmDlryPmnt]').val('');
                $('[id$=txtComments]').val('');
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
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
            var dateToday = new Date();
            $('[id$=txtPIDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        });

        function chkAllCheckbox(obj) {
            var gv = $("#<%=gvPfrmaInvce.ClientID %> input");
            for (var i = 0; i < gv.length; i++) {
                if (gv[i].type == 'checkbox') {
                    gv[i].checked = obj.checked;
                }
            }
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
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
            var result = PrfmaInvoice.AddItemListBox();
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
                    var result = PrfmaInvoice.DeleteItemListBox($('#lbItems').val());
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
            var result = PrfmaInvoice.AddItemListBox();
            var getDivGDNItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGDNItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            // }
        });

        function Myvalidations() {
            try {
                if ($('[id$=gvPfrmaInvce]').length != 0) {
                    var GdnItems = $('[id$=gvPfrmaInvce]')[0].rows.length;
                    //var GdnItems = $("#gvPfrmaInvce tbody tr").length;
                }
                if (($('[id$=ddlChkLst]').val()).trim() == '00000000-0000-0000-0000-000000000000' && $('[id$=CHkBivac]')[0].checked == false) {
                    ErrorMessage('Shipment Planning Number is Required.');
                    $('[id$=ddlChkLst]').focus();
                    return false;
                } //ddlRefno
                if (($('[id$=ddlBivacShpmntPlnngNo]').val()).trim() == '00000000-0000-0000-0000-000000000000' && $('[id$=CHkBivac]')[0].checked == true) {
                    ErrorMessage('Bivac Shipment Planning Number is Required.');
                    $('[id$=ddlBivacShpmntPlnngNo]').focus();
                    return false;
                }
                else if (($('[id$=ddlCstmr]').val()).trim() == '0') {
                    ErrorMessage('Customer Name is Required.');
                    $('[id$=ddlCstmr]').focus();
                    return false;
                }
                else if ($('[id$=lbfpos]').val() == null) {
                    ErrorMessage('Frn PO Number is Required.');
                    $('[id$=lbfpos]').focus();
                    return false;
                }
                else if (($('[id$=txtPrfmInvce]').val()).trim() == '') {
                    ErrorMessage('Proforma Invoice Number is Required.');
                    $('[id$=txtPrfmInvce]').focus();
                    return false;
                }
                else if (($('[id$=txtPIDate]').val()).trim() == '') {
                    ErrorMessage('Proforma Invoice Date is Required.');
                    $('[id$=txtPIDate]').focus();
                    return false;
                }
                //                else if (($('[id$=ddlNtfy]').val()).trim() == '0') {
                //                    ErrorMessage('Notify is Required.');
                //                    $('[id$=ddlNtfy]').focus();
                //                    return false;
                //                }
                else if (($('[id$=ddlPlcOrgGds]').val()).trim() == '0') {
                    ErrorMessage('Place of Origin of Goods is Required.');
                    $('[id$=ddlPlcOrgGds]').focus();
                    return false;
                }
                else if (($('[id$=ddlPlcFnlDstn]').val()).trim() == '0') {
                    ErrorMessage('Place of Final Destination is Required.');
                    $('[id$=ddlPlcFnlDstn]').focus();
                    return false;
                }
                //                else if (($('[id$=txtPCrBy]').val()).trim() == '') {
                //                    ErrorMessage('Pre-Carriage By is Required.');
                //                    $('[id$=txtPCrBy]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtPlcRcptPCr]').val()).trim() == '') {
                //                    ErrorMessage('Place of Receipt by Pre-Carrier is Required.');
                //                    $('[id$=txtPlcRcptPCr]').focus();
                //                    return false;
                //                }

                //                else if (($('[id$=txtPlcRcptPCr]').val()).trim() == '') {
                //                    ErrorMessage('Place of Receipt by Pre-Carrier is Required.');
                //                    $('[id$=txtPlcRcptPCr]').focus();
                //                    return false;
                //                }
                //                else if (($('[id$=txtVslFlt]').val()).trim() == '') {
                //                    ErrorMessage('Vessel/Flight No. is Required.');
                //                    $('[id$=txtVslFlt]').focus();
                //                    return false;
                //                }
                else if (($('[id$=ddlPrtLdng]').val()).trim() == '0') {
                    ErrorMessage('Port of Loading is Required.');
                    $('[id$=ddlPrtLdng]').focus();

                    return false;
                }
                else if (($('[id$=ddlPrtDscrg]').val()).trim() == '0') {
                    ErrorMessage('Port of Discharge is Required.');
                    $('[id$=ddlPrtDscrg]').focus();

                    return false;
                }
                else if (($('[id$=ddlPlcDlry]').val()).trim() == '0') {
                    ErrorMessage('Place of Delivery is Required.');
                    $('[id$=ddlPlcDlry]').focus();
                    return false;
                }
                //                else if (($('[id$=txtFrieghtAmt]').val()).trim() == '') {
                //                    ErrorMessage('Freight Amount is Required.');
                //                    $('[id$=txtFrieghtAmt]').focus();
                //                    return false;
                //                }
                else if (($('[id$=txtTrmDlryPmnt]').val()).trim() == '') {
                    ErrorMessage('Terms of Delivery & Payment is Required.');
                    $('[id$=txtTrmDlryPmnt]').focus();
                    return false;
                }
                if (GdnItems == 0) {
                    ErrorMessage('No Items to Save.');
                    $('[id$=gvLpoItems]').focus();
                    return false;
                }
                if (GdnItems > 0) {
                    if (GdnItems == 1) {
                        ErrorMessage('No Items to Save.');
                        $('[id$=gvLpoItems]').focus();
                        return false;
                    }
                    else {
                        var select = 0;
                        for (var i = 0; i < GdnItems; i++) {
                            if ($('#Grpchkbx' + i).length > 0) {
                                if ($('#Grpchkbx' + i)[0].checked) {
                                    select = select + 1;
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
        
    </script>
</asp:Content>
