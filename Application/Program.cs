using System.Diagnostics;
using Domain.Bucket;
using Domain.Page;
using Domain.Utils;

const string PATH = "/home/HashIndex/Application/words.txt";

var pageManager = new PageManager();
var bucketDictionary = new BucketDictionary(1000); // capacidade de tuplas
var stopwatch = new Stopwatch();

if (File.Exists(PATH))
{
	try
	{
		var lines = File.ReadLines(PATH);
		var words = lines.SelectMany(line => line.Split('\n')).ToArray();

		pageManager.CreatePages(words, 1);
		var numOfBuckets = pageManager.CalculateBuckets(1000); // calculo de baldes por capacidade de tuplas
		BucketManager.CreateBuckets(pageManager.GetPages(), numOfBuckets, bucketDictionary);
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
