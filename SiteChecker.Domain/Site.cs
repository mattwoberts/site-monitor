using System;
using System.Collections.Generic;

namespace SiteChecker.Domain
{
    public class Site
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastCheckedTime { get; set; }
        public SiteStatusEnum CurrentStatus { get; set; }
        public List<SiteDownIssue> SiteDownIssues { get; set; }

        public Site()
        {
            SiteDownIssues = new List<SiteDownIssue>();
        }
    }

    public class SiteDownIssue
    {
        public DateTime TimeDown { get; set; }
        public DateTime TimeUp { get; set; }
    }

    public enum SiteStatusEnum
    {
        Up,
        Down
    }
}