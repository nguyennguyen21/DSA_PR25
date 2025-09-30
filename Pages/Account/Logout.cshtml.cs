using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DSA_PR25.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Đăng xuất người dùng
            await HttpContext.SignOutAsync();

            // Redirect về trang chủ (hoặc trang đăng nhập)
            return RedirectToPage("/Index");
        }
    }
}