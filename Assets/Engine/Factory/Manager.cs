using System.Threading.Tasks;
using Engine.Loading;

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
        public abstract void Init();

        /// <summary>
        /// Called every frame
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// Virtual void that is called when the game is loaded.
        /// </summary>
        public virtual async Task LoadGameTask(ClientLoadData loadData) { 
            await Task.CompletedTask; 
        }
    }
}