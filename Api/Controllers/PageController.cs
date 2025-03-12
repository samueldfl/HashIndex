using Api.Request;
using Domain.Page;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;

namespace Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PageController(PageManager pageManager) : ControllerBase
	{
		private readonly PageManager _pageManager = pageManager;

		[HttpGet(Routes.PAGES)]
		public IActionResult GetPages()
		{
			return Ok(_pageManager.GetPages());
		}

		[HttpGet(Routes.PAGE)]
		public IActionResult GetPage([FromRoute] int index)
		{
			try
			{
				return Ok(_pageManager.GetPageByIndex(index));
			}
			catch (ArgumentOutOfRangeException)
			{
				return NotFound();
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}

		[HttpGet(Routes.PAGES_TABLE_SCAN)]
		public IActionResult GetWordTableScan([FromRoute] string target)
		{
			return Ok(_pageManager.TableScan(target));
		}

		[HttpPost(Routes.PAGES)]
		public async Task<IActionResult> CreatePages([FromBody] CreatePagesRequest request)
		{
			try
			{
				var lines = await SystemIOFile.ReadAllLinesAsync(Routes.WORDS_PATH);
				var words = lines.SelectMany(line => line.Split('\n')).ToArray();
				_pageManager.CreatePages(words, request.PageSize);

				return Created();
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}
	}
}
