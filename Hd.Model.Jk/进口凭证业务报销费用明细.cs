using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Jk
{
    [Serializable]
    public class 进口凭证业务报销费用明细 : BaseBOEntity
    {
        public string 费用项代码
        {
            get;
            set;
        }

        public decimal 金额
        {
            get;
            set;
        }

        public 收付款方式 支付方式要求
        {
            get;
            set;
        }

        public string 自编号
        {
            get;
            set;
        }

        public string 箱号
        {
            get;
            set;
        }
    }
}