using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feng;

namespace Hd.Model.Ck
{
    public class 对账单Dao : Hd.Model.对账单Dao
    {
        internal class RepCon : IRepositoryConsumer
        {
            public string RepositoryCfgName
            {
                get { return "hd.model.ck.config"; }
                set { }
            }
        }
        private static RepCon s_repCon = new RepCon();

        public override IRepository GenerateRepository()
        {
            return ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository(s_repCon);
        }
    }
}
