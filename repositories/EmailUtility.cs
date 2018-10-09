using System;
using System.Net.Mail;
using System.Threading.Tasks;
using contracts;
using Microsoft.Extensions.Options;

namespace repositories
{
    public class EmailUtility
    {
        EmailConfig _emailSettings;

        public EmailUtility(EmailConfig emailSettings)
        {
            this._emailSettings = emailSettings;
        }

        public async Task<string> SendEmailAsync(string email, string subject, string message)
        {

            var result = await Execute(email, subject, message);
            return result;
        }

        public async Task<string> Execute(string toEmail, string subject, string message)
        {
            string response = string.Empty;

            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Divkum Polri")
                };
                mail.To.Add(new MailAddress(toEmail));
                //mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                mail.Subject =  subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                mail.IsBodyHtml = true;


                using (SmtpClient smtp = new SmtpClient(_emailSettings.Domain, _emailSettings.Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                //do something here
                response = ex.Message;
            }

            return response;
        }

    }
}
