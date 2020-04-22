using Engine.Factory;

namespace Engine.Dispatch
{
    /// <summary>
    /// Manages the dispatcher
    /// </summary>
    public class DispatchManager : Manager
    {

        /*
         * Override Functions
         */

        /// <summary>
        /// Called upon initialization
        /// </summary>
        public override void Init()
        {
            
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {

        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
            Dispatcher.Update();
        }
    }
}