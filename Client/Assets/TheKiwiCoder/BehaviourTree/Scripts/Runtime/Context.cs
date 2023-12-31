using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context 
    {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator;
        public EntityBehaviour entityComponent;

        public AddressableResourceSystem resourceSystem;
		public ScriptParsingSystem scriptParsingSystem;
        public EntitySystem entitySystem;
        public CommandSystem commandSystem;
        // Add other game specific systems here

        public static Context CreateFromGameObject(GameObject gameObject) 
        {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.animator = gameObject.GetComponent<Animator>();
            context.entityComponent = gameObject.GetComponent<EntityBehaviour>();
			// Add whatever else you need here...

			return context;
        }

        public void AddSystem(
            AddressableResourceSystem resourceSystem, 
		    CommandSystem commandSystem, 
		    EntitySystem entitySystem,
		    ScriptParsingSystem scriptParsingSystem)
        {
            this.resourceSystem = resourceSystem;
            this.commandSystem = commandSystem;
            this.entitySystem = entitySystem;
            this.scriptParsingSystem = scriptParsingSystem;
        }
    }
}