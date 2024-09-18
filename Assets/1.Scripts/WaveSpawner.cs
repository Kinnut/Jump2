using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;     // 4방향의 스폰 지점
    public string normalEnemyTag;       // ObjectPooler에서 사용할 기본 적 태그
    public string strongEnemyTag;       // ObjectPooler에서 사용할 강화된 적 태그
    public DynamicObjectPooler objectPooler;  // DynamicObjectPooler 참조

    public int startEnemiesPerWave = 10;   // 첫 웨이브에서 소환할 몬스터 수
    public float spawnInterval = 2f;       // 몬스터 간 소환 간격
    public float waveWaitTime = 10f;       // 다음 웨이브 대기 시간

    private int currentWave = 1;           // 현재 웨이브 번호
    private int enemiesRemaining;          // 남아있는 적 수
    private bool waveInProgress = false;   // 웨이브가 진행 중인지 여부

    void Start()
    {
        if (objectPooler == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Object Pooler가 연결되지 않았거나 스폰 포인트가 설정되지 않았습니다.");
            return;
        }

        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (enemiesRemaining <= 0 && !waveInProgress)
        {
            StartCoroutine(StartNextWave());
        }
    }

    // 웨이브 시작
    IEnumerator StartNextWave()
    {
        waveInProgress = true;

        int enemiesToSpawn = startEnemiesPerWave + (currentWave / 2) * 2;
        int strongEnemyCount = Mathf.CeilToInt(enemiesToSpawn * 0.1f);
        int normalEnemyCount = enemiesToSpawn - strongEnemyCount;

        enemiesRemaining = enemiesToSpawn;

        List<int> availableSpawnPoints = new List<int> { 0, 1, 2, 3 };

        while (normalEnemyCount > 0 || strongEnemyCount > 0)
        {
            List<int> spawnIndices = GetRandomSpawnIndices(availableSpawnPoints);

            // 첫 번째 스폰 포인트에서 적 소환
            if (normalEnemyCount > 0)
            {
                SpawnEnemy(normalEnemyTag, spawnIndices[0]);
                normalEnemyCount--;
            }
            else if (strongEnemyCount > 0)
            {
                SpawnEnemy(strongEnemyTag, spawnIndices[0]);
                strongEnemyCount--;
            }

            // 두 번째 스폰 포인트에서 적 소환
            if (normalEnemyCount > 0)
            {
                SpawnEnemy(normalEnemyTag, spawnIndices[1]);
                normalEnemyCount--;
            }
            else if (strongEnemyCount > 0)
            {
                SpawnEnemy(strongEnemyTag, spawnIndices[1]);
                strongEnemyCount--;
            }

            yield return new WaitForSeconds(spawnInterval);
        }


        while (enemiesRemaining > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(waveWaitTime);

        currentWave++;
        waveInProgress = false;
    }

    // 적 소환 메서드 (DynamicObjectPooler를 사용하여 적 소환)
    void SpawnEnemy(string enemyTag, int spawnIndex)
    {
        if (objectPooler == null || spawnPoints == null || spawnIndex < 0 || spawnIndex >= spawnPoints.Length)
        {
            Debug.LogError("Object Pooler가 연결되지 않았거나 잘못된 스폰 인덱스입니다.");
            return;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject enemyObject = objectPooler.SpawnFromPool(enemyTag, spawnPoint.position, Quaternion.identity);

        if (enemyObject == null)
        {
            Debug.LogError("적 생성 실패: " + enemyTag + " 풀에서 적을 가져오지 못했습니다.");
            return;
        }

        EnemyBase enemyBase = enemyObject.GetComponent<EnemyBase>();
        if (enemyBase == null)
        {
            Debug.LogError("EnemyBase 스크립트가 프리팹에 붙어 있지 않습니다. 태그: " + enemyTag);
            return;
        }

        enemyBase.OnEnemyDestroyed += OnEnemyDestroyed;
    }

    // 랜덤으로 두 개의 스폰 포인트 선택
    List<int> GetRandomSpawnIndices(List<int> availableSpawnPoints)
    {
        int firstSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int first = availableSpawnPoints[firstSpawnIndex];
        availableSpawnPoints.RemoveAt(firstSpawnIndex);

        int secondSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int second = availableSpawnPoints[secondSpawnIndex];

        availableSpawnPoints.Add(first); // 다시 리스트에 추가

        return new List<int> { first, second };
    }

    // 적 사망 시 호출되는 메서드
    void OnEnemyDestroyed()
    {
        enemiesRemaining--;
    }
}
