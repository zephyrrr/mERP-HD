using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Subclass(NameType = typeof(非业务费用), ExtendsType = typeof(费用), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 费用类型.非业务费用)]
    public class 非业务费用 : 费用
    {
        public override void PreparingOperate(OperateArgs e)
        {
            base.PreparingOperate(e);

            if (e.OperateType == OperateType.Save || e.OperateType == OperateType.Update)
            {
                非业务费用 entity = e.Entity as 非业务费用;
                if (string.IsNullOrEmpty(entity.费用项编号))
                {
                    entity.费用类别编号 = null;
                }
                else
                {
                    // 当费用项变换时，要重新设置费用类别编号
                    entity.费用项 = EntityBufferCollection.Instance.Get<费用项>(entity.费用项编号);
                    entity.费用类别编号 = entity.收付标志 == 收付标志.收 ? entity.费用项.收入类别 : entity.费用项.支出类别;
                    if (!entity.费用类别编号.HasValue)
                    {
                        throw new InvalidUserOperationException("您选择的费用项和收付有误，请重新选择！");
                    }
                }
            }
        }

        [ManyToOne(Insert = false, Update = false, Column = "费用实体", NotNull = false)]
        public virtual 费用实体 非业务费用实体
        {
            get;
            set;
        }
    }
}
