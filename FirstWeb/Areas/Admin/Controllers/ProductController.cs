using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
		public async Task<IActionResult> Index()
		{
			var produtcs = await this._dataContext.Products
				.OrderByDescending(p => p.Id)
				.Include("Category")
				.Include("Brand")
				.ToListAsync();

			return View(produtcs);

		}


	}
}
