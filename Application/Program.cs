using Domain.Bucket;
using Domain.Page;

const string PATH = "/home/HashIndex/Application/words.txt";

var pageManager = new PageManager();
var bucketManager = new BucketManager(10);
if (File.Exists(PATH))
{
	try
	{
		var lines = File.ReadLines(PATH);
		var words = lines.SelectMany(line => line.Split('\n')).ToArray();

		pageManager.CreatePages(words, 100_000);
		Console.WriteLine(pageManager.TableScan("wild-looking"));
	}
	catch (Exception e)
	{
		Console.WriteLine(e.Message);
	}
}
else
{
	Console.WriteLine($"{PATH} não existe!");
}
