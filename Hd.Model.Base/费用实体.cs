using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model
{
    [Class(NameType = typeof(费用实体), Table = "财务_费用实体", OptimisticLock = OptimisticLockMode.Version, Polymorphism = PolymorphismType.Explicit)]
    //[Discriminator(Column = "费用实体类型")]
    public class 费用实体 : SubmittedEntity, IOperatingEntity, 
        IMasterGenerateDetailEntity<费用实体, 费用>
    {
        #region "Interface"

        public virtual void PreparedOperate(OperateArgs e)
        {
        }
        public virtual void PreparingOperate(OperateArgs e)
        {
            if (e.OperateType == OperateType.Save)
            {
                费用实体 entity = e.Entity as 费用实体;

                string typeName = entity.GetType().FullName;
                IEntityBuffer eb = EntityBufferCollection.Instance[typeof(费用类别)];

                if (entity.费用实体类型编号 == 0)
                {
                    foreach (费用类别 i in eb)
                    {
                        if (i.代码类型名 == typeName)
                        {
                            entity.费用实体类型编号 = i.代码;
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(entity.编号))
                {
                    string preKeyValue = Feng.Utils.RepositoryHelper.GetRepositoryPk(e.Repository, typeName);
                    if (string.IsNullOrEmpty(preKeyValue))
                    {
                        foreach (费用类别 i in eb)
                        {
                            if (i.代码类型名 == typeName)
                            {
                                entity.编号 = PrimaryMaxIdGenerator.GetMaxId("财务_费用实体", "编号", 8, i.前缀).ToString();
                                preKeyValue = entity.编号;
                                Feng.Utils.RepositoryHelper.SetRepositoryPk(e.Repository, typeName, preKeyValue);
                                break;
                            }
                        }
                    }
                    else
                    {
                        int delta = Feng.Utils.RepositoryHelper.GetRepositoryDelta(e.Repository, typeName);

                        foreach (费用类别 i in eb)
                        {
                            if (i.代码类型名 == typeName)
                            {
                                entity.编号 = PrimaryMaxIdGenerator.GetMaxIdFromPrevId(preKeyValue, i.前缀, delta + 1);
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(entity.编号))
                    {
                        throw new InvalidOperationException("Invalid 费用实体类型 of " + typeName);
                    }
                }
            }
        }

        IList<费用> IMasterEntity<费用实体, 费用>.DetailEntities
        {
            get { return 费用; }
            set { 费用 = value; }
        }

        IList<费用> IMasterGenerateDetailEntity<费用实体, 费用>.GenerateDetails()
        {
            return Generate费用Details();
        }

        protected virtual IList<费用> Generate费用Details()
        {
            return new List<费用>();
        }

        #endregion

        //[Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.相关号 FROM 视图信息_费用实体_相关号 A WHERE A.ID = Id)")]
        //public virtual string 相关号
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///编号
        ///</summary>
        [Property(Length = 30, NotNull = true, Unique = true, UniqueKey = "UK_费用实体_编号", Index = "Idx_费用实体_编号")]
        public virtual string 编号
        {
            get;
            set;
        }

        //[Map(0, Cascade = "none", Inverse = true)]
        //[Key(1, Column = "费用实体")]
        //[Index(2, Column = "费用项", TypeType = typeof(string))]
        //[OneToMany(3, ClassType = typeof(费用), NotFound = NotFoundMode.Ignore)]
        //public virtual IDictionary<string, 费用> 费用
        //{
        //    get;
        //    set;
        //}

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "费用实体")]
        [OneToMany(2, ClassType = typeof(费用), NotFound = NotFoundMode.Ignore)]
        public virtual IList<费用> 费用
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_费用实体_费用类别")]
        public virtual 费用类别 费用实体类型
        {
            get;
            set;
        }

        [Property(Column = "费用实体类型", NotNull = true)]
        public virtual int 费用实体类型编号
        {
            get;
            set;
        }
    }
}
