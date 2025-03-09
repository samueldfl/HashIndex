namespace Domain.Bucket;

public class BucketModel(string key)
{
	public string Key { get; private set; } = key;

	public IList<Tuple> Tuples { get; private set; } = [];
}
