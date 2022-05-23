using BusinessObjects;
using eStoreClient.Constants;
using eStoreClient.Models;
using eStoreClient.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eStoreClient.Pages.Carts
{
    public class EditModel : PageModel
    {
        HttpSessionStorage sessionStorage;
        public EditModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public string QuantityMessage { get; set; }

        [FromQuery(Name = "item-index")] public int ItemIndex { get; set; } = -1;
        [BindProperty]
        public CartDetail CartDetail { get; set; }

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
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Cart}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Cart cart = JsonSerializer.Deserialize<Cart>(await content.ReadAsStringAsync(), options);
                        int itemCount = cart.CartDetails.Count;
                        if (ItemIndex <= itemCount && ItemIndex >= 0)
                        {
                            CartDetail = cart.CartDetails[ItemIndex];
                            return Page();
                        }
                        return RedirectToPage(PageRoute.Cart);
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
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Cart}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Cart cart = JsonSerializer.Deserialize<Cart>(await content.ReadAsStringAsync(), options);
                        int itemCount = cart.CartDetails.Count;
                        if (ItemIndex > itemCount && ItemIndex < 0)
                        {
                            return RedirectToPage(PageRoute.Cart);
                        }
                        CartDetail currentDetail = cart.CartDetails[ItemIndex];
                        response = await httpClient.GetAsync($"{Endpoints.Products}/{currentDetail.ProductItem.ProductId}");
                        content = response.Content;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Product product = JsonSerializer.Deserialize<Product>(await content.ReadAsStringAsync(), options);
                            int? unitsInStock = product.UnitsInStock;
                            if (product.UnitsInStock < CartDetail.Quantity || CartDetail.Quantity <= 0)
                            {
                                QuantityMessage = $"Invalid quantity selected. Only up to {unitsInStock} units can be added.";
                                return Page();
                            }
                            cart.CartDetails[ItemIndex].Quantity = CartDetail.Quantity;
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
                            return RedirectToPage(PageRoute.Cart);
                        }
                        return RedirectToPage(PageRoute.Cart);
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
