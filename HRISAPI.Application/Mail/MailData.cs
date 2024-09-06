using Microsoft.AspNetCore.Http;

namespace HRISAPI.Application.Mail
{
    public class MailData
    {
        public List<string> EmailToIds { get; set; } = new List<string>();
        public List<string> EmailCCIds { get; set; } = new List<string>();
        public string EmailToName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public IFormFileCollection Attachments { get; set; }
    }
}
