using System;
using System.Collections.Generic;
using System.Text;
using Hd.Model;
using Feng;

namespace Hd.Service
{
    public class HdDataBuffer : Singleton<HdDataBuffer>, IDisposable, IDataBuffer
    {
        public HdDataBuffer()
        {
            ServiceProvider.GetService<IDataBuffers>().AddDataBuffer(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                s_fkhts.Clear();
                s_wtrhts.Clear();
                s_strhts.Clear();
            }
        }

        public void Clear()
        {
            s_fkhts.Clear();
            s_wtrhts.Clear();
            s_strhts.Clear();
        }
        public void Reload()
        {
            Clear();
        }

        public void LoadData()
        {
        }

        private Dictionary<string, 付款合同> s_fkhts = new Dictionary<string, 付款合同>();
        public 付款合同 Get付款合同(IRepository rep, int 费用实体类型)
        {
            string key = 费用实体类型.ToString();
            if (!s_fkhts.ContainsKey(key))
            {
                IList<付款合同> list = (rep as Feng.NH.INHibernateRepository).List<付款合同>(NHibernate.Criterion.DetachedCriteria.For<付款合同>()
                            .Add(NHibernate.Criterion.Expression.Eq("业务类型编号", 费用实体类型))
                            .Add(NHibernate.Criterion.Expression.Le("生效时间", System.DateTime.Today))
                            .AddOrder(NHibernate.Criterion.Order.Desc("生效时间"))
                            .SetMaxResults(1));

                if (list.Count > 0)
                {
                    付款合同 fkht = list[0];
                    rep.Initialize(fkht.合同费用项, fkht);
                    s_fkhts[key] = fkht;
                }
                else
                {
                    s_fkhts[key] = null;
                }
            }
            return s_fkhts[key];
        }

        private Dictionary<string, 受托人合同> s_strhts = new Dictionary<string, 受托人合同>();
        public 受托人合同 Get受托人合同(IRepository rep, int 费用实体类型, string 受托人编号)
        {
            string key = 费用实体类型.ToString() + "," + 受托人编号;
            if (!s_wtrhts.ContainsKey(key))
            {
                IList<受托人合同> list = (rep as Feng.NH.INHibernateRepository).List<受托人合同>(NHibernate.Criterion.DetachedCriteria.For<受托人合同>()
                            .Add(NHibernate.Criterion.Expression.Eq("业务类型编号", 费用实体类型))
                            .Add(NHibernate.Criterion.Expression.Eq("受托人编号", 受托人编号))
                            .Add(NHibernate.Criterion.Expression.Le("有效期始", System.DateTime.Today))
                            .Add(NHibernate.Criterion.Expression.Ge("有效期止", System.DateTime.Today))
                            .AddOrder(NHibernate.Criterion.Order.Desc("签约时间"))
                            .SetMaxResults(1));

                if (list.Count > 0)
                {
                    受托人合同 strht = list[0];
                    rep.Initialize(strht.合同费用项, strht);
                    s_strhts[key] = strht;
                }
                else
                {
                    s_strhts[key] = null;
                }
            }
            return s_strhts[key];
        }

        private Dictionary<string, 委托人合同> s_wtrhts = new Dictionary<string, 委托人合同>();
        public 委托人合同 Get委托人合同(IRepository rep, int 费用实体类型, string 委托人编号)
        {
            string key = 费用实体类型.ToString() + "," + 委托人编号;
            if (!s_wtrhts.ContainsKey(key))
            {
                IList<委托人合同> list = (rep as Feng.NH.INHibernateRepository).List<委托人合同>(NHibernate.Criterion.DetachedCriteria.For<委托人合同>()
                            .Add(NHibernate.Criterion.Expression.Eq("业务类型编号", 费用实体类型))
                            .Add(NHibernate.Criterion.Expression.Eq("委托人编号", 委托人编号))
                            .Add(NHibernate.Criterion.Expression.Le("有效期始", System.DateTime.Today))
                            .Add(NHibernate.Criterion.Expression.Ge("有效期止", System.DateTime.Today))
                            .AddOrder(NHibernate.Criterion.Order.Desc("签约时间"))
                            .SetMaxResults(1));
                if (list.Count > 0)
                {
                    委托人合同 wtrht = list[0];
                    rep.Initialize(wtrht.合同费用项, wtrht);
                    s_wtrhts[key] = wtrht;
                }
                else
                {
                    s_wtrhts[key] = null;
                }
            }
            return s_wtrhts[key];
        }
    }
}
