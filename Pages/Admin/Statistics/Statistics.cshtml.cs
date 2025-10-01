// /Pages/Profile/Statistics.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Models;
using DSA_PR25.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DSA_PR25.Pages.Profile;

[Authorize(Roles = "admin")] // Chỉ admin được xem thống kê toàn hệ thống
public class StatisticsModel : PageModel
{
    private readonly ApplicationDBcontext _context;

    public StatisticsModel(ApplicationDBcontext context)
    {
        _context = context;
    }

    // Dữ liệu hiển thị
    public List<UserStats> UserStatsList { get; set; } = new();
    
    // Input tìm kiếm
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int TopN { get; set; } = 10; // Mặc định top 10

    public async Task<IActionResult> OnGetAsync()
    {
        var query = _context.Users
            .Include(u => u.Badge)
            .AsQueryable();

        // Tìm kiếm theo tên hoặc ID
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            if (Guid.TryParse(SearchTerm, out Guid id))
            {
                query = query.Where(u => u.Id == id || u.Fullname.Contains(SearchTerm) || u.Username.Contains(SearchTerm));
            }
            else
            {
                query = query.Where(u => u.Fullname.Contains(SearchTerm) || u.Username.Contains(SearchTerm));
            }
        }

        // Lấy top N người dùng có số lần làm bài nhiều nhất
        var userExamCounts = await _context.Examresults
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, ExamCount = g.Count() })
            .ToDictionaryAsync(g => g.UserId, g => g.ExamCount);

        var users = await query.ToListAsync();

        UserStatsList = users
            .Select(u => new UserStats
            {
                User = u,
                ExamCount = userExamCounts.GetValueOrDefault(u.Id, 0),
                AvgScore = _context.Examresults
                    .Where(e => e.UserId == u.Id)
                    .Average(e => (double?)e.TotalScore) ?? 0,
                MaxScore = _context.Examresults
                    .Where(e => e.UserId == u.Id)
                    .Max(e => (double?)e.TotalScore) ?? 0,
                MinScore = _context.Examresults
                    .Where(e => e.UserId == u.Id)
                    .Min(e => (double?)e.TotalScore) ?? 0
            })
            .OrderByDescending(s => s.ExamCount)
            .ThenByDescending(s => s.AvgScore)
            .Take(TopN)
            .ToList();

        return Page();
    }

    // Class hỗ trợ hiển thị
    public class UserStats
    {
        public User User { get; set; } = new();
        public int ExamCount { get; set; }
        public double AvgScore { get; set; }
        public double MaxScore { get; set; }
        public double MinScore { get; set; }
    }
}