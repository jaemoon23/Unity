using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    public void SetData(int rank, string nickname, int bestScore)
    {
        rankText.text = rank.ToString();
        nicknameText.text = nickname;
        bestScoreText.text = $"점수: {bestScore}점";
    }
}
