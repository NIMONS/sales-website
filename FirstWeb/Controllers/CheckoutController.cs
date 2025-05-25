using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Controllers
{
	public class CheckoutController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
