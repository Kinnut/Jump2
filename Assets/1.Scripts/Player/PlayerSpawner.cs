using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;

public class PlayerSpawner : MonoBehaviourPun
{
    public GameObject playerPrefab;  // 플레이어 캐릭터 프리팹

    // 궁극기 선택에 따른 색상 설정
    public Color iceColor;
    public Color fireColor;
    public Color healColor;

    void Start()
    {
        if (PhotonNetwork.IsConnected && playerPrefab != null)
        {
            // 랜덤한 위치에 플레이어 캐릭터 생성
            Vector3 spawnPosition = new Vector3(-4, 0, 4);
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

            // 플레이어가 제대로 생성되었는지 확인
            if (player == null)
            {
                Debug.LogError("Player instantiation failed!");
                return;
            }

            // PhotonView 확인 (플레이어 오브젝트의 PhotonView)
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView == null)
            {
                Debug.LogError("PhotonView not found on player prefab.");
                return;
            }

            // SpriteRenderer 확인 후 색상 적용
            SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer != null)
            {
                // CustomProperties에서 'SelectedUltimate' 키가 있는지 확인
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SelectedUltimate"))
                {
                    int selectedUltimate = (int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedUltimate"];
                    Color playerColor = GetColorByUltimate(selectedUltimate);

                    // 로컬에서 색상 적용
                    playerSpriteRenderer.color = playerColor;

                    // 플레이어 오브젝트의 PhotonView로 RPC 호출 (MyPlayer 스크립트의 SyncPlayerColor 메서드)
                    playerPhotonView.RPC("SyncPlayerColor", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer.ActorNumber, playerColor.r, playerColor.g, playerColor.b);
                }
                else
                {
                    Debug.LogError("SelectedUltimate not found in CustomProperties.");
                }
            }
            else
            {
                Debug.LogError("SpriteRenderer not found on player prefab.");
            }
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected or playerPrefab is null");
        }
    }

    // 궁극기에 따라 색상 선택
    private Color GetColorByUltimate(int ultimateIndex)
    {
        switch (ultimateIndex)
        {
            case 0: return iceColor;
            case 1: return fireColor;
            case 2: return healColor;
            default: return Color.white;
        }
    }
}
