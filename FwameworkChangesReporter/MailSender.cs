using FwameworkChangesReporter.PushEvent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FwameworkChangesReporter
{
    public class MailSender
    {
        public static void SendMail(List<PushEventCommits> invalidCommitsCommitById, PushEventProperties commit)
        {
            try
            {
                MailMessage mail = new MailMessage();
                var mailSubject = ConfigurationManager.AppSettings["mailSubject"];
                mail.Subject = string.Format(mailSubject, commit.UserName);
                mail.IsBodyHtml = true;
                mail.Body = HtmlGenerator.GenerateHtmlTable(invalidCommitsCommitById, commit);
                mail.To.Add(commit.UserEmail);
                //AddCopyMail(mail);
                SmtpClient smtp = new SmtpClient();
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            
        }

        public static void AddCopyMail(MailMessage mail)
        {
            var emailsToSendCopy = ConfigurationManager.AppSettings["emailsToSendCopy"];
            var emailSendCopy = emailsToSendCopy.Split(';');
            foreach (var email in emailSendCopy)
            {
                mail.CC.Add(email);
            }
        }
    }
}
