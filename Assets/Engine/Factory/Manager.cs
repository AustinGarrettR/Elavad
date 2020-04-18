namespace Engine.Factory
{
    /// <summary>
    /// Base class for managers
    /// </summary>
    public abstract class Manager
    {
        /// <summary>
        /// Called on initialization
        /// </summary>
        /// <param name="parameters">Variable parameters</param>
        public abstract void init(params System.Object[] parameters);

        /// <summary>
        /// Called every frame
        /// </summary>
        public abstract void update();

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public abstract void shutdown();
    }
}