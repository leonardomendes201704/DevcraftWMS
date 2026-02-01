using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.ViewModels.Auth;

public sealed class LoginViewModel
{
    [EmailAddress]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public string? Token { get; set; }
}


