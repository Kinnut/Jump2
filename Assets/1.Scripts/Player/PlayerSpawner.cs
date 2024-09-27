using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;  // 플레이어 캐릭터 프리팹

    void Start()
    {
        // 네트워크 상에서 각 플레이어가 자신의 캐릭터를 생성
        if (PhotonNetwork.IsConnected && playerPrefab != null)
        {
            // 랜덤한 위치에 플레이어 캐릭터 생성
            Vector3 spawnPosition = new Vector3(-4, 0, 4);

            // PhotonNetwork.Instantiate로 캐릭터를 생성하고 모든 클라이언트에 동기화
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        }
    }
}
