﻿using System;
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

namespace eStoreClient.Pages.Members
{
    public class EditModel : PageModel
    {
        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
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
                    HttpResponseMessage response = await httpClient.PutAsync($"http://localhost:5000/api/Members/{Member.MemberId}", body);
                    HttpContent content = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
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
