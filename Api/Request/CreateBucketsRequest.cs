using Domain.Page;

namespace Api.Request;

public record CreateBucketsRequest
{
	public int NumOfBuckets { get; init; }
}
