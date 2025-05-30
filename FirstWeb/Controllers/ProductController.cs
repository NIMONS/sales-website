using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Controllers
{
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

		public async Task<IActionResult > Details(int id)
		{
			if(id == null) return RedirectToAction("Index");

			var productById = this._dataContext.Products.Where(c => c.Id == id).FirstOrDefault();


			return View(productById);
		}
	}
}
