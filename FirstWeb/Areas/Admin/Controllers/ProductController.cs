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

		public async Task<IActionResult> Delete(int Id)
		{
			var productModel = await this._dataContext.Products.FindAsync(Id);

			if (productModel == null) 
			{
				TempData["error"] = "sản phẩm không tồn tại";
				return NotFound();
			}

			
			string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
			string oldfileImage = Path.Combine(uploadDir, productModel.Image);
			try
			{
				if (System.IO.File.Exists(oldfileImage))
				{
					System.IO.File.Delete(oldfileImage);
				}
			}
			catch (Exception ex) 
			{
				ModelState.AddModelError("", "Lỗi khi cố gắng xóa ảnh");
			}

			

			this._dataContext.Products.Remove(productModel);
			await this._dataContext.SaveChangesAsync();
			
			TempData["success"] = "Xóa sản phẩm thành công";
			return RedirectToAction("Index");
		}

		[Route("Edit")]
		public async Task<IActionResult> Edit(int Id)
		{
			ProductModel productModel = await this._dataContext.Products.FindAsync(Id);

			if (productModel == null) 
			{
				TempData["error"] = "không tìm thấy sản phẩm";
				return RedirectToAction("Index");
			}

			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", productModel.CategoryID);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", productModel.BrandId);

			return View(productModel);
		}

		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryID);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			product.Description = HttpUtility.HtmlDecode(product.Description);
			product.Description = Regex.Replace(product.Description, "<.*?>", string.Empty);

			var existed_product = await this._dataContext.Products.FindAsync(product.Id);
			if (existed_product == null) 
			{
				TempData["error"] = "không tìm thấy sản phẩm để cập nhật";
				return RedirectToAction("Index");
			}

			if(existed_product.Name != product.Name)
			{
				product.Slug = product.Name.Replace(" ", "-");

				var slug = await this._dataContext.Products
					.FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);

				if(slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã tồn tại (Slug bị trùng)");
					return View(product);
				}
				existed_product.Slug = product.Slug;
			}

			if (ModelState.IsValid)
			{
				
				if (product.ImageUpload != null)
				{
					string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadDir, imageName);

					string oldfileImage = Path.Combine(uploadDir, existed_product.Image);
					try
					{
						if (System.IO.File.Exists(oldfileImage))
						{
							System.IO.File.Delete(oldfileImage);
						}
					}
					catch (Exception ex)
					{
						ModelState.AddModelError("", "Lỗi khi cố gắng xóa ảnh");
					}

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);

					fs.Close();
					existed_product.Image = imageName;

					
				}

				existed_product.Name = product.Name;
				existed_product.Description = product.Description;
				existed_product.Price = product.Price;
				existed_product.CategoryID = product.CategoryID;
				existed_product.BrandId = product.BrandId;

				this._dataContext.Update(existed_product);

				await this._dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật sản phẩm thành công";
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
