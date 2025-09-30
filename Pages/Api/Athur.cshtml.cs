using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using DSA_PR25.Data;
using DSA_PR25.Models;

namespace DSA_PR25.Pages.api
{
    [IgnoreAntiforgeryToken]
    public class AuthModel : PageModel
    {
        private readonly ApplicationDBcontext _context;

        public AuthModel(ApplicationDBcontext context)
        {
            _context = context;
        }

        [BindProperty] public string? Username { get; set; }
        [BindProperty] public string? Password { get; set; }
        [BindProperty] public string? Fullname { get; set; }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
              
                return new JsonResult(new { message = "Invalid credentials" });


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "student"),
                new Claim("UserId", user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return new JsonResult(new { message = "Login successful", role = user.Role });
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (await _context.Users.AnyAsync(u => u.Username == Username))
                return BadRequest(new { message = "Username already exists" });

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = Username!,
                Fullname = Fullname ?? "",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password!),
                Role = "student",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new JsonResult(new { message = "Register successful" });
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return new JsonResult(new { message = "Logout successful" });
        }
    }
}
