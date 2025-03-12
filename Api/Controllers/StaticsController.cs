using Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StaticsController : ControllerBase
	{
		[HttpGet(Routes.COLLISION)]
		public IActionResult GetCollision()
		{
			return Ok(new { collision = Statics.Collision });
		}

		[HttpGet(Routes.OVERFLOW)]
		public IActionResult GetOverflow()
		{
			return Ok(new { overflow = Statics.Overflow });
		}
	}
}
