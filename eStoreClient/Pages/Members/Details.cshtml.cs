using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using System.Net.Http;
using System.Text.Json;
using System.Net;

namespace eStoreClient.Pages.Members
{
    public class DetailsModel : PageModel
    {
        public Member Member { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    if (id == null)
                    {
                        return NotFound();
                    }
                    HttpResponseMessage response = await httpClient.GetAsync($"http://localhost:5000/api/Members/{id}");
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        Member = JsonSerializer.Deserialize<Member>(await content.ReadAsStringAsync(), jsonSerializerOptions);
                    }
                    else
                    {

                    }
                    return Page();
                }
                catch
                {
                    Member = null;
                    return Page();
                }
            }
        }
    }
}
