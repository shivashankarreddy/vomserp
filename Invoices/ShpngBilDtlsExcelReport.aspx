<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ShpngBilDtlsExcelReport.aspx.cs" Inherits="VOMS_ERP.Invoices.ShpngBilDtlsExcelReport" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="ESD" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
             <div style="width: 100%">
                <table style="width: 100%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Export Shipment Details Report"
                                                CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                    class="formError1" />
                                        </td>
                                        <td colspan="2" style="text-align: right;">
                                        </td>
                                    </tr>
                                </table>
                        </td>
                    </tr>
                    <tr>
                    <td>
                            <asp:RadioButton runat="server" ID="InvNumber" checked=true Text="Commercial Invoice No" GroupName="RdbTest"
                                onclick="ShowHide('1');" />
                            <asp:RadioButton runat="server" ID="InvcDate" Text="As On Date" GroupName="RdbTest" onclick="ShowHide('2');" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable" id="AsOnInvNo">
                            <div style="width: 100%">
                                <table>
                                    <tr>
                                        <td class="bcTdnormal">
                                            <span class="bcLabel">Commercial Invoice No :</span>
                                        </td>
                                        <td class="bcTdnormal">
                                            <asp:DropDownList runat="server" ID="ddlCommInvNo" CssClass="bcAspdropdown" AutoPostBack="true">
                                            </asp:DropDownList>
                                            <%----OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged" --%>
                                        </td>
                                        <td class="bcTdnormal">
                                            <span id="lblEnqNo" style="color: red; visibility: hidden" class="bcLabel">Export Excel
                                                Shipment Details <font color="red" size="2"><b>*</b></font>:</span>
                                        </td>
                                        <td class="bcTdnormal">
                                        </td>
                                        <td colspan="2" style="width: 30%">
                                            <div runat="server" id="dvexport">
                                                <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                                    class="item_top_icons" title="Export Excel" OnClientClick="javascript:validations()" OnClick="btnExcelExpt_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable" id="AsOnDate">
                            <div style="width: 100%">
                                <table>
                                    <tr>
                                        <td class="bcTdnormal">
                                            <span class="bcLabel">From Date :</span>
                                        </td>
                                        <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="fromDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td>
                                        <td class="bcTdnormal">
                                            <span class="bcLabel">To Date :</span>
                                        </td>
                                        <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="toDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td>
                                        <td colspan="2" style="width: 30%">
                                            <div runat="server" id="Div1">
                                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../images/EXCEL.png"
                                                    class="item_top_icons" title="Export Excel" OnClientClick="javascript:validations()" OnClick="btn_Date_ExcelExpt_Click"  />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
                </div>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=fromDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=toDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

    </script>
    <script type="text/javascript">
        $("#AsOnInvNo").show();
        $("#AsOnDate").hide();
        function ShowHide(val) {
            debugger;
            if (val == 1) {
                $("#AsOnInvNo").show();
                $("#AsOnDate").hide();
                document.getElementById('<%=InvNumber.ClientID%>').checked = true;
                $('[id$=fromDate]').val('');
                $('[id$=toDate]').val('');
            }
            else if (val == 2) {
                $("#AsOnInvNo").hide();
                $("#AsOnDate").show();
                document.getElementById('<%=InvcDate.ClientID%>').checked = true;
                $('[id$=ddlCommInvNo]').val('00000000-0000-0000-0000-000000000000');
            } else {
                $("#AsOnInvNo").hide();
                $("#AsOnDate").show();
                document.getElementById('<%=InvNumber.ClientID%>').checked = true;
                document.getElementById('<%=InvcDate.ClientID%>').checked = false;

            }
        };


        function InvUsd() {
            try {
                if (($('[id$=ddlCommInvNo]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Shipment Proforma Invoice Number is Required.');
                    $('[id$=ddlCommInvNo]').focus();
                    return false;
                }
                return true;
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }
        function Myvalidations() {
            debugger;
            var res = document.getElementById('<%=InvNumber.ClientID%>').checked;  //$('[id$=InvNumber]').checked;
            var res2 = document.getElementById('<%=InvcDate.ClientID%>').checked;
            // var res2 = $('[id$=InvcDate]').val();
            //if (($('[id$=InvNumber]').val()).trim() == "InvNumber") {
            if (res == true) {
                if (($('[id$=ddlCommInvNo]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Commercial Invoice Number is Required.');
                    $('[id$=ddlCommInvNo]').focus();
                    return false;
                }
            }
            else {
                if (($('[id$=InvcDate]').val()).trim() == "InvcDate") {
                    if ($('[id$=fromDate]').val() == '') {
                        ErrorMessage('Commercial Invoice Date is Required.');
                        $('[id$=fromDate]').focus();
                        return false;
                    }
                }

            }
        }
        </script>
</asp:Content>
