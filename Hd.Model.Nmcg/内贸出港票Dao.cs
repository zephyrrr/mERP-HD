using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Nmcg
{
    public class 内贸出港票Dao : BaseSubmittedDao<内贸出港票>
    {
        public 内贸出港票Dao()
        {
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 内贸出港票 entity)
        {
            if (string.IsNullOrEmpty(entity.货代自编号) || string.IsNullOrEmpty(entity.委托人编号))
            {
                throw new InvalidUserOperationException("货代自编号、委托人，不能为空！");
            }

            rep.Initialize(entity.箱, entity);

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
        public override void Unsubmit(IRepository rep, 内贸出港票 entity)
        {
            throw new NotSupportedException("不能对内贸出港票进行撤销提交操作！");
        }
    }
}
