using FirstWeb.Models;
using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;

namespace FirstWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
		{
			this._dataContext = dataContext;
			this._webHostEnvironment = webHostEnvironment;
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

		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryID);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			product.Description = HttpUtility.HtmlDecode(product.Description);
			product.Description = Regex.Replace(product.Description, "<.*?>", string.Empty);

			if (ModelState.IsValid)
			{
				product.Slug = product.Name.Replace(" ", "-");
				var slug = await this._dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null) 
				{
					ModelState.AddModelError("","Sản phẩm đã có trong database");
					return View(product);
				}
			
				if(product.ImageUpload != null)
				{
					string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);

					fs.Close();
					product.Image = imageName;
				}

				TempData["success"] = "Thêm sản phẩm thành công";
				this._dataContext.Add(product);
				await this._dataContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "model bị thiếu giá trị";
				List<string> errors = new List<string>();
				foreach(var value in ModelState.Values)
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

		public async Task<IActionResult> Delete(int productId)
		{
			var productModel = await this._dataContext.Products.FindAsync(productId);

			if (productModel == null) 
			{
				TempData["error"] = "sản phẩm không tồn tại";
				return RedirectToAction("Index");
			}

			this._dataContext.Products.Remove(productModel);
			await this._dataContext.SaveChangesAsync();
			
			TempData["success"] = "Xóa sản phẩm thành công";
			return RedirectToAction("Index");
		}
		
	}
}
