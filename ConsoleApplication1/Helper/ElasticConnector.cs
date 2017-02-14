using System;
using System.Configuration;
using Elasticsearch.Net;
using Nest;

namespace AttachmentImport.Helper
{
    /// <summary>
    ///Used to connect to ElasticSearch
    /// </summary>
    public class ElasticConnector
    {
        //Define parameters for connection
        private static string NodeURI = ConfigurationManager.AppSettings.Get("ElasticSearchURI");
        private static string username = ConfigurationManager.AppSettings.Get ( "elkuser" );
        private static string password = ConfigurationManager.AppSettings.Get ( "elkpass" );


        private static ElasticConnector _instance;

        private static Uri _node = new Uri( NodeURI );
        public  ConnectionSettings _settings { set; get; }
        public  ElasticClient Client {private set; get; }
        private ElasticConnector(){}

        private static volatile object mutex = new object();

        /// <summary>
        /// singleton connection to elasticsearch
        /// </summary>
        public static ElasticConnector Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (mutex)
                    {
                        if (_instance == null)
                        {
                            _instance = new ElasticConnector ( );
                            _instance._settings = new ConnectionSettings ( _node ).BasicAuthentication ( username , password );
                            _instance.Client = new ElasticClient ( _instance._settings );
                        }   
                    }
                }
                return _instance;
            }
            
        }

      
    }
}
