<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="EnumsMaster.aspx.cs" Inherits="VOMS_ERP.Masters.EnumsMaster"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input id="hiddenddlEnmPrnt" type="hidden" value="0" runat="server" />
    <asp:HiddenField ID="hfEditID" runat="server" Value="" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <div id="divMessage">
                </div>
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Enums Master" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;">
                                        <span id="Span6" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;" class="bcTdnormal" colspan="3">
                            <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                width: 98%; margin: 5px; height: 99%;">
                                <div class="row" style="text-align: center; width: 98%;">
                                    <div style="text-align: right; width: 49%;">
                                        <span>
                                            <asp:RadioButton runat="server" ID="rbgnenmtp" GroupName="enm" Text="Enum Type" onclick="javascript:RbtnShow('rbgnenmtp', 'dvenmtp', 'dvenmDesc', 'dvEnmTp', 'dvEnmMstr')" />
                                        </span>
                                    </div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:RadioButton runat="server" ID="rbtnenmdescs" GroupName="enm" Text="Enum" onclick="javascript:RbtnShow('rbtnenmdescs', 'dvenmDesc', 'dvenmtp', 'dvEnmMstr', 'dvEnmTp')" />
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;" colspan="3" class="bcTdnormal">
                            <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                width: 98%; margin: 5px; height: 99%;">
                                <div id="dvenmtp" class="row" style="text-align: center; width: 98%; display: none;">
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span1" class="bcLabelright">Enum Type<font color="red" size="2"><b>*</b></font>:</span></div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtEnmTp" onkeypress="return SpclChars(event);" onchange="SearchDescTp()"
                                                class="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                        </span>
                                    </div>
                                </div>
                                <div id="dvenmDesc" class="row" style="text-align: center; width: 98%;">
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span3" class="bcLabelright">Enum Type<font color="red" size="2"><b>*</b></font>:</span></div>
                                    <div style="text-align: left; width: 50%;">
                                        <asp:DropDownList runat="server" ID="ddlENmTP" CssClass="bcAspdropdown" onchange=" javascript:CheckParent('ddlENmTP', 'enmPrnt', 'enmPrntddl');javascript:UpdateDropDownList('ddlENmTP', 'EnumsMaster.Aspx', 'ddlEnmPrnt');">
                                        </asp:DropDownList>
                                    </div>
                                    <div id="enmPrnt" style="text-align: right; display: none; width: 49%;">
                                        <span id="Span2" class="bcLabelright"><font color="red" size="2"><b>*</b></font>:</span>
                                        <asp:Label runat="server" class="bcLabelright" ID="lblPrntTxt" Text=""></asp:Label></div>
                                    <div id="enmPrntddl" style="text-align: left; display: none; width: 50%;">
                                        <span>
                                            <asp:DropDownList runat="server" ID="ddlEnmPrnt" CssClass="bcAspdropdown" Enabled="false"
                                                onchange="javascript:getval('hiddenddlEnmPrnt', 'ddlEnmPrnt')">
                                            </asp:DropDownList>
                                        </span>
                                    </div>
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span4" class="bcLabelright"><font color="red" size="2"><b>*</b></font>:</span>
                                        <asp:Label runat="server" class="bcLabelright" ID="lblUsrTxt" Text="Enum Description"></asp:Label>
                                    </div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtEnmDesrp" onkeypress="return SpclChars(event);"
                                                onchange="SearchDesc()" MaxLength="150" class="bcAsptextbox"></asp:TextBox>
                                            &nbsp; </span>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click"></asp:LinkButton>
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div3" class="bcButtonDiv">
                                                <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="bcTdNewTable">
                            <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                width: 98%; margin: 5px; height: 99%;">
                                <div id="Div4" class="row" style="text-align: center; width: 98%; display: None;">
                                    <div style="text-align: right; width: 49%;">
                                        <span id="Span5" class="bcLabelright">Enum Type/Enum Master:</span></div>
                                    <div style="text-align: left; width: 50%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtSearch" class="autosuggest" CssClass="bcAsptextbox"
                                                MaxLength="100"></asp:TextBox>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <div id="dvEnmMstr">
                                <asp:GridView runat="server" ID="gvEnumMaster" RowStyle-CssClass="bcGridViewRowStyle"
                                    RowStyle-VerticalAlign="Bottom" AutoGenerateColumns="False" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                    AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" OnRowCommand="gvEnumMaster_RowCommand"
                                    OnRowDataBound="gvEnumMaster_RowDataBound" Caption="EnumMaster" OnPreRender="gvEnumMaster_PreRender">
                                    <Columns>
                                        <asp:TemplateField HeaderText="S.No." HeaderStyle-Width="10px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Type" DataField="Type" HeaderStyle-Width="170px" />
                                        <asp:TemplateField HeaderText="Description">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblDescription" Text='<%#Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Parent">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblEnmPrnt" Text='<%#Eval("Parent") %>'></asp:Label>
                                                <asp:Label runat="server" ID="lblEnmPrntID" Text='<%#Eval("PrentID") %>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Enum Type ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblEnmTpID" Text='<%#Eval("EnumTypeID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Enum Master ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblEnmMsterID" Text='<%#Eval("ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                            Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Remove"
                                            Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div id="dvEnmTp">
                                <asp:GridView runat="server" ID="gvEnmTp" AutoGenerateColumns="False" RowStyle-CssClass="bcGridViewRowStyle"
                                    EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                    PagerStyle-HorizontalAlign="Center" class="display" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                    AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" OnRowCommand="gvEnmTp_RowCommand"
                                    OnRowDataBound="gvEnmTp_RowDataBound" Caption="EnumType" OnPreRender="gvEnmTp_PreRender">
                                    <Columns>
                                        <asp:TemplateField HeaderText="S.No." HeaderStyle-Width="5px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Type">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblEnmTPNm" Text='<%#Eval("Name") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Description" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblID" Text='<%#Eval("EnmID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                            Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Remove"
                                            Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
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
            RbtnShow('rbtnenmdescs', 'dvenmDesc', 'dvenmtp', 'dvEnmMstr', 'dvEnmTp');
        });
    </script>
    <script type="text/javascript">
        var oTable, oTable1, oTable2, oTable3;
        $(document).ready(function () {
            $('[id$=gvEnumMaster]').dataTable(
            {
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search:"
                },
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
            oTable = $("[id$=gvEnumMaster]").dataTable();
        });

        //------------- -----------------

        function SearchDescTp() {
            var value1 = $('[id$=txtEnmTp]').val();
            oTable1.fnFilter(value1, 1);
            if ($('[id$=gvEnmTp] >tbody >tr >td').length > 1) {
                ErrorMessage('Enum Master Already Exist for this Enum Type');
                $('[id$=txtEnmTp]').val('');
            }
            oTable1.fnFilter('', 1);
        }

        // -------------         ------------

        $(document).ready(function () {
            $("[id$=gvEnmTp]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search:"
                },
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
            oTable1 = $("[id$=gvEnmTp]").dataTable();
        });

        //-------------      -----------

        function SearchDesc() {
            var value1 = $("[id*='ddlENmTP'] :selected").text();
            oTable.fnFilter("^" + value1 + "$", 1, true);
            if ($('[id$=gvEnumMaster] >tbody >tr >td').length > 1 &&
            (($('[id$=ddlENmTP]').val()).trim() == '4' || ($('[id$=ddlENmTP]').val()).trim() == '5' || ($('[id$=ddlENmTP]').val()).trim() == '24')) {
                var value2 = $("[id*='ddlEnmPrnt'] :selected").text();
                oTable.fnFilter("^" + value2 + "$", 3, true);
                if ($('[id$=gvEnumMaster] >tbody >tr >td').length > 1) {
                    var value3 = $('[id$=txtEnmDesrp]').val();
                    if (value3 != '') {
                        oTable.fnFilter("^" + value3 + "$", 2, true);
                        if ($('[id$=gvEnumMaster] >tbody >tr >td').length > 1) {
                            for (var i = 1; i < oTable[0].rows.length; i++) {
                                var aData = oTable[0].rows[i];
                                if (aData.cells[2].textContent.trim().toLowerCase() == value3.trim().toLowerCase()) {
                                    alert('Enum Master Already Exist for this Enum Type');
                                    $('[id$=txtEnmDesrp]').val('');
                                }
                            }
                        }
                    }
                }
            }
            else if ($('[id$=gvEnumMaster] >tbody >tr >td').length > 1) {
                var value2 = $('[id$=txtEnmDesrp]').val();
                if (value2 != '') {
                    oTable.fnFilter("^" + value2 + "$", 2, true);
                    if ($('[id$=gvEnumMaster] >tbody >tr >td').length > 1) {
                        alert('Enum Master Already Exist for this Enum Type');
                        $('[id$=txtEnmDesrp]').val('');
                    }
                }
            }
            oTable.fnFilter('', 1); oTable.fnFilter('', 2); oTable.fnFilter('', 3);
        }

        function deleteID(DID) {
            var EditID = $('[id$=hfEditID]').val().toLower();
            alert(DID);
            if (DID.toLower() == EditID) {
                ErrorMessage('You cannot Delete, when the same row is Editing.');
                return false;
            }
            else {
                if (confirm('Are you sure you want to delete this?'))
                    return true;
            }
        }
    </script>
    <script type="text/javascript">
        function CheckParent(ddlselect, hd1, hd2) {

            if (($('[id$=ddlENmTP] :selected').text()).trim() == 'State' || ($('[id$=ddlENmTP] :selected').text()).trim() == 'City' || ($('[id$=ddlENmTP] :selected').text()).trim() == 'Incoterms Sepc') {
                document.getElementById(hd1).style.display = 'block';
                document.getElementById(hd2).style.display = 'block';
                if (($('[id$=ddlENmTP] :selected').text()).trim() == 'State')
                    $('[id$=lblPrntTxt]').text('Select Country');
                else if (($('[id$=ddlENmTP] :selected').text()).trim() == 'City')
                    $('[id$=lblPrntTxt]').text('Select State');
                else if (($('[id$=ddlENmTP] :selected').text()).trim() == 'Incoterms Sepc')
                    $('[id$=lblPrntTxt]').text('Select Incoterms');
            }
            else {
                document.getElementById(hd1).style.display = 'none';
                document.getElementById(hd2).style.display = 'none';
                $('[id$=lblPrntTxt]').val('');
            }
            if (($('[id$=ddlENmTP]').val()).trim() != '0')
                $('[id$=lblUsrTxt]').text('Enter ' + $("[id*='ddlENmTP'] :selected").text());
            else
                $('[id$=lblUsrTxt]').text('Enter Description');
        }
        var DstnddlVar; var val1; var isprnt = false;
        function UpdateDropDownList(Scrddl, pageName, Dstnddl, valu) {
            xmlHttp = null; DstnddlVar = Dstnddl; val1 = valu;
            xmlHttp = getXMLHttpRequest();
            if (xmlHttp != null) {
                var contID = document.getElementById("ctl00_ContentPlaceHolder1_" + Scrddl).value;
                var contName = ($('[id$=ddlENmTP] :selected').text());
                if (contName == 0) {
                    document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).disabled = true;
                    return false;
                }
                else {
                    document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).value = 0;
                    document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).disabled = false;
                }
                xmlHttp.onreadystatechange = state_Change;
                xmlHttp.open("GET", pageName + "?ID=" + contName, true);
                xmlHttp.send(null);
            }
        }
        function state_Change() {
            if (xmlHttp.readyState == 4) {
                if (xmlHttp.status == 200) {
                    var countries = xmlHttp.responseText.split(';');
                    var length = countries.length;
                    document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).options.length = 0;
                    var dropDown = document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar);
                    for (var i = 0; i < length - 1; ++i) {
                        var optn = countries[i].split(',');
                        var option = document.createElement("option");
                        option.text = optn[0];
                        option.value = optn[1];
                        dropDown.options.add(option);
                        if (optn[1] == val1) {
                            option.selected = true;
                            isprnt = true;
                            $('[id$=hiddenddlEnmPrnt]').val(val1);
                        }
                    }
                    if (!isprnt) {
                        $('[id$=hiddenddlEnmPrnt]').val('0');
                    }
                    dropDown.disabled = false;
                    dropDown.focus();
                }
            }
        }
        function Myvalidations() {
            if ($('[id$=rbtnenmdescs]')[0].checked) {
                if (($('[id$=ddlENmTP]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Enum Type is Required.</span>');
                    $('[id$=ddlENmTP]').focus();
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    return false;
                }
                if (($('[id$=ddlENmTP] option:selected').text()).trim() == 'City' || ($('[id$=ddlENmTP] option:selected').text()).trim() == 'State' || ($('[id$=ddlENmTP]').val()).trim() == 'Incoterms Sepc') {
                    if (($('[id$=ddlEnmPrnt]').val()).trim() == '0') {
                        $("#<%=divMyMessage.ClientID %> span").remove();
                        $('[id$=divMyMessage]').append('<span class="Error">' + $("[id*='ddlENmTP'] :selected").text() + ' Parent is Required.</span>');
                        $('[id$=ddlENmTP]').focus();
                        $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                        return false;
                    }
                }
                if (($('[id$=txtEnmDesrp]').val()).trim() == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Enum Description is Required.</span>');
                    $('[id$=txtEnmDesrp]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }

                else {
                    return true;
                }
            }
            else if ($('[id$=rbgnenmtp]')[0].checked) {
                if (($('[id$=txtEnmTp]').val()).trim() == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Enum Type is Required.</span>');
                    $('[id$=txtEnmTp]').focus();
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    return false;
                }
                else {
                    return true;
                }
            }
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function SpclChars(evt) {
            return true;
        }
        function validateEmail(emailField) {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            if (emailField.value == '') {
                return true;
            }
            else if (reg.test(emailField.value) == false) {
                emailField.value = '';
                emailField.focus();
                alert('invalid Email-ID');
                return false;
            }
            return true;
        }
        window.onload = function () {
            RbtnShow('rbtnenmdescs', 'dvenmDesc', 'dvenmtp', 'dvEnmMstr', 'dvEnmTp');
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
</asp:Content>
