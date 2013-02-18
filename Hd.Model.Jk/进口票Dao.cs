using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Jk
{
    /*Process, 打上submitted标志，以后不能再次处理。
主管有权限再次处理，主管也能修改重要值，例如转关标志。

如果要删除，在后续自己删除。（清关转关）



例如，对于进口备案，普通保存不产生后继记录。只有按了“备案提交”按钮后，“是否提交”=true，产生相应后继记录。备案员此时不能修改重要数据，也不能删除，也不能再次提交，撤销提交。
如果数据有错误，主管编辑重要数据，然后再次按“备案提交”按钮。此时可能会删除相应的后继和产生相应的后继。
如果数据不需要，即要删除备案，也需要主管先手动删除后继，再删除备案。
     */

    public class 进口票Dao : BaseSubmittedDao<进口票>
    {
        public 进口票Dao()
        {
            // 费用信息不是在备案的时候产生
            // this.AddSubDao(new MasterGenerateDetailDao<普通票, 费用信息>(new HdBaseDao<费用信息>()));
            this.AddRelationalDao(new OnetoOneGenerateChildDao<进口票, 进口票过程转关标志>(new HdBaseDao<进口票过程转关标志>()));
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 进口票 entity)
        {
            rep.Initialize(entity.箱, entity);

            if (!entity.箱量.HasValue || entity.箱量.Value != entity.箱.Count)
            {
                throw new InvalidUserOperationException("箱量不付，请重新填写！");
            }
            if (!entity.到港时间.HasValue)
            {
                throw new InvalidUserOperationException("到港时间不能为空！");
            }

            for (int i = 0; i < entity.箱.Count - 1; i++)
            {
                for (int k = i + 1; k < entity.箱.Count; k++)
                {
                    if (entity.箱[i].箱号 == entity.箱[k].箱号 && !string.IsNullOrEmpty(entity.箱[i].箱号))
                    {
                        throw new InvalidUserOperationException("包含了重复箱号，无法提交");
                    }
                }
            }

            entity.Submitted = true;
            this.Update(rep, entity);

        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 进口票 entity)
        {
            throw new NotSupportedException("不能对进口票进行撤销提交操作！");
        }
    }
}
