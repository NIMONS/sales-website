using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Controllers
{
	public class CategoryController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
