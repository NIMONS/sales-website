using FirstWeb.Models;
using FirstWeb.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;

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

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CategoryModel category)
		{

			category.Description = HttpUtility.HtmlDecode(category.Description);
			category.Description = Regex.Replace(category.Description, "<.*?>", string.Empty);

			if (ModelState.IsValid)
			{
				category.Slug = category.Name.Replace(" ", "-");
				var slug = await this._dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Danh mục đã có trong database");
					return View(category);
				}

				this._dataContext.Add(category);
				await this._dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm danh mục thành công";

				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "model bị thiếu giá trị";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}

				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}

		}

		[HttpGet]
		public async Task<IActionResult> Delete(int Id)
		{
			var categoryModel = await this._dataContext.Categories.FindAsync(Id);

			if (categoryModel == null)
			{
				TempData["error"] = "Danh mục không tồn tại";
				return NotFound();
			}

			this._dataContext.Categories.Remove(categoryModel);
			await this._dataContext.SaveChangesAsync();

			TempData["success"] = "Xóa danh mục thành công";
			return RedirectToAction("Index");
		}

		[Route("Category/Edit/{id?}")]
		[HttpGet]
		public async Task<IActionResult> Edit(int Id)
		{
			CategoryModel categoryModel = await this._dataContext.Categories.FindAsync(Id);

			if (categoryModel == null)
			{
				TempData["error"] = "không tìm thấy danh mục sản phẩm";
				return RedirectToAction("Index");
			}

			return View(categoryModel);
		}

		[Route("Category/Edit/{id?}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CategoryModel category)
		{
			category.Description = HttpUtility.HtmlDecode(category.Description);
			category.Description = Regex.Replace(category.Description, "<.*?>", string.Empty);

			var existed_product = await this._dataContext.Categories.FindAsync(category.Id);
			if (existed_product == null)
			{
				TempData["error"] = "không tìm thấy danh mục để cập nhật";
				return RedirectToAction("Index");
			}

			if (existed_product.Name != category.Name)
			{
				category.Slug = category.Name.Replace(" ", "-");

				var slug = await this._dataContext.Categories
					.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);

				if (slug != null)
				{
					ModelState.AddModelError("", "Danh mục đã tồn tại (Slug bị trùng)");
					return View(category);
				}
				existed_product.Slug = category.Slug;
			}

			if (ModelState.IsValid)
			{
				existed_product.Name = category.Name;
				existed_product.Description = category.Description;
				existed_product.Status = category.Status;

				this._dataContext.Update(existed_product);

				await this._dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật danh mục thành công";
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "model bị thiếu giá trị";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}

				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}

		}

	}
}
