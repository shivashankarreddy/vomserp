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
using BAL;

namespace VOMS_ERP
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        DataSet dsScreens = new DataSet();
        RoleActionBLL RABll = new RoleActionBLL();
        ModuleAccessBLL MABL = new ModuleAccessBLL();
        private ArrayList childParentRelationship = null;
        private SiteMapNode rootNode = null;
        private ArrayList siteMapNodes = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] != null || Request.QueryString["SupID"] != null)
                {
                    MenuItem mnItem = null;
                    if (!IsPostBack)
                    {
                        if (new Guid(Session["CompanyID"].ToString()) != Guid.Empty) { }
                        CmpLogo.ImageUrl = "Admin/ShowImg.ashx?id=" + new Guid(Session["CompanyID"].ToString());
                        //else
                        //    CmpLogo.ImageUrl = Server.MapPath("//images/Logos/defaultimg.jpg"); 
                        Session["MenuItem"] = null;
                        AddEventHandlers();
                        AddPageContent();

                    }
                    // Session["UserID"] = "1";
                    if (Session["UserID"] != null)
                        dsScreens = MABL.ModuleAccess(CommonBLL.FlagCSelect, new Guid(Session["CompanyID"].ToString()));
                    //RABll.GetScreens(new Guid(Session["UserID"].ToString()));

                    Menu mnu = (Menu)this.FindControl("NavigationMenu");
                    if (Session["MenuItem"] != null)
                        mnItem = (MenuItem)Session["MenuItem"];
                    // mnu.Visible = false;
                    username.Text = Session["UserName"].ToString();
                }
                else
                    Response.Redirect("../Login.aspx?logout=no", false);
            }
            catch (Exception)
            {
            }
        }

        private void HideMenuItems()
        {
            foreach (MenuItem mi in NavigationMenu.Items)
            {
                foreach (MenuItem li in mi.ChildItems)
                {
                    if (li.ChildItems.Count > 0)
                    {
                        foreach (MenuItem ci in li.ChildItems)
                        {
                            if (ci.NavigateUrl == "")
                            {
                                //Menu m = (Menu)NavigationMenu;
                                NavigationMenu.Items.Remove(NavigationMenu.FindItem(ci.ValuePath));

                                //li.ChildItems.Remove(ci);
                            }
                        }
                    }
                    //string r = mi.DataItem.ToString();
                }
            }
        }


        protected override void AddedControl(Control control, int index)
        {
            // This is necessary because Safari and Chrome browsers don't display the Menu control correctly.
            // Add this to the code in your master page.
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
                this.Page.ClientTarget = "uplevel";

            base.AddedControl(control, index);
        }

        private void AddPageContent()
        {
            string pageName = HttpContext.Current.Request.Url.AbsolutePath.Substring(
                         HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") + 1);

            ContentPlaceHolder contentPlaceHolder = (ContentPlaceHolder)this.Page.Master.FindControl("ContentPlaceHolder1");
            Label label = new Label();
            //label.Text = " <br /> Content for page: " + pageName;
            //contentPlaceHolder.Controls.Add(label);
        }

        private void AddEventHandlers()
        {
            NavigationMenu.MenuItemDataBound += new MenuEventHandler(NavigationMenu_MenuItemDataBound);
            SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);
            SiteMapPath1.ItemCreated += new SiteMapNodeItemEventHandler(SiteMapPath1_ItemCreated);
            //NavigationMenu.Unload += new EventHandler(NavigationMenu_Unload);
        }

        public void RemoveMenuItem()
        {
            Menu menu = this.FindControl("SiteMapPath1") as Menu;

            foreach (MenuItem Item in menu.Items)
            {
                if (Item.Text.Contains("User Management"))
                {
                    string str = Item.ChildItems[0].Text;
                    Item.ChildItems.RemoveAt(0);
                }
            }
        }


        protected void SiteMapPath1_ItemCreated(object sender, SiteMapNodeItemEventArgs e)
        {
            if (e.Item.ItemType == SiteMapNodeItemType.Root)
            {
                SiteMapPath1.PathSeparator = " ";
            }
            else
            {
                SiteMapPath1.PathSeparator = " >> ";
            }
        }

        //http://msdn.microsoft.com/en-us/library/system.web.sitemap.sitemapresolve.aspx
        //solve > SiteMapNode is readonly, property Title cannot be modified. 

        SiteMapNode SiteMap_SiteMapResolve(object sender, SiteMapResolveEventArgs e)
        {
            childParentRelationship = new ArrayList();
            siteMapNodes = new ArrayList();
            if (SiteMap.CurrentNode != null)
            {
                SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);
                SiteMapNode tempNode = currentNode;
                if (tempNode != null)
                {
                    childParentRelationship.Add(new DictionaryEntry(tempNode.Url, tempNode));
                    siteMapNodes.Add(new DictionaryEntry(tempNode.Url, tempNode));
                    if (rootNode == null)
                        rootNode = tempNode;
                }
                SiteMapNodeCollection t1 = null;
                t1 = GetChildNodes(tempNode);
                tempNode = ReplaceNodeText(tempNode);

                return currentNode;
            }

            return null;
        }

        //remove <u></u> tag recursively
        internal SiteMapNode ReplaceNodeText(SiteMapNode smn)
        {
            //current node
            if (smn != null && smn.Title.Contains("<u>"))
            {
                smn.Title = smn.Title.Replace("<u>", "").Replace("</u>", "");
            }

            //parent nd
            if (smn.ParentNode != null)
            {
                if (smn.ParentNode.Title.Contains("<u>"))
                {
                    SiteMapNode gpn = smn.ParentNode;
                    smn.ParentNode.Title = smn.ParentNode.Title.Replace("<u>", "").Replace("</u>", "");
                    smn = ReplaceNodeText(gpn);
                }
            }
            return smn;
        }

        void NavigationMenu_MenuItemDataBound(object sender, MenuEventArgs e)
        {
            try
            {
                if (HttpContext.Current.Session["AccessRole"].ToString() != "")
                {
                    if (HttpContext.Current.Session["AccessRole"].ToString() == "Customer")
                    {
                        if (e.Item.DataItem != null)
                        {
                            if (((SiteMapNode)e.Item.DataItem).Roles.Contains("Admin"))
                            {
                                if (e.Item.Parent != null)
                                    e.Item.Parent.ChildItems.Remove(e.Item);
                                else
                                    NavigationMenu.Items.Remove(e.Item);

                                //if (e.Item.Parent != null)
                                //    e.Item.Enabled = false;
                                ////e.Item.Parent.ChildItems.Remove(e.Item);

                                //else
                                //{
                                //    e.Item.Enabled = false;
                                //    //   NavigationMenu.Items.Remove(e.Item);
                                //}
                            }
                        }
                    }
                }
                else
                    Response.Redirect("../login.aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
            }
            #region OLD COde before Customer Access

            //try
            //{
            //    SiteMapNode node = (SiteMapNode)e.Item.DataItem;
            //    if (node.Description == "Company Details" && Session["AccessRole"].ToString() != CommonBLL.SuperAdminRole)
            //    {
            //        e.Item.Parent.ChildItems.Remove(e.Item);
            //    }
            //    if (node["target"] != null)
            //    {
            //        e.Item.Target = node["target"];
            //    }
            //    if (node["accesskey"] != null)
            //    {
            //        CreateAccessKeyButton(node["accesskey"] as string, node.Url);
            //    }
            //    if (dsScreens != null)
            //    {
            //        if (dsScreens.Tables[0].Rows.Count > 0)
            //        {
            //            for (int i = 0; i < dsScreens.Tables[0].Rows.Count; i++)
            //            {
            //                if (node.Description == dsScreens.Tables[0].Rows[i]["ScreenName"].ToString())
            //                {
            //                    e.Item.Parent.ChildItems.Remove(e.Item);
            //                    if (e.Item.Parent.ChildItems.Count == 0)
            //                    {

            //                    }
            //                }
            //            }
            //        }
            //    }
            //    Session["MenuItem"] = e.Item;
            //    //HideMenuItems();
            //}
            //catch (Exception ex)
            //{
            //}
            #endregion
        }

        //create access key button
        void CreateAccessKeyButton(string ak, string url)
        {
            HtmlButton inputBtn = new HtmlButton();
            inputBtn.Style.Add("width", "1px");
            inputBtn.Style.Add("height", "1px");
            inputBtn.Style.Add("position", "absolute");
            inputBtn.Style.Add("left", "-2555px");
            inputBtn.Style.Add("z-index", "-1");
            inputBtn.Attributes.Add("type", "button");
            inputBtn.Attributes.Add("value", "");
            inputBtn.Attributes.Add("accesskey", ak);
            inputBtn.Attributes.Add("onclick", "navigateTo('" + url + "');");

            AccessKeyPanel.Controls.Add(inputBtn);
        }

        // Implement the GetChildNodes method. 
        SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {
            SiteMapNodeCollection children = new SiteMapNodeCollection();
            // Iterate through the ArrayList and find all nodes that have the specified node as a parent. 
            lock (this)
            {
                for (int i = 0; i < childParentRelationship.Count; i++)
                {

                    string nodeUrl = ((DictionaryEntry)childParentRelationship[i]).Key as string;

                    SiteMapNode parent = GetNode(childParentRelationship, nodeUrl);

                    if (parent != null && node.Url == parent.Url)
                    {
                        // The SiteMapNode with the Url that corresponds to nodeUrl 
                        // is a child of the specified node. Get the SiteMapNode for 
                        // the nodeUrl.
                        SiteMapNode child = FindSiteMapNode(nodeUrl);
                        if (child != null)
                        {
                            children.Add(child as SiteMapNode);
                        }
                        else
                        {
                            throw new Exception("ArrayLists not in sync.");
                        }
                    }
                }
            }
            return children;
        }

        // Implement the RootNode property. 
        SiteMapNode RootNode
        {
            get
            {
                return rootNode;
            }
        }

        // Implement the FindSiteMapNode method. 
        SiteMapNode FindSiteMapNode(string rawUrl)
        {

            // Does the root node match the URL? 
            if (RootNode.Url == rawUrl)
            {
                return RootNode;
            }
            else
            {
                SiteMapNode candidate = null;
                // Retrieve the SiteMapNode that matches the URL. 
                lock (this)
                {
                    candidate = GetNode(siteMapNodes, rawUrl);
                }
                return candidate;
            }
        }

        // Private helper methods 

        private SiteMapNode GetNode(ArrayList list, string url)
        {
            for (int i = 0; i < list.Count; i++)
            {
                DictionaryEntry item = (DictionaryEntry)list[i];
                if ((string)item.Key == url)
                    return item.Value as SiteMapNode;
            }
            return null;
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                GC.Collect(1, GCCollectionMode.Forced);
            }
            catch (Exception)
            {
            }
        }


    }
}
