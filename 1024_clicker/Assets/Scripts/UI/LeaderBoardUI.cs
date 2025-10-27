using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;

    [SerializeField] private GameObject leaderboardTextPrefab;
    [SerializeField] private Transform leaderboardContentParent;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button RefreshButton;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image synchronizedImage;

    List<GameObject> leaderboardItemList = new List<GameObject>();
    private async UniTaskVoid Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            gameUI.SetActive(true);
        });

        RefreshButton.onClick.AddListener(() => UpdateUI().Forget());

        for (int i = 0; i < 20; i++)
        {
            var obj = Instantiate(leaderboardTextPrefab, leaderboardContentParent);
            leaderboardItemList.Add(obj);
            obj.SetActive(false);
        }

    }

    private async UniTaskVoid OnEnable()
    {
        await UpdateUI();
    }

    private async UniTask UpdateUI()
    {
        synchronizedImage.color = Color.red;
        await UniTask.WaitUntil(() => LeaderBoardManager.Instance != null);

        var leaderboard = await LeaderBoardManager.Instance.LoadLeaderBoardAsync();
        countText.text = $"업데이트 됨: ({leaderboard.Count}명)";

        for (int i = 0; i < leaderboard.Count; i++)
        {
            var entry = leaderboard[i];

            var item = leaderboardItemList[i].GetComponent<LeaderboardItem>();
            leaderboardItemList[i].SetActive(true);
            item.SetData(i + 1, entry.nickname, entry.score);
        }
         synchronizedImage.color = Color.green;
    }

}
