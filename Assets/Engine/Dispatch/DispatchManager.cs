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
        /// <param name="parameters"></param>
        public override void init(params object[] parameters)
        {
            
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void shutdown()
        {

        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void update()
        {
            Dispatcher.Update();
        }
    }
}