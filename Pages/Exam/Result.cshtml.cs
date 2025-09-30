using DSA_PR25.Data;
using Microsoft.AspNetCore.Mvc;
using DSA_PR25.Models; 
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace DSA_PR25.Pages.Exam;
public class ResultModel : PageModel
{
    private readonly ApplicationDBcontext _context;

    public ResultModel(ApplicationDBcontext context) => _context = context;

    public Examresult Result { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Result = await _context.Examresults.FindAsync(id);
        if (Result == null) return NotFound();
        return Page();
    }
}