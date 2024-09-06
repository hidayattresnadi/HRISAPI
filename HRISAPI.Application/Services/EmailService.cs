using HRISAPI.Application.IServices;
using HRISAPI.Application.Mail;
using HRISAPI.Domain.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HRISAPI.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task<bool> SendEmailAsync(MailData mailData) 
        {
            var emailMessage = CreateEmailMessage(mailData);
            var result = Send(emailMessage);
            return result;
            
        }
        private MimeMessage CreateEmailMessage(MailData mailData)
        {
            MimeMessage emailMessage = new MimeMessage();
            MailboxAddress emailFrom = new MailboxAddress(_mailSettings.Name, _mailSettings.EmailId);

            emailMessage.From.Add(emailFrom);
            if(mailData.EmailToIds != null && mailData.EmailToIds.Any())
            {
                foreach (var to in mailData.EmailToIds)
                {
                    MailboxAddress emailTo = new MailboxAddress(to, to);
                    emailMessage.To.Add(emailTo);
                    
                }
            }
            if (mailData.EmailCCIds != null && mailData.EmailCCIds.Any())

            {
                foreach (var cc in mailData.EmailCCIds)
                {

                    MailboxAddress emailCc = new MailboxAddress(cc, cc);

                    emailMessage.Cc.Add(emailCc);
                }

            }

            //MailboxAddress email_To = new MailboxAddress(mailData.EmailToName, mailData.EmailToId);

            //emailMessage.To.Add(email_To);

            emailMessage.Subject = mailData.EmailSubject;

            BodyBuilder emailBodyBuilder = new BodyBuilder();
            emailBodyBuilder.HtmlBody = mailData.EmailBody;
            //var htmlTemplate = System.IO.File.ReadAllText(@"./Templates/EmailTemplate/Whatever.html");
            //htmlTemplate = htmlTemplate.Replace("{{Name}}", mailData.EmailToName);
            //emailBodyBuilder.HtmlBody = htmlTemplate;

            //emailBodyBuilder.TextBody = mailData.EmailBody;
            if (mailData.Attachments != null && mailData.Attachments.Any())

            {

                byte[] fileBytes;

                foreach (var attachment in mailData.Attachments)

                {

                    using (var ms = new MemoryStream())

                    {

                        attachment.CopyTo(ms);

                        fileBytes = ms.ToArray();

                    }

                    emailBodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));

                }

            }

            emailMessage.Body = emailBodyBuilder.ToMessageBody();

            return emailMessage;

        }
        private bool Send(MimeMessage mailMessage)

        {

            using (var client = new SmtpClient())
            {
                try

                {

                    client.Connect(_mailSettings.Host, _mailSettings.Port, true);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.Authenticate(_mailSettings.UserName, _mailSettings.Password);

                    client.Send(mailMessage);

                    return true;

                }

                catch (Exception ex)

                {
                    Console.WriteLine(ex);

                    //log an error message or throw an exception or both.

                    return false;

                }

                finally

                {

                    client.Disconnect(true);

                    client.Dispose();

                }

            }

        }

    }
}
