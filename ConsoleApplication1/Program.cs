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
            string esIndex = ConfigurationManager.AppSettings.Get("");
           
            //get base64 values of all the files in FilePath
            var documents = DocumentControl.ConvertBase64(Directory.GetFiles(documentPath, "*", SearchOption.AllDirectories).ToList());

            //POC.Pipeline.Ingest();
            //POC.Mapping.AutoMapping();
            POC.Mapping.ManualMapping();

            //POC.Example1.IngestwithField(documents);
            //POC.Example1.ViewResults(elasticConnector);
            //POC.Search.Highlight();




        }
    }
}
