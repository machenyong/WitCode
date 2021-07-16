using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WitCode.Common
{
    public class JWTTokenOptions
    {
        /// <summary>
        /// 订阅者
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 过期时间 单位秒
        /// </summary>
        public long Expire { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string Secret { get; set; }

      
       
    }
}
