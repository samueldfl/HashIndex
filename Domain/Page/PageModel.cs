namespace Domain.Page;

public class PageModel
{
	public int Index { get; private set; }

	public string[] Words { get; private set; }

	public PageModel(int index, string[] words, int size)
	{
		Index = index;
		Words = new string[size];
		Array.Copy(words, 0, Words, 0, size);
	}
}
