using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Feng;
using Feng.Utils;

namespace Hd.Web
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
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
                        //CreateColumns(0, tabInfos[0]);

                        dict["winTab"] = tabInfos[0].Id;
                    }

                }
                else if (!string.IsNullOrEmpty(this.Context.Request.QueryString["winTab"]))
                {
                    string winTabId = this.Context.Request.QueryString["winTab"];
                    WindowTabInfo tabInfo = ADInfoBll.Instance.GetWindowTabInfo(winTabId);
                    if (tabInfo != null)
                    {
                        this.Title = tabInfo.Text;
                        //CreateColumns(0, tabInfo);

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
                //// 按照当前登录用户，附加查询条件
                //ISearchExpression se = Helper.GetConstraitByRole(Helper.人员类型.委托人, Helper.业务类型.进口);
                //if (se != null)
                //{
                //    if (dict.ContainsKey("se"))
                //    {
                //        dict["se"] = "(" + dict["se"] + ") AND " + se.ToString();
                //    }
                //    else
                //    {
                //        dict["se"] = se.ToString();
                //    }
                //}
                this.Grid1.WebServiceCustomParameter = Helper.DictionaryToString(dict);
                this.Grid1.PageSize = 100;

                //显示上次查询条件
                foreach (string key in Session.Keys)
                {
                    if (key != null)
                    {
                        TextBox tb = (TextBox)this.FindControl(key);
                        tb.Text = Session[key].ToString();
                    }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string zid = id.Text.Trim().ToString();
            //string weiTuoRen = weiTuoRen.SelectedValue.ToString();
            string tiDanHao1 = tiDanHao.Text.Trim().ToString();
            string xiangLiang1 = xiangLiang.Text.Trim().ToString();
            string zongZhongLiang1 = zongZhongLiang.Text.Trim().ToString();
            string daiBiaoXing1 = daiBiaoXing.Text.Trim().ToString();
            string pinMing1 = pinMing.Text.Trim().ToString();
            string state1 = state.SelectedValue.ToString();
            string url = "Search.aspx?window=网页查询_客户委托情况_进口&se=";
            Session.RemoveAll();
            ISearchExpression se = null;
            if (!string.IsNullOrEmpty(zid))
            {
                se = AndIfNecessary(se, SearchExpression.Like("货代自编号", "%" + zid + "%"));
                Session["id"] = zid;
            }
            if (!string.IsNullOrEmpty(tiDanHao1))
            {
                se = AndIfNecessary(se, SearchExpression.Like("提单号", "%" + tiDanHao1 + "%"));
                Session["tiDanHao"] = tiDanHao1;
            }
            if (!string.IsNullOrEmpty(xiangLiang1))
            {
                se = AndIfNecessary(se, SearchExpression.Eq("箱量", xiangLiang1));
                Session["xiangLiang"] = xiangLiang1;
            }
            if (!string.IsNullOrEmpty(zongZhongLiang1))
            {
                se = AndIfNecessary(se, SearchExpression.Eq("总重量", zongZhongLiang1));
                Session["zongZhongLiang"] = zongZhongLiang1;
            }
            if (!string.IsNullOrEmpty(daiBiaoXing1))
            {
                se = AndIfNecessary(se, SearchExpression.Like("代表性箱号", "%" + daiBiaoXing1 + "%"));
                Session["daiBiaoXing"] = daiBiaoXing1;
            }
            if (!string.IsNullOrEmpty(pinMing1))
            {
                se = AndIfNecessary(se, SearchExpression.Like("品名", "%" + pinMing1 + "%"));
                Session["pinMing"] = pinMing1;
            }
            if (!string.IsNullOrEmpty(state1))
            {
                se = AndIfNecessary(se, SearchExpression.Eq("当前状态", state1));
                //Session["state"] = state1;
            }
            if (!weiTuoTime1.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Ge("委托时间", weiTuoTime1.Value.Trim().ToString()));
            }
            if (!weiTuoTime2.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Le("委托时间", weiTuoTime2.Value.Trim().ToString()));
            }
            if (!daoGangTime1.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Ge("到港时间", daoGangTime1.Value.Trim().ToString()));
            }
            if (!daoGangTime2.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Le("到港时间", daoGangTime2.Value.Trim().ToString()));
            }
            if (!danZhengTime1.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Ge("单证齐全时间", danZhengTime1.Value.Trim().ToString()));
            }
            if (!danZhengTime2.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Le("单证齐全时间", danZhengTime2.Value.Trim().ToString()));
            }
            if (!jieGuanTime1.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Ge("结关时间", jieGuanTime1.Value.Trim().ToString()));
            }
            if (!jieGuanTime2.Value.Trim().ToString().Equals(""))
            {
                se = AndIfNecessary(se, SearchExpression.Le("结关时间", jieGuanTime2.Value.Trim().ToString()));
            }

            if (se != null)
            {
                //exp = HttpUtility.UrlEncode(exp);
                string exp = se.ToString();
                //exp = HttpUtility.UrlEncode(exp, System.Text.Encoding.GetEncoding("gb2312"));
                exp = Server.UrlEncode(exp);
                url += exp;
            }
            Response.Redirect(url);
        }

        private ISearchExpression AndIfNecessary(ISearchExpression original, ISearchExpression and)
        {
            if (original != null)
                return SearchExpression.And(original, and);
            else
                return and;
        }

        protected string And(ref string url)
        {
            if (url.Length > 35)
            {
                url += " and";
            }
            return url;
        }

        //protected void Grid1_SelectCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        //{
        //    ComponentArt.Web.UI.Grid myGrid = (ComponentArt.Web.UI.Grid)sender;
        //    object[] selectInfo = myGrid.SelectedItems[0].ToArray();
        //    WeiTuoInfo weiTuoInfo = new WeiTuoInfo();
        //    weiTuoInfo.货代自编号1 = selectInfo[0].ToString();
        //    weiTuoInfo.委托时间1 = Convert.ToDateTime(selectInfo[1]);
        //    weiTuoInfo.委托人1 = selectInfo[2].ToString();
        //    weiTuoInfo.提单号1 = selectInfo[3].ToString();
        //    weiTuoInfo.箱量1 = Convert.ToInt32(selectInfo[4]);
        //    weiTuoInfo.总重量1 = Convert.ToInt32(selectInfo[5]);
        //    weiTuoInfo.到港时间1 = Convert.ToDateTime(selectInfo[6]);
        //    weiTuoInfo.单证齐全时间1 = Convert.ToDateTime(selectInfo[7]);
        //    weiTuoInfo.结关时间1 = Convert.ToDateTime(selectInfo[8]);
        //    weiTuoInfo.代表性箱号1 = selectInfo[9].ToString();
        //    weiTuoInfo.品名1 = selectInfo[10].ToString();
        //    weiTuoInfo.单证晚到天数1 = Convert.ToInt32(selectInfo[11]);
        //    weiTuoInfo.当前状态1 = selectInfo[12].ToString();
        //    weiTuoInfo.操作完全标志1 = Convert.ToBoolean(selectInfo[13]);
        //    Session["WeiTuoInfo"] = weiTuoInfo; 
        //    Response.Write("<script>window.open('WebForm2.aspx');</script>");
        //}

        protected void selectItem()
        {
            ComponentArt.Web.UI.GridItemCollection col = Grid1.SelectedItems;
            
        }

    }
}
