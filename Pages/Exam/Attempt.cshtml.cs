using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Models;
using DSA_PR25.Data;
using System.Security.Claims;

namespace DSA_PR25.Pages.Exam;

public class AttemptModel : PageModel
{
    private readonly ApplicationDBcontext _context;

    public AttemptModel(ApplicationDBcontext context)
    {
        _context = context;
    }

    public List<QuestionViewModel> Questions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Kiểm tra xem session có đủ thông tin không
        if (!HttpContext.Session.TryGetValue("ExamChapterId", out _) ||
            !HttpContext.Session.TryGetValue("ExamBloomLevel", out _) ||
            !HttpContext.Session.TryGetValue("ExamQuestionCount", out _))
        {
            return RedirectToPage("Setup");
        }

        var chapterIdStr = HttpContext.Session.GetString("ExamChapterId");
        var bloomLevel = HttpContext.Session.GetString("ExamBloomLevel");
        var countNullable = HttpContext.Session.GetInt32("ExamQuestionCount");

        // Xác thực dữ liệu session
        if (string.IsNullOrEmpty(chapterIdStr) ||
            string.IsNullOrEmpty(bloomLevel) ||
            !countNullable.HasValue ||
            !int.TryParse(chapterIdStr, out var chapterId) ||
            countNullable <= 0)
        {
            return RedirectToPage("Setup");
        }

        var count = countNullable.Value;

        // Lấy danh sách ID câu hỏi phù hợp
        var questionIds = await _context.Questions
            .Where(q => q.ChapterId == chapterId && q.BloomLevel == bloomLevel)
            .Select(q => q.Id)
            .ToListAsync();

        if (!questionIds.Any())
        {
            ModelState.AddModelError("", "Không có câu hỏi phù hợp!");
            return RedirectToPage("Setup");
        }

        // Chọn ngẫu nhiên
        var random = new Random();
        var selectedIds = questionIds
            .OrderBy(x => random.Next())
            .Take(count)
            .ToList();

        // Lấy toàn bộ câu hỏi đã chọn
        var questions = await _context.Questions
            .Where(q => selectedIds.Contains(q.Id))
            .ToListAsync();

        // Map sang ViewModel
        foreach (var q in questions)
        {
            if (q.QuestionType == "mcq")
            {
                var mc = await _context.Multiplechoiceexams
                    .FirstOrDefaultAsync(m => m.QuestionId == q.Id);
                Questions.Add(new QuestionViewModel
                {
                    Id = q.Id,
                    Content = q.Content ?? "",
                    Img = q.Img,
                    Type = "mcq",
                    Options = new[]
                    {
                        mc?.OptionA ?? "",
                        mc?.OptionB ?? "",
                        mc?.OptionC ?? "",
                        mc?.OptionD ?? ""
                    }
                });
            }
            else // fill-in-the-blank
            {
                var fb = await _context.Fillinblankexams
                    .FirstOrDefaultAsync(f => f.QuestionId == q.Id);
                Questions.Add(new QuestionViewModel
                {
                    Id = q.Id,
                    Content = q.Content ?? "",
                    Img = q.Img,
                    Type = "fn",
                    CorrectAnswer = fb?.CorrectAnswer ?? ""
                });
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(List<UserAnswer> Answers)
    {
        // Xác thực người dùng
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                       ?? User.FindFirst("Id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        if (Answers == null || !Answers.Any())
        {
            return BadRequest("Không có câu trả lời nào được gửi.");
        }

        float totalScore = 0;
        int correct = 0;
        int total = Answers.Count;

        foreach (var ans in Answers)
        {
            var question = await _context.Questions.FindAsync(ans.QuestionId);
            if (question == null) continue;

            var bloom = await _context.Bloomscoring
                .FirstOrDefaultAsync(b => b.BloomLevel == question.BloomLevel);

            if (bloom == null) continue; // phòng trường hợp không có scoring rule

            if (question.QuestionType == "mcq")
            {
                // Lấy đáp án đúng (giả sử bảng Multiplechoicecorrectanswers có QuestionId)
                var correctOptions = await _context.Multiplechoicecorrectanswers
                    .Where(c => c.Id == ans.QuestionId)
                    .Select(c => c.CorrectOption)
                    .ToListAsync();

                if (correctOptions.Contains(ans.Response))
                {
                    correct++;
                    totalScore += bloom.ScoreMultiplier;
                }
            }
            else // fill-in-the-blank
            {
                var fb = await _context.Fillinblankexams
                    .FirstOrDefaultAsync(f => f.QuestionId == ans.QuestionId);

                if (fb == null) continue;

                bool isCorrect = false;

                // Kiểm tra số (có tolerance)
                if (double.TryParse(fb.CorrectAnswer, out double correctNum) &&
                    double.TryParse(ans.Response, out double userNum))
                {
                    var tolerance = fb.Tolerance ?? 0;
                    isCorrect = Math.Abs(userNum - correctNum) <= tolerance;
                }
                else
                {
                    // Kiểm tra văn bản: so sánh không phân biệt hoa thường + từ đồng nghĩa
                    var userResponse = ans.Response.Trim().ToLower();
                    var correctLower = fb.CorrectAnswer.Trim().ToLower();
                    var synonyms = (fb.Synonyms ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLower())
                        .ToList();

                    isCorrect = userResponse == correctLower || synonyms.Contains(userResponse);
                }

                if (isCorrect)
                {
                    correct++;
                    totalScore += bloom.ScoreMultiplier;
                }
            }
        }

        // Lưu kết quả bài thi
        var examResult = new Examresult
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TotalScore = totalScore,
            TotalQuestions = total,
            CorrectAnswers = correct,
            BloomScore = totalScore,
            ExamDate = DateTime.UtcNow, // hoặc DateTime.Now tùy theo yêu cầu
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Examresults.Add(examResult);
        await _context.SaveChangesAsync();

        // Cập nhật EXP cho người dùng
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Exp += (int)(totalScore * 10);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("Result", new { id = examResult.Id });
    }

    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Img { get; set; }
        public string Type { get; set; } = "mcq"; // "mcq" hoặc "fn"
        public string[] Options { get; set; } = new string[4];
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    public class UserAnswer
    {
        public Guid QuestionId { get; set; }
        public string Response { get; set; } = string.Empty;
    }
}