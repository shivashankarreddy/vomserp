<%@ WebHandler Language="C#" Class="VOMS_ERP.Upload" %>

using System;
using System.Web;
using System.IO;
using BAL;

namespace VOMS_ERP
{
    public class Upload : IHttpHandler
    {
        CommonBLL cbll = new CommonBLL();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Expires = -1;
            try
            {
                HttpPostedFile postedFile = context.Request.Files["Filedata"];                
                string savepath = "";
                string tempPath = "";
                tempPath = System.Configuration.ConfigurationManager.AppSettings["FolderPath"];
                savepath = context.Server.MapPath(tempPath);
                string filename = postedFile.FileName;
                if (!Directory.Exists(savepath))
                    Directory.CreateDirectory(savepath);
                cbll.UploadedFiles(filename, null);
                postedFile.SaveAs(context.Server.MapPath(@"uploads\" + filename));
                context.Response.Write(tempPath + "/" + filename);
                context.Response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                context.Response.Write("Error: " + ex.Message);
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