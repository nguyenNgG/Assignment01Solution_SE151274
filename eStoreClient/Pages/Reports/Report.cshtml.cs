using BusinessObjects;
using eStoreClient.Constants;
using eStoreClient.Models;
using eStoreClient.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace eStoreClient.Pages.Reports
{
    public class ReportModel : PageModel
    {
        HttpSessionStorage sessionStorage;
        public ReportModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        [BindProperty] public DateTime? StartDate { get; set; }
        [BindProperty] public DateTime? EndDate { get; set; }

        public decimal TotalOfPeriod { get; set; }
        public List<OrderWithTotal> TotalOrders { get; set; } = new List<OrderWithTotal>();

        public async Task<ActionResult> OnGetAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Orders}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        List<Order> orders = JsonSerializer.Deserialize<List<Order>>(await content.ReadAsStringAsync(), options);
                        orders = orders.Where(o =>
            (DateTime.Compare(o.OrderDate, StartDate ?? DateTime.Now) >= 0)
            && (DateTime.Compare(o.OrderDate, EndDate ?? DateTime.Now.AddDays(7)) <= 0)
            ).ToList();
                        foreach (Order order in orders)
                        {
                            decimal totalOfOrder = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity);
                            OrderWithTotal totalOrder = new()
                            {
                                MemberId = order.MemberId,
                                OrderDate = order.OrderDate,
                                OrderId = order.OrderId,
                                RequiredDate = order.RequiredDate,
                                ShippedDate = order.ShippedDate,
                                Freight = order.Freight,
                                Member = order.Member,
                                TotalPrice = totalOfOrder,
                            };
                            TotalOrders.Add(totalOrder);
                        }
                        TotalOfPeriod = TotalOrders.Sum(o => o.TotalPrice);
                        return Page();
                    }
                }
            }
            catch
            {
            }
            return Page();
        }
        public async Task<ActionResult> OnPostAsync()
        {
            try
            {
                HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Orders}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        List<Order> orders = JsonSerializer.Deserialize<List<Order>>(await content.ReadAsStringAsync(), options);
                        orders = orders.Where(o =>
            (DateTime.Compare(o.OrderDate, StartDate ?? DateTime.Now) >= 0)
            && (DateTime.Compare(o.OrderDate, EndDate ?? DateTime.Now.AddDays(7)) <= 0)
            ).ToList();
                        foreach (Order order in orders)
                        {
                            decimal totalOfOrder = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity);
                            OrderWithTotal totalOrder = new()
                            {
                                MemberId = order.MemberId,
                                OrderDate = order.OrderDate,
                                OrderId = order.OrderId,
                                RequiredDate = order.RequiredDate,
                                ShippedDate = order.ShippedDate,
                                Freight = order.Freight,
                                Member = order.Member,
                                TotalPrice = totalOfOrder,
                            };
                            TotalOrders.Add(totalOrder);
                        }
                        TotalOfPeriod = TotalOrders.Sum(o => o.TotalPrice);
                        return Page();
                    }
                }
            }
            catch
            {
            }
            return Page();
        }
    }
}
