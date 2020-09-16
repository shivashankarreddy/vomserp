<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="TaxMater.aspx.cs" Inherits="VOMS_ERP.Masters.TaxMater"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Tax Master" CssClass="bcTdTitleLabel"></asp:Label><div
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
                                                <td>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span1" class="bcLabelright">Financial Year<font color="red" size="2"><b>*</b></font>:</span></div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:DropDownList runat="server" ID="ddlFnclYr" onchange="SearchFnclrYr()" CssClass="bcAspdropdown">
                                                                <asp:ListItem Text="Select Financial Year" Value="0"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span6" class="bcLabelright">Excise Duty<font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtExdt" MaxLength="5" CssClass="bcAsptextbox" onchange='return CheckNmbrs(this)'
                                                                onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span7" class="bcLabelright">Sales Tax <font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtSltx" MaxLength="5" CssClass="bcAsptextbox" onchange='return CheckNmbrs(this)'
                                                                onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span5" class="bcLabelright">Dollar Conversion Rate(Rs.) <font color="red"
                                                                size="2"><b>*</b></font>: </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtDCRt" MaxLength="5" CssClass="bcAsptextbox" onchange='return CheckNmbrs(this)'
                                                                onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span2" class="bcLabelright">From<font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtFrmDt" onchange="changedate(this.id);" MaxLength="12"
                                                                CssClass="bcAsptextbox"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span3" class="bcLabelright">To<font color="red" size="2"><b>*</b></font>:</span></div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtToDt" MaxLength="12" CssClass="bcAsptextbox"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:HiddenField runat="server" ID="hdfdTaxMasterID" />
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
                                        <asp:GridView runat="server" ID="gvTxMstr" AutoGenerateColumns="False" Width="100%"
                                            RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            OnRowCommand="gvTxMstr_RowCommand" OnRowDataBound="gvTxMstr_RowDataBound" OnPreRender="gvTxMstr_PreRender">
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
                                                <asp:TemplateField HeaderText="Excise Duty">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblexdt" runat="server" Text='<%#Eval("ExPercent") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sales Tax">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsltx" runat="server" Text='<%#Eval("Salestax") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Conversion Rate($)">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDCRt" runat="server" Text='<%#Eval("DCRate") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Start Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStartDt" runat="server" Text='<%#Eval("StartDate") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="End Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEndDt" runat="server" Text='<%#Eval("EndDate") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Is Current Year" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTaxMaster" runat="server" Text='<%#Eval("ExID") %>'></asp:Label>
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
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvTxMstr]").dataTable({
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

        function SearchFnclrYr() {
            var value1 = $("[id*='ddlFnclYr'] :selected").text();
            oTable.fnFilter(value1, 1);
            if ($('[id$=gvTxMstr] >tbody >tr >td').length > 1) {
                ErrorMessage('Tax for this Financial Year Already Exist');
                $('[id$=ddlFnclYr]').val('0');
            }
            oTable.fnFilter('', 1);
        }

        function Myvalidations() {
            if (($('[id$=ddlFnclYr]').val()).trim() == '0') {
                ErrorMessage('Financial Year is Required.');
                $('[id$=ddlFnclYr]').focus();
                return false;
            }
            else if (($('[id$=txtExdt]').val()).trim() == '') {
                ErrorMessage('Excise Duty is Required.');
                $('[id$=txtExdt]').focus();
                return false;
            }
            else if (($('[id$=txtSltx]').val()).trim() == '') {

                ErrorMessage('Sales Tax is Required.');
                $('[id$=txtSltx]').focus();

                return false;
            }
            else if (($('[id$=txtDCRt]').val()).trim() == '') {
                ErrorMessage('Doller Conversion Rate is Required.');
                $('[id$=txtDCRt]').focus();
                return false;
            }
            else if (($('[id$=txtFrmDt]').val()).trim() == '') {
                ErrorMessage('From/Start Date is Required.');
                $('[id$=txtFrmDt]').focus();
                return false;
            }
            else if (($('[id$=txtToDt]').val()).trim() == '') {
                ErrorMessage('To/End Date is Required.');
                $('[id$=txtToDt]').focus();
                return false;
            }
            else {
                return true;
            }
        }

        var dateToday = new Date();
        $('[id$=txtFrmDt]').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true,
            maxDate: dateToday
        });

        function changedate(FrmDt) {
            try {

                var oneYear = ($('[id$=txtFrmDt]').val()).split('-');
                var CurrentDate = oneYear[1] + '-' + oneYear[0] + '-' + oneYear[2];
                var oneYearDt = new Date(CurrentDate.replace(/-/g, "/"));
                var CrntDt = new Date(CurrentDate.replace(/-/g, "/"));
                oneYearDt.setDate(oneYearDt.getDate() - 1);
                oneYearDt.setYear(oneYearDt.getFullYear() + 1);
                $('[id$=txtToDt]').datepicker({
                    dateFormat: 'dd-mm-yy',
                    changeMonth: true,
                    changeYear: true,
                    minDate: CrntDt,
                    maxDate: oneYearDt
                });
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }



        function CheckNmbrs(txtbox) {
            var regex = /^\d+(?:\.\d{0,2})$/;
            if (txtbox.value == '') {
                ErrorMessage('Excise Duty is Required.');
                $('[id$=txtExdt]').focus();
                return false;
            }
            else {
                if (!regex.test(txtbox.value)) {
                    ErrorMessage('Invalid Number Format !Enter in 00.00 Formats.');
                    $('[id$=txtExdt]').focus();
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
        function AddYear() {
            var cd = new Date();
            //cd = Convert($('[id$=txtFrmDt]').val());
            //cd.setDate();
            var dat = cd.getDate(); var yer = cd.getFullYear() + 1; var month = cd.getMonth() + 1;
            if (dat >= 31) {
                dat = 1;
            }
            else dat = dat + 1;
            var tdt = month + '-' + dat + '-' + yer;
            $('[id$=txtToDt]').val(tdt);
            $('[id$=txtToDt]').focus();
        }
        
    </script>
</asp:Content>
