using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Models;
using System.ComponentModel.DataAnnotations;
using DSA_PR25.Data;

namespace DSA_PR25.Pages.Admin.Questions
{
    public class EditFillInBlankPageModel : PageModel
    {
        private readonly ApplicationDBcontext _context;

        public EditFillInBlankPageModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        // Danh sách chương để đổ vào dropdown
        public List<Chapter> Chapters { get; set; } = new();

        [BindProperty]
        public InputModel Input { get; set; } = new();

        // GET: Load dữ liệu câu hỏi để sửa
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            // Lấy câu hỏi + chapter + fill-in-blank
            var question = await _context.Questions
                .Include(q => q.Chapter)
                .Include(q => q.Fillinblankexams)
                .FirstOrDefaultAsync(q => q.Id == id && q.QuestionType == "fn");

            if (question == null) return NotFound();

            var fillExam = question.Fillinblankexams.FirstOrDefault();
            if (fillExam == null) return NotFound();

            // Gán dữ liệu vào Input
            Input = new InputModel
            {
                Id = question.Id,
                Content = question.Content ?? string.Empty,
                Img = question.Img,
                ChapterId = question.ChapterId ?? 0,
                BloomLevel = question.BloomLevel ?? "r",
                CorrectAnswer = fillExam.CorrectAnswer,
                Tolerance = fillExam.Tolerance,
                Synonyms = fillExam.Synonyms
            };

            Chapters = await _context.Chapters.ToListAsync();
            return Page();
        }

        // POST: Cập nhật câu hỏi
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Chapters = await _context.Chapters.ToListAsync();
                return Page();
            }

            var question = await _context.Questions
                .Include(q => q.Fillinblankexams)
                .FirstOrDefaultAsync(q => q.Id == Input.Id);

            if (question == null) return NotFound();

            var fillExam = question.Fillinblankexams.FirstOrDefault();
            if (fillExam == null) return NotFound();

            // Cập nhật Question
            question.Content = Input.Content.Trim();
            question.Img = string.IsNullOrWhiteSpace(Input.Img) ? null : Input.Img.Trim();
            question.ChapterId = Input.ChapterId;
            question.BloomLevel = Input.BloomLevel;
            question.UpdatedAt = DateTime.UtcNow;

            // Cập nhật Fill-in-blank
            fillExam.CorrectAnswer = Input.CorrectAnswer.Trim();
            fillExam.Tolerance = Input.Tolerance;
            fillExam.Synonyms = string.IsNullOrWhiteSpace(Input.Synonyms) ? null : Input.Synonyms.Trim();
            fillExam.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật câu hỏi thành công!";
            return RedirectToPage("Index");
        }

        // Input model cho form
        public class InputModel
        {
            public Guid Id { get; set; }

            [Required(ErrorMessage = "Nội dung câu hỏi không được để trống")]
            public string Content { get; set; } = string.Empty;

            public string? Img { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn chương")]
            public int ChapterId { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn mức độ Bloom")]
            public string BloomLevel { get; set; } = "r";

            [Required(ErrorMessage = "Đáp án đúng không được để trống")]
            public string CorrectAnswer { get; set; } = string.Empty;

            public double? Tolerance { get; set; }

            public string? Synonyms { get; set; }
        }
    }
}