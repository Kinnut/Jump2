using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class RankingManager : MonoBehaviour
{
    public static RankingManager instance;
    private DatabaseReference dbReference;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // ��ŷ ������ �����ϴ� �Լ�
    public void SaveRanking(string roomName, int wave, float playTime)
    {
        string key = dbReference.Child("rankings").Push().Key;  // ���ο� ��ŷ �����͸� ���� Ű ����
        RankingData newRanking = new RankingData(roomName, wave, playTime);
        string json = JsonUtility.ToJson(newRanking);

        dbReference.Child("rankings").Child(key).SetRawJsonValueAsync(json);
    }

    // ��ŷ ������ �ҷ����� �Լ� (�ִ� 10���� wave �������� ����)
    public void LoadRankings(System.Action<List<RankingData>> callback)
    {
        dbReference.Child("rankings").OrderByChild("wave").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                List<RankingData> rankings = new List<RankingData>();
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot rankSnapshot in snapshot.Children)
                {
                    string json = rankSnapshot.GetRawJsonValue();
                    RankingData rank = JsonUtility.FromJson<RankingData>(json);
                    rankings.Add(rank);
                }
                rankings.Reverse();  // wave �������� �������� ����
                callback(rankings);
            }
        });
    }
}

[System.Serializable]
public class RankingData
{
    public string roomName;
    public int wave;
    public float playTime;

    public RankingData(string roomName, int wave, float playTime)
    {
        this.roomName = roomName;
        this.wave = wave;
        this.playTime = playTime;
    }
}
