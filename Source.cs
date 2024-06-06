using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forumcord
{
    public class Source
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? IconPath { get; set; }
        public string? Folder { get; set; } // Optional: Folder organization

        public SourceType? ForumType { get; set; }
    }

    public enum SourceType
    {
        None,
        XenForo,
        XenForo2,
        MyBB,
        phpBB
    }
}
