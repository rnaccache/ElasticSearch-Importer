using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttachmentImport.Helper;
using AttachmentImport.Model;
using Nest;

namespace AttachmentImport.POC
{
   static class Mapping
    {
        static ElasticClient elasticConnector = ElasticConnector.Instance.Client;

        /// <summary>
        /// Sets Mapping in elastic search to be automatically configured. This mapping is done based on the data field types of Word class.
        /// There is no curl command for automap as it does it automatically when you input a field in elasticsearch.
        /// later examples demonstrate how to override automapping incase you need to.
        /// </summary>
        public static void AutoMapping()
        {
            var mapper = elasticConnector.CreateIndex("vbc", c => c.Mappings(m => m.Map<Word>(ma => ma.AutoMap())));
        }

        /// <summary>
        /// The below example shows how to override automapping for a nested value.
        /// If instantiated with automap, the value of attachment.author would be of type "text" in elastic search. Here we override the mapping to Date, while leaving the rest of the fields
        /// to be derived automatically. Curl Equivalent Command:
        ///	curl -XPUT 'vbc' -d '
        ///	{
        ///	  "mappings":{
        ///		"word":{
        ///		  "properties":{
        ///			"attachment":{
        ///			  "type" : "nested",
        ///			  "properties":{
        ///				"author": {"type":"date"}
        ///			  }
        ///			}
        ///		  }
        ///		}
        ///	  }
        ///	}'
        /// This does not include the automap() as it does not need to be defined in ES
        /// </summary>
        public static void ManualMapping()
        {
            var mapper = elasticConnector.CreateIndex("vbc", c => c.Mappings(ms => ms.Map<Word>(m => m.Properties(ps => ps.Nested<MetaDataProperty>(p => p.Name(n=>n.attachment).Properties(pro=>pro.Date(dt=>dt.Name(nam=>nam.author))).AutoMap())))));
        }
    }
}
