using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ky
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "空运箱", ExtendsType = typeof(普通箱), Table = "业务备案_空运箱")]
    [Key(Column = "Id", ForeignKey = "FK_空运箱_箱")]
    public class 空运箱 : 普通箱,
        IDetailEntity<空运票, 空运箱>
    {
        #region "Interface"
        空运票 IDetailEntity<空运票, 空运箱>.MasterEntity
        {
            get { return 票; }
            set { 票 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_空运箱_空运票", Column = "票", Cascade = "none")]
        public virtual 空运票 票
        {
            get;
            set;
        }


    }
}
