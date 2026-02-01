using MediatR;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Auth.Queries;

public sealed class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, RequestResult<UserProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public GetProfileQueryHandler(ICurrentUserService currentUserService, IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<RequestResult<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return RequestResult<UserProfileDto>.Failure("auth.unauthorized", "User not authenticated.");
        }

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (user is null)
        {
            return RequestResult<UserProfileDto>.Failure("auth.user_not_found", "User not found.");
        }

        var dto = new UserProfileDto(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, user.LastLoginUtc);
        return RequestResult<UserProfileDto>.Success(dto);
    }
}
