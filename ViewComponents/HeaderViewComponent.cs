
using Microsoft.AspNetCore.Mvc;

namespace DSA_PR25.ViewComponents  
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            
            return View("Header");
        }
    }

   
}