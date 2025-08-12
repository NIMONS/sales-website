using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;

		public ProductController(DataContext dataContext)
		{
			this._dataContext = dataContext;
		}
		public IActionResult Index()
		{
			return View();
		}
	}
}
