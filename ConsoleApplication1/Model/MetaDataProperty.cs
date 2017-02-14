using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace AttachmentImport.Model
{
    public class MetaDataProperty
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
