using UnityEngine;
using Firebase.Database;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance => instance;

    private DatabaseReference scoreRef;

    private int cachedBestScore = 0;
    public int CachedBestScore => cachedBestScore;
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

    // 초기화
    private async UniTaskVoid Start()
    {
        await FirebaseInitializer.Instance.WaitForInitializationAsync();

        scoreRef = FirebaseDatabase.DefaultInstance.RootReference.Child("scores");

        Debug.Log("[Score] 초기화 완료");

        await LoadBestScoreAsync();
    }

    // 최고 점수 불러오기
    public async UniTask<int> LoadBestScoreAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return 0;
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            DataSnapshot snapshot = await scoreRef.Child(userId).Child("bestScore").GetValueAsync().AsUniTask();
            if (snapshot.Exists)
            {
                cachedBestScore = int.Parse(snapshot.Value.ToString());
                Debug.Log($"[Score] 최고 기록 로드: {cachedBestScore}");
            }
            else
            {
                cachedBestScore = 0;
                Debug.Log("[Score] 최고 기록 없음");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Score] 최고 기록 로드 실패: {ex.Message}");
        }

        return 0;
    }

    // 점수 저장
    public async UniTask<(bool success, string error)> SaveScoreAsync(int score)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "로그인이 필요합니다.");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Score] 점수 저장 시도: {score}");

            DatabaseReference historyRef = scoreRef.Child(userId).Child("history");
            DatabaseReference newHistoryRef = historyRef.Push();

            var scoreData = new Dictionary<string, object>();

            scoreData.Add("score", score);
            scoreData.Add("timestamp", ServerValue.Timestamp);

            await newHistoryRef.UpdateChildrenAsync(scoreData).AsUniTask();

            bool shouldUpdateBestScore = false;
            if (cachedBestScore == 0)
            {
                var bestScoreSnapshot = await scoreRef.Child(userId).Child("bestScore").GetValueAsync().AsUniTask();

                if (!bestScoreSnapshot.Exists)
                {
                    shouldUpdateBestScore = true;
                }
                // else if (score > cachedBestScore)
                // {
                //     shouldUpdateBestScore = true;
                // }
                else
                {
                    int existingBestScore = int.Parse(bestScoreSnapshot.Value.ToString());
                    cachedBestScore = existingBestScore;  
                    
                    if (score > cachedBestScore)  
                    {
                        shouldUpdateBestScore = true;
                    }
                }
            }
            else if (score > cachedBestScore)
            {
                shouldUpdateBestScore = true;
            }

            if (shouldUpdateBestScore)
            {
                await UpdateBestScoreAsync(score);
            }

            Debug.Log($"[Score] 점수 저장 성공: {score}");
            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Score] 점수 저장 실패! {ex.Message}");
            return (false, ex.Message);
        }
    }

    // 최고 점수 갱신
    private async UniTask UpdateBestScoreAsync(int newBestScore)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return;
        }
        string userId = AuthManager.Instance.UserId;

        try
        {
            await scoreRef.Child(userId).Child("bestScore").SetValueAsync(newBestScore).AsUniTask();
            cachedBestScore = newBestScore;

            // 리더보드도 갱신
            DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.RootReference.Child("leaderboard");
            var leaderboardData = new Dictionary<string, object>
            {
                { "score", newBestScore },
                { "nickname", ProfileManager.Instance.CachedProfile?.nickname ?? "없음" }
            };

            await leaderboardRef.Child(userId).UpdateChildrenAsync(leaderboardData).AsUniTask();

            Debug.Log($"[Score] 최고 기록 갱신 성공: {newBestScore}");
        }
        catch (System.Exception ex)
        {
            Debug.LogErrorFormat($"[Score] 최고 기록 갱신 실패: {0}", ex.Message);
        }
    }

    // 최근 점수 기록 불러오기
    public async UniTask<List<ScoreData>> LoadHistoryAsync(int limit = 10)
    {
        var list = new List<ScoreData>();
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return list;
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log("[Score] 히스토리 로드 시도");
            DatabaseReference historyRef = scoreRef.Child(userId).Child("history");
            Query query = historyRef.OrderByChild("timestamp").LimitToLast(limit);

            DataSnapshot snapshot = await query.GetValueAsync().AsUniTask();
            if (snapshot.Exists)
            {
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    ScoreData data = ScoreData.FromJson(json);
                    list.Add(data);
                }
            }

            Debug.Log($"[Score] 히스토리 로드 성공: {list.Count}개");
            return list;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Score] 히스토리 로드 실패: {ex.Message}");
        }
        return list;
    }
}
