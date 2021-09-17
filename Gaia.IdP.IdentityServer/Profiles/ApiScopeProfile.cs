using AutoMapper;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using Gaia.IdP.Message.Responses;
using IdentityServer4.EntityFramework.Mappers;
using IS4Entities = IdentityServer4.EntityFramework.Entities;
using IS4Models = IdentityServer4.Models;


namespace Gaia.IdP.IdentityServer.Profiles
{
    public class ApiScopeProfile : Profile
    {
        public ApiScopeProfile()
        {
            CreateMap<IS4Entities.ApiScope, IS4Entities.ApiScope>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateApiScopeCommand, CreateApiScopeRequest>();
            CreateMap<UpdateApiScopeCommand, UpdateApiScopeRequest>();

            CreateMap<IS4Entities.ApiScope, ApiScopeListItem>()
                .ConstructUsing(src => CreateApiScopeListItem(src));
        }

        private static ApiScopeListItem CreateApiScopeListItem(IS4Entities.ApiScope src)
        {
            var id = src.Id;
            
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<IS4Entities.ApiScope, IS4Models.ApiScope>()
                    .ConstructUsing(src => src.ToModel());

                cfg.CreateMap<IS4Models.ApiScope, ApiScopeListItem>()
                    .AfterMap((src, dest) => {
                        dest.Id = id;
                    });
            })
            .CreateMapper();

            var model = mapper.Map<IS4Models.ApiScope>(src);
            var result = mapper.Map<ApiScopeListItem>(model);

            return result;
        }
    }
}
