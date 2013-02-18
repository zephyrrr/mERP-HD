using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Subclass(NameType = typeof(业务费用), ExtendsType = typeof(费用), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 费用类型.业务费用)]
    public class 业务费用 : 费用, 
        IDetailEntity<费用信息, 业务费用>,
        IDetailEntity<普通票, 业务费用>
    //IDetailEntity<费用信息统计, 业务费用>
    {
        #region "Interface"

        public override bool CanBeDelete(OperateArgs e)
        {
            业务费用 entity = e.Entity as 业务费用;
            if (string.IsNullOrEmpty(entity.费用项编号))
            {
            }
            else
            {
                entity.费用类别 = EntityBufferCollection.Instance.Get<费用类别>(entity.费用类别编号);
                entity.费用项 = EntityBufferCollection.Instance.Get<费用项>(entity.费用项编号);

                if (entity.费用类别.大类 == "业务额外" || entity.费用类别.大类 == "业务常规")
                {
                    IList<费用信息> list = (e.Repository as Feng.NH.INHibernateRepository).List<费用信息>(NHibernate.Criterion.DetachedCriteria.For<费用信息>()
                       .Add(NHibernate.Criterion.Expression.Eq("费用项编号", entity.费用项.编号))
                       .Add(NHibernate.Criterion.Expression.Eq("票.ID", entity.票.ID)));
                    if (list.Count == 0)
                    {
                        // 可能原来未有费用项，没生成费用信息
                        //throw new ArgumentException("Deleted 费用 must have 费用信息!");
                    }
                    else if (list.Count == 1)
                    {
                        // 修改的时候，和完全标志无关?? && e.OperateType == OperateType.Save
                        if ((entity.收付标志 == 收付标志.收 && list[0].Submitted)
                            || (entity.收付标志 == 收付标志.付 && list[0].完全标志付))
                        {
                            throw new InvalidUserOperationException("票" + entity.票.货代自编号 + " 费用项" + entity.费用项编号 + "已打完全标志，不能操作费用！");
                        }

                        HdBaseDao<费用信息> daoFyxx = new HdBaseDao<费用信息>();

                        daoFyxx.Update(e.Repository, list[0]); // 更新Updated时间
                        entity.费用信息 = list[0];
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false, "费用信息对同一费用主体同一费用项有多条！");
                    }
                }
            }

            return base.CanBeDelete(e);
        }

        费用信息 IDetailEntity<费用信息, 业务费用>.MasterEntity
        {
            get { return 费用信息; }
            set { 费用信息 = value; }
        }

        普通票 IDetailEntity<普通票, 业务费用>.MasterEntity
        {
            get { return 票; }
            set { 票 = value; base.费用实体 = value; }
        }
        #endregion

        [ManyToOne(NotNull = false, ForeignKey = "FK_业务费用_费用信息", Cascade = "none")]
        public virtual 费用信息 费用信息
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_业务费用_普通箱")]
        public virtual 普通箱 箱
        {
            get;
            set;
        }

        [Property(Column = "箱", NotNull = false)]
        public virtual Guid? 箱Id
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, Column = "费用实体", NotNull = false)]
        public virtual 普通票 票
        {
            get;
            set;
        }
    }
}
