using System;
using System.Linq;
using AutoMapper;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using Gaia.IdP.Message.Responses;
using IdentityServer4.EntityFramework.Mappers;
using IS4Entities = IdentityServer4.EntityFramework.Entities;
using IS4Models = IdentityServer4.Models;

namespace Gaia.IdP.IdentityServer.Profiles
{
    public class IdentityResourceProfile : Profile
    {
        public IdentityResourceProfile()
        {
            CreateMap<IS4Entities.IdentityResource, IS4Entities.IdentityResource>()
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                        dest.Updated = DateTime.UtcNow;
                    });

            CreateMap<CreateIdentityResourceCommand, CreateIdentityResourceRequest>();
            CreateMap<UpdateIdentityResourceCommand, UpdateIdentityResourceRequest>();

            CreateMap<IS4Entities.IdentityResource, IdentityResourceListItem>()
                .ConstructUsing(src => CreateIdentityResourceListItem(src));
        }

        private static IdentityResourceListItem CreateIdentityResourceListItem(IS4Entities.IdentityResource src)
        {
            var id = src.Id;
            
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<IS4Entities.IdentityResource, IS4Models.IdentityResource>()
                    .ConstructUsing(src => src.ToModel());

                cfg.CreateMap<IS4Models.IdentityResource, IdentityResourceListItem>()
                    .AfterMap((src, dest) => {
                        dest.Id = id;
                    });
            })
            .CreateMapper();

            var model = mapper.Map<IS4Models.IdentityResource>(src);
            var result = mapper.Map<IdentityResourceListItem>(model);

            return result;
        }
    }
}
