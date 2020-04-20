﻿using System.Threading.Tasks;

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
        public abstract void Init(params System.Object[] parameters);

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
        public virtual async Task LoadGameTask() { 
            await Task.CompletedTask; 
        }
    }
}