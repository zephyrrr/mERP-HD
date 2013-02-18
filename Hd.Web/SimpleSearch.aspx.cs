using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Feng;
using Feng.Utils;

namespace Hd.Web
{
    // WebServiceCustomParameter="网页查询_客户委托情况_进口" WebService="WebDataService" WebServiceSelectMethod="SelectMethod"
    /// <summary>
    /// <example>http://***/Search.aspx?window=1&se=总重量 = 0</example>
    /// </summary>
    public partial class SimpleSearch : System.Web.UI.Page
    {
        protected void Button1_Click(object sender, EventArgs e)
        {
            //this.Label1.Text = this.Grid1.PageSize.ToString() + "/" + this.Grid1.PageCount.ToString() + "/" + this.Grid1.RecordCount.ToString()
            //    + "/" + this.Grid1.GroupBy
            //    + "/" + (int)Math.Ceiling((double)(((double)this.Grid1.RecordCount) / ((double)this.Grid1.PageSize))); ;
            IDataBuffer db = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IDataBuffer>();
            if (db != null)
            {
                db.Reload();
            }
        }

        private void CreateColumns(int level, WindowTabInfo tabInfo)
        {
            string gridName = tabInfo.GridName; //"网页查询_客户委托情况_进口";
            foreach (GridColumnInfo info in ADInfoBll.Instance.GetGridColumnInfos(gridName))
            {
                if (info.GridColumnType == GridColumnType.Normal)
                {
                    if (!Authority.AuthorizeByRule(info.ColumnVisible))
                        continue;

                    if (this.Grid1.Levels.Count <= level)
                    {
                        this.Grid1.Levels.Add(new ComponentArt.Web.UI.GridLevel());
                    }

                    ComponentArt.Web.UI.GridColumn column = GetColumn(info);
                    if (!this.Grid1.Levels[level].Columns.Contains(column))
                    {
                        this.Grid1.Levels[level].Columns.Add(GetColumn(info));
                    }
                }
            }

            //m_levelTabInfos[level] = tabInfo;
            //if (tabInfo.Childs.Count > 1)
            //{
            //    //throw new NotSupportedException("only one child is supported!");
            //}

            //for (int i = 0; i < Math.Min(1, tabInfo.Childs.Count); i++)
            //{
            //    WindowTabInfo subTabInfo = tabInfo.Childs[i];
            //    CreateColumns(level + 1, subTabInfo);

            //    if (!m_isHierarchicalEventCreated)
            //    {
            //        this.Grid1.NeedChildDataSource += new ComponentArt.Web.UI.Grid.NeedChildDataSourceEventHandler(Grid1_NeedChildDataSource);
            //        m_isHierarchicalEventCreated = true;
            //    }
            //}

            for (int i = 0; i <tabInfo.ChildTabs.Count; i++)
            {
                this.Grid1.Levels[level].Columns[i].DataCellCssClass = "DetailTemplate";
            }
        }

        //private bool m_isHierarchicalEventCreated;
        //private Dictionary<int, WindowTabInfo> m_levelTabInfos = new Dictionary<int, WindowTabInfo>();
        //private MockDisplayManager m_dmMock = new MockDisplayManager();

        void Grid1_NeedChildDataSource(object sender, ComponentArt.Web.UI.GridNeedChildDataSourceEventArgs e)
        {
            //WindowTabInfo tabInfo = m_levelTabInfos[e.Item.Level];

            //ISearchManager sm = Feng.Windows.Forms.ArchiveFormFactory.GenerateSearchManager(tabInfo, m_dmMock);

            //m_dmMock.SetCurrentItem(e.Item);

            ////string exp = Helper.ReplaceParameterizedEntity(m_searchExpression, e.Item);
            ////object ret = m_innerSearchManager.FindData(new List<ISearchExpression> { SearchExpression.Parse(exp) }, null);

            //e.DataSource = ret;

            //Services.SoaDataService service = new Hd.Web.Services.SoaDataService();
            //ComponentArt.SOA.UI.SoaDataGridSelectResponse response = service.Select(new ComponentArt.SOA.UI.SoaDataGridSelectRequest());
            //e.DataSource = response.Data;


            //List<List<object>> ret = new List<List<object>>();
            //ret.Add(new List<object> { "a" });
            //ret.Add(new List<object> { "b" });
            //e.DataSource = ret;

            //if (args.Item.Level == 0)
            //{
            //    args.DataSource =
            //        (from product in db.Products
            //         where product.CategoryID == (int)args.Item["CategoryId"]
            //         select new
            //         {
            //             ProductId = product.ProductID,
            //             ProductName = product.ProductName,
            //             SupplierId = product.SupplierID,
            //             QuantityPerUnit = product.QuantityPerUnit,
            //             UnitPrice = product.UnitPrice,
            //             UnitsInStock = product.UnitsInStock
            //         }).ToList();
            //}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                Label1.Text = System.Threading.Thread.CurrentPrincipal.Identity.Name;

                Dictionary<string, object> dict = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["window"]))
                {
                    string windowId = this.Context.Request.QueryString["window"];

                    WindowInfo windowInfo = ADInfoBll.Instance.GetWindowInfo(windowId);
                    IList<WindowTabInfo> tabInfos = null;
                    if (windowInfo != null)
                    {
                        this.Title = windowInfo.Text;
                        tabInfos = ADInfoBll.Instance.GetWindowTabInfosByWindowId(windowInfo.Id);
                    }
                    if (tabInfos != null && tabInfos.Count > 0)
                    {
                        CreateColumns(0, tabInfos[0]);

                        dict["winTab"] = tabInfos[0].Id;
                    }
                }
                else if (!string.IsNullOrEmpty(this.Context.Request.QueryString["wintab"]))
                {
                    string winTabId = this.Context.Request.QueryString["wintab"];
                    WindowTabInfo tabInfo = ADInfoBll.Instance.GetWindowTabInfo(winTabId);
                    if (tabInfo != null)
                    {
                        this.Title = tabInfo.Text;
                        CreateColumns(0, tabInfo);

                        dict["winTab"] = tabInfo.Id.ToString();
                    }
                }
                else
                {
                    return;
                }

                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["se"]))
                {
                    //this.Grid1.Filter = this.Context.Request.QueryString["se"];
                    dict["se"] = this.Context.Request.QueryString["se"];
                }
                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["so"]))
                {
                    this.Grid1.Sort = this.Context.Request.QueryString["so"];
                }
                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["ro"]))
                {
                    this.Grid1.RecordOffset = ConvertHelper.ToInt(this.Context.Request.QueryString["ro"]).Value;
                }
                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["ps"]))
                {
                    this.Grid1.PageSize = ConvertHelper.ToInt(this.Context.Request.QueryString["ps"]).Value;
                }

                this.Grid1.WebServiceCustomParameter = Helper.DictionaryToString(dict);
            }
        }

        private ComponentArt.Web.UI.GridColumn GetColumn(GridColumnInfo info)
        {
            ComponentArt.Web.UI.GridColumn column = new ComponentArt.Web.UI.GridColumn { DataField = info.GridColumnName, 
                HeadingText = (string.IsNullOrEmpty(info.Caption) ? info.PropertyName : info.Caption) };

            Type type = info.CreateType();
            if (type.IsEnum
                || (info.CellViewerManager == "Combo" || info.CellViewerManager == "MultiCombo"))
                column.DataType = typeof(string);
            else
            {
                column.DataType = type;
            }

            string fs = Feng.Utils.DataProcess.GetFormatString(info);
            if (!string.IsNullOrEmpty(fs))
            {
                column.FormatString = fs;
            }
            return column;
        }
    }
}
