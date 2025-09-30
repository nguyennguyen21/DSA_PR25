
using Microsoft.AspNetCore.Mvc;

namespace DSA_PR25.ViewComponents  
{
    public class MathKeyboardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string targetInputId, string? value = null)
        {
            var model = new MathKeyboardModel
            {
                TargetInputId = targetInputId,
                Value = value ?? string.Empty
            };
            return View("MathKeyboard",model);
        }
    }

    public class MathKeyboardModel
    {
        public string TargetInputId { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}