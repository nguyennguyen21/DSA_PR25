
using Microsoft.AspNetCore.Mvc;

namespace DSA_PR25.ViewComponents  
{
    public class SideBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            
            return View("SideBar");
        }
    }

   
}