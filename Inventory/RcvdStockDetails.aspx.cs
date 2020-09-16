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
using System.Collections.Generic;

namespace VOMS_ERP.Inventory
{
    public partial class RcvdStockDetails : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        StockDetailsBLL SDBL = new StockDetailsBLL();
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
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        //btnSearch.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            GetData();
                            txtFrmDt.Attributes.Add("readonly", "readonly");
                            txtToDt.Attributes.Add("readonly", "readonly");
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Bind Default Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                txtToDt.Text = txtFrmDt.Text = txtSuplrNm.Text = "";
                hfSuplrId.Value = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Request For Central Excise Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.DateFormat(txtFrmDt.Text), CommonBLL.DateFormat(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagGSelect, 0, 0, 0, CommonBLL.DateFormat(txtFrmDt.Text),
                //         CommonBLL.DateFormat(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.StartDate, CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.DateFormat(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.StartDate, CommonBLL.DateFormat(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagGSelect, 0, 0, 0,
                //        CommonBLL.DateFormat(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagGSelect, 0, 0, 0, CommonBLL.StartDate,
                //         CommonBLL.DateFormat(txtToDt.Text)));
                //else
                //    BindGridView(gvRcvdStock, GDNBL.SelectGdnDtls(CommonBLL.FlagSelectAll, 0, 0));
                BindGridView(gvRcvdStock, SDBL.SelectStockDetails(CommonBLL.FlagASelect, 0, true));
                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet Rceds)
        {
            try
            {
                if (Rceds.Tables.Count > 0 && Rceds.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = Rceds;
                    gview.DataBind();
                }
                else
                {
                    gview.DataSource = null;
                    gview.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRcvdStock_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;

                int lastCellIndex = e.Row.Cells.Count - 1;
                
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRcvdStock_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvRcvdStock.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblgdnID")).Text);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Accepting...');", true);
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRcvdStock_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvRcvdStock.UseAccessibleHeader = false;
                gvRcvdStock.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }
        #endregion

        #region Button Click Events

        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInputs();
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Inventory/ErrorLog"), "Received Stock Details", ex.Message.ToString());
            }
        }
        #endregion
    }
}
