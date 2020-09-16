<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ItemSubCategory.aspx.cs" Inherits="VOMS_ERP.Masters.ItemSubCategory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hdfdItmMstrID" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Item SubCategory" CssClass="bcTdTitleLabel"> </asp:Label><div
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
                                    <td colspan="3" class="bcTdNewTable">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Category<font color="red" size="2"><b>*</b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:DropDownList runat="server" ID="ddlItmCtgry" CssClass="bcAspdropdown"  onchange="SearchItemCode()">
                                                        <asp:ListItem Text="Select ItemCategory" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Sub Category Code<font color="red" size="2"><b>*</b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtSubCategoryCode" TextMode="SingleLine" CssClass="bcAsptextbox"
                                                        onblur="SearchItemSubCatgCode()" MaxLength="2" onkeyup="extractNumber(this, 0, false);SearchItemSubCatgCode();"></asp:TextBox>
                                                    <asp:HiddenField ID="hfSubCatEdit" runat="server" Value="" />
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Description<font color="red" size="2"><b>*</b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtItmDscrip" TextMode="SingleLine" CssClass="bcAsptextbox"
                                                        onkeyup="alphaNumericSpaceAnd_AndBracket(this);SearchDesc();" MaxLength="500"></asp:TextBox>
                                                    <asp:HiddenField ID="HF_EditID" runat="server" Value="00000000-0000-0000-0000-000000000000" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Is Not Sub Sub Category: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:CheckBox ID="ChkIsSpareParts" runat="server" />
                                                </td>
                                                <td class="bcTdnormal">
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                </td>
                                                <td class="bcTdnormal">
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <asp:FileUpload ID="FileUpload_SubCat" runat="server" Visible="false" />
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:Button ID="BTN_Upload" runat="server" Text="Upload" OnClick="BTN_Upload_Click"
                                                        Visible="false" />
                                                </td>
                                                <td class="bcTdnormal">
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                </td>
                                                <td class="bcTdnormal">
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
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
                                <th width="03%">
                                    SubCat Code
                                </th>
                                <th width="20%">
                                    SubCat Description
                                </th>
                                <th width="10%">
                                    cat Code
                                </th>
                                <th width="20%">
                                    Cat Desc
                                </th>
                                <th width="5%">
                                    Is Not Sub Sub Category
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
            if (($('[id$=ddlItmCtgry]').val()).trim() == '0') {
                $('[id$=ddlItmCtgry]').focus();
                ErrorMessage('Item Category is required.');
                return false;
            }
            else if (($('[id$=txtSubCategoryCode]').val()).trim() == '') {
                $('[id$=txtSubCategoryCode]').focus();
                ErrorMessage('SubCategory Code is required.');
                return false;
            }
            else if (($('[id$=txtItmDscrip]').val()).trim() == '') {
                $('[id$=txtItmDscrip]').focus();
                ErrorMessage('Item Description is Required.');
                return false;
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

            var result = ItemSubCategory.CheckItemsMaster(Item, ItemPrtNmbr, ItemSpec, EditItmID, IsChecked);
            if (result.value == false) {
                $("#<%=txtItmDscrip.ClientID%>")[0].value = '';
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
        
    </script>
    <script type="text/javascript">

        var oTable;
        $(document).ready(function () {
            $("[id$=btnUpdate]").hide();
            oTable = $("[id$=gvItmMstr]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, -1], [100, 200, 500, 'ALL']],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "ItemCategory.asmx/GetSubCategoryItems",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                "sScrollY": "250px",
                "sScrollX": "100%",
                "bScrollCollapse": true,

                "fnServerParams": function (aoData) { 
                    aoData.push({ "name": "ItemDesc", "value": $('[id$=txtItmDscrip]').val() });
                    aoData.push({ "name": "ItemSubCatgCode", "value": $('[id$=txtSubCategoryCode]').val() });
                    aoData.push({ "name": "DDL_ItemCat", "value": $("[id$='ddlItmCtgry'] :selected").val() });
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
            oTable.fnFilter(value1, 0);
        }
        function SearchItemCode() {
            var value1 = $("[id$='ddlItmCtgry'] :selected").val();
            oTable.fnFilter(value1, 2);
        }
        function SearchItemSubCatgCode() {
            var value1 = $('[id$=txtSubCategoryCode]').val();
            oTable.fnFilter(value1, 1);
        }
        function fn_codeclear() {
            $('[id$=txtItmDscrip]').val('');
        }
        function SearchSubCategory() {
            var Catgry = $("[id$=ddlItmCtgry]").val();
            var Value = $("[id$=txtSubCategoryCode]").val();
            var HFValue = $("[id$=hfSubCatEdit]").val();
            if (Value.length == 2) {
                if (Catgry != "0" && Value != "" && HFValue != Value) {
                    var result = ItemSubCategory.CheckSubCategory(Value, Catgry);
                    if (result.value == "0") {
                        $("[id$=txtSubCategoryCode]").val('');
                        ErrorMessage('Sub Category Code Exists for Selected Category');
                    }
                }
                else if (Catgry == "0") {
                    $("[id$=ddlItmCtgry]").focus();
                    ErrorMessage('Category is required');
                    $("[id$=txtSubCategoryCode]").val('');

                }
                else if (Value == "") {
                    $("[id$=txtSubCategoryCode]").focus();
                    ErrorMessage('Sub Category Code is required.');
                }
            }
            else {
                $("[id$=txtSubCategoryCode]").val('');
                ErrorMessage('Sub Category Code Required Two Digits');
            }
        }

        function DeleteDetails(valddd, value) {
            try {
                if ($("[id$=HF_EditID]").val().toLowerCase() != value.toLowerCase()) {
                    if (confirm("Are you sure you want to Delete?")) {
                        var result = ItemSubCategory.DeleteItemDetails(value);
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

        function EditDetails(valddd, ID, Enable) {
            try {
                var value = ID;
                $("[id$=HF_EditID]").val(value);
                var EditText = valddd.parentNode.parentNode.childNodes[2].innerHTML.replace("&amp;","&");
                $("[id$=ddlItmCtgry] option:contains(" + EditText + ")").attr('selected', true);
                var Desc = valddd.parentNode.parentNode.childNodes[0].innerHTML;
                $("[id$=txtSubCategoryCode]").val(Desc);
                if (parseInt(Enable) > 0)
                    $("[id$=txtSubCategoryCode]").prop('disabled', true);
                else
                    $("[id$=txtSubCategoryCode]").removeAttr('disabled');
                $("[id$=hfSubCatEdit]").val(Desc);
                $("[id$=txtItmDscrip]").val(valddd.parentNode.parentNode.childNodes[1].innerHTML);
                $("[id$=ChkHadSubItems]").val(valddd.parentNode.parentNode.childNodes[4].innerHTML);
                $("[id$=btnSave]").hide();
                $("[id$=btnUpdate]").show();
                var IsChecked = valddd.parentNode.parentNode.childNodes[4].innerHTML;
                if (IsChecked == 'true')
                    $("[id$=ChkIsSpareParts]").attr('checked', 'checked');
                else
                    $("[id$=ChkIsSpareParts]").removeAttr('checked');
                document.getElementById('<%= btnUpdate.ClientID %>').innerHTML = 'Update';
            } catch (e) {
                alert(e.Message);
            }
        }

        function BindDataTable() {
            oTable.fnFilter("^" + $("[id$='ddlItmCtgry'] :selected").val() + "$", 3, true);
        }
    </script>
</asp:Content>
