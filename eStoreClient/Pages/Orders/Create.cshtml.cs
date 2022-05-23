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
using eStoreClient.Constants;
using System.Net;
using System.Text.Json;
using eStoreClient.Models;
using System.Text;

namespace eStoreClient.Pages.Orders
{
    public class CreateModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public CreateModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        [BindProperty]
        public Order Order { get; set; }
        public Cart Cart { get; set; }
        public List<Member> Members { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Members}");
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Members = JsonSerializer.Deserialize<List<Member>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                        ViewData["MemberId"] = new SelectList(Members, "MemberId", "Email");

                        response = await httpClient.GetAsync($"{Endpoints.Cart}");
                        content = response.Content;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Cart = JsonSerializer.Deserialize<Cart>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                            if (Cart.CartDetails.Count <= 0)
                            {
                                return RedirectToPage(PageRoute.OrderPrepare);
                            }
                            return Page();
                        }
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

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Members}");
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Members = JsonSerializer.Deserialize<List<Member>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                        ViewData["MemberId"] = new SelectList(Members, "MemberId", "Email");

                        response = await httpClient.GetAsync($"{Endpoints.Cart}");
                        content = response.Content;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Cart = JsonSerializer.Deserialize<Cart>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                            if (Cart.CartDetails.Count <= 0)
                            {
                                return RedirectToPage(PageRoute.OrderPrepare);
                            }

                            if (!ModelState.IsValid)
                            {
                                return Page();
                            }

                            Order.OrderDate = DateTime.Now;
                            foreach (var detail in Cart.CartDetails)
                            {
                                OrderDetail orderDetail = new()
                                {
                                    ProductId = detail.ProductItem.ProductId,
                                    Quantity = detail.Quantity,
                                    UnitPrice = (decimal)detail.ProductItem.UnitPrice,
                                };
                                Order.OrderDetails.Add(orderDetail);

                                response = await httpClient.GetAsync($"{Endpoints.Products}/{orderDetail.ProductId}");
                                content = response.Content;
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    Product product = JsonSerializer.Deserialize<Product>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                                    product.UnitsInStock -= orderDetail.Quantity;
                                    if (product.UnitsInStock < 0) product.UnitsInStock = 0;
                                    product.Category = null;
                                    StringContent body = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
                                    response = await httpClient.PutAsync($"{Endpoints.Products}/{product.ProductId}", body);
                                }
                                if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    return RedirectToPage(PageRoute.Orders);
                                }
                            }
                            StringContent orderBody = new StringContent(JsonSerializer.Serialize(Order), Encoding.UTF8, "application/json");
                            response = await httpClient.PostAsync($"{Endpoints.Orders}", orderBody);

                            Cart.CartDetails.Clear();
                            StringContent cartBody = new StringContent(JsonSerializer.Serialize(Cart), Encoding.UTF8, "application/json");
                            response = await httpClient.PostAsync(Endpoints.Cart, cartBody);

                            return RedirectToPage(PageRoute.Orders);
                        }
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return RedirectToPage(PageRoute.Orders);
                    }
                }
            }
            catch
            {
            }
            return RedirectToPage(PageRoute.Login);
        }
    }
}
