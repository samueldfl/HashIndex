namespace Domain.Page
{
	/// <summary>
	/// Gerencia a criação e pesquisa de páginas de palavras.
	/// </summary>
	public class PageManager
	{
		/// <summary>
		/// Lista que contém todas as páginas criadas.
		/// </summary>
		private readonly IList<PageModel> pages = [];

		/// <summary>
		/// Cria páginas a partir de um array de palavras, dividindo-o em páginas com o tamanho especificado.
		/// </summary>
		/// <param name="words">Array de palavras a serem divididas em páginas.</param>
		/// <param name="pageSize">Tamanho máximo de palavras por página.</param>
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

		/// <summary>
		/// Retorna a lista de páginas criadas.
		/// </summary>
		/// <returns>Lista de páginas.</returns>
		public IEnumerable<PageModel> GetPages()
		{
			return pages;
		}

		public PageModel GetPageByIndex(int index)
		{
			return pages[index];
		}

		/// <summary>
		/// Realiza uma busca em todas as páginas para encontrar uma palavra específica e retorna o índice da página onde foi encontrada.
		/// </summary>
		/// <param name="target">Palavra a ser buscada.</param>
		/// <returns>Índice da página onde a palavra foi encontrada, ou -1 se não encontrada.</returns>
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
}
