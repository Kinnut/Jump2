using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Collections.Generic;

public class FirebaseRankingManager : MonoBehaviour
{
    public TMP_Text[] rankingLines; // 각 순위, 닉네임, 점수, 플레이 타임을 한 줄로 표시할 TextMeshPro 필드 배열 (5개)

    private DatabaseReference databaseRef;
    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        LoadRanking();  // 게임 시작 시 랭킹을 불러옴
    }

    // Firebase에 플레이어 점수 업데이트
    public void UpdateRank(float score, float playTime)
    {
        string userId = auth.CurrentUser.UserId;
        string username = auth.CurrentUser.DisplayName;

        // Firebase에서 기존 데이터를 불러와서 비교
        databaseRef.Child("ranking").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    // 기존 점수를 불러와서 더 높은 점수만 업데이트
                    PlayerData existingData = JsonUtility.FromJson<PlayerData>(snapshot.GetRawJsonValue());
                    if (score > existingData.score)
                    {
                        SavePlayerData(userId, username, score, playTime);
                    }
                }
                else
                {
                    // 처음 점수를 등록하는 경우
                    SavePlayerData(userId, username, score, playTime);
                }
            }
            else
            {
                Debug.LogError("랭킹 업데이트 실패: " + task.Exception);
            }
        });
    }

    // Firebase에 플레이어 데이터 저장 함수
    private void SavePlayerData(string userId, string username, float score, float playTime)
    {
        PlayerData newData = new PlayerData(username, score, playTime);
        databaseRef.Child("ranking").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(newData))
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("랭킹 업데이트 성공");
                    LoadRanking();  // 데이터 저장 후 랭킹 불러오기
                }
                else
                {
                    Debug.LogError("랭킹 저장 실패: " + task.Exception);
                }
            });
    }

    // 상위 5명의 랭킹 불러오기
    public void LoadRanking()
    {
        databaseRef.Child("ranking").OrderByChild("score").LimitToLast(5).GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<PlayerData> rankingList = new List<PlayerData>();

                    foreach (DataSnapshot data in snapshot.Children)
                    {
                        PlayerData playerData = JsonUtility.FromJson<PlayerData>(data.GetRawJsonValue());
                        rankingList.Add(playerData);
                    }

                    // 점수 내림차순으로 정렬 (높은 점수부터)
                    rankingList.Sort((x, y) => y.score.CompareTo(x.score));

                    // 랭킹 UI 업데이트
                    UpdateRankingUI(rankingList);
                }
                else
                {
                    Debug.LogError("랭킹 불러오기 실패: " + task.Exception);
                }
            });
    }

    private void UpdateRankingUI(List<PlayerData> rankingList)
    {
        for (int i = 0; i < rankingList.Count && i < 5; i++)
        {
            rankingLines[i].text = string.Format("{0}위   {1}   {2}   {3}",
                                                 (i + 1),                  // 순위
                                                 rankingList[i].username,  // 닉네임
                                                 rankingList[i].score,     // 점수
                                                 FormatPlaytime(rankingList[i].playTime)); // 플레이 타임
        }
    }

    private string FormatPlaytime(float playTimeInSeconds)
    {
        int minutes = Mathf.FloorToInt(playTimeInSeconds / 60);
        int seconds = Mathf.FloorToInt(playTimeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

[System.Serializable]
public class PlayerData
{
    public string username;
    public float score;
    public float playTime;

    public PlayerData(string username, float score, float playTime)
    {
        this.username = username;
        this.score = score;
        this.playTime = playTime;
    }
}
