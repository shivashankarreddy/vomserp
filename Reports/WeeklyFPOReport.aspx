<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="WeeklyFPOReport.aspx.cs" Inherits="VOMS_ERP.Reports.WeeklyFPOReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Style="font-size: 15.5px;
                                            font-weight: bold" Text="Weekly FPO Report" CssClass="bcTdTitleLabel"></asp:Label><div
                                                id="divMyMessage" runat="server" align="center" class="formError1" style="margin-right: 5%;" />
                                    </td>
                                    <td align="right">
                                        <div runat="server" id="dvexport">
                                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                                class="item_top_icons" title="Export Excel" OnClick="btnExcelExpt_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 5px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Customer Name :</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlcustomer" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">From Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFrmDt" CssClass="bcAsptextbox" MaxLength="12"
                                            onchange="changedate(this.id);" Width="80px"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">To Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtToDt" CssClass="bcAsptextbox" MaxLength="12" Width="80px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="8">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div4" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="lbtnSearch" Text="Search" OnClick="lbtnSearch_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div5" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="lbtnClear" Text="Clear" OnClick="lbtnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div6" class="bcButtonDiv">
                                                            <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>&nbsp;
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
                        <td colspan="6">
                            <div class="aligntable" id="aligntbl">
                                <div style="min-height: 200px; max-height: 300px; overflow: auto;">
                                    <asp:GridView runat="server" ID="GvWklyRpt" Width="100%" RowStyle-CssClass="bcGridViewRowStyle"
                                        AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                        PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                        DataKeyNames="ForeignPurchaseOrderId" EmptyDataText="No Records are Exists" CssClass="bcGridViewMain"
                                        HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                        OnRowDataBound="GvWklyRpt_RowDataBound" OnRowCommand="GvWklyRpt_RowCommand">
                                        <Columns>
                                            <asp:BoundField DataField="Frn Purchase Order Number" HeaderText="FP Order No." />
                                            <%--<asp:BoundField DataField="FPO Date" HeaderText="Date" />--%>
                                            <asp:BoundField DataField="Recv Date" HeaderText="Recv Date" />
                                            <asp:BoundField DataField="FPO DueDate" HeaderText="Due Date" />
                                            <asp:BoundField DataField="FPO Subject" HeaderText="FPO Subject" />
                                            <asp:BoundField DataField="Total FPO Amt" HeaderText="FPO Value($)" />
                                            <asp:BoundField DataField="Lcl Purchase Order Number" HeaderText="LP Order No." />
                                            <asp:BoundField DataField="Lcl Purchase Order Date" HeaderText="LP Order Date" />
                                            <asp:BoundField DataField="LPO Due Date" HeaderText="LP Order Due Date" />
                                            <%--<asp:BoundField DataField="Instruction" HeaderText="Instruction" />--%>
                                            <asp:BoundField DataField="Supplier Name" HeaderText="Supplier Name" />
                                            <%--<asp:BoundField DataField="Total Amt" HeaderText="LPO Amount(Rs)" />--%>
                                            <asp:BoundField DataField="StatusDes" HeaderText="Current System Status" />
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                                            <asp:TemplateField HeaderText="Privious Status">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtPreviousStat" runat="server" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9/ -]/g,'');"
                                                        Text='<%# Eval("PreviousSatus") %>'></asp:TextBox>
                                                    <asp:HiddenField ID="hfPrevDt" runat="server" Value='<%# Eval("PreviousDate") %>' />
                                                    <asp:HiddenField ID="hfFPOID" runat="server" Value='<%# Eval("ForeignPurchaseOrderId") %>' />
                                                    <asp:Label ID="lblLPOID" runat="server" Visible="false" Text='<%# Eval("LocalPurchaseOrderId") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderTemplate>
                                                    Previous Status
                                                    <br />
                                                    <asp:TextBox runat="server" ID="txtPreviousDt" CssClass="DatePicker" OnTextChanged="txtPreviousDt_TextChanged"
                                                        AutoPostBack="true"></asp:TextBox>
                                                </HeaderTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Current Status">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtCurentStat" runat="server" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9/ -]/g,'');"
                                                        Text='<%# Eval("Status") %>'></asp:TextBox>
                                                    <asp:HiddenField ID="hfcurrentDt" runat="server" Value='<%# Eval("CurrentDate") %>' />
                                                </ItemTemplate>
                                                <HeaderTemplate>
                                                    Current Status
                                                    <br />
                                                    <asp:TextBox runat="server" ID="txtCurrentDt" CssClass="DatePicker1" OnTextChanged="txtCurrentDt_TextChanged"
                                                        AutoPostBack="true"></asp:TextBox>
                                                </HeaderTemplate>
                                            </asp:TemplateField>
                                            <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Save.png" CommandName="Modify"
                                                Text="Save" ShowHeader="true" HeaderText="Action" HeaderStyle-Width="20px" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="6">
                            <table width="5%">
                                <tr>
                                    <td class="bcTdButton">
                                        <div class="bcButtonDiv">
                                            <asp:LinkButton ID="btnsave" runat="server" Text="SAVE" OnClick="btnsave_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="6">
                            <asp:Label ID="lblLegend" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <%--<tr>
                        <td colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div style="min-height: 200px; max-height: 500px; overflow: auto;">
                                            <asp:Repeater ID="GvWklyRpt" runat="server">
                                                <HeaderTemplate>
                                                    <table id="tblCountryList" cellpadding="0" cellspacing="0" border="0" class="display">
                                                        <thead>
                                                            <tr>
                                                                <th width="30px">
                                                                    S.No
                                                                </th>
                                                                <th>
                                                                    Frn Purchase Order Number
                                                                </th>
                                                                <th>
                                                                    FPO Date
                                                                </th>
                                                                <th>
                                                                    Recv Date
                                                                </th>
                                                                <th>
                                                                    FPO DueDate
                                                                </th>
                                                                <th>
                                                                    Lcl Purchase Order Number
                                                                </th>
                                                                <th>
                                                                    Lcl Purchase Order Date
                                                                </th>
                                                                <th>
                                                                    Instruction
                                                                </th>
                                                                <th>
                                                                    Supplier Name
                                                                </th>
                                                                <th>
                                                                    Total Amt
                                                                </th>
                                                                <th>
                                                                    Privious Status
                                                                </th>
                                                                <th>
                                                                    Current Status
                                                                </th>
                                                                <th>
                                                                    Remarks
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody class="tbody">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr id="<%# Eval("ForeignPurchaseOrderId") %>">
                                                        <td>
                                                            <%# Container.ItemIndex + 1 %>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Frn Purchase Order Number")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("FPO Date")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Recv Date")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("FPO DueDate")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Lcl Purchase Order Number")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Lcl Purchase Order Date")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Instruction")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Supplier Name")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Total Amt")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("PreviousSatus")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Status")%>
                                                        </td>
                                                        <td>
                                                            <%# Eval("Remarks")%>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </tbody> </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </td>
                                </tr>                                
                            </table>
                        </td>
                    </tr>--%>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/jquery.jeditable.js" type="text/javascript"></script>
    <script src="../JScript/jquery.dataTables.editable.js" type="text/javascript"></script>
    <%--<script type="text/javascript">

        $(document).ready(function () {
            var data = "";
            data = $('[id$=hfData]').val();
            $('#tblCountryList').dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,

                //--- Dynamic Language---------
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page <b>DOUBLE CLICK ON THE RECORD TO EDIT AND CLICK ENTER TO SAVE</b>",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search all columns:"
                }
            }).makeEditable({
                sUpdateURL: "WeeklyFPOReportHandler.ashx",
                "aoColumns": [null, null, null, null, null, null, null, null,
                null, null, {}, {},
                {
                    indicator: 'Saving...',
                    tooltip: 'Click to Edit',
                    loadtext: 'loading...',
                    type: 'textarea',
                    onblur: 'submit'
                }]
            });
        });

    </script>--%>
    <script type="text/javascript">


        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 84 + "px");
        });


        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtFrmDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtToDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('.DatePicker').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
            $('.DatePicker1').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        });
        

    </script>
</asp:Content>
