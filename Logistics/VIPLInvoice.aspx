<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="VIPLInvoice.aspx.cs" Inherits="VOMS_ERP.Logistics.VIPLInvoice" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Proforma Invoice"
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
                        <%-- start--%>
                                <div id="main_content">
    <!-- InstanceBeginEditable name="body content" -->
       
        <div id="add_enquiry">
        	
            <div class="row" style="padding:5px;border-bottom:1px solid #000;width:99%;">
                <div style="width:35%;">
                    <span class="bold">Customer : </span>
                    
                    <select style="width:145px;"><option>--select--</option>
                    		<option>TACL</option>
                            <option>DCM</option>
                            <option>WACEM</option>
                            <option>FORTIA</option>
                            <option>ABCL</option>
                    							
                    </select>&nbsp;&nbsp;&nbsp;
            	</div>
                <div style="width:38%;">
                    
                </div>
                <div style="width:27%;float:right;">
                    <span class=""><input type="radio" />Profirma Invoice</span>
                    <span><input type="radio" />Final Invoice</span>
            	</div>
                
                </div>
            	
            <div class="row" style="border-bottom:1px solid #000;">
                <!-- left part-->
                <div style="width:38.5%;border-right:1px solid #000;padding:5px">
                    
                        <div style="font-weight:bold;float:left;width:100%;">
                            Exporter :
                        </div>
                        <div class="row">M/S VOLTA IMPEX PVT LTD,</div>
                        <div class="row">123/3RT, SANJEEVA REDDY NAGAR,</div>
                        <div class="row">HYDERBAD - 500 038</div>
                        <div class="row">INDIA.</div>
                        <div class="row"><span class="bold">TIN No. 28920296531</span></div>
                        <div class="row"><span class="bold">IEC Code No. 0996008306</span></div>
                    
            	</div>
                <!-- Left part End-->
                <!-- Right part-->
                <div style="width:59%;padding:5px;">
                    <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                        <div><span class="bold">Proforma Invoice No : </span><span><input type="text" value="1212/2008-2009" /></span></div>
                        <div>
                            <span class="bold">Date :</span>
                            <span class=""><script type="text/javascript">
					<!--
                                               var currentTime = new Date()
                                               var month = currentTime.getMonth() + 1
                                               var day = currentTime.getDate()
                                               var year = currentTime.getFullYear()

                                               document.write(day + "/" + month + "/" + year)
					//-->
					</script></span>
                        </div>
                    </div>
                    <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                    <span class="bold" style="vertical-align:top;">FPOs : </span><span><select size="3" class="select"><option>DC-015</option><option>DCGL-028</option></select></span>
                    <span class="" style="vertical-align:top;">DC-015/DCGL-028</span>
                    </div>
                    <div class="row" style="padding-top:8px;">
                    <span class="bold" style="vertical-align:top;">Other reference : </span><span><input type="text" size="40"/></span>
                    </div>
                </div>
            <!-- Right part END-->
           </div>
           
           <div class="row" style="border-bottom:1px solid #000;">
                <!-- left part-->
                <div style="width:38.5%;border-right:1px solid #000;padding:5px">
                    	<div class="row" style="border-bottom:1px solid #000;padding:0px 0px ;">
                        <div style="font-weight:bold;float:left;width:100%;">
                            Consignee :
                        </div>
                        <div class="row bold">DIAMOND CEMENT (GH) LTD</div>
                        <div class="row">Post Box No.69,</div>
                        <div class="row">Aflao</div>
                        <div class="row">GhanaWest Africa</div>
                        <div class="row"><span class="bold">PH: 28920296531</span></div>
                        </div>
                        <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                        <span class="bold" style="vertical-align:top;">Notify : </span>
                        <span ><textarea cols="35"></textarea></span>
                        </div>
                        
                        <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                            <div style="border-right:1px solid #000;"><span class="bold">Pre-Carriage by : </span><input type="text" value="" size="16"/></div>
                            <div style="padding:0px 0px 0px 5px;"><span class="bold">Place of receipt by pre-Carrier : </span><input type="text" value="" size="16"/></div>
                        </div>
                    	
                        <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                            <div style="border-right:1px solid #000;"><span class="bold">Vessel / Flight No. : </span><input type="text" value="By Air" size="16"/></div>
                            <div style="padding:0px 0px 0px 5px;"><span class="bold">Port Of Loading : </span><input type="text" value="Mumbai" size="16"/></div>
                            
                        </div>
                        
                        <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                            <div style="border-right:1px solid #000;"><span class="bold">Port Of Discharge : </span><input type="text" value="Tema" size="16"/></div>
                            <div style="padding:0px 0px 0px 5px;"><span class="bold">Place of Delivery : </span><input type="text" value="" size="16"/></div>
                        </div>
                        
                    
            	</div>
                <!-- Left part End-->
                <!-- Right part-->
                <div style="width:59%;padding:5px;">
                    <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                        <div class="row bold">DIAMOND CEMENT (GH) LTD</div>
                        <div class="row">Post Box No.69,</div>
                        <div class="row">Aflao</div>
                        <div class="row">GhanaWest Africa</div>
                        <div class="row"><span class="bold">PH: 28920296531</span></div>
                    </div>
                    <div class="row" style="border-bottom:1px solid #000;padding:5px 0px;">
                    <div style="border-right:1px solid #000;"><span class="bold">Place of Orgin of Goods : </span><input type="text" value="India" size="14"/></div>
                    <div style="padding:0px 0px 0px 5px;"><span class="bold">Place of Final Destination : </span><input type="text" value="" size="14"/></div>
                    </div>
                    <div class="row" style="padding-top:5px;">
                    <span class="bold" style="vertical-align:top;">Tems Of Delivery and Payment : </span>
                    <span ><textarea cols="40" rows="8"></textarea></span>
                    </div>
                </div>
            <!-- Right part END-->
           </div>
           
           
           <div id="invoice_details">
    		<table class="enquiry_details" cellpadding="0" cellspacing="0" border="0">
                <tr>
                <th style="width:60%;"></th>
                <th style="width:15%;">Quantity</th>
                <th style="width:10%;">Rate US$</th>
                <th style="width:15%;">Amount US$</th>
                </tr>
                <tr>
                <td class="bold text_left text_decoration">FPO No. DC-015 Date : <script type="text/javascript">
					<!--
                                                                                     var currentTime = new Date()
                                                                                     var month = currentTime.getMonth() + 1
                                                                                     var day = currentTime.getDate()
                                                                                     var year = currentTime.getFullYear()

                                                                                     document.write(day + "/" + month + "/" + year)
					//-->
					</script></td>
                <td></td>
                <td></td>
                <td></td>
                </tr>
                <tr>
                <td class="text_left">Dry Gas Meter for Netel make Stack Monitorng kit</td>
                <td class="text_right">1.00 Nos.</td>
                <td class="text_right">500.00</td>
                <td class="text_right">500.00</td>
                </tr>
                <tr>
                <td class="text_left">Vaccum Pump for Netel make Stack Monitorng kit</td>
                <td class="text_right">1.00 Nos.</td>
                <td class="text_right">650.00</td>
                <td class="text_right">650.00</td>
                <tr>
                <td class="text_left">"U" Tube Monometer with scale</td>
                <td class="text_right">2.00 Nos.</td>
                <td class="text_right">20.00</td>
                <td class="text_right">40.00</td>
                </tr>
                <tr>
                <td class="bold text_left text_decoration">FPO No. DCGL-028 Date : <script type="text/javascript">
					<!--
                                                                                       var currentTime = new Date()
                                                                                       var month = currentTime.getMonth() + 1
                                                                                       var day = currentTime.getDate()
                                                                                       var year = currentTime.getFullYear()

                                                                                       document.write(day + "/" + month + "/" + year)
					//-->
					</script>
                    </td>
                <td></td>
                <td></td>
                <td></td>
                </tr>
                <tr>
                <td class="text_left">Grease sprary ststem spares</td>
                <td class="text_right">1.00 set.</td>
                <td class="text_right">4980.00</td>
                <td class="text_right">25550.00</td>
                </tr>
                <tr></tr><tr></tr>
                <tr>
                <td></td>
                <td></td>
                <td></td>
                <td class="text_right">26740.00</td>
                </tr>
                <tr>
                <td></td>
                <td></td>
                <td></td>
                <td class="text_right">0.00</td>
                </tr>
                <tr>
                <td></td>
                <td></td>
                <td></td>
                <td class="text_right">26740.00</td>
                </tr>
            </table>
            </div>
        </div>
        
	<!-- InstanceEndEditable -->
    </div>

                         <%--End--%>
                        </td>
                    </tr>
                    <tr><td>
                    <center>
                                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                                <tbody>
                                                    <tr valign="middle">
                                                        <td align="center" valign="middle" class="bcTdButton" style="visibility: hidden">
                                                            <div id="Div4" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnsavenew" Text="Save & New" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div1" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div2" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnclear" OnClientClick="Javascript:clearAll()"
                                                                    Text="Clear" />
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
                                        </center>
                    </td></tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
