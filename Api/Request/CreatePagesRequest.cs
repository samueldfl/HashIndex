namespace Api.Request;

public record CreatePagesRequest
{
	public int PageSize { get; init; }
}
