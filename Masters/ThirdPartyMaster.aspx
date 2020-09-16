<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="ThirdPartyMaster.aspx.cs" Inherits="VOMS_ERP.Masters.ThirdPartyMaster"
     %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Third Party Master"
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
                        <td colspan="6">
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblOrg" class="bcLabel">Inspection Agency/Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtInspection" ValidationGroup="D" MaxLength="500"
                                            CssClass="bcAsptextbox" TabIndex="1" onkeypress="return isOrgName(event)" onchange="SearchCmpnyNm();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblBName" class="bcLabel">Contact Person<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtContactPerson" ValidationGroup="D" MaxLength="500"
                                            CssClass="bcAsptextbox" Height="17px" TabIndex="2" onkeyup="this.value=this.value.replace(/[^a-zA-Z ]/g,'');"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblTelPh" class="bcLabel">Contract Code<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtContactCode" ValidationGroup="D" CssClass="bcAsptextbox"
                                            TabIndex="3" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9/ -]/g,'');"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Is Contract Valid<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:RadioButton ID="rbtnYes" runat="server" Text="Yes" GroupName="r" TabIndex="4" />
                                        &nbsp;&nbsp;&nbsp;
                                        <asp:RadioButton ID="rbtnNo" runat="server" Text="No" Checked="true" GroupName="r"
                                            TabIndex="5" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Incharge Ship To<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlInchargeShipTo" CssClass="bcAspdropdown"
                                            TabIndex="6">
                                            <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblMobileNo" class="bcLabel">Street No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtStreet" CssClass="bcAsptextbox" MaxLength="1000"
                                            TabIndex="7" onkeyup="this.value=this.value.replace(/[^a-zA-Z0-9/ -]/g,'');"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Country<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCountry" CssClass="bcAspdropdown" TabIndex="8">
                                            <asp:ListItem Value="0" Text="--Select Country--"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown1" runat="server" Category="Country"
                                            TargetControlID="ddlCountry" PromptText="Select Country" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCountrydropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblpem" class="bcLabel">State<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlState" CssClass="bcAspdropdown" TabIndex="9">
                                            <asp:ListItem Value="0" Text="--Select State--"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown2" runat="server" Category="State" TargetControlID="ddlState"
                                            ParentControlID="ddlCountry" PromptText="Select State" LoadingText="Loading States..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindStatedropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblSEM" class="bcLabel">City<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlCity" CssClass="bcAspdropdown" TabIndex="10">
                                            <asp:ListItem Value="0" Text="--Select City--"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CascadingDropDown ID="CascadingDropDown3" runat="server" Category="City" TargetControlID="ddlCity"
                                            ParentControlID="ddlState" PromptText="Select City" LoadingText="Loading Cities..."
                                            ServicePath="cascadingdataservice.asmx" ServiceMethod="BindCityropdown">
                                        </asp:CascadingDropDown>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblOrg" class="bcLabel">Start Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtStartDate" ValidationGroup="D" CssClass="bcAsptextbox"
                                            TabIndex="11" onchange="changedate();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblOrg" class="bcLabel">End Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtEndDate" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onchange="ChangeDate2()" TabIndex="12"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Contract Valid Reminder (Weeks)<font color="red"
                                            size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:HiddenField ID="DateDiff" runat="server" Value="" />
                                        <asp:TextBox runat="server" ID="txtContractValidRemainder" MaxLength="2" CssClass="bcAsptextbox"
                                            onkeypress="return isNumberKey(event);" onchange="CheckValiedRemainder()" TabIndex="13"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        &nbsp;
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
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
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
                    <tr>
                        <td colspan="6">
                            <div>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:GridView runat="server" ID="gvThirdParty" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                EmptyDataText="No data to display...!" PagerStyle-CssClass="bcGridViewPagerStyle"
                                                PagerStyle-HorizontalAlign="Center" DataKeyNames="ID" CssClass="bcGridViewMain"
                                                HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                                Width="100%" OnPreRender="gvThirdParty_PreRender" OnRowCommand="gvThirdParty_RowCommand"
                                                OnRowDataBound="gvThirdParty_RowDataBound" OnRowDeleting="gvThirdParty_RowDeleting">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No.">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Inspection Agent/Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblorgName" runat="server" Text='<%#Eval("InspAgentname") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Contact Person">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblBussName" runat="server" Text='<%#Eval("ContactPerson") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Contract Valid">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblphonembl" runat="server" Text='<%# Eval("IsContractValied") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Incharge Ship">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblprieml" runat="server" Text='<%#Eval("InchargeShipTo") %>'></asp:Label>
                                                        </ItemTemplate>
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
        });

        function CheckValiedRemainder() {
            var discount = $('[id$=txtContractValidRemainder]').val();
            var valuee = $('[id$=DateDiff]').val();
            if (parseInt(discount) > parseInt(valuee)) {
                ErrorMessage('Contract Valid Remainder Should not be greater than ' + valuee + ' weeks.');
                $('[id$=txtContractValidRemainder]').val('');
                return false;
            }
        }
        var dateToday = new Date();
        $(document).ready(function () {
            $('[id$=txtStartDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtEndDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
                //maxDate: dateToday
            });
        });

        function changedate() {
            var strdate = $('[id$=txtStartDate]').val();
            var strdate1 = strdate.split('-');
            strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
            strdate = new Date(strdate.replace(/-/g, "/"));
            $('[id$=txtEndDate]').datepicker('option', {
                minDate: new Date(strdate)// + "+0M +420D"
                //maxDate: "+2M +10D"
            });
            //var month = strdate.getMonth() + 1;            
            //$('[id$=txtEndDate]').val(("0" + strdate.getDate()).slice(-2) + '-' + ("0" + month).slice(-2) + '-' + strdate.getFullYear());
        }

        function ChangeDate2() {
            var strdate = $('[id$=txtStartDate]').val();
            var strdate1 = strdate.split('-');
            strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
            strdate = new Date(strdate.replace(/-/g, "/"));

            var EndDate = $('[id$=txtEndDate]').val();
            var EndDate1 = EndDate.split('-');
            EndDate = (EndDate1[1] + '-' + EndDate1[0] + '-' + EndDate1[2]);
            EndDate = new Date(EndDate.replace(/-/g, "/"));

            var diff = parseInt((EndDate - strdate) / 604800000);
            $('[id$=txtContractValidRemainder]').val(diff);
            $('[id$=DateDiff]').val(diff);
        }
        
    </script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvThirdParty]").dataTable({
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
            oTable = $("[id$=gvThirdParty]").dataTable();
        });

        function SearchCmpnyNm() {
            var value1 = $('[id$=txtInspection]').val();
            oTable.fnFilter(value1, 1);
            if ($('[id$=gvThirdParty] >tbody >tr >td').length > 1) {
                ErrorMessage('Inspection Agency Name already Exists');
                $('[id$=txtInspection]').val('');
            }
            oTable.fnFilter('', 1);
        }

        function Myvalidations() {
            if (($('[id$=txtInspection]').val()).trim() == '') {
                ErrorMessage('Inspection Agency/Name is required.');
                $('[id$=txtInspection]').focus();
                return false;
            }
            else if (($('[id$=txtContactPerson]').val()).trim() == '') {
                ErrorMessage('Contact person is required.');
                $('[id$=txtContactPerson]').focus();
                return false;
            }
            else if (($('[id$=txtContactCode]').val()).trim() == '') {
                ErrorMessage('Contract code is required.');
                $('[id$=txtContactCode]').focus();
                return false;
            }
            else if (($('[id$=ddlInchargeShipTo]').val()).trim() == '0') {
                ErrorMessage('Incharge ship to is required.');
                $('[id$=ddlInchargeShipTo]').focus();
                return false;
            }
            else if (($('[id$=txtStreet]').val()).trim() == '') {
                ErrorMessage('Street no is required.');
                $('[id$=txtStreet]').focus();
                return false;
            }
            else if (($('[id$=ddlCountry]').val()).trim() == '') {
                ErrorMessage('Country is required.');
                $('[id$=ddlCountry]').focus();
                return false;
            }
            else if (($('[id$=ddlState]').val()).trim() == '') {
                ErrorMessage('State is required.');
                $('[id$=ddlState]').focus();
                return false;
            }
            else if (($('[id$=ddlCity]').val()).trim() == '') {
                ErrorMessage('City is required.');
                $('[id$=ddlCity]').focus();
                return false;
            }
            else if (($('[id$=txtStartDate]').val()).trim() == '') {
                ErrorMessage('Start Date is required.');
                $('[id$=txtStartDate]').focus();
                return false;
            }
            else if (($('[id$=txtEndDate]').val()).trim() == '') {
                ErrorMessage('End Date is required.');
                $('[id$=txtEndDate]').focus();
                return false;
            }
            else if (($('[id$=txtContractValidRemainder]').val()).trim() == '') {
                ErrorMessage('Contract Validate Remainder is required.');
                $('[id$=txtContractValidRemainder]').focus();
                return false;
            }
            else {
                return true;
            }
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) //charCode != 46 &&
                return false;
            return true;
        }
        function noSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 44 && charCode != 45 && charCode != 46 && charCode != 47 && charCode != 58 && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        function isAlphaKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 65 && charCode < 97 || charCode > 90 && charCode > 122)) //charCode != 46 &&
                return false;
            return true;
        }
        function isOrgName(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 46 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }
        // E-Mail Validation
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
