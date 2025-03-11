namespace Domain.Page;

public class PageManager
{
	private readonly IList<PageModel> pages = [];

	public void CreatePages(string[] words, int pageSize)
	{
		int pageNumber = 0;

		for (int i = 0; i < words.Length; i += pageSize)
		{
			int currentPageSize = Math.Min(pageSize, words.Length - i);
			string[] pageWords = new string[currentPageSize];
			Array.Copy(words, i, pageWords, 0, currentPageSize);

			pages.Add(new PageModel(pageNumber, pageWords, currentPageSize));
			pageNumber++;
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

	public int CalculateBuckets(int FR)
	{
		int NR = pages.Sum(pagina => pagina.Words.Length);
		return NR / FR + 1;
	}

	public int TableScan(string target)
	{
		foreach (var page in pages)
		{
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
