using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace ProductApp.Server.Services
{
    public interface IMailService
    {

        Task SendEmailAsync(string toEmail, string subject, string content);

    }

    public class SendGridMailService : IMailService
    {

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "info@trustsoft.site"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = content
            };

            using (var client = new SmtpClient())
            {
                //TODO: настроить подтверждение электронной почты
                //TODO: сделать ssl
                await client.ConnectAsync("smtp.mail.ru", 25, false);// что за почта и настройки ?
                await client.AuthenticateAsync("info@trustsoft.site", "Mail12");// что за почта и настройки ?
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

        }

    }
}
