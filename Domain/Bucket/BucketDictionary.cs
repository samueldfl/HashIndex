using Domain.Page;
using Domain.Utils;

namespace Domain.Bucket;

public class BucketDictionary
{
	public int BucketSize { get; set; } = 0;

	private BucketDictionary? _overflowBucketDictionary;

	private readonly Dictionary<string, HashSet<int>> _bucketStorage = [];

	public HashSet<int> this[string key]
	{
		get => _bucketStorage[key];
		set
		{
			if (_bucketStorage.TryGetValue(key, out var existingValues))
			{
				Statics.IncrementCollision();

				if (existingValues.Count < BucketSize)
				{
					_bucketStorage[key] = [.. existingValues, .. value];
				}
				else
				{
					_overflowBucketDictionary ??= new BucketDictionary();
					_overflowBucketDictionary.BucketSize = BucketSize;
					_overflowBucketDictionary[key] = value;

					Statics.IncrementOverflow();
				}
			}
			else
			{
				_bucketStorage.Add(key, value);

				Statics.IncrementNonCollision();
			}
		}
	}

	public IEnumerable<int> GetBucketPages(string target)
	{
		string key = Hash.Compute(target, _bucketStorage.Count);
		var pages = _bucketStorage[key];

		return pages;
	}

	public void CreateBuckets(IList<PageModel> pages, int numOfBuckets)
	{
		foreach (var page in pages)
		{
			foreach (var word in page.Words)
			{
				var key = Hash.Compute(word, numOfBuckets);
				this[key] = [page.Index];
			}
		}
	}

	public HashSet<int> GetPagesIndexesByKey(string target)
	{
		string key = Hash.Compute(target, _bucketStorage.Count);

		if (_bucketStorage.TryGetValue(key, out var values))
		{
			return values;
		}

		if (_overflowBucketDictionary is not null)
		{
			return _overflowBucketDictionary.GetPagesIndexesByKey(target);
		}

		return [];
	}

	public int Scan(IList<PageModel> pages, string target, out int cost)
	{
		cost = 0;
		var pagesIndexes = GetPagesIndexesByKey(target);

		if (pagesIndexes.Count != 0)
		{
			foreach (var index in pagesIndexes)
			{
				cost++;
				var page = pages[index];

				foreach (var word in page.Words)
				{
					if (word.Equals(target, StringComparison.Ordinal))
					{
						return page.Index;
					}
				}
			}
		}

		return -1;
	}

	public int CalculateBuckets(int NR)
	{
		return NR / BucketSize + 1;
	}
}
