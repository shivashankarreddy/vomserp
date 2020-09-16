<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="TrackerExportReport.aspx.cs" Inherits="VOMS_ERP.Enquiries.TrackerExportReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" align="center">
                <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                    <tbody>
                        <tr valign="middle">
                            <td align="center" valign="middle" class="bcTdButton">
                                <div id="Div4" class="bcButtonDiv">
                                    <asp:Button runat="server" ID="btnFeTracker" Text="FE Report" OnClick="btnFeTracker_Click"
                                         />
                                    &nbsp;</div>
                            </td>
                            <%--<td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div1" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnFQClosureTracker" Text="Save" OnClick="btnSave_Click" />
                                                            </div>
                                                        </td>--%>
                            <td align="center" valign="middle" class="bcTdButton">
                                <div id="Div2" class="bcButtonDiv">
                                    <asp:Button runat="server" ID="btnFPOTracker" Text="FPO Local Delivery Report" OnClick="btnFPOTracker_Click"
                                         />
                                    &nbsp;
                                </div>
                            </td>
                            <td align="center" valign="middle" class="bcTdButton">
                                <div id="Div3" class="bcButtonDiv">
                                    <asp:Button runat="server" ID="btnFPOExportTracker" Text="FPO Export Delivery Report" OnClick="btnFPOExportTracker_Click"
                                         />
                                    &nbsp;
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery.ui.theme.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery-ui.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/jquery.dataTables_themeroller.css" rel="stylesheet"
        type="text/css" />
    <script src="../JScript/Scripts/js/jquery.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/ZeroClipboard.js" type="text/javascript"></script>
    <script src="../JScript/media/js/TableTools.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">
//        function FeClick() {
//            $('p').text("Form submiting.....");
//            $('[id$=btnFPOTracker]').attr('disabled', 'disabled');
//            $('[id$=btnFPOExportTracker]').attr('disabled', 'disabled');
//            //            $('[id$=ddlcustmr]').removeAttr('disabled', 'disabled');

//        }

//        function FPOClick() {
//            $('[id$=btnFeTracker]').attr("disabled", 'disabled');
//            $('[id$=btnFPOExportTracker]').attr('disabled', 'disabled');
//        }
//        function FPOExportClick() {
//            $('[id$=btnFPOTracker]').attr('disabled', 'disabled');
//            $('[id$=btnFeTracker]').attr("disabled", 'disabled');
//        }

        function EnableButtons() {
            $('[id$=btnFPOTracker]').removeAttr('disabled', 'disabled');
            $('[id$=btnFeTracker]').removeAttr("disabled", 'disabled');
            $('[id$=btnFPOExportTracker]').removeAttr('disabled', 'disabled');
        }
    </script>
</asp:Content>
