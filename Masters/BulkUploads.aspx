<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="BulkUploads.aspx.cs" Inherits="VOMS_ERP.Masters.BulkUploads" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Bulk Uploads" CssClass="bcTdTitleLabel"> </asp:Label><div
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
                                    <td class="bcTdNewTable">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <span class="bcLabel">Screen Name<font color="red" size="2"><b>*</b></font>: </span>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlScreenname" runat="server" CssClass="bcAsptextbox">
                                                        <asp:ListItem Text="--Select Screen name--" Value=""></asp:ListItem>
                                                        <asp:ListItem Text="Spare Parts" Value="SpareParts"></asp:ListItem>
                                                        <asp:ListItem Text="Item Category" Value="ItemCategory"></asp:ListItem>
                                                        <%--<asp:ListItem Text="Item Sub Category" Value="ItemSubCategory"></asp:ListItem>--%>
                                                        <asp:ListItem Text="Item Sub Sub Category" Value="ItemSubSubCategory"></asp:ListItem>
                                                        <asp:ListItem Text="Item Master" Value="ItemMaster"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td align="left">
                                                    <asp:FileUpload ID="FileUpload1" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div1" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnUpload" Text="Upload" OnClientClick="javascript:Myvalidations()"
                                                                OnClick="btnUpload_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div4" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnUpdate" Text="Update" Visible="false" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div2" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnClear" Text="Clear" 
                                                                onclick="btnClear_Click" />
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
                </table>
            </td>
        </tr>
    </table>    
    
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        function Myvalidations() {
            if ($('[id$=ddlScreenname]').val() == "") {
                $('[id$=ddlScreenname]').focus();
                ErrorMessage('Screen Name is required.');
                return false;
            }
            else if ($('[id$=FileUpload1]').val() == '') {
                ErrorMessage('Select Excel File to Upload.');
                $('[id$=FileUpload1]').trigger("click");
                return false;
            }
        }
        
    </script>
</asp:Content>
