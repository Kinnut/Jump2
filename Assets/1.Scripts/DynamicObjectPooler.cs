using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;   // 초기 풀 사이즈
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
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
            Debug.LogError("풀에 해당 태그가 존재하지 않습니다: " + tag);
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];

        if (pool == null)
        {
            Debug.LogError("오브젝트 풀이 null입니다: " + tag);
            return null;
        }

        if (pool.Count == 0)
        {
            Pool matchingPool = pools.Find(p => p.tag == tag);

            if (matchingPool == null || matchingPool.prefab == null)
            {
                Debug.LogError("해당 태그에 맞는 풀 또는 프리팹이 없습니다: " + tag);
                return null;
            }

            GameObject obj = Instantiate(matchingPool.prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        pool.Enqueue(objectToSpawn);  // 오브젝트를 다시 풀에 반환

        return objectToSpawn;
    }
}
