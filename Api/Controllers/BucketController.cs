using Api.Request;
using Domain.Bucket;
using Domain.Page;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BucketController(PageManager pageManager, BucketDictionary bucketDictionary)
	: ControllerBase
{
	private readonly PageManager _pageManager = pageManager;

	private readonly BucketDictionary _bucketDictionary = bucketDictionary;

	[HttpGet(Routes.BUCKETS_CALCULATE)]
	public async Task<IActionResult> GetBucketsCalculation(int FR)
	{
		try
		{
			var lines = await SystemIOFile.ReadAllLinesAsync(Routes.WORDS_PATH);
			var wordsCount = lines.SelectMany(line => line.Split('\n')).Count();
			return Ok(BucketDictionary.CalculateBuckets(wordsCount, FR));
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpGet(Routes.BUCKETS_SEARCH_BY_TARGET_PAGE)]
	public IActionResult GetBucketPages([FromRoute] string target)
	{
		var pagesIndexes = _bucketDictionary.GetPagesIndexesByKey(target);

		foreach (var index in pagesIndexes)
		{
			var page = _pageManager.GetPageByIndex(index);

			foreach (var word in page.Words)
			{
				if (word.Equals(target, StringComparison.Ordinal))
				{
					return Ok(page);
				}
			}
		}

		return NotFound();
	}

	[HttpPost(Routes.BUCKETS)]
	public IActionResult CreateBuckets([FromBody] CreateBucketsRequest request)
	{
		try
		{
			var pages = _pageManager.GetPages();
			_bucketDictionary.CreateBuckets(pages, request.NumOfBuckets);

			return Created();
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}
