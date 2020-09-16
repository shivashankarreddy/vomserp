<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true" CodeBehind="CHome.aspx.cs" Inherits="VOMS_ERP.Masters.CHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table class="MainTable" align="center">
        <tr class="bcTRTitleRow">
            <td class="bcTdTitleLeft" align="left">
                <table width="100%">
                    <tr>
                        <td>
                            &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Dashboard" CssClass="bcTdTitleLabel"></asp:Label><div
                                id="divMyMessage" runat="server" align="center" class="formError1" />
                        </td>
                        <td colspan="2" style="text-align: right;">
                            <span id="username" class="bcLabelright">Welcome
                                <asp:Label runat="server" ID="lblLoginName"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="bcTdnormal">
                <div id="full_dashbord" class="full_dashbord" style="margin-top: 10px; margin-bottom: 20px;">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <th style="text-align: left; padding: 0px 20px 0px; border-bottom: solid 1px #A5C5DB;
                                            background-color: #EAEFF5;" class="dash_heading">
                                            Foreign Enquiries / Quotations / FPO&#39;s
                                        </th>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Enquiries/FEStatus.aspx?Mode=tdt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Enquiries Received Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblFrnEnqTd" runat="server"></asp:Label>
                                                </span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Quotations/FQStatus.aspx?Mode=tdt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Quotations Submitted Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblFQTd" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/FPOStatus.aspx?Mode=tdt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of FPO&#39;s Received Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblFPORcvdToday" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2" style="color: red;">
                                            <a href="../Enquiries/FEStatusNew.aspx"><span class="text_left" style="float: left;
                                                color: red; width: 79%;">Local Enquiries Yet to Float </span><span style="text-align: center;
                                                    float: right; width: 19%; color: red;">
                                                    <asp:Label ID="lblFEnqDueLEnq" runat="server" ForeColor="Red"></asp:Label>
                                                </span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;" colspan="2">
                                            <a href="../Enquiries/FEStatusNew.aspx"><span class="text_left" style="float: left;
                                                color: red; width: 79%;">Foreign Quotations Yet to Submit </span><span style="text-align: center;
                                                    float: right; width: 19%; color: red;">
                                                    <asp:Label ID="lblFEnqDueFQ" runat="server" ForeColor="Red"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;" colspan="2">
                                            <a href=""><span class="text_left" style="float: left; color: red; width: 79%;">Invoices
                                                Submitted Today </span><span style="text-align: center; float: right; width: 19%;
                                                    color: red;">
                                                    <asp:Label runat="server" ID="lblInvs"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Enquiries/FEStatusNew.aspx?Mode=tldt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Enquiries Received to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label runat="server" ID="lblFrnEnqTillDt"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Quotations/FQStatus.aspx?Mode=opd"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Quotations Submitted to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblFQTillDt" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/FPOStatus.aspx?Mode=tldt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of FPO&#39;s Received to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblFPORcvdTillDt" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;" colspan="2">
                                            <a href=""><span class="text_left" style="float: left; color: red; width: 79%;">No.
                                                of Enquiries Cancelled </span><span style="text-align: center; float: right; width: 19%;
                                                    color: red;">
                                                    <asp:Label ID="lblFECancel" runat="server" ForeColor="Red"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;" colspan="2">
                                            <a href="#"><span class="text_left" style="float: left; color: red; width: 79%;">FPO&#39;s
                                                Dispatch Over Due </span><span style="text-align: center; float: right; width: 19%;
                                                    color: red;">
                                                    <asp:Label ID="lblFDespDue" runat="server" Text="0" ForeColor="Red"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: Red" colspan="2">
                                            <a href="" style="color: Red;"><span class="text_left" style="float: left; width: 79%;">
                                                No. of Invoices Sent to Date </span><span style="text-align: center; float: right;
                                                    width: 19%; color: red;">
                                                    <asp:Label ID="lblInvsTillDt" runat="server" Text="0" ForeColor="Red"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="full_dashbord0" class="full_dashbord" style="margin-top: 10px; margin-bottom: 20px;">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <th style="text-align: left; padding: 0px 20px 0px;" class="dash_heading">
                                            Local Enquiries / Quotations / LPO&#39;s
                                        </th>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Enquiries/LEStatus.aspx?Mode=tdt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Enquiries Floated Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblLETd" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Quotations/LQStatus.aspx?Mode=tdt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Quotations Received Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblLQTod" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=tdt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of LPO&#39;s Released Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblLpoTd" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;" colspan="2">
                                            <a href="../Quotations/LQStatus.aspx"><span class="text_left" style="float: left;
                                                color: red; width: 79%;">LPO&#39;s Delivery Over Due </span><span style="text-align: center;
                                                    float: right; width: 19%; color: red;">
                                                    <asp:Label ID="lblLQTod1" runat="server" ForeColor="Red"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Enquiries/LEStatus.aspx?Mode=odt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Enquiries Floated to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblLETillDt" runat="server"></asp:Label></span> </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Quotations/LQStatus.aspx?Mode=tldt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of Quotations Received to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblLQTillDt" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=tldt"><span class="text_left" style="float: left;
                                                width: 79%;">No. of LPO&#39;s Released to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblLPOTillDt" runat="server"></asp:Label></span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;" colspan="2">
                                            <a href="../Enquiries/LEStatus.aspx"><span class="text_left" style="float: left;
                                                color: red; width: 79%;">Supplier Quotations Over Due </span><span style="text-align: center;
                                                    float: right; width: 19%; color: red;">
                                                    <asp:Label ID="lblLEnqDue" runat="server" ForeColor="Red"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="full_dashbord3" class="full_dashbord" style="margin-top: 10px; margin-bottom: 20px;">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <th style="text-align: left; padding: 0px 20px 0px;" class="dash_heading">
                                            Reminders
                                        </th>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <%--<tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx"><span class="text_left" style="float: left;
                                                width: 80%;">Drawing Approvals Remainder Time Today </span><span style="text-align: center;
                                                    float: right; width: 20%;">
                                                    <asp:Label ID="lblDrawingApprovalsToday" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=DPtdt"><span class="text_left" style="float: left;
                                                width: 79%;">Drawing Approvals to be Completed Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblDrwngAprlsTBCmpldToday" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/DrawingApprovalStatus.aspx?Mode=Dtdt"><span class="text_left"
                                                style="float: left; width: 79%;">Drawing Approvals Completed Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblDrwngAprlsCmpldToday" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=Ict"><span class="text_left" style="float: left;
                                                width: 79%;">Inspections to be Completed Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblInsptnTBCmpltdToday" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/InsptnReportStatus.aspx?Mode=DCtldt"><span class="text_left"
                                                style="float: left; width: 79%;">Inspections Completed Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblInsptnCmpltdToday" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=Etdt"><span class="text_left" style="float: left;
                                                width: 79%;">CT-1's (CEDEA) to be Completed Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblCEDEATBCmpltdToday" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Logistics/CTOneStatus.aspx?Mode=Etdt"><span class="text_left" style="float: left;
                                                width: 79%;">CT-1's(CEDEA) Completed Today </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblCEDEACmpltdToday" runat="server"></asp:Label></span> </a>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx"><span class="text_left" style="float: left;
                                                width: 80%;">Inspection Remainder Time Today </span><span style="text-align: center;
                                                    float: right; width: 20%;">
                                                    <asp:Label ID="Label2" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>--%>
                                    <%--<tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx"><span class="text_left" style="float: left;
                                                width: 80%;">Excise Duty Exemption Applicable Remainder Time Today </span><span style="text-align: center;
                                                    float: right; width: 20%;">
                                                    <asp:Label ID="lblExciseDutyExcemptionToday" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>--%>
                                </table>
                            </td>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <%--<tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx"><span class="text_left" style="float: left;
                                                width: 80%;">Drawing Approvals Remainder Time to Date </span><span style="text-align: center;
                                                    float: right; width: 20%;">
                                                    <asp:Label ID="lblDrawingApprovals" runat="server"></asp:Label></a>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/DrawingApprovalStatus.aspx?Mode=DCtldt"><span class="text_left"
                                                style="float: left; width: 79%;">Drawing Approvals Completed to Date </span><span
                                                    style="text-align: center; float: right; width: 19%;">
                                                    <asp:Label ID="lblDrawingApprovalsCmpld" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=dtdtd"><span class="text_left" style="float: left;
                                                width: 79%;">Drawing Approvals Pending to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblDrawingApprovalsPndng" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/InsptnReportStatus.aspx?Mode=ICtldt"><span class="text_left"
                                                style="float: left; width: 79%;">Inspections Completed to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblInstptnCmpltdDt" runat="server"></asp:Label></span> </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=mtd"><span class="text_left" style="float: left;
                                                width: 79%;">Inspections Pending to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblInstptnPndngDt" runat="server"></asp:Label></span> </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Logistics/CTOneStatus.aspx?Mode=ECtldt"><span class="text_left" style="float: left;
                                                width: 79%;">CT-1's(CEDEA) Completed to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblCEDEACmpltDt" runat="server"></asp:Label></span> </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx?Mode=cpd"><span class="text_left" style="float: left;
                                                width: 79%;">CT-1's(CEDEA) Pending to Date </span><span style="text-align: center;
                                                    float: right; width: 19%;">
                                                    <asp:Label ID="lblCEDEAPndngDt" runat="server"></asp:Label></span> </a>
                                        </td>
                                    </tr>
                                    <%-- <tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx"><span class="text_left" style="float: left;
                                                width: 80%;">No. of Quotations to Date </span><span style="text-align: center; float: right;
                                                    width: 20%;">
                                                    <asp:Label ID="lblInspectionApprovals" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>--%>
                                    <%--<tr>
                                        <td class="dashbord_list_left" colspan="2">
                                            <a href="../Purchases/LPOStatus.aspx"><span class="text_left" style="float: left;
                                                width: 80%;">Excise Duty Exemption Applicable Remainder Time to Date </span><span
                                                    style="text-align: center; float: right; width: 20%;">
                                                    <asp:Label ID="lblExciseDutyExcemption" runat="server"></asp:Label></span>
                                            </a>
                                        </td>
                                    </tr>--%>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="full_dashbord1" class="full_dashbord" style="margin-top: 10px; margin-bottom: 20px;">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <th style="text-align: left; padding: 0px 20px 0px;" class="dash_heading">
                                            Inspection Status
                                        </th>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <tr>
                                        <td class="dashbord_list_left">
                                            No. of Internal Inspections Planned for<span class="weeknumber"> 45</span><sup></sup>
                                            week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="lbinsp" runat="server" PostBackUrl=""></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left">
                                            No. of Internal Inspections Carried out in <span class="weeknumber">46</span><sup>
                                            </sup>week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="lpcarriedoutinsp" runat="server" PostBackUrl=""></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;">
                                            Pending Internal Inspections Up to <span class="weeknumber1">51</span><sup></sup>
                                            week
                                        </td>
                                        <td class="dashbord_list_right" style="color: red;">
                                            <asp:LinkButton ID="Lbpendinginsp" runat="server" ForeColor="Red" PostBackUrl=""></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 50%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list">
                                    <tr>
                                        <td class="dashbord_list_left">
                                            No. of Ext / Pre-Shipment Inspections Planned for <span class="weeknumber">49</span>
                                            <sup></sup>Week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="preplanned" runat="server" PostBackUrl=""></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left">
                                            No. of Ext / Pre-Shipment Inspections Carried out in <span class="weeknumber">47</span>
                                            <sup></sup>week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="preover" runat="server" PostBackUrl=""></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;">
                                            Pending Ext / Pre-Shipment Inspections Up to <span class="weeknumber1">42</span>
                                            <sup></sup>week
                                        </td>
                                        <td class="dashbord_list_right" style="color: red;">
                                            <asp:LinkButton ID="prepen" runat="server" ForeColor="Red" PostBackUrl=""></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="full_dashbord2" class="full_dashbord" style="margin-top: 10px; margin-bottom: 20px;">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr class="dash_heading">
                                        <th style="text-align: left; padding: 0px 20px 0px; width: 52%;">
                                            Dispatch Status
                                        </th>
                                        <th>
                                        </th>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 52%; border-right: 0 0 rgb(0, 0, 0); padding-right: 2%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list" style="margin: 0px;">
                                    <tr>
                                        <td class="dashbord_list_left">
                                            Dispatches of FPO&#39;s Due in <span class="weeknumber">23</span><sup></sup>week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="DfpoDue" runat="server" PostBackUrl=""></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left">
                                            Dispatches of FPO&#39;s During <span class="weeknumber"></span><sup></sup>week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="lbDDuring" runat="server" PostBackUrl="" Text="0"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;">
                                            Dispatches of FPO&#39;s Over Due
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="lbdfpood" ForeColor="Red" runat="server" Text="0" PostBackUrl=""></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 48%;">
                                <table border="0" cellpadding="0" cellspacing="0" class="all_list" style="padding-left: 25px;">
                                    <tr>
                                        <td class="dashbord_list_left">
                                            Delivery of LPO&#39;s Due in <span class="weeknumber"></span><sup></sup>week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="DBLpodues" runat="server" PostBackUrl=""></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left">
                                            Delivery of LPO&#39;s During <span class="weeknumber"></span><sup></sup>week
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="DBLpoDuring" runat="server" PostBackUrl=""></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dashbord_list_left" style="color: red;">
                                            Over Due Delivery of LPO&#39;s
                                        </td>
                                        <td class="dashbord_list_right">
                                            <asp:LinkButton ID="DBLpoOverdue" runat="server" ForeColor="Red" PostBackUrl=""></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
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
    <style type="text/css">
        a, u
        {
            text-decoration: none;
        }
    </style>

</asp:Content>
