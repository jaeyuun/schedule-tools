using GirderSchedule.Domain.Models;
using System.Collections.Generic;

namespace GirderSchedule.Application.Layout
{
	public static class GirderPaginator
	{
		public static List<GirderPageModel> Paginate(List<GirderSetModel> sets)
		{
			var pages = new List<GirderPageModel>();
			if (sets == null || sets.Count == 0)
			{
				pages.Add(new GirderPageModel { PageNumber = 1 });
				return pages;
			}

			var pageNumber = 1;
			for (var i = 0; i < sets.Count; i += GirderLayout.MaxSetPerPage)
			{
				var page = new GirderPageModel();
				page.PageNumber = pageNumber++;

				var end = i + GirderLayout.MaxSetPerPage;
				if (end > sets.Count)
				{
					end = sets.Count;
				}

				for (var j = i; j < end; j++)
				{
					page.Sets.Add(sets[j]);
				}

				pages.Add(page);
			}

			return pages;
		}
	}
}