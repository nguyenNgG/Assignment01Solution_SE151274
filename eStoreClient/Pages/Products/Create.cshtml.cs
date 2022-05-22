using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using eStoreClient.Utilities;
using System.Net.Http;
using System.Net;
using eStoreClient.Constants;
using System.Text.Json;
using System.Text;

namespace eStoreClient.Pages.Products
{
    public class CreateModel : PageModel
    {
        HttpSessionStorage sessionStorage;
        public CreateModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public List<Category> Categories { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Categories}");
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Categories = JsonSerializer.Deserialize<List<Category>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                        ViewData["CategoryId"] = new SelectList(Categories, "CategoryId", "CategoryName");
                        Product = SessionHelper.GetFromSession<Product>(HttpContext.Session, "Product");
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

        [BindProperty]
        public Product Product { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Product = StringTrimmer.TrimProduct(Product);
                SessionHelper.SaveToSession(HttpContext.Session, Product, "Product");
                return RedirectToAction(nameof(OnGetAsync));
            }

            try
            {
                Product = StringTrimmer.TrimProduct(Product);
                HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                StringContent body = new StringContent(JsonSerializer.Serialize(Product), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(Endpoints.Products, body);
                HttpContent content = response.Content;
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    Product = JsonSerializer.Deserialize<Product>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                    return RedirectToPage(PageRoute.Products);
                }
            }
            catch
            {
            }
            Product = StringTrimmer.TrimProduct(Product);
            SessionHelper.SaveToSession(HttpContext.Session, Product, "Product");
            return RedirectToAction(nameof(OnGetAsync));
        }
    }
}
