using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feng;
using Hd.Model;

namespace Hd.Model.Cn
{
    public class 换汇Dao : HdBaseDao<换汇>
    {
        protected override void DoSave(Feng.IRepository rep, 换汇 entity)
        {
            if (entity.出款金额.币制编号.Equals(entity.入款金额.币制编号))
            {
                throw new InvalidUserOperationException("换汇业务：" + Environment.NewLine + "出款币制与入款币制不可相同");
            }
            base.DoSave(rep, entity);
        }

        protected override void DoUpdate(Feng.IRepository rep, 换汇 entity)
        {
            if (entity.出款金额.币制编号.Equals(entity.入款金额.币制编号))
            {
                throw new InvalidUserOperationException("换汇业务：" + Environment.NewLine + "出款币制与入款币制不可相同");
            }
            base.DoUpdate(rep, entity);
        }

        protected override void DoSaveOrUpdate(Feng.IRepository rep, 换汇 entity)
        {
            if (entity.出款金额.币制编号.Equals(entity.入款金额.币制编号))
            {
                throw new InvalidUserOperationException("换汇业务：" + Environment.NewLine + "出款币制与入款币制不可相同");
            }
            base.DoSaveOrUpdate(rep, entity);
        }
    }
}
