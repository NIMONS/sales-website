﻿using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Controllers
{
	public class CartController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		
	}
}
