using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WitCode.Common;
using WitCode.Common.Configs;
using WitCode.Repository;
using WitCode.Service;

namespace WitCode.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;

        //public IUserService _userService { set; get; } //属性注入
        public AccountController(IUserService userService,IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult TestAutoMapper()
        {
            UserEntity userEntity = new UserEntity() { 
                Id=1,Name="王五",Age=19
            };
            var userModel = _mapper.Map<UserEntity, UserModel>(userEntity);
            return Ok("好的");
        }
        [HttpGet]
        public string GetResult()
        {
            var res = _userService.GetMeg();
            return res;
        }

        [HttpGet]
        public IActionResult GetDbConfig()
        {
            DbConfig db = new DbConfig();
            DbFactory dbFactory = new DbFactory();
            dbFactory.DbHelper();
            MySqlDapperHelper my = new Common.MySqlDapperHelper();

            my.Execute("af");
           // var res = db.GetConfigs();
            return Ok();
        }
    }
}
