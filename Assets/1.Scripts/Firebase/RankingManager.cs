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

    // 랭킹 정보를 저장하는 함수
    public void SaveRanking(string roomName, int wave, float playTime)
    {
        string key = dbReference.Child("rankings").Push().Key;  // 새로운 랭킹 데이터를 위한 키 생성
        RankingData newRanking = new RankingData(roomName, wave, playTime);
        string json = JsonUtility.ToJson(newRanking);

        dbReference.Child("rankings").Child(key).SetRawJsonValueAsync(json);
    }

    // 랭킹 정보를 불러오는 함수 (최대 10개를 wave 기준으로 정렬)
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
                rankings.Reverse();  // wave 기준으로 내림차순 정렬
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
