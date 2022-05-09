using AutoMapper;
using UserManagerService.Entities;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //CreateMap<User, GetUserResponse>().ReverseMap();
            CreateMap<User, UserModel>();
        }
    }
}