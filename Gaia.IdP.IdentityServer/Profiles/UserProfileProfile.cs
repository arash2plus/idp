using Gaia.IdP.DomainModel.Models;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using AutoMapper;
using Gaia.IdP.Message.Responses;


namespace Gaia.IdP.IdentityServer.Profiles
{
    public class UserProfileProfile : Profile
    {
        public UserProfileProfile()
        {
            CreateMap<UpdateUserProfileCommand, UpdateUserProfileRequest>();
            CreateMap<UpdateUserProfileRequest, AradUser>();
            CreateMap<AradUser, UserProfile>();
        }
    }
}
