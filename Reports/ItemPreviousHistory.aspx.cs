using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using Ajax;
using System.Text;

namespace VOMS_ERP.Reports
{
    public partial class ItemPreviousHistory : System.Web.UI.Page
    {
        # region variables

        int res;
        ErrorLog ELog = new ErrorLog();
        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(ItemPreviousHistory));
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
        }

        #region Methods
        
        /// Method To Export Table
        /// </summary>
        /// <param name="ds"></param>
        public string Export_Report_Excel(DataSet ds)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder sb = new StringBuilder();
                sb.Append("<table id=ExportReport width='100%'> <tr><td colspan=5>");
                sb.Append("<table id=ExportReport1 width='100%' border = '1px'>");
                sb.Append("<tr><th>SNo</th><th>Item Description</th><th>Supplier</th><th>Quotation Number</th><th>Rate</th>" +
                    "<th>LPO Number</th><th>Rate</th></tr>");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    sb.Append("<tr><td align='middle' style= 'width:15% !important'>" + (i + 1) + "</td>" +
                        "<td align='middle' style= 'width:85% !important'>" + ds.Tables[0].Rows[i]["ItemDescription"].ToString() + "</td>"
                        + "<td align='middle' style= 'width:85% !important'>" + ds.Tables[0].Rows[i]["Supplier"].ToString() + "</td>" +
                        "<td style= 'width:80% !important'>" + ds.Tables[0].Rows[i]["Quotationnumber"].ToString() + "</td>" +
                        "<td align='center' style= 'width:10% !important'>" + ds.Tables[0].Rows[i]["Rate"].ToString() + "</td>"
                        + "<td align='center' style= 'width:10% !important'>" + ds.Tables[0].Rows[i]["LocalPurchaseOrderNo"].ToString() + "</td>" +
                        "<td align='center' style= 'width:10% !important'>" + ds.Tables[0].Rows[i]["Rate1"].ToString() + "</td></tr>");
                }
                sb.Append("</table> </td></tr></table>");
                string res = sb.ToString();
                return res;
            }
            catch (Exception ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(HttpContext.Current.Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion

        #region Button Click Events

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtItmDscrip.Text = txtItmPrtNmbr.Text = txtspec.Text = "";
        }

        #endregion

        #region WebMethods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ReportExp(string Id)
        {
            string filePath = System.IO.Path.GetFullPath(Server.MapPath("test12345.xlsx"));
            System.IO.FileInfo targetFile = new System.IO.FileInfo(filePath);
            try
            {
                DataSet ds = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagQSelect, Guid.Empty, new Guid(Id), Guid.Empty, Guid.Empty,
                    Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now,
                    true, Guid.Empty);
                string ExcelData = "";
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ExcelData = Export_Report_Excel(ds);
                    return ExcelData;
                }
                return ExcelData;
                #region Not in Use

                //string attachment = "attachment; filename=city.xls";
                //HttpContext.Current.Response.ClearContent();
                //HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                //HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                //string tab = "";
                //foreach (DataColumn dc in ds.Tables[0].Columns)
                //{
                //    HttpContext.Current.Response.Write(tab + dc.ColumnName);
                //    tab = "\t";
                //}
                //HttpContext.Current.Response.Write("\n");
                //int i;
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    tab = "";
                //    for (i = 0; i < ds.Tables[0].Columns.Count; i++)
                //    {
                //        HttpContext.Current.Response.Write(tab + dr[i].ToString());
                //        tab = "\t";
                //    }
                //    HttpContext.Current.Response.Write("\n");
                //}
                //HttpContext.Current.Response.Flush();
                //StringBuilder stringBuilder = new StringBuilder();
                //ds.Tables[0].Rows.Cast<DataRow>().ToList().ForEach(dataRow =>
                //{
                //    ds.Tables[0].Columns.Cast<DataColumn>().ToList().ForEach(column =>
                //    {
                //        stringBuilder.AppendFormat("{0}:{1} ", column.ColumnName, dataRow[column]);
                //    });
                //    stringBuilder.Append(Environment.NewLine);
                //});
                //string attachment = "attachment; filename=ForeignEnquirystatus.xls";
                //HttpContext.Current.Response.ClearContent();
                //HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                //HttpContext.Current.Response.ContentType = "application/ms-excel";
                //StringWriter stw = new StringWriter();
                //HtmlTextWriter htextw = new HtmlTextWriter(stw);



                //DataGrid dgGrid = new DataGrid();
                //dgGrid.DataSource = ds.Tables[0];
                //dgGrid.DataBind();
                //Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                //dgGrid = t.Item2;
                //dgGrid.RenderControl(htextw);
                //HttpContext.Current.Response.Write(t.Item1);


                //HttpContext.Current.Response.Write(stw.ToString());
                //HttpContext.Current.Response.End();

                //return "";
                #endregion
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #endregion
      
    }
}