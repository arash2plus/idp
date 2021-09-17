using Gaia.IdP.DomainModel.Models;
using Gaia.IdP.Infrastructure.Enums;
using Gaia.IdP.Infrastructure.Exceptions;
using Gaia.IdP.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gaia.IdP.DomainModel.Customizations.Managers
{
    public class AradUserManager : UserManager<AradUser>
    {
        public AradUserManager(
            IUserStore<AradUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<AradUser> passwordHasher,
            IEnumerable<IUserValidator<AradUser>> userValidators,
            IEnumerable<IPasswordValidator<AradUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<AradUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<bool> IsEmailConfirmedAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return await this.Users.AnyAsync(o => o.Id == id && o.EmailConfirmed);
        }

        public async Task<bool> IsPhoneNumberConfirmedAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return await this.Users.AnyAsync(o => o.Id == id && o.PhoneNumberConfirmed);
        }

        public async Task<AradUser> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await this.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public void HandleIdentityResult(IdentityResult identityResult)
        {
            if (!identityResult.Succeeded)
            {
                var errorCodes = identityResult.Errors.Select(e => e.Code);

                if (errorCodes.Any(o => o.Contains("DuplicateUserName")))
                    throw new DomainBadRequestException("userName", ErrorMessage.duplicateUserName);

                else if (errorCodes.Any(o => o.Contains("DuplicateEmail")))
                    throw new DomainBadRequestException("email", ErrorMessage.duplicateEmail);

                else if (errorCodes.Any(o => o.Contains("DuplicatePhoneNumber")))
                    throw new DomainBadRequestException("phoneNumber", ErrorMessage.duplicatePhoneNumber);

                else if (errorCodes.Any(o => o.Contains("InvalidEmail")))
                    throw new DomainBadRequestException("email", ErrorMessage.invalidEmail);

                else if (errorCodes.Any(o => o.Contains("EmptyPhoneNumber")))
                    throw new DomainBadRequestException("phoneNumber", ErrorMessage.emptyPhoneNumber);

                else if (errorCodes.Any(o => o.Contains("InvalidPhoneNumber")))
                    throw new DomainBadRequestException("phoneNumber", ErrorMessage.invalidPhoneNumber);

                else if (errorCodes.Any(o => o.Contains("Password")))
                    throw new DomainBadRequestException("password", errorCodes.GetString());

                throw new DomainBadRequestException("unknownField", errorCodes.GetString());
            }
        }
    }
}