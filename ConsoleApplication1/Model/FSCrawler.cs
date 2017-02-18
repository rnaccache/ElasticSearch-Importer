using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttachmentImport.Model
{
    /// <summary>
    /// Model for FS Crawler
    /// Layout for model
    ///     |-content
    ///     |-meta
    ///     |     |-author
    ///     |     |-title
    ///     |     |-date
    ///     |     |-raw
    ///     |        |-company
    ///     |        |-keywords
    ///     |        |-subject
    ///     |        |-Author
    ///     |        |-Application-Name
    ///     |        |     |-Application-Name.keyword
    ///     |        |-Creation-Date  
    ///     |-file
    ///          |-filename
    ///</summary>
    /// </summary>
     public class FSCrawler
    {
        public string content { get; set; }
        public metaobj meta { get; set; }
        public fileobj file { get; set; }


        public class metaobj
        {
            public string author { get; set; }
            public string title { get; set; }
            public DateTime date { get; set; }
            public rawobj raw { get; set; }

        }
        public class rawobj
        {
            public string Company { get; set; }
            public string Keywords { get; set; }
            public string subject { get; set; }
            public string Author { get; set; }
            [Text(Name="Application-Name")]
            public string ApplicationName { get; set; }
            [Text(Name="Application-Name.keyword")]
            public keywordobj AplicationNameKeyword { get; set; }
            [Text(Name="Creation-Date")]
            public DateTime datecreated { get; set; }
        }
        public class fileobj
        {
            public string filename { get; set; }
            public string url { get; set; }
        }
        
        public class keywordobj
        { 
            public string keyword { get; set; }
        }
    }
}
