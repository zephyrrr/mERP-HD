using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Service
{
    //public class HdDataFactory : IDataFactory
    //{
    //    public Definition CreateDefinition()
    //    {
    //        return new HdDef();
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public DataBuffers CreateDataBuffers()
    //    {
    //        return new HdGlobalData();
    //    }
    //}

    //public class HdGlobalData : DBDataBuffer
    //{
    //    public override void LoadData()
    //    {
    //        base.LoadData();

    //        string[] assemblies = new string[] { "Hd.Model.Base.dll", "Hd.Model.Cn.dll" };
    //        foreach (string assembly in assemblies)
    //        {
    //            foreach (Type type in System.Reflection.Assembly.LoadFrom(Feng.SystemConfiguration.WorkDirectory + "\\" + assembly).GetExportedTypes())
    //            {
    //                if (!type.IsEnum)
    //                    continue;
    //                // user eunm's index
    //                NameValueMappingCollection.Instance.Add(type, true);
    //                //NameValueMappingCollection.Instance.AddAias(type.Name, NameValueMappingCollection.TypeToNameValueMappingName(type, true));
    //            }
    //        }
    //    }

    //    public override void Reload()
    //    {
    //        HdDataBuffer.Instance.Clear();

    //        base.Reload();
    //    }
    //}
}
