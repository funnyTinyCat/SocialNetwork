using AutoMapper;
using Cwk.Domain.Aggregates.PostAggregate;
using CwkSocial.Api.Contracts.Posts.Responses;

namespace CwkSocial.Api.MappingProfiles
{
    public class PostMappings : Profile
    {
        public PostMappings()
        {
            CreateMap<Post, PostResponse>();
            CreateMap<PostComment, PostCommentResponse>();
            CreateMap<Cwk.Domain.Aggregates.PostAggregate.PostInteraction,
                CwkSocial.Api.Contracts.Posts.Responses.PostInteraction>()
                //.ForMember(dest => dest.Author, opt =>
                //    opt.MapFrom(src => src.UserProfile.BasicInfo.FirstName +
                //    " " + src.UserProfile.BasicInfo.LastName))
                //.ForMember(dest => dest.Author.City,
                //    opt => opt.MapFrom(src => src.UserProfile.BasicInfo.CurrentCity))
                //.ForMember(dest => dest.Author.UserProfileId, opt =>
                //    opt.MapFrom(src => src.UserProfileId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    src.InteractionType.ToString()))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.UserProfile));
        }
    }
}
