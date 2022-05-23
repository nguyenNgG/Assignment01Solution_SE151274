using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using eStoreClient.Models;
using eStoreClient.Utilities;
using eStoreClient.Constants;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Text;

namespace eStoreClient.Pages.Orders
{
    public class EditModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public EditModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public string Message { get; set; }

        [FromQuery(Name = "order-id")]
        public string OrderId { get; set; }

        [BindProperty]
        public EditOrderForm EditOrderForm { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OrderId))
                {
                    return RedirectToPage(PageRoute.Orders);
                }

                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Orders}/{OrderId}");
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Order order = JsonSerializer.Deserialize<Order>(await content.ReadAsStringAsync(), options);

                        EditOrderForm = new()
                        {
                            Freight = order.Freight,
                            ShippedDate = order.ShippedDate,
                        };
                        return Page();
                    }
                }
                return RedirectToPage(PageRoute.Orders);
            }
            catch
            {
            }
            return RedirectToPage(PageRoute.Login);
        }

        public async Task<ActionResult> OnPostAsync(string orderId)
        {
            try
            {
                if (EditOrderForm == null)
                {
                    return RedirectToPage(PageRoute.Orders);
                }

                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Orders}/{OrderId}");
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Order order = JsonSerializer.Deserialize<Order>(await content.ReadAsStringAsync(), options);

                        if (EditOrderForm.ShippedDate != null)
                        {
                            if (((DateTime)EditOrderForm.ShippedDate).CompareTo(order.OrderDate) <= 0)
                            {
                                Message = $"Shipped Date must be after Order Date: {order.OrderDate}";
                                return Page();
                            }
                            else
                            {
                                Message = "";
                            }
                        }
                        order.Freight = EditOrderForm.Freight;
                        order.ShippedDate = EditOrderForm.ShippedDate;

                        StringContent body = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
                        response = await httpClient.PutAsync($"{Endpoints.Orders}/{OrderId}", body);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToPage(PageRoute.Orders);
                        }
                        return Page();
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
