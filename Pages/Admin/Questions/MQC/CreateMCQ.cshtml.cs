// Pages/Questions/CreateMCQ.cshtml.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using DSA_PR25.Data;

namespace DSA_PR25.Pages.Questions
{
   
    public class CreateMCQModel : PageModel
    {
        private readonly ApplicationDBcontext _context; // Thay bằng tên DbContext thực tế của bạn

        public CreateMCQModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        [BindProperty]
        public MCQInputModel Input { get; set; } = new();

        public SelectList ChapterList { get; set; } = new SelectList(new List<Chapter>(), "Id", "Name");

        public async Task<IActionResult> OnGetAsync()
        {
            var chapters = await _context.Chapters.ToListAsync();
            ChapterList = new SelectList(chapters, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var chapters = await _context.Chapters.ToListAsync();
                ChapterList = new SelectList(chapters, "Id", "Name");
                return Page();
            }

            // Tạo Question
            var question = new Question
            {
                Id = Guid.NewGuid(),
                ChapterId = Input.ChapterId,
                BloomLevel = Input.BloomLevel,
                QuestionType = "mcq", // Theo enum trong DB
                Content = Input.Content,
                Img = Input.Img,
                CreatedBy = Guid.Parse("..."), // Thay bằng ID user thực tế (vd: từ HttpContext.User)
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(question);

            // Tạo MultipleChoiceExam
            var mcqExam = new Multiplechoiceexam
            {
                Id = Guid.NewGuid(),
                QuestionId = question.Id,
                OptionA = Input.OptionA,
                OptionB = Input.OptionB,
                OptionC = Input.OptionC,
                OptionD = Input.OptionD,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Multiplechoiceexams.Add(mcqExam);

            // Lưu đáp án đúng
            foreach (var option in Input.CorrectOptions)
            {
                _context.Multiplechoicecorrectanswers.Add(new Multiplechoicecorrectanswer
                {
                    Id = Guid.NewGuid(),
                    ExamId = mcqExam.Id,
                    CorrectOption = option
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index"); // Hoặc trang thành công
        }
    }

    public class MCQInputModel
    {
        [Required]
        public int ChapterId { get; set; }

        [Required]
        public string BloomLevel { get; set; } = "r";

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? Img { get; set; }

        [Required]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        public string OptionB { get; set; } = string.Empty;

        public string? OptionC { get; set; }

        public string? OptionD { get; set; }

        [Required]
        public List<string> CorrectOptions { get; set; } = new();
    }
}