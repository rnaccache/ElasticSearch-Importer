using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttachmentImport;
using AttachmentImport.Helper;
using AttachmentImport.Model;
using Nest;

namespace AttachmentImport.POC
{
    /// <summary>
    /// Different samples of searching.
    /// </summary>
    static public class Search
    {
        //Define Connection to elasticsearch
        static ElasticClient elasticConnector = ElasticConnector.Instance.Client;

        /// <summary>
        /// General Search. Curl Equivalent Command:
        /// curl -XGET 'http://10.1.1.229:9200/vbc/example1/_search' 
        /// Use the Size parameter to set the max value of search result shown
        /// </summary>
        static public void General()
        {
            var searchResult = elasticConnector.Search<Word>(s => s.Index("vbc").Type("example1").Size(100));
        }
        
        /// <summary>
        /// Searches for the term Robert in the field attachment.content. Curl Equivalent Command:
        ///	curl -XPOST 'vbc/example1/_search' -d '
        ///	{
        ///	  "query":{
        ///		"match": {
        ///		  "attachment.content": "Robert"
        ///		}
        ///	  }
        ///	}'
        /// </summary>
        static public void NestedField()
        {
            var searchResult = elasticConnector.Search<Word>(
                 s => s.Index("vbc").Type("example1").Query(q => q.Match(m => m.Field(f => f.attachment.content).Query("Robert"))));
        }

        /// <summary>
        /// Searches the field attachment.content for a term (robert) and highlights the queried term in the specified Highlight field with <em></em>. Curl Equivalent Command:
        ///	curl -XPOST 'vbc/example1/_search' -d '
        ///	{
        ///	  "query":{
        ///		"match": {
        ///		  "attachment.content": "robert"
        ///		}
        ///	  },
        ///	  "highlight": {
        ///		"fields": {
        ///		  "attachment.content": {}
        ///		}
        ///	  }
        ///	}'
        /// </summary>
        static public void Highlight()
        {
            var searchResult = elasticConnector.Search<Word>(s => s.Index("vbc").Type("example2").Query(q => q.Match(m => m.Field(f => f.attachment.content).Query("robert"))).Highlight(h => h.Fields(fs => fs.Field(ff => ff.attachment.content))));

            foreach (var item in searchResult.Documents)
            {
                var result = item.attachment.content;
            }
        }

        /// <summary>
        /// Searches the field attachment.content for a term (robert), and highlights a term (alex) in the attachment.content field with <em></em>. Curl Equivalent Command:
        /// TODO: add curl command
        /// 
        /// 
        /// 
        /// </summary>
        static public void Highlight2()
        {

           
            var searchResult = elasticConnector.Search<Word>(s => s.Index("vbc").Type("example1").Query(q => q.Match(m => m.Field(f => f.attachment.content).Query("robert"))).Highlight(h => h.Fields(fs => fs.Field(ff => ff.attachment.content).HighlightQuery(hq => hq.Match(ma => ma.Field(fie => fie.attachment.content).Query("alex"))))));

            foreach (var item in searchResult.Documents)
            {
                var result = item.attachment.content;
            }
        }

        /// <summary>
        /// Searches the field attachment.content for term rober with fuzzy distance 1. Curl Equivalent Command:
        ///	curl -XPOST 'vbc/example1/_search' -d '
        ///	{
        ///	  "query":{
        ///		"fuzzy": {
        ///		  "attachment.content": {
        ///			"fuzziness": 1,
        ///			"value": "rober"
        ///		  }
        ///		}
        ///	  }
        ///	}' 
        /// </summary>
        static public void Fuzzy()
        {
            //Query
            var searchResult = elasticConnector.Search<Word>(s => s.Index("vbc").Type("example1").Query(q=>q.Fuzzy(fu=>fu.Field(f=>f.attachment.content).Fuzziness(Fuzziness.EditDistance(1)).Value("rober"))));

            //Result for the attachment.content field
            foreach (var item in searchResult.Documents)
            {
                var test = item.attachment.content;
            }
        }

        static public void Scroll()
        {
            var searchResult = elasticConnector.Search<Word>(s => s.Index("vbc").Type("example1").Scroll("1m"));
            string scrollid = searchResult.ScrollId;


        }

        
    }
}
