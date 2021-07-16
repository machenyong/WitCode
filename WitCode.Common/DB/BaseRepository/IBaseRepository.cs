using System;
using System.Collections.Generic;

namespace WitCode.Common
{
    //数据库操作通用接口
    public interface IBaseRepository
    {
        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        int Execute(string sql, object param = null);

        /// <summary>
        /// 查询 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<T> Query<T>(string sql, object param = null) where T : class, new();

        /// <summary>
        /// 事务增删改方法一
        /// </summary>
        /// <param name="sqlarr"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        int ExecuteTransaction(string[] sqlarr, object[] param);

        /// <summary>
        /// 事务增删改方法二
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        int ExecuteTransaction(Dictionary<string, object> dic);
    }
}
