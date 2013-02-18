using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model
{
    public abstract class BaseSubmittedDao<T> : HdBaseDao<T>, ISubmittedEntityDao<T>, ISubmittedEntityDao
        where T: class, IMultiOrgEntity, ILogEntity, ISubmittedEntity
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public void Submit(IRepository rep, object entity)
        {
            Submit(rep, entity as T);
        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public void Unsubmit(IRepository rep, object entity)
        {
            Unsubmit(rep, entity as T);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public abstract void Submit(IRepository rep, T entity);

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public abstract void Unsubmit(IRepository rep, T entity);
    }
}
