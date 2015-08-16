using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;

namespace ImageMaker.Utils.Services
{
    public interface ISmtpService
    {
        bool SendEmail(byte[] image, string emailTo = "george.39reg@gmail.com");
    }

    public class SmtpService : ISmtpService
    {
        public bool SendEmail(byte[] image, string emailTo = "george.39reg@gmail.com")
        {
            try
            {
                SmtpClient smtp = new SmtpClient();

                MailMessage message = new System.Net.Mail.MailMessage();
                message.To.Add(new MailAddress(emailTo));
                message.Subject = "Image";

                System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
                contentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                contentType.Name = "image.jpeg";

                using (MemoryStream stream = new MemoryStream(image))
                using (MemoryStream emptyStream = new MemoryStream())
                {
                    Image.FromStream(stream).Save(emptyStream, ImageFormat.Jpeg);
                    emptyStream.Seek(0, SeekOrigin.Begin);
                    Attachment attachment = new Attachment(emptyStream, contentType);
                    message.Attachments.Add(attachment);
                    smtp.Send(message);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(e.Message);
                return false;
            }

        }
    }
}
