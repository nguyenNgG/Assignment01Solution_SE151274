using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Net;
using eStoreClient.Constants;
using eStoreClient.Utilities;

namespace eStoreClient.Pages.Members
{
    public class CreateModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public CreateModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public async Task<ActionResult> OnGet()
        {
            try
            {
                HttpResponseMessage response = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Page();
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return RedirectToPage(PageRoute.Login);
                }
            }
            catch
            {
            }
            return RedirectToPage(PageRoute.Login);
        }

        [BindProperty]
        public Member Member { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Member = StringTrimmer.TrimMember(Member);
                return Page();
            }

            try
            {
                Member = StringTrimmer.TrimMember(Member);
                HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                StringContent body = new StringContent(JsonSerializer.Serialize(Member), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(Endpoints.Members, body);
                HttpContent content = response.Content;
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    Member = JsonSerializer.Deserialize<Member>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                    return RedirectToPage(PageRoute.Members);
                }
            }
            catch
            {
            }
            Member = StringTrimmer.TrimMember(Member);
            return Page();
        }
    }
}
