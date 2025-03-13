using Api.Request;
using Domain.Bucket;
using Domain.Page;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HashIndexController(PageManager pageManager, BucketDictionary bucketDictionary)
	: ControllerBase
{
	private readonly PageManager _pageManager = pageManager;

	private readonly BucketDictionary _bucketDictionary = bucketDictionary;

	[HttpGet(Routes.BUCKETS_CALCULATE)]
	public async Task<IActionResult> GetBucketsCalculation()
	{
		try
		{
			if (_bucketDictionary.BucketSize > 0)
			{
				var lines = await SystemIOFile.ReadAllLinesAsync(Routes.WORDS_PATH);
				var wordsCount = lines.SelectMany(line => line.Split('\n')).Count();
				int numOfBuckets = _bucketDictionary.CalculateBuckets(wordsCount);
				return Ok(numOfBuckets);
			}
			return Unauthorized();
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpGet(Routes.SEARCH_TARGET_WORD_PAGE)]
	public IActionResult SearchTargetWord([FromRoute] string target)
	{
		try
		{
			var pages = _pageManager.GetPages();

			var bucketPageIndex = _bucketDictionary.Scan(pages, target, out int bucketCost);

			var tableScanPageIndex = _pageManager.TableScan(target, out int tableScanCost);

			return Ok(
				new
				{
					BucketScan = new { pageIndex = bucketPageIndex, cost = bucketCost },
					PageScan = new { pageIndex = tableScanPageIndex, cost = tableScanCost },
				}
			);
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpPost(Routes.BUCKETS)]
	public IActionResult CreateBuckets()
	{
		try
		{
			var pages = _pageManager.GetPages();

			if (pages.Count != 0 && _bucketDictionary.BucketSize > 0)
			{
				_bucketDictionary.CreateBuckets(
					pages,
					_bucketDictionary.CalculateBuckets(_pageManager.Count)
				);

				return Created();
			}

			return Unauthorized();
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpPost(Routes.BUCKETS_TUPLES)]
	public IActionResult SetTuplesCapacity([FromBody] SetBucketsSizeRequest request)
	{
		try
		{
			if (request.Size <= 0)
			{
				return BadRequest();
			}

			_bucketDictionary.BucketSize = request.Size;

			return Created();
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpGet(Routes.PAGES)]
	public IActionResult GetPages([FromQuery] int? skip, [FromQuery] int take)
	{
		var pages = _pageManager.GetPages().Skip(skip ?? default).Take(take);
		return Ok(pages);
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
