﻿using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Controllers
{
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
