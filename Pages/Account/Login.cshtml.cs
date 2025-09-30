using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DSA_PR25.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DSA_PR25.Pages.Account
{
    public class Login : PageModel
    {
        private readonly ApplicationDBcontext _context;

        public Login(ApplicationDBcontext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var hashedPassword = HashPassword(Input.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Input.Username && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            // Tạo danh sách claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "student"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Điều hướng sau khi đăng nhập
            if (user.Role == "admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            if (user.Role == "student")
            {
                return RedirectToPage("/Index");
            }
            return RedirectToPage("/Login");
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
