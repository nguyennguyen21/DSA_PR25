using DSA_PR25.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DSA_PR25.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDBcontext _context;

        public int TotalUsers { get; set; }
        public int TotalExams { get; set; }

        public DashboardModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            TotalUsers = await _context.Users.CountAsync();
          
        }
    }
}
