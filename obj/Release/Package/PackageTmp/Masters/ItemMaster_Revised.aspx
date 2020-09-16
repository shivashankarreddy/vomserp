<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ItemMaster_Revised.aspx.cs" Inherits="VOMS_ERP.Masters.ItemMaster_Revised" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hdfdItmMstrID" />
    <asp:HiddenField runat="server" ID="hdSubCatgryItmMstrID" />
    <asp:HiddenField runat="server" ID="hdSubSubCatryItmMrID" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="New Item Master" CssClass="bcTdTitleLabel"> </asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdNewTable">
                                        <asp:UpdatePanel ID="UpdtPnlItems" runat="server">
                                            <ContentTemplate>
                                                <asp:HiddenField ID="hfEditItemCode" Value="" runat="server" />
                                                <asp:HiddenField ID="HFItemCatCodeIsSpare" Value="False" runat="server" />
                                                <asp:HiddenField ID="HFItemSubCatCodeIsSpare" Value="False" runat="server" />
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Category<font color="red" size="2"><b>*</b></font>: </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:DropDownList runat="server" ID="ddlItmCtgry" CssClass="bcAspdropdown" onchange="fn_codeclear();"
                                                                OnSelectedIndexChanged="ddlItmCtgry_SelectedIndexChanged" AutoPostBack="true">
                                                                <asp:ListItem Text="Select ItemCategory" Value="0"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Sub Category<font color="red" size="2"><b>*</b></font>: </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:DropDownList runat="server" ID="ddlItmSubCatgry" CssClass="bcAspdropdown" onchange="fn_codeclear();"
                                                                OnSelectedIndexChanged="ddlItmSubCatgry_SelectedIndexChanged" AutoPostBack="true"
                                                                Enabled="false">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Sub Sub / Spares Category</span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:DropDownList runat="server" ID="ddlItmSubSubCatgry" CssClass="bcAspdropdown"
                                                                onchange="fn_codeclear();" Enabled="false" OnSelectedIndexChanged="ddlItmSubSubCatgry_SelectedIndexChanged"
                                                                AutoPostBack="true">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Category Code<font color="red" size="2"><b>*</b></font>: </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:Label ID="lblCode" runat="server" Text="" Font-Bold="true"></asp:Label>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Sub Category Code<font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:Label ID="lblSCode" runat="server" Text="" Font-Bold="true"></asp:Label>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Sub Sub / Spares category Code: </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:Label ID="lblSSCode" runat="server" Text="" Font-Bold="true"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Item Code<font color="red" size="2"><b>*</b></font>: </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:TextBox runat="server" ID="txtItmCode" onfocus="this.select()" MaxLength="7"
                                                                onkeyup="Search_ItemCode();" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                                onMouseUp="return false" CssClass="bcAsptextbox" onchange="SearchItmCode();"
                                                                Enabled="false"></asp:TextBox>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Description<font color="red" size="2"><b>*</b></font>: </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:TextBox runat="server" ID="txtItmDscrip" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                                                onkeyup="SearchDesc()"></asp:TextBox>
                                                            <asp:HiddenField ID="HF_EditID" runat="server" Value="00000000-0000-0000-0000-000000000000" />
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Part Number: </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:TextBox runat="server" ID="txtItmPrtNmbr" MaxLength="150" CssClass="bcAsptextbox"
                                                                onkeyup="SearchPartNo()"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Specifications : </span>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="bcTdnormal" colspan="4">
                                                            <asp:TextBox runat="server" ID="txtspec" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                                                onkeyup="SearchSpec()" Width="595px" Columns="80" Rows="2" onblur="javascript:return CheckItemsMaster();"></asp:TextBox>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel" style="visibility: hidden">HS Code: </span>
                                                        </td>
                                                        <td>
                                                            <span class="bcLabel">Contains Sub Item : </span>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:CheckBox ID="ChkHadSubItems" runat="server" onkeyup="SearchItms()" />
                                                            <asp:TextBox runat="server" ID="txtHSCode" MaxLength="150" CssClass="bcAsptextbox"
                                                                Visible="false"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div1" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" OnClientClick="javascript:return Myvalidations()" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div4" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnUpdate" Text="Update" OnClick="btnUpdate_Click" />
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
                                <tr>
                                    <td>
                                        <asp:Panel ID="PnlImp" runat="server" Width="50%" Visible="false">
                                            <table>
                                                <tr>
                                                    <td>
                                                        Upload Items From Excel:
                                                    </td>
                                                    <td colspan="2">
                                                        <asp:FileUpload ID="FileUpload1" runat="server" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnBulkUpload" runat="server" Text="Upload" OnClick="btnBulkUpload_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                    <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                        border="0" width="100%">
                        <thead>
                            <tr>
                                <th id="ID" runat="server" visible="false">
                                </th>
                                <%-- <th width="03%">
                                    Category
                                </th>
                                <th width="03%">
                                    Sub-Category
                                </th>
                                <th width="03%">
                                    Sub/Sub-Category
                                </th>--%>
                                <th width="03%">
                                    Item Code
                                </th>
                                <th width="20%">
                                    Item Description
                                </th>
                                <th width="10%">
                                    Part Number
                                </th>
                                <th width="20%">
                                    Specification
                                </th>
                                <th width="05%">
                                    Contains Sub-Items
                                </th>
                                <th width="02%">
                                    E
                                </th>
                                <th width="02%">
                                    D
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_filter
        {
            visibility: visible !important;
        }
    </style>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">

        function Myvalidations() {
            if (($('[id$=ddlItmCtgry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Category is Required.</span>');
                $('[id$=ddlItmCtgry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlItmSubCatgry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Sub Category is Required.</span>');
                $('[id$=ddlItmSubCatgry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlItmSubSubCatgry]').val()).trim() == '00000000-0000-0000-0000-000000000000' && ($("[id$=HFItemCatCodeIsSpare]").val() == "False" && $("[id$=HFItemSubCatCodeIsSpare]").val() == "False")) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Sub/Sub Category is Required.</span>');
                $('[id$=ddlItmSubSubCatgry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($("[id$=HFItemCatCodeIsSpare]").val() == "True" || $("[id$=HFItemSubCatCodeIsSpare]").val() == "True") && $("[id$=txtItmCode]").val().length < 7) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Code is Required.</span>');
                $('[id$=txtItmCode]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtItmCode]').val()).trim() == '') {
                ErrorMessage('Item Code is Required.');
                $('[id$=txtItmCode]').focus();
                return false;
            }
            if (($('[id$=txtItmDscrip]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Description is Required.</span>');
                $('[id$=txtItmDscrip]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else {
                var Item = $('[id$=txtItmDscrip]').val();
                var ItemPrtNmbr = $('[id$=txtItmPrtNmbr]').val();
                var ItemSpec = $('[id$=txtspec]').val();
                var Sub = $('[id$=ChkHadSubItems]').val();
                var EditItmID = $('[id$=HF_EditID]').val();
                var IsChecked = "False";
                if ($('[id$=ChkHadSubItems]').is(':checked'))
                    IsChecked = "True";

                var lblCode = $('[id$=lblCode]').text();
                var lblSCode = $('[id$=lblSCode]').text();
                var lblSSCode = $('[id$=lblSSCode]').text();
                var value1 = lblCode + lblSCode + lblSSCode + $('[id$=txtItmCode]').val();

                if ($("[id$=hfEditItemCode]").val() == "") { //&& $("[id$=hfEditItemCode]").val() != value1
                    var result = ItemMaster_Revised.CheckItemsMaster(Item, ItemPrtNmbr, ItemSpec, EditItmID, IsChecked);
                    if (result.value == false) {
                        $("[id$=ddlItmCtgry]").val(0);
                        $("[id$=ddlItmSubCatgry]").val(0);
                        $("[id$=ddlItmSubSubCatgry]").val(0);
                        $("#<%=txtItmDscrip.ClientID%>")[0].value = '';
                        $("#<%=txtItmCode.ClientID%>")[0].value = '';
                        $("#<%=txtItmPrtNmbr.ClientID%>")[0].value = '';
                        $("#<%=txtspec.ClientID%>")[0].value = '';
                        $("#<%=ChkHadSubItems.ClientID%>")[0].checked = false;
                        $("#<%=divMyMessage.ClientID %> span").remove();
                        $('[id$=divMyMessage]').append('<span class="Error">Item Already Exists.</span>');
                        $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                        // $("#<%=txtItmDscrip.ClientID%>")[0].focus();
                        //oTable.draw();
                        oTable.fnFilter('', 1);
                        return false;
                    }
                    else
                        return true;
                }
            }
        }

    </script>
    <script type="text/javascript">
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
    <script type="text/javascript">
        function CheckItemsMaster() {
            var Item = $('[id$=txtItmDscrip]').val().trim();
            var ItemPrtNmbr = $('[id$=txtItmPrtNmbr]').val().trim();
            var ItemSpec = $('[id$=txtspec]').val().trim();
            var EditItmID = $('[id$=HF_EditID]').val();
            var IsChecked = "False";
            if ($('[id$=ChkHadSubItems]').is(':checked'))
                IsChecked = "True";

            var result = ItemMaster_Revised.CheckItemsMaster(Item, ItemPrtNmbr, ItemSpec, EditItmID, IsChecked);
            if (result.value == false) {
                $("#<%=txtItmDscrip.ClientID%>")[0].value = '';
                $("#<%=txtItmPrtNmbr.ClientID%>")[0].value = '';
                $("#<%=txtspec.ClientID%>")[0].value = '';
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Already Exists.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                clearall();
                $("#<%=txtItmDscrip.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }
        //		  function EpmtyText() {
        //            var Currentweeks = $('[id$=txtItmCode]').val();
        //            if (Currentweeks == "") {
        //                $('[id$=txtItmCode]').focus(0, 0);
        //            }
    </script>
    <script type="text/javascript">

        var oTable;
        $(document).ready(function () {
            //$("[id$=btnUpdate]").hide();
            oTable = $("[id$=gvItmMstr]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "../Enquiries/WebService1.asmx/GetIMItems_forRevised",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                "sScrollY": "250px",
                "sScrollX": "100%",
                "bScrollCollapse": true,

                "fnServerParams": function (aoData) {
                    aoData.push({ "name": "ItemCode", "value": ($('[id$=lblCode]').text() + $('[id$=lblSCode]').text() + $('[id$=lblSSCode]').text() + $('[id$=txtItmCode]').val()) });
                    //                    aoData.push({ "name": "ItemCode",
                    //                        "value": (
                    //                        $("[id$='ddlItmCtgry'] :selected").text().split('-')[0] +
                    //                         $("[id$='ddlItmSubCatgry'] :selected").text().split('-')[0] +
                    //                          $("[id$='ddlItmSubSubCatgry'] :selected").text().split('-')[0] +
                    //                           $('[id$=txtItmCode]').val())
                    //                    });
                    aoData.push({ "name": "iItemDesc", "value": $('[id$=txtItmDscrip]').val() });
                    aoData.push({ "name": "iPartNo", "value": $('[id$=txtItmPrtNmbr]').val() });
                    aoData.push({ "name": "iSpec", "value": $('[id$=txtspec]').val() });
                    aoData.push({ "name": "DDL_ItemCat", "value": $("[id$='ddlItmCtgry'] :selected").val() });
                    aoData.push({ "name": "DDL_ItemSubCat", "value": $("[id$='ddlItmSubCatgry'] :selected").val() });
                    aoData.push({ "name": "DDL_ItemSubSubCat", "value": $("[id$='ddlItmSubSubCatgry'] :selected").val() });
                },

                "fnServerData": function (sSource, aoData, fnCallback) {
                    $.ajax({
                        "dataType": 'json',
                        "contentType": "application/json; charset=utf-8",
                        "type": "GET",
                        "url": sSource,
                        "data": aoData,
                        "success":
								function (msg) {
								    var json = jQuery.parseJSON(msg.d);
								    fnCallback(json);
								    $("#gvItmMstr").show();
								}
                    });
                }
            });
        });
        /* Init the table */
        oTable = $("#gvItmMstr").dataTable();

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });

        function SearchDesc() {
            var value1 = $('[id$=txtItmDscrip]').val();
            oTable.fnFilter(value1, 1);
        }

        function fn_codeclear() {
            $('[id$=txtItmCode]').val('');
        }

        function SearchItmCode() {
            var lblCode = $('[id$=lblCode]').text();
            var lblSCode = $('[id$=lblSCode]').text();
            var lblSSCode = $('[id$=lblSSCode]').text();

            var value1 = lblCode + lblSCode + lblSSCode + $('[id$=txtItmCode]').val();
            var Val_Six = value1;
            var Val_Split = Val_Six.substr(0, 7);
            var Val_Split_IC = Val_Six.substr(0, 2);
            var Val_Split_ISC = Val_Six.substr(2, 2);
            var Val_Split_ISSC = Val_Six.substr(4, 3);
            var IC = $("[id$='ddlItmCtgry'] :selected").text();
            IC = IC.substr(0, 2);
            var ISC = $("[id$='ddlItmSubCatgry'] :selected").text();
            ISC = ISC.substr(0, 2);
            var ISSVal = $('[id$=ddlItmSubSubCatgry]').length > 0 ? $('[id$=ddlItmSubSubCatgry]').val() : EmptyGuid;
            if (ISSVal != EmptyGUID) {
                ISSC = $("[id$='ddlItmSubSubCatgry'] :selected").text();
                ISSC = ISSC.split('-');
                ISSC = ISSC[0];
            }
            if (value1.length == 11) {
                var Var_Split1 = Val_Split;
                var s1 = Var_Split1.substr(0, 2); // will be '123'
                var s2 = Var_Split1.substr(2, 2);
                var s3 = Var_Split1.substr(4, 4);
                if ($('[id$=ddlItmCtgry]').val() != '' && $('[id$=ddlItmSubCatgry]').val() != '' || $('[id$=ddlItmSubCatgry]').val() != '') {
                    //if ($('[id$=ddlItmCtgry]').val() != s1) {
                    if (IC != s1) {
                        $('[id$=txtItmCode]').focus(0, 0);
                        $('[id$=txtItmCode]').val('');
                        ErrorMessage("Selected Item Category doesnot match");
                        return false;
                    }
                    //else if ($('[id$=ddlItmSubCatgry]').val() != s2) {
                    else if (ISC != s2) {
                        $('[id$=txtItmCode]').focus(0, 0);
                        $('[id$=txtItmCode]').val('');
                        ErrorMessage("Selected Item Sub Category doesnot match");
                        return false;
                    }
                    if ($('[id$=ddlItmSubSubCatgry]').val() != '' && $('[id$=ddlItmSubSubCatgry]').val() != EmptyGUID && $('[id$=txtItmCode]').val().length == 3) {
                        //if ($('[id$=ddlItmSubSubCatgry]').val() != s3) {
                        if (ISSC != s3) {
                            $('[id$=txtItmCode]').focus(0, 0);
                            $('[id$=txtItmCode]').val('');
                            ErrorMessage("Selected Item Sub Sub Category doesnot match");
                            return false;
                        }
                    }
                }
                if ($("[id$=hfEditItemCode]").val() == "" || ($("[id$=hfEditItemCode]").val() != "" && $("[id$=hfEditItemCode]").val() != value1)) {
                    var result = ItemMaster_Revised.CheckCode(value1);
                    if (result.value == false) {
                        ErrorMessage("Item Code Exists");
                        $('[id$=txtItmCode]').val('');
                        return false
                    }
                }
            }
            else {

                if ($("[id$=HFItemCatCodeIsSpare]").val() == "True" || $("[id$=HFItemSubCatCodeIsSpare]").val() == "True") {
                    ErrorMessage("Item Code Must be 7 digits");
                    $('[id$=txtItmCode]').val('');
                }
                else if ($("[id$=HFItemCatCodeIsSpare]").val() == "False" && $("[id$=HFItemSubCatCodeIsSpare]").val() == "False" && $('[id$=txtItmCode]').val().length > 4) {
                    ErrorMessage("Please Enter Sub Sub Category in the Master");
                    $('[id$=txtItmCode]').val('');
                }
                else {
                    ErrorMessage("Enter Item Code in Specified Format");
                    $('[id$=txtItmCode]').val('');
                }
            }

        }
        function SearchPartNo() {
            var value2 = $('[id$=txtItmPrtNmbr]').val();
            oTable.fnFilter("^" + value2 + "$", 2, true);
        }
        function SearchSpec() {
            var value3 = $('[id$=txtspec]').val();
            oTable.fnFilter("^" + value3 + "$", 3, true);
        }
        function SearchItms() {
            var value4 = $('[id$=ChkHadSubItems]').val();
            oTable.fnFilter("^" + value4 + "$", 4, true);
        }
        function SearchItemCat() {
            oTable.fnFilter("^" + $("[id$='ddlItmCtgry'] :selected").val() + "$", 5, true);
        }
        function SearchSubItemCat() {
            oTable.fnFilter("^" + $("[id$='ddlItmSubCatgry'] :selected").val() + "$", 6, true);
        }
        function SearchSubSubItemCat() {
            oTable.fnFilter("^" + $("[id$='ddlItmSubSubCatgry'] :selected").val() + "$", 7, true);
        }
        function Search_ItemCode() {
            var value12 = $('[id$=txtItmCode]').val();
            oTable.fnFilter("^" + value12 + "$", 8, true);
        }
        function Delet(valddd, CreatedBy, IsCust) {
            try {
                var EditID = valddd.parentNode.parentNode.id;
                if ($("[id$=hdfdItmMstrID]").val().toLowerCase() != EditID.toLowerCase()) {
                    if (confirm("Are you sure you want to Delete?")) {
                        var result = ItemMaster_Revised.DeleteItemDetails(EditID, CreatedBy, IsCust);
                        var fres = result.value;
                        if (fres.contains('Success::')) {
                            oTable.fnDraw();
                            SuccessMessage(fres.replace('Success::', ''));
                        }
                        else if (fres.contains('Error::')) {
                            ErrorMessage(fres.replace('Error::', ''));
                        }
                        else {
                            ErrorMessage(fres);
                        }
                    }
                }
                else
                    ErrorMessage('You cannot Delete, when the same row is Editing');
            } catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd) {
            try {

                //                $("[id$=ddlItmCtgry]").val(0);
                //                $("[id$=ddlItmSubCatgry]").val(0);
                //                $("[id$=ddlItmSubSubCatgry]").val(0);

                //                var value = valddd.parentNode.parentNode.id;
                //                $("[id$=HF_EditID]").val(value);
                //                $("[id$=hdfdItmMstrID]").val(valddd.parentNode.parentNode.id);

                //                var Categary = valddd.parentNode.parentNode.childNodes[0].innerHTML.trim();
                //                $("[id$=ddlItmCtgry] option:contains(" + Categary + ")").attr('selected', 'true');

                //                var SubCategary = valddd.parentNode.parentNode.childNodes[1].innerHTML.trim();
                //                $("[id$=ddlItmSubCatgry] option:contains(" + SubCategary + ")").attr('selected', 'true');

                //                var SubSubCategary = valddd.parentNode.parentNode.childNodes[2].innerHTML.trim();
                //                $("[id$=ddlItmSubSubCatgry] option:contains(" + SubSubCategary + ")").attr('selected', 'true');

                //                //var Code = valddd.parentNode.parentNode.childNodes[3].innerHTML.trim();

                //                //$("[id$=txtItmCode]").val(Code.substr(0, 9));
                //                $("[id$=txtItmCode]").val(valddd.parentNode.parentNode.childNodes[3].innerHTML.trim());
                //                $("[id$=txtItmDscrip]").val(valddd.parentNode.parentNode.childNodes[4].innerHTML.trim());
                //                $("[id$=txtItmPrtNmbr]").val(valddd.parentNode.parentNode.childNodes[5].innerHTML.trim());
                //                $("[id$=txtspec]").val(valddd.parentNode.parentNode.childNodes[6].innerHTML.trim());
                //                $("[id$=ChkHadSubItems]").val(valddd.parentNode.parentNode.childNodes[7].innerHTML.trim());

                //                //$("[id$=txtItmDscrip]").val(valddd.parentNode.parentNode.childNodes[1].innerHTML);
                //                //$("[id$=txtItmPrtNmbr]").val(valddd.parentNode.parentNode.childNodes[2].innerHTML);
                //                //$("[id$=txtspec]").val(valddd.parentNode.parentNode.childNodes[3].innerHTML);
                //                //$("[id$=ChkHadSubItems]").val(valddd.parentNode.parentNode.childNodes[4].innerHTML);

                //                $("[id$=btnSave]").hide();
                //                $("[id$=btnUpdate]").show();
                //                var IsChecked = valddd.parentNode.parentNode.childNodes[7].innerHTML.trim();
                //                if (IsChecked == 'Yes')
                //                    $("[id$=ChkHadSubItems]").attr('checked', 'checked');
                //                else
                //                    $("[id$=ChkHadSubItems]").removeAttr('checked');
                //                document.getElementById('<%= btnUpdate.ClientID %>').innerHTML = 'Update';

                window.location.replace("../Masters/ItemMaster_Revised.aspx?ID=" + valddd.parentNode.parentNode.id);
            } catch (e) {
                alert(e.Message);
            }
        }
        function BindDataTable() {
            oTable.fnFilter("^" + $("[id$='ddlItmCtgry'] :selected").val() + "$", 5, true);
        }
    </script>
</asp:Content>
