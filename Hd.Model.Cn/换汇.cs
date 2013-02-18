using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model.Cn
{
    [Serializable]
    [Auditable]
    [Class(Name = "换汇", Table = "财务_换汇", OptimisticLock = OptimisticLockMode.Version)]
    public class 换汇 : BaseBOEntity, IOperatingEntity
    {
        void IOperatingEntity.PreparingOperate(OperateArgs e)
        {
            if (string.IsNullOrEmpty(this.编号) && (e.OperateType == OperateType.Save || e.OperateType == OperateType.Update))
            {
                this.编号 = PrimaryMaxIdGenerator.GetMaxId("财务_换汇", "编号", 8, "H" + PrimaryMaxIdGenerator.GetIdYearMonth(日期)).ToString();
            }
        }
        void IOperatingEntity.PreparedOperate(OperateArgs e)
        {
        }

        ///<summary>
        ///编号
        ///</summary>
        [Property(Length = 8, NotNull = true, Unique = true, UniqueKey = "UK_换汇_编号", Index = "Idx_换汇_编号")]
        public virtual string 编号
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 日期
        {
            get;
            set;
        }

        //[ComponentProperty(PropertyName = "出款货币金额")]
        //[Property(1, Name = "出款币制", Column = "出款币制", TypeType = typeof(string), Length = 3, NotNull = true)]
        //[Property(2, Name = "出款数额", Column = "出款数额", TypeType = typeof(decimal), NotNull = true)]
        //public virtual 金额 出款货币金额
        //{
        //    get;
        //    set;
        //}

        //[ComponentProperty(PropertyName = "入款货币金额")]
        //[Property(1, Name = "入款币制", Column = "入款币制", TypeType = typeof(string), Length = 3, NotNull = true)]
        //[Property(2, Name = "入款数额", Column = "入款数额", TypeType = typeof(decimal), NotNull = true)]
        //public virtual 金额 入款货币金额
        //{
        //    get;
        //    set;
        //}

        private 金额 m_出款金额 = new 金额();
        //[ComponentProperty()]
        [RawXml(After = typeof(ComponentAttribute), Content = @"<component name=""出款金额"">
            <property name=""币制编号"" column = ""出款币制"" length=""3"" not-null=""true""/>
            <property name=""数额"" column = ""出款数额"" not-null=""true""/>
            <many-to-one name=""币制"" column = ""出款币制"" update=""false"" insert=""false"" foreign-key=""FK_换汇_出款币制""/>
            </component>")]
        public virtual 金额 出款金额
        {
            get { return m_出款金额; }
            set { if (value != null) m_出款金额 = value; }
        }

        private 金额 m_入款金额 = new 金额();
        [RawXml(After = typeof(ComponentAttribute), Content = @"<component name=""入款金额"">
            <property name=""币制编号"" column = ""入款币制"" length=""3"" not-null=""true""/>
            <property name=""数额"" column = ""入款数额"" not-null=""true""/>
            <many-to-one name=""币制"" column = ""入款币制"" update=""false"" insert=""false"" foreign-key=""FK_换汇_入款币制"" />
            </component>")]
        public virtual 金额 入款金额
        {
            get { return m_入款金额; }
            set { if (value != null) m_入款金额 = value; }
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_换汇_出款账户")]
        public virtual 银行账户 出款账户
        {
            get;
            set;
        }



        [Property(Column = "出款账户", NotNull = false)]
        public virtual Guid 出款账户编号
        {
            get;
            set;
        }



        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_换汇_入款账户")]
        public virtual 银行账户 入款账户
        {
            get;
            set;
        }

        [Property(Column = "入款账户", NotNull = false)]
        public virtual Guid 入款账户编号
        {
            get;
            set;
        }


        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_换汇_经办人")]
        public virtual 人员 经办人
        {
            get;
            set;
        }

        [Property(Column = "经办人", Length = 6)]
        public virtual string 经办人编号
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 备注
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 摘要//出款账户、入款账户、币制、汇率
        {
            get;
            set;
        }
        
    }
}
