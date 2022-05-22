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

namespace eStoreClient.Pages.Members
{
    public class LoginModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public LoginModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        [BindProperty]
        public LoginForm LoginForm { get; set; }

        public async Task<ActionResult> OnGet()
        {
            try
            {
                HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                HttpResponseMessage response = await httpClient.GetAsync($"http://localhost:5000/api/Members/auth");
                HttpContent content = response.Content;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToPage(PageRoute.Members);
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    LoginForm = new LoginForm();
                    return Page();
                }
            }
            catch
            {
                return RedirectToPage(PageRoute.Members);
            }
            return RedirectToPage(PageRoute.Members);
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                StringContent body = new StringContent(JsonSerializer.Serialize(LoginForm), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync($"http://localhost:5000/api/Members/login", body);
                HttpContent content = response.Content;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToPage(PageRoute.Members);
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    LoginForm = new LoginForm();
                    return Page();
                }
            }
            catch
            {
                return RedirectToPage(PageRoute.Members);
            }
            return RedirectToPage(PageRoute.Members);
        }
    }
}
