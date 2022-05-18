using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace GenesisBlogTakeTwo.Services
{
    public class BasicEmailService : IEmailSender
    {
        private readonly IConfiguration _appSettings;

        public BasicEmailService(IConfiguration appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //I have to get someone, use an smtp server, construct an email, build an email address from a string
            var emailSender = _appSettings["SmtpSettings:UserName"];

            //Start the process of creating an Email object from various pieces of data
            MimeMessage newEmail = new();
            newEmail.Sender = MailboxAddress.Parse(emailSender);
            newEmail.To.Add(MailboxAddress.Parse(email));
            newEmail.Subject = subject;

            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //At this point we are done composing the email and now we need to turn
            //our focus on configuring the Simple Mail Transfer Protocol (SMTP) server
            using SmtpClient smtpClient = new();

            var host = _appSettings["SmtpSettings:Server"];
            var port = Convert.ToInt32(_appSettings["SmtpSettings:Port"]);

            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(emailSender, _appSettings["SmtpSettings:AppPassword"]);

            await smtpClient.SendAsync(newEmail);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
