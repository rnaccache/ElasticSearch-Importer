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
        static Dictionary<string,FSCrawler> dictionary { get; set; }

        /// <summary>
        /// General Search. Curl Equivalent Command:
        /// curl -XGET 'http://10.1.1.229:9200/import/doc/_search' 
        /// Use the Size parameter to set the max value of search result shown
        /// </summary>
        static public List<FSCrawler> General()
        {
                var searchResult = elasticConnector.Search<FSCrawler>(s => s.Index("import").Type("doc").Size(100));
                return searchResult.Documents.ToList();
        }

        /// <summary>
        /// Searches for the term eve in the field meta.author. Curl Equivalent Command:
        /// curl -XPOST 'http://10.1.1.229:9200/import/doc/_search' -d '
        ///	{
        ///	  "query":{
        ///		"match": {
        ///		  "meta.author": "eve"
        ///		}
        ///	  }
        ///	}'
        /// </summary>
        static public List<FSCrawler> NestedField()
        {
            var searchResult = elasticConnector.Search<FSCrawler>(
                 s => s.Index("import").Type("doc").Query(q => q.Match(m => m.Field(f => f.meta.author).Query("eve"))));
            return searchResult.Documents.ToList();
        }

        /// <summary>
        /// Searches the field meta.content for a term word and highlights the queried term in the specified Highlight field with <em></em>. Curl Equivalent Command:
        /// curl -XPOST 'http://10.1.1.229:9200/import/doc/_search' -d '
        ///	{
        ///	  "query":{
        ///		"match": {
        ///		  "meta.content": "word"
        ///		}
        ///	  },
        ///	  "highlight": {
        ///		"fields": {
        ///		  "meta.content": {}
        ///		}
        ///	  }
        ///	}'
        ///	
        /// The result is displayed in a new highlight field.
        /// </summary>
        static public Dictionary<string,List<string>>Highlight()
        {
            Dictionary<string,List<string>> dictionary = new Dictionary<string,List<string>>();

            var searchResult = elasticConnector.Search<FSCrawler>(s => s.Index("import").Type("doc").Query(q => q.Match(m => m.Field(f => f.content).Query("word"))).Highlight(h => h.Fields(fs => fs.Field(ff => ff.content))));

            //Creates a dictionary with the file name, and a list of hits
            foreach (var item in searchResult.Hits)
            {
                var filename = item.Source.file.filename;
                var Highlight = item.Highlights.First().Value.Highlights.ToList();
                dictionary.Add(filename, Highlight);
            }

            return dictionary;

        }

        /// <summary>
        /// Searches the field meta.raw.Application-name for a term "Microsoft Office Word", and highlights a term "Word" in the content field with <em></em>. Curl Equivalent Command:
        ///curl -XPOST 'http://10.1.1.229:9200/import/doc/_search' -d '
        ///{
        ///	"query":{
        ///		"match": {
        ///			"meta.raw.Application-Name": "Microsoft Office Word"
        ///				}
        ///					},
        ///	"highlight":{
        ///		"fields": {
        ///			"attachment.content": {
        ///				"highlight_query":{
        ///					"match": {
        ///						"conent": "Word"
        ///					}
        ///				}
        ///			}
        ///		}
        ///	}
        ///}
        /// </summary>
        static public Dictionary<string,List<string>> Highlight2()
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            var searchResult = elasticConnector.Search<FSCrawler>(s => s.Index("import").Type("doc").Query(
                q => q.Match(m => m.Field(f => f.meta.raw.AplicationNameKeyword).Query("Microsoft Office Word"))).Highlight(h => h.Fields(
                    fs => fs.Field(ff => ff.content).HighlightQuery(
                        hq => hq.Match(ma => ma.Field(fie => fie.content).Query("word"))
            ))));
            
            foreach (var item in searchResult.Hits)
            {
                var filename = item.Source.file.filename;
                var Highlight = item.Highlights.First().Value.Highlights.ToList();
                dictionary.Add(filename, Highlight);
            }
            return dictionary;
        }
        /// <summary>
        /// Searches both the meta.raw.applicationname for the keyword "Microsoft Office Word" and the field meta.author for the value "robert" and returns only if both cases are matched.
        /// A Highlight is then done to the field "content" and the value "word", and the PreTags and PostTags are changed from the default to a defined value of <tag1></tag1>.
        /// Curl Equivalent Command:
        ///  curl -XPOST 'http://10.1.1.229:9200/import/doc/_search' -d '
        ///	 {
        ///		"query":{
        ///			"bool": {
        ///		  "must": [
        ///			 { "term": {"meta.raw.Application-Name.keyword": {"value": "Microsoft Office Word"}}},
        ///			 { "term": {"meta.author": {"value": "robert"}}}
        ///		   ]
        ///				}
        ///			},
        ///		  "highlight":{
        ///		  "pre_tags": ["<tag1>"],
        ///		  "post_tags": ["</tag1>"], 
        ///			"fields": {
        ///		  "content": {
        ///			"highlight_query":{
        ///					"match":{
        ///					"content": "word"
        ///				}
        ///			  }
        ///			}
        ///		  }
        ///	   }
        ///	 }'
        /// </summary>
        /// <returns></returns>
        static public Dictionary<string, List<string>> Highlight3()
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            //Searhc Query parameter with highlighting
            var searchResult = elasticConnector.Search<FSCrawler>(s => s.Index("import").Type("doc").Query(
                q => q.Bool(b=>b.Must(
                    m=>m.Term(t=>t.Field(f=>f.meta.raw.AplicationNameKeyword).Value("Microsoft Office Word")) & 
                    m.Term(tt=>tt.Field(f=>f.meta.author).Value("robert"))))).Highlight(
                        h => h.PreTags("<tag1>").PostTags("</tag1>").Fields(
                            fs => fs.Field(ff => ff.content).HighlightQuery( hq => hq.Match(ma => ma.Field(fie => fie.content).Query("word"))
                ))));

            foreach (var item in searchResult.Hits)
            {
                var filename = item.Source.file.filename;
                var Highlight = item.Highlights.First().Value.Highlights.ToList();
                dictionary.Add(filename, Highlight);
            }
            return dictionary;
        }

        /// <summary>
        /// Searches the field content for term microsof with fuzzy distance 1. Curl Equivalent Command:
        ///	curl -XPOST 'http://10.1.1.229:9200/import/doc/_search' -d '
        ///	{
        ///	  "query":{
        ///		"fuzzy": {
        ///		  "content": {
        ///			"fuzziness": 1,
        ///			"value": "microsof"
        ///		  }
        ///		}
        ///	  }
        ///	}' 
        /// </summary>
        static public Dictionary<string,FSCrawler> Fuzzy()
        {
            Dictionary<string, FSCrawler> dictionary = new Dictionary<string, FSCrawler>();

            var searchResult = elasticConnector.Search<FSCrawler>(s => s.Index("import").Type("doc").Query(q=>q.Fuzzy(fu=>fu.Field(f=>f.content).Fuzziness(Fuzziness.EditDistance(1)).Value("microsof"))));
            
            foreach (var item in searchResult.Documents)
            {
                dictionary.Add(item.file.filename, item);
            }

            return dictionary;
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
        static public Dictionary<int,List<FSCrawler>> Scroll()
        {
            int pageNumber = 1;
            Dictionary<int, List<FSCrawler>> dictionary = new Dictionary<int, List<FSCrawler>>();

            //Change the ElasticConnecter to have a default index in vbc for the bulk operation
            var connection = ElasticConnector.Instance._settings.DefaultIndex("import");
            var searchResult = elasticConnector.Search<FSCrawler>(s => s.Index("import").Type("doc").Scroll("1m").Size(1));
            
            
            //Search if the pages have documents in them.
            while (searchResult.Documents.Any())
            {
                var request = new BulkRequest();
                request.Operations = new List<IBulkOperation>();

                //Store each document from each page in a list
                foreach (var item in searchResult.Documents)
                {
                    request.Operations.Add(new BulkIndexOperation<FSCrawler>(item));
                }
                //Store result
                var pageResult = elasticConnector.Bulk(request);

            
                //Go to the next page
                searchResult = elasticConnector.Scroll<FSCrawler>("1m", searchResult.ScrollId);

                dictionary.Add(pageNumber, searchResult.Documents.ToList());
                pageNumber++;
            }

            return dictionary;
        }

        static public void ComplexSearch()
        {

        } 
    }
}
