using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Jk2
{
    public class 进口其他业务票Dao : BaseSubmittedDao<进口其他业务票>
    {
        public 进口其他业务票Dao()
        {
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 进口其他业务票 entity)
        {
            entity.Submitted = true;
            this.Update(rep, entity);
        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 进口其他业务票 entity)
        {
            throw new NotSupportedException("不能对进口其他业务票进行撤销提交操作！");
        }
    }
}
