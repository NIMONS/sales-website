using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstWeb.Repository.Components
{
	public class BrandsViewComponent : ViewComponent
	{
		private readonly DataContext _dataContext;

		public BrandsViewComponent(DataContext dataContext)
		{
			this._dataContext = dataContext;
		}

		public async Task<IViewComponentResult> InvokeAsync() => View(await this._dataContext.Brands.ToListAsync());
	}
}
