namespace Domain.Utils;

public static class Hash
{
	public static string Compute(string key, int numBuckets)
	{
		var hash = key.Aggregate(0, (current, c) => (current * 31 + c) % numBuckets);
		return Math.Abs(hash).ToString();
	}
}
