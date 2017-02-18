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
        /// curl -XPOST 'http://10.1.1.229:9200/vbc/example1/_search' -d '
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
        /// curl -XPOST 'http://10.1.1.229:9200/vbc/example1/_search' -d '
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
        ///curl -XPOST 'http://10.1.1.229:9200/vbc/example1/_search' -d '
        ///{
        ///	"query":{
        ///		"match": {
        ///			"attachment.content": "robert"
        ///				}
        ///					},
        ///	"highlight":{
        ///		"fields": {
        ///			"attachment.content": {
        ///				"highlight_query":{
        ///					"match": {
        ///						"attachment.content": "alex"
        ///					}
        ///				}
        ///			}
        ///		}
        ///	}
        ///}
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
        ///	curl -XPOST 'http://10.1.1.229:9200/vbc/example1/_search' -d '
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

        /// <summary>
        /// Using Scroll to retrieve large numbers of results from a single search request.You also need to specify how long you want the scroll to be alive for (in this case 1 min).
        /// Curl Equivalent Command:
        /// curl -XPOST 'http://10.1.1.229:9200/vbc/example1/_search?scroll=1m' -d '
        /// {
        ///     "size" : 100,
        ///     "query" : {
        ///         "match_all":{}
        ///         }
        /// }        
        ///  
        /// This search result displays the first 100 documents found, and creats a scroll_id which is used to point to the next page (also need to specify scroll life duration).
        /// To query the next page the below Curl Command needs to be issued:
        /// 
        /// curl -XPOST /_search/scroll' -d '
        /// {
        ///     "scroll" : "1m",
        ///     "scroll_id" : "value obtained fromt the previous search"
        ///} 
        /// </summary>
        static public void Scroll()
        {

            //Change the ElasticConnecter to have a default index in vbc for the bulk operation
            var connection = ElasticConnector.Instance._settings.DefaultIndex("vbc");
            var searchResult = elasticConnector.Search<Word>(s => s.Index("vbc").Type("example1").Scroll("1m").Size(100));
            
            
            //Search if the pages have documents in them.
            while (searchResult.Documents.Any())
            {
                var request = new BulkRequest();
                request.Operations = new List<IBulkOperation>();

                //Store each document from each page in a list
                foreach (var item in searchResult.Documents)
                {
                    request.Operations.Add(new BulkIndexOperation<Word>(item));
                }
                //Store result
                var pageResult = elasticConnector.Bulk(request);
            
                //Go to the next page
                searchResult = elasticConnector.Scroll<Word>("1m", searchResult.ScrollId);

            }




        }
    }
}
