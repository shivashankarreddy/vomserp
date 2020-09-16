<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master" CodeBehind="ShippingDocumentsCustomer.aspx.cs" Inherits="VOMS_ERP.Invoices.ShippingDocumentsCustomer" %>

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
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Shipping Documents Sent to Customer"
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
                        <td colspan="6" class="bcTdNewTable" id="Td1">
                            <div style="width: 100%">
                                <table>
                                    <tr>
                                    <td class="bcTdnormal">
                                            <span class="bcLabel">Customer :</span>
                                        </td>
                                        <td class="bcTdnormal">
                                        <asp:ListBox ID="ListBoxCustomer" runat="server" SelectionMode="Multiple" AutoPostBack="true"
                                                    CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ListBoxCustomer_SelectedIndexChanged" >
                                                </asp:ListBox>
                                            <%--<asp:DropDownList runat="server" ID="ddlAirLading" CssClass="bcAspdropdown" AutoPostBack="true">
                                            </asp:DropDownList>--%>
                                            <%----OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged" --%>
                                        </td>
                                        <td class="bcTdnormal">
                                            <span class="bcLabel">Airway bill No/Bill of Lading No :</span>
                                        </td>
                                        <td class="bcTdnormal">
                                        <asp:ListBox ID="ListBoxAirBillNo" runat="server" SelectionMode="Multiple" AutoPostBack="true"
                                                    CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ListBoxAirBill_SelectedIndexChanged"  >
                                                </asp:ListBox>
                                            <%--<asp:DropDownList runat="server" ID="ddlAirLading" CssClass="bcAspdropdown" AutoPostBack="true">
                                            </asp:DropDownList>--%>
                                            <%----OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged" --%>
                                        </td>
                                        <td class="bcTdnormal">
                                            <span class="bcLabel">FPO :</span>
                                        </td>
                                        <td class="bcTdnormal">
                                        <asp:ListBox ID="ListBoxFpo" runat="server" SelectionMode="Multiple" AutoPostBack="false"
                                                    CssClass="bcAspMultiSelectListBox" >
                                                </asp:ListBox>
                                            <%--<asp:DropDownList runat="server" ID="ddlAirLading" CssClass="bcAspdropdown" AutoPostBack="true">
                                            </asp:DropDownList>--%>
                                            <%----OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged" --%>
                                        </td>
                                        <%--<td class="bcTdnormal">
                                            <span id="Span1" style="color: red; visibility: hidden" class="bcLabel">Export Excel
                                                Shipment Details <font color="red" size="2"><b>*</b></font>:</span>
                                        </td>
                                        <td class="bcTdnormal">
                                        </td>
                                        <td colspan="2" style="width: 30%">
                                            <div runat="server" id="dvexport">
                                                <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                                    class="item_top_icons" title="Export Excel" OnClientClick="javascript:validations()" OnClick="btnExcelExpt_Click" />
                                            </div>
                                        </td>--%>
                                    </tr>
                                    
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                    <td>
                            <asp:RadioButton runat="server" ID="InvNumber" Text="Is Send By Courier" GroupName="RdbTest"
                                onclick="ShowHide('1');" />
                                 <asp:HiddenField ID="HFInvNo" runat="server" Value="" />
                            <asp:RadioButton runat="server" ID="InvcDate"   Text="Is Send By Person" GroupName="RdbTest" onclick="ShowHide('2');"/>
                            <asp:HiddenField ID="HFInvNDate" runat="server" Value="" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="bcTdNewTable" id="Td2">
                            <div style="width: 100%">
                                <table>
                                 <tr>
                                      <td class="bcTdnormal">
                                            <span class="bcLabel">Courier Name :</span>
                                        </td>
                                        <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="CourierName" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td>  
                                    </tr>
                                    <tr>
                                      <td class="bcTdnormal">
                                            <span class="bcLabel">Courier No :</span>
                                        </td>
                                        <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="CourierNo" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td>  
                                    </tr>
                                    <tr>
                                    <td class="bcTdnormal">
                                            <span class="bcLabel">Courier Send Date :</span>
                                        </td>
                                       <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="RecDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td> 
                                    </tr>
                                    <tr>
                                    <td class="bcTdnormal">
                                            <span class="bcLabel"> Document Sent By :</span>
                                        </td>
                                       <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="DocumentSent" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td> 
                                    </tr>
                                    <tr>
                                    <td class="bcTdnormal">
                                            <span class="bcLabel"> Received Person :</span>
                                        </td>
                                       <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="ReceivedPerson" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td> 
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    
                    <tr>
                        <td colspan="6" class="bcTdNewTable" id="Td3">
                            <div style="width: 100%">
                                <table>
                                    <tr>
                                      <td class="bcTdnormal">
                                            <span class="bcLabel">Document carrier:</span>
                                        </td>
                                        <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="Documentcarrier" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td>  
                                    </tr>
                                    <tr>
                                    <td class="bcTdnormal">
                                            <span class="bcLabel">Document carrier Contact No :</span>
                                        </td>
                                       <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="ContactNo" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td> 
                                    </tr>
                                    <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Unit  <font color="red" size="2"><b>
                                            *</b></font> :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCpnyLst" CssClass="bcAspdropdown" 
                                            >
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hfIsEdit" runat="server" Value="False" />
                                    </td></tr>
                                    <%--<tr>
                                    <td class="bcTdnormal">
                                            <span class="bcLabel"> Received Person :</span>
                                        </td>
                                       <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="TextBox6" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td> 
                                    </tr>
                                    <tr>
                                    <td class="bcTdnormal">
                                            <span class="bcLabel"> Received Date :</span>
                                        </td>
                                       <td class="bcTdnormal" style="width: 185px">
                                            <asp:TextBox ID="ReceivedDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                        </td> 
                                    </tr>--%>
                                </table>
                            </div>
                        </td>
                    </tr>
                    
                    </table>
               </div>
                </td>
                </tr>
    
       <tr>
                        <td align="center" colspan="6">
                                <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                    <tbody>
                                        <tr valign="middle">
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div1" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click"  />
                                                </div>
                                            </td>
                                            <td align="center" valign="middle" class="bcTdButton">
                                                <div id="Div2" class="bcButtonDiv">
                                                    <asp:LinkButton runat="server" ID="btnclear" OnClientClick="Javascript:clearAll()" Text="Clear" OnClick="btnclear_Click" />
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
            $('[id$=RecDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=ReceivedDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

    </script>
    <script type="text/javascript">
        var urlParams = new URLSearchParams(window.location.search);
        if (!urlParams.has('ID')) {
            $("#Td2").show();
            $("#Td3").hide();
            $('[id$=InvNumber]').prop("checked", true);
        }
        if (document.getElementById("<%= HFInvNo.ClientID %>").value == "1") {
            $("#Td2").show();
            $("#Td3").hide();
            document.getElementById('<%=InvNumber.ClientID%>').checked = true;
        }
        else if (document.getElementById("<%= HFInvNDate.ClientID %>").value == "1") {
            $("#Td2").hide();
            $("#Td3").show();
            document.getElementById('<%=InvcDate.ClientID%>').checked = true;
        }
        console.log(document.getElementById("<%= HFInvNDate.ClientID %>").value == "1");
        function ShowHide(val) {
            debugger;
            
            if (val == 1) {
                $("#Td2").show();
                $("#Td3").hide();
                document.getElementById('<%=InvNumber.ClientID%>').checked = true;
             
            }
            else if (val == 2) {
                $("#Td2").hide();
                $("#Td3").show();
                document.getElementById('<%=InvcDate.ClientID%>').checked = true;
            
            }
            else  {
                $("#Td2").hide();
                $("#Td3").show();
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
            if ($('[id$=ListBoxAirBillNo]').val() == null) {
               
                ErrorMessage('AirBillNo is Required.');
                return false;
            }
            else if (res == true) {
                if (($('[id$=CourierName]').val()).trim() == "") {
                    ErrorMessage('Courier Name is Required.');
                    $('[id$=CourierName]').focus();
                    return false;
                }
            }
            else {
                if (($('[id$=Documentcarrier]').val()).trim() == "InvcDate") {
                    if ($('[id$=Documentcarrier]').val() == '') {
                        ErrorMessage('Document carrier is Required.');
                        $('[id$=Documentcarrier]').focus();
                        return false;
                    }
                }

            }
        }
        </script>
    </asp:Content>



