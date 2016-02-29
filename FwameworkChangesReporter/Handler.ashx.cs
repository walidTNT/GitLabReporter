using FwameworkChangesReporter.PushEvent;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NGitLab;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FwameworkChangesReporter
{
    /// <summary>
    /// This handler retreives data from push events and validates which commit should be reported to Template team.
    /// </summary>
    public class Handler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //Gets push data from context
                Stream req = context.Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string json = new StreamReader(req).ReadToEnd();

                //Parsing data 
                var push = JsonConvert.DeserializeObject<PushEventProperties>(json, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                var commits = push.Commits;

                //Creating GitLabClient instance to retreive data from Git; Todo: root private token
                var hostNameGitFWA = ConfigurationManager.AppSettings["hostNameGitFWA"];
                var privateToken = ConfigurationManager.AppSettings["privateToken"];
                var client = GitLabClient.Connect(hostNameGitFWA, privateToken);
                var repository = client.GetRepository(push.ProjectId);

                //List of commits which files changed in Fwamework
                List<PushEventCommits> commitsInFwamework = new List<PushEventCommits>();
                var folderToVerify = ConfigurationManager.AppSettings["folderToVerify"];
                foreach (var commit in commits)
                {
                    var commitDiff = repository.GetCommitDiff(commit.Id);
                    var filePaths = commitDiff.SelectMany(x => new string[] { x.NewPath, x.OldPath }).Distinct().ToArray();
                    if (filePaths.Any(x => x.ToLower().Contains(folderToVerify)))
                    {
                        commitsInFwamework.Add(commit);
                    }
                }

                //First validation: List of commits with ticket link in message & List of commits without.
                Dictionary<PushEventCommits, string[]> ticketLinksInCommitById = new Dictionary<PushEventCommits, string[]>();
                List<PushEventCommits> invalidCommitsCommitById = new List<PushEventCommits>();
                var regexMatchTicket = ConfigurationManager.AppSettings["regexMatchTicket"];
                foreach (var commit in commitsInFwamework)
                {
                    var message = commit.Message;
					MatchCollection matches = Regex.Matches(message, regexMatchTicket); // If link ticket different from regexMatchTicket matches == 0 even if valid ticket in Template ex: http://support.fwa.eu/bug_update_page.php?bug_id=42267
                    if (matches.Count == 0)
                        invalidCommitsCommitById.Add(commit);
                    else ticketLinksInCommitById.Add(commit, matches.Cast<Match>().Select(x => x.Value).ToArray());
                }


                //Second validation: Checking if found tickets in commit message exists in Template
                string cs = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
                using (var conn = new MySqlConnection(cs))
                {
                    try
                    {
                        conn.Open();
                        foreach (var id in ticketLinksInCommitById)
                        {
                            var projectId = ConfigurationManager.AppSettings["projectId"];
                            string stm = @"select count(*) from mantis_bug_table where project_id in ("+projectId+") AND id in (" + string.Join(", ", id.Value) + ")";
                            var cmd = new MySqlCommand(stm, conn);
                            var result = Convert.ToInt32(cmd.ExecuteScalar());
                            if (result == 0)
                            {
                                invalidCommitsCommitById.Add(id.Key);
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Log(ex);
                        throw;
                    }
                }

                if (invalidCommitsCommitById.Any())
                    MailSender.SendMail(invalidCommitsCommitById, push);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}