using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public ObjectPooler objectPooler;     // Object Pooler 참조
    public Transform player;              // 플레이어 위치
    public float spawnRadius = 15f;       // 플레이어로부터 일정 거리에서 몬스터 생성
    public float spawnInterval = 5f;      // 몬스터 생성 간격 (5초)
    public float gameDuration = 0f;       // 게임이 진행된 시간
    public string[] enemyTags;            // 사용할 몬스터 태그 목록

    private float nextSpawnTime = 0f;

    void Update()
    {
        gameDuration += Time.deltaTime;  // 게임 시간이 흐름

        // 5초마다 몬스터를 스폰
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;  // 다음 스폰 시간 설정
        }
    }

    // 5초마다 새로운 몬스터를 스폰 (기존 몬스터는 사라지지 않음)
    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomPositionOutsideView();  // 화면 밖에서 랜덤 위치로 스폰
        string selectedTag = GetEnemyTagBasedOnDifficulty();  

        objectPooler.SpawnFromPool(selectedTag, spawnPosition, Quaternion.identity);
    }

    // 적이 화면 밖에서 랜덤하게 생성되도록 위치 설정
    Vector3 GetRandomPositionOutsideView()
    {
        Vector3 spawnDirection = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = player.position + spawnDirection;
        return spawnPosition;
    }

    // 게임 시간이 길어질수록 더 강한 몬스터를 반환
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
