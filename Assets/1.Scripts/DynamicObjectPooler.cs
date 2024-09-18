using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;   // �ʱ� Ǯ ������
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // �ʱ� Ǯ ũ�⸸ŭ ������Ʈ�� �����Ͽ� ť�� �߰�
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
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];

        // Ǯ�� ����� ������Ʈ�� ������ ���� ���� �� Ǯ�� �߰�
        if (pool.Count == 0)
        {
            Debug.Log("Pool empty, instantiating new object");
            GameObject obj = Instantiate(pools.Find(p => p.tag == tag).prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject objectToSpawn = pool.Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        pool.Enqueue(objectToSpawn);  // ������Ʈ�� �ٽ� Ǯ�� ��ȯ

        return objectToSpawn;
    }
}
