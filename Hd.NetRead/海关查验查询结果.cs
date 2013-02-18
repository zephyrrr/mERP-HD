using System;
using System.Collections.Generic;
using System.Text;

namespace Hd.NetRead
{
    /// <summary>
    /// 海关查验查询结果
    /// </summary>
    public class 海关查验查询结果
    {
        //NO. 船名 航次 箱号 码头 箱型尺寸 移箱类型 H986 目的场站 指令时间 移箱标志 处理时间 反馈信息 
        //1 MAERSKALGOL  1008  MSKU2990124  北二集司(三期)  22  查验  Y   2010-08-16 13:51:05  完成归位  2010-08-16 21:30:31  
        public 海关查验查询结果(string 船名, string 航次, string 箱号, string 码头, string 箱型, string 移箱类型
            , string H986, string 目的场站, DateTime 指令时间, string 移箱标志, DateTime 处理时间, string 反馈信息)
        {
            this.船名 = 船名;
            this.航次 = 航次;
            this.箱号 = 箱号;
            this.码头 = 码头;
            this.箱型 = 箱型;
            this.移箱类型 = 移箱类型;
            this.H986 = H986;
            this.目的场站 = 目的场站;
            this.指令时间 = 指令时间;
            this.移箱标志 = 移箱标志;
            this.处理时间 = 处理时间;
            this.反馈信息 = 反馈信息;
        }

        public string 船名
        {
            get;
            internal set;
        }

        public string 航次
        {
            get;
            internal set;
        }

        public string 箱号
        {
            get;
            internal set;
        }

        public string 码头
        {
            get;
            internal set;
        }

        public string 箱型
        {
            get;
            internal set;
        }

        public string 移箱类型
        {
            get;
            internal set;
        }

        public string 移箱标志
        {
            get;
            internal set;
        }

        public string H986
        {
            get;
            internal set;
        }

        public string 目的场站
        {
            get;
            internal set;
        }

        public DateTime 指令时间
        {
            get;
            internal set;
        }

        public DateTime 处理时间
        {
            get;
            internal set;
        }

        public string 反馈信息
        {
            get;
            internal set;
        }
    }
}
