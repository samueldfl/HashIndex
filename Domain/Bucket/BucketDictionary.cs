using Domain.Page;
using Domain.Utils;

namespace Domain.Bucket;

public class BucketDictionary
{
    public int Size { get; set; } = 0;

    private BucketDictionary? _overflowBucketDictionary;

    private readonly Dictionary<string, Dictionary<string, int>> _bucketStorage = [];

    public Dictionary<string, int> this[string key]
    {
        get => _bucketStorage[key];
        set
        {
            if (_bucketStorage.TryGetValue(key, out var tuples))
            {
                Statics.IncrementCollision();

                foreach (var kvp in value.Where(kvp => !tuples.ContainsKey(kvp.Key)))
                {
                    if (tuples.Count < Size)
                    {
                        tuples[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        _overflowBucketDictionary ??= new BucketDictionary();
                        _overflowBucketDictionary.Size = Size;
                        _overflowBucketDictionary[key] = value;

                        Statics.IncrementOverflow();
                    }
                }
            }
            else
            {
                _bucketStorage.Add(key, value);
                Statics.IncrementNonCollision();
            }
        }
    }

    public Dictionary<string, int> GetBucketPages(string target)
    {
        var key = Hash.Compute(target, _bucketStorage.Count);
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
                this[key] = new Dictionary<string, int> { { word, page.Index } };
            }
        }
    }

    public Dictionary<string, int> GetPagesIndexesByKey(string target)
    {
        var key = Hash.Compute(target, _bucketStorage.Count);

        if (_bucketStorage.TryGetValue(key, out var values))
        {
            return values;
        }

        return _overflowBucketDictionary is not null ? _overflowBucketDictionary.GetPagesIndexesByKey(target) : [];
    }

    public int Scan(string target, int numOfBuckets, out int cost)
    {
        cost = 0;

        var key = Hash.Compute(target, numOfBuckets);

        if (!_bucketStorage.TryGetValue(key, out var tuples)) return -1;

        if (!tuples.TryGetValue(target, out var value)) return -1;

        cost++;

        return value;
    }

    public int CalculateBuckets(int nr)
    {
        return nr / Size + 1;
    }
}