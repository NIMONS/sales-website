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

			TempData["success"] = "Add item to cart successfully";

			return Redirect(Request.Headers["Referer"].ToString());
		}

		public async Task<IActionResult> Decrease(int id)
		{
			//giỏ hàng ở web
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(x => x.ProductId == id).FirstOrDefault();

			if(cartItem.Quantity >= 1)
			{
				cartItem.Quantity-=1;
			}
			else
			{
				 //cart.Remove(cartItem);
				 cart.RemoveAll(x => x.ProductId == id);
			}

			if(cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Decrease item quantity to cart successfully";

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Increase(int id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(x => x.ProductId == id).FirstOrDefault();

			if (cartItem.Quantity >= 1)
			{
				cartItem.Quantity+=1;
			}
			else
			{
				//cart.Remove(cartItem);
				cart.RemoveAll(x => x.ProductId == id);
			}

			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Increase item quantity to cart successfully";

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Remove(int id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

			cart.RemoveAll(p => p.ProductId == id);

			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Remove item to cart successfully";

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Clear()
		{
			HttpContext.Session.Remove("Cart");

			TempData["success"] = "You're clead your cart successfully";
			return RedirectToAction("Index");

		}
	}

}
