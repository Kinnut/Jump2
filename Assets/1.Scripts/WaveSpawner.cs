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
    public TextMeshProUGUI waveCountdownText;  // ���̺� ���ð��� ǥ���� �ؽ�Ʈ

    void Start()
    {
        if (objectPooler == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Object Pooler�� ������� �ʾҰų� ���� ����Ʈ�� �������� �ʾҽ��ϴ�.");
            return;
        }

        photonView.RPC("UpdateWaveText", RpcTarget.All, currentWave);
        waveCountdownText.gameObject.SetActive(false);  // ��� �ð� �ؽ�Ʈ ����
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
        if (!PhotonNetwork.IsMasterClient) yield break; // ������ Ŭ���̾�Ʈ�� ����

        waveInProgress = true;

        // ���̺� ��� �ð� ǥ�� ����
        waveCountdownText.gameObject.SetActive(true);

        float countdown = waveWaitTime;
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            photonView.RPC("UpdateWaveCountdownText", RpcTarget.All, Mathf.Ceil(countdown)); // ���� �ð� ����ȭ
            yield return null;
        }

        // ��� �ð� ������ �ؽ�Ʈ ����
        photonView.RPC("HideWaveCountdownText", RpcTarget.All); // ��� Ŭ���̾�Ʈ���� �ؽ�Ʈ ����

        int enemiesToSpawn = startEnemiesPerWave + (currentWave / 2) * 2;
        int strongEnemyCount = Mathf.CeilToInt(enemiesToSpawn * 0.1f);
        int normalEnemyCount = enemiesToSpawn - strongEnemyCount;

        enemiesRemaining = enemiesToSpawn;
        photonView.RPC("SyncEnemyCount", RpcTarget.All, enemiesRemaining);  // ��� Ŭ���̾�Ʈ�� �� �� ����ȭ

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
        photonView.RPC("UpdateWaveText", RpcTarget.All, currentWave); // ��� Ŭ���̾�Ʈ���� ���̺� ���� ����ȭ
        waveInProgress = false;
    }

    void SpawnEnemy(string enemyTag, int spawnIndex)
    {
        if (PhotonNetwork.IsMasterClient)  // ������ Ŭ���̾�Ʈ�� �� ����
        {
            Transform spawnPoint = spawnPoints[spawnIndex];

            // ���� ��Ʈ��ũ �󿡼� ����
            GameObject enemyObject = PhotonNetwork.Instantiate(enemyTag, spawnPoint.position, Quaternion.identity);

            if (enemyObject == null)
            {
                Debug.LogError("�� ���� ����: " + enemyTag);
                return;
            }

            EnemyBase enemyBase = enemyObject.GetComponent<EnemyBase>();

            if (enemyBase != null)
            {
                // �� ��� �� ȣ��� �̺�Ʈ ���
                enemyBase.OnEnemyDestroyed += OnEnemyDestroyed;
            }
            else
            {
                Debug.LogError("EnemyBase ��ũ��Ʈ�� �����տ� �پ� ���� �ʽ��ϴ�. �±�: " + enemyTag);
            }
        }
    }

    public void OnEnemyDestroyed()
    {
        enemiesRemaining--;  // ���� ���� ������ ���� �� �� ����
        Debug.Log("���� �׾����ϴ�. ���� ��: " + enemiesRemaining);
        photonView.RPC("SyncEnemyCount", RpcTarget.All, enemiesRemaining); // �� �� ����ȭ
    }

    List<int> GetRandomSpawnIndices(List<int> availableSpawnPoints)
    {
        int firstSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int first = availableSpawnPoints[firstSpawnIndex];
        availableSpawnPoints.RemoveAt(firstSpawnIndex);

        int secondSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int second = availableSpawnPoints[secondSpawnIndex];

        availableSpawnPoints.Add(first); // �ٽ� ����Ʈ�� �߰�

        return new List<int> { first, second };
    }

    [PunRPC] // RPC�� �Լ� ����
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
            waveCountdownText.text = "���� ���̺���� : " + countdown + "��";
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
        enemiesRemaining = remainingEnemies;  // ��� Ŭ���̾�Ʈ���� �� �� ����ȭ
        Debug.Log("�� �� ����ȭ");
    }
}
