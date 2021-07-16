using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WitCode.Service
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<UserEntity,UserModel>();

            //写数据操作
            ////新增
            //CreateMap<UserAddInput, UserEntity>();
            //CreateMap<UserUpdateInput, UserEntity>();

            ////修改
            //CreateMap<UserChangePasswordInput, UserEntity>();
            //CreateMap<UserUpdateBasicInput, UserEntity>();

            ////查询
            //CreateMap<UserEntity, UserGetOutput>().ForMember(
            //    d => d.RoleIds,
            //    m => m.MapFrom(s => s.Roles.Select(a => a.Id))
            //);

            //CreateMap<UserEntity, UserListOutput>().ForMember(
            //    d => d.RoleNames,
            //    m => m.MapFrom(s => s.Roles.Select(a => a.Name))
            //);
        }
    }
}
