<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="ItemMaster.aspx.cs" Inherits="VOMS_ERP.Masters.ItemMaster"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Item Master" CssClass="bcTdTitleLabel"> </asp:Label><div
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
                                                    <asp:DropDownList runat="server" ID="ddlItmCtgry" CssClass="bcAspdropdown">
                                                        <asp:ListItem Text="Select ItemCategory" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
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
                        border="0" width = "100%" >
                        <thead>
                            <tr>
                                <th id="ID" runat="server" visible="false">
                                </th>
                                <th width="03%">
                                    Category
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
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
                $('[id$=divMyMessage]').append('<span class="Error">Category is Required.</span>');
                $('[id$=ddlItmCtgry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtItmDscrip]').val()).trim() == '') {
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
                var result = ItemMaster.CheckItemsMaster(Item, ItemPrtNmbr, ItemSpec, EditItmID, IsChecked);
                if (result.value == false) {
                    $("#<%=txtItmDscrip.ClientID%>")[0].value = '';
                    $("#<%=txtItmPrtNmbr.ClientID%>")[0].value = '';
                    $("#<%=txtspec.ClientID%>")[0].value = '';
                    $("#<%=ChkHadSubItems.ClientID%>")[0].checked = false;
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Item Already Exists.</span>');
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    $("#<%=txtItmDscrip.ClientID%>")[0].focus();
                    return false;
                }
                else
                    return true;
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

            var result = ItemMaster.CheckItemsMaster(Item, ItemPrtNmbr, ItemSpec, EditItmID, IsChecked);
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
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "../Enquiries/WebService1.asmx/GetIMItems",
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

        function SearchDesc() {
            var value1 = $('[id$=txtItmDscrip]').val();
            oTable.fnFilter("^" + value1 + "$", 1, true);
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

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = ItemMaster.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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
            } catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd) {
            try {
                var value = valddd.parentNode.parentNode.id;
                $("[id$=HF_EditID]").val(value);
                $("[id$=hdfdItmMstrID]").val(valddd.parentNode.parentNode.id);
                $("[id$=txtItmDscrip]").val(valddd.parentNode.parentNode.childNodes[1].innerHTML);
                $("[id$=txtItmPrtNmbr]").val(valddd.parentNode.parentNode.childNodes[2].innerHTML);
                $("[id$=txtspec]").val(valddd.parentNode.parentNode.childNodes[3].innerHTML);
                $("[id$=ChkHadSubItems]").val(valddd.parentNode.parentNode.childNodes[4].innerHTML);
                $("[id$=btnSave]").hide(); 
                $("[id$=btnUpdate]").show(); 
                var IsChecked = valddd.parentNode.parentNode.childNodes[4].innerHTML;
                if (IsChecked == 'True')
                    $("[id$=ChkHadSubItems]").attr('checked', 'checked');
                else
                    $("[id$=ChkHadSubItems]").removeAttr('checked');
                document.getElementById('<%= btnUpdate.ClientID %>').innerHTML = 'Update';

            } catch (e) {
                alert(e.Message);
            }
        }
    </script>
</asp:Content>
