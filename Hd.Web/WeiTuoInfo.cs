using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Hd.Web
{
    public class WeiTuoInfo
    {
        private string 货代自编号;

        public string 货代自编号1
        {
            get { return 货代自编号; }
            set { 货代自编号 = value; }
        }

        private DateTime 委托时间;

        public DateTime 委托时间1
        {
            get { return 委托时间; }
            set { 委托时间 = value; }
        }

        private string 委托人;

        public string 委托人1
        {
            get { return 委托人; }
            set { 委托人 = value; }
        }

        private string 通关类别;

        public string 通关类别1
        {
            get { return 通关类别; }
            set { 通关类别 = value; }
        }

        private string 委托分类;

        public string 委托分类1
        {
            get { return 委托分类; }
            set { 委托分类 = value; }
        }

        private string 提单号;

        public string 提单号1
        {
            get { return 提单号; }
            set { 提单号 = value; }
        }

        private string 船名航次;

        public string 船名航次1
        {
            get { return 船名航次; }
            set { 船名航次 = value; }
        }

        private string 停靠码头;

        public string 停靠码头1
        {
            get { return 停靠码头; }
            set { 停靠码头 = value; }
        }
        
        private int 箱量;

        public int 箱量1
        {
            get { return 箱量; }
            set { 箱量 = value; }
        }

        private string 报检状态;

        public string 报检状态1
        {
            get { return 报检状态; }
            set { 报检状态 = value; }
        }

        private string 报关状态;

        public string 报关状态1
        {
            get { return 报关状态; }
            set { 报关状态 = value; }
        }

        private string 承运状态;

        public string 承运状态1
        {
            get { return 承运状态; }
            set { 承运状态 = value; }
        }

        private string 报关单号;

        public string 报关单号1
        {
            get { return 报关单号; }
            set { 报关单号 = value; }
        }

        //private int 总重量;

        //public int 总重量1
        //{
        //    get { return 总重量; }
        //    set { 总重量 = value; }
        //}

        private DateTime 到港时间;

        public DateTime 到港时间1
        {
            get { return 到港时间; }
            set { 到港时间 = value; }
        }

        //private DateTime 单证齐全时间;

        //public DateTime 单证齐全时间1
        //{
        //    get { return 单证齐全时间; }
        //    set { 单证齐全时间 = value; }
        //}

        //private DateTime 结关时间;

        //public DateTime 结关时间1
        //{
        //    get { return 结关时间; }
        //    set { 结关时间 = value; }
        //}

        //private string 代表性箱号;

        //public string 代表性箱号1
        //{
        //    get { return 代表性箱号; }
        //    set { 代表性箱号 = value; }
        //}

        //private string 品名;

        //public string 品名1
        //{
        //    get { return 品名; }
        //    set { 品名 = value; }
        //}

        //private int 单证晚到天数;

        //public int 单证晚到天数1
        //{
        //    get { return 单证晚到天数; }
        //    set { 单证晚到天数 = value; }
        //}

        //private string 当前状态;

        //public string 当前状态1
        //{
        //    get { return 当前状态; }
        //    set { 当前状态 = value; }
        //}

        //private bool 操作完全标志;

        //public bool 操作完全标志1
        //{
        //    get { return 操作完全标志; }
        //    set { 操作完全标志 = value; }
        //}
    }
}
