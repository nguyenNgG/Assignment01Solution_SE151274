using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using eStoreClient.Constants;
using Microsoft.AspNetCore.Http;
using System.Net;
using eStoreClient.Utilities;

namespace eStoreClient.Pages.Members
{
    public class EditModel : PageModel
    {
        HttpSessionStorage sessionStorage;

        public EditModel(HttpSessionStorage _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        [BindProperty]
        public Member Member { get; set; }

        [TempData]
        public int MemberId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToPage(PageRoute.Members);
                }

                MemberId = (int)id;
                TempData["MemberId"] = MemberId;

                HttpResponseMessage authResponse = await SessionHelper.Current(HttpContext.Session, sessionStorage);
                HttpContent content = authResponse.Content;
                int _memberId = int.Parse(await content.ReadAsStringAsync());
                if (_memberId == id)
                {
                    authResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    authResponse = await SessionHelper.Authorize(HttpContext.Session, sessionStorage);
                }
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    HttpClient httpClient = SessionHelper.GetHttpClient(HttpContext.Session, sessionStorage);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Endpoints.Members}/{id}");
                    content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Member = JsonSerializer.Deserialize<Member>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                        return Page();
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return RedirectToPage(PageRoute.Members);
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
            Member.MemberId = (int)TempData.Peek("MemberId");
            TempData.Keep("MemberId");
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
                HttpResponseMessage response = await httpClient.PutAsync($"{Endpoints.Members}/{MemberId}", body);
                HttpContent content = response.Content;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    Member = JsonSerializer.Deserialize<Member>(await content.ReadAsStringAsync(), jsonSerializerOptions);

                    HttpResponseMessage authResponse = await SessionHelper.Current(HttpContext.Session, sessionStorage);
                    content = authResponse.Content;
                    int _memberId = int.Parse(await content.ReadAsStringAsync());
                    if (_memberId == Member.MemberId)
                    {
                        return RedirectToPage(PageRoute.Profile, new { id = _memberId});
                    }
                    return RedirectToPage(PageRoute.Members);
                }
            }
            catch
            {
            }
            return Page();
        }

    }
}
