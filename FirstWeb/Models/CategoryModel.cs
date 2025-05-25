using System.ComponentModel.DataAnnotations;

namespace FirstWeb.Models
{
	public class CategoryModel
	{
		[Key]
		public int Id { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên loại sản phẩm")]
		public string Name { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả loại sản phẩm")]
		public string Description { get; set; }

		[Required]
		public string Slug { get; set; }

		public int Status { get; set; }


	}
}
