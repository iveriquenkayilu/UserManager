using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace UserManagerService.Shared.Interfaces.Helpers
{
    public interface IHttpOrchestrator
    {
        Task<TOutput> GetAsync<TOutput>(string url);
        Task<TOutput> GetWithHeadersAsync<TOutput>(string url, Dictionary<string, string> header);
        Task<TOutput> PostAsync<TOutput, TInput>(string url, TInput input, Dictionary<string, string> header = null);
        Task<string> SendRequestAsync<TInput>(HttpMethod method, string url, TInput input, Dictionary<string, string> header);
    }
}
