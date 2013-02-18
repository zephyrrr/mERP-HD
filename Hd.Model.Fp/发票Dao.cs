using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Fp
{
    public class 发票Dao : BaseSubmittedDao<发票>, ICancellateDao
    {
        public 发票Dao()
        {
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 发票 entity)
        {
            entity.Submitted = true;
            entity.入账日期 = entity.日期;
            this.Update(rep, entity);
        }

        /// <summary>
        /// 撤销
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 发票 entity)
        {
            entity.Submitted = false;
            entity.是否作废 = false;
            entity.入账日期 = null;
            this.Update(rep, entity);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public void Cancellate(IRepository rep, object entity)
        {
            发票 entity2 = entity as 发票;
            entity2.是否作废 = true;
            this.Update(rep, entity2);
        }
    }
}
