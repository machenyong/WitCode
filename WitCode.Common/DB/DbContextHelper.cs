

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WitCode.Common.Configs;

namespace WitCode.Common
{
    public class DbContextHelper:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DbFactory.conDb== "SqlServer")
            {
                optionsBuilder.UseSqlServer(DbConfig.SqlServerConStr);
            }
            else if (DbFactory.conDb == "MySql")
            {
                optionsBuilder.UseMySQL(DbConfig.MySqlConStr);
            }
            
            base.OnConfiguring(optionsBuilder);
        }

    }
}
