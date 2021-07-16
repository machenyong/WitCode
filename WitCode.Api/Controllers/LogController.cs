using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WitCode.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        [HttpGet]
        public IActionResult TestExpression()
        {
            try
            {
                string a = "哈";
                Convert.ToInt32(a);
                return Ok(a);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
