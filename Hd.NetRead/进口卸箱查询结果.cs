using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hd.NetRead
{
    public class 进口卸箱查询结果
    {
        public 进口卸箱查询结果(string 船名航次, string 集装箱号, string 提单号, DateTime? 卸船时间, string 码头, string 装货港, string 箱主, string 尺寸类型, string 箱状态, string 箱毛重)
        {
            this.船名航次 = 船名航次;
            this.集装箱号 = 集装箱号;
            this.提单号 = 提单号;
            this.卸船时间 = 卸船时间;
            this.码头 = 码头;
            this.装货港 = 装货港;
            this.箱主 = 箱主;
            this.尺寸类型 = 尺寸类型;
            this.箱状态 = 箱状态;
            this.箱毛重 = 箱毛重;
        }

        public string 船名航次
        {
            get;
            internal set;
        }

        public string 集装箱号
        {
            get;
            internal set;
        }

        public string 提单号
        {
            get;
            internal set;
        }

        public DateTime? 卸船时间
        {
            get;
            internal set;
        }

        public string 码头
        {
            get;
            internal set;
        }

        public string 装货港
        {
            get;
            internal set;
        }

        public string 箱主
        {
            get;
            internal set;
        }

        public string 尺寸类型
        {
            get;
            internal set;
        }

        public string 箱状态
        {
            get;
            internal set;
        }

        public string 箱毛重
        {
            get;
            internal set;
        }
    }
}
