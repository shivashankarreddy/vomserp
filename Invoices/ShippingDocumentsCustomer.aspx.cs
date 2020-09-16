using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Threading;
using BAL;
using System.Data;
using Ajax;
using System.Data.SqlClient;
using VOMS_ERP.Admin;

namespace VOMS_ERP.Invoices
{
    public partial class ShippingDocumentsCustomer : System.Web.UI.Page
    {
        ErrorLog ELog = new ErrorLog();
        CustomerBLL CBLL = new CustomerBLL();
        NewFPOBLL FPOBLL = new NewFPOBLL();
        ShipDocCustBLL SDCBLL = new ShipDocCustBLL();
        AuditLogs ALS = new AuditLogs();

        Dictionary<string, string> CustID1 = new Dictionary<string, string>();
        Dictionary<string, string> Airway1 = new Dictionary<string, string>();
        Dictionary<string, string> FPOIDs1 = new Dictionary<string, string>();
        string air = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ShippingDocumentsCustomer));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {

                            ClearAll();
                            GetData();


                            // GetData();
                            //txtFrmDt.Attributes.Add("readonly", "readonly");
                            // txtToDt.Attributes.Add("readonly", "readonly");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Documents Customer", ex.Message.ToString());
            }

        }
        private void GetData()
        {
            try
            {


                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                {

                    DataSet ds = new DataSet();
                    ds = CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    BindDropDownList(ddlCpnyLst, ds.Tables[0]);

                    Customers();
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                }
                //ddlCommInvNo.SelectedValue = Request.QueryString["ID"].ToString();
                else
                {

                    DataSet ds = new DataSet();
                    ds = CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    BindDropDownList(ddlCpnyLst, ds.Tables[0]);

                    Customers();
                    //Td2.Visible = true;
                    //Td3.Visible = false;
                    //InvNumber.Checked = true;
                    //InvcDate.Checked = false;

                    //FillInputFields(IOMTBLL.Select(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, ddlRefno.SelectedValue, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "ShippingDocumentsCustomer", ex.Message.ToString());
            }
        }
        private void EditRecord(Guid ID)
        {
            string CustID = ""; string AirLadingIds = ""; string FPOIDs = ""; string send;
            DataSet ds = new DataSet();
            ds = SDCBLL.GetAirBOLNO(Convert.ToChar("E"), new Guid(Session["CompanyID"].ToString()), ID);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
            {
                Customers();
                ViewState["Edit_Rows"] = ds.Tables[1];
                CustID = ds.Tables[0].Rows[0]["CustomerID"].ToString();
                CustID1 = CustID.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                if (CustID1 != null && CustID1.Count > 0)
                {
                    for (int i = 0; i < ListBoxCustomer.Items.Count; i++)
                    {
                        string cs = ListBoxCustomer.Items[i].Value.ToString();
                        if (CustID1.ContainsKey(cs.Trim()))
                            ListBoxCustomer.Items[i].Selected = true;
                    }
                }
                ListBoxCustmrSelected();
                AirLadingIds = ds.Tables[0].Rows[0]["AirBillId"].ToString();
                air = AirLadingIds;
                Airway1 = AirLadingIds.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                if (Airway1 != null && Airway1.Count > 0)
                {
                    for (int i = 0; i < ListBoxAirBillNo.Items.Count; i++)
                    {
                        string cs = ListBoxAirBillNo.Items[i].Value.ToString();
                        if (Airway1.ContainsKey(cs.Trim()))
                            ListBoxAirBillNo.Items[i].Selected = true;
                    }
                }
                ListBoxAirBillSelected();
                FPOIDs = ds.Tables[0].Rows[0]["FPOIDs"].ToString();
                FPOIDs1 = FPOIDs.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                if (FPOIDs1 != null && FPOIDs1.Count > 0)
                {
                    for (int i = 0; i < ListBoxFpo.Items.Count; i++)
                    {
                        string cs = ListBoxFpo.Items[i].Value.ToString();
                        if (FPOIDs1.ContainsKey(cs.Trim()))
                            ListBoxFpo.Items[i].Selected = true;
                    }
                }

                Session["AirLading_IDs"] = Airway1;
                Session["FPO_IDs"] = FPOIDs1;
                Session["Customer_IDs"] = CustID;

                //ListBoxCustmrSelected();
                //ListBoxAirBillSelected();
                //LBSelectedItems();
                send = ds.Tables[0].Rows[0]["SentByCourier"].ToString();
                if (send == "True")
                {
                    HFInvNo.Value = "1";
                }
                else
                {
                    HFInvNDate.Value = "1";
                }
                CourierName.Text = ds.Tables[0].Rows[0]["CourierName"].ToString();
                CourierNo.Text = ds.Tables[0].Rows[0]["CourierNo"].ToString();
                DocumentSent.Text = ds.Tables[0].Rows[0]["DocumentSent"].ToString();
                ReceivedPerson.Text = ds.Tables[0].Rows[0]["ReceivedPerson"].ToString();
                Documentcarrier.Text = ds.Tables[0].Rows[0]["Documentcarrier"].ToString();
                RecDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["CourierSendDate"].ToString()));
                ddlCpnyLst.SelectedValue = ds.Tables[0].Rows[0]["Unit"].ToString();
                if (RecDate.Text == "31-12-1999")
                {
                    RecDate.Text = "";
                }
                ContactNo.Text = ds.Tables[0].Rows[0]["CarrierContactNo"].ToString();
                btnSave.Text = "Update";
            }
        }



        //private void ListBoxCustSelect()
        //{
        //    string custIds = ""; string AirLadingIds = "";

        //    custIds = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
        //    AirLadingIds = String.Join(",", ListBoxAirBillNo.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
        //    if (custIds == "" && (string)Session["Customer_IDs"] != "")
        //        custIds = (string)Session["Customer_IDs"];
        //    if (AirLadingIds == "" && (string)Session["AirLading_IDs"] != "")
        //        AirLadingIds = (string)Session["AirLading_IDs"];
        //    if (Request.QueryString["ID"] != null)
        //    {
        //        Airway1 = AirLadingIds.Split(',').ToDictionary(key => key, value => value);

        //    }
        //    DataSet ds = new DataSet();
        //    if (Request.QueryString["ID"] != null)
        //    {
        //        LBSelectedItems();
        //    }

        //}

        private void LBSelectedItems()
        {
            try
            {
                string CustID = ""; string AirLadingIds = ""; string FPOIDs = "";

                if (Request.QueryString["ID"] != null)
                {
                    if (CustID1 != null && CustID1.Count > 0)
                    {
                        for (int i = 0; i < ListBoxCustomer.Items.Count; i++)
                            ListBoxCustomer.Items[i].Selected = CustID1.Where(Field => Field.Value.Trim().ToLower() == ListBoxCustomer.Items[i].Value.ToString().Trim().ToLower()).Any();
                    }
                }
                if (Airway1 != null && Airway1.Count > 0)
                {
                    for (int i = 0; i < ListBoxAirBillNo.Items.Count; i++)
                        ListBoxAirBillNo.Items[i].Selected = Airway1.Where(Field => Field.Value.Trim().ToLower() == ListBoxAirBillNo.Items[i].Value.ToString().Trim().ToLower()).Any();
                }
                if (FPOIDs1 != null && FPOIDs1.Count > 0)
                {
                    for (int i = 0; i < ListBoxFpo.Items.Count; i++)
                        ListBoxFpo.Items[i].Selected = FPOIDs1.Where(Field => Field.Value.Trim().ToLower() == ListBoxFpo.Items[i].Value.ToString().Trim().ToLower()).Any();
                }


                CustID = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                AirLadingIds = String.Join(",", ListBoxAirBillNo.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                FPOIDs = String.Join(",", ListBoxFpo.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        private void Customers()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ListBoxCustomer.DataSource = ds.Tables[0];
                    ListBoxCustomer.DataTextField = "Description";
                    ListBoxCustomer.DataValueField = "ID";
                    ListBoxCustomer.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "ShippingDocumentsCustomer", ex.Message.ToString());
            }

        }
        protected void ListBoxCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBoxCustmrSelected();
        }
        private void ListBoxCustmrSelected()
        {
            string custIds = ""; string AirLadingIds = "";

            custIds = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            AirLadingIds = String.Join(",", ListBoxAirBillNo.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

            //if (Request.QueryString["ID"] != null)
            //{
            //    custIds = (string)Session["Customer_IDs"];
            //    AirLadingIds = air;
            //    Airway1 = AirLadingIds.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
            //    FPOIDs1 = Session["FPO_IDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
            //    LBSelectedItems();

            //}
            DataSet ds = new DataSet();
            if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
            {
                ds = SDCBLL.GetAWBNO(Convert.ToChar("A"), custIds, AirLadingIds);
            }
            else
            {
                ds = SDCBLL.GetAWBNO(Convert.ToChar("X"), custIds, AirLadingIds);
            }
            ListBoxAirBillNo.Items.Clear();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                ListBoxAirBillNo.DataSource = ds.Tables[0];
                ListBoxAirBillNo.DataTextField = "AWBNumber";
                ListBoxAirBillNo.DataValueField = "AWBID";
                ListBoxAirBillNo.DataBind();
            }

        }
        protected void ListBoxAirBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBoxAirBillSelected();
        }
        private void ListBoxAirBillSelected()
        {
            string custIds = ""; string AirLadingIds = "";

            custIds = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            AirLadingIds = String.Join(",", ListBoxAirBillNo.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            //if (Request.QueryString["ID"] != null)
            //{
            //    custIds = (string)Session["Customer_IDs"];
            //    AirLadingIds = air;
            //    Airway1 = AirLadingIds.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
            //    FPOIDs1 = Session["FPO_IDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());

            //}

            DataSet ds = new DataSet();
            ds = SDCBLL.GetFPONO(Convert.ToChar("B"), custIds, AirLadingIds);
            ListBoxFpo.Items.Clear();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                ListBoxFpo.DataSource = ds.Tables[0];
                ListBoxFpo.DataTextField = "FPONumber";
                ListBoxFpo.DataValueField = "FPOIds";
                ListBoxFpo.DataBind();

            }


        }
        private void ClearAll()
        {
            // ddlAirLading.SelectedIndex = -1;
            ListBoxCustomer.SelectedIndex = -1;
            ListBoxAirBillNo.Items.Clear();
            ListBoxFpo.Items.Clear();
            CourierName.Text = "";
            CourierNo.Text = "";
            DocumentSent.Text = "";
            ReceivedPerson.Text = "";
            Documentcarrier.Text = "";
            //TextBox6.Text = "";


        }
        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }

        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    ddl.DataSource = CommonDt;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "ShippingDocumentsCustomer", ex.Message.ToString());
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {

            if (RecDate.Text == "")
            {
                RecDate.Text = "31-12-9999";
            }

            int res = 1; string custIds = ""; string AirLadingIds = ""; string FpoIds = ""; Boolean RecCour; Boolean RecPer; string Filename = FileName();
            custIds = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            AirLadingIds = String.Join(",", ListBoxAirBillNo.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            FpoIds = String.Join(",", ListBoxFpo.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

            if (InvNumber.Checked)
            {
                RecCour = true;
                RecPer = false;
                if (btnSave.Text == "Save")
                {
                    res = SDCBLL.InsertUpdateDeleteShipDocCust(Convert.ToChar("I"), Guid.Empty, custIds, AirLadingIds, FpoIds, RecCour, RecPer, CourierName.Text, CourierNo.Text, CommonBLL.DateInsert(RecDate.Text), DocumentSent.Text, ReceivedPerson.Text, Documentcarrier.Text, ContactNo.Text, new Guid(ddlCpnyLst.SelectedValue), new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, CommonBLL.DateInsert("31-12-9999"), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("../Invoices/ShippingDocCustStatus.aspx", false);
                    }
                    else if (res != 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "ShippingDocumentsCustomer", "Error while Inserting.");

                    }

                }
                else if (btnSave.Text == "Update")
                {
                    res = SDCBLL.InsertUpdateDeleteShipDocCust(Convert.ToChar("U"), new Guid(ViewState["ID"].ToString()), custIds, AirLadingIds, FpoIds, RecCour, RecPer, CourierName.Text, CourierNo.Text, CommonBLL.DateInsert(RecDate.Text), DocumentSent.Text, ReceivedPerson.Text, Documentcarrier.Text, ContactNo.Text, new Guid(ddlCpnyLst.SelectedValue), new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, CommonBLL.DateInsert("31-12-9999"), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("../Invoices/ShippingDocCustStatus.aspx", false);
                    }
                    else if (res != 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "ShippingDocumentsCustomer", "Error while Inserting.");

                    }
                }
            }
            else
            {
                RecCour = false;
                RecPer = true;
                if (btnSave.Text == "Save")
                {
                    res = SDCBLL.InsertUpdateDeleteShipDocCust(Convert.ToChar("I"), Guid.Empty, custIds, AirLadingIds, FpoIds, RecCour, RecPer, CourierName.Text, CourierNo.Text, CommonBLL.DateInsert(RecDate.Text), DocumentSent.Text, ReceivedPerson.Text, Documentcarrier.Text, ContactNo.Text, new Guid(ddlCpnyLst.SelectedValue), new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, CommonBLL.DateInsert("31-12-9999"), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("../Invoices/ShippingDocCustStatus.aspx", false);
                    }
                    else if (res != 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "ShippingDocumentsCustomer", "Error while Inserting.");

                    }

                }
                else if (btnSave.Text == "Update")
                {
                    res = SDCBLL.InsertUpdateDeleteShipDocCust(Convert.ToChar("U"), new Guid(ViewState["ID"].ToString()), custIds, AirLadingIds, FpoIds, RecCour, RecPer, CourierName.Text, CourierNo.Text, CommonBLL.DateInsert(RecDate.Text), DocumentSent.Text, ReceivedPerson.Text, Documentcarrier.Text, ContactNo.Text, new Guid(ddlCpnyLst.SelectedValue), Guid.Empty, DateTime.Now, new Guid(Session["UserID"].ToString()), CommonBLL.DateInsert("31-12-9999"), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("../Invoices/ShippingDocCustStatus.aspx", false);
                    }
                    else if (res != 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "ShippingDocumentsCustomer", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "ShippingDocumentsCustomer", "Error while Inserting.");

                    }
                }

            }

        }
        protected void btnclear_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Invoices/ShippingDocumentsCustomer.aspx");
        }

    }
}