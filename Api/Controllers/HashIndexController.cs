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

			var overflowRate = Math.Round(
				(Statics.Overflow / (double)(Statics.Overflow + Statics.NonOverflow)) * 100,
				2
			);

			var collisionRate = Math.Round(
				(Statics.Collision / (double)(Statics.Collision + Statics.NonCollision)) * 100,
				2
			);

			var searchPageTimer = new Stopwatch();
			searchPageTimer.Start();
			var tableScanPageIndex = _pageManager.TableScan(target, out var tableScanCost);
			searchPageTimer.Stop();

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
			return BadRequest(e.Message);
		}
	}

	[HttpPost(Routes.BUCKETS)]
	public IActionResult CreateBuckets([FromBody] SetSizeRequest request)
	{
		try
		{
			if (request.Size <= 0)
				return BadRequest();

			var pages = _pageManager.GetPages();
			if (pages.Count <= 0)
				return BadRequest();

			_bucketDictionary.Size = request.Size;
			_bucketDictionary.CreateBuckets(
				pages,
				_bucketDictionary.CalculateBuckets(_pageManager.Count)
			);

			return Ok(new { overflow = Statics.Overflow, collision = Statics.Collision });
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpGet(Routes.PAGES)]
	public IActionResult GetPages([FromQuery] int? skip, [FromQuery] int take)
	{
		var pages = _pageManager.GetPages().Skip(skip ?? 0).Take(take);
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
			return BadRequest(e.Message);
		}
	}

	[HttpDelete(Routes.CLEAR)]
	public IActionResult Clear()
	{
		Statics.ResetAllStats();
		_pageManager.Clear();
		_bucketDictionary.Clear();
		return NoContent();
	}
}
