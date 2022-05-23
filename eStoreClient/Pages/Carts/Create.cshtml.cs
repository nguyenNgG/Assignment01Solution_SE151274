using BusinessObjects;
using eStoreClient.Constants;
using eStoreClient.Models;
using eStoreClient.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eStoreClient.Pages.Carts
{
    public class CreateModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public CreateModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public string ProductMessage { get; set; }
        public string QuantityMessage { get; set; }

        [BindProperty]
        public ProductItem ProductItem { get; set; }

        [BindProperty]
        public CartDetail CartDetail { get; set; }

        public List<Product> Products { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Current(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                int _memberId = int.Parse(await content.ReadAsStringAsync());
                authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Products}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Products = JsonSerializer.Deserialize<List<Product>>(await content.ReadAsStringAsync(), options);
                        ViewData["ProductId"] = new SelectList(Products, "ProductId", "ProductName");
                        return Page();
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return RedirectToPage(PageRoute.Orders);
                    }
                }
                return RedirectToPage(PageRoute.Orders);
            }
            catch
            {
            }
            return RedirectToPage(PageRoute.Login);
        }

        public async Task<ActionResult> OnPostAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Current(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                int _memberId = int.Parse(await content.ReadAsStringAsync());
                authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Products}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Products = JsonSerializer.Deserialize<List<Product>>(await content.ReadAsStringAsync(), options);
                        ViewData["ProductId"] = new SelectList(Products, "ProductId", "ProductName");
                        response = await httpClient.GetAsync($"{Endpoints.Cart}");
                        content = response.Content;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Cart cart = JsonSerializer.Deserialize<Cart>(await content.ReadAsStringAsync(), options);

                            bool isExisted = cart.CartDetails.Where(cartDetail => cartDetail.ProductItem.ProductId == ProductItem.ProductId).Any();
                            if (isExisted)
                            {
                                ProductMessage = "Product already exists in order.";
                                return Page();
                            }
                            response = await httpClient.GetAsync($"{Endpoints.Products}/{ProductItem.ProductId}");
                            content = response.Content;
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                Product product = JsonSerializer.Deserialize<Product>(await content.ReadAsStringAsync(), options);
                                int? unitsInStock = product.UnitsInStock;
                                if (product.UnitsInStock < CartDetail.Quantity || CartDetail.Quantity <= 0)
                                {
                                    QuantityMessage = $"Only up to {unitsInStock} units can be added.";
                                    return Page();
                                }
                                ProductItem.ProductName = product.ProductName;
                                ProductItem.UnitPrice = product.UnitPrice;
                                CartDetail.ProductItem = ProductItem;
                                cart.CartDetails.Add(CartDetail);
                                StringContent body = new StringContent(JsonSerializer.Serialize(cart), Encoding.UTF8, "application/json");
                                response = await httpClient.PostAsync(Endpoints.Cart, body);
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    return RedirectToPage(PageRoute.Cart);
                                }
                                return Page();
                            }
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                return Page();
                            }
                        }
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return RedirectToPage(PageRoute.Orders);
                    }
                }
                return RedirectToPage(PageRoute.Orders);
            }
            catch
            {
            }
            return RedirectToPage(PageRoute.Login);
        }
    }
}
