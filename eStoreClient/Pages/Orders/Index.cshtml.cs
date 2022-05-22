using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace eStoreClient.Pages.Orders
{
    public class IndexModel : PageModel
    {
        public IList<Order> Order { get;set; }

        public async Task OnGetAsync()
        {
            
        }
    }
}
