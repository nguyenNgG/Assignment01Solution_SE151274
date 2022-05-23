using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using System.Net.Http;
using eStoreClient.Utilities;
using eStoreClient.Constants;
using System.Net;
using System.Text.Json;

namespace eStoreClient.Pages.Orders
{
    public class IndexModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public IndexModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public List<Order> Orders { get; set; }

        public bool IsStaff { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                int _memberId = -993901;
                IsStaff = false;
                string query = "";
                if (authResponse.StatusCode != HttpStatusCode.OK)
                {
                    authResponse = await SessionHelper.Current(HttpContext.Session, sessionStorage);
                    content = authResponse.Content;
                    _memberId = int.Parse(await content.ReadAsStringAsync());
                    query = $"?memberId={_memberId}";
                }
                else
                {
                    IsStaff = true;
                }

                HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Orders}{query}");
                content = response.Content;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    Orders = JsonSerializer.Deserialize<List<Order>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                    return Page();
                }
            }
            catch
            {
            }
            return RedirectToPage(PageRoute.Login);
        }
    }
}
