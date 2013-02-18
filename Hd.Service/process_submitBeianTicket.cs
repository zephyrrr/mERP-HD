using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Net;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Jk;
using Hd.Model.Nmcg;
using Hd.Model.Jk2;

namespace Hd.Service
{
    

    public class process_submitBeianTicket
    {
        

        public class 产生内贸出港票相关数据Dao : HdBaseDao<内贸出港票>
        {
            public 产生内贸出港票相关数据Dao()
            {
                //this.AddSubDao(new MasterGenerateDetailDao<费用实体, 费用>(new HdBaseDao<费用>()));
            }
        }

        public static void 内贸出港票提交(ArchiveOperationForm masterForm)
        {
            内贸出港票 ticket = masterForm.ControlManager.DisplayManager.CurrentItem as 内贸出港票;
            if (ticket == null)
            {
                return;
            }
            
        }


        
    }
}
