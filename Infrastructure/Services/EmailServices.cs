using Application.Common.Dtos;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailService(
        IOptions<EmailSettings> options)
        : IEmailService
    {

        private readonly EmailSettings settings = options.Value;


        public async Task<Result<string>> SendEmailAsync(
            EmailRequest request)
        {
            try
            {
                var email = new MimeMessage();


                email.From.Add(
                    new MailboxAddress(
                        settings.DisplayName,
                        settings.From));


                email.To.Add(
                    MailboxAddress.Parse(request.To));


                email.Subject = request.Subject;


                var body = new BodyBuilder
                {
                    HtmlBody = request.Body
                };


                email.Body = body.ToMessageBody();



                using var smtp = new SmtpClient();


                await smtp.ConnectAsync(
                    settings.Host,
                    settings.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);



                await smtp.AuthenticateAsync(
                    settings.From,
                    settings.Password);



                await smtp.SendAsync(email);



                await smtp.DisconnectAsync(true);



                return Result<string>.Success(
                    "Email sent successfully",
                    "sent");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(
                    $"Email failed: {ex.Message}");
            }
        }
    }
}