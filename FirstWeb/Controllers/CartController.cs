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

		public async Task<IActionResult> Add(int id)
		{
			ProductModel product = await _datacontext.Products.FindAsync(id);
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItems = cart.Where(x => x.ProductId == id).FirstOrDefault();

			if (cartItems == null) {
				cart.Add(new CartItemModel(product));
			}
			else
			{
				cartItems.Quantity+=1;
			}

			HttpContext.Session.SetJson("Cart", cart);

			return Redirect(Request.Headers["Referer"].ToString());
		}
		
	}
}
