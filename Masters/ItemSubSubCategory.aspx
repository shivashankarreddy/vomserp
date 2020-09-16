<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ItemSubSubCategory.aspx.cs" Inherits="VOMS_ERP.Masters.ItemSubSubCategory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hdfdItmCategoryID" />
    <asp:HiddenField runat="server" ID="hdfItemSubCatID" />
    <asp:HiddenField runat="server" ID="hfEditID" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Item Sub Sub / Spares Category Master"
                                            CssClass="bcTdTitleLabel"> </asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
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
                                        <table style="width: 100%;">
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Category Code<font color="red" size="2"><b>*</b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:DropDownList runat="server" ID="ddlItmCtgry" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlItmCtgry_SelectedIndexChanged"
                                                      onchange="SearchItemCode()"  AutoPostBack="true">
                                                        <asp:ListItem Text="--Select ItemCategory--"></asp:ListItem>
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
                                                    <asp:DropDownList runat="server" ID="ddlItmSubCatCode" CssClass="bcAspdropdown" AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlItmSubCatCode_SelectedIndexChanged" onchange="SearchItemSubCatCode()">
                                                        <asp:ListItem Text="--Select Item SubCategory--" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <tr>
                                                    <td class="bcTdnormal">
                                                        <span class="bcLabel">Item Sub Sub / Spares Category Code<font color="red" size="2"><b>*</b></font>:
                                                        </span>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td class="bcTdnormal">
                                                        <asp:TextBox runat="server" ID="txtItmSubSubCatCode" CssClass="bcAsptextbox" onblur="SearchItemSubCatgCode();"
                                                            Enabled="false" onkeyup="extractNumber(this, 0, false);SearchItemSubCatgCode();" MaxLength="3"></asp:TextBox>
                                                    </td>
                                                    <td class="bcTdnormal">
                                                        <span class="bcLabel">Item Sub Sub / Spares Category Description<font color="red" size="2"><b>*</b></font>:
                                                        </span>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td class="bcTdnormal">
                                                        <asp:TextBox runat="server" ID="txtItmSubSubCatDesc"  onkeyup="alphaNumericSpaceAnd_AndBracket(this);SearchDesc();" CssClass="bcAsptextbox"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" class="bcTdTitleLabel">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="bcTdTitleLabel">
                                                        <asp:FileUpload ID="FileUpload1" runat="server" Visible="false" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnBulkUpload" runat="server" Text="Upload" OnClick="btnBulkUpload_Click"
                                                            Visible="false" />
                                                    </td>
                                                    <td colspan="4">
                                                    </td>
                                                </tr>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
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
                    <table id="gvItmSubSubCategory" class="widthFull fontsize10 displayNone" cellpadding="0"
                        cellspacing="0" border="0" width="100%">
                        <thead>
                            <tr>
                                <th id="ID" runat="server" visible="false">
                                </th>
                                <th width="08%">
                                    Item Sub Sub / Spares Category Code
                                </th>
                                <th width="15%">
                                    Item Sub Sub / Spares Category Description
                                </th>
                                <th width="08%">
                                    Item Sub Category Code
                                </th>
                                <th width="15%">
                                    Item Sub Category Description
                                </th>
                                <th width="08%">
                                    Item Category Code
                                </th>
                                <th width="15%">
                                    Item Category Description
                                </th>
                                <th width="03%">
                                    E
                                </th>
                                <th width="03%">
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
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Category is Required.</span>');
                $('[id$=ddlItmCtgry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtItemCatCode]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Category Code is Required.</span>');
                $('[id$=txtItmDscrip]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlItmSubCatCode]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Sub Category is Required.</span>');
                $('[id$=ddlItmCtgry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtItmSubCatCode]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Sub Category Code is Required.</span>');
                $('[id$=txtItmDscrip]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtItmSubSubCatCode]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Sub Sub Category Code is Required.</span>');
                $('[id$=txtItmDscrip]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtItmSubCatCode]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Sub Sub Category Description is Required.</span>');
                $('[id$=txtItmDscrip]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
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

        var oTable;
        $(document).ready(function () {
            $("[id$=btnUpdate]").hide();
            oTable = $("[id$=gvItmSubSubCategory]").dataTable({
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
                "sAjaxSource": "ItemCategory.asmx/GetSubSubCategoryItems",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                "sScrollY": "250px",
                "sScrollX": "100%",
                "bScrollCollapse": true,
                "fnServerParams": function (aoData) {
                    aoData.push({ "name": "ItemDesc", "value": $('[id$=txtItmSubSubCatDesc]').val() });
                    aoData.push({ "name": "ItemSubCatgCode", "value": $('[id$=txtItmSubSubCatCode]').val() });
                    aoData.push({ "name": "DDL_ItemCat", "value": $("[id$='ddlItmCtgry'] :selected").val() });
                    aoData.push({ "name": "DDL_ItemSubCat", "value": $("[id$='ddlItmSubCatCode'] :selected").val() });
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
								    $("#gvItmSubSubCategory").show();
								}
                    });
                }
            });
        });
        /* Init the table */
        oTable = $("#gvItmSubSubCategory").dataTable();

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });

        function SearchDesc() {
            var value1 = $('[id$=txtItmSubSubCatDesc]').val();
            oTable.fnFilter(value1, 0);
        }
        function SearchItemCode() {
            var value1 = $("[id$='ddlItmCtgry'] :selected").val();
            oTable.fnFilter(value1, 2);
        }
        function SearchItemSubCatCode() {
            var value1 = $("[id$='ddlItmSubCatCode'] :selected").val();
            oTable.fnFilter(value1, 3);
        }
        function SearchItemSubCatgCode() {
            var value1 = $('[id$=txtItmSubSubCatCode]').val();
            oTable.fnFilter(value1, 1);
        }



        function SearchItmCode() {
            var value1 = $('[id$=txtItmSubSubCatCode]').val();
            if (value1.length == 3) {
                //                if (value1 == "000") {
                //                    ErrorMessage("Item Sub Sub Category Code cannot be '000'.");
                //                    $('[id$=txtItmSubSubCatCode]').val('').focus();
                //                    return false;
                //                }
                //                else {
                var CatID = $('[id$=ddlItmCtgry]').val();
                var SubCatID = $('[id$=ddlItmSubCatCode]').val();
                var result = ItemSubSubCategory.CheckCode(value1, CatID, SubCatID);
                if (result.value == "false") {
                    ErrorMessage("Item Sub Sub Category Code Exists");
                    $('[id$=txtItmSubSubCatCode]').val('');
                    return false
                    //}
                }
            }
            else {
                $('[id$=txtItmSubSubCatCode]').val('').focus();
                ErrorMessage("Item Sub Sub Category Code required three digits");
                return false
            }
        }

        function DeleteDetails(valddd, ID) {
            try {
                if ($("[id$=hfEditID]").val().toLowerCase() != ID.toLowerCase()) {
                    if (confirm("Are you sure you want to Delete?")) {
                        var result = ItemSubSubCategory.DeleteItemDetails(ID);
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

        function EditDetails(valddd, ID) {

            try {

                window.location.replace("../Masters/ItemSubSubCategory.Aspx?ID=" + ID);
            } catch (e) {
                alert(e.Message);
            }
            //            try {

            //                $("[id$=ddlItmCtgry]").val(0);
            //                $("[id$=ddlItmSubCatCode]").val(0);
            //                $("[id$=txtItmSubSubCatCode]").val('');
            //                $("[id$=txtItmSubSubCatDesc]").val('');

            //                var value =ID;
            //                $("[id$=HF_EditID]").val(value);
            //                //$("[id$=hdfdItmCategoryID]").val(valddd.parentNode.parentNode.id);
            //                $("[id$=ddlItmCtgry] option:contains(" + valddd.parentNode.parentNode.childNodes[4].innerHTML + ")").attr('selected', true);
            //                $("[id$=ddlItmSubCatCode]").val(value);

            //                $("[id$=txtItmSubSubCatCode]").val(valddd.parentNode.parentNode.childNodes[0].innerHTML.trim());
            //                $("[id$=txtItmSubSubCatDesc]").val(valddd.parentNode.parentNode.childNodes[1].innerHTML.trim());

            //                $("[id$=btnSave]").hide();
            //                $("[id$=btnUpdate]").show();
            //                
            //            } catch (e) {
            //                alert(e.Message);
            //            }
        }
    </script>
</asp:Content>
