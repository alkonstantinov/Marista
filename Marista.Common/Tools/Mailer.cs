using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.Tools
{
    public class Mailer
    {
        public Task<bool> SendMail(string fnm, string email, string subject)
        {
            MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["FromEmail"], email);
            mm.Subject = subject;
            //AlternateView avHtml = new AlternateView(@"D:\Projects\birex\pismo1.html", MediaTypeNames.Text.Html);
            //AlternateView avPlain = new AlternateView(@"D:\Projects\birex\pismo1plain.txt", MediaTypeNames.Text.Plain);
            //mm.AlternateViews.Add(avHtml);
            //mm.AlternateViews.Add(avPlain);
            mm.Body = File.ReadAllText(fnm);
            mm.IsBodyHtml = true;
            mm.SubjectEncoding = System.Text.Encoding.UTF8;
            mm.BodyEncoding = System.Text.Encoding.UTF8;

            SmtpClient cl = new SmtpClient();

            cl.Send(mm);
            return new Task<bool>(() => false);
        }

        public Task<bool> SendMailSpecific(string content, string email, string subject)
        {
            MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["FromEmail"], email);
            mm.Subject = subject;
            //AlternateView avHtml = new AlternateView(@"D:\Projects\birex\pismo1.html", MediaTypeNames.Text.Html);
            //AlternateView avPlain = new AlternateView(@"D:\Projects\birex\pismo1plain.txt", MediaTypeNames.Text.Plain);
            //mm.AlternateViews.Add(avHtml);
            //mm.AlternateViews.Add(avPlain);
            mm.Body = content;
            mm.IsBodyHtml = true;
            mm.SubjectEncoding = System.Text.Encoding.UTF8;
            mm.BodyEncoding = System.Text.Encoding.UTF8;

            SmtpClient cl = new SmtpClient();

            cl.Send(mm);
            return new Task<bool>(() => false);
        }
    }
}
