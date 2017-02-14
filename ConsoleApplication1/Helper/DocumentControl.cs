using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.RegularExpressions;

/// <summary>
/// Used to convert any file to its base64 value. 
/// </summary>
namespace AttachmentImport.Helper
{
    static class DocumentControl
    {     
        /// <summary>
        /// Convert a single file to base64
        /// </summary>
        /// <param name="file">String with the path of the file</param>
        /// <returns></returns>
        static private string ConvertBase64(this string file)
        {
            byte[] outputBytes = File.ReadAllBytes(file);
            string fileBase64 = Convert.ToBase64String(outputBytes);
            return fileBase64;
        }
        
        /// <summary>
        /// Converts a list of files to base64
        /// </summary>
        /// <param name="file">List of string with the path of the files</param>
        /// <returns></returns>
        static public List<string> ConvertBase64(List<string> file)
        {
            List<string> res = new List<string>();

            file.ForEach(item => res.Add(item.ConvertBase64()));

            return res;
        }

    }
}
