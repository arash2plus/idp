using IS4Entities = IdentityServer4.EntityFramework.Entities;
using IS4Models = IdentityServer4.Models;
using AutoMapper;
using System.Linq;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using IdentityServer4.EntityFramework.Mappers;
using Gaia.IdP.Message.Responses;
using System;

namespace Gaia.IdP.IdentityServer.Profiles
{
    public class ApiResourceProfile : Profile
    {
        public ApiResourceProfile()
        {
            CreateMap<IS4Entities.ApiResource, IS4Entities.ApiResource>()
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                        dest.Updated = DateTime.UtcNow;
                    });

            CreateMap<CreateApiResourceCommand, CreateApiResourceRequest>();
            CreateMap<UpdateApiResourceCommand, UpdateApiResourceRequest>();

            CreateMap<IS4Entities.ApiResource, ApiResourceListItem>()
                .ConstructUsing(src => CreateApiResourceListItem(src));
        }

        private static ApiResourceListItem CreateApiResourceListItem(IS4Entities.ApiResource src)
        {
            var id = src.Id;
            
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<IS4Entities.ApiResource, IS4Models.ApiResource>()
                    .ConstructUsing(src => src.ToModel());

                cfg.CreateMap<IS4Models.ApiResource, ApiResourceListItem>()
                    .AfterMap((src, dest) => {
                        dest.Id = id;
                    });
            })
            .CreateMapper();

            var model = mapper.Map<IS4Models.ApiResource>(src);
            var result = mapper.Map<ApiResourceListItem>(model);

            return result;
        }
    }
}
