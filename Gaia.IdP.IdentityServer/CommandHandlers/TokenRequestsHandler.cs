using Gaia.IdP.DomainModel.Customizations.Managers;
using Gaia.IdP.DomainModel.Customizations.Tokens;
using Gaia.IdP.Infrastructure.Enums;
using Gaia.IdP.Infrastructure.Exceptions;
using Gaia.IdP.Message.Requests;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Gaia.MessageBus.EventBusKafka.Abstractions;
using Gaia.IdP.Message.Events;

namespace Gaia.IdP.IdentityServer.CommandHandlers
{
    public class TokenRequestsHandler : 
        IRequestHandler<SendLoginTokenViaSmsRequest>,
        IRequestHandler<SendPhoneNumberConfirmationTokenRequest>,
        IRequestHandler<SendResetPasswordTokenViaSmsRequest>,
        IRequestHandler<SendResetPasswordTokenViaEmailRequest>
    {
        private readonly AradUserManager _userManager;
        private readonly IMapper _mapper;
        private readonly IEventBusKafka _eventBus;

        public TokenRequestsHandler(
            AradUserManager userManager,
            IEventBusKafka eventBus,
            IMapper mapper)
        {
            _userManager = userManager;
            _eventBus = eventBus;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SendLoginTokenViaSmsRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            var isPhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
            if (!isPhoneNumberConfirmed)
                throw new DomainException(ErrorStatusCode.conflict, ErrorMessage.phoneNumberNotConfirmed);

            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, TokenPurposes.OtpLogin);

            var @event = new LoginSmsTokenGeneratedEvent { 
                Token = token,
                Receipient = user.PhoneNumber
            };
            await _eventBus.PublishAsync(@event);

            return Unit.Value;
        }

        public async Task<Unit> Handle(SendPhoneNumberConfirmationTokenRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            if (user.PhoneNumberConfirmed)
                throw new DomainException(ErrorStatusCode.conflict, ErrorMessage.alreadyConfirmed);

            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, TokenPurposes.PhoneNumberConfirmation);

            var @event = new PhoneNumberConfirmationTokenGeneratedEvent { 
                Token = token,
                Receipient = user.PhoneNumber
            };
            await _eventBus.PublishAsync(@event);

            return Unit.Value;
        }

        public async Task<Unit> Handle(SendResetPasswordTokenViaSmsRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByPhoneNumberAsync(request.PhoneNumber);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, TokenPurposes.ResetPasswordViaSms);

            var @event = new ResetPasswordSmsTokenGeneratedEvent { 
                Token = token,
                Receipient = user.PhoneNumber
            };
            await _eventBus.PublishAsync(@event);

            return Unit.Value;
        }


        public async Task<Unit> Handle(SendResetPasswordTokenViaEmailRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var @event = new ResetPasswordEmailTokenGeneratedEvent { 
                Token = token,
                ReceipientName = user.FirstName ?? user.LastName ?? user.UserName,
                ReceipientAddress = user.Email
            };
            await _eventBus.PublishAsync(@event);

            return Unit.Value;
        }
    }
}
