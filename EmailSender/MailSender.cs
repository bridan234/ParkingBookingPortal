using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmailSender
{
    public static class MailSender
    {
        public static void SendMail(IEmail email, Stream Attachment )
        {
            MimeMessage msg = new MimeMessage();
            MailboxAddress from = new MailboxAddress("Admin", email.FromAddress);
            MailboxAddress To = new MailboxAddress("User", email.ToAddress);
            msg.Subject = email.Subject;
            msg.From.Add(from);
            msg.To.Add(To);

            BodyBuilder body = new BodyBuilder();
            body.TextBody = email.Message;

            body.Attachments.Add("Booking Confirmation", Attachment);

            msg.Body = body.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("in-v3.mailjet.com");
            client.Authenticate("bd4e27c9d500c3b9f32c66e1c2bba2ab", "1c227284efce00bf9b9234c3e23d167f");

            client.Send(msg);
            client.Disconnect(true);
            client.Dispose();

        }
    }
}
