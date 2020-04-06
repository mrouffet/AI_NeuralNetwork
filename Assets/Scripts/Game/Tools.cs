using System;

public class Tools
{
	static Random mRandomizer = new Random();

	public static int Rand(int min, int max)
	{
		lock (mRandomizer)
			return mRandomizer.Next(min, max);
	}
	public static uint Rand(uint min, uint max)
	{
		lock (mRandomizer)
			return (uint)mRandomizer.Next((int)min, (int)max);
	}
	public static float Rand(float min, float max)
	{
		lock (mRandomizer)
			return (float)mRandomizer.NextDouble() * (max - min) + min;
	}

	public static void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp = lhs;
		lhs = rhs;
		rhs = temp;
	}
}