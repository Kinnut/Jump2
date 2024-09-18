using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public DynamicObjectPooler objectPooler; 
    public Transform player;          
    public float spawnRadius = 15f;  
    public float spawnInterval = 5f;    
    public float gameDuration = 0f;       
    public string[] enemyTags;            

    private float nextSpawnTime = 0f;

    void Update()
    {
        gameDuration += Time.deltaTime;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomPositionOutsideView();
        string selectedTag = GetEnemyTagBasedOnDifficulty();  

        objectPooler.SpawnFromPool(selectedTag, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomPositionOutsideView()
    {
        Vector3 spawnDirection = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = player.position + spawnDirection;
        return spawnPosition;
    }

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
