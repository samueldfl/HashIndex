namespace Api.Controllers;

public static class Routes
{
	public const string WORDS_PATH = @"/home/HashIndex/Api/words.txt";

	public const string PAGES = @"/pages";

	public const string PAGE = @"/pages/{index}";

	public const string PAGES_TABLE_SCAN = @"/pages/{target}/table-scan";

	public const string BUCKETS = @"/buckets";

	public const string BUCKETS_SEARCH_BY_TARGET_PAGE = @"/buckets/{target}/page";

	public const string BUCKETS_CALCULATE = @"/buckets/{FR}/calculate";

	public const string COLLISION = @"/collision";

	public const string OVERFLOW = @"/overflow";
}
