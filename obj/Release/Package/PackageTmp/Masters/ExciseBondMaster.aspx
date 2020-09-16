<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ExciseBondMaster.aspx.cs" Inherits="VOMS_ERP.Masters.ExciseBondMaster"
     %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Excise Bond Master"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
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
                                    <td colspan="3" class="bcTdNewTable">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span1" class="bcLabelright">Financial Year<font color="red" size="2"><b>*</b></font>:</span></div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:DropDownList runat="server" ID="ddlFnclYr" CssClass="bcAspdropdown">
                                                                <asp:ListItem Text="Select Financial Year" Value="0"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span2" class="bcLabelright">Date<font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtDt" MaxLength="12" CssClass="bcAsptextbox"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span6" class="bcLabelright">Excise Bond Value<font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtExValue" MaxLength="9" CssClass="bcAsptextbox"
                                                                onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                            <%-- onchange='return CheckNmbrs(this)'--%>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span3" class="bcLabelright">Towards Description<font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txttowardsdescription" CssClass="bcAsptextbox">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:HiddenField runat="server" ID="hdfdExBndMasterID" />
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
                                                            <asp:LinkButton runat="server" ID="btnClear" OnClientClick="Javascript:clearAll()"
                                                                Text="Clear" OnClick="btnClear_Click" />
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
                    <tr>
                        <td colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:GridView runat="server" ID="gvExBndMstr" AutoGenerateColumns="False" Width="100%"
                                            RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            OnRowCommand="gvExBndMstr_RowCommand" OnRowDataBound="gvExBndMstr_RowDataBound"
                                            OnPreRender="gvExBndMstr_PreRender">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Financial Year">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCtgryNm" runat="server" Text='<%#Eval("FinancialYear") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bond Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStartDt" runat="server" Text='<%#Eval("BondDate") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Excise Bond Value">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblexdt" runat="server" Text='<%#Eval("BondValue") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Towords Description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblexdtDescription" runat="server" Text='<%#Eval("TwsDscptn") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Is Current Year" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblExBndMaster" runat="server" Text='<%#Eval("ExciseBondID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                                    Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Remove"
                                                    Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                            </Columns>
                                        </asp:GridView>
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
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
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
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvExBndMstr]").dataTable({
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
                    "sSearch": "Search :"
                },

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
            oTable = $("[id$=gvTxMstr]").dataTable();
        });

        function Myvalidations() {
            if (($('[id$=ddlFnclYr]').val()).trim() == '0') {
                ErrorMessage('Financial Year is Required.');
                $('[id$=ddlFnclYr]').focus();
                return false;
            }
            else if (($('[id$=txtDt]').val()).trim() == '') {
                ErrorMessage('Bond Date is Required.');
                $('[id$=txtDt]').focus();
                return false;
            }
            else if (($('[id$=txtExValue]').val()).trim() == '') {
                ErrorMessage('Excise Bond Value is Required.');
                $('[id$=txtExValue]').focus();
                return false;
            }
            else if (($('[id$=txttowardsdescription]').val()).trim() == '') {
                ErrorMessage('Towards Description is Required.');
                $('[id$=txttowardsdescription]').focus();
                return false;
            }
            else {
                return true;
            }
        }
        var dateToday = new Date();
        $('[id$=txtDt]').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true,
            maxDate: dateToday
        });

        function CheckNmbrs(txtbox) {
            var regex = /^\d+(?:\.\d{0,2})$/;
            if (txtbox.value == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Excise Bond Value is Required.</span>');
                $('[id$=txtExValue]').focus();
                $('[id$=divMyMessage]').fadeTo(500, 1).fadeOut(2000);
                return false;
            }
            else {
                if (!regex.test(txtbox.value)) {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Invalid Number Format !Enter in 00.00 Formats.</span>');
                    $('[id$=txtExValue]').focus();
                    $('[id$=divMyMessage]').fadeTo(500, 1).fadeOut(2000);
                    txtbox.value = '';
                    return false;
                }
                else
                    return true;
            }
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode != 45 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        
    </script>
</asp:Content>
