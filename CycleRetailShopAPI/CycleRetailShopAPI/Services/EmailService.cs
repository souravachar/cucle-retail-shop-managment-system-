using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using CycleRetailShopAPI.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailWithAttachmentAsync(string[] recipients, string subject, string body, byte[] pdfBytes, string fileName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Cycle Shop", _config["EmailSettings:Sender"]));

        foreach (var recipient in recipients)
        {
            message.To.Add(new MailboxAddress("", recipient));
        }

        message.Subject = subject;

        var builder = new BodyBuilder { TextBody = body };

        builder.Attachments.Add(fileName, pdfBytes, new ContentType("application", "pdf"));
        message.Body = builder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]), true);
            await client.AuthenticateAsync(_config["EmailSettings:Sender"], _config["EmailSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}


