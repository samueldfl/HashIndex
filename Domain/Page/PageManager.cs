namespace Domain.Page;

public class PageManager
{
	private readonly IList<PageModel> _pages = [];

	public int Count => _pages.Sum(page => page.Words.Length);

	public void CreatePages(string[] words, int pageSize)
	{
		var pageNumber = 0;

		for (var i = 0; i < words.Length; i += pageSize)
		{
			var currentPageSize = Math.Min(pageSize, words.Length - i);

			_pages.Add(new PageModel(pageNumber, currentPageSize));
			pageNumber++;
		}

		for (var i = 0; i < _pages.Count; i++)
		{
			var page = _pages[i];
			var index = i * pageSize;
			var length = Math.Min(pageSize, words.Length - index);

			var pageWords = new string[length];
			Array.Copy(words, index, pageWords, 0, length);

			page.Words = pageWords;
		}
	}

	public IList<PageModel> GetPages()
	{
		return _pages;
	}

	public PageModel GetPageByIndex(int index)
	{
		return _pages[index];
	}

	public int TableScan(string target, out int cost)
	{
		cost = 0;
		foreach (var page in _pages)
		{
			cost++;
			if (page.Words.Any(word => word.Equals(target, StringComparison.Ordinal)))
			{
				return page.Index;
			}
		}

		return -1;
	}

	public void Clear() => _pages.Clear();
}
