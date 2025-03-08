using System.Text;

namespace Domain.Page;

public class PageManager
{
    private readonly List<PageModel> pages = [];

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

    public List<PageModel> GetPages()
    {
        return pages;
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

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var page in pages)
        {
            sb.Append("Page ").Append(page.Index).Append(": ");
            sb.Append(string.Join(", ", page.Words));
            sb.Append('\n');
        }

        return sb.ToString();
    }
}

