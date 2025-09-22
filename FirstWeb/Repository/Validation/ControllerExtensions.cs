using Microsoft.AspNetCore.Mvc;

namespace FirstWeb.Repository.Validation
{
	public static class ControllerExtensions
	{
		public static IActionResult ModelErrorsToBadRequest(this Controller controller)
		{
			controller.TempData["error"] = "Model bị thiếu giá trị";

			var errors = controller.ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(s => s.ErrorMessage)
				.ToList();

			string errorMessage = string.Join("\n", errors);

			return controller.BadRequest(errorMessage);
		}
	}
}
