<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="Customer_LocalQuotation_Comparision.aspx.cs"
    Inherits="VOMS_ERP.Customer_Access.Customer_LocalQuotation_Comparision" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">
        tr.even:hover td, #Compare_Range_wrapper tr.even:hover .dataTable DTFC_Cloned
        {
            background-color: #CBCBCB;
        }
        tr.odd:hover td, #Compare_Range_wrapper tr.odd:hover .dataTable DTFC_Cloned
        {
            background-color: #CBCBCB;
        }
    </style>
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Customer Local Quotation Comparison Statement"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="6">
                                        All <font color="red" size="4"><b>*</b></font> fields are Mandatory
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
                <table width="98%" style="background-color: #F5F4F4; border: solid 1px #ccc" align="center">
                    <tr>
                        <td class="bcTdnormal">
                            <span id="lblCustName" class="bcLabel">Customer<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                                <asp:ListItem Value="0" Text="Select Customer"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="bcTdnormal">
                            <span id="lblEnqNo" class="bcLabel">Purchase Enquiry Number(s)<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox runat="server" ID="lbEnquiry" CssClass="bcAspMultiSelectListBox" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlEnquiry_SelectedIndexChanged" SelectionMode="Multiple">
                            </asp:ListBox>
                            <asp:DropDownList runat="server" ID="ddlEnquiry" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlEnquiry_SelectedIndexChanged" Visible="false">
                                <asp:ListItem Value="0" Text="Select Enquiry Number"></asp:ListItem>
                            </asp:DropDownList>
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
                        <td colspan="6">
                            <div id="divCompare" runat="server" style="overflow: auto; width: 100%;">
                            </div>
                            <div style="overflow: auto; display: none; width: 98%; height: 400px;">
                                <table style="width: 100%; overflow: auto;" cellpadding="0" cellspacing="0" runat="server"
                                    id="ComparisionGrid">
                                    <%--<div id="tableDiv_Arrays" class="tableDiv" align="center">
                                <table class="FixedTables" runat="server" id="ComparisionGrid" style="width: 1000px;">--%>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div style="padding: 10px 20px 10px 10px; width: 90%;">
                                <div class="sub_heading">
                                    Comments</div>
                                <textarea id="txtComComments" style="border-color: #9BC2EE; border-style: solid;
                                    border-width: 1px;" rows="4" runat="server" cols="138" tabindex="90"></textarea>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table runat="server" id="ComparisionGrid_Export" style="width: 100%; visibility: hidden;">
                            </table>
                            <%--<asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound" visible="false"
                                OnRowCreated="GridView1_RowCreated">
                                <HeaderStyle CssClass="GridviewScrollHeader" />
                                <RowStyle CssClass="GridviewScrollItem" />
                                <PagerStyle CssClass="GridviewScrollPager" />
                            </asp:GridView>--%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" align="right">
                            <center>
                                <asp:HiddenField ID="hdfItmIds" runat="server" Value="" />
                                <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                    <tbody>
                                        <tr valign="middle">
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div1" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnSave" Text="Continue" OnClick="btnSave_Click" />
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/FixedColumns.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/tooltip/jquery.powertip.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            //            $('[id$=GridView1]').gridviewScroll({
            //                width: 1100,
            //                height: 250,
            //                freezesize: 4,
            //                headerrowcount: 2
            //            });
        });

        $(function () {
            $('#mousefollow-examples div').powerTip({
                //followMouse: true,
                mouseOnToPopup: true,
                smartPlacement: true,
                placement: 'nw'
            });
        });

        $(document).ready(function () {
            //Expnder();
        });

        function Expnder() {
            $('div.expanderR').expander();
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            var oTable = $("[id$=Compare_Range]").dataTable({
                "sScrollY": "300px",
                "sScrollX": "100%",
                //"sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bPaginate": false,
                "bSort": false,
                "bFilter": false, //to disable search Box 
                "bInfo": false,
                "bJQueryUI": true,
                "bAutoWidth": false
                //'sDom': 't' 
                //"sDom": '<"top"i>rt<"bottom"flp><"clear">'                
            });
            new FixedColumns(oTable,
            {
                "iLeftColumns": 4
                , "iLeftWidth": 400
            });
        });

        $("[id$=Compare_Range tbody tr]").each(function () {
            $(this).mouseover(function () {
                $(this).addClass("hover");
            }).mouseout(function () {
                $(this).removeClass("hover");
            });
        });

        //        $(document).on({
        //            mouseenter: function() {
        //                trIndex = $(this).index() + 1;
        //                $("table.dataTable DTFC_Cloned").each(function(index) {
        //                    $(this).find("tr:eq(" + trIndex + ")").each(function(index) {
        //                        $(this).find("td").addClass("hover");
        //                    });
        //                });
        //            },
        //            mouseleave: function() {
        //                trIndex = $(this).index() + 1;
        //                $("table.dataTable DTFC_Cloned").each(function(index) {
        //                    $(this).find("tr:eq(" + trIndex + ")").each(function(index) {
        //                        $(this).find("td").removeClass("hover");
        //                    });
        //                });
        //            }
        //        }, ".dataTables_wrapper tr");
    </script>
    <script type="text/javascript">

        function Myvalidations() {

            if (($('[id$=ddlCustomer]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer is Required.</span>');
                $('[id$=ddlCustomer]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if ($('[id$=lbEnquiry]').val() == null) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Enquiry is Required.</span>');
                $('[id$=ddlEnquiry]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            //            else {
            //                var rowCount = 0;
            //                var celCount = 0;
            //                if ($('[id$=Compare_Range]').length > 0) {
            //                    rowCount = ($('[id$=Compare_Range]'))[0].rows.length; //rowCount[0].rows.length
            //                    celCount = ($('[id$=Compare_Range]'))[0].rows[2].cells.length;
            //                }
            //                if (rowCount > 1) {
            //                    var chebx = 0; var sel = 0;
            //                    for (var i = 3; i <= rowCount; i++) {
            //                        var start = 6; var flag = 0;
            //                        for (var j = 0; j < ((celCount - 1) / 6); j++) {
            //                            if ($('[id$=ChkHead' + start + chebx + ']').val() == 'on') {
            //                                if ($('[id$=ChkHead' + start + chebx + ']')[0].checked == true)
            //                                { flag = flag + 1; sel = sel + 1; }
            //                            }
            //                            start = start + 1;
            //                        }
            //                        if (flag > 1) {
            //                            $("#<%=divMyMessage.ClientID %> span").remove();
            //                            $('[id$=divMyMessage]').append('<span class="Error">Please Select only one Supplier for Item Number ' + (i - 2) + '.</span>');
            //                            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            //                            return false;
            //                            break;
            //                        }
            //                        chebx = chebx + 1;
            //                    }
            //                    if (sel == 0) {
            //                        $("#<%=divMyMessage.ClientID %> span").remove();
            //                        $('[id$=divMyMessage]').append('<span class="Error">Select at Least One Item.</span>');
            //                        $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
            //                        return false;
            //                    }
            //                }
            //                else {
            //                    ErrorMessage('No items to save.');
            //                    return false;
            //                }
            //            }
        }

        function CheckAll(id) {
            var rowCount = ($('[id$=ComparisionGrid]'))[0].rows.length;
            if ($('[id$=' + id + ']')[0].checked) {
                if (rowCount > 2) {
                    var inputs = document.getElementsByTagName('input');
                    for (var k = 0; k < inputs.length; k++) {
                        var input = inputs[k]
                        if (input.type != 'checkbox') continue;
                        input.checked = false;
                    }
                    $('[id$=' + id + ']')[0].checked = true;
                    for (var i = 3; i <= rowCount; i++) {
                        if ($('[id$=' + id + (i - 3) + ']').length > 0)
                            $('[id$=' + id + (i - 3) + ']')[0].checked = true;
                    }
                }
            }
            else {
                if (rowCount > 2) {
                    for (var i = 3; i <= rowCount; i++) {
                        if ($('[id$=' + id + (i - 3) + ']').length > 0)
                            $('[id$=' + id + (i - 3) + ']')[0].checked = false;
                    }
                }
            }
        }

        function CheckAll1(id, CNO) {
            var ElmntID = GetClientID("ChkHead" + (parseInt(id))).attr("id");
            var IsChecked = $('#' + ElmntID).is(':checked');
            var rowCount = $('#Compare_Range tbody')[0].rows.length;
            if ($('#' + ElmntID + '')[0].checked) {
                if (rowCount > 2) {
                    var inputs = document.getElementsByTagName('input');
                    for (var k = 0; k < inputs.length; k++) {
                        var input = inputs[k]
                        if (input.type != 'checkbox') continue;
                        input.checked = false;
                    }
                    $('#' + ElmntID + '')[0].checked = true;
                    for (var i = 3; i <= rowCount - 4; i++) {
                        var ItmStatusChk = $('[id$=hfItemStatus' + (i - 3) + ']').val();
                        var ItmStatus = ItmStatusChk == 'undefined' ? "" : ItmStatusChk;
                        //.css('display') == 'none'//.filter(':hidden')//$(element).is(":visible")
                        if (ItmStatus != "" && typeof (ItmStatus) != 'undefined') {
                            if ($('#' + ElmntID + (i - 3) + '').length > 0 && $('#' + ElmntID + (i - 3) + '').css('display') != 'none' && ItmStatus <= 40)//60
                                $('#' + ElmntID + (i - 3) + '')[0].checked = true;
                            else {
                                $('#' + ElmntID + (i - 3) + '')[0].checked = false;
                                //$('#' + ElmntID + '')[0].checked = false;
                                ErrorMessage("These items were already raised");
                            }
                        }
                    }
                }
            }
            else {
                if (rowCount > 2) {
                    for (var i = 3; i <= rowCount; i++) {
                        if ($('#' + ElmntID + (i - 3) + '').length > 0 && $('#' + ElmntID + (i - 3) + '').css('display') != 'none')
                            $('#' + ElmntID + (i - 3) + '')[0].checked = false;
                    }
                }
            }
            var result = Customer_LocalQuotation_Comparision.CheckHeader(CNO, IsChecked);
        }

        function CheckInd(CID, RNO, SupID) {
            var ElmntID = GetClientID("ChkHead" + (parseInt(CID))).attr("id");
            var ItmStatus = $('[id$=hfItemStatus' + (parseInt(RNO)) + ']').val();
            //            if ($('#' + ElmntID).attr('checked') == 'checked') {
            //                $('#' + ElmntID).attr('checked', 'checked');
            //            } else {
            //                $('#' + ElmntID).attr('checked', false);
            //            }

            var IsCheckedMain = $('#' + ElmntID).is(':checked');
            var result = Customer_LocalQuotation_Comparision.CheckInd(RNO, SupID, IsCheckedMain);
            var numCols = $("#Compare_Range tbody")[0].rows[RNO].cells.length;
            for (var i = 5; i < numCols; i++) {

                if ($("#Compare_Range tbody")[0].rows[RNO].children[i].firstChild.type == 'checkbox' && result.value == "Checked") {
                    var ischecked = $("#Compare_Range tbody")[0].rows[RNO].children[i].firstChild.checked;
                    if ($("#Compare_Range tbody")[0].rows[RNO].children[i].firstChild.id != ElmntID && ischecked == true) {
                        ErrorMessage('Only one Quotation can be selected for an Item.');
                    }
                    $("#Compare_Range tbody")[0].rows[RNO].children[i].firstChild.checked = false;
                }
                i += 5;
            }
            if (IsCheckedMain && result.value == "Checked")
                $('#' + ElmntID).attr('checked', 'checked');
            else {
                $('#' + ElmntID).attr('checked', false);
                if (ItmStatus > 40) {
                    ErrorMessage("This item was already raised");
                }
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
    <script type="text/javascript">
        // this "tableDiv" must be the table's class
        $(".tableDiv").each(function () {
            var Id = $(this).get(0).id;
            var maintbheight = 555;
            var maintbwidth = 911;

            $("#" + Id + " .FixedTables").fixedTable({
                width: maintbwidth,
                height: maintbheight,
                fixedColumns: 4,
                // header style
                classHeader: "fixedHead",
                // footer style        
                classFooter: "fixedFoot",
                // fixed column on the left        
                classColumn: "fixedColumn",
                // the width of fixed column on the left      
                fixedColumnWidth: 500,
                // table's parent div's id           
                outerId: Id,
                // tds' in content area default background color                     
                Contentbackcolor: "#FFFFFF",
                // tds' in content area background color while hover.     
                Contenthovercolor: "#99CCFF",
                // tds' in fixed column default background color
                fixedColumnbackcolor: "#FFFFFF",
                // tds' in fixed column background color while hover. 
                fixedColumnhovercolor: "#99CCFF"
            });
        });
    </script>
</asp:Content>
