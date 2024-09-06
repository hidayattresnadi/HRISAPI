using HRISAPI.Application.Mail;

namespace HRISAPI.Application.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(MailData maildata);
    }
}
