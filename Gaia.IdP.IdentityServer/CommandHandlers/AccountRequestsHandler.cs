using Gaia.IdP.DomainModel.Customizations.Managers;
using Gaia.IdP.DomainModel.Customizations.Tokens;
using Gaia.IdP.DomainModel.Models;
using Gaia.IdP.Infrastructure.Enums;
using Gaia.IdP.Infrastructure.Exceptions;
using Gaia.IdP.Message.Requests;
using Gaia.IdP.Message.Responses;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;
using Gaia.IdP.IdentityServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using IdPEntities = Gaia.IdP.DomainModel.Models;
using IdPDtos = Gaia.IdP.Message.Responses;
using System;
using Gaia.IdP.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.RegularExpressions;

namespace Gaia.IdP.IdentityServer.CommandHandlers
{
    public class AccountRequestsHandler :
        IRequestHandler<RegisterAccountRequest, RegisterAccountResponse>,
        IRequestHandler<LoginPreCheckRequest>,
        IRequestHandler<LoginRequest>,
        IRequestHandler<LoginViaOtpPreCheckRequest>,
        IRequestHandler<LoginViaOtpRequest>,

        IRequestHandler<LogoutRequest, LogoutResponse>,

        IRequestHandler<ConfirmPhoneNumberRequest>,
        IRequestHandler<ResetPasswordViaTokenInSmsRequest>,
        IRequestHandler<ResetPasswordViaTokenInEmailRequest>
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly AradDbContext _aradDbContext;
        private readonly AradUserManager _userManager;
        private readonly SignInManager<AradUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly GoogleRecaptchaVerificationService _captchaVerificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountRequestsHandler> _logger;
        private readonly IWebHostEnvironment _environment;

        public AccountRequestsHandler(
            ConfigurationDbContext configurationDbContext,
            AradDbContext aradDbContext,
            AradUserManager userManager,
            SignInManager<AradUser> signInManager,
            IIdentityServerInteractionService interaction,
            GoogleRecaptchaVerificationService captchaVerificationService,
            IEventService events,
            IMapper mapper,
            ILogger<AccountRequestsHandler> logger,
            IWebHostEnvironment environment)
        {
            _configurationDbContext = configurationDbContext;
            _aradDbContext = aradDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
            _captchaVerificationService = captchaVerificationService;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
        }

        public async Task<RegisterAccountResponse> Handle(RegisterAccountRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<AradUser>(request);

            var identityResult = await _userManager.CreateAsync(user, request.Password);

            _userManager.HandleIdentityResult(identityResult);

            var result = new RegisterAccountResponse
            {
                Username = user.UserName
            };

            return result;
        }

        public async Task<Unit> Handle(LoginPreCheckRequest request, CancellationToken cancellationToken)
        {
            // if (_environment.IsProduction())
            // {
            //     var verified = await _captchaVerificationService.Verify(request.CaptchaToken);
            //     if (!verified)
            //         throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.captchaTokenNotVerified);
            // }

            var user = await _userManager.FindByPhoneNumberAsync(request.PhoneNumber);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            await PreSignInCheck(user, request.HttpRequest, request.ReturnUrl);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                await StoreLoginActivity(user.Id, request.HttpRequest, request.ReturnUrl, false, ErrorMessage.invalidUsernameOrPassword.ToString());
                throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.invalidUsernameOrPassword);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            Event evt;

            // if (_environment.IsProduction())
            // {
            //     var verified = await _captchaVerificationService.Verify(request.CaptchaToken);
            //     if (!verified)
            //         throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.captchaTokenNotVerified);
            // }

            var user = await _userManager.FindByPhoneNumberAsync(request.PhoneNumber);
            if (user == null)
            {
                evt = new UserLoginFailureEvent(user.UserName, ErrorMessage.userNotFound.ToString());
                await _events.RaiseAsync(evt);

                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);
            }

            await PreSignInCheck(user, request.HttpRequest, request.ReturnUrl);

            // TODO: get confirmation from PO about user lockout on sign-in failure
            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (!signInResult.Succeeded)
            {

                evt = new UserLoginFailureEvent(user.UserName, ErrorMessage.invalidUsernameOrPassword.ToString());
                await _events.RaiseAsync(evt);

                throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.invalidUsernameOrPassword);
            }

            evt = new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName);
            await _events.RaiseAsync(evt);
            await StoreLoginActivity(user.Id, request.HttpRequest, request.ReturnUrl, true);

            if (_userManager.SupportsUserLockout)
                await _userManager.ResetAccessFailedCountAsync(user);

            return Unit.Value;
        }

        public async Task<Unit> Handle(LoginViaOtpRequest request, CancellationToken cancellationToken)
        {
            Event evt;
            
            var user = await _userManager.FindByPhoneNumberAsync(request.PhoneNumber);
            if (user == null)
            {
                evt = new UserLoginFailureEvent(user.UserName, ErrorMessage.userNotFound.ToString());
                await _events.RaiseAsync(evt);

                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);
            }

            var verified = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, TokenPurposes.OtpLogin, request.Token);
            if (!verified)
            {
                evt = new UserLoginFailureEvent(user.UserName, ErrorMessage.tokenNotVerified.ToString());
                await _events.RaiseAsync(evt);
                
                throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.tokenNotVerified);
            }

            await PreSignInCheck(user, request.HttpRequest, request.ReturnUrl);

            // TODO: make decision about choosing appropriate SignIn method:
            await _signInManager.SignInAsync(user, true, "otp-login");

            evt = new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName);
            await _events.RaiseAsync(evt);
            await StoreLoginActivity(user.Id, request.HttpRequest, request.ReturnUrl, true);

            if (_userManager.SupportsUserLockout)
                await _userManager.ResetAccessFailedCountAsync(user);

            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(LoginViaOtpPreCheckRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByPhoneNumberAsync(request.PhoneNumber);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            var verified = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, TokenPurposes.OtpLogin, request.Token);
            if (!verified)
            {
                await StoreLoginActivity(user.Id, request.HttpRequest, request.ReturnUrl, false, ErrorMessage.tokenNotVerified.ToString());
                throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.tokenNotVerified);
            }

            await PreSignInCheck(user, request.HttpRequest, request.ReturnUrl);

            return Unit.Value;
        }

        public async Task<LogoutResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            var logout = await _interaction.GetLogoutContextAsync(request.LogoutId);
            string clientId = null;

            // TODO: PostLogoutRedirectUri must be retreived from logout object.

            // It will be filled based on post_logout_redirect_uri configured on oidc-client configuration.
            // This uri will be validated on EndSessionRequest to being checked if uri is already seed for
            // requesting client.

            // there is an open issue on dotnet/aspnetcore with follwing title and link
            // AutoRedirectEndSessionEndpoint doesn't redirect to the configured PostLogoutUri #22170
            // "https://github.com/dotnet/aspnetcore/issues/22170"

            //  Microsft team has declared that they moved this issue to their Backlog milestone at Sep 25, 2020

            var postLogoutRedirectUri = "";
            if (logout != null && logout.ClientIds != null && logout.ClientIds.Any())
            {
                clientId = logout.ClientIds.First();
                postLogoutRedirectUri = !string.IsNullOrEmpty(logout.PostLogoutRedirectUri) ?
                    logout.PostLogoutRedirectUri :
                    _configurationDbContext.Set<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>()
                        .FirstOrDefault(o => o.Client.ClientId == clientId)
                        ?.PostLogoutRedirectUri;
            }

            var result = new LogoutResponse
            {
                LogoutId = request.LogoutId,
                ClientName = logout?.ClientName,
                PostLogoutRedirectUri = postLogoutRedirectUri,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            if (request.User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(request.User.GetSubjectId(), request.User.GetDisplayName()));
                await StoreLogoutActivity(request.User.GetSubjectId(), request.HttpRequest, clientId);
            }

            return result;
        }

        public async Task<Unit> Handle(ConfirmPhoneNumberRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            if (user.PhoneNumberConfirmed)
                throw new DomainException(ErrorStatusCode.conflict, ErrorMessage.alreadyConfirmed);

            var verified = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, TokenPurposes.PhoneNumberConfirmation, request.Token);
            if (!verified)
                throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.tokenNotVerified);

            user.PhoneNumberConfirmed = true;
            var identityResult = await _userManager.UpdateAsync(user);

            _userManager.HandleIdentityResult(identityResult);

            return Unit.Value;
        }

        public async Task<Unit> Handle(ResetPasswordViaTokenInSmsRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByPhoneNumberAsync(request.PhoneNumber);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            var verified = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, TokenPurposes.ResetPasswordViaSms, request.Token);
            if (!verified)
                throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.tokenNotVerified);

            var tempToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var identityResult = await _userManager.ResetPasswordAsync(user, tempToken, request.NewPassword);
            _userManager.HandleIdentityResult(identityResult);

            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                identityResult = await _userManager.UpdateAsync(user);
                _userManager.HandleIdentityResult(identityResult);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(ResetPasswordViaTokenInEmailRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            var identityResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!identityResult.Succeeded)
                throw new DomainException(ErrorStatusCode.unauthorized, ErrorMessage.tokenNotVerified);

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                identityResult = await _userManager.UpdateAsync(user);
                _userManager.HandleIdentityResult(identityResult);
            }

            return Unit.Value;
        }

        // --------------

        private async Task PreSignInCheck(AradUser user, HttpRequest httpRequest, string returnUrl)
        {
            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                await StoreLoginActivity(user.Id, httpRequest, returnUrl, false, ErrorMessage.requiresTwoFactor.ToString());
                throw new DomainException(ErrorStatusCode.forbidden, ErrorMessage.requiresTwoFactor);
            }

            if (!await _signInManager.CanSignInAsync(user))
            {
                if (_userManager.Options.SignIn.RequireConfirmedPhoneNumber && !await _userManager.IsPhoneNumberConfirmedAsync(user))
                {
                    await StoreLoginActivity(user.Id, httpRequest, returnUrl, false, ErrorMessage.phoneNumberNotConfirmed.ToString());
                    throw new DomainException(ErrorStatusCode.forbidden, ErrorMessage.phoneNumberNotConfirmed);
                }

                if (_userManager.Options.SignIn.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    await StoreLoginActivity(user.Id, httpRequest, returnUrl, false, ErrorMessage.emailNotConfirmed.ToString());
                    throw new DomainException(ErrorStatusCode.forbidden, ErrorMessage.emailNotConfirmed);
                }

                await StoreLoginActivity(user.Id, httpRequest, returnUrl, false, "can't sign in");
            }

            if ((_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)))
            {
                await StoreLoginActivity(user.Id, httpRequest, returnUrl, false, ErrorMessage.isLockedOut.ToString());
                throw new DomainException(ErrorStatusCode.forbidden, ErrorMessage.isLockedOut);
            }
        }

        private Task StoreLoginActivity(string userId, HttpRequest httpRequest, string returnUrl, bool succeed, string errorMessage = null)
        {
            string clientId;
            
            try
            {
                returnUrl = WebUtility.UrlDecode(returnUrl);
                var pattern = "client_id=([^&]*)";
                var match = Regex.Match(returnUrl, pattern, RegexOptions.IgnoreCase);
                clientId = match.Groups[1].Value;
            }
            catch
            {
                clientId = "unknown";
            }

            var activity = new IdPEntities.ActivityLog {
                UserId = userId,
                Type = IdPEntities.ActivityLog.ActivityType.login,
                ClientId = clientId,
                Date = DateTime.UtcNow,
                IP = httpRequest.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = httpRequest.Headers["User-Agent"].ToString(),
                Succeed = succeed,
                ErrorMessage = errorMessage
            };

            _aradDbContext.ActivityLogs.Add(activity);
            _aradDbContext.SaveChanges();

            return Task.CompletedTask;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        }

        private Task StoreLogoutActivity(string userId, HttpRequest httpRequest, string clientId)
        {
            var activity = new IdPEntities.ActivityLog {
                UserId = userId,
                Type = IdPEntities.ActivityLog.ActivityType.logout,
                ClientId = clientId,
                Date = DateTime.UtcNow,
                IP = httpRequest.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = httpRequest.Headers["User-Agent"].ToString(),
                Succeed = true
            };

            _aradDbContext.ActivityLogs.Add(activity);
            _aradDbContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
