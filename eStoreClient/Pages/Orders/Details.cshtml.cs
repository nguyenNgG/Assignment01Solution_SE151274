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
using System.Net;
using eStoreClient.Constants;
using System.Text.Json;

namespace eStoreClient.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public DetailsModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        [FromQuery(Name = "order-id")]
        public string OrderId { get; set; }
        public bool IsStaff { get; set; } = false;

        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OrderId))
                {
                    return RedirectToPage(PageRoute.Orders);
                }

                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    IsStaff = true;
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Orders}/{OrderId}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Order = JsonSerializer.Deserialize<Order>(await content.ReadAsStringAsync(), jsonSerializerOptions);

                        httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                        response = await httpClient.GetAsync($"{Endpoints.OrderDetails}?orderId={OrderId}");
                        content = response.Content;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            OrderDetails = JsonSerializer.Deserialize<List<OrderDetail>>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                            return Page();
                        }
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
