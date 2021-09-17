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
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<IS4Entities.Client, IS4Entities.Client>()
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                        dest.Updated = DateTime.UtcNow;
                    });

            CreateMap<CreateClientCommand, CreateClientRequest>();
            CreateMap<UpdateClientCommand, UpdateClientRequest>();

            CreateMap<IS4Entities.Client, ClientListItem>()
                .ConstructUsing(src => CreateClientListItem(src));
        }

        private static ClientListItem CreateClientListItem(IS4Entities.Client src)
        {
            var id = src.Id;
            
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<IS4Entities.Client, IS4Models.Client>()
                    .ConstructUsing(src => src.ToModel());

                cfg.CreateMap<IS4Models.Client, ClientListItem>()
                    .AfterMap((src, dest) => {
                        dest.Id = id;
                    });
            })
            .CreateMapper();

            var model = mapper.Map<IS4Models.Client>(src);
            var result = mapper.Map<ClientListItem>(model);

            return result;
        }
    }
}
