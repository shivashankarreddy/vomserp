using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing.Imaging;

namespace VOMS_ERP
{
    public partial class CImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a random code and store it in the Session object.
            this.Session["CaptchaImageText"] = GenerateRandomCode();
            // Create a CAPTCHA image using the text stored in the Session object.
            RandomImage ci = new RandomImage(this.Session
                ["CaptchaImageText"].ToString(), 300, 75);
            // Change the response headers to output a JPEG image.
            this.Response.Clear();
            this.Response.ContentType = "image/jpeg";
            // Write the image to the response stream in JPEG format.
            ci.Image.Save(this.Response.OutputStream, ImageFormat.Jpeg);
            // Dispose of the CAPTCHA image object.
            ci.Dispose();
        }

        // Function to generate random string with Random class.
        private string GenerateRandomCode()
        {
            Random r = new Random();
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(0, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100, 1999);
            }
            return s;
        }
    }
}
