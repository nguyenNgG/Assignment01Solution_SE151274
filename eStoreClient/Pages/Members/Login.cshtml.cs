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
                HttpResponseMessage authResponse = await SessionHelper.Current(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                int _memberId = int.Parse(await content.ReadAsStringAsync());

                HttpResponseMessage response = await SessionHelper.Authenticate(HttpContext.Session, sessionStorage);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return RedirectToPage(PageRoute.Members);
                    }
                    return RedirectToPage(PageRoute.Profile, new { id = _memberId });
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    LoginForm = new LoginForm();
                }
            }
            catch
            {
            }
            return Page();
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
                HttpResponseMessage response = await httpClient.PostAsync(Endpoints.Login, body);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpResponseMessage authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                    if (authResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return RedirectToPage(PageRoute.Members);
                    }
                    authResponse = await SessionHelper.Current(HttpContext.Session, sessionStorage);
                    HttpContent content = authResponse.Content;
                    return RedirectToPage(PageRoute.Profile, new { id = int.Parse(await content.ReadAsStringAsync()) });
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    LoginForm = new LoginForm();
                    return Page();
                }
            }
            catch
            {
            }
            return Page();
        }
    }
}
