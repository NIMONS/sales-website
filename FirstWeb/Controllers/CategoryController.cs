using FirstWeb.Models;
using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstWeb.Controllers
{
	public class CategoryController : Controller
	{
		private readonly DataContext _dataContext;
		public CategoryController(DataContext dataContext)
		{
			this._dataContext = dataContext;
		}

		public async Task<IActionResult> Index(string slug = "")
		{
			CategoryModel category = this._dataContext.Categories.Where(c => c.Slug == slug).FirstOrDefault();
			if (category == null) return RedirectToAction("Index");
			var productsByCategory = this._dataContext.Products.Where(c => c.CategoryID == category.Id);

			return View(await productsByCategory.OrderByDescending(c=> c.Id).ToListAsync());
		}
	}
}
