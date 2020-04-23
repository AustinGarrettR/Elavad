using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine.Factory;
using UnityEngine.AI;

namespace Engine.Player
{
    public class ServerPlayerManager : Manager
    {
        /// <summary>
        /// Called on initialization
        /// </summary>
        public override void Init()
        {
            agent = GameObject.Find("Test").AddComponent<NavMeshAgent>();
            target = GameObject.Find("Target");
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
            agent.destination = target.transform.position;
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {

        }

        private NavMeshAgent agent;
        private GameObject target;
    }
}