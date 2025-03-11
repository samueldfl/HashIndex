using Domain.Page;
using Domain.Utils;

namespace Domain.Bucket;

public class BucketDictionary(int tuplesCapacity)
{
	private readonly int _tuplesCapacity = tuplesCapacity;

	private BucketDictionary? _overflowBucketDictionary;

	private readonly Dictionary<string, IEnumerable<int>> _bucketStorage = [];

	public IEnumerable<int> this[string key]
	{
		get => _bucketStorage[key];
		set
		{
			if (_bucketStorage.TryGetValue(key, out IEnumerable<int>? existingValues))
			{
				if (existingValues.Count() < _tuplesCapacity)
				{
					_bucketStorage[key] = [.. existingValues, .. value];
				}
				else
				{
					_overflowBucketDictionary ??= new BucketDictionary(_tuplesCapacity);
					_overflowBucketDictionary[key] = value;
				}
			}
			else
			{
				_bucketStorage.Add(key, value);
			}
		}
	}

	public ICollection<string> Keys => _bucketStorage.Keys;

	public ICollection<IEnumerable<int>> Values => _bucketStorage.Values;

	public int Count => _bucketStorage.Count;

	public IEnumerable<int> GetBucketPages(string target)
	{
		string key = Hash.Compute(target, _bucketStorage.Count);
		var pages = _bucketStorage[key];

		return pages;
	}

	public IEnumerable<int> GetPagesIndexesByKey(string target)
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
}
