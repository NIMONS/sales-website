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

	}
}
