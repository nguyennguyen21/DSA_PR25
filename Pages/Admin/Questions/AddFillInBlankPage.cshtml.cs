using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DSA_PR25.Models;
using System.ComponentModel.DataAnnotations;
using DSA_PR25.Data;

namespace DSA_PR25.Pages.Questions
{
  
    public class AddFillInBlankPageModel : PageModel
    {
        
        private readonly ApplicationDBcontext _context;

        public AddFillInBlankPageModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        // Danh sách chương để đổ vào dropdown
        public List<Chapter> Chapters { get; set; } = new();

        // Input model
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet()
        {
            Chapters = _context.Chapters.ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Chapters = _context.Chapters.ToList(); // Load lại để hiển thị nếu lỗi

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Tạo Question
            var question = new Question
            {
                Id = Guid.NewGuid(),
                ChapterId = Input.ChapterId,
                BloomLevel = Input.BloomLevel,
                QuestionType = "fn", // điền khuyết đơn
                Content = Input.Content.Trim(),
                Img = string.IsNullOrWhiteSpace(Input.Img) ? null : Input.Img.Trim(),
                CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000"), // ⚠️ Thay bằng User ID thực tế
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Tạo Fill-in-blank chi tiết
            var fillExam = new Fillinblankexam
            {
                Id = Guid.NewGuid(),
                QuestionId = question.Id,
                CorrectAnswer = Input.CorrectAnswer.Trim(),
                Tolerance = Input.Tolerance,
                Synonyms = string.IsNullOrWhiteSpace(Input.Synonyms) ? null : Input.Synonyms.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(question);
            _context.Fillinblankexams.Add(fillExam);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm câu hỏi điền khuyết thành công!";
            return RedirectToPage("./Index"); // hoặc quay lại danh sách
        }

        // Input model cho form
        public class InputModel
        {
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