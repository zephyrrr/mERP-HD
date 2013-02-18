using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Reflection;

namespace Hd.Web.UpdateCx
{
    /// <summary>
    /// $codebehindclassname$ 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://www.kawlw.cn/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GetLastUploadDate : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string clientid = context.Request.QueryString["ClientId"];
            if (!string.IsNullOrEmpty(clientid))
            {
                object ret = Feng.Data.DbHelper.Instance.ExecuteScalar("SELECT (CASE WHEN MAX(CREATED) > MAX(UPDATED) THEN MAX(CREATED) ELSE MAX(UPDATED) END ) FROM 财务_费用实体 WHERE ClientId = " + clientid);
                DateTime? date = Feng.Utils.ConvertHelper.ToDateTime(ret);
                if (date.HasValue)
                {
                    context.Response.Write(date.Value.ToString("yyyy-MM-dd"));
                    return;
                }
                else
                {
                    context.Response.Write("2007-01-01");
                    return;
                }
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
