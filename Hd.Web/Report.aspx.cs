using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Feng;
using Feng.Utils;
using Feng.Web;
using Feng.Search;
using Feng.Windows.Forms;

namespace Hd.Web
{
    /// <summary>
    /// <example>http://**/Report.aspx?window=300&se=凭证号 Like 08070001</example>
    /// </summary>
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
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
                        ISearchManager sm = ArchiveFormFactory.GenerateSearchManager(tabInfos[0], null) as ISearchManager;

                        IDisplayManager dm = ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName(tabInfos[0].DisplayManagerClassName), sm) as IDisplayManager;

                        ReportInfo reportInfo = ADInfoBll.Instance.GetReportInfo(windowInfo.Id);
                        CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = ReflectionHelper.CreateInstanceFromName(reportInfo.ReportDocument) as CrystalDecisions.CrystalReports.Engine.ReportDocument;
                        Feng.Web.MyCrystalReportViewer reportViewer = this.CrystalReportViewer1;
                        reportViewer.CrystalHelper.ReportSource = reportDocument;

                        //string[] ss = reportInfo.ConnectionInfo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        //if (ss.Length != 4)
                        //{
                        //    throw new ArgumentException("Invalid ConnectionInfo format!");
                        //}
                        //reportViewer.CrystalHelper.ServerName = ss[0].Trim();
                        //reportViewer.CrystalHelper.DatabaseName = ss[1].Trim();
                        //reportViewer.CrystalHelper.UserId = ss[2].Trim();
                        //reportViewer.CrystalHelper.Password = ss[3].Trim();

                        if (!string.IsNullOrEmpty(this.Context.Request.QueryString["se"]))
                        {
                            string searchExpression = this.Context.Request.QueryString["se"];
                            SetParameter(SearchExpression.Parse(searchExpression), reportViewer);
                        }

                        reportViewer.OpenReport();
                    }
                   
                }
                else
                {
                    return;
                }
            }
        }

        private void SetParameter(ISearchExpression se, Feng.Web.MyCrystalReportViewer reportViewer)
        {
            LogicalExpression le = se as LogicalExpression;
            if (le != null)
            {
                SetParameter(le.LeftHandSide, reportViewer);
                SetParameter(le.RightHandSide, reportViewer);
            }
            else
            {
                SimpleExpression sse = se as SimpleExpression;

                reportViewer.CrystalHelper.SetParameter("@" + sse.FullPropertyName + sse.Operator.ToString(), sse.Values);
            }
        }
    }
}
