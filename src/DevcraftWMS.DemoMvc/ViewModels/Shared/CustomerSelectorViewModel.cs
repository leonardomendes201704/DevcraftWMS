using DevcraftWMS.DemoMvc.ApiClients;

namespace DevcraftWMS.DemoMvc.ViewModels.Shared;

public sealed class CustomerSelectorViewModel
{
    public IReadOnlyList<CustomerDto> Customers { get; init; } = Array.Empty<CustomerDto>();
    public Guid? SelectedCustomerId { get; init; }
    public string ReturnUrl { get; init; } = "/";
    public string? ErrorMessage { get; init; }
    public bool IsAuthenticated { get; init; }
}
