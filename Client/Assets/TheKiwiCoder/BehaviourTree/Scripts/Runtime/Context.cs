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

        public AddressableResourceSystem resourceSystem;
		public ScriptParsingSystem scriptParsingSystem;

        public static Context CreateFromGameObject(GameObject gameObject) 
        {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.animator = gameObject.GetComponent<Animator>();
			// Add whatever else you need here...

			return context;
        }

        public void AddSystem(
            AddressableResourceSystem resourceSystem,
		    ScriptParsingSystem scriptParsingSystem)
        {
            this.resourceSystem = resourceSystem;
            this.scriptParsingSystem = scriptParsingSystem;
        }
    }
}