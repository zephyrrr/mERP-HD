using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections;

namespace Hd.NetRead.npedi
{
    public class HtmlInfoBase
    {
        #region 基本信息
        private CookieContainer _cookie = new CookieContainer();
        public static string Host = "http://www.npedi.com";
        public static string LoginUrl = "http://www.npedi.com/edi/webLoginAction.do";
        public static string BmpUrl = "http://www.npedi.com/edi/ediweb/image.jsp";
        #endregion
        #region  InitialHeadInfo(HttpWebRequest httpWebRequest) 初始化请求头信息

        private static void InitialHeadInfo(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Accept =
            "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            httpWebRequest.Referer = LoginUrl;
            httpWebRequest.UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 3.5.30729)";
            httpWebRequest.Method = "Post";
        }

        #endregion


        #region LoginPostData(string userName,string pwd,string chkInfo) 登录页面验证Post信息
        public static string LoginPostData(string userName,string pwd,string chkInfo)
        {
            StringBuilder postdata = new StringBuilder();

            if (string.IsNullOrEmpty(userName))
            {
                userName = "GUEST";
            }

            if (string.IsNullOrEmpty(pwd))
            {
                pwd = "guest";
            }

            postdata.Append("usercode=");
            postdata.Append(userName);
            postdata.Append("&password=");
            postdata.Append(pwd);
            postdata.Append("&randcode=");
            postdata.Append(chkInfo);
                        
            return string.Format(postdata.ToString());

        }
        #endregion


        #region SearchPostDataByblNo(string blNo) 按提单号查询页面Post信息
        public static string SearchPostDataByblNo(string blNo)
        {
            StringBuilder postData = new StringBuilder();

            postData.Append("vesselvoyage=nonono8&ctnoperatorcode=");
            postData.Append("&options=blno");
            postData.Append("&blno=");
            postData.Append(blNo);
            postData.Append("&ctnno=123&selectAll=on"); // selectAll=on：查询全部

            return string.Format(postData.ToString());
        }
        #endregion


        #region GetHtmlInfo(string host, string URL, string cookie) 获取需登录请求页面信息
        public static string GetHtmlInfo(string host, string URL, string cookie)
         {
             long contentLength;    // 获取请求返回的内容的长度
             
             HttpWebResponse webResponse;
             Stream getStream;

             HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
             CookieContainer co = new CookieContainer();
             co.SetCookies(new Uri(host), cookie);

             request.CookieContainer = co;

             InitialHeadInfo(request);

             webResponse = request.GetResponse() as HttpWebResponse;

             contentLength = webResponse.ContentLength;

             getStream = webResponse.GetResponseStream();

             StreamReader sr = new StreamReader(getStream,
             System.Text.Encoding.Default);

             String strContent = sr.ReadToEnd();

             sr.Close();
             request = null;
             webResponse = null;

             return strContent;
         }
        #endregion

        #region Gett提交方式
        public static string GetHtmlInfo(string URL, out string cookie)
        {
            WebRequest request = WebRequest.Create(URL);

            request.Credentials = CredentialCache.DefaultCredentials; // 当进行请求时使用存储在该属性中的凭据验证客户端

            WebResponse response = request.GetResponse();

            string htm = new StreamReader(response.GetResponseStream(), Encoding.Default).ReadToEnd();

            cookie = response.Headers.Get("Set-Cookie");

            return htm;

        }
        #endregion

        #region Post提交方式
        public static string GetHtml(string URL, string postData, string cookie, out string header)
        {
            return GetHtml(Host, URL, postData, cookie, out header);
        }
        #endregion

        public static string GetHtml(string server, string URL, string postData, string cookie, out string header)
        {
            byte[] byteRequest = Encoding.Default.GetBytes(postData);
            return GetHtml(server, URL, byteRequest, cookie, out header);
        }

        public static string GetHtml(string server, string URL, byte[] byteRequest, string cookie, out string header)
        {
            byte[] bytes = GetHtmlByBytes(server, URL, byteRequest, cookie, out header);
            Stream getStream = new MemoryStream(bytes);
            StreamReader streamReader = new StreamReader(getStream, System.Text.Encoding.Default);
            string getString = streamReader.ReadToEnd();
            streamReader.Close();
            getStream.Close();
            return getString;
        }

        #region  GetHtml(string URL) 获取页面信息
        public static string GetHtml(string URL)
        {
            WebRequest wrt;
            wrt = WebRequest.Create(URL);
            wrt.Credentials = CredentialCache.DefaultCredentials;
            WebResponse wrp;
            wrp = wrt.GetResponse();
            return new StreamReader(wrp.GetResponseStream(), System.Text.Encoding.Default).ReadToEnd();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">服务器地址</param>
        /// <param name="URL">图片链接地址</param>
        /// <param name="byteRequest"></param>
        /// <param name="Cookie"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        #region  GetHtmlByBytes(string host, string URL, byte[] byteRequest,string cookie, out string header) 获取验证码信息
        public static byte[] GetHtmlByBytes(string host, string URL, byte[] byteRequest,string cookie, out string header)
        {
            long contentLength;    // 获取请求返回的内容的长度                        
            HttpWebResponse webResponse;            
            Stream getStream;
          
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            CookieContainer co = new CookieContainer();
            co.SetCookies(new Uri(host), cookie);

            request.CookieContainer = co;

            InitialHeadInfo(request);
            
            request.ContentLength = byteRequest.Length;
            Stream stream;
            stream = request.GetRequestStream();
            stream.Write(byteRequest, 0, byteRequest.Length);
            stream.Close();

            webResponse = request.GetResponse() as HttpWebResponse;

            contentLength = webResponse.ContentLength;


            header = webResponse.Headers.ToString();
            getStream = webResponse.GetResponseStream();
            
            byte[] outBytes = new byte[5000];
            outBytes = ReadFully(getStream);
            getStream.Close();
            request = null;
            webResponse = null;
            return outBytes;

        }
        #endregion

        public static byte[] ReadFully(Stream stream)
        {
            byte[] buffer = new byte[128];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public static Stream GetStreamByBytes(string host,string URL, byte[] byteRequest,string cookie, out string header)
        {
            Stream stream = new MemoryStream(GetHtmlByBytes(host,URL, byteRequest,cookie,out header));
            return stream;
        }
             
    }
}
