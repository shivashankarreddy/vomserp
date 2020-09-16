using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using BAL;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace VOMS_ERP
{
    public partial class Tracker : System.Web.UI.Page
    {
        ErrorLog ELog = new ErrorLog();
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Tracker));
        }

        #region GetDataMethod
        /// <summary>
        /// WebMethod to get the values for filling the grid
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public static string GetData()
        {
            string jsonText = string.Empty;
            try
            {
                string outputJson = string.Empty;
                var sb = new StringBuilder();
                string ServiceUris = string.Empty;
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string FE = HttpContext.Current.Request.Params["sSearch_1"];
                string MFFENo = HttpContext.Current.Request.Params["sSearch_2"];
                string MFFEDate = HttpContext.Current.Request.Params["sSearch_3"];
                string FPO = HttpContext.Current.Request.Params["sSearch_4"];
                string MFFPONo = HttpContext.Current.Request.Params["sSearch_5"];
                string MFFPODate = HttpContext.Current.Request.Params["sSearch_6"];
                DataSet ds = new DataSet();

                ds = CommonBLL.GetTrackerData(CommonBLL.FlagSelectAll, FE ?? "", MFFENo ?? "", MFFEDate ?? "", FPO ?? "", MFFPONo ?? "", MFFPODate ?? "");

                int TotalRowCount = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    TotalRowCount = Convert.ToInt32(ds.Tables[0].Rows.Count);
                    DataTable data = ds.Tables[0];

                    //foreach (DataColumn DC in data.Columns)
                    //{
                    //    foreach (DataRow DR in data.Rows)
                    //    {
                    //        if (DR[DC].GetType() != typeof(System.DBNull))
                    //        {
                    //            string D = Convert.ToString(DR[DC] ?? "").Replace("12:00:00 AM", "");
                    //            var dkkd = ((DateTime)DR[DC]).ToString("dd-mm-yyy");
                    //            DR[DC] = dkkd;
                    //            data.AcceptChanges();
                    //        }
                    //    }

                    //}


                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        sb.Append("{");
                        sb.AppendFormat(@"""DT_RowId"": ""{0}""", data.Rows[i]["FEID"].ToString());
                        sb.Append(",");
                        sb.AppendFormat(@"""DT_RowClass"": ""{0}""", 2);
                        sb.Append(",");

                        sb.AppendFormat(@"""0"": ""{0}""", data.Rows[i]["ENQNO"].ToString());
                        sb.Append(",");

                        sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data.Rows[i]["Fe Received Date"]);
                        sb.Append(",");

                        sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data.Rows[i]["FE Created Date"]); //+ ",type: 'date-dd-mmm-yyyy', targets: 0 \"");
                        sb.Append(",");

                        sb.AppendFormat(@"""3"": ""{0}""", data.Rows[i]["FE No in VOMS"].ToString());
                        sb.Append(",");

                        sb.AppendFormat(@"""4"": ""{0}""", data.Rows[i]["FQ No"].ToString());
                        sb.Append(",");

                        sb.AppendFormat(@"""5"": ""{0:dd-MM-yyyy}""", data.Rows[i]["FQ Created DATE"]);
                        sb.Append(",");

                        sb.AppendFormat(@"""6"": ""{0}""", data.Rows[i]["FPO received in email"].ToString());
                        sb.Append(",");

                        sb.AppendFormat(@"""7"": ""{0}""", data.Rows[i]["FPO create in system"].ToString());
                        sb.Append(",");

                        sb.AppendFormat(@"""8"": ""{0:dd-MM-yyyy}""", data.Rows[i]["FPO received Date"]);
                        sb.Append(",");

                        sb.AppendFormat(@"""9"": ""{0:dd-MM-yyyy}""", data.Rows[i]["FPO Created DATE"]);
                        sb.Append(",");

                        sb.AppendFormat(@"""10"": ""{0}""", data.Rows[i]["LPONO"].ToString());
                        sb.Append(",");

                        sb.AppendFormat(@"""11"": ""{0:dd-MM-yyyy}""", data.Rows[i]["LPO Created DATE"]);
                        sb.Append(",");
                        sb.AppendFormat(@"""12"": ""{0}""", data.Rows[i]["No of days to create FE in VOMS"].ToString());
                        sb.Append(",");
                        sb.AppendFormat(@"""13"": ""{0}""", data.Rows[i]["No of days to float FE to FQ"].ToString());
                        sb.Append(",");
                        sb.AppendFormat(@"""14"": ""{0}""", data.Rows[i]["No of days FQ to FPO"].ToString());
                        sb.Append(",");
                        sb.AppendFormat(@"""15"": ""{0}""", data.Rows[i]["No of Days FPO to LPO"].ToString());
                        sb.Append(",");
                        sb.AppendFormat(@"""16"": ""{0}""", data.Rows[i]["FeStatus"].ToString());
                        sb.Append(",");
                        sb.AppendFormat(@"""17"": ""{0}""", data.Rows[i]["Created By"].ToString());
                        sb.Append("},");


                    }
                }
                outputJson = sb.ToString();
                if (sb.Length > 0)
                    outputJson = sb.Remove(sb.Length - 1, 1).ToString();
                sb.Clear();
                sb.Append("{");
                sb.Append(@"""sEcho"": ");
                sb.AppendFormat(@"""{0}""", sEcho);
                sb.Append(",");
                sb.Append(@"""iTotalRecords"": ");
                sb.Append(TotalRowCount);
                sb.Append(",");
                sb.Append(@"""iTotalDisplayRecords"": ");
                sb.Append(TotalRowCount);
                sb.Append(", ");
                sb.Append(@"""aaData"": [ ");
                sb.Append(outputJson);
                sb.Append("]}");
                outputJson = sb.ToString();
                return outputJson;
            }

            catch (Exception ex)
            {
                return null;
                throw ex;
            }

        }

        /// <summary>
        /// To Int Conversioin for converting string to integer
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }

        #endregion

        protected void IMG_Btn_Export_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string FE = Txt_MFFESearchNo.Text;
                string MFFENo = Txt_MFFENo.Text;
                string MFFEDate = Txt_MFFEDate.Text;
                string FPO = Txt_MFFPOSearchFPO.Text;
                string MFFPONo = Txt_MFFPONO.Text;
                string MFFPODate = Txt_MFFPODate.Text;

                DataSet ds = new DataSet();

                ds = CommonBLL.GetTrackerData(CommonBLL.FlagSelectAll, FE ?? "", MFFENo ?? "", MFFEDate ?? "", FPO ?? "", MFFPONo ?? "", MFFPODate ?? "");

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=ForeignEnquirystatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);


                    DataGrid dgGrid = new DataGrid();
                    dgGrid.DataSource = ds.Tables[0];
                    dgGrid.DataBind();
                    Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                    dgGrid = t.Item2;
                    dgGrid.RenderControl(htextw);
                    Response.Write(t.Item1);

                    //string headerTable = "<img src='" + CommonBLL.CommonAdminLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
                    //Response.Write(headerTable);

                    Response.Write(stw.ToString().Replace("12:00:00 AM", ""));
                    Response.End();
                }
            }

            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }


        protected void Btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                #region UNUSED
                
                //string FE = Txt_MFFESearchNo.Text;
                //string MFFENo = Txt_MFFENo.Text;
                //string MFFEDate = Txt_MFFEDate.Text;
                //string FPO = Txt_MFFPOSearchFPO.Text;
                //string MFFPONo = Txt_MFFPONO.Text;
                //string MFFPODate = Txt_MFFPODate.Text;

                //string DummyFEDate = MFFEDate;
                //String DummyFPODate = Txt_MFFPODate.Text;

                //for (int i = 1; i < MFFENo.Split(',').Count(); i++) { MFFEDate += "," + DummyFEDate; }

                //for (int i = 1; i < MFFPONo.Split(',').Count(); i++) { MFFPODate += "," + DummyFPODate; }
                #endregion

                string[] SFEs = Txt_MFFESearchNo.Text.Split(',');
                string[] SMFFENos = Txt_MFFENo.Text.Split(',');
                string[] SMFFEDates = Txt_MFFEDate.Text.Split(',');
                string[] SFPOs = Txt_MFFPOSearchFPO.Text.Split(',');
                string[] SMFFPONos = Txt_MFFPONO.Text.Split(',');
                string[] SMFFPODates = Txt_MFFPODate.Text.Split(',');

                List<FEKeywords> _SLFE = new List<FEKeywords>();
                List<FPOKeywords> _SLFPO = new List<FPOKeywords>();

                for (int i = 0; i < SMFFENos.Count(); i++)
                {
                    _SLFE.Add(new FEKeywords() { MFFENO = SMFFENos[i], MFFEDate = SMFFEDates[0], FESearchNO = SFEs[i] });
                }
                for (int i = 0; i < SMFFPONos.Count(); i++)
                {
                    _SLFPO.Add(new FPOKeywords() { MFFPONO = SMFFPONos[i], MFFEPODate = SMFFPODates[0], FPOSearchNo = SFPOs[i] });
                }


                List<FEKeywords> _DLFE = new List<FEKeywords>();
                List<FPOKeywords> _DLFPO = new List<FPOKeywords>();
                DataSet _DS = CommonBLL.GetTrackerData(CommonBLL.FlagGSelect, "", "", "", "", "", "");
                if (_DS != null && _DS.Tables != null)
                {
                    string[] DFEs = Convert.ToString(_DS.Tables[0].Rows[0]["search_feno"]).Split(',');
                    string[] DMFFENos = Convert.ToString(_DS.Tables[0].Rows[0]["mffe_no"]).Split(',');
                    string[] DMFFEDates = Convert.ToString(_DS.Tables[0].Rows[0]["mffe_date"]).Split(',');
                    string[] DFPOs = Convert.ToString(_DS.Tables[0].Rows[0]["search_fpono"]).Split(',');
                    string[] DMFFPONos = Convert.ToString(_DS.Tables[0].Rows[0]["mffpo_no"]).Split(',');
                    string[] DMFFPODates = Convert.ToString(_DS.Tables[0].Rows[0]["mffpo_date"]).Split(',');
                    for (int i = 0; i < DMFFENos.Count(); i++)
                    {
                        _DLFE.Add(new FEKeywords() { MFFENO = DMFFENos[i], MFFEDate = DMFFEDates[i], FESearchNO = DFEs[i] });
                    }
                    for (int i = 0; i < DMFFPONos.Count(); i++)
                    {
                        _DLFPO.Add(new FPOKeywords() { MFFPONO = DMFFPONos[i], MFFEPODate = DMFFPODates[i], FPOSearchNo = DFPOs[i] });
                    }
                }

                var FEKeywords = (from SN in _SLFE
                                  where !_DLFE.Select(C => C.MFFENO.ToLower()).Contains(SN.MFFENO.ToLower()) && SN.MFFENO.Length > 0 && SN.MFFENO != null
                                  select SN).Union(_DLFE).ToList();

                var FPOKeywords = (from SN in _SLFPO
                                   where !_DLFPO.Select(C => C.MFFPONO.ToLower()).Contains(SN.MFFPONO.ToLower()) && SN.MFFPONO.Length > 0 && SN.MFFPONO != null
                                   select SN).Union(_DLFPO).ToList();

                CommonBLL.GetTrackerData(CommonBLL.FlagUpdate, string.Join(",", FEKeywords.Select(C => C.FESearchNO).ToList()),
                                                                string.Join(",", FEKeywords.Select(C => C.MFFENO).ToList()),
                                                                string.Join(",", FEKeywords.Select(C => C.MFFEDate).ToList()),
                                                                string.Join(",", FPOKeywords.Select(C => C.FPOSearchNo).ToList()),
                                                                string.Join(",", FPOKeywords.Select(C => C.MFFPONO).ToList()),
                                                                string.Join(",", FPOKeywords.Select(C => C.MFFEPODate).ToList()));

                Txt_MFFESearchNo.Text = Txt_MFFENo.Text = Txt_MFFEDate.Text = Txt_MFFPOSearchFPO.Text = Txt_MFFPONO.Text = Txt_MFFPODate.Text = "";

                #region UNUSED

                //XElement _MXE = new XElement("TrackerData");

                //for (int i = 0; i < MFFENos.Count(); i++)
                //{
                //    _MXE.Add(new XElement("Sno", new XElement("MFFENO", MFFENos[i]),
                //                            new XElement("MFFEDate", MFFEDates[0]),
                //                            new XElement("FE", FEs[i]),
                //                            new XElement("MFFPONo", MFFPONos[i]),
                //                            new XElement("MFFPODate", MFFPODates[0]),
                //                            new XElement("FPO", FPOs[i])));

                //}

                //DataSet ds = new DataSet();

                //ds = CommonBLL.GetTrackerData(CommonBLL.FlagUpdate, FE == "" ? "" : FE + ",", MFFENo == "" ? "" : MFFENo + ",",
                //    MFFEDate == "" ? "" : MFFEDate + ",", FPO == "" ? "" : FPO + ",",
                //    MFFPONo == "" ? "" : MFFPONo + ",", MFFPODate == "" ? "" : MFFPODate + ",");

                #endregion

                GetData();
            }

            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }

        protected void Btn_Clear_Click(object sender, EventArgs e)
        {
            Txt_MFFESearchNo.Text = Txt_MFFENo.Text = Txt_MFFEDate.Text = Txt_MFFPOSearchFPO.Text = Txt_MFFPONO.Text = Txt_MFFPODate.Text = "";
        }

        class FEKeywords
        {
            public string MFFENO { get; set; }
            public string MFFEDate { get; set; }
            public string FESearchNO { get; set; }
        }
        class FPOKeywords
        {
            public string MFFPONO { get; set; }
            public string MFFEPODate { get; set; }
            public string FPOSearchNo { get; set; }
        }
    }
}
