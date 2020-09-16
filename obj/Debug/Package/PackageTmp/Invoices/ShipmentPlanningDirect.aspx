<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ShipmentPlanningDirect.aspx.cs" Inherits="VOMS_ERP.Invoices.ShipmentPlanningDirect" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="BIVAC SHIPMENT PLANNING"
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
                                                            <a href="#" class="href">Header</a> &nbsp;<asp:Image runat="server" ID="imgAtchmt"
                                                                AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif" Visible="false" />
                                                        </td>
                                                        <td align="center">
                                                            <asp:Label ID="lblBlink" runat="server" Text="Click here" CssClass="blinkytext"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </Header>
                                            <Content>
                                                <asp:Panel ID="Panel2" runat="server" Width="98%">
                                                    <table>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span class="bcLabel">Notify :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:DropDownList runat="server" ID="ddlNtfy" CssClass="bcAspdropdown">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span5" class="bcLabel">Country of Origin of Goods<font color="red" size="2"><b>*</b></font>
                                                                    :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:DropDownList runat="server" ID="ddlPlcOrgGds" CssClass="bcAspdropdown">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span7" class="bcLabel">Country of Final Destination<font color="red" size="2"><b>*</b></font>
                                                                    :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:DropDownList runat="server" ID="ddlPlcFnlDstn" CssClass="bcAspdropdown">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span id="Span10" class="bcLabel">Port Of Loading<font color="red" size="2"><b>*</b></font>
                                                                    : </span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:DropDownList runat="server" ID="ddlPrtLdng" CssClass="bcAspdropdown">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span13" class="bcLabel">Port Of Discharge<font color="red" size="2"><b>*</b></font>
                                                                    : </span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:DropDownList runat="server" ID="ddlPrtDscrg" CssClass="bcAspdropdown">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span15" class="bcLabel">Place of Delivery<font color="red" size="2"><b>*</b></font>
                                                                    :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:DropDownList runat="server" ID="ddlPlcDlry" CssClass="bcAspdropdown">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span id="Span8" class="bcLabel">Pre-Carriage by :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtPCrBy" CssClass="bcAsptextbox"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span9" class="bcLabel">Place of receipt by pre-Carrier :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtPlcRcptPCr" CssClass="bcAsptextbox"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span11" class="bcLabel">Vessel / Flight No. :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtVslFlt" CssClass="bcAsptextbox"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span id="Span12" class="bcLabel">Terms Of Delivery and Payment <font color="red"
                                                                    size="2"><b>*</b></font>: </span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtTrmDlryPmnt" TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span class="bcLabel">Incoterm :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:DropDownList runat="server" ID="ddlIncoTrm" CssClass="bcAspdropdown">
                                                                                <asp:ListItem Text="Select Price Basis" Value="0"></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox runat="server" CssClass="bcAsptextboxRight" ID="txtPriceBasis"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span id="Span1" class="bcLabel">Other References :</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtOtrRfs" TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
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
                                        <span id="lblCustName" class="bcLabel">Name of Customer<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td valign="top">
                                        <asp:ListBox ID="ListBoxCustomer" runat="server" SelectionMode="Multiple" AutoPostBack="True"
                                            CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ListBoxCustomer_SelectedIndexChanged">
                                        </asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="ListBoxFpos" Enabled="true" SelectionMode="Multiple"
                                            OnSelectedIndexChanged="ListBoxFpo_SelectedIndexChanged" CssClass="bcAspMultiSelectListBox"
                                            AutoPostBack="true"></asp:ListBox>
                                    </td>
                                    <td align="right">
                                        <span class="bcLabel">Shipment Mode<font color="red" size="2"><b>*</b></font>:</span>
                                        <br />
                                        <br />
                                        <span id="Span3" class="bcLabel">Shipment Planning Ref No.:</span>
                                    </td>
                                    <td align="left">
                                        <asp:RadioButtonList ID="rbtnshpmnt" runat="server" RepeatDirection="Horizontal"
                                            ForeColor="#000000" Font-Size="11px" font-family="Arial" AutoPostBack="True"
                                            OnSelectedIndexChanged="rbtnshpmnt_SelectedIndexChanged">
                                            <%--<asp:ListItem Text="By Air" Value="303"></asp:ListItem>
                                            <asp:ListItem Text="By Sea" Value="304" Selected="True"></asp:ListItem>--%>
                                            <%--<asp:ListItem Text="By Air" Value="F180B3B3-25A8-4ED6-8459-CFA232A9970B"></asp:ListItem>
                                            <asp:ListItem Text="By Sea" Value="65D23EF5-DE0A-492A-A81A-1FAF9A4A4CC5" Selected="True"></asp:ListItem>--%>
                                        </asp:RadioButtonList>
                                        <br />
                                        <asp:TextBox runat="server" ID="txtRefNo" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span id="Span6" class="bcLabel">Supplier Invoice :</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtSupInv" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="lblImpInstructions" class="bcLabel">Important Instructions :</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtImpInstructions" CssClass="bcAsptextboxmulti"
                                            TextMode="MultiLine"></asp:TextBox>
                                        <asp:Label ID="Erms" runat="server" align="center"></asp:Label>
                                    </td>
                                    <td colspan="2" class="bcTdnormal">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="40%">
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
                                    <td colspan="6" class="bcTdNewTable" align="right">
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <div id="divItems" runat="server" style="width: 100%">
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" align="right">
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
                                                                <asp:LinkButton runat="server" ID="btnclear" OnClientClick="Javascript:clearAll()"
                                                                    Text="Clear" OnClick="btnclear_Click" />
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
            </td>
        </tr>
    </table>
    <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_scrollBody
        {
            height: 150px !important;
            overflow-y: scroll !important;
            overflow: -moz-scrollbars-vertical !important;
            overflow: scroll !important;
        }
        tr.row_selected td
        {
            background: #FFCF8B !important;
        }
    </style>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
    </script>
    <script type="text/javascript">
        function GetSeaOrAir(CtrlID) {
            var val = $('#' + CtrlID + ' input:checked').val();
            var res = (val == 303 ? 'Air' : 'Sea');
            var result = ShipmentPlanningDirect.GetCount(val);
            var txtval = $('[id$=txtRefNo]').val().split("/");
            $('[id$=txtRefNo]').val(txtval[0] + '/' + res + '/' + result.value + '/' + txtval[3]);
        }

        function OpenAccordion() {
            $find('ctl00_ContentPlaceHolder1_UserAccordion_AccordionExtender').set_SelectedIndex(0);
        }

        function Myvalidations() {
            if (($('[id$=ddlPlcOrgGds]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Country of origin of goods is Required.');
                $('[id$=ddlPlcOrgGds]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=ddlPlcFnlDstn]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Country of Final Destination is Required.');
                $('[id$=ddlPlcFnlDstn]').focus();
                OpenAccordion();
                return false;
            }

            else if (($('[id$=ddlPrtLdng]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Port of Loading is Required.');
                $('[id$=ddlPrtLdng]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=ddlPrtDscrg]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Port of Discharge is Required.');
                $('[id$=ddlPrtDscrg]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=ddlPlcDlry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Place of Delivery is Required.');
                $('[id$=ddlPlcDlry]').focus();
                OpenAccordion();
                return false;
            }
            else if (($('[id$=txtTrmDlryPmnt]').val()).trim() == '') {
                ErrorMessage('Terms Of Delivery and Payment is Required.');
                $('[id$=txtTrmDlryPmnt]').focus();
                OpenAccordion();
                return false;
            }

            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            else if ($('#ctl00_ContentPlaceHolder1_ListBoxCustomer :selected').length == 0) {
                ErrorMessage('Customer is Required.');
                $('[id$=ListBoxCustomer]').focus();
                return false;
            }

            else if ($('#ctl00_ContentPlaceHolder1_ListBoxFpos :selected').length == 0) {
                ErrorMessage('Foreign PO(s) is Required.');
                $('[id$=ListBoxFpos]').focus();
                return false;
            }
            var rowCount = $('#tblSPDItems tbody tr td').length;
            if (rowCount <= 1) {
                ErrorMessage('Add atleast one FPO NO to Shipment Planning in the grid');
                return false;
            }

        }
    </script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {
            DesignGrid();
        });

        function DesignGrid() {
            $("[id$=tblSPDItems]").dataTable({
                "bScrollCollapse": true,
                "bPaginate": false,
                "bSort": false,
                "bFilter": false, //to disable search Box 
                "bInfo": false,
                "bJQueryUI": true,
                "bAutoWidth": false,

                //Scrolling--------------
                "sScrollY": "700px",
                "sScrollX": "100%",
                "sScrollXInner": "100%"
                //------------
            });
            /* Init the table */
            oTable = $('#tblSPDItems').dataTable();
        }

        function fnGetSelected(oTableLocal) {
            return oTableLocal.$('tr.row_selected');
        }

        function CheckWeights(evt) {
            if ((parseFloat(($('[id$=txtGrWeight0]').val()).trim())) < (parseFloat(($('[id$=txtNetWeight0]').val()).trim()))) {
                ErrorMessage('Net Weight should not be greaterthan Gross Weight.');
                $('[id$=txtNetWeight0]').val('');
                $('[id$=txtGrWeight0]').val('');
                $('[id$=txtNetWeight0]').focus();
                return false;
            }
            else
                return true;
        }

        function EditSelectedItem(RowID) {

            var SelID = Number(RowID) + 1;
            $('#lblEditID0').text(SelID);
            var FPOID = $('#hfFPOID' + SelID).val();
            $('#ddlFPO0').val(FPOID);
            var SupID = $('#hfSUPID' + SelID).val();
            FillSuppliers(0);
            $('#ddlSup0').val(SupID);
            var result = ShipmentPlanningDirect.EditRow(RowID);
            var arr = result.value.split('~^~');

            $('#btnaddItem').hide();
            $('#btnEditItem').show();
            $('#btnCancel').show();

            $('#hfCustID0').val(arr[0]);
            $('#txtNoOfPkgs0').val(arr[1]);
            $('#txtGodownRcptNo0').val(arr[2]);
            if (arr[3] == "True")
                $("#ChkIsAreOne0").prop('checked', true);
            else
                $("#ChkIsAreOne0").prop('checked', false);
            $('#txtNetWeight0').val(arr[4]);
            $('#txtGrWeight0').val(arr[5]);
            $('#txtRemarks0').val(arr[6]);
        }


        function UpdateSelectedItem() {
            var EDitID = $('#lblEditID0').text();
            var FPOID = $('#ddlFPO0').val();
            var FPONo = $("#ddlFPO0 option[value='" + FPOID + "']").text();
            var SupID = $('#ddlSup0').val();
            var SupNo = $("#ddlSup0 option[value='" + SupID + "']").text();
            var CustID = $('#hfCustID0').val();
            var NoOfPkgs = $('#txtNoOfPkgs0').val();
            var GodonRcptNo = $('#txtGodownRcptNo0').val();
            var IsARE1 = $('#ChkIsAreOne0').prop('checked');
            var NetWeight = $('#txtNetWeight0').val() == '' ? 0 : $('#txtNetWeight0').val();
            var GrWeight = $('#txtGrWeight0').val() == '' ? 0 : $('#txtGrWeight0').val();
            var remarks = $('#txtRemarks0').val();
            if (FPOID != 0 && SupID != 0 && parseInt(NoOfPkgs) > 0 && parseInt(NetWeight) > 0 && parseInt(GrWeight) > 0) {
                var result = ShipmentPlanningDirect.UpdateSelectedItem(EDitID, FPOID, FPONo, SupID, SupNo, CustID, NoOfPkgs, GodonRcptNo, IsARE1, NetWeight, GrWeight, remarks);
                $('#tblSPDItems tbody').html('');
                $('#tblSPDItems tbody').append(result.value);

                var Count = (Number($('#tblSPDItems tbody tr').length));
                $('#btnDel' + Count).focus();
                $('#lblEditID0').hide();
                $('#btnaddItem').show();
                $('#btnEditItem').hide();
                $('#btnCancel').show();
                $('#ddlFPO0').val('0');
                $('#ddlSup0').val('0');
                $('#hfCustID0').val('');
                $('#txtNoOfPkgs0').val('');
                $('#txtGodownRcptNo0').val('');
                $("#ChkIsAreOne0").prop('checked', false);
                $('#txtNetWeight0').val('');
                $('#txtGrWeight0').val('');
                $('#txtRemarks0').val('');
            }
            else {
                if (FPOID == 0) {
                    ErrorMessage('FPOID is required.');
                    $('#ddlFPO0').focus();
                }
                else if (SupID == 0) {
                    ErrorMessage('Supplier Id is required.');
                    $('#ddlSup0').focus();
                }
                else if (NoOfPkgs == 0 || NoOfPkgs == '') {
                    ErrorMessage('No of Packages cannot be empty / zero.');
                    $('#txtNoOfPkgs0').focus();
                }
                else if (parseInt(NetWeight) == "NaN" || parseInt(NetWeight) == 0 || NetWeight == '') {
                    ErrorMessage('Net weight cannot be zero / Empty.');
                    $('#txtNetWeight0').focus();
                }
                else if (parseInt(GrWeight) == "NaN" || parseInt(GrWeight) == 0 || GrWeight == '') {
                    ErrorMessage('Gross weight cannot be zero / Empty.');
                    $('#txtGrWeight0').focus();
                }
            }
        }

        function FillSuppliers(ID) {
            var ItemID = $('#ddlFPO' + ID).val();
            if (ItemID != '0') {
                var result = ShipmentPlanningDirect.FillSuppliers(ItemID);
                var val = result.value.split('~^~');
                $('#ddlSup0').find('option').remove();
                $('#ddlSup0').append($('<option></option>').val('0').html('--Select--'));

                var custID = val[1];
                $('#hfCustID0').val(custID);

                var obj = JSON.parse(val[0]);
                $(obj).each(function (index, item) {
                    $('#ddlSup0').append($('<option></option>').val(item.ID).html(item.Description));
                });
            }
        }

        function AddItemRow() {
            var FPOID = $('#ddlFPO0').val();
            var FPONo = $("#ddlFPO0 option[value='" + FPOID + "']").text();
            var SupID = $('#ddlSup0').val();
            var SupNo = $("#ddlSup0 option[value='" + SupID + "']").text();
            var CustID = $('#hfCustID0').val();
            var NoOfPkgs = $('#txtNoOfPkgs0').val();
            var GodonRcptNo = $('#txtGodownRcptNo0').val();
            var IsARE1 = $('#ChkIsAreOne0').prop('checked');
            var NetWeight = $('#txtNetWeight0').val() == '' ? 0 : $('#txtNetWeight0').val();
            var GrWeight = $('#txtGrWeight0').val() == '' ? 0 : $('#txtGrWeight0').val();
            var remarks = $('#txtRemarks0').val();
            if (FPOID != 0 && SupID != 0 && parseInt(NoOfPkgs) > 0 && parseInt(NetWeight) > 0 && parseInt(GrWeight) > 0) {
                var result = ShipmentPlanningDirect.AddItem(FPOID, FPONo, SupID, SupNo, CustID, NoOfPkgs, GodonRcptNo, IsARE1, NetWeight, GrWeight, remarks);
                $('#tblSPDItems tbody').html('');
                $('#tblSPDItems tbody').append(result.value);

                var Count = (Number($('#tblSPDItems tbody tr').length));
                $('#btnDel' + Count).focus();

                $('#ddlFPO0').val('0');
                $('#ddlSup0').val('0');
                $('#hfCustID0').val('');
                $('#txtNoOfPkgs0').val('');
                $('#txtGodownRcptNo0').val('');
                $("#ChkIsAreOne0").prop('checked', false);
                $('#txtNetWeight0').val('');
                $('#txtGrWeight0').val('');
                $('#txtRemarks0').val('');
                $('#lblEditID0').text('');
            }
            else {
                if (FPOID == 0) {
                    ErrorMessage('FPOID is required.');
                    $('#ddlFPO0').focus();
                }
                else if (SupID == 0) {
                    ErrorMessage('Supplier Id is required.');
                    $('#ddlSup0').focus();
                }
                else if (NoOfPkgs == 0 || NoOfPkgs == '') {
                    ErrorMessage('No of Packages cannot be empty / zero.');
                    $('#txtNoOfPkgs0').focus();
                }
                else if (parseInt(NetWeight) == "NaN" || parseInt(NetWeight) == 0 || NetWeight == '') {
                    ErrorMessage('Net weight cannot be zero / Empty.');
                    $('#txtNetWeight0').focus();
                }
                else if (parseInt(GrWeight) == "NaN" || parseInt(GrWeight) == 0 || GrWeight == '') {
                    ErrorMessage('Gross weight cannot be zero / Empty.');
                    $('#txtGrWeight0').focus();
                }
            }
        }

        function CancelEdit() {
            $('#btnaddItem').show();
            $('#btnEditItem').hide();

            $('#ddlFPO0').val('0');
            $('#ddlSup0').val('0');
            $('#hfCustID0').val('');
            $('#txtNoOfPkgs0').val('');
            $('#txtGodownRcptNo0').val('');
            $("#ChkIsAreOne0").prop('checked', false);
            $('#txtNetWeight0').val('');
            $('#txtGrWeight0').val('');
            $('#txtRemarks0').val('');
            $('#lblEditID0').text('');
        }

        function doConfirm(id) {
            if (confirm("Are you sure you want to Continue?")) {
                var result = ShipmentPlanningDirect.DeleteItem(id);
                $('[id$=divItems]').html('');
                $('[id$=divItems]').html(result.value);
                DesignGrid();
            }
        }

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
    </table>
</asp:Content>
