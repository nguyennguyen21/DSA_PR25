// /Pages/Profile/Progress.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Models;
using DSA_PR25.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace DSA_PR25.Pages.Profile;
[Authorize]
public class ProgressModel : PageModel
{
    
    private readonly ApplicationDBcontext _context;

    public ProgressModel(ApplicationDBcontext context)
    {
        _context = context;
    }

    // ✅ Đổi tên để tránh xung đột với PageModel.User
    public User? CurrentUser { get; set; }
    public Badge? CurrentBadge { get; set; }
    public List<Examresult> ExamResults { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // ✅ Lấy UserId từ Claims (sau khi đăng nhập)
         var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
        {
            return Unauthorized(); // hoặc Redirect đến login
        }

        // ✅ Truy vấn người dùng
        CurrentUser = await _context.Users.FindAsync(userId);
        if (CurrentUser == null)
        {
            return NotFound();
        }

        // ✅ Truy vấn badge
        CurrentBadge = await _context.Badges.FindAsync(CurrentUser.BadgeId);

        // ✅ Truy vấn lịch sử thi
        ExamResults = await _context.Examresults
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.ExamDate)
            .Take(10)
            .ToListAsync();

        return Page();
    }
}