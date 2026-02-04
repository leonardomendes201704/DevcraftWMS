namespace DevcraftWMS.Portaria.Infrastructure;

public interface IPortariaAuthorizationService
{
    bool CanManageGateCheckins();
}

public sealed class PortariaAuthorizationService : IPortariaAuthorizationService
{
    private readonly IPortariaUserContext _userContext;

    public PortariaAuthorizationService(IPortariaUserContext userContext)
    {
        _userContext = userContext;
    }

    public bool CanManageGateCheckins()
        => _userContext.IsInRole(PortariaRoles.Admin)
           || _userContext.IsInRole(PortariaRoles.Portaria);
}
