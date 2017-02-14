using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using AttachmentImport.Model;
using AttachmentImport.Helper;

namespace AttachmentImport.POC
{
   /// <summary>
   /// Add Metadata Field + any extra field
   /// </summary>
    static class Example1
    {

        static ElasticClient elasticConnector = ElasticConnector.Instance.Client;


        /// <summary>
        /// Used to input a list of document's base64 vaule and extract the metadata using IngestNode.  Curl Equivalent Command:
        ///	curl -XPUT 'vbc/example1/_search' -d '
        ///	{
        ///		"data" : "....base64 value...."
        ///	}'
        /// </summary>
        /// <param name="documents"></param>
        static public void Ingest(List<string> documents)
        {
            foreach (string item in documents)
            {
                var result = elasticConnector.Index(new Word {data = item }, i => i.Index("vbc").Type("example1").Pipeline("attachment"));
            }
           
        }

        /// <summary>
        /// Used to input a single document of base64 vaule and extract the metadata using IngestNode, added an extra field defined by the application.  Curl Equivalent Command:
        ///	curl -XPUT 'vbc/example1/_search' -d '
        ///	{
        ///	    "size" : "1000KB",
        ///		"data" : "....base64 value...."
        ///	}'
        /// </summary>
        /// <param name="documents"></param>
        static public void IngestwithField (List<string> documents)
        {
            var result = elasticConnector.Index(new Word { data = documents[1], size = "1000KB"  }, i => i.Index("vbc").Type("example1").Pipeline("attachment"));
        }   

    }
}
