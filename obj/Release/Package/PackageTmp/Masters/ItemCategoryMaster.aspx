<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ItemCategoryMaster.aspx.cs" Inherits="VOMS_ERP.Masters.ItemCategoryMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hdfdItmCategoryID" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Item Category Master"
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
                                                    <asp:TextBox runat="server" ID="txtitemCatCode" CssClass="bcAsptextbox" onblur="SearchItmCode();"
                                                        MaxLength="2" onkeyup="extractNumber(this, 0, false);"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Item Category Description<font color="red" size="2"><b>*</b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtItemCatDesc" onkeyup="alphaNumericSpaceAnd_(this)" CssClass="bcAsptextbox" MaxLength="1000"></asp:TextBox>
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
                    <table id="gvItmCategory" class="widthFull fontsize10 displayNone" cellpadding="0"
                        cellspacing="0" border="0" width="100%">
                        <thead>
                            <tr>
                                <th id="ID" runat="server" visible="false">
                                </th>
                                <th width="05%">
                                    Item Category Code
                                </th>
                                <th width="15%">
                                    Item Category Description
                                </th>
                                <th width="15%">
                                     Is Not Sub Sub Category
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
            if (($('[id$=txtitemCatCode]').val()).trim() == '') {
                ErrorMessage('Item Category Code is Required.');
                $('[id$=txtitemCatCode]').focus();
                return false;
            }
            else if (($('[id$=txtItemCatDesc]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Item Category Description is Required.</span>');
                $('[id$=txtItemCatDesc]').focus();
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
            oTable = $("[id$=gvItmCategory]").dataTable({
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
                "sAjaxSource": "../Enquiries/WebService1.asmx/GetItemsCategory",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                "sScrollY": "250px",
                "sScrollX": "100%",
                "bScrollCollapse": true,
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
								    $("#gvItmCategory").show();
								}
                    });
                }
            });
        });
        /* Init the table */
        oTable = $("#gvItmCategory").dataTable();

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });

        function SearchItmCode() {
            var value1 = $('[id$=txtitemCatCode]').val();
            if (value1.length == 2) {
                var result = ItemCategoryMaster.CheckCode(value1);
                if (result.value == false) {
                    ErrorMessage("Item Category Code Exists");
                    $('[id$=txtitemCatCode]').val('');
                    return false
                }
            }
            else {
                ErrorMessage("Item Category Code Required Two Digits");
                $('[id$=txtitemCatCode]').val('');
                return false
            }
        }

        function CheckUnCheck() {
            var value1 = $('[id$=hdfdItmCategoryID]').val();
            var IsChecked = $('[id$=ChkIsSpareParts]')[0].checked;
            alert(IsChecked);
            //            var result = ItemCategoryMaster.CheckCode(value1);
            //            if (result.value == false) {
            //                ErrorMessage("Item Category Code Exists");
            //                $('[id$=txtitemCatCode]').val('');
            //                return false
            //            }
        }

        $('[id$=ChkIsSpareParts]').click(function () {
            var value1 = $('[id$=hdfdItmCategoryID]').val();
            if (value1 != "") {
                var rslt = ItemCategoryMaster.CheckUnCheck(value1, this.checked);
                if (rslt.value == false) {
                    this.checked = true;
                    ErrorMessage('you cannot UnCheck as it is used in ItemMaster.');
                }
            }
        });

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                var DelVal = valddd.parentNode.parentNode.id;
                var EditVal = $("[id$=hdfdItmCategoryID]").val();
                if (DelVal.toLowerCase() != EditVal.toLowerCase()) {
                    if (confirm("Are you sure you want to Delete?")) {
                        var result = ItemCategoryMaster.DeleteItemDetails(DelVal, CreatedBy, IsCust);
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

        function EditDetails(valddd, Enable) {
            try {

                $("[id$=txtitemCatCode]").val('');
                $("[id$=txtItemCatDesc]").val('');
                             
                $("[id$=hdfdItmCategoryID]").val(valddd.parentNode.parentNode.id);

                $("[id$=txtitemCatCode]").val(valddd.parentNode.parentNode.childNodes[0].innerHTML.trim());
                if (parseInt(Enable) > 0)
                    $("[id$=txtitemCatCode]").prop('disabled', true);
                else
                    $("[id$=txtitemCatCode]").removeAttr('disabled');
                $("[id$=txtItemCatDesc]").val(valddd.parentNode.parentNode.childNodes[1].innerHTML.trim());

                var IsChecked = valddd.parentNode.parentNode.childNodes[2].innerHTML;
                if (IsChecked == 'True')
                    $("[id$=ChkIsSpareParts]").attr('checked', 'checked');
                else
                    $("[id$=ChkIsSpareParts]").removeAttr('checked');

                $("[id$=btnSave]").hide();
                $("[id$=btnUpdate]").show();
                document.getElementById('<%= btnUpdate.ClientID %>').innerHTML = 'Update';
            } catch (e) {
                alert(e.Message);
            }
        }
    </script>
</asp:Content>
