using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WaveSpawner : MonoBehaviourPun
{
    public Transform[] spawnPoints;
    public string normalEnemyTag;
    public string strongEnemyTag;
    public DynamicObjectPooler objectPooler;

    public int startEnemiesPerWave = 10;
    public float spawnInterval = 2f;
    public float waveWaitTime = 10f;

    private int currentWave = 1;
    private int enemiesRemaining;
    private bool waveInProgress = false;

    public TextMeshProUGUI waveText;

    void Start()
    {
        if (objectPooler == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Object Pooler가 연결되지 않았거나 스폰 포인트가 설정되지 않았습니다.");
            return;
        }

        UpdateWaveText();
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (enemiesRemaining <= 0 && !waveInProgress)
        {
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        if (!PhotonNetwork.IsMasterClient) yield break; // 마스터 클라이언트만 스폰

        waveInProgress = true;

        int enemiesToSpawn = startEnemiesPerWave + (currentWave / 2) * 2;
        int strongEnemyCount = Mathf.CeilToInt(enemiesToSpawn * 0.1f);
        int normalEnemyCount = enemiesToSpawn - strongEnemyCount;

        enemiesRemaining = enemiesToSpawn;

        List<int> availableSpawnPoints = new List<int> { 0, 1, 2, 3 };

        while (normalEnemyCount > 0 || strongEnemyCount > 0)
        {
            List<int> spawnIndices = GetRandomSpawnIndices(availableSpawnPoints);

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
        UpdateWaveText();
        waveInProgress = false;
    }

    void SpawnEnemy(string enemyTag, int spawnIndex)
    {
        if (PhotonNetwork.IsMasterClient)  // 마스터 클라이언트만 적 스폰
        {
            Transform spawnPoint = spawnPoints[spawnIndex];

            // 적을 네트워크 상에서 스폰
            GameObject enemyObject = PhotonNetwork.Instantiate(enemyTag, spawnPoint.position, Quaternion.identity);

            if (enemyObject == null)
            {
                Debug.LogError("적 생성 실패: " + enemyTag);
                return;
            }

            EnemyBase enemyBase = enemyObject.GetComponent<EnemyBase>();

            if (enemyBase != null)
            {
                // 적 사망 시 호출될 이벤트 등록
                enemyBase.OnEnemyDestroyed += OnEnemyDestroyed;
            }
            else
            {
                Debug.LogError("EnemyBase 스크립트가 프리팹에 붙어 있지 않습니다. 태그: " + enemyTag);
            }
        }
    }

    void OnEnemyDestroyed()
    {
        enemiesRemaining--;  // 적이 죽을 때마다 남은 적 수 감소
        Debug.Log("적이 죽었습니다. 남은 적: " + enemiesRemaining);
    }


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

    void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + currentWave;
        }
    }
}
