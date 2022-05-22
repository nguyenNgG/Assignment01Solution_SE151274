using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using eStoreClient.Utilities;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using eStoreClient.Constants;
using System.Text;

namespace eStoreClient.Pages.Products
{
    public class EditModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public EditModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        [BindProperty]
        public Product Product { get; set; }

        [TempData]
        public int ProductId { get; set; }

        public List<Category> Categories { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToPage(PageRoute.Products);
                }

                ProductId = (int)id;
                TempData["ProductId"] = ProductId;

                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    // get product
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

                        // get categories
                        response = await httpClient.GetAsync($"{Endpoints.Categories}");
                        content = response.Content;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Categories = JsonSerializer.Deserialize<List<Category>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                            ViewData["CategoryId"] = new SelectList(Categories, "CategoryId", "CategoryName");
                            return Page();
                        }

                        return RedirectToPage(PageRoute.Products);
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Product.ProductId = (int)TempData.Peek("ProductId");
            TempData.Keep("ProductId");

            HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
            HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Categories}");
            HttpContent content = response.Content;

            if (!ModelState.IsValid)
            {
                Product = StringTrimmer.TrimProduct(Product);
                // get categories
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    Categories = JsonSerializer.Deserialize<List<Category>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                    ViewData["CategoryId"] = new SelectList(Categories, "CategoryId", "CategoryName");
                    return Page();
                }
                return RedirectToPage(PageRoute.Products);
            }

            try
            {
                Product = StringTrimmer.TrimProduct(Product);
                Product.Category = null;
                StringContent body = new StringContent(JsonSerializer.Serialize(Product), Encoding.UTF8, "application/json");
                response = await httpClient.PutAsync($"{Endpoints.Products}/{ProductId}", body);
                content = response.Content;
                if (response.StatusCode == HttpStatusCode.OK)
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
            // get categories
            if (response.StatusCode == HttpStatusCode.OK)
            {
                JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                Categories = JsonSerializer.Deserialize<List<Category>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                ViewData["CategoryId"] = new SelectList(Categories, "CategoryId", "CategoryName");
                return Page();
            }
            return RedirectToPage(PageRoute.Products);
        }
    }
}
