using Eticaret.Core.Entities;
using System.Net;
using System.Net.Mail;

namespace Eticaret.WebUI.Utils
{
    public class MailHelper
    {
        public static async Task SendMailAsync(Contact contact)
        { 
            SmtpClient smtpClient = new SmtpClient("mail.canpare.com", 587);
            smtpClient.Credentials = new NetworkCredential("info@canparetoys.com", "canpareToys*222");
            smtpClient.EnableSsl = true;
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@canparetoys.com");
            message.To.Add("mcanparmak@gmail.com");
            message.Subject = "Canpare Oyuncak";
            message.Body = $"İsim: {contact.Name} - Soyisim: {contact.Surname} - Email: {contact.Email} - Telefon: {contact.Phone} - Mesaj: {contact.Message}";
            message.IsBodyHtml = true;
            await smtpClient.SendMailAsync(message);
            smtpClient.Dispose();
        }
    }
}
