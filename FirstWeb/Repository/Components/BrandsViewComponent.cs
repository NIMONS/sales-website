using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstWeb.Repository.Components
{
	public class CategoriesViewComponent : ViewComponent
	{
		private readonly DataContext _dataContext;

		public CategoriesViewComponent(DataContext dataContext)
		{
			this._dataContext = dataContext;
		}

		public async Task<IViewComponentResult> InvokeAsync() => View(await this._dataContext.Categories.ToListAsync());
	}
}
