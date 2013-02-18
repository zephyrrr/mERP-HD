using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using Feng.Utils;

namespace Hd.Model
{
    public interface I支付方式
    {
        void 支付(IRepository rep, 凭证收支明细 entity);
        void 取消支付(IRepository rep, 凭证收支明细 entity);
    }

    public interface I收入方式
    {
        void 收入(IRepository rep, 凭证收支明细 entity);
        void 取消收入(IRepository rep, 凭证收支明细 entity);
    }

    public class 空收付方式 : I支付方式, I收入方式
    {
        public void 支付(IRepository rep, 凭证收支明细 entity) { }
        public void 取消支付(IRepository rep, 凭证收支明细 entity) { }

        public void 收入(IRepository rep, 凭证收支明细 entity) { }
        public void 取消收入(IRepository rep, 凭证收支明细 entity) { }
    }

    public class 凭证收支明细Dao : HdBaseDao<凭证收支明细>
    {
        public 凭证收支明细Dao()
        {
        }

        private static I支付方式 Create支付方式(凭证收支明细 entity)
        {
            switch (entity.收付款方式)
            {
                case 收付款方式.现金:
                    return new 空收付方式(); // ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.现金Bll")) as I支付方式;
                case 收付款方式.现金支票:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.现金支票Bll")) as I支付方式;
                case 收付款方式.转账支票:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.转账支票Bll")) as I支付方式;
                case 收付款方式.银行承兑汇票:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.承兑汇票Bll")) as I支付方式;
                case 收付款方式.银行收付:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.银行收付Bll")) as I支付方式;
                case 收付款方式.电汇:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.银行收付Bll")) as I支付方式;
                default:
                    throw new InvalidUserOperationException(entity.收付款方式 + "当前不可用！");
            }
        }

        private static I收入方式 Create收入方式(凭证收支明细 entity)
        {
            switch (entity.收付款方式)
            {
                case 收付款方式.现金:
                    return new 空收付方式(); // ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn", "Hd.Model.Cn.现金Bll")) as I收入方式;
                case 收付款方式.现金支票:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.现金支票Bll")) as I收入方式;
                case 收付款方式.转账支票:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.转账支票Bll")) as I收入方式;
                case 收付款方式.银行承兑汇票:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.承兑汇票Bll")) as I收入方式;
                case 收付款方式.银行收付:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.银行收付Bll")) as I收入方式;
                case 收付款方式.银行本票汇票:
                    return ReflectionHelper.CreateInstanceFromType(ReflectionHelper.GetTypeFromName("Hd.Model.Cn.Dao", "Hd.Model.Cn.银行收付Bll")) as I收入方式;
                default:
                    throw new InvalidUserOperationException(entity.收付款方式 + "当前不可用！");
            }
        }

        protected override void DoSave(IRepository rep, 凭证收支明细 entity)
        {
            // 支付(rep, entity);

            base.DoSave(rep, entity);
        }

        protected override void DoUpdate(IRepository rep, 凭证收支明细 entity)
        {
            throw new InvalidOperationException("Update is not supported here");
        }

        protected override void DoDelete(IRepository rep, 凭证收支明细 entity)
        {
            // 取消支付(rep, entity);

            base.DoDelete(rep, entity);
        }

        public static void 支付(IRepository rep, 凭证收支明细 entity)
        {
            switch (entity.收付标志)
            {
                case 收付标志.付:
                    I支付方式 iPay1 = Create支付方式(entity);
                    iPay1.支付(rep, entity);
                    break;
                case 收付标志.收:
                    I收入方式 iPay2 = Create收入方式(entity);
                    iPay2.收入(rep, entity);
                    break;
                default:
                    throw new InvalidUserOperationException("未填写有效的收付标志！");
            }
        }

        public static void 取消支付(IRepository rep, 凭证收支明细 entity)
        {
            switch (entity.收付标志)
            {
                case 收付标志.付:
                    I支付方式 iPay1 = Create支付方式(entity);
                    iPay1.取消支付(rep, entity);
                    break;
                case 收付标志.收:
                    I收入方式 iPay2 = Create收入方式(entity);
                    iPay2.取消收入(rep, entity);
                    break;
                default:
                    throw new InvalidUserOperationException("未填写有效的收付标志！");
            }
        }
    }

}
