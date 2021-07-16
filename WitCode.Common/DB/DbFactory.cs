using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WitCode.Common.Configs;


namespace WitCode.Common
{
    public class DbFactory
    {
        //选择数据库
        public static string conDb= DbConfig.UseDb;
        
        //通用CRUD接口
        IBaseRepository _baseRepository = null;
        
        /// <summary>
        /// 工厂获取选择的数据库对应DapperHelper
        /// </summary>
        /// <returns></returns>
        public IBaseRepository DbHelper()
        {
            switch (conDb)
            {
                case "SqlServer":
                    _baseRepository = new SqlServerDapperHelper();
                    break;
                case "MySql":
                    _baseRepository = new MySqlDapperHelper();
                    break;
                default:
                    break;
            }
            return _baseRepository;
        }
    }
}
