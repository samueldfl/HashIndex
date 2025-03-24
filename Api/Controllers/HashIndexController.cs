using System.Diagnostics;
using Api.Request;
using Domain.Bucket;
using Domain.Page;
using Domain.Utils;
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

	[HttpGet(Routes.SEARCH_TARGET_WORD_PAGE)]
	public IActionResult SearchTargetWord([FromRoute] string target)
	{
		try
		{
			var key = Hash.Compute(target, _bucketDictionary.CalculateBuckets(_pageManager.Count));

			var searchBucketTimer = new Stopwatch();
			searchBucketTimer.Start();
			var bucketPageIndex = _bucketDictionary.Scan(key, target, out var bucketCost);
			searchBucketTimer.Stop();

			var searchPageTimer = new Stopwatch();
			searchPageTimer.Start();
			var tableScanPageIndex = _pageManager.TableScan(target, out var tableScanCost);
			searchPageTimer.Stop();

			if (bucketPageIndex == -1 || tableScanPageIndex == -1)
				return BadRequest("The target word was not found.");

			return Ok(
				new
				{
					BucketScan = new
					{
						pageIndex = bucketPageIndex,
						cost = bucketCost,
						time = searchBucketTimer.Elapsed.TotalNanoseconds / 1_000_000_000.0,
					},
					PageScan = new
					{
						pageIndex = tableScanPageIndex,
						cost = tableScanCost,
						time = searchPageTimer.Elapsed.TotalNanoseconds / 1_000_000_000.0,
					},
				}
			);
		}
		catch (Exception e)
		{
			return StatusCode(500, e.Message);
		}
	}

	[HttpPost(Routes.BUCKETS)]
	public IActionResult CreateBuckets([FromBody] SetSizeRequest request)
	{
		try
		{
			if (request.Size <= 0)
				return BadRequest("Invalid value for 'size'. It must be greater than zero.");

			var pages = _pageManager.GetPages();
			if (pages.Count <= 0)
				return BadRequest("No pages available to create buckets.");

			_bucketDictionary.Size = request.Size;
			_bucketDictionary.CreateBuckets(
				pages,
				_bucketDictionary.CalculateBuckets(_pageManager.Count)
			);

			var overflowTotal = Statics.Overflow + Statics.NonOverflow;
			var collisionTotal = Statics.Collision + Statics.NonCollision;

			var overflowRate = Math.Round((Statics.Overflow / (double)overflowTotal) * 100, 2);
			var collisionRate = Math.Round((Statics.Collision / (double)collisionTotal) * 100, 2);

			return Ok(new { overflow = overflowRate, collision = collisionRate });
		}
		catch (Exception e)
		{
			return StatusCode(500, e.Message);
		}
	}

	[HttpGet(Routes.PAGES)]
	public IActionResult GetPages([FromQuery] int? skip, [FromQuery] int take)
	{
		try
		{
			if (take <= 0 || skip < 0)
				return BadRequest("Invalid value for 'skip' and/or 'take'.");

			var pages = _pageManager.GetPages().Skip(skip ?? 0).Take(take);
			return Ok(pages);
		}
		catch (Exception e)
		{
			return StatusCode(500, e.Message);
		}
	}

	[HttpGet(Routes.PAGE)]
	public IActionResult GetPage([FromRoute] int index)
	{
		try
		{
			if (index < 0)
				return BadRequest("Index must be greater than or equal to zero.");

			var page = _pageManager.GetPageByIndex(index);
			return Ok(page);
		}
		catch (ArgumentOutOfRangeException)
		{
			return NotFound(
				$"No page found at index {index}. It exceeds the total number of pages ({_pageManager.Count})."
			);
		}
		catch (Exception e)
		{
			return StatusCode(500, e.Message);
		}
	}

	[HttpPost(Routes.PAGES)]
	public async Task<IActionResult> CreatePages([FromBody] SetSizeRequest request)
	{
		try
		{
			var lines = await SystemIOFile.ReadAllLinesAsync(Routes.WORDS_PATH);
			var words = lines.SelectMany(line => line.Split('\n')).ToArray();
			_pageManager.CreatePages(words, request.Size);

			return Created();
		}
		catch (Exception e)
		{
			return StatusCode(500, e.Message);
		}
	}

	[HttpDelete(Routes.CLEAR)]
	public IActionResult Clear()
	{
		try
		{
			Statics.ResetAllStats();
			_pageManager.Clear();
			_bucketDictionary.Clear();
			return NoContent();
		}
		catch (Exception e)
		{
			return StatusCode(500, e.Message);
		}
	}
}
