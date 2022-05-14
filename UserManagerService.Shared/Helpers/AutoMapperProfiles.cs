﻿using AutoMapper;
using UserManagerService.Entities;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserModel>();
            CreateMap<CompanyUser, CompanyUserModel>().ReverseMap();
            CreateMap<CompanyInputModel, CompanyModel>();
            CreateMap<CompanyModel, Company>();
        }
    }
}