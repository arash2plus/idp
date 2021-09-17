using Gaia.IdP.DomainModel.Customizations.Managers;
using Gaia.IdP.DomainModel.Models;
using Gaia.IdP.Infrastructure.Enums;
using Gaia.IdP.Infrastructure.Exceptions;
using Gaia.IdP.Message.Requests;
using Gaia.IdP.Message.Responses;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Gaia.IdP.Data.Models;

namespace Gaia.IdP.IdentityServer.CommandHandlers
{
    public class UserProfileRequestsHandler :
        IRequestHandler<GetUserProfileRequest, UserProfile>,
        IRequestHandler<UpdateUserProfileRequest, UserProfile>
    {
        private readonly AradDbContext _db;
        private readonly AradUserManager _userManager;
        private readonly IMapper _mapper;

        public UserProfileRequestsHandler(
            AradDbContext db,
            AradUserManager userManager,
            IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserProfile> Handle(GetUserProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            var result = _mapper.Map<UserProfile>(user);

            return result;
        }

        public async Task<UserProfile> Handle(UpdateUserProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.userNotFound);

            _mapper.Map(request, user);

            _db.Set<AradUser>().Update(user);
            await _db.SaveChangesAsync();

            var result = _mapper.Map<UserProfile>(user);

            return result;
        }
    }
}
