using System;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(业务费用收付), Table = "视图查询_财务费用_收付", Mutable = false, SchemaAction = "none")]
    public class 业务费用收付 : IEntity
    {
        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, Column = "费用实体", NotNull = false)]
        public virtual 费用实体 费用实体
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual int? SeqNo
        {
            get;
            set;
        }

        [Property(Column = "费用项")]
        public virtual string 费用项编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, Column = "费用项", NotNull = false)]
        public virtual 费用项 费用项
        {
            get;
            set;
        }

        private 业务费用 m_收_业务费用;  
        [ManyToOne(Insert = false, Update = false, Column = "收_Id", NotNull = false, Access = "field.pascalcase-m-underscore")]
        public virtual 业务费用 收_业务费用
        {
            get
            {
                if (m_收_业务费用 != null)
                {
                    return m_收_业务费用;
                }
                else
                {
                    m_收_业务费用 = new 业务费用();
                    m_收_业务费用.收付标志 = 收付标志.收;
                    m_收_业务费用.费用项 = 费用项;
                    m_收_业务费用.费用项编号 = 费用项编号;
                    m_收_业务费用.ID = Guid.Empty;
                    return m_收_业务费用; 
                }
            }
            set { m_收_业务费用 = value; }
        }

        private 业务费用 m_付_业务费用;                    
        [ManyToOne(Insert = false, Update = false, Column = "付_Id", NotNull = false, Access = "field.pascalcase-m-underscore")]
        public virtual 业务费用 付_业务费用
        {
            get
            {
                if (m_付_业务费用 != null)
                {
                    return m_付_业务费用;
                }
                else
                {
                    m_付_业务费用 = new 业务费用();
                    m_付_业务费用.收付标志 = 收付标志.付;
                    m_付_业务费用.费用项 = 费用项;
                    m_付_业务费用.费用项编号 = 费用项编号;
                    m_付_业务费用.ID = Guid.Empty;
                    return m_付_业务费用;
                }
            }
            set { m_付_业务费用 = value; }
        }
    }
}
