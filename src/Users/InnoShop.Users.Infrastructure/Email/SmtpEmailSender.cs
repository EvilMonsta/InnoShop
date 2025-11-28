using System.Net;
using System.Net.Mail;
using InnoShop.Users.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InnoShop.Users.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _cfg;
    private readonly ILogger<SmtpEmailSender> _log;
    public SmtpEmailSender(IConfiguration cfg, ILogger<SmtpEmailSender> log)
    { _cfg = cfg; _log = log; }

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var host = _cfg["Smtp:Host"] ?? throw new InvalidOperationException("Smtp:Host is not set");
        var port = int.TryParse(_cfg["Smtp:Port"], out var p) ? p : 587;
        var user = _cfg["Smtp:User"];
        var pass = _cfg["Smtp:Password"];
        var from = _cfg["Smtp:From"] ?? user ?? throw new InvalidOperationException("Smtp:From is not set");
        var ssl = bool.TryParse(_cfg["Smtp:EnableSsl"], out var e) ? e : true;

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = ssl,
            Credentials = string.IsNullOrEmpty(user) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(user, pass)
        };

        using var msg = new MailMessage(from, to, subject, body) { IsBodyHtml = true };

        _log.LogInformation("SMTP sending To={To}, Subj={Subject}", to, subject);
        await client.SendMailAsync(msg, ct);
    }
}
