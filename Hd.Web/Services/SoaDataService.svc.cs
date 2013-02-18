using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ComponentArt.SOA.UI;
using System.Reflection;
using Feng;
using Feng.Utils;

namespace Hd.Web.Services
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SoaDataService : SoaDataGridService
    {
        static SoaDataService()
        {
            Feng.Utility.InitProgram();
        }
        public override SoaDataGridGroupResponse Group(SoaDataGridGroupRequest request)
        {
            //List<object> list = new List<object>();

            //DataSet oDS = LoadData();
            //DataView oDataView = new DataView(oDS.Tables[0]);

            //oDataView.Sort = request.Groupings.ToSqlString();
            //oDataView.RowFilter = request.Filters.ToSqlString();

            //string field = request.Groupings[0].Column.Name;

            SoaDataGridGroupResponse response = new SoaDataGridGroupResponse();

            //int iGroupCount = 0;
            //object _lastValue = Guid.NewGuid();
            //for (int i = 0; i < oDataView.Count; i++)
            //{
            //    if ((_lastValue == null && oDataView[i][field] != null) ||
            //      _lastValue != null && !_lastValue.Equals(oDataView[i][field]))
            //    {
            //        _lastValue = oDataView[i][field];

            //        if (iGroupCount >= request.Offset && iGroupCount < request.Offset + request.Count)
            //        {
            //            SoaDataGridGroup oGroup = new SoaDataGridGroup();
            //            oGroup.Column = field;
            //            oGroup.GroupValue = _lastValue;

            //            response.Groups.Add(oGroup);

            //            list.Add(_lastValue);
            //        }

            //        iGroupCount++;
            //    }
            //}

            //response.GroupCount = iGroupCount;

            return response;
        }

        public override SoaDataGridSelectResponse Select(SoaDataGridSelectRequest request)
        {
            SoaDataGridSelectResponse response = new SoaDataGridSelectResponse();
            if (request.Tag == null || string.IsNullOrEmpty(request.Tag.ToString()))
                return response;

            try
            {
                //WindowInfo windowInfo = ADInfoBll.Instance.GetWindowInfo(request.Tag.ToString());
                //IList<WindowTabInfo> tabInfos = null;
                //if (windowInfo != null)
                //{
                //    tabInfos = ADInfoBll.Instance.GetWindowTabInfo(windowInfo.Id);
                //}

                Dictionary<string, string> dict = Helper.StringToDictionary(request.Tag.ToString());

                if (!dict.ContainsKey("winTab") || string.IsNullOrEmpty(dict["winTab"]))
                    return response;

                WindowTabInfo tabInfo = ADInfoBll.Instance.GetWindowTabInfo(dict["winTab"]);

                if (tabInfo != null)
                {
                    ISearchManager sm = Feng.Windows.Forms.ArchiveFormFactory.GenerateSearchManager(tabInfo, null);
                    Feng.Grid.BoundGridExtention.CreateSearchManagerEagerFetchs(sm, tabInfo.GridName);

                    IList<ISearchExpression> searchExps = new List<ISearchExpression>();
                    if (dict.ContainsKey("se") && !string.IsNullOrEmpty(dict["se"]))
                    {
                        string exp = dict["se"];
                        //exp = HttpUtility.UrlDecode(exp, System.Text.Encoding.GetEncoding("gb2312"));
                        //exp = HttpContext.Current.Server.UrlDecode(exp);
                        searchExps.Add(SearchExpression.Parse(exp));
                    }

                    //if (request.Filters != null)
                    //{
                    //    //searchs.Add(SearchExpression.Parse(request.Tag.ToString()));
                    //    foreach (SoaDataGridDataCondition i in request.Filters)
                    //    {
                    //        switch (i.Operand)
                    //        {
                    //            case SoaDataGridDataConditionOperand.Contains:
                    //                searchExps.Add(SearchExpression.Like(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.Equals:
                    //                searchExps.Add(SearchExpression.Eq(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.GreaterThan:
                    //                searchExps.Add(SearchExpression.Gt(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.GreaterThanOrEqual:
                    //                searchExps.Add(SearchExpression.Ge(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.In:
                    //                searchExps.Add(SearchExpression.InG(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.LessThan:
                    //                searchExps.Add(SearchExpression.Lt(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.LessThanOrEqual:
                    //                searchExps.Add(SearchExpression.Le(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.NotEqualTo:
                    //                searchExps.Add(SearchExpression.NotEq(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.NotIn:
                    //                searchExps.Add(SearchExpression.NotInG(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //            case SoaDataGridDataConditionOperand.StartsWith:
                    //                searchExps.Add(SearchExpression.Like(i.DataFieldName, i.DataFieldValue));
                    //                break;
                    //        }
                    //    }
                    //}

                    IList<ISearchOrder> searchOrders = new List<ISearchOrder>();
                    if (request.Sortings != null)
                    {
                        foreach (SoaDataGridSorting i in request.Sortings)
                        {
                            searchOrders.Add(i.Direction == SoaSortDirection.Ascending ? SearchOrder.Asc(i.Column.Name) : SearchOrder.Desc(i.Column.Name));
                        }
                    }

                    sm.FirstResult = request.Offset;
                    sm.MaxResult = request.Count;

                    List<List<object>> arMessages;

                    object ret = sm.FindData(searchExps, searchOrders);
                    System.Data.DataTable dt = ret as System.Data.DataTable;

                    if (dt == null)
                    {
                        arMessages = GenerateData(ret as IEnumerable, tabInfo.GridName, request.Columns);
                    }
                    else
                    {
                        arMessages = GenerateData(dt.Rows, tabInfo.GridName, request.Columns);
                    }

                    //IList<Dictionary<string, object>> values = null;
                    //

                    //using (DataProcess dp = new DataProcess())
                    //{
                    //    if (dt == null)
                    //    {
                    //        // should be IEnumerable because IList<T> not support IList
                    //        IEnumerable data = ret as IEnumerable;
                    //        values = dp.Process(data, tabInfo.GridName);
                    //    }
                    //    else
                    //    {
                    //        values = dp.Process(dt.Rows, tabInfo.GridName);
                    //    }
                    //}

                    //foreach (Dictionary<string, object> obj in values)
                    //{
                    //    List<object> msg = new List<object>();

                    //    foreach (SoaDataGridColumn oColumn in request.Columns)
                    //    {
                    //        if (!obj.ContainsKey(oColumn.Name))
                    //        {
                    //            msg.Add(null);
                    //        }
                    //        else
                    //        {
                    //            msg.Add(obj[oColumn.Name]);
                    //        }
                    //    }

                    //    arMessages.Add(msg);
                    //}
                    response.Data = arMessages;

                    if (sm.EnablePage)
                    {
                        response.ItemCount = sm.FindDataCount(searchExps);
                    }
                    else
                    {
                        response.ItemCount = arMessages.Count;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalide tabName of " + dict["winTab"]);
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }

            return response;
        }

        private List<List<object>> GenerateData(IEnumerable list, string gridName, List<SoaDataGridColumn> columns)
        {
            List<List<object>> ret = new List<List<object>>();
            foreach (object row in list)
            {
                ret.Add(GenerateData(row, gridName, columns));
            }
            return ret;
        }
        private List<object> GenerateData(object entity, string gridName, List<SoaDataGridColumn> columns)
        {
            Dictionary<string, object> dict;
            using (DataProcess dp = new DataProcess())
            {
                dict = dp.Process(entity, gridName);
            }

            List<object> ret = new List<object>();

            foreach (SoaDataGridColumn col in columns)
            {
                if (dict.ContainsKey(col.Name))
                {
                    ret.Add(dict[col.Name]);
                }
                else
                {
                    object o = null;
                    try
                    {
                        o = EntityHelper.GetPropertyValue(entity, col.Name);
                    }
                    catch (Exception)    // 不一定有这个属性，例如大写金额
                    {
                    }
                    ret.Add(o);
                }
            }

            return ret;
        }
    }
}
