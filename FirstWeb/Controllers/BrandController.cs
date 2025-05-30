using FirstWeb.Models;
using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstWeb.Controllers
{
	public class BrandController : Controller
	{
		private readonly DataContext _dataContext;

		public BrandController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public async Task<IActionResult > Index(string slug = "")
		{
			BrandModel brand = this._dataContext.Brands.Where(c => c.Slug == slug).FirstOrDefault();	
			if (brand == null) return RedirectToAction("Index");
			var productsByBrand = this._dataContext.Products.Where(c => c.BrandId == brand.Id);

			return View(await productsByBrand.OrderByDescending(c => c.Id).ToListAsync());
		}
	}
}
