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
    public TextMeshProUGUI waveCountdownText;  // 웨이브 대기시간을 표시할 텍스트

    void Start()
    {
        if (objectPooler == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Object Pooler가 연결되지 않았거나 스폰 포인트가 설정되지 않았습니다.");
            return;
        }

        photonView.RPC("UpdateWaveText", RpcTarget.All, currentWave);
        waveCountdownText.gameObject.SetActive(false);  // 대기 시간 텍스트 숨김
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

        // 웨이브 대기 시간 표시 시작
        waveCountdownText.gameObject.SetActive(true);

        float countdown = waveWaitTime;
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            photonView.RPC("UpdateWaveCountdownText", RpcTarget.All, Mathf.Ceil(countdown)); // 남은 시간 동기화
            yield return null;
        }

        // 대기 시간 끝나면 텍스트 숨김
        photonView.RPC("HideWaveCountdownText", RpcTarget.All); // 모든 클라이언트에서 텍스트 숨김

        int enemiesToSpawn = startEnemiesPerWave + (currentWave / 2) * 2;
        int strongEnemyCount = Mathf.CeilToInt(enemiesToSpawn * 0.1f);
        int normalEnemyCount = enemiesToSpawn - strongEnemyCount;

        enemiesRemaining = enemiesToSpawn;
        photonView.RPC("SyncEnemyCount", RpcTarget.All, enemiesRemaining);  // 모든 클라이언트에 적 수 동기화

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
        photonView.RPC("UpdateWaveText", RpcTarget.All, currentWave); // 모든 클라이언트에게 웨이브 정보 동기화
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

    public void OnEnemyDestroyed()
    {
        enemiesRemaining--;  // 적이 죽을 때마다 남은 적 수 감소
        Debug.Log("적이 죽었습니다. 남은 적: " + enemiesRemaining);
        photonView.RPC("SyncEnemyCount", RpcTarget.All, enemiesRemaining); // 적 수 동기화
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

    [PunRPC] // RPC로 함수 선언
    void UpdateWaveText(int wave)
    {
        currentWave = wave;
        if (waveText != null)
        {
            waveText.text = "Wave " + currentWave;
        }
    }

    [PunRPC]
    void UpdateWaveCountdownText(float countdown)
    {
        if (waveCountdownText != null)
        {
            waveCountdownText.text = "다음 웨이브까지 : " + countdown + "초";
        }
    }

    [PunRPC]
    void HideWaveCountdownText()
    {
        waveCountdownText.gameObject.SetActive(false);
    }

    [PunRPC]
    void SyncEnemyCount(int remainingEnemies)
    {
        enemiesRemaining = remainingEnemies;  // 모든 클라이언트에서 적 수 동기화
        Debug.Log("적 수 동기화");
    }
}
