using Microsoft.Extensions.Options;
using DevcraftWMS.Application.Abstractions.Email;

namespace DevcraftWMS.Infrastructure.Email;

public sealed class EmailDefaultsProvider : IEmailDefaults
{
    private readonly EmailSmtpOptions _options;

    public EmailDefaultsProvider(IOptions<EmailSmtpOptions> options)
    {
        _options = options.Value;
    }

    public string DefaultFrom => _options.DefaultFrom;
}


