using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using BAL;
using System.Text;
using System.Collections.Generic;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Logistics
{
    public partial class Iom2Form : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        CommonBLL CBLL = new CommonBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        CheckListBLL CLBLL = new CheckListBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        IOMTemplate2BLL IOMTBLL = new IOMTemplate2BLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";

        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(Iom2Form));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        txtRcd.Attributes.Add("readonly", "readonly");
                        if (!IsPostBack)
                        {
                            Session["IomTmp2"] = null;
                            txtRcd.Text = DateTime.Now.ToString("dd-MM-yyyy");
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
                            divOpen_attachments.InnerHtml = Att_open();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        #endregion Methods

        #region Methods

        /// <summary>
        /// Get File Name
        /// </summary>
        /// <returns></returns>
        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }

        /// <summary>
        /// Get Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                Session.Remove("IomTmp2");
                BindDropDownList(ddlRefno, CLBLL.SelectChekcListDtls(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())).Tables[2]);
                BindDropDownList(ddlBivacShpmntPlnngNo, (CLBLL.SelectChekcListDtls(CommonBLL.FlagISelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[1]);
                BindDropDownList(ddlByAir_Sea, (embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ShipmentMode).Tables[0]));
                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                {
                    BindDropDownList(ddlRefno, CLBLL.SelectChekcListDtls(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())).Tables[2]);
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    BindDropDownList(ddlBivacShpmntPlnngNo, (CLBLL.SelectChekcListDtls(CommonBLL.FlagISelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[1]);
                    EditRecord(IOMTBLL.Select(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                }
                else if (Request.QueryString["PIReqID"] != null && Request.QueryString["PIReqID"] != "")
                {
                    ddlRefno.SelectedValue = Request.QueryString["PIReqID"].ToString();
                    FillInputFields(IOMTBLL.Select(CommonBLL.FlagModify, Guid.Empty, new Guid(Request.QueryString["PIReqID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Boxes
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        protected void BindListBox(ListBox lb, DataSet CommonDt)
        {
            try
            {
                lb.DataSource = CommonDt.Tables[0];
                lb.DataTextField = "Description";
                lb.DataValueField = "ID";
                lb.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Display Data into Input Fields Using Data Set
        /// </summary>
        /// <param name="CommonDt"></param>
        private void FillInputFields(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    if (ddlRefno.SelectedIndex != 0)
                    {
                        txtPkngLst.Text = CommonDt.Tables[0].Rows[0]["PkngListNo"].ToString();
                        hdfPkngLstID.Value = CommonDt.Tables[0].Rows[0]["PkingLstID"].ToString();
                    }
                    txtRefno.Text = "IOM-PUR/" + Session["AliasName"].ToString() + "/" + CommonDt.Tables[0].Rows[0]["RefNoID"].ToString()
                    + "/" + CommonBLL.FinacialYearShort;

                    ddlStatus.SelectedValue = "2";
                    ddlStatus.Enabled = false;
                    txtPrfmatInvc.Text = CommonDt.Tables[0].Rows[0]["PrfmInvcNo"].ToString();
                    hdfPrfmInvcID.Value = CommonDt.Tables[0].Rows[0]["PrfmInvcID"].ToString();
                    hdfPrfmInvcDt.Value = CommonDt.Tables[0].Rows[0]["PrfmaInvcDt"].ToString();
                    txtFAdr.Text = Session["UserMail"].ToString();
                    txtPrfmatInvc.Enabled = txtPkngLst.Enabled = false;
                    txtPkngLst.Text = CommonDt.Tables[0].Rows[0]["PkngListNo"].ToString();
                    string SelectedCustomers = String.Join(",", CommonDt.Tables[0].AsEnumerable().Select(r => r.Field<Guid>("CustomerID")).ToArray().Distinct());

                    BindListBox(lbfpos, IOMTBLL.Select(CommonBLL.FlagFSelect, Guid.Empty, new Guid(CommonDt.Tables[0].Rows[0]["CustomerID"].ToString()),
                        SelectedCustomers, new Guid(Session["CompanyID"].ToString())));

                    string[] FPoString = new string[] { }; string[] LPoString = new string[] { };
                    string FPoids = "", LPoids = "";
                    foreach (DataRow frow in CommonDt.Tables[0].Rows)
                    {
                        FPoids += frow["FPOs"].ToString() + ",";
                        if (frow["LPOs"].ToString() != "" || frow["LPOs"].ToString() != null)
                            LPoids += frow["LPOs"].ToString() + ",";
                    }
                    FPoString = FPoids.ToLower().TrimEnd(',').Split(',');
                    LPoString = LPoids.ToLower().TrimEnd(',').Split(',');

                    foreach (ListItem li in lbfpos.Items)
                        li.Selected = FPoString.Where(F => F.ToString().Trim() == li.Value.ToString()).Any();

                    BindListBox(lblpos, IOMTBLL.Select(CommonBLL.FlagGSelect, Guid.Empty, new Guid(CommonDt.Tables[0].Rows[0]["SuplrID"].ToString()),
                        FPoids.TrimEnd(',').ToString(), new Guid(Session["CompanyID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = LPoString.Where(F => F.ToString().Trim() == li.Value.ToString()).Any();

                    if (CommonDt.Tables[0].Rows[0]["Attachements"].ToString().Trim(',') != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachements"].ToString().Trim(',').Split(',')).ToArray());
                        Session["IomTmp2"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        divOpen_attachments.InnerHtml = Att_open();
                    }
                    string txt = "";
                    if (CHkBivac.Checked == false)
                        txt = ddlRefno.SelectedItem.Text;
                    else
                        txt = ddlBivacShpmntPlnngNo.SelectedItem.Text;

                    txtBdy.Text = "Dear Sir/Madam," + System.Environment.NewLine + System.Environment.NewLine +
                            "       With reference to the listed IOM details, please check respective Shipment Planning : " + txt + ", " +
                            "Proforma Invoice : " + txtPrfmatInvc.Text + " and Packing List : " + txtPkngLst.Text +
                            " and please confirm the receipt by approving and update the status subsequently." + System.Environment.NewLine +
                            System.Environment.NewLine +
                            " For any clarifications, kindly revert." + System.Environment.NewLine + System.Environment.NewLine +
                            " Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                            " Yours Faithfully," + System.Environment.NewLine +
                            " " + Session["UserName"].ToString() + ".";
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Check List does't have P.Invoice or Packing List.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM2Form", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit/Update Details
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                if (Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["ISBivac"]) != false)
                {
                    CHkBivac.Checked = true;
                    ddlBivacShpmntPlnngNo.SelectedValue = CommonDt.Tables[0].Rows[0]["ChkListID"].ToString();
                    txtPsd.Attributes.Add("value", "volta@12");
                    if (CommonDt.Tables[0].Rows[0]["By_Air_Sea"].ToString() != null && CommonDt.Tables[0].Rows[0]["By_Air_Sea"].ToString() != Guid.Empty.ToString())
                        ddlByAir_Sea.SelectedValue = CommonDt.Tables[0].Rows[0]["By_Air_Sea"].ToString();
                    txtRefno.Text = CommonDt.Tables[0].Rows[0]["IOMRefNo"].ToString();
                    txtTAdr.Text = CommonDt.Tables[0].Rows[0]["ToEmailID"].ToString();
                    txtFAdr.Text = CommonDt.Tables[0].Rows[0]["FromEmailID"].ToString();
                    txtSbjt.Text = CommonDt.Tables[0].Rows[0]["Subject"].ToString();
                    txtPrfmatInvc.Text = CommonDt.Tables[0].Rows[0]["PrfmInvcNo"].ToString();
                    hdfPrfmInvcID.Value = CommonDt.Tables[0].Rows[0]["PrfmaInvcID"].ToString();
                    txtRcd.Text = CommonDt.Tables[0].Rows[0]["IOMDt"].ToString();
                    txtBdy.Text = CommonDt.Tables[0].Rows[0]["Body"].ToString();
                    txtSpmntInvcNo.Text = CommonDt.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString();
                    txtSpmntInvcDt.Text = CommonDt.Tables[0].Rows[0]["PrfmaInvcDt"].ToString();
                    txtFAdr.Enabled = txtPsd.Enabled = txtRcd.Enabled = ddlRefno.Enabled = false;
                    txtSbjt.Enabled = txtBdy.Enabled = txtTAdr.Enabled = false;
                    txtPrfmatInvc.Enabled = txtPkngLst.Enabled = false;
                    ddlStatus.Items.RemoveAt(1);
                }
                else
                {
                    txtRefno.Text = CommonDt.Tables[0].Rows[0]["IOMRefNo"].ToString();
                    ddlRefno.SelectedValue = CommonDt.Tables[0].Rows[0]["ChkListID"].ToString();
                    if (CommonDt.Tables[0].Rows[0]["By_Air_Sea"].ToString() != null && CommonDt.Tables[0].Rows[0]["By_Air_Sea"].ToString() != Guid.Empty.ToString())
                        ddlByAir_Sea.SelectedValue = CommonDt.Tables[0].Rows[0]["By_Air_Sea"].ToString();
                    txtPsd.Attributes.Add("value", "volta@12");
                    txtTAdr.Text = CommonDt.Tables[0].Rows[0]["ToEmailID"].ToString();
                    txtFAdr.Text = CommonDt.Tables[0].Rows[0]["FromEmailID"].ToString();
                    txtSbjt.Text = CommonDt.Tables[0].Rows[0]["Subject"].ToString();
                    txtPrfmatInvc.Text = CommonDt.Tables[0].Rows[0]["PrfmInvcNo"].ToString();
                    txtPkngLst.Text = CommonDt.Tables[0].Rows[0]["PkngListNo"].ToString();
                    hdfPrfmInvcID.Value = CommonDt.Tables[0].Rows[0]["PrfmaInvcID"].ToString();
                    hdfPkngLstID.Value = CommonDt.Tables[0].Rows[0]["PkngLstID"].ToString();
                    hdfPrfmInvcDt.Value = CommonDt.Tables[0].Rows[0]["PrfmaInvcDt"].ToString();
                    txtRcd.Text = CommonDt.Tables[0].Rows[0]["IOMDt"].ToString();
                    txtBdy.Text = CommonDt.Tables[0].Rows[0]["Body"].ToString();
                    txtSpmntInvcNo.Text = CommonDt.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString();
                    txtSpmntInvcDt.Text = CommonDt.Tables[0].Rows[0]["PrfmaInvcDt"].ToString();
                    txtVessel.Text = CommonDt.Tables[0].Rows[0]["Vessel"].ToString();
                    txtBlno.Text = CommonDt.Tables[0].Rows[0]["Blno"].ToString();
                    txtSbno.Text = CommonDt.Tables[0].Rows[0]["Sbno"].ToString();
                    txtFAdr.Enabled = txtPsd.Enabled = txtRcd.Enabled = ddlRefno.Enabled = false;
                    txtSbjt.Enabled = txtBdy.Enabled = txtTAdr.Enabled = false;
                    txtPrfmatInvc.Enabled = txtPkngLst.Enabled = false;
                    ddlStatus.Items.RemoveAt(1);
                }

                if (CommonDt.Tables[0].Rows[0]["Status"].ToString() == "Created")
                    ddlStatus.SelectedValue = "0";
                else
                    ddlStatus.Items.FindByText(CommonDt.Tables[0].Rows[0]["Status"].ToString()).Selected = true;

                BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                    CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                string FPoids = CommonDt.Tables[0].Rows[0]["FPOs"].ToString();
                string[] FPoString = FPoids.Split(',');
                string LPoids = CommonDt.Tables[0].Rows[0]["LPOs"].ToString();
                string[] LPoString = LPoids.Split(',');
                foreach (ListItem li in lbfpos.Items)
                    li.Selected = FPoString.Where(F => F.ToString().Trim().ToLower() == li.Value.ToString().Trim().ToLower()).Any();

                BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "",
                    CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                foreach (ListItem li in lblpos.Items)
                    li.Selected = LPoString.Where(F => F.ToString().Trim().ToLower() == li.Value.ToString().Trim().ToLower()).Any();

                if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                {
                    Session.Remove("IomTmp2");
                    ArrayList attms = new ArrayList();
                    attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                    Session["IomTmp2"] = attms;
                    divListBox.InnerHtml = AttachedFiles();
                    divOpen_attachments.InnerHtml = Att_open();
                }
                ddlRefno.Enabled = false;
                DivComments.Visible = true;
                DvShpmntInvc.Visible = true;
                DvShpmntInvcDt.Visible = true;
                btnSend.Text = "Update";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                txtRefno.Text = txtTAdr.Text = txtFAdr.Text = txtPsd.Text = "";
                txtSbjt.Text = txtComments.Text = txtBdy.Text = txtPrfmatInvc.Text = txtPkngLst.Text = "";
                ddlRefno.SelectedValue = ddlStatus.SelectedValue = Guid.Empty.ToString();
                lbfpos.Items.Clear();
                lblpos.Items.Clear();
                Session.Remove("IomTmp2");
                divListBox.InnerHtml = AttachedFiles();
                divOpen_attachments.InnerHtml = "";
                btnSend.Text = "Send";
                ddlBivacShpmntPlnngNo.SelectedIndex = -1;
                txtSpmntInvcNo.Text = "";

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Upload Files 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileUploadComplete(object sender, EventArgs e)
        {
            try
            {
                if (AsyncFileUpload1.HasFile)
                {
                    if (AsyncFileUpload1.PostedFile.ContentLength < 25165824)
                    {
                        ArrayList alist = new ArrayList();
                        string strPath = MapPath("~/uploads/");
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["IomTmp2"] != null)
                        {
                            alist = (ArrayList)Session["IomTmp2"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["IomTmp2"] == null)
                            alist.Add(FileNames);
                        Session["IomTmp2"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Bind Attachments
        /// </summary>
        /// <returns></returns>
        private string AttachedFiles()
        {
            try
            {
                if (Session["IomTmp2"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["IomTmp2"];
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    if (all.Count > 0)
                    {
                        sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                        for (int k = 0; k < all.Count; k++)
                            sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                        sb.Append("</select>");
                    }
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }

        public string Att_open()
        {
            StringBuilder sbb = new StringBuilder();
            try
            {
                if (Session["IomTmp2"] != null)
                {
                    string url = "../uploads/";
                    ArrayList al = new ArrayList();
                    al = (ArrayList)Session["IomTmp2"];
                    for (int i = 0; i < al.Count; i++)
                    {
                        string finUrl = url + "" + al[i].ToString();
                        string fileName = al[i].ToString();
                        int fileExtPos = fileName.LastIndexOf(".");
                        if (fileExtPos >= 0)
                            fileName = fileName.Substring(0, fileExtPos);
                        sbb.Append(" " + (i + 1) + ") <a href='" + finUrl + "' id='openfile" + i + "' onclick='saveToDisk("
                            + finUrl + "," + al[i].ToString() + ");' target='_blank'>" + fileName + "</a><br/>");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
            return sbb.ToString(); ;
        }

        #endregion

        #region Button Click Evetns

        /// <summary>
        /// Save/Send IOM Template Details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName(); string BtnText = "";
                Guid Id = Guid.Empty;
                Guid Id1 = Guid.Empty;
                string status = "";
                string Path = MapPath("~/uploads/");
                string Atchmnts = Session["IomTmp2"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["IomTmp2"]).Cast<string>().ToArray());
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (ddlRefno.SelectedIndex != 0)
                {
                    Id = new Guid(ddlRefno.SelectedValue);
                    Id1 = new Guid(hdfPkngLstID.Value);
                }
                else
                {
                    Id = new Guid(ddlBivacShpmntPlnngNo.SelectedValue);
                    Id1 = Guid.Empty;
                }
                if (btnSend.Text == "Send")
                {
                    String[] Cc = new String[0];
                    status = "Sent";
                    status = CommonBLL.SendMailsWithPath(txtFAdr.Text.Trim(), txtPsd.Text.Trim(), txtTAdr.Text.Trim().Split(','), Cc,
                        string.Empty, txtSbjt.Text, txtBdy.Text.Trim(), Atchmnts.Trim().Split(','));
                    if (status == "Sent")
                    {
                        DataSet IOMIDREF = IOMTBLL.Select(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        txtRefno.Text = "IOM-PUR/" + Session["AliasName"].ToString() + "/" + IOMIDREF.Tables[0].Rows[0]["RefNoID"].ToString()
                       + "/" + CommonBLL.FinacialYearShort;
                        res = IOMTBLL.InsertUpdateDeleteIOMT(CommonBLL.FlagNewInsert, Guid.Empty, txtRefno.Text, txtTAdr.Text, txtFAdr.Text,
                            CommonBLL.DateInsert(txtRcd.Text), Id, new Guid(hdfPrfmInvcID.Value), Id1, FpoIds, LpoIds, txtSpmntInvcNo.Text,
                            CommonBLL.DateInsert(hdfPrfmInvcDt.Value), txtSbjt.Text, txtBdy.Text, ddlStatus.SelectedItem.Text,
                            Atchmnts, txtVessel.Text, txtBlno.Text, txtSbno.Text, txtComments.Text, new Guid(Session["UserID"].ToString()),
                            new Guid(Session["CompanyID"].ToString()), new Guid(ddlByAir_Sea.SelectedValue));
                    }
                }
                else if (btnSend.Text == "Update")
                {
                    res = IOMTBLL.InsertUpdateDeleteIOMT(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        txtRefno.Text, txtTAdr.Text, txtFAdr.Text, CommonBLL.DateInsert(txtRcd.Text), Id, new Guid(hdfPrfmInvcID.Value),
                        Id1, FpoIds, LpoIds, txtSpmntInvcNo.Text, CommonBLL.DateInsert(txtSpmntInvcDt.Text), txtSbjt.Text, txtBdy.Text,
                        ddlStatus.SelectedItem.Text, Atchmnts, txtVessel.Text, txtBlno.Text, txtSbno.Text, txtComments.Text,
                        new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()), new Guid(ddlByAir_Sea.SelectedValue));
                }
                if (res == 0)
                {
                    if (btnSend.Text == "Send")
                        BtnText = "Save";
                    else
                        BtnText = "Update";
                    ALS.AuditLog(res, BtnText, "", "IOM Form Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "IOM Form Details", "Data inserted successfully.");
                    ClearInputs();
                    Response.Redirect("Iom2FormStatus.Aspx", false);
                }
                else
                {
                    if (btnSend.Text == "Send")
                    {
                        ALS.AuditLog(res, "Save", "", "IOM Form Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details",
                        "Error while Inserting. " + status);
                    }
                    else
                    {
                        ALS.AuditLog(res, "Update", "", "IOM Form Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details",
                        "Error while Updating. " + status);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                txtRcd.Text = "";
                ClearInputs();
                CHkBivac.Checked = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// Refference Number Selected index changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRefno_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlRefno.SelectedValue != Guid.Empty.ToString() && ddlBivacShpmntPlnngNo.SelectedIndex == 0)
                {
                    divListBox.InnerHtml = "";
                    divOpen_attachments.InnerHtml = "";
                    FillInputFields(IOMTBLL.Select(CommonBLL.FlagESelect, Guid.Empty, new Guid(ddlRefno.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                }
                else if (ddlRefno.SelectedIndex == 0 && ddlBivacShpmntPlnngNo.SelectedIndex != 0)
                {
                    FillInputFields(IOMTBLL.Select(CommonBLL.FlagXSelect, Guid.Empty, new Guid(ddlBivacShpmntPlnngNo.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                    ddlRefno.Enabled = false;
                }
                else
                    ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Check Box BIVAC Checked Changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CHkBivac_CheckedChanged(object sender, EventArgs e)
        {
            ClearInputs();
            if (CHkBivac.Checked)
            {
                ddlBivacShpmntPlnngNo.Enabled = true;
                ddlRefno.Enabled = false;
            }
            else
            {
                ddlBivacShpmntPlnngNo.Enabled = false;
                ddlRefno.Enabled = true;
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStatus.SelectedValue == "4")
                txtSpmntInvcNo.Enabled = false;
            else
                txtSpmntInvcNo.Enabled = true;
        }


        #endregion

        #region Web Methods

        /// <summary>
        /// This is used to Check the Enquiry Number
        /// </summary>
        /// <param name="EnqNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckSHPINVNo(string SHPINVNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckNo('D', SHPINVNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["IomTmp2"];
                all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        /// <summary>
        /// This is used to get Proforma Invoice No
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetProformaInvNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo('U', RefNo, Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string attachmnts_ReLoad()
        {
            return Att_open();
        }
        #endregion
    }
}