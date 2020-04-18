namespace Engine.Networking
{
    /// <summary>
    /// The reliability scheme enum
    /// </summary>
    public enum ReliabilityScheme
    {
        /// <summary>
        /// UDP data sent unreliably
        /// </summary>
        UNRELIABLE,

        /// <summary>
        /// UDP data send reliably (like TCP)
        /// </summary>
        RELIABLE
    }
}
