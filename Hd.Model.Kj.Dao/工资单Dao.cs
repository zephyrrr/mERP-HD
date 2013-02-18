using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Kj
{
    public class 工资单Dao : BaseSubmittedDao<工资单>
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 工资单 entity)
        {
            if (entity.费用 != null)
            {
                rep.Initialize(entity.费用, entity);
                System.Diagnostics.Debug.Assert(entity.费用.Count <= 1, "工资单费用最多只有一条！");
            }
            else
            {
                entity.费用 = new List<费用>();
            }
            非业务费用 fy;
            if (entity.费用.Count == 0)
            {
                fy = new 非业务费用();
                fy.费用实体 = entity;
                fy.费用项编号 = "341";
                fy.金额 = entity.工资小计;
                fy.收付标志 = 收付标志.付;
                fy.相关人编号 = entity.员工编号;
                (new 非业务费用Dao()).Save(rep, fy);

                entity.费用.Add(fy);
            }
            else
            {
                fy = entity.费用[0] as 非业务费用;
                fy.金额 = entity.工资小计;
                fy.相关人编号 = entity.员工编号;
                (new 非业务费用Dao()).Update(rep, fy);
            }

            entity.Submitted = true;
            this.Update(rep, entity);

            entity.登记金额 = entity.工资小计;
        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 工资单 entity)
        {
            rep.Initialize(entity.费用, entity);
            System.Diagnostics.Debug.Assert(entity.费用.Count == 1, "工资单费用有且只有一条！");

            rep.Delete(entity.费用[0]);
            entity.费用.Clear();
        }
    }
}
