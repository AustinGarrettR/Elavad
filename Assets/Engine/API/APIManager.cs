using Engine.Factory;
using System.Collections.Generic;

namespace Engine.API
{
    public class APIManager : Manager
    {
        public override void init(params object[] parameters)
        {
            List<Manager> managers = (List<Manager>) parameters[0];
            API.SetManagers(managers);
        }

        public override void shutdown()
        {
           
        }

        public override void update()
        {
            
        }
    }

}