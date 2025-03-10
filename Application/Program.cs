using System.Diagnostics;
using Domain.Page;

const string PATH = "/root/HashIndex/Application/words.txt";

var pageManager = new PageManager();
var stopwatch = new Stopwatch();

if (File.Exists(PATH))
{
	try
	{
		var lines = File.ReadLines(PATH);
		var words = lines.SelectMany(line => line.Split('\n')).ToArray();

		pageManager.CreatePages(words, 100_000);
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
