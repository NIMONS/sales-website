using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
			if(id == 0) return RedirectToAction("Index");

			var productById = await this._dataContext.Products.Where(c => c.Id == id).FirstOrDefaultAsync();


			return View(productById);
		}
	}
}
