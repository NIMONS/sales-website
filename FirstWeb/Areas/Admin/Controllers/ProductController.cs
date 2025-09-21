﻿using FirstWeb.Models;
using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
			return View();
		}

		public async Task<IActionResult> Create(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryID);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			return View(product);
		}
	}
}
