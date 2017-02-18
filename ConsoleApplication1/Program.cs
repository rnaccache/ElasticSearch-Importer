using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttachmentImport.Model;
using AttachmentImport.Helper;

namespace AttachmentImport
{
    class Program
    {
        /// <summary>
        /// Imports all files from specified FilePath to Elastic Searc 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            
            string documentPath = ConfigurationManager.AppSettings.Get("FilePath");

            //get base64 values of all the files in FilePath
            var documents = Directory.GetFiles(documentPath, "*", SearchOption.AllDirectories).ToList().ConvertBase64();

            POC.Pipeline.DocumentType();
            POC.Mapping.AutoMapping();

            List<FSCrawler> searchResults1 =  POC.Search.General();
            List<FSCrawler> searchResult2 = POC.Search.NestedField();
            Dictionary<string,List<string>> searchResult3 = POC.Search.Highlight();
            Dictionary<string,List<string>> searchResult4 = POC.Search.Highlight2();
            Dictionary<string, List<string>> searchResult5 = POC.Search.Highlight3();
            Dictionary<string, FSCrawler> searchResult6 = POC.Search.Fuzzy();
            Dictionary<int,List<FSCrawler>> searchresult7 = POC.Search.Scroll();
        }
    }
}
