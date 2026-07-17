using AutoMapper;
using Cwk.Domain.Aggregates.UserProfileAggregate;
using CwkSocial.Application.UserProfiles.Commands;
namespace CwkSocial.Application.MappingProfiles
{
    internal class UserProfileMap_bck : Profile
    {
        public UserProfileMap_bck()
        {
            CreateMap<CreateUserCommand_bck, BasicInfo>();
            //CreateMap<UserProfile, UserProfileResponse>();
        }
    }
}
