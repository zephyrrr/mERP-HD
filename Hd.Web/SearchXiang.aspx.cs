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
    public partial class SearchXiang : System.Web.UI.Page
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
                    
                    WeiTuoInfo();
                }
                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["so"]))
                {
                    this.Grid3.Sort = this.Context.Request.QueryString["so"];
                }
                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["ro"]))
                {
                    this.Grid3.RecordOffset = ConvertHelper.ToInt(this.Context.Request.QueryString["ro"]).Value;
                }
                if (!string.IsNullOrEmpty(this.Context.Request.QueryString["ps"]))
                {
                    this.Grid3.PageSize = ConvertHelper.ToInt(this.Context.Request.QueryString["ps"]).Value;
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
                this.Grid3.WebServiceCustomParameter = Helper.DictionaryToString(dict);
            }
        }

        public void WeiTuoInfo()
        {
            string url = Request.Url.ToString();
            int num = url.IndexOf("\"") + 1;
            string HDId = url.Substring(num, url.Substring(num).Length - 1);
            WeiTuoInfo weiTuoInfo = DBManager.SelectWeiTuoInfo(HDId);

            if (weiTuoInfo != null)
            {
                txtHDId.Text = weiTuoInfo.货代自编号1;
                txtWeiTuoTime.Text = weiTuoInfo.委托时间1.ToString().Substring(0, 10);
                txtWeiTuoRen.Text = weiTuoInfo.委托人1;
                txtTgId.Text = weiTuoInfo.通关类别1;
                txtWtfl.Text = weiTuoInfo.委托分类1;
                txtTiDanHao.Text = weiTuoInfo.提单号1;
                txtXiangLiang.Text = weiTuoInfo.箱量1.ToString();
                txtDaoGangTime.Text = weiTuoInfo.到港时间1.ToString().Substring(0, 10);
                txtChuanMing.Text = weiTuoInfo.船名航次1;
                txtTkmt.Text = weiTuoInfo.停靠码头1;
                txtBaoGuanHao.Text = weiTuoInfo.报关单号1;
                txtBjState.Text = weiTuoInfo.报检状态1;
                txtBgState.Text = weiTuoInfo.报关状态1;
                txtCyState.Text = weiTuoInfo.承运状态1;
            }  
        }
    }
}
