using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gaia.IdP.DomainModel.Customizations.Managers;
using Gaia.IdP.DomainModel.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace Gaia.IdP.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<AradUser> _claimsFactory;
        private readonly AradUserManager _userManager;

        public ProfileService(
            IUserClaimsPrincipalFactory<AradUser> claimsFactory,
            AradUserManager userManager)
        {
            _claimsFactory = claimsFactory;
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var claimsPrincipal = await _claimsFactory.CreateAsync(user);

            var claims = claimsPrincipal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            // Add custom claims in token here based on user properties or any other source
            claims.Add(new Claim("national_id", user.NationalId ?? string.Empty));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}