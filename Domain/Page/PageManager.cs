namespace Domain.Page;

public class PageManager
{
	private readonly IList<PageModel> pages = [];

	public int Count => pages.Sum(page => page.Words.Length);

	public void CreatePages(string[] words, int pageSize)
	{
		int pageNumber = 0;

		for (int i = 0; i < words.Length; i += pageSize)
		{
			int currentPageSize = Math.Min(pageSize, words.Length - i);

			pages.Add(new PageModel(pageNumber, currentPageSize));
			pageNumber++;
		}

		for (int i = 0; i < pages.Count; i++)
		{
			var page = pages[i];
			int index = i * pageSize;
			int length = Math.Min(pageSize, words.Length - index);

			string[] pageWords = new string[length];
			Array.Copy(words, index, pageWords, 0, length);

			page.Words = pageWords;
		}
	}

	public IList<PageModel> GetPages()
	{
		return pages;
	}

	public PageModel GetPageByIndex(int index)
	{
		return pages[index];
	}

	public int TableScan(string target, out int cost)
	{
		cost = 0;
		foreach (var page in pages)
		{
			cost++;
			foreach (var word in page.Words)
			{
				if (word.Equals(target, StringComparison.Ordinal))
				{
					return page.Index;
				}
			}
		}

		return -1;
	}
}
