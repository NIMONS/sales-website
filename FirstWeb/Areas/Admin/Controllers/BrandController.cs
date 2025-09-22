using FirstWeb.Models;
using FirstWeb.Repository;
using FirstWeb.Repository.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;

namespace FirstWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class BrandController : Controller
	{
		private readonly DataContext _dataContext;

		public BrandController(DataContext dataContext)
		{
			this._dataContext = dataContext;
		}

		public async Task<IActionResult> Index()
		{
			var listBrand = await this._dataContext.Brands.OrderByDescending(b=>b.Id).ToArrayAsync();
			return View(listBrand);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BrandModel brand)
		{

			brand.Description = HttpUtility.HtmlDecode(brand.Description);
			brand.Description = Regex.Replace(brand.Description, "<.*?>", string.Empty);

			if (ModelState.IsValid)
			{
				brand.Slug = brand.Name.Replace(" ", "-");
				var slug = await this._dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Thương hiệu đã có trong database");
					return View(brand);
				}

				this._dataContext.Add(brand);
				await this._dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm thương hiệu thành công";

				return RedirectToAction("Index");
			}
			else
			{
				this.ModelErrorsToBadRequest();
			}
			return View(brand);
		}

		[Route("Brand/Delete/{id?}")]
		[HttpGet]
		public async Task<IActionResult> Delete(int Id)
		{
			var brandModel = await this._dataContext.Brands.FindAsync(Id);

			if (brandModel == null)
			{
				TempData["error"] = "Thương hiệu không tồn tại";
				return NotFound();
			}

			this._dataContext.Brands.Remove(brandModel);
			await this._dataContext.SaveChangesAsync();

			TempData["success"] = "Xóa thương hiệu thành công";
			return RedirectToAction("Index");
		}

		[Route("Brand/Edit/{id?}")]
		[HttpGet]
		public async Task<IActionResult> Edit(int Id)
		{
			BrandModel brandModel = await this._dataContext.Brands.FindAsync(Id);

			if (brandModel == null)
			{
				TempData["error"] = "không tìm thấy thương hiệu sản phẩm";
				return RedirectToAction("Index");
			}

			return View(brandModel);
		}

		[Route("Brand/Edit/{id?}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(BrandModel brand)
		{
			brand.Description = HttpUtility.HtmlDecode(brand.Description);
			brand.Description = Regex.Replace(brand.Description, "<.*?>", string.Empty);

			var existed_product = await this._dataContext.Categories.FindAsync(brand.Id);
			if (existed_product == null)
			{
				TempData["error"] = "không tìm thấy thương hiệu để cập nhật";
				return RedirectToAction("Index");
			}

			if (existed_product.Name != brand.Name)
			{
				brand.Slug = brand.Name.Replace(" ", "-");

				var slug = await this._dataContext.Brands
					.FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);

				if (slug != null)
				{
					ModelState.AddModelError("", "Thương hiệu đã tồn tại (Slug bị trùng)");
					return View(brand);
				}
				existed_product.Slug = brand.Slug;
			}

			if (ModelState.IsValid)
			{
				existed_product.Name = brand.Name;
				existed_product.Description = brand.Description;
				existed_product.Status = brand.Status;

				this._dataContext.Update(existed_product);

				await this._dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật thương hiệu thành công";
				return RedirectToAction("Index");
			}
			else
			{
				this.ModelErrorsToBadRequest();
			}

			return View(brand);

		}
	}
}
