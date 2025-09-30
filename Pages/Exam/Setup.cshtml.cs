using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Models;
using DSA_PR25.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DSA_PR25.Pages.Exam;

public class SetupModel : PageModel
{
    private readonly ApplicationDBcontext _context;

    public SetupModel(ApplicationDBcontext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> Chapters { get; set; } = new();
    public List<(string Value, string Text)> BloomLevels { get; } = new()
    {
        ("r", "Nhớ (Remember)"),
        ("u", "Hiểu (Understand)"),
        ("ap", "Áp dụng (Apply)"),
        ("an", "Phân tích (Analyze)")
    };

    public async Task OnGetAsync()
    {
        var chapters = await _context.Chapters
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();
        Chapters = chapters;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Input.QuestionCount < 1 || Input.QuestionCount > 20)
        {
            await OnGetAsync();
            return Page();
        }

        // Lưu cấu hình vào Session hoặc TempData để dùng ở trang thi
        HttpContext.Session.SetString("ExamChapterId", Input.ChapterId.ToString());
        HttpContext.Session.SetString("ExamBloomLevel", Input.BloomLevel);
        HttpContext.Session.SetInt32("ExamQuestionCount", Input.QuestionCount);

        return RedirectToPage("Attempt");
    }

    public class InputModel
    {
        public int ChapterId { get; set; }

        [Required]
        public string BloomLevel { get; set; } = "r";

        [Range(1, 20, ErrorMessage = "Số câu phải từ 1 đến 20")]
        public int QuestionCount { get; set; }
    }
}