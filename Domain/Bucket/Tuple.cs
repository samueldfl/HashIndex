namespace Domain.Bucket;

public class Tuple(string word, int pageIndex)
{
	public string Word { get; private set; } = word;

	public int PageIndex { get; private set; } = pageIndex;
}
