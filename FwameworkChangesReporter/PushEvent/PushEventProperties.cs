using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FwameworkChangesReporter.PushEvent
{
    public class PushEventProperties
    {
        [JsonProperty("object_kind")]
        public string ObjectKind { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string Ref { get; set; }
        [JsonProperty("checkout_sha")]
        public string CheckoutSha { get; set; }
        public string Message { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("user_name")]
        public string UserName { get; set; }
        [JsonProperty("user_email")]
        public string UserEmail { get; set; }
        [JsonProperty("project_id")]
        public int ProjectId { get; set; }
        [JsonProperty("repository")]
        public PushEventRepository Repository { get; set; }
        [JsonProperty("commits")]
        public PushEventCommits[] Commits { get; set; }
        [JsonProperty("total_commits_count")]
        public int TotalCommitsCount { get; set; }
    }
}
