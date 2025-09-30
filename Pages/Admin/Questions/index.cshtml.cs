using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Models;
using System.ComponentModel.DataAnnotations;
using DSA_PR25.Data;

namespace DSA_PR25.Pages.Admin.Questions
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBcontext _context;

        public IndexModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        public IList<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

        public async Task OnGetAsync()
        {
            // 1. Truy vấn dữ liệu từ DB, Include các quan hệ
            var questionList = await _context.Questions
                .Where(q => q.QuestionType == "fn") // chỉ lấy câu điền khuyết
                .Include(q => q.Chapter)            // để lấy tên chương
                .Include(q => q.Fillinblankexams)   // để lấy đáp án, tolerance, synonyms
                .ToListAsync();

            // 2. Ánh xạ sang ViewModel (sau khi đã load về memory → được dùng ?.)
            Questions = questionList.Select(q => new QuestionViewModel
            {
                Id = q.Id,
                Content = q.Content ?? string.Empty,
                ChapterName = q.Chapter?.Name ?? "Không có chương",
                CorrectAnswer = q.Fillinblankexams.FirstOrDefault()?.CorrectAnswer ?? string.Empty,
                Tolerance = q.Fillinblankexams.FirstOrDefault()?.Tolerance,
                Synonyms = q.Fillinblankexams.FirstOrDefault()?.Synonyms,
                CreatedAt = q.CreatedAt
            }).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa câu hỏi thành công!";
            }
            return RedirectToPage();
        }

        // ViewModel để hiển thị
        public class QuestionViewModel
        {
            public Guid Id { get; set; }
            public string Content { get; set; } = string.Empty;
            public string ChapterName { get; set; } = string.Empty;
            public string CorrectAnswer { get; set; } = string.Empty;
            public double? Tolerance { get; set; }
            public string? Synonyms { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
    }
}