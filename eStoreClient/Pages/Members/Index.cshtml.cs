using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Net;
using eStoreClient.Utilities;
using eStoreClient.Constants;

namespace eStoreClient.Pages.Members
{
    public class IndexModel : PageModel
    {
        HttpSessionStorage sessionStorage;
        public IndexModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public IList<Member> Members { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                HttpClient httpClient;
                int? httpSessionIndex = SessionHelper.GetFromSession<int?>(HttpContext.Session, SessionValue.HttpSessionIndex);
                if (httpSessionIndex == null)
                {
                    httpClient = new HttpClient();
                    sessionStorage.HttpClients.Add(httpClient);
                    SessionHelper.SaveToSession<int?>(HttpContext.Session, sessionStorage.HttpClients.IndexOf(httpClient), SessionValue.HttpSessionIndex);
                }
                else
                {
                    httpClient = sessionStorage.HttpClients[(int)httpSessionIndex];
                }

                HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5000/api/Members");
                HttpContent content = response.Content;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    Members = JsonSerializer.Deserialize<List<Member>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                }
                else
                {

                }
            }
            catch
            {
                Members = new List<Member>();
            }
        }
    }
}
