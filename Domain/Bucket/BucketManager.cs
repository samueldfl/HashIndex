using Domain.Page;
using Domain.Utils;

namespace Domain.Bucket;

public static class BucketManager
{
	public static void CreateBuckets(
		IList<PageModel> pages,
		int numOfBuckets,
		BucketDictionary bucketDictionary
	)
	{
		foreach (var page in pages)
		{
			foreach (var word in page.Words)
			{
				var key = Hash.Compute(word, numOfBuckets);
				bucketDictionary[key] = [page.Index];
			}
		}
	}
}
