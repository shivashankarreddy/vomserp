<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="FinancialYear.aspx.cs" Inherits="VOMS_ERP.Masters.FinancialYear"
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Financial Year Master"
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
                                                            <asp:TextBox runat="server" ID="txtFnclYr" onchange="SearchFnclrYr()" CssClass="bcAsptextbox"
                                                                onkeypress="return isNumberKey(event)" MaxLength="10"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span2" class="bcLabelright">From<font color="red" size="2"><b>*</b></font>:
                                                            </span>
                                                        </div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtFrmDt" MaxLength="12" onchange="changedate(this.id);" CssClass="bcAsptextbox"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span3" class="bcLabelright">To<font color="red" size="2"><b>*</b></font>:</span></div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:TextBox runat="server" ID="txtToDt" MaxLength="12" CssClass="bcAsptextbox"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align: center; width: 98%;">
                                                        <div style="text-align: right; width: 49%;">
                                                            <span id="Span5" class="bcLabelright">Is Current Year:</span></div>
                                                        <div style="text-align: left; width: 50%;">
                                                            <asp:CheckBox runat="server" ID="chkbiscrnt" />
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:HiddenField runat="server" ID="hdfdFnclyrID" />
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
                                        <asp:GridView runat="server" ID="gvfnclyr" AutoGenerateColumns="False" Width="100%"
                                            RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            OnRowCommand="gvfnclyr_RowCommand" OnRowDataBound="gvfnclyr_RowDataBound" OnPreRender="gvfnclyr_PreRender">
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
                                                <asp:TemplateField HeaderText="Is Current Year">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIscrntyr" runat="server" Text='<%#Eval("CurrentYear") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="10px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Is Current Year" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFnclYr" runat="server" Text='<%#Eval("ID") %>'></asp:Label>
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
            $("[id$=gvfnclyr]").dataTable({
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
            oTable = $("[id$=gvfnclyr]").dataTable();
        });

        function SearchFnclrYr() {
            var value1 = $('[id$=txtFnclYr]').val();
            oTable.fnFilter(value1, 1);
            if ($('[id$=gvfnclyr] >tbody >tr >td').length > 1) {
                ErrorMessage('Financial Year Already Exist');
                $('[id$=txtFnclYr]').val('');
            }
            oTable.fnFilter('', 1);
        }

        function Myvalidations() {

            if (($('[id$=txtFnclYr]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Financial Year is Required.</span>');
                $('[id$=txtFnclYr]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtFrmDt]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">From/Start Date is Required.</span>');
                $('[id$=txtFrmDt]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtToDt]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">To/End Date is Required.</span>');
                $('[id$=txtToDt]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
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
        
//        $('[id$=txtToDt]').datepicker({
//            dateFormat: 'dd-mm-yy',
//            changeMonth: true,
//            changeYear: true,
//            maxDate: "+1Y"
//        });

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
