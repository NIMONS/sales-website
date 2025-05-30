namespace FirstWeb.Models
{
	public class CartItemModel
	{
		public long ProductId { get; set; }

		public string ProductName { get; set; }

		public int Quantity { get; set; }

		public decimal Price { get; set; }

		public decimal Total {
			get { return Quantity * Price; }
		}

		public string Image{ get; set; }

		public CartItemModel() { }

		public CartItemModel(ProductModel product)
		{
			this.ProductId = product.Id;
			this.ProductName = product.Name;
			this.Price = product.Price;
			this.Quantity = 1;
			this.Image = product.Image;
		}
	}
}
