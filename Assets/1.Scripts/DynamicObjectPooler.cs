using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

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
            Debug.LogError("Ǯ�� �ش� �±װ� �������� �ʽ��ϴ�: " + tag);
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];

        if (pool.Count == 0)
        {
            Debug.LogWarning("Ǯ�� ���� ��ü�� ���� ���ο� ��ü�� �����մϴ�: " + tag);
            Pool matchingPool = pools.Find(p => p.tag == tag);

            if (matchingPool == null || matchingPool.prefab == null)
            {
                Debug.LogError("�ش� �±׿� �´� Ǯ �Ǵ� �������� �����ϴ�: " + tag);
                return null;
            }

            GameObject obj = Instantiate(matchingPool.prefab, position, rotation);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        pool.Enqueue(objectToSpawn); // Ǯ�� �ٽ� ��ȯ

        return objectToSpawn;
    }
}
