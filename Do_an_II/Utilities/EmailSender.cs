using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Do_an_II.Utilities
{
    public class EmailSender : IEmailSender
    {
        public string SendGridKey { get; set; }
        public EmailSender(IConfiguration _config)
        {
            SendGridKey = _config.GetValue<string>("SendGrid:SecretKey");
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
           var client = new SendGridClient(SendGridKey);
           var from = new EmailAddress("duck59a3@gmail.com", "Eshop");
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            return client.SendEmailAsync(message);
        }
    }
   
}
