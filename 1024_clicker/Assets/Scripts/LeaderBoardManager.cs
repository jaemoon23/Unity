using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class LeaderBoardEntry
{
    public string nickname;
    public int score;
}

public class LeaderBoardManager : MonoBehaviour
{
    private static LeaderBoardManager instance;
    public static LeaderBoardManager Instance => instance;

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

    // 리더보드 불러오기
    public async UniTask<List<LeaderBoardEntry>> LoadLeaderBoardAsync()
    {
        await FirebaseInitializer.Instance.WaitForInitializationAsync();

        var leaderBoard = new List<LeaderBoardEntry>();

        try
        {
            Debug.Log("[LeaderBoard] 리더보드 로드 시도");

            DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.RootReference.Child("leaderboard");
            DataSnapshot snapshot = await leaderboardRef.GetValueAsync().AsUniTask();

            if (!snapshot.Exists)
            {
                Debug.Log("[LeaderBoard] 리더보드 데이터 없음");
                return leaderBoard;
            }

            foreach (DataSnapshot child in snapshot.Children)
            {
                var scoreChild = child.Child("score");
                var nicknameChild = child.Child("nickname");

                if (scoreChild.Exists && nicknameChild.Exists)
                {
                    int score = int.Parse(scoreChild.Value.ToString());
                    string nickname = nicknameChild.Value.ToString();

                    leaderBoard.Add(new LeaderBoardEntry
                    {
                        nickname = nickname,
                        score = score
                    });
                }
            }

            // 점수 높은 순으로 정렬
            leaderBoard.Sort((a, b) => b.score.CompareTo(a.score));

            Debug.Log($"[LeaderBoard] 리더보드 로드 성공: {leaderBoard.Count}명");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[LeaderBoard] 리더보드 로드 실패: {ex.Message}");
        }

        return leaderBoard;
    }
}
