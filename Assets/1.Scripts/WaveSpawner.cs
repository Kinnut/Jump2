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
        StartCoroutine(StartNextWave());
        Debug.Log("���ӽ���");
    }

    void Update()
    {
        // ���� ���� ���� ���̺갡 ���� ���� �ƴ� ���¿��� ���� ���̺� ����
        if (enemiesRemaining <= 0 && !waveInProgress)
        {
            StartCoroutine(StartNextWave());
        }
    }

    // ���̺� ����
    IEnumerator StartNextWave()
    {
        Debug.Log("���̺����");
        waveInProgress = true;

        int enemiesToSpawn = startEnemiesPerWave + (currentWave / 2) * 2;  // 2���̺긶�� 2���� �߰�
        int strongEnemyCount = Mathf.CeilToInt(enemiesToSpawn * 0.1f);     // 20%�� ��ȭ�� ����
        int normalEnemyCount = enemiesToSpawn - strongEnemyCount;

        enemiesRemaining = enemiesToSpawn;
        List<int> availableSpawnPoints = new List<int> { 0, 1, 2, 3 };  // ���� ���� ����Ʈ

        while (normalEnemyCount > 0 || strongEnemyCount > 0)
        {
            Debug.Log("while�� ����");
            // �� ���� ���� ���� ����Ʈ ���� (���ÿ� �� ���� ��ȯ X)
            List<int> spawnIndices = GetRandomSpawnIndices(availableSpawnPoints);
            Debug.Log("����1");
            // ù ��° ���� ����Ʈ�� ���� ��ȯ
            if (normalEnemyCount > 0)
            {
                SpawnEnemy(normalEnemyTag, spawnIndices[0]);  // Ǯ���� �⺻ ���� ��ȯ
                normalEnemyCount--;
            }
            else if (strongEnemyCount > 0)
            {
                SpawnEnemy(strongEnemyTag, spawnIndices[0]);  // Ǯ���� ��ȭ ���� ��ȯ
                strongEnemyCount--;
            }

            // �� ��° ���� ����Ʈ�� ���� ��ȯ
            if (normalEnemyCount > 0)
            {
                SpawnEnemy(normalEnemyTag, spawnIndices[1]);  // Ǯ���� �⺻ ���� ��ȯ
                normalEnemyCount--;
            }
            else if (strongEnemyCount > 0)
            {
                SpawnEnemy(strongEnemyTag, spawnIndices[1]);  // Ǯ���� ��ȭ ���� ��ȯ
                strongEnemyCount--;
            }

            yield return new WaitForSeconds(spawnInterval);  // ��ȯ ���� ���
        }

        // ���Ͱ� ��� ��ȯ�� ��, ���� óġ�� ������ ���
        while (enemiesRemaining > 0)
        {
            yield return null;  // ��� ���
        }

        // ���� ���̺� ���� �� ��� �ð�
        yield return new WaitForSeconds(waveWaitTime);

        currentWave++;  // ���̺� ����
        waveInProgress = false;  // ���� ���̺� ���� ���� ����
    }

    // �� ��ȯ �޼��� (DynamicObjectPooler�� ����Ͽ� �� ��ȯ)
    void SpawnEnemy(string enemyTag, int spawnIndex)
    {
        Debug.Log("����2");
        Transform spawnPoint = spawnPoints[spawnIndex];

        // ObjectPooler���� �� ��ȯ
        GameObject enemyObject = objectPooler.SpawnFromPool(enemyTag, spawnPoint.position, Quaternion.identity);

        EnemyBase enemy = enemyObject.GetComponent<EnemyBase>(); // Enemy ��ũ��Ʈ�� ������
        if (enemy != null)
        {
            // ���� ��ȯ�� �� OnEnemyDestroyed �̺�Ʈ�� ����
            enemy.OnEnemyDestroyed += OnEnemyDestroyed;
            Debug.Log("�� �̺�Ʈ ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("���� Enemy ��ũ��Ʈ�� �پ� ���� �ʽ��ϴ�.");
        }
    }

    // �������� �� ���� ���� ����Ʈ ����
    List<int> GetRandomSpawnIndices(List<int> availableSpawnPoints)
    {
        int firstSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int first = availableSpawnPoints[firstSpawnIndex];
        availableSpawnPoints.RemoveAt(firstSpawnIndex);  // ù ��° ������ ����Ʈ�� ����

        int secondSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
        int second = availableSpawnPoints[secondSpawnIndex];

        availableSpawnPoints.Add(first);  // �ٽ� ����Ʈ�� �߰�

        return new List<int> { first, second };
    }

    // �� ��� �� ȣ��Ǵ� �޼���
    void OnEnemyDestroyed()
    {
        enemiesRemaining--;
        Debug.Log("�� ���");
    }
}
