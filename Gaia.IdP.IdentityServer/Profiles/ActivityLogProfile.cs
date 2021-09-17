using Gaia.IdP.DomainModel.Models;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using IdPEntities = Gaia.IdP.DomainModel.Models;
using IdPDtos = Gaia.IdP.Message.Responses;
using AutoMapper;

namespace Gaia.IdP.IdentityServer.Profiles
{
    public class ActivityLogProfile : Profile
    {
        public ActivityLogProfile()
        {
            CreateMap<IdPEntities.ActivityLog, IdPDtos.ActivityLog>()
                .ForMember(o => o.UserName, opt => opt.MapFrom(o => o.User.UserName));
        }
    }
}
