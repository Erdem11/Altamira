using Altamira.Api.Models.Users;
using Altamira.Entities;
using AutoMapper;

namespace Altamira.Api.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserModel, User>();
        }
    }
}