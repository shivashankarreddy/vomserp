<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="FQComparisionByItems.aspx.cs" Inherits="VOMS_ERP.Purchases.FQComparisionByItems"
     %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; overflow: auto; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <asp:HiddenField ID="HFFQSelectedItems" runat="server" />
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Foreign Quotation Comparison Statement"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="6">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="98%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                    align="center">
                    <tr>
                        <td class="bcTdnormal" width = "13%">
                            <span id="lblCustName" class="bcLabel" runat="server">Customer Name<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlcustmr" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged">
                                <asp:ListItem Value="00000000-0000-0000-0000-000000000000" Text="Select Customer"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="bcTdnormal" width = "16%">
                            <span id="lblEnqNo" class="bcLabel">Foreign Enquiry Number(s)<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlfenqy" CssClass="bcAspdropdown" AutoPostBack="true"
                                Visible="false">
                                <asp:ListItem Value="00000000-0000-0000-0000-000000000000" Text="Select Enquiry Number"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ListBox runat="server" ID="Lstfenqy" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                OnSelectedIndexChanged="Lstfenqy_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                        </td>
                        <td colspan="2" style="width: 30%">
                            <div runat="server" id="dvexport">
                                <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                    class="item_top_icons" title="Export Excel" OnClick="btnExcelExpt_Click" />
                                <%--<asp:ImageButton ID="btnWordExpt" runat="server" ImageUrl="../images/word.png" class="item_top_icons"
                                    title="Export Word" OnClick="btnWordExpt_Click" />--%>
                                <%--<asp:ImageButton ID="btnPdfExpt" runat="server" ImageUrl="../images/pdf.png" class="item_top_icons"
                                    title="Export PDF" OnClick="btnPdfExpt_Click"></asp:ImageButton>--%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div id="tableDiv_Arrays" class="tableDiv" align="center">
                                <table class="FixedTables" runat="server" id="ComparisionGrid" style="width: 1000px;">
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <%--visibility: hidden;--%>
                            <table runat="server" id="ComparisionGrid_Export" style="width: 100%; visibility: hidden;">
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div id="divCompare" runat="server" style="overflow: auto; width: 100%;">
                            </div>
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
                                                    <asp:LinkButton runat="server" ID="btnSave" Text="Select" OnClick="btnSave_Click" />
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div2" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div3" class="bcButtonDiv">
                                                    <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit</a>
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
    <style type="text/css">
        tr.even:hover td, #Compare_Range_wrapper tr.even:hover .dataTable
        {
            background-color: #CBCBCB;
        }
        tr.odd:hover td, #Compare_Range_wrapper tr.odd:hover .dataTable
        {
            background-color: #CBCBCB;
        }
    </style>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/FixedColumns.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/tooltip/jquery.powertip.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
        $(function () {
            $('#mousefollow-examples div').powerTip({
                //followMouse: true,
                mouseOnToPopup: true,
                smartPlacement: true,
                placement: 'nw'
            });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            var oTable = $("#Compare_Range").dataTable({
                "sScrollY": "300px",
                "sScrollX": "100%",
                //"sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bPaginate": false,
                "bSort": false,
                "bFilter": false,
                "bInfo": false,
                "bJQueryUI": true,
                "bAutoWidth": false              
            });
            new FixedColumns(oTable,
            {
                "iLeftColumns": 4
                , "iLeftWidth": 400
            });
        });

        $(document).on({
            mouseenter: function () {
                trIndex = $(this).index() + 1;
                $("table.dataTable").each(function (index) {
                    $(this).find("tr:eq(" + trIndex + ")").each(function (index) {
                        $(this).find("td").addClass("hover");
                    });
                });
            },
            mouseleave: function () {
                trIndex = $(this).index() + 1;
                $("table.dataTable").each(function (index) {
                    $(this).find("tr:eq(" + trIndex + ")").each(function (index) {
                        $(this).find("td").removeClass("hover");
                    });
                });
            }
        }, ".dataTables_wrapper tr");
    </script>
    <script type="text/javascript">

        function Myvalidations() {
            var rowCount = (($('[id$=Compare_Range]')).length > 0) ? ($('[id$=Compare_Range]'))[0].rows.length : 0; //rowCount[0].rows.length
            if (($('[id$=ddlcustmr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer is Required.</span>');
                $('[id$=ddlcustmr]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if ($('[id$=Lstfenqy]').val() == null) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Enquiry is Required.</span>');
                $('[id$=Lstfenqy]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            if (rowCount > 1) {
                var chebx = 0; var sel = 0;
                var celCount = ($('[id$=Compare_Range]'))[0].rows[2].cells.length;
                for (var i = 3; i <= rowCount; i++) {
                    var start = 6; var flag = 0;
                    for (var j = 0; j < ((celCount - 1) / 6); j++) {
                        if ($('[id$=ch1' + start + chebx + ']').val() == 'on') {
                            if ($('[id$=ch1' + start + chebx + ']')[0].checked == true)
                            { flag = flag + 1; sel = sel + 1; }
                        }
                        start = start + 1;
                    }
                    if (flag > 1) {
                        $("#<%=divMyMessage.ClientID %> span").remove();
                        $('[id$=divMyMessage]').append('<span class="Error">Please Select only one Supplier for Item Number ' + (i - 2) + '.</span>');
                        $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                        return false;
                        break;
                    }
                    chebx = chebx + 1;
                }
                if (sel == 0) {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Select At Least One Item.</span>');
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    return false;
                }
            }
            else if (rowCount <= 1) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Select At Least One Item.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }

        function CheckIndividual(id, RNo, QNo) {
            var rowCount = ($('[id$=Compare_Range]'))[0].rows.length;
            var ErrMsg = "";
            var result = "";
            var SelectedItems = $('[id$=HFFQSelectedItems]').val();
            if ($("#ch1" + id + "")[0].checked) {
                if (rowCount > 2) {
                    if ($('[id$=HFFQSelectedItems]').val() == "") {
                        var FEID = $('[id$=Lstfenqy]').val();
                        result = FQComparisionByItems.GetSelectedItems(FEID);
                        $('[id$=HFFQSelectedItems]').val(result.value);
                    }
                    var ElmntID = $("#ch1" + id + "")[0].id;
                    var numCols = $("#Compare_Range tbody")[0].rows[RNo - 1].cells.length;
                    for (var i = 4; i < numCols; i++) {
                        if ($("#Compare_Range tbody")[0].rows[RNo - 1].children[i].firstChild.type == 'checkbox') {
                            var ischecked = $("#Compare_Range tbody")[0].rows[RNo - 1].children[i].firstChild.checked;
                            if ($("#Compare_Range tbody")[0].rows[RNo - 1].children[i].firstChild.id != ElmntID && ischecked == true) {
                                ErrorMessage('Only one Supplier can be selected for an Item.');
                            }
                            $("#Compare_Range tbody")[0].rows[RNo - 1].children[i].firstChild.checked = false;
                        }
                        i += 4;
                    }

                    if ($('[id$=ch1' + id + ']').length > 0) {
                        var ItemId = $('[id$=hdn' + RNo + ']').val();
                        var fenqID = $('[id$=fenqid' + id + ']').val();
                        var fenq_ItmID = (fenqID + '_' + ItemId);
                        //var exists = $.inArray(ItemId, SelectedItems.split(',')) != -1;

                        if ($.inArray(fenq_ItmID, SelectedItems.split(',')) == -1) {
                            //if (SelectedItems.indexOf(ItemId) == -1) {
                            $('[id$=ch1' + id + ']')[0].checked = true;
                        }
                        else {
                            //$('[id$=ch1' + id + ']')[0].checked = false;
                            $('#ch1' + id).attr('checked', false);
                            ErrorMessage("FPO/LPO was release for this Item.");
                        }
                    }
                }
            }

            var IsChecked = $('#ch1' + id).is(':checked');
            var reslt = FQComparisionByItems.ChecIndividual(RNo, QNo, IsChecked);
            if (reslt.value != '')
                ErrMsg(reslt.value);
        }

        function CheckAll(id, QNo) {
            var rowCount = ($('[id$=Compare_Range]'))[0].rows.length;
            var ErrMsg = "";
            var result = "";
            var SelectedItems = $('[id$=HFFQSelectedItems]').val();
            if ($("#ch1" + id + "")[0].checked) {
                if (rowCount > 2) {
                    var inputs = document.getElementsByTagName('input');
                    for (var k = 0; k < inputs.length; k++) {
                        var input = inputs[k]
                        if (input.type != 'checkbox') continue;
                        input.checked = false;
                    }
                    if ($('[id$=HFFQSelectedItems]').val() == "") {
                        var FEID = $('[id$=Lstfenqy]').val();
                        result = FQComparisionByItems.GetSelectedItems(FEID);
                        $('[id$=HFFQSelectedItems]').val(result.value);
                    }
                    $('[id$=ch1' + id + ']')[0].checked = true;
                    for (var i = 3; i <= rowCount; i++) {
                        if ($('[id$=ch1' + id + (i - 3) + ']').length > 0 && $('[id$=ch1' + id + (i - 3) + ']').css('display') != 'none') {
                            var ItemId = $('[id$=hdn' + (i - 2) + ']').val();
                            var fenqID = $('[id$=fenqid' + id + (i - 3) + ']').val();
                            var fenq_ItmID = (fenqID + '_' + ItemId);
                            if ($.inArray(fenq_ItmID, SelectedItems.split(',')) == -1) {
                                //if (SelectedItems.indexOf(ItemId) == -1)
                                $('[id$=ch1' + id + (i - 3) + ']')[0].checked = true;
                            }
                            else {
                                //$('[id$=ch1' + id + (i - 3) + ']')[0].checked = false;
                                $('#ch1' + id + (i - 3)).attr('checked', false);
                                ErrMsg = ErrMsg + (i - 2) + ",";
                            }
                        }
                    }
                    if (ErrMsg != "")
                        ErrorMessage("FPO/LPO was release for " + ErrMsg + " Items.");
                }
            }
            else {
                if (rowCount > 2) {
                    for (var i = 3; i <= rowCount; i++) {
                        if ($('[id$=ch1' + id + (i - 3) + ']').length > 0 && $('[id$=ch1' + id + (i - 3) + ']').css('display') != 'none')
                            $('[id$=ch1' + id + (i - 3) + ']')[0].checked = false;
                    }
                }
            }
            var IsChecked = $('#ch1' + id).is(':checked');
            var reslt = FQComparisionByItems.CheckAll(QNo, IsChecked);
            if (reslt.value != '')
                ErrMsg(reslt.value);
        }

    </script>
</asp:Content>