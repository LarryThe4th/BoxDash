using UnityEngine;
using System.Collections.Generic;

namespace BoxDash.Utility {
    // All the pooling object should inherit form this base class
    public abstract class PoolObject : MonoBehaviour
    {
        public virtual void EnableObject(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public virtual void InitPoolObject() { }

        public abstract void OnObjectReuse(params object[] options);
    }

    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private Dictionary<string, int> m_Pools = new Dictionary<string, int>();

        // Store all the pooling object in this dictionary.
        private Dictionary<int, Queue<ObjectInstance>> m_PoolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

        // Create a new object pool
        public bool CreaterPool(string poolName, PoolObject prefab, Transform parent, int poolSize) {
            if (m_Pools.ContainsKey(poolName)) return false;
            // Use the prefab instance id as the dictionary key.
            int poolKey = prefab.GetInstanceID();
            // If the key is not exist in the dictionary
            if (!m_PoolDictionary.ContainsKey(poolKey)) {
               // Add a new pool
               m_PoolDictionary.Add(poolKey, new Queue<ObjectInstance>());
               for (int index = 0; index < poolSize; index++)
                {
                    // Create new object
                    GameObject gameObject = Instantiate(prefab.gameObject) as GameObject;

                    // Set its parent transform
                    gameObject.transform.SetParent(parent, false);

                    ObjectInstance newObject = new ObjectInstance(gameObject.GetComponent<PoolObject>());

                    // Add the object into the pool.
                    m_PoolDictionary[poolKey].Enqueue(newObject);
                }
                m_Pools.Add(poolName, poolKey);
            }
            return false;
        }

        public ObjectInstance ReuseObject(string poolName, Vector3 position, Quaternion rotation, params object[] options) {
            if (m_Pools.ContainsKey(poolName)) {
                int key = m_Pools[poolName];
                // Check if key exist.
                if (m_PoolDictionary.ContainsKey(key))
                {
                    // Get the last object in the queue
                    ObjectInstance reuseObject = m_PoolDictionary[key].Dequeue();
                    // Resign it into the queue.
                    m_PoolDictionary[key].Enqueue(reuseObject);
                    // Call the reuse method.
                    reuseObject.Reuse(position, rotation);

                    return reuseObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Request object reuse without reset its position.
        /// </summary>
        /// <param name="poolKey">The instance ID of the object prefab.</param>
        /// <param name="options">paramets</param>
        public ObjectInstance ReuseObject(int poolKey, params object[] options)
        {
            // Check if key exist.
            if (m_PoolDictionary.ContainsKey(poolKey))
            {
                // Get the last object in the queue
                ObjectInstance reuseObject = m_PoolDictionary[poolKey].Dequeue();
                // Resign it into the queue.
                m_PoolDictionary[poolKey].Enqueue(reuseObject);
                // Call the reuse method.
                reuseObject.Reuse(options);

                return reuseObject;
            }
            return null;
        }

        /// <summary>
        /// Request object reuse.
        /// </summary>
        /// <param name="poolKey">The instance ID of the object prefab.</param>
        /// <param name="position">The reset position</param>
        /// <param name="rotation">The reset rotation</param>
        /// <param name="options">paramets</param>
        public ObjectInstance ReuseObject(int poolKey, Vector3 position, Quaternion rotation, params object[] options)
        {
            // Check if key exist.
            if (m_PoolDictionary.ContainsKey(poolKey))
            {
                // Get the last object in the queue
                ObjectInstance reuseObject = m_PoolDictionary[poolKey].Dequeue();
                // Resign it into the queue.
                m_PoolDictionary[poolKey].Enqueue(reuseObject);
                // Call the reuse method.
                reuseObject.Reuse(position, rotation, options);

                return reuseObject;
            }
            return null;
        }
    }

    public class ObjectInstance {
        // The instance of pooling object
        public PoolObject Instance;

        // The transform of the pooling game object
        private Transform m_PoolObjectTransform;

        // The constructor of the object instance 
        public ObjectInstance(PoolObject poolObjectInstance) {
            Instance = poolObjectInstance;
            // Initialize the new object
            Instance.InitPoolObject();
            m_PoolObjectTransform = Instance.transform;
            Instance.EnableObject(false);
        }

        public void Reuse(Vector3 position, Quaternion rotation)
        {
            // Reset the object's location
            m_PoolObjectTransform.position = position;
            m_PoolObjectTransform.rotation = rotation;

            // Show the object
            Instance.EnableObject(true);

            // Reuse the object
            Instance.OnObjectReuse();
        }

        public void Reuse(Vector3 position, Quaternion rotation, params object[] options) {
            // Reset the object's location
            m_PoolObjectTransform.position = position;
            m_PoolObjectTransform.rotation = rotation;

            // Show the object
            Instance.EnableObject(true);

            // Reuse the object
            Instance.OnObjectReuse(options);
        }

        public void Reuse(params object[] options)
        {
            // Show the object
            Instance.EnableObject(true);

            // Reuse the object
            Instance.OnObjectReuse(options);
        }
    }
}