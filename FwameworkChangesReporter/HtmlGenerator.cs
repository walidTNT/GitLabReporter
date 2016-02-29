using FwameworkChangesReporter.PushEvent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FwameworkChangesReporter
{
    public class HtmlGenerator
    {
        public static string GenerateHtmlTable(List<PushEventCommits> invalidCommitsCommitById, PushEventProperties commit)
        {
            var mailBody = new StringBuilder();
            mailBody.Append(string.Format("<div><p style='font-family:Arial,Verdana,Helvetica'>Bonjour,<br/><br/>Attention, vous avez commit dans le projet <b>{0}</b> sur le dossier FWAmework sans référence à un ticket NPO.<br/>Cette modification n’est pas autorisée, vous devez voir avec l’équipe Template le processus de modification du code FWAmework.<br/></p></div>", commit.Repository.Name));
            mailBody.Append(@"<style>table { border-collapse: collapse; cellspacing :2;font-family:Arial,Verdana,Helvetica'} 
                        th, td, tr {border: 2px solid white;align='center';cellspacing :10;marge-left:5;padding:5px; rowSpan:1;colSpan:1;font-family : Arial,Verdana,Helvetica}</style>");
            mailBody.Append("<table><thead style='background-color:#eeeeee'><tr><th>Commit</th><th>Message</th></tr><thead><tbody>");
            var rowCount = 1;
            var row1 = "#FFE0D8";
            var row2 = "#FFF0EC"; 
            foreach (var ticket in invalidCommitsCommitById)
            {
                var link = ticket.Url;
                mailBody.Append("<tr>");
                string format = rowCount % 2 == 0 ? row1 : row2;

                mailBody.Append(string.Format("<td style='background-color: {0}'>", format));
                mailBody.Append(string.Format("<a href=\"{0}\">", link));
                mailBody.Append(ticket.Id);
                mailBody.Append("</a>");
                mailBody.Append("</td>");

                mailBody.Append(string.Format("<td style='text-align:left;background-color: {0}'>", format));
                mailBody.Append(ticket.Message);
                mailBody.Append("</td>");
                mailBody.Append("</tr>");
                rowCount++;
            }
            mailBody.Append("</tbody></table><br/><br/><br/>");

            var linkBugProcess = ConfigurationManager.AppSettings["linkBugProcess"];
            mailBody.Append("<div><p style='font-family:Arial,Verdana,Helvetica'>Procédure des corrections sur Template :</p></div>");
            mailBody.Append(string.Format("<a style='font-family:Arial,Verdana,Helvetica' href=\"{0}\">", linkBugProcess));
            mailBody.Append("FWAmework fixing process");
            mailBody.Append("</a>");

            return mailBody.ToString();
        }
    }
}
