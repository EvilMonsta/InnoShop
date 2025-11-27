using InnoShop.Users.Application.Abstractions;
using Microsoft.Extensions.Logging;


namespace InnoShop.Users.Infrastructure.Email;


public class StubEmailSender : IEmailSender
{
    private readonly ILogger<StubEmailSender> _log;
    public StubEmailSender(ILogger<StubEmailSender> log) => _log = log;


    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _log.LogInformation("[EMAIL] To={To}; Subject={Subject}; Body={Body}", to, subject, body);
        return Task.CompletedTask;
    }
}