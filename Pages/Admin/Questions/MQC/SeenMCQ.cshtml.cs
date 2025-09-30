// Pages/Admin/Questions/SeenMCQ.cshtml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Data;
using DSA_PR25.Models;

namespace DSA_PR25.Pages.Admin.Questions
{
    
    public class SeenMCQModel : PageModel
    {
        private readonly ApplicationDBcontext _context;

        public SeenMCQModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        public IList<QuestionWithMCQ> MCQList { get; set; } = new List<QuestionWithMCQ>();

        public async Task OnGetAsync()
        {
            var questions = await _context.Questions
                .Where(q => q.QuestionType == "mcq")
                .Include(q => q.Chapter)
                .Include(q => q.Multiplechoiceexams)
                    .ThenInclude(m => m.Multiplechoicecorrectanswers)
                .ToListAsync();

            MCQList = questions.Select(q => new QuestionWithMCQ
            {
                Question = q,
                MCQ = q.Multiplechoiceexams.FirstOrDefault(),
                CorrectAnswers = q.Multiplechoiceexams
                    .SelectMany(m => m.Multiplechoicecorrectanswers)
                    .Select(a => a.CorrectOption)
                    .ToList()
            }).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var question = await _context.Questions
                .Include(q => q.Multiplechoiceexams)
                    .ThenInclude(m => m.Multiplechoicecorrectanswers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question != null)
            {
                // Xóa đáp án đúng → EF tự động xóa do cascade (nếu cấu hình)
                // Xóa exam → cascade xóa đáp án
                // Xóa question → cascade xóa exam

                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }

    // Helper class để truyền dữ liệu sang view
    public class QuestionWithMCQ
    {
        public Question Question { get; set; } = null!;
        public Multiplechoiceexam? MCQ { get; set; }
        public List<string> CorrectAnswers { get; set; } = new();
    }
}