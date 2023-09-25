using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MonoBehaviourTree))]
    public class MBTExecutor : MonoBehaviour
    {
        public bool isEnable = true;
        public MonoBehaviourTree monoBehaviourTree;

        void Reset()
        {
            monoBehaviourTree = GetComponent<MonoBehaviourTree>();
            OnValidate();
        }

        private void Awake()
        {
            if (monoBehaviourTree == null)
            {
                monoBehaviourTree = gameObject.GetComponent<MonoBehaviourTree>();
            }
        }

        private void Update()
        {
            if(isEnable)
            {
                monoBehaviourTree.Tick();
            }
        }

        void OnValidate()
        {
            if (monoBehaviourTree != null && monoBehaviourTree.parent != null)
            {
                monoBehaviourTree = null;
                Debug.LogWarning("Subtree should not be target of update. Select parent tree instead.", this.gameObject);
            }
        }
    }
}
