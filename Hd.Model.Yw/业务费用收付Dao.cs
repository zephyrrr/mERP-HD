using System;
using System.Collections.Generic;
using System.Data;
using Feng;

namespace Hd.Model
{
    public class 业务费用收付Dao : BaseDao<业务费用收付>//: Feng.Data.DataTableDao
    {
        private Guid m_费用实体Id;
        public void Set费用实体(Guid 费用实体Id)
        {
            m_费用实体Id = 费用实体Id;
        }

        private 业务费用Dao ywfyDao = new 业务费用Dao();

        protected override void DoSave(IRepository rep, 业务费用收付 entity)
        {
            throw new InvalidOperationException();
        }

        protected override void DoUpdate(IRepository rep, 业务费用收付 entity)
        {
            // 新增收款
            if (entity.收_业务费用.ID == Guid.Empty &&
                (entity.收_业务费用.相关人编号 != null ||
                entity.收_业务费用.金额 != null ||
                entity.收_业务费用.备注 != null))
            {
                ywfyDao.Set费用实体(entity.费用实体.ID);
                ywfyDao.Save(rep, entity.收_业务费用);
            }
            // 修改收款
            else if (entity.收_业务费用.ID != Guid.Empty &&
                    (entity.收_业务费用.相关人编号 != null ||
                    entity.收_业务费用.金额 != null ||
                    entity.收_业务费用.备注 != null))
            {
                ywfyDao.Update(rep, entity.收_业务费用);
            }
            // 删除收款
            else if (entity.收_业务费用.ID != Guid.Empty &&
                entity.收_业务费用.相关人编号 == null &&
                entity.收_业务费用.金额 == null &&
                entity.收_业务费用.备注 == null)
            {
                ywfyDao.Delete(rep, entity.收_业务费用);
            }

            // 新增付款
            if (entity.付_业务费用.ID == Guid.Empty &&
                (entity.付_业务费用.相关人编号 != null ||
                entity.付_业务费用.金额 != null ||
                entity.付_业务费用.备注 != null))
            {
                ywfyDao.Set费用实体(entity.费用实体.ID);
                ywfyDao.Save(rep, entity.付_业务费用);
            }
            // 修改付款
            else if (entity.付_业务费用.ID != Guid.Empty &&
                (entity.付_业务费用.相关人编号 != null ||
                entity.付_业务费用.金额 != null ||
                entity.付_业务费用.备注 != null))
            {
                ywfyDao.Update(rep, entity.付_业务费用);
            }
            // 删除付款
            else if (entity.付_业务费用.ID != Guid.Empty &&
                entity.付_业务费用.相关人编号 == null &&
                entity.付_业务费用.金额 == null &&
                entity.付_业务费用.备注 == null)
            {
                ywfyDao.Delete(rep, entity.付_业务费用);
            }
        }


        #region 视图

        //public override void Save(object entity)
        //{
        //    throw new NullReferenceException();
        //}

        //public override void SaveOrUpdate(object entity)
        //{
        //    throw new NullReferenceException();
        //}

        //public override void Delete(object entity)
        //{
        //    throw new NullReferenceException();
        //}

        //public override void Update(object entity)
        //{
        //    DataRowView rowView = entity as DataRowView;
        //    DataRow row = rowView.Row;
        //    if (row != null)
        //    {
        //        string 收_Id = row["收_Id"].ToString(), 收_Version = row["收_Version"].ToString(), 收_相关人 = row["收_相关人"].ToString(), 收_金额 = row["收_金额"].ToString(), 收_备注 = row["收_备注"].ToString(),
        //            付_Id = row["付_Id"].ToString(), 付_Version = row["付_Version"].ToString(), 付_相关人 = row["付_相关人"].ToString(), 付_金额 = row["付_金额"].ToString(), 付_备注 = row["付_备注"].ToString();

        //        // 删除
        //        if (!string.IsNullOrEmpty(收_Id) && !string.IsNullOrEmpty(收_Version) && string.IsNullOrEmpty(收_相关人) && string.IsNullOrEmpty(收_金额) && string.IsNullOrEmpty(收_备注))
        //        {
        //            Delete(new Guid(收_Id), int.Parse(收_Version));
        //            row["收_Id"] = null;
        //            row["收_Version"] = DBNull.Value;
        //        }

        //        if (!string.IsNullOrEmpty(付_Id) && !string.IsNullOrEmpty(付_Version) && string.IsNullOrEmpty(付_相关人) && string.IsNullOrEmpty(付_金额) && string.IsNullOrEmpty(付_备注))
        //        {
        //            Delete(new Guid(付_Id), int.Parse(付_Version));
        //            row["付_Id"] = null;
        //            row["付_Version"] = DBNull.Value;
        //        }

        //        // 新增
        //        if (string.IsNullOrEmpty(收_Id) &&
        //            (!string.IsNullOrEmpty(收_相关人) || !string.IsNullOrEmpty(收_金额) || !string.IsNullOrEmpty(收_备注)))
        //        {
        //            Guid guid; int version;
        //            Save(收_相关人, row["费用项"].ToString(), 收_金额, 收_备注, 收付标志.收, out guid, out version);
        //            row["收_Id"] = guid.ToString();
        //            row["收_Version"] = version;
        //        }

        //        if (string.IsNullOrEmpty(付_Id) &&
        //            (!string.IsNullOrEmpty(付_相关人) || !string.IsNullOrEmpty(付_金额) || !string.IsNullOrEmpty(付_备注)))
        //        {
        //            Guid guid; int version;
        //            Save(付_相关人, row["费用项"].ToString(), 付_金额, 付_备注, 收付标志.付, out guid, out version);
        //            row["付_Id"] = guid.ToString();
        //            row["付_Version"] = version;
        //        }

        //        // 修改
        //        if (!string.IsNullOrEmpty(收_Id) && !string.IsNullOrEmpty(收_Version) &&
        //            (!string.IsNullOrEmpty(收_相关人) || !string.IsNullOrEmpty(收_金额) || !string.IsNullOrEmpty(收_备注)))
        //        {
        //            int version = int.Parse(收_Version);
        //            Update(收_相关人, 收_金额, 收_备注, new Guid(收_Id), version);
        //            row["收_Version"] = version + 1;
        //        }

        //        if (!string.IsNullOrEmpty(付_Id) && !string.IsNullOrEmpty(付_Version) &&
        //            (!string.IsNullOrEmpty(付_相关人) || !string.IsNullOrEmpty(付_金额) || !string.IsNullOrEmpty(付_备注)))
        //        {
        //            int version = int.Parse(付_Version);
        //            Update(付_相关人, 付_金额, 付_备注, new Guid(付_Id), version);
        //            row["付_Version"] = version + 1;
        //        }
        //    }
        //}

        //private 业务费用 Get业务费用(IRepository rep, Guid 费用Id, int version)
        //{
        //    业务费用 ywfy = rep.Get<业务费用>(费用Id);
        //    if (ywfy.Version != version)
        //    {
        //        Feng.ServiceProvider.GetService<IMessageBox>().ShowError("出现多人操作，请先刷新数据");
        //        return null;
        //    }
        //    else
        //    {
        //        return ywfy;
        //    }
        //}

        //private void Save(string 相关人编号, string 费用项编号, string 金额, string 备注, 收付标志 收付标志, out Guid guid, out int version)
        //{
        //    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
        //    {
        //        try
        //        {
        //            rep.BeginTransaction();
        //            业务费用 ywfy = new 业务费用();
        //            ywfy.收付标志 = 收付标志;
        //            ywfy.相关人编号 = string.IsNullOrEmpty(相关人编号) ? null : 相关人编号;
        //            ywfy.费用项编号 = 费用项编号;
        //            if (string.IsNullOrEmpty(金额))
        //            {
        //                ywfy.金额 = null;
        //            }
        //            else
        //            {
        //                ywfy.金额 = decimal.Parse(金额);
        //            }
        //            ywfy.备注 = 备注;
        //            ywfy.票 = rep.Get<普通票>(m_费用实体Id);
        //            ywfy.费用实体 = ywfy.票;
        //            new 业务费用Dao().Save(rep, ywfy);
        //            rep.CommitTransaction();
        //            guid = ywfy.ID;
        //            version = ywfy.Version;
        //        }
        //        catch (Exception ex)
        //        {
        //            rep.RollbackTransaction();
        //            throw new InvalidOperationException(ex.Message, ex);
        //        }
        //    }
        //}

        //private void Update(string 相关人编号, string 金额, string 备注, Guid 费用Id, int version)
        //{
        //    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
        //    {
        //        try
        //        {
        //            rep.BeginTransaction();
        //            业务费用 ywfy = Get业务费用(rep, 费用Id, version);
        //            if (ywfy == null)
        //            {
        //                return;
        //            }
        //            ywfy.相关人编号 = string.IsNullOrEmpty(相关人编号) ? null : 相关人编号;
        //            if (string.IsNullOrEmpty(金额))
        //            {
        //                ywfy.金额 = null;
        //            }
        //            else
        //            {
        //                ywfy.金额 = decimal.Parse(金额);
        //            }
        //            ywfy.备注 = 备注;
        //            new 业务费用Dao().Update(rep, ywfy);
        //            rep.CommitTransaction();
        //        }
        //        catch (Exception ex)
        //        {
        //            rep.RollbackTransaction();
        //            throw new InvalidOperationException(ex.Message, ex);
        //        }
        //    }
        //}

        //private void Delete(Guid 费用Id, int version)
        //{
        //    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
        //    {
        //        rep.BeginTransaction();
        //        业务费用 ywfy = Get业务费用(rep, 费用Id, version);
        //        if (ywfy == null)
        //        {
        //            return;
        //        }
        //        if (ywfy != null)
        //        {
        //            try
        //            {
        //                new 业务费用Dao().Delete(rep, ywfy);
        //                rep.CommitTransaction();
        //            }
        //            catch (Exception ex)
        //            {
        //                rep.RollbackTransaction();
        //                throw new InvalidOperationException(ex.Message, ex);
        //            }
        //        }
        //        else
        //        {
        //            throw new NullReferenceException("找不到这条费用记录" + Environment.NewLine + 费用Id.ToString());
        //        }
        //    }
        //}

        #endregion
    }
}
