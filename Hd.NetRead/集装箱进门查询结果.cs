using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hd.NetRead
{
    public class 集装箱进门查询结果
    {
        public DateTime? 信息收到时间
        {
            get;
            internal set;
        }
        public DateTime? 进门时间
        {
            get;
            internal set;
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
        public string 码头
        {
            get;
            internal set;
        }
        public string 卸货港
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
        public string 铅封号
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

        public 集装箱进门查询结果(DateTime? 信息收到时间, DateTime? 进门时间, string 船名航次, string 集装箱号, string 提单号, string 码头, string 卸货港, string 箱主, string 尺寸类型, string 铅封号, string 箱状态, string 箱毛重)
        {
            this.信息收到时间 = 信息收到时间;
            this.进门时间 = 进门时间;
            this.船名航次 = 船名航次;
            this.集装箱号 = 集装箱号;
            this.提单号 = 提单号;
            this.码头 = 码头;
            this.卸货港 = 卸货港;
            this.箱主 = 箱主;
            this.尺寸类型 = 尺寸类型;
            this.铅封号 = 铅封号;
            this.箱状态 = 箱状态;
            this.箱毛重 = 箱毛重;
        }
    }
}
