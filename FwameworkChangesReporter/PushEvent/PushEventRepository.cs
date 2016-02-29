using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FwameworkChangesReporter.PushEvent
{
    public class PushEventRepository
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Homepage { get; set; }
        [JsonProperty("git_http_url")]
        public string GitHttpUrl { get; set; }
        [JsonProperty("git_ssh_url")]
        public string GitSshUrl { get; set; }
        [JsonProperty("visibility_level")]
        public string VisibilityLevel { get; set; }

    }
}
