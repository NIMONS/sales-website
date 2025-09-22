using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
	{
		private readonly DataContext _dataContext;

		public CategoryController(DataContext dataContext)
		{
			this._dataContext = dataContext;
		}

		public async Task<IActionResult> Index()
		{
			var listCategories = await this._dataContext.Categories.
				OrderByDescending(c => c.Id).ToListAsync();

			return View(listCategories);
		}


	}
}
