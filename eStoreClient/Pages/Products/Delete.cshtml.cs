using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using eStoreClient.Utilities;
using System.Net.Http;
using eStoreClient.Constants;
using System.Net;
using System.Text.Json;

namespace eStoreClient.Pages.Products
{
    public class DeleteModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public DeleteModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToPage(PageRoute.Products);
                }

                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Products}/{id}");
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Product = JsonSerializer.Deserialize<Product>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                        return Page();
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return RedirectToPage(PageRoute.Products);
                    }
                }
            }
            catch
            {
            }
            return RedirectToPage(PageRoute.Login);
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToPage(PageRoute.Products);
                }

                HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                HttpResponseMessage response = await httpClient.DeleteAsync($"{Endpoints.Products}/{id}");

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return RedirectToPage(PageRoute.Products);
                }
            }
            catch
            {
            }
            return RedirectToAction(nameof(OnGetAsync), new { id = id });
        }
    }
}
