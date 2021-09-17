using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gaia.IdP.Data.Models;
using Gaia.IdP.DomainModel.Customizations.Managers;
using Gaia.IdP.DomainModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class Identity
    {
        public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services)
        {
            services.Configure((IdentityOptions options) =>
            {
                // todo: set password options from identity settings (or by hard code)
                // options.Password.RequireDigit               = ???;
                // options.Password.RequireLowercase           = ???;
                // options.Password.RequireNonAlphanumeric     = ???;
                // options.Password.RequireUppercase           = ???;
                // options.Password.RequiredLength             = ???;
                // options.Password.RequiredUniqueChars        = ???;

                // todo: set lockout config if is needed
                // options.Lockout.AllowedForNewUsers          = ???;
                // options.Lockout.DefaultLockoutTimeSpan      = ???;
                // options.Lockout.MaxFailedAccessAttempts     = ???;
            });

            services.AddIdentity<AradUser, IdentityRole>()
                .AddUserManager<AradUserManager>()
                .AddUserValidator<PhoneNumberValidator>()
                .AddUserValidator<EmailValidator>()
                .AddEntityFrameworkStores<AradDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public class PhoneNumberValidator : IUserValidator<AradUser>
        {
            public Task<IdentityResult> ValidateAsync(UserManager<AradUser> manager, AradUser user)
            {
                if (user.PhoneNumber == null)
                    return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "EmptyPhoneNumber" }));

                var phoneNumberExists = manager.Users.Any(o => o.Id != user.Id && o.PhoneNumber == user.PhoneNumber);
                if (phoneNumberExists)
                    return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "DuplicatePhoneNumber" }));

                return Task.FromResult(IdentityResult.Success);
            }
        }

        public class EmailValidator : IUserValidator<AradUser>
        {
            public Task<IdentityResult> ValidateAsync(UserManager<AradUser> manager, AradUser user)
            {
                if (user.Email == null)
                    return Task.FromResult(IdentityResult.Success);

                var emailExists = manager.Users.Any(o => o.Id != user.Id && o.Email == user.Email);
                if (emailExists)
                    return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail" }));

                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}
