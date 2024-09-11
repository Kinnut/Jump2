using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;           // 몬스터 태그
        public GameObject prefab;    // 몬스터 프리팹
        public int size;             // 초기 풀 크기
    }

    public List<Pool> pools;
    public Dictionary<string, List<GameObject>> poolDictionary;  // 풀에 오브젝트 관리

    void Start()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            poolDictionary[pool.tag] = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);  // 처음엔 비활성화
                poolDictionary[pool.tag].Add(obj);  // 리스트에 추가
            }
        }
    }

    // 오브젝트 풀에서 오브젝트를 요청하는 함수
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // 비활성화된 오브젝트를 찾아서 재사용
        GameObject objectToSpawn = poolDictionary[tag].Find(obj => !obj.activeInHierarchy);

        // 활성화 가능한 오브젝트가 없다면 새로 생성해서 추가
        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(poolDictionary[tag][0]);
            poolDictionary[tag].Add(objectToSpawn);  // 새로 생성한 오브젝트를 풀에 추가
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }
}
