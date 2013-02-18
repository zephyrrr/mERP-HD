using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Reflection;
using ComponentArt.Web.UI;
using Feng;

public class TestData
{
    public TestData(string s)
    {
        this.产地 = s;
    }

    public string 产地
    {
        get;
        set;
    }
}
    /// <summary>
    /// WebDataService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://www.kawlw.cn/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class WebDataService : System.Web.Services.WebService
    {
        [WebMethod]
        public GridWebServiceSelectResponse SelectMethod(GridWebServiceSelectRequest request)
        {
            GridWebServiceSelectResponse response = new GridWebServiceSelectResponse();

            try
            {
                request.CustomParameter = "进口_备案";
                WindowTabInfo tabInfo = ADInfoBll.Instance.GetWindowTabInfo(request.CustomParameter);

                if (tabInfo != null)
                {
                    ISearchManager sm = Feng.Windows.Forms.ArchiveFormFactory.GenerateSearchManager(tabInfo, null);
                    IList<ISearchExpression> searchExps = new List<ISearchExpression>();
                    IList<ISearchOrder> searchOrders = new List<ISearchOrder>();
                    if (!string.IsNullOrEmpty(request.Filter))
                    {
                        searchExps.Add(SearchExpression.Parse(request.Filter));
                    }

                    if (!string.IsNullOrEmpty(request.SortField))
                    {
                        searchOrders.Add(request.SortOrder == "ASC" ? SearchOrder.Asc(request.SortField) : SearchOrder.Desc(request.SortField));
                    }

                    sm.FirstResult = request.CurrentPageIndex * request.PageSize;
                    sm.MaxResult = request.PageSize;

                    List<TestData> arMessages = new List<TestData>();

                    object ret = sm.FindData(searchExps, searchOrders);

                    System.Data.DataTable dt = ret as System.Data.DataTable;
                    if (dt == null)
                    {
                        IEnumerable data = ret as IEnumerable;
                        foreach (object obj in data)
                        {
                            arMessages.Add(new TestData(EntityHelper.GetPropertyValue(obj, "产地").ToString()));
                        }
                    }
                    else
                    {
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            List<object> msg = new List<object>();

                            foreach (string oColumn in request.Columns)
                            {
                                msg.Add(row[oColumn]);
                            }
                            //arMessages.Add(msg);
                        }
                    }

                    response.RecordCount = sm.FindDataCount(searchExps);
                    response.Items = arMessages;
                }
                else
                {
                    throw new ArgumentException("Invalide tabName");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }


            return response;
        }

    }
