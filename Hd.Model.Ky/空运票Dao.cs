using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Ky
{
    public class 空运票Dao : BaseSubmittedDao<空运票>
    {
        public 空运票Dao()
        {

        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 空运票 entity)
        {
            rep.Initialize(entity.箱, entity);

            //if (!entity.箱量.HasValue || entity.箱量.Value != entity.箱.Count)
            //{
            //    throw new InvalidUserOperationException("箱量不付，请重新填写！");
            //}

            entity.Submitted = true;
            this.Update(rep, entity);

        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 空运票 entity)
        {
            throw new NotSupportedException("不能对空运票进行撤销提交操作！");
        }
    }
}
