using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.IO;
using BAL;

namespace VOMS_ERP.Enquiries
{
    public partial class ExcelImport : System.Web.UI.Page
    {
        EnumMasterBLL embal = new EnumMasterBLL();
        ItemMasterBLL ItmMstr = new ItemMasterBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        private DataSet ReadExcelData()
        {
            try
            {
                string FilePath = "";
                if (FileUpload1.HasFile)
                {
                    string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                    string FolderPath = ConfigurationManager.AppSettings["FolderPath"];

                    FilePath = MapPath("~/uploads/" + FileName); //Server.MapPath(FolderPath + FileName);
                    FileUpload1.SaveAs(FilePath);
                }
                string sConnection = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                OleDbConnection dbCon = new OleDbConnection(sConnection);
                dbCon.Open();
                DataTable dtSheetName = dbCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataSet dsOutput = new DataSet();
                for (int nCount = 0; nCount < dtSheetName.Rows.Count; nCount++)
                {
                    string sSheetName = dtSheetName.Rows[nCount]["TABLE_NAME"].ToString();
                    string sQuery = "Select * From [" + sSheetName + "]";
                    OleDbCommand dbCmd = new OleDbCommand(sQuery, dbCon);
                    OleDbDataAdapter dbDa = new OleDbDataAdapter(dbCmd);
                    DataTable dtData = new DataTable();
                    dtData.TableName = sSheetName;
                    dbDa.Fill(dtData);
                    dsOutput.Tables.Add(dtData);
                }
                dbCon.Close();
                return dsOutput;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return null;
            }
        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblDisplay.Text = "";
                btnShow.Text = ListBox1.SelectedItem.Value;

                var list1 = new string[] { "1", "2", "3", "4", "5", "6" };
                var list2 = new string[] { "2", "3", "4" };
                var listCommon = list1.Intersect(list2);

                foreach (string s in listCommon)
                    lblDisplay.Text += s + ",";
                lblDisplay.Text += "<br/>";

                var listCommon1 = list1.Except(list2);
                foreach (string s in listCommon1)
                    lblDisplay.Text += s + ",";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = ReadExcelData();

                if (ds != null && ds.Tables.Count > 2)
                {
                    if (ds.Tables.Contains("Items$") && ds.Tables.Contains("Header$"))
                    {
                        DataSet dsUnits = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                        DataTable dtCloned = ds.Tables["Items$"].Clone();
                        dtCloned.Columns["Sno"].DataType = typeof(Int32);
                        dtCloned.Columns["Item Description"].DataType = typeof(string);
                        dtCloned.Columns["Part No"].DataType = typeof(string);
                        dtCloned.Columns["Specification"].DataType = typeof(string);
                        dtCloned.Columns["Make"].DataType = typeof(string);
                        dtCloned.Columns["Quantity"].DataType = typeof(decimal);
                        dtCloned.Columns["Units"].DataType = typeof(string);
                        //Column Added
                        dtCloned.Columns.Add("UnumsID", typeof(Guid));
                        dtCloned.Columns.Add("ItemId", typeof(Guid));

                        Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                        int RowId = 0;
                        Guid ItemCatagory = Guid.Empty;
                        Guid ItemID = Guid.Empty;
                        foreach (DataRow row in ds.Tables["Items$"].Rows)
                        {
                            dtCloned.ImportRow(row);
                            string Desc = row["Item Description"].ToString();
                            string PartNo = row["Part No"].ToString();
                            string Spec = row["Specification"].ToString();
                            string Make = row["Make"].ToString();

                            DataSet dss = embal.EnumMasterSelectforDescription(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory);
                            if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                            {
                                ItemCatagory = new Guid(dss.Tables[0].Rows[0][0].ToString());

                                #region Inserting Item
                                string itemCode = "";
                                if (Desc.Trim().Length > 8)
                                    itemCode = (ItemCatagory + (Desc.Trim()).Substring(0, 8));
                                else
                                    itemCode = (ItemCatagory + (Desc.Trim()));

                                // check whether Item Exists or Not
                                DataSet ItmExsts = ItmMstr.InsertUpdateItemMasterWithID(CommonBLL.FlagYSelect, Guid.Empty, ItemCatagory,"",
                                    Desc, Spec, PartNo, itemCode, new Guid(Session["UserID"].ToString()), "", false, new Guid(Session["CompanyID"].ToString()));
                                if (ItmExsts != null && ItmExsts.Tables.Count > 0 && ItmExsts.Tables[0].Rows.Count > 0)
                                {
                                    Codes.Add(RowId, new Guid(ItmExsts.Tables[0].Rows[0][0].ToString()));
                                    ItemID = new Guid(ItmExsts.Tables[0].Rows[0][0].ToString());
                                }
                                else
                                {
                                    DataSet result = ItmMstr.InsertUpdateItemMasterWithID(CommonBLL.FlagNewInsert, Guid.Empty, ItemCatagory,"",
                                        Desc, Spec, PartNo, itemCode, new Guid(Session["UserID"].ToString()), "", false, new Guid(Session["CompanyID"].ToString()));
                                    if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                                    {
                                        Codes.Add(RowId, new Guid(result.Tables[0].Rows[0][0].ToString()));
                                        ItemID = new Guid(result.Tables[0].Rows[0][0].ToString());
                                    }
                                }
                                string U = row["Units"].ToString();
                                if (dsUnits != null && dsUnits.Tables.Count > 0 && dsUnits.Tables[0].Rows.Count > 0)
                                {
                                    DataRow[] units = dsUnits.Tables[0].Select("Description = '" + row["Units"].ToString() + "'");
                                    if (units.Length == 0)
                                        units = dsUnits.Tables[0].Select("Description = 'No(s)'");
                                    if (units.Length > 0)
                                        dtCloned.Rows[RowId]["UnumsID"] = new Guid(units[0]["ID"].ToString());
                                }
                                dtCloned.Rows[RowId]["ItemId"] = ItemID;
                                #endregion

                                RowId++;
                            }
                        }
                        if (Codes != null && Codes.Count > 0)
                        {
                            HttpContext.Current.Session["SelectedItems"] = Codes;
                            Session["HFITemsValues"] = string.Join(",", Codes.ToArray().Select(o => o.Value.ToString().Trim()).ToArray()).ToString();
                        }

                        Session["BulkUploads"] = dtCloned;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Show All", "SendSelectedVal(true);", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }
    }
}