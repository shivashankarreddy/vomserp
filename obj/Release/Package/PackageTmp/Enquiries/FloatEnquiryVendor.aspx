<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FloatEnquiryVendor.aspx.cs" Inherits="VOMS_ERP.Enquiries.FloatEnquiryVendor" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<link href="../JScript/tooltip/jquery.powertip.css" rel="stylesheet" type="text/css" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <asp:HiddenField ID="HselectedItems" runat="server" Value="" />
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Floated Enquiry Vendor" CssClass="bcTdTitleLabel"></asp:Label><div
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
                                    <td class="bcTdnormal">
                                        <span id="lblCustName" class="bcLabel">Name of Customer<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlcustmr" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <%--<asp:ListItem Value="0" Text="Select Customer"></asp:ListItem>--%>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblEnqNo" class="bcLabel">Enquiry Number <font color="red" size="2"><b>*</b>
                                        </font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlfenqy" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlfenqy_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Value="0" Text="--Select EnquiryNo--"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblLocalenqno" class="bcLabel">Local Enquiry Number <font color="red" size="2">
                                            <b></b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtenqno" ValidationGroup="D" CssClass="bcAsptextbox"
                                            ReadOnly="true" ToolTip="Local Enquiry Number is Automatically Generated on Saving...!"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblfedt" class="bcLabel">Foreign Enquiry Date:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfedt" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblIssdt" class="bcLabel">FE Issue Date <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfenqdt" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onchange="SetFEdate();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblRecvDt" class="bcLabel">FE Due Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtfeduedt" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblSubject" class="bcLabel">Subject<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtsubject" onkeypress="return isSomeSplChar(event)"
                                            ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblDept" class="bcLabel">Department Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="Ddldeptnm" ValidationGroup="D" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                   <%-- <td class="bcTdnormal">
                                        <span id="lblEnquiry" class="bcLabel">Supplier Category<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" CssClass="bcAspdropdown" ID="ddlsuplrctgry" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlsuplrctgry_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Select Category"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>--%>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Vendor Name(s)<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lbvendors" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox">
                                            <%--<asp:ListItem Value="0" Text="Select"></asp:ListItem>--%>
                                        </asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="41%">
                                                        <span id="Span1" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
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
                                        <%-- <ajax:ToolkitScriptManager ID="Scriptmanager1" runat="server" />--%>
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="UserAccordion" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane3" runat="server" Width="98%">
                                                        <Header>
                                                            <a href="#" class="href">Attachments</a> &nbsp;<asp:Image runat="server" ID="imgAtchmt"
                                                                AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif" Visible="false" />
                                                        </Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel2" runat="server" Width="97%">
                                                                <table>
                                                                    <tr>
                                                                        <td valign="top">
                                                                            Attached Files :
                                                                        </td>
                                                                        <td valign="top">
                                                                            <asp:Panel ID="pnlattachemnts" runat="server">
                                                                            </asp:Panel>
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;&nbsp;&nbsp;
                                                                        </td>
                                                                        <td valign="top">
                                                                            <asp:Panel ID="pnlDelAttachments" runat="server">
                                                                            </asp:Panel>
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
                                        &nbsp;&nbsp;&nbsp;Add Items
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <div style="overflow: auto; width: 100%;">
                                                        <div id="divGridItems" style="max-height: 250px; min-height: 200px;" runat="server"
                                                            width="100%" height="220px">
                                                        </div>
                                                    </div>
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
                                <tr>
                                    <td colspan="6" align="right">
                                        <center>
                                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                                <tbody>
                                                    <tr valign="middle">
                                                        <td align="center" valign="middle" class="bcTdButton" style="visibility: hidden">
                                                            <div id="Div4" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnsavenew" Text="Save & New" OnClick="btnsavenew_Click" />
                                                            </div>
                                                        </td>
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
                                                                <%--<asp:Button runat="server" ID="btnExit" OnClientClick="Javascript:redirect()" Text="Exit"/>--%>
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/tooltip/jquery.powertip.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });

        });
        //        function toolTip(id) {
        //            var indx = id;


        //            var itmid = GetClientID("HItmID" + indx).attr("id");
        //            var sv = $('#' + itmid).val();
        //            var result = LEVendors.GetHistoryItems(sv);


        //            $('#History' + indx).data('powertipjq', $(result.value));
        //            $('#History' + indx).powerTip({
        //                followMouse: true,
        //                mouseOnToPopup: true,
        //                smartPlacement: true,
        //                placement: 'e'
        //            });


        //        }



        $(document).ready(function () {
            Expnder();
        });
        function Expnder() {
            //$('div.expanderR').expander();
        }



        function Download() {
            if ($('#lbItems').val() != "") {
                var list = document.getElementById('lbItems');
                var indx = list.selectedIndex;
                FloatEnquiryVendor.DownloadItemListBox(list[indx].text);
            }
        }

        function doConfirm(id) {
            if (confirm("Are you sure you want to Continue?")) {
                var result = FloatEnquiryVendor.DeleteItem(id);
                var getDivFEItems = GetClientID("divGridItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
            }
            else {
                return false;
            }
            Expnder();
        }

        $('[id$=ItmADD]').click(function () {
            alert("Handler for .click() called.");
            ItmADD = true;
        });

        function CheckAllBoxs() {
            var CheckAll = $('#ckhMain').is(':checked');
            var result = FloatEnquiryVendor.CheckAll(CheckAll);
            var getDivFEItems = GetClientID("divGridItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var ErrMsg = $('[id$=TblErrorMessage]').val();
            if (ErrMsg != "") {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">' + ErrMsg + '.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            }
            Expnder();
        }

        var ItmADD = false;
        function AddItemColumn1(id) {
            ItmADD = true;
            AddItemColumn(id);
        }

        var returnVal = "";
        var flag = 0;
        function fnOpen(id, rowIndex) {
            returnVal = window.showModalDialog("../Enquiries/AddItems.aspx", "Add Item",
            "dialogHeight:680px; dialogWidth:940px; dialogLeft:150; dialogright:150; dialogTop:150; ");
            if (returnVal != null) {
                //returnVal = "";
                flag = 1;
                AddItemColumn(id);
                returnVal = "";
                flag = 0;
            }
            else
                returnVal = "";
        }
        function AddItemColumn(id) {
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
            var txtPrtNo = GetClientID("txtPrtNo" + (parseInt(id))).attr("id");
            var PrtNo = $('#' + txtPrtNo).val();
            var txtRmrks = GetClientID("txtRemarks" + (parseInt(id))).attr("id");
            var Rmrks = $('#' + txtRmrks).val();
            var Check = $('#ckhChaild' + id).is(':checked');

            if (sv == undefined && Qty == '') {
                var txtQty = GetClientID("txtQuantity" + (parseInt(id))).attr("id");
                $('#' + txtQty).val('0');
                $('#' + txtQty).focus();
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Quantity is Required.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                if (ddlUnits != undefined) {
                    $("#" + ddlUnits + " option[value='0']").attr("selected", "selected");
                }
            }
            else if (sv == undefined && Units == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Units is Required.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                if (ddlUnits != undefined) {
                    $("#" + ddlUnits + " option[value='0']").attr("selected", "selected");
                }
            }
            else if (sv != undefined && sv == 0 && flag == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Select Item Description.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            }
            else {
                if (returnVal != '')
                    sv = returnVal;
                if (sv == undefined)
                    sv = '';
                if (PrtNo == undefined)
                    PrtNo = '';
                if (Units == undefined)
                    Units = '';
                if ((Qty != '' && Qty != '.')) {
                    var result = FloatEnquiryVendor.AddNewRow(id, sv, PrtNo, spec, Make, Qty, Units, Rmrks, Check, ItmADD);
                    var getDivFEItems = GetClientID("divGridItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    ItmADD = false;
                }
                else {
                    if (Qty == '' || Qty == '.') {
                        var txtqty = GetClientID("txtQuantity" + (parseInt(id))).attr("id");
                        $('#' + txtqty).val('0');
                        $('#' + txtqty).focus();
                        $("#<%=divMyMessage.ClientID %> span").remove();
                        $('[id$=divMyMessage]').append('<span class="Error">Quantity Cannot Be Empty.</span>');
                        $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    }
                }
            }
            var ErrMsg = $('[id$=TblErrorMessage]').val();
            //alert(ErrMsg);
            if (ErrMsg != "") {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">' + ErrMsg + '.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            }
            Expnder();
        }


        function FillItemDRP(id) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = FloatEnquiryVendor.FillItemDRP(id, sv);
            var getDivFEItems = GetClientID("divGridItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        function FillUnitDRP(id) {
            var ddlCat = GetClientID("ddlU" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = FloatEnquiryVendor.FillUnitDRP(id, sv);
            var getDivFEItems = GetClientID("divGridItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        function Myvalidations() {
            var rowCount = $('#tblItems tbody tr').length; var count = 0;
            for (var i = 1; i <= rowCount; i++) {

                var units = $('#ddlUnits' + '' + i).val();
                var qty = $('#txtQuantity' + '' + i).val();
                var Hid = $('#HItmID' + '' + i).val(); //HItmID
                var isChecked = $('#ckhChaild' + i).is(':checked');
                if (!isChecked) {
                    //alert('Row Count : ' + rowCount + ', J val : ' + i + ', IsSelected : ' + isChecked);
                    var notSelected = notSelected + ',' + Hid;
                    count = count + 1;
                    $('[id$=HselectedItems]').val(notSelected);
                }
                //alert('Units : ' + units + ', qty : ' + qty);
                if (isChecked && ($('#txtQuantity' + '' + i).val() == '0' || $('#txtQuantity' + '' + i).val() == '')) {
                    //alert('sample : ' + $('#ddlUnits' + '' + i).val());
                    var message = '';
                    if (qty == '')
                        message = 'Quantity is Required';
                    else if (qty == '0')
                        message = 'Quantity Cannot Be Zero';
                    else if (units == 0)
                        message = 'Units is Required';
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">' + message + ' of SNo : ' + i + '.</span>');
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    return false;
                    break;
                }
            }
            if (($('[id$=ddlcustmr]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer Name is Required.</span>');
                $('[id$=ddlcustmr]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlfenqy]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Enquiry number is Required.</span>');
                $('[id$=ddlfenqy]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
//            else if (($('[id$=txtlenqdt]').val()).trim() == '') {
//                $("#<%=divMyMessage.ClientID %> span").remove();
//                $('[id$=divMyMessage]').append('<span class="Error">Local Enquiry Issue Date is Required.</span>');
//                $('[id$=txtlenqdt]').focus();
//                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
//                return false;
//            }
//            else if (($('[id$=txtleduedt]').val()).trim() == '') {
//                $("#<%=divMyMessage.ClientID %> span").remove();
//                $('[id$=divMyMessage]').append('<span class="Error">Response Due Date is Required.</span>');
//                $('[id$=txtleduedt]').focus();
//                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
//                return false;
//            }
            else if (($('[id$=txtsubject]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Subject is Required.</span>');
                $('[id$=txtsubject]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=Ddldeptnm]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Project/Department Name is Required.</span>');
                $('[id$=Ddldeptnm]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            //            else if (($('[id$=ddlsuplrctgry]').val()).trim() == '0') {
            //                $("#<%=divMyMessage.ClientID %> span").remove();
            //                $('[id$=divMyMessage]').append('<span class="Error">Supplier Catagory is Required.</span>');
            //                $('[id$=ddlsuplrctgry]').focus();
            //                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            //                return false;
            //            }
            else if (($('[id$=lbvendors]').val() == null)) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Vendor Name is Required.</span>');
                $('[id$=lbvendors]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (count == rowCount) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Select Atleast 1 Item.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (rowCount <= 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">No Items to Save</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
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
            else {
                return true;
            }
        }

        function ExpandTXT(obj) {
            $('#txtSpecifications' + '' + obj).animate({ "height": "75px" }, "slow");
            $('#txtSpecifications' + '' + obj).slideDown("slow");
        }

        function ReSizeTXT(obj) {
            $('#txtSpecifications' + '' + obj).animate({ "height": "20px" }, "slow");
            $('#txtSpecifications' + '' + obj).slideDown("slow");
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function FillItemsAll(obj) {
        }
        function FillGrid() {
            var result = FloatEnquiryVendor.BindGridView($('[id$=ddlfenqy]').val());
            var GetdivGridItems = GetClientID("divGridItems").attr("id");
            $('#' + GetdivGridItems).html(result.value);
            Expnder();
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = FloatEnquiryVendor.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
        });

        function ShowPopup(id, rowIndex) {

            var options = {
                title: "Add User Account",
                width: 950,
                height: 400,
                url: "/../Home.aspx"
            };
            SP.UI.ModalDialog.showModalDialog(options);
            return false;
        }
        function fnRandom(iModifier) {
            return parseInt(Math.random() * iModifier);
        }

        function fnSetValues() {
            var iHeight = 300;
            var iWidth = 1000;
            var sFeatures = "dialogHeight: " + iHeight + "px; dialogWidth: " + iWidth + "px;";
            return sFeatures;
        }

        $(document).ready(function () {
            $('div.expanderR').expander();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }
    </script>

</asp:Content>
