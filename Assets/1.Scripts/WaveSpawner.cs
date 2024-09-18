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
        StartCoroutine(StartNextWave());
        Debug.Log("게임시작");
    }

    void Update()
    {
        // 남은 적이 없고 웨이브가 진행 중이 아닌 상태에서 다음 웨이브 시작
        if (enemiesRemaining <= 0 && !waveInProgress)
        {
            StartCoroutine(StartNextWave());
        }
    }

    // 웨이브 시작
    IEnumerator StartNextWave()
    {
        Debug.Log("웨이브시작");
        waveInProgress = true;

        int enemiesToSpawn = startEnemiesPerWave + (currentWave / 2) * 2;  // 2웨이브마다 2마리 추가
        int strongEnemyCount = Mathf.CeilToInt(enemiesToSpawn * 0.1f);     // 20%는 강화된 몬스터
        int normalEnemyCount = enemiesToSpawn - strongEnemyCount;

        enemiesRemaining = enemiesToSpawn;
        List<int> availableSpawnPoints = new List<int> { 0, 1, 2, 3 };  // 스폰 지점 리스트

        while (normalEnemyCount > 0 || strongEnemyCount > 0)
        {
            Debug.Log("while문 시작");
            // 두 개의 랜덤 스폰 포인트 선택 (동시에 두 마리 소환 X)
            List<int> spawnIndices = GetRandomSpawnIndices(availableSpawnPoints);
            Debug.Log("오류1");
            // 첫 번째 스폰 포인트에 몬스터 소환
            if (normalEnemyCount > 0)
            {
                SpawnEnemy(normalEnemyTag, spawnIndices[0]);  // 풀에서 기본 몬스터 소환
                normalEnemyCount--;
            }
            else if (strongEnemyCount > 0)
            {
                SpawnEnemy(strongEnemyTag, spawnIndices[0]);  // 풀에서 강화 몬스터 소환
                strongEnemyCount--;
            }

            // 두 번째 스폰 포인트에 몬스터 소환
            if (normalEnemyCount > 0)
            {
                SpawnEnemy(normalEnemyTag, spawnIndices[1]);  // 풀에서 기본 몬스터 소환
                normalEnemyCount--;
            }
            else if (strongEnemyCount > 0)
            {
                SpawnEnemy(strongEnemyTag, spawnIndices[1]);  // 풀에서 강화 몬스터 소환
                strongEnemyCount--;
            }

            yield return new WaitForSeconds(spawnInterval);  // 소환 간격 대기
        }

        // 몬스터가 모두 소환된 후, 전부 처치될 때까지 대기
        while (enemiesRemaining > 0)
        {
            yield return null;  // 계속 대기
        }

        // 다음 웨이브 시작 전 대기 시간
        yield return new WaitForSeconds(waveWaitTime);

        currentWave++;  // 웨이브 증가
        waveInProgress = false;  // 다음 웨이브 진행 가능 상태
    }

    // 적 소환 메서드 (DynamicObjectPooler를 사용하여 적 소환)
    void SpawnEnemy(string enemyTag, int spawnIndex)
    {
        Debug.Log("오류2");
        Transform spawnPoint = spawnPoints[spawnIndex];

        // ObjectPooler에서 적 소환
        GameObject enemyObject = objectPooler.SpawnFromPool(enemyTag, spawnPoint.position, Quaternion.identity);

        EnemyBase enemy = enemyObject.GetComponent<EnemyBase>(); // Enemy 스크립트를 가져옴
        if (enemy != null)
        {
            // 적이 소환될 때 OnEnemyDestroyed 이벤트를 연결
            enemy.OnEnemyDestroyed += OnEnemyDestroyed;
            Debug.Log("적 이벤트 연결 완료");
        }
        else
        {
            Debug.LogWarning("적에 Enemy 스크립트가 붙어 있지 않습니다.");
        }
    }

    // 랜덤으로 두 개의 스폰 포인트 선택
    List<int> GetRandomSpawnIndices(List<int> availableSpawnPoints)
    {
        int firstSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int first = availableSpawnPoints[firstSpawnIndex];
        availableSpawnPoints.RemoveAt(firstSpawnIndex);  // 첫 번째 선택한 포인트는 제거

        int secondSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int second = availableSpawnPoints[secondSpawnIndex];

        availableSpawnPoints.Add(first);  // 다시 리스트에 추가

        return new List<int> { first, second };
    }

    // 적 사망 시 호출되는 메서드
    void OnEnemyDestroyed()
    {
        enemiesRemaining--;
        Debug.Log("적 사망");
    }
}
