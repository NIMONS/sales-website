using FirstWeb.Models;
using FirstWeb.Models.ViewModels;
using FirstWeb.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Controllers
{
	public class CartController : Controller
	{
		private readonly DataContext _datacontext;
		public CartController(DataContext datacontext)
		{
			this._datacontext = datacontext;
		}

		public IActionResult Index()
		{
			List<CartItemModel> cartitems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemViewModel cartVM = new()
			{
				CartItems = cartitems,
				GrandTotal = cartitems.Sum(x => x.Quantity *x.Price),
			};
			return View(cartVM);
		}
		
	}
}
