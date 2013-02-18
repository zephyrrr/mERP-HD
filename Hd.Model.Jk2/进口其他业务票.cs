using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk2
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "进口其他业务票", ExtendsType = typeof(普通票), Table = "业务备案_进口其他业务票")]
    [Key(Column = "Id", ForeignKey = "FK_进口其他业务票_票")]
    public class 进口其他业务票 : 普通票,
        IMasterEntity<进口其他业务票, 进口其他业务箱>
    {
        #region "Interface"
        IList<进口其他业务箱> IMasterEntity<进口其他业务票, 进口其他业务箱>.DetailEntities
        {
            get { return 箱; }
            set { 箱 = value; }
        }
        #endregion

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "票")]
        [OneToMany(2, ClassType = typeof(进口其他业务箱), NotFound = NotFoundMode.Ignore)]
        public virtual IList<进口其他业务箱> 箱
        {
            get;
            set;
        }

        ///<summary>
        ///进口其他业务类型
        ///</summary>
        [Property(Length = 50, NotNull = true)]
        public virtual string 进口其他业务类型
        {
            get;
            set;
        }

        /////<summary>
        /////进口其他业务类型
        /////</summary>
        //[Property(NotNull = true)]
        //public virtual 进口其他业务类型 进口其他业务类型
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///票箱型
        ///</summary>
        [ManyToOne(Insert = false, Update = false)]
        public virtual 箱型 票箱型
        {
            get;
            set;
        }
        [Property(Column = "票箱型", NotNull = false)]
        public virtual int? 票箱型编号
        {
            get;
            set;
        }
        ///<summary>
        ///受理时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 受理时间
        {
            get;
            set;
        }

        ///<summary>
        ///换单时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 换单时间
        {
            get;
            set;
        }

        ///<summary>
        ///到港时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 到港时间
        {
            get;
            set;
        }

        //#region Overriden Methods

        ///// <summary>
        ///// Returns <code>true</code> if the argument is a Board instance and all identifiers for this entity
        ///// equal the identifiers of the argument entity. Returns <code>false</code> otherwise.
        ///// </summary>
        //public override bool Equals(Object other)
        //{
        //    if (object.ReferenceEquals(this, other))
        //    {
        //        return true;
        //    }

        //    进口其他业务票 that = (进口其他业务票)other;
        //    if (that == null)
        //    {
        //        return false;
        //    }
        //    if (this.ID == null || that.ID == null || !this.ID.Equals(that.ID))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Returns a hash code based on this entity's identifiers.
        ///// </summary>
        //public override int GetHashCode()
        //{
        //    int hashCode = 14;
        //    hashCode = 29 * hashCode + (Id == null ? 0 : Id.GetHashCode());
        //    return hashCode;
        //}

        ///// <summary>
        ///// Returns a string representation of this VersionedEntity
        ///// </summary>
        //public override string ToString()
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.Append("进口其他业务: ");
        //    sb.Append("提单号").Append('=').Append(提单号);
        //    return sb.ToString();
        //}

        //#endregion
    }
}
