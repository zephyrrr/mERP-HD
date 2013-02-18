using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk2
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "进口其他业务箱", ExtendsType = typeof(普通箱), Table = "业务备案_进口其他业务箱")]
    [Key(Column = "Id", ForeignKey = "FK_进口其他业务箱_箱")]
    public class 进口其他业务箱 : 普通箱, 
         IDetailEntity<进口其他业务票, 进口其他业务箱>
    {
        #region "Interface" 
        进口其他业务票 IDetailEntity<进口其他业务票, 进口其他业务箱>.MasterEntity
        {
            get { return 票; }
            set { 票 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_进口其他业务箱_进口其他业务票", Cascade = "none", Lazy = Laziness.False, OuterJoin = OuterJoinStrategy.True)]
        public virtual 进口其他业务票 票
        {
            get;
            set;
        }

        #region "业务信息"


        #endregion

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

        //    进口其他业务箱 that = (进口其他业务箱)other;
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
        //    sb.Append("进口其他业务箱: ");
        //    sb.Append("箱号").Append('=').Append(箱号);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
