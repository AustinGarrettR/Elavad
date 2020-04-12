using System;

namespace Engine.Utility
{
	public class TimeHandler
	{
		public static long getTimeInMilliseconds()
		{
			return DateTime.Now.ToUniversalTime().Ticks / 10000L;
		}
		public static int getMinute()
		{
			return DateTime.Now.Minute;
		}
	}
}