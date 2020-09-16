<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ItemsMapping.aspx.cs" Inherits="VOMS_ERP.Masters.ItemsMapping" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Items Mapping" CssClass="bcTdTitleLabel"> </asp:Label><div
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
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Item Category<font color="red" size="2"><b>*</b></font>: </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlItmCtgry" CssClass="bcAspdropdown" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlItmCtgry_SelectedIndexChanged">
                                            <asp:ListItem Text="Select Category" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hfItmCtgry" runat="server" Value="" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Item Category Code<font color="red" size="2"><b>*</b></font>:
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:Label ID="lblItemCtgryCode" runat="server" Text="." Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Item Sub Category<font color="red" size="2"><b>*</b></font>:
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlItmSubCtgry" CssClass="bcAspdropdown" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlItmSubCtgry_SelectedIndexChanged">
                                            <asp:ListItem Text="Select Sub Category" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hfItmSubCtgry" runat="server" Value="" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Item Sub Category Code<font color="red" size="2"><b>*</b></font>:
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:Label ID="lblItemSubCtgryCode" runat="server" Text="." Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Item Sub Sub Category<font color="red" size="2"><b>*</b></font>:
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlItmSubSubCtgry" CssClass="bcAspdropdown"
                                            AutoPostBack="True" OnSelectedIndexChanged="ddlItmSubSubCtgry_SelectedIndexChanged">
                                            <asp:ListItem Text="Select Sub Sub Category" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hfItmSubSubCtgry" runat="server" Value="" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Item Sub Sub Category Code<font color="red" size="2"><b>*</b></font>:
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:Label ID="lblItemSubSubCtgryCode" runat="server" Text="." Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <%--                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Select Item<font color="red" size="2"><b>*</b></font>: </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlItems" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Item" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hfItems" runat="server" Value="" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Item Code<font color="red" size="2"><b>*</b></font>: </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtItemCode" Text="" onkeyup="FullItemCode()" CssClass="bcAsptextbox"
                                            MaxLength="7"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Full Item Code<font color="red" size="2"><b>*</b></font>:
                                        </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:Label ID="lblFullItemCode" runat="server" Font-Bold="true" Text="."></asp:Label>
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                </tr>--%>
                                <tr>
                                    <td colspan="3" class="bcTdNewTable">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="4">
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
                                <th width="2%">
                                    Cat Code
                                </th>
                                <th width="30%">
                                    Cat Desc
                                </th>
                                <th width="2%">
                                    SubCat Code
                                </th>
                                <th width="30%">
                                    SubCat Description
                                </th>
                                <th width="2%">
                                    SubSubCat Code
                                </th>
                                <th width="30%">
                                    SubSubCat Description
                                </th>
                                <th width="2%">
                                    E
                                </th>
                                <th width="2%">
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

            function FullItemCode() {
                var ItemCtgryCode = $("[id$=lblItemCtgryCode]").text();
                var ItemSubCtgryCode = $("[id$=lblItemSubCtgryCode]").text();
                var ItemSubSubCtgryCode = $("[id$=lblItemSubSubCtgryCode]").text();
                if (ItemSubSubCtgryCode == ".")
                    ItemSubSubCtgryCode = "";
                var Value = $("[id$=txtItemCode]").val();
                $("[id$=lblFullItemCode]").text(ItemCtgryCode + ItemSubCtgryCode + ItemSubSubCtgryCode + Value);
            }


            function Myvalidations() {
                if ($("[id$=ddlItmCtgry]").val() == "0") {
                    ErrorMessage('Item Category is required.');
                    $("[id$=ddlItmCtgry]").focus();
                    return false;
                }
                else if ($("[id$=ddlItmSubCtgry]").val() == "0") {
                    ErrorMessage('Item Sub Category is required.');
                    $("[id$=ddlItmSubCtgry]").focus();
                    return false;
                }
                else if ($("[id$=ddlItems]").val() == "0") {
                    ErrorMessage('Item is required.');
                    $("[id$=ddlItems]").focus();
                    return false;
                }
                else if ($("[id$=txtItemCode]").val() == "") {
                    ErrorMessage('Item Code is required.');
                    $("[id$=txtItemCode]").focus();
                    return false;
                }
                else if ($("[id$=ddlItmSubSubCtgry]").val() == "0" && $("[id$=txtItemCode]").val().length != 6) {
                    ErrorMessage('Item Code length should be 7 Digits.');
                    $("[id$=txtItemCode]").focus();
                    return false;
                }
                else if ($("[id$=ddlItmSubSubCtgry]").val() != "0" && $("[id$=txtItemCode]").val().length != 4) {
                    ErrorMessage('Item Code length should be 5 Digits.');
                    $("[id$=txtItemCode]").focus();
                    return false;
                }
                else if ($("[id$=lblFullItemCode]").val() == "" || $("[id$=lblFullItemCode]").val() == ".") {
                    ErrorMessage('Full Item Code is required.');
                    $("[id$=lblFullItemCode]").focus();
                    return false;
                }
            }

        });
    </script>
    <script type="text/javascript">

        var oTable;
        $(document).ready(function () {
            //            $("[id$=btnUpdate]").hide();
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
                "sAjaxSource": "ItemCategory.asmx/GetMappedItems",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                "sScrollY": "250px",
                "sScrollX": "100%",
                "bScrollCollapse": true,

                "fnServerParams": function (aoData) {
                    aoData.push({ "name": "iItemDesc", "value": $('[id$=txtItmDscrip]').val() });
                    aoData.push({ "name": "iPartNo", "value": $('[id$=txtItmPrtNmbr]').val() });
                    aoData.push({ "name": "iSpec", "value": $('[id$=txtspec]').val() });
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

        function DeleteDetails(valddd, value) {
            if (confirm("Are you sure you want to Delete?")) {
                var result = ItemsMapping.DeleteItemDetails(value);
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

        function EditDetails(valddd, ID) {
            window.location.replace("ItemsMapping.Aspx?ID=" + ID);
        }
    </script>
</asp:Content>
