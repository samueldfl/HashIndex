namespace Domain.Page;

public class PageModel(int index, int size)
{
	public int Index { get; private set; } = index;

	public string[] Words { get; set; } = new string[size];
}
