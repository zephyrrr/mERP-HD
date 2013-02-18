using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Ck
{

    public class 出口票Dao : BaseSubmittedDao<出口票>
    {
        public 出口票Dao()
        {         
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 出口票 entity)
        {
            rep.Initialize(entity.箱, entity);

            //if (string.IsNullOrEmpty(entity.委托人编号))
            //{
            //    throw new InvalidUserOperationException("请填写完整后提交！");
            //}

            if (!entity.箱量.HasValue || entity.箱量.Value != entity.箱.Count)
            {
                throw new InvalidUserOperationException("箱量不付，请重新填写！");
            }

            for (int i = 0; i < entity.箱.Count - 1; i++)
            {
                for (int k = i + 1; k < entity.箱.Count; k++)
                {
                    if (entity.箱[i].箱号 == entity.箱[k].箱号 && !string.IsNullOrEmpty(entity.箱[i].箱号))
                    {
                        throw new InvalidUserOperationException("包含了重复箱号，无法提交");
                    }
                }
            }

            entity.Submitted = true;
            this.Update(rep, entity);

        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 出口票 entity)
        {
            throw new NotSupportedException("不能对出口票进行撤销提交操作！");
        }
    }
}
