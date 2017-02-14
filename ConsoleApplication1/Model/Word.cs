using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;


namespace AttachmentImport.Model
{   /// <summary>
    /// Layout for model
    ///     |-data 
    ///     |-attachment
    ///          |-author
    ///          |-content
    ///          |-content_length
    ///          |-content_type
    ///          |-date
    ///          |-keywords
    ///          |-title
    ///          |-language
    ///</summary>

    public class Word
    {
        public string data { get; set; }
        public string size { get; set; }
        public attachmentObj attachment { get; set; }

    }

    [ElasticsearchType]
    public class attachmentObj
    {
        
        public string author { get; set; }
        public string content { get; set; }
        public long content_length { get; set; }
        public string content_type { get; set; }       
        public DateTime date { get; set; }
        public string keywords { get; set; }
        public string title { get; set; } 
        public string language { get; set; }
        
    }

}
        




