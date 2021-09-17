using Gaia.IdP.DomainModel.Models;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using AutoMapper;

namespace Gaia.IdP.IdentityServer.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterAccountCommand, RegisterAccountRequest>();
            CreateMap<RegisterAccountRequest, AradUser>()
                .ForMember(o => o.UserName, opt => opt.MapFrom( o => o.PhoneNumber));

            CreateMap<LoginCommand, LoginRequest>();
            CreateMap<LoginCommand, LoginPreCheckRequest>();

            CreateMap<LoginViaOtpCommand, LoginViaOtpRequest>();
            CreateMap<LoginViaOtpCommand, LoginViaOtpPreCheckRequest>();

            CreateMap<ConfirmPhoneNumberCommand, ConfirmPhoneNumberRequest>();
            CreateMap<ResetPasswordViaTokenInSmsCommand, ResetPasswordViaTokenInSmsRequest>();
            CreateMap<ResetPasswordViaTokenInEmailCommand, ResetPasswordViaTokenInEmailRequest>();
        }
    }
}
