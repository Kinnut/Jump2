using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;     // 4������ ���� ����
    public string normalEnemyTag;       // ObjectPooler���� ����� �⺻ �� �±�
    public string strongEnemyTag;       // ObjectPooler���� ����� ��ȭ�� �� �±�
    public DynamicObjectPooler objectPooler;  // DynamicObjectPooler ����

    public int startEnemiesPerWave = 10;   // ù ���̺꿡�� ��ȯ�� ���� ��
    public float spawnInterval = 2f;       // ���� �� ��ȯ ����
    public float waveWaitTime = 10f;       // ���� ���̺� ��� �ð�

    private int currentWave = 1;           // ���� ���̺� ��ȣ
    private int enemiesRemaining;          // �����ִ� �� ��
    private bool waveInProgress = false;   // ���̺갡 ���� ������ ����

    void Start()
    {
        if (objectPooler == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Object Pooler�� ������� �ʾҰų� ���� ����Ʈ�� �������� �ʾҽ��ϴ�.");
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

    // ���̺� ����
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

            // ù ��° ���� ����Ʈ���� �� ��ȯ
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

            // �� ��° ���� ����Ʈ���� �� ��ȯ
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

    // �� ��ȯ �޼��� (DynamicObjectPooler�� ����Ͽ� �� ��ȯ)
    void SpawnEnemy(string enemyTag, int spawnIndex)
    {
        if (objectPooler == null || spawnPoints == null || spawnIndex < 0 || spawnIndex >= spawnPoints.Length)
        {
            Debug.LogError("Object Pooler�� ������� �ʾҰų� �߸��� ���� �ε����Դϴ�.");
            return;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject enemyObject = objectPooler.SpawnFromPool(enemyTag, spawnPoint.position, Quaternion.identity);

        if (enemyObject == null)
        {
            Debug.LogError("�� ���� ����: " + enemyTag + " Ǯ���� ���� �������� ���߽��ϴ�.");
            return;
        }

        EnemyBase enemyBase = enemyObject.GetComponent<EnemyBase>();
        if (enemyBase == null)
        {
            Debug.LogError("EnemyBase ��ũ��Ʈ�� �����տ� �پ� ���� �ʽ��ϴ�. �±�: " + enemyTag);
            return;
        }

        enemyBase.OnEnemyDestroyed += OnEnemyDestroyed;
    }

    // �������� �� ���� ���� ����Ʈ ����
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

    // �� ��� �� ȣ��Ǵ� �޼���
    void OnEnemyDestroyed()
    {
        enemiesRemaining--;
    }
}
