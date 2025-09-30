// Pages/Admin/Questions/MQC/EditMCQ.cshtml.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Data;
using DSA_PR25.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DSA_PR25.Pages.Admin.Questions.MQC
{
    public class EditMCQModel : PageModel
    {
        
        private readonly ApplicationDBcontext _context;

        public EditMCQModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        [BindProperty]
        public MCQEditModel Input { get; set; } = new();

       public SelectList ChapterList { get; set; } = null!; // ✅ SỬA DÒNG NÀY
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions
                .Include(q => q.Multiplechoiceexams)
                    .ThenInclude(m => m.Multiplechoicecorrectanswers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null || question.QuestionType != "mcq") return NotFound();

            var mcq = question.Multiplechoiceexams.FirstOrDefault();
            if (mcq == null) return NotFound();

            var chapters = await _context.Chapters.ToListAsync();
            ChapterList = new SelectList(chapters, "Id", "Name", question.ChapterId);

            Input = new MCQEditModel
            {
                Id = question.Id,
                ChapterId = question.ChapterId ?? 0,
                BloomLevel = question.BloomLevel ?? "r",
                Content = question.Content ?? "",
                Img = question.Img,
                OptionA = mcq.OptionA ?? "",
                OptionB = mcq.OptionB ?? "",
                OptionC = mcq.OptionC,
                OptionD = mcq.OptionD,
                CorrectOptions = mcq.Multiplechoicecorrectanswers.Select(a => a.CorrectOption).ToList()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var chapters = await _context.Chapters.ToListAsync();
                ChapterList = new SelectList(chapters, "Id", "Name", Input.ChapterId);
                return Page();
            }

            var question = await _context.Questions
                .Include(q => q.Multiplechoiceexams)
                    .ThenInclude(m => m.Multiplechoicecorrectanswers)
                .FirstOrDefaultAsync(q => q.Id == Input.Id);

            if (question == null) return NotFound();

            // Cập nhật Question
            question.ChapterId = Input.ChapterId;
            question.BloomLevel = Input.BloomLevel;
            question.Content = Input.Content;
            question.Img = Input.Img;
            question.UpdatedAt = DateTime.UtcNow;

            var mcq = question.Multiplechoiceexams.FirstOrDefault();
            if (mcq != null)
            {
                // Cập nhật đáp án
                mcq.OptionA = Input.OptionA;
                mcq.OptionB = Input.OptionB;
                mcq.OptionC = Input.OptionC;
                mcq.OptionD = Input.OptionD;
                mcq.UpdatedAt = DateTime.UtcNow;

                // Xóa đáp án cũ
                _context.Multiplechoicecorrectanswers.RemoveRange(mcq.Multiplechoicecorrectanswers);

                // Thêm đáp án mới
                foreach (var opt in Input.CorrectOptions)
                {
                    _context.Multiplechoicecorrectanswers.Add(new Multiplechoicecorrectanswer
                    {
                        Id = Guid.NewGuid(),
                        ExamId = mcq.Id,
                        CorrectOption = opt
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("../SeenMCQ");
        }
    }

    public class MCQEditModel
    {
        public Guid Id { get; set; }

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