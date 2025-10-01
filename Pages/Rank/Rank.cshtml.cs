using Microsoft.AspNetCore.Mvc.RazorPages;
using DSA_PR25.Models;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Data;

namespace DSA_PR25.Pages.Rank
{
    public class RankModel : PageModel
    {
        private readonly ApplicationDBcontext _context; // ← Thay bằng tên DbContext thực tế của bạn

        public RankModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        public IList<RankItem> RankList { get; set; } = new List<RankItem>();

        public void OnGet()
        {
            // Bước 1: Lấy tất cả kết quả thi + thông tin user về bộ nhớ
            var allResults = _context.Examresults
                .Include(e => e.User)
                .ToList(); // ← Thực thi truy vấn tại đây

            // Bước 2: Xử lý trên client (in-memory)
            var rankedResults = allResults
                .GroupBy(e => e.UserId)
                .Select(g => g.OrderByDescending(e => e.TotalScore).First()) // điểm cao nhất của mỗi user
                .OrderByDescending(e => e.TotalScore)
                .ThenBy(e => e.ExamDate) // ai thi trước xếp trên nếu bằng điểm
                .ToList();

            RankList = rankedResults.Select((exam, index) => new RankItem
            {
                Rank = index + 1,
                Username = exam.User.Username,
                Fullname = exam.User.Fullname,
                TotalScore = exam.TotalScore,
                ExamDate = exam.ExamDate
            }).ToList();
        }
    }

    public class RankItem
    {
        public int Rank { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public float TotalScore { get; set; }
        public DateTime? ExamDate { get; set; }
    }
}