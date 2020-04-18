using System;

namespace Engine.Utility
{
	/// <summary>
	/// Utility methods for time
	/// </summary>
	public class TimeHandler
	{
		/// <summary>
		/// Returns the current system time in milliseconds
		/// </summary>
		/// <returns></returns>
		public static long getTimeInMilliseconds()
		{
			return DateTime.Now.ToUniversalTime().Ticks / 10000L;
		}

		/// <summary>
		/// Returns the current time in minute format
		/// </summary>
		/// <returns></returns>
		public static int getMinute()
		{
			return DateTime.Now.Minute;
		}
	}
}