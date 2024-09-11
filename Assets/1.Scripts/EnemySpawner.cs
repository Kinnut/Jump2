using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public ObjectPooler objectPooler;     // Object Pooler ����
    public Transform player;              // �÷��̾� ��ġ
    public float spawnRadius = 15f;       // �÷��̾�κ��� ���� �Ÿ����� ���� ����
    public float spawnInterval = 5f;      // ���� ���� ���� (5��)
    public float gameDuration = 0f;       // ������ ����� �ð�
    public string[] enemyTags;            // ����� ���� �±� ���

    private float nextSpawnTime = 0f;

    void Update()
    {
        gameDuration += Time.deltaTime;  // ���� �ð��� �帧

        // 5�ʸ��� ���͸� ����
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;  // ���� ���� �ð� ����
        }
    }

    // 5�ʸ��� ���ο� ���͸� ���� (���� ���ʹ� ������� ����)
    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomPositionOutsideView();  // ȭ�� �ۿ��� ���� ��ġ�� ����
        string selectedTag = GetEnemyTagBasedOnDifficulty();  

        objectPooler.SpawnFromPool(selectedTag, spawnPosition, Quaternion.identity);
    }

    // ���� ȭ�� �ۿ��� �����ϰ� �����ǵ��� ��ġ ����
    Vector3 GetRandomPositionOutsideView()
    {
        Vector3 spawnDirection = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = player.position + spawnDirection;
        return spawnPosition;
    }

    // ���� �ð��� ��������� �� ���� ���͸� ��ȯ
    string GetEnemyTagBasedOnDifficulty()
    {
        if (gameDuration < 60f)
        {
            return enemyTags[0];
        }
        else if (gameDuration < 120f)
        {
            return enemyTags[1];
        }
        else if (gameDuration < 180f)
        {
            return enemyTags[2];
        }
        else
        {
            return enemyTags[3];
        }
    }
}
