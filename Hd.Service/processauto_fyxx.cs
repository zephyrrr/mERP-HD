using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;

namespace Hd.Service
{
    public class processauto_fyxx
    {
        //public static void 费用信息AutoExecute(ArchiveOperationForm masterForm)
        //{
        //    masterForm.DisplayManager.PositionChanged += new EventHandler(DisplayManager_PositionChanged);
        //}

        //static void DisplayManager_PositionChanged(object sender, EventArgs e)
        //{
        //    IDisplayManager dm = sender as IDisplayManager;
        //    if (dm.CurrentItem == null)
        //        return;
        //    NameValueMappingCollection.Instance["费用项_业务_费用类别_动态"].Params["@费用类别"] =
        //        dm.CurrentItem == null ? (object)System.DBNull.Value : (object)((费用信息)dm.CurrentItem).费用类别编号;
        //    int ywlx = ((费用信息)dm.CurrentItem).业务类型编号;
            
        //    NameValueMappingCollection.Instance["费用项_业务_费用类别_动态"].Params["@业务类型"] = "%" + ywlx + ",%";
        //    NameValueMappingCollection.Instance.Reload("费用项_业务_费用类别_动态");

        //    NameValueMappingCollection.Instance["信息_箱号_动态"].Params["@票"] =
        //        dm.CurrentItem == null ? (object)System.DBNull.Value : (object)((费用信息)dm.CurrentItem).票.ID;
        //    NameValueMappingCollection.Instance.Reload("信息_箱号_动态");
        //}
    }
}
