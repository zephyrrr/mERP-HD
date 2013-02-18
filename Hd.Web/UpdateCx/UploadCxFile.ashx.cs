using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Reflection;
using System.IO;
using Feng;

namespace Hd.Web.UpdateCx
{
    /// <summary>
    /// $codebehindclassname$ 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://www.kawlw.cn/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class UploadCxFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Done!");
            string fileName = HttpContext.Current.Server.MapPath("UploadData\\") + context.Request.QueryString["fileName"];
            Stream raw = context.Request.InputStream;
            if (!string.IsNullOrEmpty(fileName) && raw.Length > 0)
            {
                byte[] b = new byte[raw.Length];
                raw.Read(b, 0, b.Length);
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    fs.Write(b, 0, b.Length);
                }

                string tempDir = HttpContext.Current.Server.MapPath("UploadData\\Temp\\");
                if (!System.IO.Directory.Exists(tempDir))
                {
                    System.IO.Directory.CreateDirectory(tempDir);
                }
                foreach (string s in System.IO.Directory.GetFiles(tempDir))
                {
                    System.IO.File.Delete(s);
                }

                Feng.Utils.CompressionHelper.UnzipToFolder(fileName, tempDir);

                ADUtils.DisableFKConstraint();
                foreach (string s in System.IO.Directory.GetFiles(tempDir))
                {
                    ADUtils.ImportFromXmlFile(s);
                    System.IO.File.Delete(s);
                }
                ADUtils.EnableFKConstraint();

                context.Response.ContentType = "text/plain";
                context.Response.Write("Done!");
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Error!");
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
