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

namespace eStoreClient.Pages.Members
{
    public class CreateModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Member Member { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    StringContent body = new StringContent(JsonSerializer.Serialize(Member), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("http://localhost:5000/api/Members", body);
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
                    return Page();
                }
                catch
                {
                    return Page();
                }
            }
        }
    }
}
