using Core.Entities.Email;
using Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _cfg;
    public SmtpEmailSender(IOptions<SmtpSettings> cfg) => _cfg = cfg.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        if (message.To == null || message.To.Count == 0)
            throw new InvalidOperationException("El correo no tiene destinatarios (To).");

        var email = new MimeMessage();

        // FROM
        email.From.Add(new MailboxAddress(_cfg.FromName, _cfg.FromEmail));

        // TO (soporta varios)
        foreach (var to in message.To.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            email.To.Add(MailboxAddress.Parse(to));
        }

        // CC (opcional)
        if (message.Cc is not null)
        {
            foreach (var cc in message.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                email.Cc.Add(MailboxAddress.Parse(cc));
            }
        }

        email.Subject = message.Subject;

        var body = new TextPart(TextFormat.Html)
        {
            Text = message.HtmlBody
        };
        body.ContentType.Charset = "utf-8";

        email.Body = body;

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        await smtp.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.SslOnConnect, ct);
        await smtp.AuthenticateAsync(_cfg.User, _cfg.Pass, ct);
        await smtp.SendAsync(email, ct);
        await smtp.DisconnectAsync(true, ct);
    }
}
