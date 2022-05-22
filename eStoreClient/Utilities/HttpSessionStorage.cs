using System.Collections.Generic;
using System.Net.Http;

namespace eStoreClient.Utilities
{
    public class HttpSessionStorage
    {
        public List<HttpClient> HttpClients { get; set; } = new List<HttpClient>();
    }
}
