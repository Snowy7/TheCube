using UnityEngine;
using System.Collections.Generic;

namespace New.Utils
{
// Object pooling for better performance
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }
        
        [SerializeField] private List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;
        
        private static ObjectPool instance;
        public static ObjectPool Instance { get { return instance; } }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            
            // Create pools
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                
                poolDictionary.Add(pool.tag, objectPool);
            }
        }
        
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }
            
            // Get object from pool
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            
            // If all objects are in use, expand the pool
            if (objectToSpawn.activeInHierarchy)
            {
                foreach (Pool pool in pools)
                {
                    if (pool.tag == tag)
                    {
                        objectToSpawn = Instantiate(pool.prefab);
                        break;
                    }
                }
            }
            
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            
            // Add back to the queue
            poolDictionary[tag].Enqueue(objectToSpawn);
            
            // Reset any IPoolable objects
            IPoolable poolableObject = objectToSpawn.GetComponent<IPoolable>();
            if (poolableObject != null)
            {
                poolableObject.OnObjectSpawn();
            }
            
            return objectToSpawn;
        }
        
        public interface IPoolable
        {
            void OnObjectSpawn();
        }
    }
}