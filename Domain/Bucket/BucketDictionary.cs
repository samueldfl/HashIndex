using Domain.Utils;

namespace Domain.Bucket;

public class BucketDictionary(uint capacity)
{
	private readonly uint _capacity = capacity;

	private BucketDictionary? _overflowBucketDictionary;

	private readonly Dictionary<string, IEnumerable<int>> _bucketStorage = [];

	public IEnumerable<int> this[string key]
	{
		get
		{
			var bucket = _bucketStorage[key];
			return bucket;
		}
		set
		{
			if (_bucketStorage.ContainsKey(key))
			{
				_bucketStorage[key] = [.. value];
			}
			else
			{
				_bucketStorage.Add(key, [.. value]);
			}
		}
	}

	public ICollection<string> Keys => _bucketStorage.Keys;

	public ICollection<IEnumerable<int>> Values => _bucketStorage.Values;

	public int Count => _bucketStorage.Count;

	public void Add(string key, IEnumerable<int> values)
	{
		if (Count < _capacity)
		{
			if (_bucketStorage.TryGetValue(key, out var existingValues))
			{
				_bucketStorage[key] = [.. existingValues, .. values];
				return;
			}

			_bucketStorage.Add(key, values);
		}

		_overflowBucketDictionary ??= new BucketDictionary(_capacity);
		_overflowBucketDictionary.Add(key, values);
	}

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
