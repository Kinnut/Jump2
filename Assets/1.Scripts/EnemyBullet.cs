using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f; // 총알 데미지
    public float lifeTime = 5f; // 총알이 사라지기까지의 시간

    void Start()
    {
        Destroy(gameObject, lifeTime); // 총알은 일정 시간이 지나면 사라짐
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Player player = hitInfo.GetComponent<Player>(); // 플레이어에 맞으면
        if (player != null)
        {
            player.TakeDamage(damage); // 플레이어에게 데미지를 줌
            Destroy(gameObject); // 총알을 파괴
        }
    }
}
