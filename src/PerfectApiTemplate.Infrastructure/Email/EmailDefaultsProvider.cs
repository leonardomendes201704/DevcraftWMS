using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Email;

namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class EmailDefaultsProvider : IEmailDefaults
{
    private readonly EmailSmtpOptions _options;

    public EmailDefaultsProvider(IOptions<EmailSmtpOptions> options)
    {
        _options = options.Value;
    }

    public string DefaultFrom => _options.DefaultFrom;
}

