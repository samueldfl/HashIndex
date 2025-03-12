namespace Domain.Utils;

public static class Statics
{
	public static int Collision { get; private set; } = 0;

	public static int NonCollision { get; private set; } = 0;

	public static int Overflow { get; private set; } = 0;

	public static void IncrementCollision()
	{
		Collision++;
	}

	public static void IncrementNonCollision()
	{
		NonCollision++;
	}

	public static void IncrementOverflow()
	{
		Overflow++;
	}

	public static void ResetAllStats()
	{
		Collision = 0;
		NonCollision = 0;
		Overflow = 0;
	}
}
