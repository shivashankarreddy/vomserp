<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="PackingList.aspx.cs" Inherits="VOMS_ERP.Invoices.PackingList" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Packing List" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span10" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <span id="Span3" class="bcLabel">Shipment Planning No. <font color="red" size="2"><b>
                                            *</b></font> :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlChkLst" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlChkLst_SelectedIndexChanged">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hfIsEdit" runat="server" Value="False" />
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
                                        <span class="bcLabel">Packing List No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtPkngListNo" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Packing List Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtPkngListNoDT" CssClass="bcAsptextbox" Enabled="False"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Other References :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtOtrRfs" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                            Enabled="False"></asp:TextBox>
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
                                        <%--<asp:TextBox runat="server" ID="txtNtfy" CssClass="bcAsptextbox" 
                                            Enabled="False"></asp:TextBox>--%>
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
                                        <span id="Span4" class="bcLabel">Pre-Carriage by :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPCrBy" CssClass="bcAsptextbox" Enabled="False"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Place of receipt by pre-Carrier :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPlcRcptPCr" CssClass="bcAsptextbox" Enabled="False"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Vessel / Flight No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtVslFlt" CssClass="bcAsptextbox" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Port Of Loading : </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPrtLdng" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">Port Of Discharge : </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPrtDscrg" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span18" class="bcLabel">Place of Delivery <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPlcDlry" CssClass="bcAspdropdown" Enabled="false">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span12" class="bcLabel">Terms Of Delivery & Payment <font color="red" size="2">
                                            <b>*</b></font>: </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTrmDlryPmnt" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                            Enabled="False"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="44%">
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
                                        &nbsp;<%--<asp:TextBox runat="server" ID="txtDcDt" CssClass="bcAsptextbox"></asp:TextBox>--%>
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
                            <asp:HiddenField runat="server" ID="HFRowCount" Value="0" />
                            <asp:GridView runat="server" ID="gvPackingList" AutoGenerateColumns="false" Width="100%"
                                EmptyDataText="No Records To Display...!" OnRowDataBound="gvPackingList_RowDataBound"
                                OnPreRender="gvPackingList_PreRender">
                                <Columns>
                                    <asp:BoundField HeaderText="FPONmbr" DataField="FPONmbr" />
                                    <asp:BoundField HeaderText="S.No." DataField="FPOSno" SortExpression="FPOSno" />
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <img id="imgExpand" runat="server" alt="" style="cursor: pointer" src="../images/Expand.png" />
                                            <div id="dvSubItems" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="IDs" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFrnPoID" runat="server" Text='<%# Eval("ForeignPOId") %>'></asp:Label>
                                            <asp:Label ID="lblsItemDtlsID" runat="server" Text='<%# Eval("StockItemsId") %>'></asp:Label>                                            
                                            <asp:HiddenField ID="HFFPONo" runat="server" Value='<%# Eval("FPONmbr") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item Desc">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfFrnPoID" runat="server" Value='<%# Eval("ForeignPOId") %>' />
                                            <asp:Label ID="lblItemDesc" runat="server" Text='<%# Eval("Description") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="HS-Code">
                                        <ItemTemplate>
                                        <asp:HiddenField ID="hfFrnPoID1" runat="server" value='<%# Eval("ForeignPOId") %>'></asp:HiddenField>
                                            <asp:HiddenField ID="HFSNo" runat="server" Value='<%# Eval("Sno") %>' />
                                            <asp:HiddenField ID="hfItemID" runat="server" Value='<%# Eval("ItemId") %>'></asp:HiddenField>
                                            <asp:Label ID="lblHSCode" runat="server" Text='<%# Eval("HSCode") %>' />
                                            <asp:HiddenField ID="HFGDNID" runat="server" Value='<%# Eval("GDNID") %>' />
                                            <asp:HiddenField ID="HFGRNID" runat="server" Value='<%# Eval("GRNID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Part No">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="HFPkgNos" runat="server" Value='<%# Eval("PkgNos") %>' />
                                            <asp:Label ID="lblPartno" runat="server" Text='<%# Eval("PartNumber") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Make">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="HFPackingBoxFrom" runat="server" Value='<%# Eval("PackingBoxFrom") %>' />
                                            <asp:HiddenField ID="HFPackingBoxTo" runat="server" Value='<%# Eval("PackingBoxTo") %>' />
                                            <asp:Label ID="lblMk" runat="server" Text='<%# Eval("Make") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Eval("Quantity") %>' MaxLength="7"
                                                Style="width: 50px; text-align: right;" onblur="return CheckQuantity(event);"
                                                onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Units" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUnitsNm" runat="server" Text='<%# Eval("UnitNm") %>'></asp:Label>
                                            <asp:HiddenField ID="HFUnitsID" runat="server" Value='<%# Eval("UNumsId") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Net Weight (kgs)" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtNetWeight" runat="server" Text='<%# Eval("NetWeight") %>' MaxLength="18"
                                                Style="width: 85px; text-align: right;" onkeyup="extractNumber(this,2,false);"
                                                onblur="return CheckWeights(event);" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Gr Weight (kgs)" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtGrWeight" runat="server" Text='<%# Eval("GrWeight") %>' MaxLength="18"
                                                Style="width: 85px; text-align: right;" onkeyup="extractNumber(this,2,false);"
                                                onblur="return CheckWeights(event);" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
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
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.rowGrouping.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $("[src*=Expand]").live("click", function () {
            var id = $(this).closest('tr')[0].cells[2].children[0].id;
            var FPOID = $("#" + id).val();

            var hfFESNo = $(this).closest('tr')[0].cells[2].children[1].id;
            var FESNo = $("#" + hfFESNo).val();

            var PItmID = $(this).closest('tr')[0].cells[2].children[2].id;
            var ParentID = $("#" + PItmID).val();
            var result = PackingList.BindSubItems(ParentID, FPOID, FESNo);
                        
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + result.value + "</td></tr>")
            $(this).attr("src", "../images/collapse.png");
        });
        $("[src*=collapse]").live("click", function () {
            $(this).attr("src", "../images/Expand.png");
            $(this).closest("tr").next().remove();
        });

        function SaveChanges(ParentID, RID, Cntrl, IsADD) {
            var fsno = RID;
            var Decimal_ = 1;
            if (RID.indexOf(".") >= 0) {
                fsno = RID.split('.')[0];
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
            var NetWeight = $("#txtNetWeight" + RID).val();
            var GrossWeight = $("#txtGrossWeight" + RID).val();
            if (IsADD == "true" && ItmDesc != "" && Qty == 0) {
                if (ItmDesc == "")
                    ErrorMessage('Item Description is required.');
                else if (qty == 0)
                    ErrorMessage('Quantity is required.');
                else if (Rate == 0)
                    ErrorMessage('Rate is required.');
            }
            else {
                var rslt = PackingList.SaveChanges(ItemID, ParentItemID, FPOID, FESNO, SNO, ItmDesc, HsCode, PartNo, Make, Qty, NetWeight, GrossWeight, IsADD);

                var RowIndex = $(Cntrl).closest('table').parent().parent().index();
                $(Cntrl).closest("table").parent().parent().remove();
                $('[id$=gvPackingList] > tbody > tr').eq(parseInt(RowIndex) - parseInt(Decimal_)).after("<tr><td></td><td colspan = '999'>" + rslt.value + "</td></tr>");
            }
        }

        function Delete_SubItem(RID, Cntrl, ItemID) {
            if (confirm("Do you want to Delete this Row...?")) {
                var PItmID = $(Cntrl).closest('table').parent().parent()[0].previousSibling.cells[1].children[2].id;
                var ParentID = $("#" + PItmID).val();

                var id = $(Cntrl).closest('table').parent().parent()[0].previousSibling.cells[2].children[0].id;
                var FPOID = $("#" + id).val();

                var rslt = PackingList.Delete_SubItem(RID, ItemID, ParentID, FPOID);

                var RowIndex = $(Cntrl).closest('table').parent().parent().index();
                $(Cntrl).closest("table").parent().parent().remove();
                $('[id$=gvPackingList] > tbody > tr').eq(parseInt(RowIndex) - 1).after("<tr><td></td><td colspan = '999'>" + rslt.value + "</td></tr>");
            }
        }

    </script>
    <script type="">
        var Otable = null;
        $(document).ready(function () {
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
            Otable = $('[id$=gvPackingList]').dataTable(
            {
                "aLengthMenu": [[-1], ["All"]],
                "iDisplayLength": -1,
                //"aaSorting": [],
                //"aaSortingFixed": [[0, 'asc']],
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
            }).rowGrouping({ iGroupingColumnIndex: 0,
                sGroupingColumnSortDirection: "asc",
                iGroupingOrderByColumnIndex: 1,
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
                ControlID++;
                var Pkgs = $(this).closest('tr').next('tr')[0].cells[3].children[0].id;
                var PkgsVal = $("#" + Pkgs).val();
                var FromValue = "";
                var ToValue = "";

                //var PkgsValSplit = PkgsVal.split('-');
                //var FromValue01 = PkgsValSplit[0].trim() == undefined ? "" : PkgsValSplit[0].trim();
                //var ToValue01 = PkgsValSplit[1].trim() == undefined ? "" : PkgsValSplit[1].trim();

                var PackingBoxFrom = $(this).closest('tr').next('tr')[0].cells[4].children[0].id;
                FromValue = $("#" + PackingBoxFrom).val().trim();
                //FromValue = $("#" + PackingBoxFrom).val().trim() == "" ? FromValue01 : $("#" + PackingBoxFrom).val().trim();

                var PackingBoxTo = $(this).closest('tr').next('tr')[0].cells[4].children[1].id;
                ToValue = $("#" + PackingBoxTo).val().trim();
                //ToValue = $("#" + PackingBoxTo).val().trim() == "" ? ToValue01 : $("#" + PackingBoxTo).val().trim();

                var ConcVal = "'" + FromValue + ":" + ToValue + "'";
                var iid = $(this).closest('tr').next('tr')[0].cells[1].children[0].id;
                var FPOIDVal = "'" + $("#" + iid).val() + "'";
                var IsDisable = '';
                IsDisable = ControlID == 1 ? " disabled " : "";
                var Validations = 'onkeyup="extractNumber(this,0,false);" onblur="return CheckWeights(event);" onkeypress="return blockNonNumbers(this, event, true, false);"';
                var Controls = 'From:<input ' + IsDisable + '' + Validations + ' type="text" style="width:30px;" maxlength="3" id="txtFrom' + ControlID + '" onchange="FromFPOGroup(this,' + ControlID + ',' + ConcVal + ',' + FPOIDVal + ');" value="' + FromValue + '"/>';
                Controls += ' To:<input ' + Validations + ' type="text" style="width:30px;" maxlength="3" id="txtTo' + ControlID + '" onchange="ToFPOGroup(this,' + ControlID + ',' + ConcVal + ',' + FPOIDVal + ');" value="' + ToValue + '"/>';
                $(this).find('td').prepend($('<span />', { 'class': 'rowCount-grid' })
                .prepend(Controls + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'));
            });

            $('.expandedOrCollapsedGroup').live('click', function () {
                if ($(this).hasClass('collapsed')) {
                    $(this).addClass('expanded').removeClass('collapsed').val('Collapse All Group').parents('.dataTables_wrapper').find('.collapsed-group').trigger('click');
                }
                else {
                    $(this).addClass('collapsed').removeClass('expanded').val('Expanded All Group').parents('.dataTables_wrapper').find('.expanded-group').trigger('click');
                }
            });
        };

        function FromFPOGroup(cntrl, ID, Pkgs, FPOID) {
            var FrmVal = cntrl.value.trim() == "" ? 0 : cntrl.value.trim();
            var ToVal = $("#txtTo" + ID).val().trim();
            var PrevToVal = $("#txtTo" + (ID - 1)).val() == undefined ? FrmVal : $("#txtTo" + (ID - 1)).val().trim();
            var rslt = PackingList.FromFPOGroup(FrmVal, ToVal, PrevToVal, FPOID, Pkgs);
            if (rslt.value == "False") {

                if (ToVal == '') {
                    ErrorMessage('To Value is required.');
                }
                else if (FrmVal == '') {
                    ErrorMessage('From Value is required.');
                }
                else if (parseInt(ToVal) < parseInt(FrmVal)) {
                    ErrorMessage('From Value cannot be greater than To Value.');
                }
                else if (parseInt(PrevToVal) > parseInt(FrmVal)) {
                    ErrorMessage('Previous To Value cannot be greater than From Value.');
                }

                for (var i = ID; i < 100; i++) {
                    if ($("#txtFrom" + i).val() != undefined) {
                        $("#txtFrom" + i).val('');
                        $("#txtTo" + i).val('')
                    }
                    else
                        break;
                }
            }
        }

        function ToFPOGroup(cntrl, ID, Pkgs, FPOID) {
            var ToVal = cntrl.value.trim() == "" ? 0 : cntrl.value.trim();
            var FrmVal = $("#txtFrom" + ID).val().trim();
            var NextFrmVal = $("#txtFrom" + (ID + 1)).val() == undefined ? ToVal : $("#txtFrom" + (ID + 1)).val().trim();
            var rslt = PackingList.ToFPOGroup(FrmVal, ToVal, NextFrmVal, FPOID, Pkgs);
            if (rslt.value == "False") {

                if (FrmVal == '') {
                    ErrorMessage('From Value is required.');
                }
                else if (ToVal == '') {
                    ErrorMessage('To Value is required.');
                }
                else if (parseInt(ToVal) < parseInt(FrmVal)) {
                    ErrorMessage('To Value cannot be less than From Value.');
                }
                else if (parseInt(ToVal) > parseInt(NextFrmVal)) {
                    ErrorMessage('To Value cannot be greater than Next From Value.');
                }

                for (var i = ID; i < 100; i++) {
                    if ($("#txtFrom" + i).val() != undefined) {
                        if (i > ID)
                            $("#txtFrom" + i).val('');
                        $("#txtTo" + i).val('')
                    }
                    else
                        break;
                }
            }
        }

    </script>
    <script type="text/javascript">

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
            $('[id$=txtPkngListNoDT]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        });

        function CheckWeights(evt) {
            if ((parseFloat(($('[id$=txtGrWeight]').val()).trim())) < (parseFloat(($('[id$=txtNetWeight]').val()).trim()))) {
                ErrorMessage('Net Weight should not be greaterthan Gross Weight.');
//                $('[id$=txtNetWeight]').val('');
//                $('[id$=txtNetWeight]').focus();
                return false;
            }
            else
                return true;
        }
        function CheckQuantity(ev) {
            var Quantit = ($('[id$=txtQuantity]').val()).trim();
            if (Quantit == '') {
                ErrorMessage('Quantity cannot be Empty.');
                return false;
            }
            if (Quantit == 0) {
                ErrorMessage('Quantity cannot be Zero.');
                return false;
                }
                
        }

        function chkAllCheckbox(obj) {
            var gv = $("#<%=gvPackingList.ClientID %> input");
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
            var result = PackingList.AddItemListBox();
            var getDivPLItems = GetClientID("divListBox").attr("id");
            $('#' + getDivPLItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                //$get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If U need Upload New File.";
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
            //$get("<%=lblstatus.ClientID%>").innerHTML = "File upload Failed.";
            ErrorMessage('File upload Failed.');
        }
        function uploadStarted() {
            // $get("<%=lblstatus.ClientID%>").innerHTML = "File upload started.";
            SuccessMessage('File Uploaded Started.');
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != "") {
                if (confirm("Are you sure you want to delete the item")) {
                    var result = PackingList.DeleteItemListBox($('#lbItems').val());
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
                if ($('[id$=gvPackingList]').length != 0)
                    var GdnItems = $('[id$=gvPackingList]')[0].rows.length;
                GdnItems = $('[id$=HFRowCount]').val();
                if (($('[id$=ddlChkLst]').val()).trim() == '' || ($('[id$=ddlChkLst]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Shipment Planning Number is Required.');
                    $('[id$=ddlChkLst]').focus();
                    return false;
                } //ddlRefno
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
                else if (($('[id$=txtPkngListNo]').val()).trim() == '') {
                    ErrorMessage('Proforma Invoice Number is Required.');
                    $('[id$=txtPkngListNo]').focus();
                    return false;
                }
                else if (($('[id$=txtPkngListNoDT]').val()).trim() == '') {
                    ErrorMessage('Proforma Invoice Date is Required.');
                    $('[id$=txtPkngListNoDT]').focus();
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
                    //                    else {
                    //                        var select = 0;
                    //                        for (var i = 1; i < GdnItems; i++) {
                    //                            var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                    //                            var Qty = GetClientID(chkbx + "_txtQuantity").attr("id");
                    //                            var NetWeight = GetClientID(chkbx + "_txtNetWeight").attr("id");
                    //                            var GrWeight = GetClientID(chkbx + "_txtGrWeight").attr("id");
                    //                            var Quantity = $('#' + Qty).val();
                    //                            if (Quantity == 0 || Quantity == "") {
                    //                                ErrorMessage('Quantity cannot be ' + Quantity == 0 ? 'Zero.' : 'Empty.');
                    //                                return false;
                    //                            }

                    //                            if ($('#' + NetWeight).length) {
                    //                                var Net = $('#' + NetWeight).val();
                    //                                var Gr = $('#' + GrWeight).val();

                    //                                if (Net == 0 || Net == "") {
                    //                                    ErrorMessage('NetWeight cannot be ' + Net == 0 ? 'Zero.' : 'Empty.');
                    //                                    $('#' + NetWeight).focus();
                    //                                    return false;
                    //                                }
                    //                                if (Gr == 0 || Gr == "") {
                    //                                    ErrorMessage('GrossWeight cannot be ' + Gr == 0 ? 'Zero.' : 'Empty.');
                    //                                    $('#' + GrWeight).focus();
                    //                                    return false;
                    //                                }
                    //                                if (parseInt(Net) > parseInt(Gr)) {
                    //                                    ErrorMessage('Net Weight can not be greator than Gross Weight');
                    //                                    $('#' + NetWeight).focus();
                    //                                    return false;
                    //                                }
                    //                            }
                    //                        }
                    ////                        var Result = PackingList.Check_Q_NW_GW();
                    ////                        return false;
                    //                    }
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
