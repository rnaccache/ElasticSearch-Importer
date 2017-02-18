using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using AttachmentImport.Helper;
using AttachmentImport.Model;

namespace AttachmentImport.POC
{
    class Pipeline
    {
        //Define Connection to elasticsearch
        static ElasticClient elasticConnector = ElasticConnector.Instance.Client;


        /// <summary>
        ///	Creates the pipeline using Ingest Plugin in ElasticSearch.By default, it exposes any property found in the document and then removes the base64 data from being stored (with remove).
        ///	You can specify specific properties to be displayed by implimenting:      Attachment<Word>(a => a.Field(f => f.data).PROPERTIES(....
        ///	Indexed_Char value of -1 will store the whole document (as appose to the limit of 100000 characters)
        ///	curl -XPUT 'http://10.1.1.229:9200/_ingest/pipeline/attachment' -d '
        ///	{
        ///		"processors" : [
        ///		  {
        ///			"attachment" : {
        ///				"field" : "data"
        ///				"properties" : ["",""]   (this field is Optional)
        ///				"indexed_char" : -1
        ///		  },
        ///			"remove": {
        ///			   "field" : "data"
        ///		  }
        ///		}
        ///	   ]
        ///	}'
        /// </summary>
        public static void Ingest()
        {
            var result = elasticConnector.PutPipeline(
                "attachment", 
                p => p.Processors(ps => ps.Attachment<Word>(
                    a => a.Field(f => f.data).IndexedCharacters(-1)
                ).Remove<Word>(r => r.Field(fi => fi.data))));
        }

        /// <summary>
        /// Pipeline used for setting index TYPE by file.extension attribute
        ///	PUT _ingest/pipeline/doc
        ///	{
        ///	  "description" : "Document Type Detector",
        ///	  "processors" : [
        ///		{
        ///		  "set": {
        ///			"field": "_type",
        ///			"value": "{{file.extension}}-files"
        ///		  }
        ///		}
        ///	  ]
        ///	}
        /// </summary>
        public static void DocumentType()
        {
            var result = elasticConnector.PutPipeline("doc",
                p => p.Processors(ps => ps.Set<FSCrawler>(s => s.Field("_type").Value("{{file.extension}}-files"))));
        }

    }
}
