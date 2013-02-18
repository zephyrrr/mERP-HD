using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk
{
    [Class(Name = "进口票过程转关标志", Table = "业务过程_进口票_转关标志", OptimisticLock = OptimisticLockMode.Version)]
    [AttributeIdentifier("Id.Generator", Value = "assigned")]
    public class 进口票过程转关标志 : BaseEntity<Guid>, 
        IOnetoOneChildEntity<进口票, 进口票过程转关标志>
    {
        public override Guid Identity
        {
            get { return this.ID; }
        }

        [Id(0, Name = "ID")]
        [Generator(1, Class = "assigned")]
        public virtual Guid ID
        {
            get;
            set;
        }

        #region "Interface"
        进口票 IOnetoOneChildEntity<进口票, 进口票过程转关标志>.ParentEntity
        {
            get { return 票; }
            set { 票 = value; }
        }
        #endregion

        /// <summary>
        /// 票信息
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程转关标志_进口票")]
        public virtual 进口票 票
        {
            get; set;
        }
    }
}
