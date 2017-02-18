using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;


namespace AttachmentImport.Model
{   /// <summary>
    /// This is an example of setting your own mapping and analyzers for any data type
    /// 
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
    ///          
    /// It is possible to define your mapping using attributes on your POCO.
    /// When you use Attributes, you MUST use .AutoMap() in order for the attributes to be applied
    /// 
    ///</summary>

    public class ExampleMapping
    {
        public string data { get; set; }
        public string size { get; set; }
        public NestedObject attachment { get; set; }

    }

    [ElasticsearchType]
    public class NestedObject
    {
        //This is an example of setting the mapping for Author to Date in format "MMMddyy"
        [Date(Format = "MMddyy")]
        public string author { get; set; }
        //The below is an example of setting whitespace analyzer to the content field.
        //list of analyzers can be found here: https://www.elastic.co/guide/en/elasticsearch/reference/current/analysis-analyzers.html
        [Text(Analyzer = "whitespace")]
        public string content { get; set; }
        public long content_length { get; set; }
        public string content_type { get; set; }       
        public DateTime date { get; set; }
        public string keywords { get; set; }
        public string title { get; set; } 
        public string language { get; set; }
        
    }

}
        




