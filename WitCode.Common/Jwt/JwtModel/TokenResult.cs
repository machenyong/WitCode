using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WitCode.Common
{
    public class TokenResult
    {
        /// <summary>
        /// token字符串
        /// </summary>
        public string Access_token { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long Expires_in { get; set; }
    }
}
