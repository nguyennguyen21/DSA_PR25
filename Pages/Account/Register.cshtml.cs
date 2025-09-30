using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DSA_PR25.Data;
using DSA_PR25.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DSA_PR25.Pages.Account
{
    public class Register : PageModel
    {
        private readonly ILogger<Register> _logger;
        private readonly ApplicationDBcontext _context;

        public Register(ILogger<Register> logger, ApplicationDBcontext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Fullname { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kiểm tra username đã tồn tại chưa
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Input.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return Page();
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = Input.Username,
                Fullname = Input.Fullname,
                PasswordHash = HashPassword(Input.Password),
                Role = "student",
                //thay  Role = "admin",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Có thể redirect về trang đăng nhập hoặc trang chủ
            return RedirectToPage("/Account/Login");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
