using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using BAL;

namespace VOMS_ERP.Admin
{
    /// <summary>
    /// Summary description for ShowImg
    /// </summary>
    public class ShowImg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Guid empno;
                if (context.Request.QueryString["id"] != null)
                    empno = new Guid(context.Request.QueryString["id"]);
                else
                    throw new ArgumentException("No parameter specified");
                if (empno != Guid.Empty)
                {
                    context.Response.ContentType = "image/jpeg";
                    Stream strm = ShowEmpImage(empno);
                    byte[] buffer = new byte[4096];
                    int byteSeq = strm.Read(buffer, 0, 4096);
                    while (byteSeq > 0)
                    {
                        context.Response.OutputStream.Write(buffer, 0, byteSeq);
                        byteSeq = strm.Read(buffer, 0, 4096);
                    }
                    //context.Response.BinaryWrite(buffer);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        public Stream ShowEmpImage(Guid empno)
        {
            object img = new object();
            CompanyBLL cmpBLL = new CompanyBLL();
            DataSet ds = new DataSet();
            ds = cmpBLL.SelectCompany(CommonBLL.FlagBSelect, empno);
            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["CompanyLogo"] != null)
            {
                img = ds.Tables[0].Rows[0]["CompanyLogo"];//cmd.ExecuteScalar();
            }
            try
            {
                return new MemoryStream((byte[])img);
            }
            catch
            {
                return null;
            }
            finally
            {
                //connection.Close();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}