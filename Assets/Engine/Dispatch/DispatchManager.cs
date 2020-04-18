using Engine.Factory;

namespace Engine.Dispatch
{
    public class DispatchManager : Manager
    {

        /*
         * Override Functions
         */

        //Called upon initialization
        public override void init(params object[] parameters)
        {
            
        }

        //On shutdown
        public override void shutdown()
        {

        }

        //On update
        public override void update()
        {
            Dispatcher.Update();
        }
    }
}