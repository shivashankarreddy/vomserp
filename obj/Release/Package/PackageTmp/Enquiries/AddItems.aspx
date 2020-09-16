<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddItems.aspx.cs" Inherits="VOMS_ERP.Enquiries.AddItems" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <%--<title>Volta Impex Pvt Ltd</title>--%>
    <link href="../css/jquery-ui-1.9.1.custom.min.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/messages.css" rel="stylesheet" type="text/css" />
    <link href="../css/uploadify.css" rel="stylesheet" type="text/css" />
    <link href="../css/TableStyle.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/themes/overcast/jquery-ui.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/media/css/themes/overcast/jquery.ui.theme.css" rel="stylesheet"
        type="text/css" />
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <link href="../JScript/media/css/demo_page.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/demo_table.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/demo_table_jui.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/jquery.dataTables.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/jquery.dataTables_themeroller.css" rel="stylesheet"
        type="text/css" />
    <style type="text/css">
        .HideTableColumn
        {
            visibility: hidden;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Items" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" style="top: 35px;" />
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
                                    <td colspan="3" class="bcTdNewTable">
                                        <table style="width: 100%;">
                                            <tr>
                                                <%--<td class="bcTdnormal">
                                                    <span class="bcLabel">Item Category<font color="red" size="2"><b>*</b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>--%>
                                                <%--  <td class="bcTdnormal">
                                                    <asp:DropDownList runat="server" ID="ddlItmCtgry" CssClass="bcAspdropdown">
                                                        <asp:ListItem Text="Select ItemCategory" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>--%>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Category<font color="red" size="2"><b></b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:DropDownList runat="server" ID="ddlItmCtgry" CssClass="bcAspdropdown" onchange="fn_codeclear();SearchItemCat()"
                                                        OnSelectedIndexChanged="ddlItmCtgry_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Text="Select ItemCategory" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Sub Category<font color="red" size="2"><b></b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:DropDownList runat="server" ID="ddlItmSubCatgry" CssClass="bcAspdropdown" onchange="fn_codeclear();SearchItemCat()"
                                                        OnSelectedIndexChanged="ddlItmSubCatgry_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Text="Select ItemSubCategory" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Sub/Sub-Category<font color="red" size="2"><b></b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:DropDownList runat="server" ID="ddlItmSubSubCatgry" CssClass="bcAspdropdown"
                                                        onchange="fn_codeclear();SearchItemCat()" OnSelectedIndexChanged="ddlItmSubSubCatgry_SelectedIndexChanged"
                                                        AutoPostBack="true">
                                                        <asp:ListItem Text="Select ItemSub/SubCategory" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Code<font color="red" size="2"><b></b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtItmCode" onfocus="this.select()" MaxLength="7"
                                                        onkeyup="Search_ItemCode();" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                        onMouseUp="return false" CssClass="bcAsptextbox" onblur="ItemCode_TextChanged();"></asp:TextBox>
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
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Part Number: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtItmPrtNmbr" MaxLength="400" CssClass="bcAsptextbox"
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
                                                        onkeyup="SearchSpec()" Width="470px" Columns="60" Rows="2" onchange="javascript:return CheckItemsMaster();"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">contains Sub Item: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:CheckBox ID="ChkHadSubItems" runat="server" />
                                                    <asp:TextBox runat="server" ID="txtHSCode" MaxLength="150" CssClass="bcAsptextbox"
                                                        Visible="false"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="3">
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
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" class="bcTdNewTable" align="center">
                                        <asp:HiddenField ID="HFSelectedVal" runat="server" />
                                        <asp:HiddenField ID="HFAllSelectedItems" runat="server" />
                                        <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                                            border="0">
                                            <thead>
                                                <tr>
                                                    <th width="5%">
                                                        SNo.
                                                    </th>
                                                    <%--<th width="10%">
                                                        Catagory
                                                    </th>--%>
                                                    <th width="45%">
                                                        ItemDescription
                                                    </th>
                                                    <th width="20%">
                                                        PartNumber
                                                    </th>
                                                    <th width="30%">
                                                        Specification
                                                    </th>
                                                    <%--<th width="10%">
                                                        HSCode
                                                    </th>--%>
                                                </tr>
                                            </thead>
                                            <tbody>
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function Myvalidations() {
            try {
                if (($('[id$=ddlItmCtgry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Category is Required.');
                    $('[id$=ddlItmCtgry]').focus();
                    return false;
                }
                else if (($('[id$=ddlItmSubCatgry]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Sub Category is Required.</span>');
                    $('[id$=ddlItmSubCatgry]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
                else if ($('[id$=ddlItmSubSubCatgry]').val() != null && $('[id$=ddlItmSubSubCatgry]').val() == '00000000-0000-0000-0000-000000000000') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Sub/Sub Category is Required.</span>');
                    $('[id$=ddlItmSubSubCatgry]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
                else if (($('[id$=txtItmCode]').val()).trim() == '') {
                    ErrorMessage('Item Code is Required.');
                    $('[id$=txtItmCode]').focus();
                    return false;
                }
                else if (($('[id$=txtItmDscrip]').val()).trim() == '') {
                    ErrorMessage('Item Description is Required.');
                    $('[id$=txtItmDscrip]').focus();
                    return false;
                }
                else {
                    var Item = $('[id$=txtItmDscrip]').val();
                    var ItemPrtNmbr = $('[id$=txtItmPrtNmbr]').val();
                    var ItemSpec = $('[id$=txtspec]').val();
                    var result = AddItems.CheckItemsMaster(Item, ItemPrtNmbr, ItemSpec, '');
                    if (result.value == false) {
                        $("[id$=ddlItmCtgry]").val(0);
                        $("[id$=ddlItmSubCatgry]").val(0);
                        $("[id$=ddlItmSubSubCatgry]").val(0);
                        //alert('Mail-ID Exists');
                        $('[txtItmCode]').val('');
                        $('[txtItmDscrip]').val('');
                        $('[txtItmPrtNmbr]').val('');
                        $('[txtspec]').val('');
                        ErrorMessage('Item Already Exists.');
                        $('[txtItmDscrip]').focus();
                        oTable.fnFilter('', 1);
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

        function OKClicked() {
            window.opener.setValue(true);
        }

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">

        var oTable;

        //$("#gvItmMstr").dataTable({
        $("[id$=gvItmMstr]").dataTable({
            "oLanguage": {
                "sZeroRecords": "There are no Records that match your search criteria",
                "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                "sInfoEmpty": "Showing 0 to 0 of 0 records",
                "sInfoFiltered": "(filtered from _MAX_ total records)",
                "sSearch": "Search : "
            },
            "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
            //"aLengthMenu": [[5000, -1], [5000, "All"]],
            "iDisplayLength": 100,
            "bSortClasses": false,
            "bStateSave": false,
            "bPaginate": true,
            "bAutoWidth": false,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "WebService1.asmx/GetItems",
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "bDeferRender": true,
            //Scrolling--------------
            "sScrollY": "300px",
            "sScrollX": "100%",
            "sScrollXInner": "100%",
            //------------
            "fnServerParams": function (aoData) {
                /*
                To Get Category Code
                */
                var CatCodeDesc = $("[id$=ddlItmCtgry] :selected").text().split('-');
                if (CatCodeDesc.length > 0)
                    var CatCode = CatCodeDesc[0];

                /*
                To Get Sub Category Code
                */
                var SCatCodeDesc = $("[id$=ddlItmSubCatgry] :selected").text().split('-');
                if (SCatCodeDesc.length > 0)
                    var SCatCode = SCatCodeDesc[0];

                /*
                To Get Sub-SubCategory Code
                */
                var SSCatCodeDesc = $("[id$=ddlItmSubSubCatgry] :selected").text().split('-');
                if (SSCatCodeDesc.length > 0)
                    var SSCatCode = SSCatCodeDesc[0];

                //aoData.push({ "name": "iAllSelectedItems", "value": $("#HFAllSelectedItems").val() });
                aoData.push({ "name": "ItemCode", "value": (CatCode + SCatCode + SSCatCode + $('[id$=txtItmCode]').val()) });
                aoData.push({ "name": "iItemDescription", "value": $("#txtItmDscrip").val() });
                aoData.push({ "name": "iPartNumber", "value": $("#txtItmPrtNmbr").val() });
                aoData.push({ "name": "iSpecification", "value": $("#txtspec").val() });
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
        /* Init the table */
        oTable = $("#gvItmMstr").dataTable();

        function fnGetSelected(oTableLocal) {
            return oTableLocal.$('tr.row_selected');
        }

        function fn_codeclear() {
            $('[id$=txtItmCode]').val('');
        }

        function SearchDesc() {
            var value1 = $('[id$=txtItmDscrip]').val();
            oTable.fnFilter(value1, 1);
        }
        function SearchPartNo() {
            var value2 = $('[id$=txtItmPrtNmbr]').val();
            oTable.fnFilter(value2, 2);
        }
        function SearchSpec() {
            var value3 = $('[id$=txtspec]').val();
            if (value3 != '')
                oTable.fnFilter(value3, 3);
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
            oTable.fnFilter("^" + $('[id$=txtItmCode]').val() + "$", 8, true);
        }
        /* Add a click handler to the rows - this could be used as a callback */
        $("#gvItmMstr tbody tr").click(function (e) {
            if ($(this).hasClass('row_selected')) {
                $(this).removeClass('row_selected');
                $('#HFSelectedVal').val('');
                //alert($('#HFSelectedVal').val());
                $('#btnSendVal').hide();
            }
            else {
                oTable.$('tr.row_selected').removeClass('row_selected');
                $(this).addClass('row_selected');
                //alert($('#HFSelectedVal').val());
                //$('#btnSendVal').show();
            }
        });
        $.fn.dataTableExt.oStdClasses["filterColumn"] = "my-style-class";

        $('#gvItmMstr tbody td').click(function () {
            /* Get the position of the current data from the node */
            var aPos = oTable.fnGetPosition(this);
            var aData = oTable.fnGetData(aPos[0]);
            GetSelRowHidValue(aData.DT_RowId);
        });

        function GetSelRowHidValue(SelID) {
            //SelID = Number(SelID) + 1;
            var slice = "";
            if (SelID < 10)
                slice = '0';
            //$('#HFSelectedVal').val($("#gvItmMstr_ctl" + slice + '' + SelID + "_HfID").val() + ",");
            $('#HFSelectedVal').val(SelID + ",");
            //alert($("#gvItmMstr_ctl" + slice + '' + SelID + "_HfID").val());
        }

        function ItemCode_TextChanged() {
            if (($('[id$=ddlItmCtgry]').val().trim().length != 0 || ($('[id$=ddlItmCtgry]').val()).trim() != '00000000-0000-0000-0000-000000000000')
                         && ($('[id$=ddlItmSubCatgry]').val().trim().length != 0 || ($('[id$=ddlItmSubCatgry]').val()).trim() != '00000000-0000-0000-0000-000000000000')
                          && ($('[id$=ddlItmSubSubCatgry]').val() != null &&
                          ($('[id$=ddlItmSubSubCatgry]').val().trim().length != 0 || $('[id$=ddlItmSubSubCatgry]').val() != '00000000-0000-0000-0000-000000000000'))
                           && $('[id$=txtItmCode]').val().length != 4) {
                ErrorMessage("Enter Item Code in Specified Format");
                $('[id$=txtItmCode]').val('');
                return false;
            }
            if (($('[id$=ddlItmCtgry]').val() != '' || ($('[id$=ddlItmCtgry]').val()).trim() != '00000000-0000-0000-0000-000000000000')
                         && ($('[id$=ddlItmSubCatgry]').val() != '' || ($('[id$=ddlItmSubCatgry]').val()).trim() != '00000000-0000-0000-0000-000000000000')
                         && ($('[id$=ddlItmSubSubCatgry]').val() == null || $('[id$=ddlItmSubSubCatgry]').val() == '00000000-0000-0000-0000-000000000000')
                           && $('[id$=txtItmCode]').val().length != 7) {
                ErrorMessage("Enter Item Code in Specified Format");
                $('[id$=txtItmCode]').val('');
                return false;
            }

            /*
            To Get Category Code
            */
            var CatCodeDesc = $("[id$=ddlItmCtgry] :selected").text().split('-');
            if (CatCodeDesc.length > 0)
                var CatCode = CatCodeDesc[0];

            /*
            To Get Sub Category Code
            */
            var SCatCodeDesc = $("[id$=ddlItmSubCatgry] :selected").text().split('-');
            if (SCatCodeDesc.length > 0)
                var SCatCode = SCatCodeDesc[0];

            /*
            To Get Sub-SubCategory Code
            */
            var SSCatCodeDesc = $("[id$=ddlItmSubSubCatgry] :selected").text().split('-');
            if (SSCatCodeDesc.length > 0)
                var SSCatCode = SSCatCodeDesc[0];

            var newItemCode = CatCode + SCatCode + SSCatCode + $('[id$=txtItmCode]').val();

            if (newItemCode.length == 11) {
                var result = AddItems.CheckCode(newItemCode);
                if (result.value == false) {
                    ErrorMessage("Item Code Exists");
                    $('[id$=txtItmCode]').val('');
                    return false
                }
            }
            else {
                ErrorMessage("Item Code Exists");
                $('[id$=txtItmCode]').val('');
                return false
            }
        }

        function SearchItmCode() {
            var value1 = $('[id$=txtItmCode]').val();
            var Val_Six = value1;
            var Val_Split = Val_Six.substr(0, 6);
            var Val_Split_IC = Val_Six.substr(0, 2);
            var Val_Split_ISC = Val_Six.substr(2, 2);
            var Val_Split_ISSC = Val_Six.substr(4, 2);
            if (value1.length > 6) {
                var Var_Split1 = Val_Split;
                var s1 = Var_Split1.substr(0, 2); // will be '123'
                var s2 = Var_Split1.substr(2, 2);
                var s3 = Var_Split1.substr(4, 4);
                if ($('[id$=ddlItmCtgry]').val() != '' && $('[id$=ddlItmSubCatgry]').val() != '' && $('[id$=ddlItmSubSubCatgry]').val() != '') {
                    if ($('[id$=ddlItmCtgry]').val() != s1) {
                        ErrorMessage("Selected Item Category doesnot match");
                        $('[id$=txtItmCode]').focus(0, 0);
                        $('[id$=txtItmCode]').val('');
                    }
                    else if ($('[id$=ddlItmSubCatgry]').val() != s2) {
                        ErrorMessage("Selected Item Sub Category doesnot match");
                        $('[id$=txtItmCode]').focus(0, 0);
                        $('[id$=txtItmCode]').val('');
                    }
                    else {
                        if ($('[id$=ddlItmSubSubCatgry]').val() != s3) {
                            ErrorMessage("Selected Item Sub Sub Category doesnot match");
                            $('[id$=txtItmCode]').focus(0, 0);
                            $('[id$=txtItmCode]').val('');
                        }
                    }
                }
                var result = AddItems.CheckCode(value1);
                if (result.value == false) {
                    ErrorMessage("Item Code Exists");
                    $('[id$=txtItmCode]').val('');
                    return false
                }
            }
            else {
                ErrorMessage("Enter Item Code in Specified Format");
                $('[id$=txtItmCode]').val('');
            }

        }

        function NoPermissions() {
            alert('You are not having permission, please contact your Administrator...');
            window.close();
        }

        $('#gvItmMstr tbody tr').live('dblclick', function (e) {
            e.preventDefault();
            //alert('clicked');
            var nRow = $(this)[0];
            var aData = oTable.fnGetData(nRow);
            GetSelRowHidValue(aData.DT_RowId);
            SendSelectedVal();
        });

        function CheckItemsMaster() {
            try {
                var Item = $('[id$=txtItmDscrip]').val();
                var ItemPrtNmbr = $('[id$=txtItmPrtNmbr]').val();
                var ItemSpec = $('[id$=txtspec]').val();
                var IsSubItem = $('[id$=ChkHadSubItems]')[0].checked;
                var result = AddItems.CheckItemsMaster(Item, ItemPrtNmbr, ItemSpec, IsSubItem);
                if (result.value == false) {
                    //alert('Mail-ID Exists');
                    $("#<%=txtItmDscrip.ClientID%>")[0].value = '';
                    $("#<%=txtItmPrtNmbr.ClientID%>")[0].value = '';
                    $("#<%=txtspec.ClientID%>")[0].value = '';
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Item Already Exists.</span>');
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    $("#<%=txtItmDscrip.ClientID%>")[0].focus();
                    return false;
                }
                else
                    return true;
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }
        
    </script>
    <script type="text/jscript">
        function SendSelectedVal() {
            //alert($('[id$=HFSelectedVal]').val());
            //            window.returnValue = $('#HFSelectedVal').val();
            //            opener.setSearchResult(window.returnValue);
            window.opener.GetValueFromChild($('#HFSelectedVal').val());
            window.close();
        }
 
    </script>
    <script type="text/jscript">
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
    </form>
</body>
</html>
