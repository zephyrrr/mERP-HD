using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model
{
    public class HdBaseDao<T> : MultiOrgEntityDao<T>
        where T: class, IMultiOrgEntity, ILogEntity
    {
    }
}
