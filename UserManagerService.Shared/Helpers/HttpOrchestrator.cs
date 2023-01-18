using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserManagerService.Shared.Interfaces.Helpers;

namespace UserManagerService.Shared.Helpers
{
    public class HttpOrchestrator : IHttpOrchestrator
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpOrchestrator> _logger;

        public HttpOrchestrator(ILogger<HttpOrchestrator> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            //_httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<TOutput> GetAsync<TOutput>(string url) => await SendHttpRequestForContentAsync<TOutput>(HttpMethod.Get, url);

        public async Task<TOutput> GetWithHeadersAsync<TOutput>(string url, Dictionary<string, string> header)
            => await SendHttpRequestForContentAsync<TOutput>(HttpMethod.Get, url, header);

        public async Task<TOutput> PostAsync<TOutput, TInput>(string url, TInput input, Dictionary<string, string> header = null) => await SendHttpRequestForContentAsync<TOutput, TInput>(HttpMethod.Post, url, input, header);
        public async Task<string> SendRequestAsync<TInput>(HttpMethod method, string url, TInput input, Dictionary<string, string> header)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(method, url, input, header);
            return await response.Content.ReadAsStringAsync();
        }
        private async Task<TOutput> SendHttpRequestForContentAsync<TOutput>(HttpMethod method, string url, Dictionary<string, string> header = null)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(method, url, header);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOutput>(content);
        }
        private async Task<TOutput> SendHttpRequestForContentAsync<TOutput, TInput>(HttpMethod method, string url, TInput input, Dictionary<string, string> header)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(method, url, input, header);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOutput>(content);
        }

        private Task<HttpResponseMessage> SendHttpRequestAsync(HttpMethod method, string url, Dictionary<string, string> header) =>
           SendHttpRequestAsync<string>(method, url, model: null, header: header);

        private async Task<HttpResponseMessage> SendHttpRequestAsync<TInput>(HttpMethod method, string url, TInput model, Dictionary<string, string> header)
        {
            _logger.LogInformation($"Creating new request to url: {url}.");
            var request = new HttpRequestMessage(method, url);

            if (header is not null)
            {
                _logger.LogInformation($"Adding request header(s) : {header}.");

                foreach (var element in header)
                    request.Headers.Add(element.Key, element.Value);
            }

            if (model != null)
            {
                var content = JsonConvert.SerializeObject(model);

                // _logger.LogInformation($"Setting request body to : {content}.");
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response;
            try
            {
                _logger.LogInformation($"Sending request: {request} to url: {url}.");
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an issue when sending the http request.");
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Http response failed with status code : {response.StatusCode}.");
            }

            return response;
        }
    }
}
