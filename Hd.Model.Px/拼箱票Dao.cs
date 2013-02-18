using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Px
{
    public class 拼箱票Dao : BaseSubmittedDao<拼箱票>
    {
        public 拼箱票Dao()
        {

        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 拼箱票 entity)
        {
            rep.Initialize(entity.箱, entity);

            if (string.IsNullOrEmpty(entity.委托人编号))
            {
                throw new InvalidUserOperationException("请填写完整后提交！");
            }

            if (!entity.箱量.HasValue || entity.箱量.Value != entity.箱.Count)
            {
                throw new InvalidUserOperationException("箱量不付，请重新填写！");
            }

            entity.Submitted = true;
            this.Update(rep, entity);

        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 拼箱票 entity)
        {
            throw new NotSupportedException("不能对拼箱票进行撤销提交操作！");
        }
    }
}
