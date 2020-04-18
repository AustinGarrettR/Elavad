using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Utility
{
    /// <summary>
    /// Format class for strings
    /// </summary>
    public static class StringFormatter
    {
        /// <summary>
        /// Capitalizes the first letter of a string
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        public static string FirstCharToUpper(string s)
        {
            // Check for empty string.  
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.  
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}