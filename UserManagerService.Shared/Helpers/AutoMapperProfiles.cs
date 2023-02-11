﻿using AutoMapper;
using UserManagerService.Entities;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.Roles;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserModel>();
            CreateMap<CompanyUser, CompanyUserModel>().ReverseMap();
            CreateMap<CompanyInputModel, Company>();
            CreateMap<CompanyModel, Company>().ReverseMap();

            CreateMap<RoleInputModel, Role>();
            CreateMap<Role, RoleModel>();
            CreateMap<UserRoleInputModel, UserRole>();
            CreateMap<UserRoleModel, UserRoleModel>();

            CreateMap<UserProfile, MyProfile>();
            CreateMap<LoginInputModel, LoginToCompanyInputModel>();
            CreateMap<User, LoginOutputWithSession>();
        }
    }
}