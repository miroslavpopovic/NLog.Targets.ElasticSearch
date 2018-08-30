using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace NLog.Targets.ElasticSearch
{
    public class ElasticSearchClient
    {
        private readonly HttpClient _client;

        public ElasticSearchClient(
            Uri elasticSearchUri, string userName, string password)
        {
            _client = InitializeHttpClient(elasticSearchUri, userName, password);
        }

        private static HttpClient InitializeHttpClient(Uri elasticSearchUri, string userName, string password)
        {
            var client = new HttpClient {BaseAddress = elasticSearchUri};

            var credentials = string.Format("{0}:{1}", userName, password);
            // TODO: Make sure ASCII is OK. What if we have unicode chars in password?
            var encodedCredentials =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", encodedCredentials);

            return client;
        }

        public ElasticSearchResult Bulk(List<object> payload)
        {
            var jsonPayload = SerializePayload(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var result = _client.PostAsync("_bulk", content).Result;

            // TODO: Check if we need other properties
            return new ElasticSearchResult
            {
                ErrorMessage = result.ReasonPhrase,
                Success = result.IsSuccessStatusCode,
                HttpStatusCode = result.StatusCode
            };
        }

        private string SerializePayload(ICollection payload)
        {
            if (payload.Count == 0)
                return string.Empty;

            var result = new StringBuilder();
            foreach (var payloadItem in payload)
            {
                result.Append(JsonConvert.SerializeObject(payloadItem));
                result.Append('\n');
            }

            return result.ToString();
        }
    }
}