using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;           // ���� �±�
        public GameObject prefab;    // ���� ������
        public int size;             // �ʱ� Ǯ ũ��
    }

    public List<Pool> pools;
    public Dictionary<string, List<GameObject>> poolDictionary;  // Ǯ�� ������Ʈ ����

    void Start()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            poolDictionary[pool.tag] = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);  // ó���� ��Ȱ��ȭ
                poolDictionary[pool.tag].Add(obj);  // ����Ʈ�� �߰�
            }
        }
    }

    // ������Ʈ Ǯ���� ������Ʈ�� ��û�ϴ� �Լ�
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // ��Ȱ��ȭ�� ������Ʈ�� ã�Ƽ� ����
        GameObject objectToSpawn = poolDictionary[tag].Find(obj => !obj.activeInHierarchy);

        // Ȱ��ȭ ������ ������Ʈ�� ���ٸ� ���� �����ؼ� �߰�
        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(poolDictionary[tag][0]);
            poolDictionary[tag].Add(objectToSpawn);  // ���� ������ ������Ʈ�� Ǯ�� �߰�
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }
}
