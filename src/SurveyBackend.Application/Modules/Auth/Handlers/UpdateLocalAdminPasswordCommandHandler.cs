using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Commands.Admin;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

    public sealed class UpdateLocalAdminPasswordCommandHandler : ICommandHandler<UpdateLocalAdminPasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;

    public UpdateLocalAdminPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task<bool> HandleAsync(UpdateLocalAdminPasswordCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsSuperAdmin)
        {
            throw new UnauthorizedAccessException("Sadece süper admin kullanıcılar bu işlemi gerçekleştirebilir.");
        }

        var adminUser = await _userRepository.GetLocalByUsernameAsync("admin", cancellationToken);
        if (adminUser is null)
        {
            throw new InvalidOperationException("Yerel admin kullanıcısı bulunamadı.");
        }

        var hash = _passwordHasher.Hash(request.NewPassword);
        adminUser.UpdateSuperAdminPassword(hash);
        await _userRepository.UpdateAsync(adminUser, cancellationToken);
        return true;
    }
}
