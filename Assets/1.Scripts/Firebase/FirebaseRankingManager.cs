using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Collections.Generic;

public class FirebaseRankingManager : MonoBehaviour
{
    public TMP_Text[] rankingLines; // �� ����, �г���, ����, �÷��� Ÿ���� �� �ٷ� ǥ���� TextMeshPro �ʵ� �迭 (5��)

    private DatabaseReference databaseRef;
    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        LoadRanking();  // ���� ���� �� ��ŷ�� �ҷ���
    }

    // Firebase�� �÷��̾� ���� ������Ʈ
    public void UpdateRank(float score, float playTime)
    {
        string userId = auth.CurrentUser.UserId;
        string username = auth.CurrentUser.DisplayName;

        // Firebase���� ���� �����͸� �ҷ��ͼ� ��
        databaseRef.Child("ranking").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    // ���� ������ �ҷ��ͼ� �� ���� ������ ������Ʈ
                    PlayerData existingData = JsonUtility.FromJson<PlayerData>(snapshot.GetRawJsonValue());
                    if (score > existingData.score)
                    {
                        SavePlayerData(userId, username, score, playTime);
                    }
                }
                else
                {
                    // ó�� ������ ����ϴ� ���
                    SavePlayerData(userId, username, score, playTime);
                }
            }
            else
            {
                Debug.LogError("��ŷ ������Ʈ ����: " + task.Exception);
            }
        });
    }

    // Firebase�� �÷��̾� ������ ���� �Լ�
    private void SavePlayerData(string userId, string username, float score, float playTime)
    {
        PlayerData newData = new PlayerData(username, score, playTime);
        databaseRef.Child("ranking").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(newData))
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("��ŷ ������Ʈ ����");
                    LoadRanking();  // ������ ���� �� ��ŷ �ҷ�����
                }
                else
                {
                    Debug.LogError("��ŷ ���� ����: " + task.Exception);
                }
            });
    }

    // ���� 5���� ��ŷ �ҷ�����
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

                    // ���� ������������ ���� (���� ��������)
                    rankingList.Sort((x, y) => y.score.CompareTo(x.score));

                    // ��ŷ UI ������Ʈ
                    UpdateRankingUI(rankingList);
                }
                else
                {
                    Debug.LogError("��ŷ �ҷ����� ����: " + task.Exception);
                }
            });
    }

    private void UpdateRankingUI(List<PlayerData> rankingList)
    {
        for (int i = 0; i < rankingList.Count && i < 5; i++)
        {
            rankingLines[i].text = string.Format("{0}��   {1}   {2}   {3}",
                                                 (i + 1),                  // ����
                                                 rankingList[i].username,  // �г���
                                                 rankingList[i].score,     // ����
                                                 FormatPlaytime(rankingList[i].playTime)); // �÷��� Ÿ��
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
