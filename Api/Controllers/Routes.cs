namespace Api.Controllers;

public static class Routes
{
	public const string WORDS_PATH = @"/home/HashIndex/Api/words.txt";

	public const string PAGES = @"/pages";

	public const string PAGE = @"/pages/{index}";

	public const string PAGES_TABLE_SCAN = @"/pages/{target}/table-scan";

	public const string BUCKETS = @"/buckets";

	public const string BUCKETS_TUPLES = @"/buckets/tuples";

	public const string BUCKETS_CALCULATE = @"/buckets/calculate";

	public const string SEARCH_TARGET_WORD_PAGE = @"/search/{target}/page";

	public const string COLLISION = @"/collision";

	public const string OVERFLOW = @"/overflow";
}
