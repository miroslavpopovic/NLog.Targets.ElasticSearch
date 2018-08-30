using System.Net;

namespace NLog.Targets.ElasticSearch
{
    public class ElasticSearchResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}