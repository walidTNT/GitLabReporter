using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FwameworkChangesReporter.PushEvent
{
    public class PushEventCommits
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string TimeStamp { get; set; }
        public string Url { get; set; }
        public Author Author { get; set; }

    }
}
