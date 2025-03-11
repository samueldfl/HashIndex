using Domain.Bucket;
using Domain.Page;

namespace Domain.Utils;

public static class Hash
{
	public static string Compute(string key, int numBuckets)
	{
		int hash = 0;
		foreach (char c in key)
		{
			hash = (hash * 31 + c) % numBuckets;
		}
		return Math.Abs(hash).ToString();
	}
}
